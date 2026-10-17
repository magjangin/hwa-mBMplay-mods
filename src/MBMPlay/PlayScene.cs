extern alias MistAudioAlias;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BeMusic;
using MBMPlay.Common;
using MBMPlay.Platform;
using MistAudioAlias::MistAudio;
using MistGraphics;
using MistGraphics.BitmapImage;
using MistUtils;
using Randomize;
using UnmanagedCollections;

namespace MBMPlay;

public sealed class PlayScene : Scene
{
	private static readonly ReadOnlyCollection<string> SoundExtension = new List<string> { ".wav", ".ogg", ".flac", ".mp3" }.AsReadOnly();

	private static readonly ReadOnlyCollection<string> ImageExtension = new List<string> { ".bmp", ".png", ".jpg", ".jpeg" }.AsReadOnly();

	private static readonly ReadOnlyCollection<string> MovieExtension = new List<string> { ".mpg", ".mpeg", ".avi", ".wmv", ".mp4" }.AsReadOnly();

	private static readonly ReadOnlyCollection<string> FallbackBgaMovieNames = new List<string> { "video.mp4", "bga.mp4", "movie.mp4", "bg.mp4", "bga.wmv", "video.wmv" }.AsReadOnly();

	private const string FallbackBgaKey = "0BGA";

	private static readonly ReadOnlyCollection<PlayingKeys> KeyList = new List<PlayingKeys>
	{
		PlayingKeys.Player1ScratchToggle,
		PlayingKeys.Player1Key1,
		PlayingKeys.Player1Key2,
		PlayingKeys.Player1Key3,
		PlayingKeys.Player1Key4,
		PlayingKeys.Player1Key5,
		PlayingKeys.Player1Key6,
		PlayingKeys.Player1Key7,
		PlayingKeys.Player1Key8,
		PlayingKeys.Player1Key9,
		PlayingKeys.Player2ScratchToggle,
		PlayingKeys.Player2Key1,
		PlayingKeys.Player2Key2,
		PlayingKeys.Player2Key3,
		PlayingKeys.Player2Key4,
		PlayingKeys.Player2Key5,
		PlayingKeys.Player2Key6,
		PlayingKeys.Player2Key7,
		PlayingKeys.Player2Key8,
		PlayingKeys.Player2Key9
	}.AsReadOnly();

	private static readonly string KeyNameChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

	private static readonly string KeyNameChars62 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

	private static readonly ReadOnlyDictionary<JudgeRank, JudgeRange> JudgeRangeList = Extensions.AsReadOnly(new Dictionary<JudgeRank, JudgeRange>
	{
		{
			JudgeRank.Easy,
			new JudgeRange
			{
				PGreat = 21.0,
				Great = 60.0,
				Good = 120.0,
				Bad = 200.0,
				FastPoor = 500.0,
				SlowPoor = -200.0,
				ScratchRangeScale = 1.2,
				LNEndRangeScale = 1.5
			}
		},
		{
			JudgeRank.Normal,
			new JudgeRange
			{
				PGreat = 18.0,
				Great = 40.0,
				Good = 100.0,
				Bad = 200.0,
				FastPoor = 500.0,
				SlowPoor = -200.0,
				ScratchRangeScale = 1.2,
				LNEndRangeScale = 1.5
			}
		},
		{
			JudgeRank.Hard,
			new JudgeRange
			{
				PGreat = 15.0,
				Great = 30.0,
				Good = 60.0,
				Bad = 200.0,
				FastPoor = 500.0,
				SlowPoor = -200.0,
				ScratchRangeScale = 1.1,
				LNEndRangeScale = 1.3
			}
		},
		{
			JudgeRank.VeryHard,
			new JudgeRange
			{
				PGreat = 8.0,
				Great = 24.0,
				Good = 40.0,
				Bad = 200.0,
				FastPoor = 500.0,
				SlowPoor = -200.0,
				ScratchRangeScale = 1.1,
				LNEndRangeScale = 1.2
			}
		}
	});

	private readonly ReadOnlyDictionary<string, Func<IList<string>, Action<double>>> ButtonCommandDictionary;

	private readonly ReadOnlyDictionary<string, ValueCommand<double>> TrackBarCommandDictionary;

	private bool _isStartBarProcessed;

	private Skin _skin;

	private Dictionary<string, TextureInfo> _skinTextures = new Dictionary<string, TextureInfo>();

	private TextureInfo _bgaTexture;

	private TextureInfo _layerTexture;

	private TextureInfo _poorTexture;

	private Dictionary<string, Font> _fonts = new Dictionary<string, Font>();

	private UnmanagedArray<byte> _tempBgaData;

	private List<string> _dropFileList = new List<string>();

	private Bms _bmsData;

	private BmsTimeTable _originalTimeTable;

	private BmsTimeTable _baseTimeTable;

	private BmsTimeTable _timeTable;

	private Dictionary<string, AudioSource> _keySounds;

	private Dictionary<string, BitmapImage> _bgaBmps;

	private Dictionary<string, IMovie> _movies;

	private List<BarStartInfo> _barInfo;

	private Dictionary<JudgeType, double> _gaugeIncrements = new Dictionary<JudgeType, double>
	{
		{
			JudgeType.PGreat,
			0.0
		},
		{
			JudgeType.Great,
			0.0
		},
		{
			JudgeType.Good,
			0.0
		},
		{
			JudgeType.Bad,
			-4.0
		},
		{
			JudgeType.PassedPoor,
			-6.0
		},
		{
			JudgeType.WastedPoor,
			-2.0
		}
	};

	private bool _isBmsReady;

	private bool _isSkinReady;

	private bool _isWavReady;

	private bool _isBmpReady;

	private int _bmpLoadCount;

	private int _maxBgaWidth = 1;

	private int _maxBgaHeight = 1;

	private string _nowBga = "";

	private string _nowLayer = "";

	private string _nowPoor = "";

	private string _prevBga = "";

	private string _prevLayer = "";

	private string _prevPoor = "";

	private bool _hasPoorBga;

	private bool _startLNHoldingProcessed;

	private List<int> _startLNHolding = new List<int>();

	private double _reverseTime;

	private int _reverseBar = -1;

	private List<NotePositionLaneCollection> _notePos = new List<NotePositionLaneCollection>();

	private List<BarlinePositionCollection> _barlinePos1 = new List<BarlinePositionCollection>();

	private List<BarlinePositionCollection> _barlinePos2 = new List<BarlinePositionCollection>();

	private double _totalMusicTime;

	private int _prevMusicSpeedLevel;

	private int _lastNoteIndex;

	private int _lastNoteCount;

	private int _lastIndex;

	private int _lastCount;

	private bool _isBarlineCountAdded;

	private bool _isBarlineReached;

	private List<string> _lastKeySounds = new List<string>();

	private List<int> _laneIndex = new List<int>();

	private List<JudgeType> _lnJudge = new List<JudgeType>();

	private List<double> _lnJudgeTime = new List<double>();

	private JudgeRange _judgeRange;

	private readonly object _loadWavLockObj = new object();

	private readonly object _loadBgaLockObj = new object();

	private Dictionary<string, KeyRepeatManager> _keyRepeatManagers = new Dictionary<string, KeyRepeatManager>();

	private bool _isContentLoaded;

	public int StartBar { get; set; }

	public string BmsPath { get; set; }

	private ReadOnlyDictionary<string, Func<IList<string>, Action<double>>> CreateButtonCommandDictionary()
	{
		return Extensions.AsReadOnly(new Dictionary<string, Func<IList<string>, Action<double>>>
		{
			{
				"music_speed_up",
				(IList<string> args) =>
				{
					List<int> list = ButtonCommands.ParseIntArgs(args);
					int i = ((list == null || list.Count < 1) ? 1 : list[0]);
					return (double _) =>
					{
						ChangeMusicSpeed(Numbers.MusicSpeedLevel + i);
					};
				}
			},
			{
				"music_speed_down",
				(IList<string> args) =>
				{
					List<int> list = ButtonCommands.ParseIntArgs(args);
					int i = ((list != null && list.Count >= 1) ? (list[0] * -1) : (-1));
					return (double _) =>
					{
						ChangeMusicSpeed(Numbers.MusicSpeedLevel + i);
					};
				}
			}
		});
	}

	private ReadOnlyDictionary<string, ValueCommand<double>> CreateTrackBarCommandDictionary()
	{
		return Extensions.AsReadOnly(new Dictionary<string, ValueCommand<double>>
		{
			{
				"music_time_position",
				new ValueCommand<double>(() => Numbers.MusicTimePosition, (double value) =>
				{
					double num = Numbers.TotalMusicTime.TotalMilliseconds * (value / 100.0);
					if (num < Numbers.ElapsedMusicTime.TotalMilliseconds)
					{
						if (num <= 0.0 || Numbers.CurrentBars < 1)
						{
							StartBar = 0;
						}
						else
						{
							StartBar = Numbers.CurrentBars;
							for (int num2 = Numbers.CurrentBars; num2 >= 0; num2--)
							{
								if (_baseTimeTable.Rows[_baseTimeTable.BarIndex[num2]].Time <= num)
								{
									StartBar = num2;
									break;
								}
								if (num2 == 0)
								{
									StartBar = 0;
								}
							}
						}
					}
					else
					{
						int num3 = ((_reverseBar >= 0) ? _reverseBar : Numbers.TotalBars);
						if (num >= Numbers.TotalMusicTime.TotalMilliseconds || Numbers.CurrentBars >= num3)
						{
							StartBar = num3;
						}
						else
						{
							StartBar = Numbers.CurrentBars;
							for (int i = Numbers.CurrentBars; i <= num3; i++)
							{
								if (_baseTimeTable.Rows[_baseTimeTable.BarIndex[i]].Time >= num)
								{
									StartBar = ((i >= 0) ? (i - 1) : 0);
									break;
								}
								if (i == num3)
								{
									StartBar = num3;
								}
							}
						}
					}
					Restart();
				})
			},
			{
				"music_speed_level",
				new ValueCommand<double>(() => Numbers.MusicSpeedLevel, (double value) =>
				{
					ChangeMusicSpeed((int)value);
				})
			}
		});
	}

	public PlayScene(GraphicsBase graphics, GraphicsWindow window, AudioInstance audio)
		: base(graphics, window, audio)
	{
		ButtonCommandDictionary = CreateButtonCommandDictionary();
		TrackBarCommandDictionary = CreateTrackBarCommandDictionary();
	}

	public override Action<double> GetButtonCommandAction(string command)
	{
		if (ButtonCommands.ParseCommand(command, out var name, out var args) && ButtonCommandDictionary.ContainsKey(name))
		{
			return ButtonCommandDictionary[name]?.Invoke(args) ?? null;
		}
		return null;
	}

	public override ValueCommand<double> GetTrackBarCommand(string command)
	{
		if (TrackBarCommandDictionary.ContainsKey(command))
		{
			return TrackBarCommandDictionary[command];
		}
		return null;
	}

	public override void LoadContent()
	{
		IsActive = true;
		Flags.AutoPlay = Program.InitialAutoplayState;
		Numbers.MasterVolume = Program.Settings.MasterVolume;
		Numbers.KeyVolume = Program.Settings.KeyVolume;
		Numbers.BgmVolume = Program.Settings.BgmVolume;
		Numbers.Player1HiSpeed = Program.Settings.HiSpeed;
		Numbers.Player2HiSpeed = Program.Settings.HiSpeed;
		Flags.HiSpeedFixType = Program.Settings.HiSpeedFixType;
		Numbers.Player1LaneCover = (int)(Program.Settings.LaneCoverRange * 100m);
		Numbers.Player2LaneCover = (int)(Program.Settings.LaneCoverRange * 100m);
		Flags.Player1LaneCoverEnabled = Program.Settings.Player1LaneCoverEnabled;
		Flags.Player2LaneCoverEnabled = Program.Settings.Player2LaneCoverEnabled;
		Numbers.Player1Lift = (int)(Program.Settings.JudgeLineLift * 100m);
		Numbers.Player2Lift = (int)(Program.Settings.JudgeLineLift * 100m);
		Numbers.GaugeBorder = 80.0;
		Window.FileDrop += OnFileDrop;
		Flags.LegacyLN = Program.Settings.LegacyLNMode;
		_keyRepeatManagers.Clear();
		_keyRepeatManagers.Add("BackBar", new KeyRepeatManager(Program.KeyConfig.PlayScene.BackBar));
		_keyRepeatManagers.Add("BackBar4", new KeyRepeatManager(Program.KeyConfig.PlayScene.BackBar4));
		_keyRepeatManagers.Add("BackBarTime", new KeyRepeatManager(Program.KeyConfig.PlayScene.BackBarTime));
		_keyRepeatManagers.Add("ForwardBar", new KeyRepeatManager(Program.KeyConfig.PlayScene.ForwardBar));
		_keyRepeatManagers.Add("ForwardBar4", new KeyRepeatManager(Program.KeyConfig.PlayScene.ForwardBar4));
		_keyRepeatManagers.Add("ForwardBarTime", new KeyRepeatManager(Program.KeyConfig.PlayScene.ForwardBarTime));
		_keyRepeatManagers.Add("IncreaseMusicSpeed", new KeyRepeatManager(Program.KeyConfig.PlayScene.IncreaseMusicSpeed));
		_keyRepeatManagers.Add("DecreaseMusicSpeed", new KeyRepeatManager(Program.KeyConfig.PlayScene.DecreaseMusicSpeed));
		_keyRepeatManagers.Add("MasterVolumeUp", new KeyRepeatManager(Program.KeyConfig.PlayScene.MasterVolumeUp));
		_keyRepeatManagers.Add("MasterVolumeUp1", new KeyRepeatManager(Program.KeyConfig.PlayScene.MasterVolumeUp1));
		_keyRepeatManagers.Add("MasterVolumeUp10", new KeyRepeatManager(Program.KeyConfig.PlayScene.MasterVolumeUp10));
		_keyRepeatManagers.Add("MasterVolumeDown", new KeyRepeatManager(Program.KeyConfig.PlayScene.MasterVolumeDown));
		_keyRepeatManagers.Add("MasterVolumeDown1", new KeyRepeatManager(Program.KeyConfig.PlayScene.MasterVolumeDown1));
		_keyRepeatManagers.Add("MasterVolumeDown10", new KeyRepeatManager(Program.KeyConfig.PlayScene.MasterVolumeDown10));
		_keyRepeatManagers.Add("KeyVolumeUp", new KeyRepeatManager(Program.KeyConfig.PlayScene.KeyVolumeUp));
		_keyRepeatManagers.Add("KeyVolumeUp1", new KeyRepeatManager(Program.KeyConfig.PlayScene.KeyVolumeUp1));
		_keyRepeatManagers.Add("KeyVolumeUp10", new KeyRepeatManager(Program.KeyConfig.PlayScene.KeyVolumeUp10));
		_keyRepeatManagers.Add("KeyVolumeDown", new KeyRepeatManager(Program.KeyConfig.PlayScene.KeyVolumeDown));
		_keyRepeatManagers.Add("KeyVolumeDown1", new KeyRepeatManager(Program.KeyConfig.PlayScene.KeyVolumeDown1));
		_keyRepeatManagers.Add("KeyVolumeDown10", new KeyRepeatManager(Program.KeyConfig.PlayScene.KeyVolumeDown10));
		_keyRepeatManagers.Add("BgmVolumeUp", new KeyRepeatManager(Program.KeyConfig.PlayScene.BgmVolumeUp));
		_keyRepeatManagers.Add("BgmVolumeUp1", new KeyRepeatManager(Program.KeyConfig.PlayScene.BgmVolumeUp1));
		_keyRepeatManagers.Add("BgmVolumeUp10", new KeyRepeatManager(Program.KeyConfig.PlayScene.BgmVolumeUp10));
		_keyRepeatManagers.Add("BgmVolumeDown", new KeyRepeatManager(Program.KeyConfig.PlayScene.BgmVolumeDown));
		_keyRepeatManagers.Add("BgmVolumeDown1", new KeyRepeatManager(Program.KeyConfig.PlayScene.BgmVolumeDown1));
		_keyRepeatManagers.Add("BgmVolumeDown10", new KeyRepeatManager(Program.KeyConfig.PlayScene.BgmVolumeDown10));
		_keyRepeatManagers.Add("HiSpeedUp", new KeyRepeatManager(Program.KeyConfig.PlayScene.HiSpeedUp));
		_keyRepeatManagers.Add("HiSpeedUp1", new KeyRepeatManager(Program.KeyConfig.PlayScene.HiSpeedUp1));
		_keyRepeatManagers.Add("HiSpeedUp005", new KeyRepeatManager(Program.KeyConfig.PlayScene.HiSpeedUp005));
		_keyRepeatManagers.Add("HiSpeedDown", new KeyRepeatManager(Program.KeyConfig.PlayScene.HiSpeedDown));
		_keyRepeatManagers.Add("HiSpeedDown1", new KeyRepeatManager(Program.KeyConfig.PlayScene.HiSpeedDown1));
		_keyRepeatManagers.Add("HiSpeedDown005", new KeyRepeatManager(Program.KeyConfig.PlayScene.HiSpeedDown005));
		_keyRepeatManagers.Add("LaneCoverUp", new KeyRepeatManager(Program.KeyConfig.PlayScene.LaneCoverUp));
		_keyRepeatManagers.Add("LaneCoverUp10", new KeyRepeatManager(Program.KeyConfig.PlayScene.LaneCoverUp10));
		_keyRepeatManagers.Add("LaneCoverUp01", new KeyRepeatManager(Program.KeyConfig.PlayScene.LaneCoverUp01));
		_keyRepeatManagers.Add("LaneCoverUp005", new KeyRepeatManager(Program.KeyConfig.PlayScene.LaneCoverUp005));
		_keyRepeatManagers.Add("LaneCoverDown", new KeyRepeatManager(Program.KeyConfig.PlayScene.LaneCoverDown));
		_keyRepeatManagers.Add("LaneCoverDown10", new KeyRepeatManager(Program.KeyConfig.PlayScene.LaneCoverDown10));
		_keyRepeatManagers.Add("LaneCoverDown01", new KeyRepeatManager(Program.KeyConfig.PlayScene.LaneCoverDown01));
		_keyRepeatManagers.Add("LaneCoverDown005", new KeyRepeatManager(Program.KeyConfig.PlayScene.LaneCoverDown005));
		_keyRepeatManagers.Add("LiftUp", new KeyRepeatManager(Program.KeyConfig.PlayScene.LiftUp));
		_keyRepeatManagers.Add("LiftUp10", new KeyRepeatManager(Program.KeyConfig.PlayScene.LiftUp10));
		_keyRepeatManagers.Add("LiftUp01", new KeyRepeatManager(Program.KeyConfig.PlayScene.LiftUp01));
		_keyRepeatManagers.Add("LiftUp005", new KeyRepeatManager(Program.KeyConfig.PlayScene.LiftUp005));
		_keyRepeatManagers.Add("LiftDown", new KeyRepeatManager(Program.KeyConfig.PlayScene.LiftDown));
		_keyRepeatManagers.Add("LiftDown10", new KeyRepeatManager(Program.KeyConfig.PlayScene.LiftDown10));
		_keyRepeatManagers.Add("LiftDown01", new KeyRepeatManager(Program.KeyConfig.PlayScene.LiftDown01));
		_keyRepeatManagers.Add("LiftDown005", new KeyRepeatManager(Program.KeyConfig.PlayScene.LiftDown005));
		_keyRepeatManagers.Add("JudgeTimingUp", new KeyRepeatManager(Program.KeyConfig.PlayScene.JudgeTimingUp));
		_keyRepeatManagers.Add("JudgeTimingUp10", new KeyRepeatManager(Program.KeyConfig.PlayScene.JudgeTimingUp10));
		_keyRepeatManagers.Add("JudgeTimingDown", new KeyRepeatManager(Program.KeyConfig.PlayScene.JudgeTimingDown));
		_keyRepeatManagers.Add("JudgeTimingDown10", new KeyRepeatManager(Program.KeyConfig.PlayScene.JudgeTimingDown10));
		Flags.Player1LaneSwapOption = Program.InitialPlayer1LaneSwapOption ?? LaneSwapOption.None;
		Flags.Player2LaneSwapOption = Program.InitialPlayer2LaneSwapOption ?? LaneSwapOption.None;
		Flags.FlipSide = Program.InitialFlipSide ?? false;
		LoadBms();
		_isContentLoaded = true;
	}

	private void LoadBms()
	{
		_bmsData = Bms.Parse(BmsPath, 0, new XorShift(Program.Random.NextLong()));
		Numbers.MusicSpeedLevel = 0;
		_prevMusicSpeedLevel = 0;
		if (_bmsData.Keys == 9 && Flags.Player1LaneSwapOption == LaneSwapOption.AllScratch)
		{
			Flags.Player1LaneSwapOption = LaneSwapOption.SRandom;
		}
		bool hasBgmNotes = _bmsData.Bars.Any(b => b.Bgm.Count > 0);
		if (!hasBgmNotes && _bmsData.Bars.Count > 0)
		{
			HashSet<string> usedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (var bar in _bmsData.Bars)
			{
				foreach (var lane in bar.Lanes)
				{
					foreach (var obj in lane)
					{
						if (!string.IsNullOrEmpty(obj.Value))
						{
							usedKeys.Add(obj.Value);
						}
					}
				}
			}

			string bgmKey = null;
			foreach (var kvp in _bmsData.WavFiles)
			{
				if (!usedKeys.Contains(kvp.Key))
				{
					string fileName = Path.GetFileNameWithoutExtension(kvp.Value).ToLowerInvariant();
					if (fileName.Contains("music") || fileName.Contains("bgm") || fileName.Contains("song") || fileName.Contains("track"))
					{
						bgmKey = kvp.Key;
						break;
					}
				}
			}

			if (bgmKey == null)
			{
				var unused = _bmsData.WavFiles.Where(kvp => !usedKeys.Contains(kvp.Key)).ToList();
				if (unused.Count == 1)
				{
					bgmKey = unused[0].Key;
				}
			}

			if (bgmKey == null && !string.IsNullOrEmpty(BmsPath))
			{
				string dir = Path.GetDirectoryName(BmsPath);
				if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir))
				{
					string[] candidates = new string[] { "music.ogg", "music.wav", "music.mp3", "music.flac", "bgm.ogg", "bgm.wav", "bgm.mp3" };
					foreach (var cand in candidates)
					{
						string fullPath = Path.Combine(dir, cand);
						if (File.Exists(fullPath))
						{
							bgmKey = "0BGM";
							_bmsData.WavFiles[bgmKey] = cand;
							break;
						}
					}
				}
			}

			if (bgmKey != null)
			{
				_bmsData.Bars[0].AddBmsObject(BmsChannelType.Bgm, new BmsObject(0.0, bgmKey, BmsObjectType.Note));
			}
		}
		AddFallbackBgaMovie();
		_originalTimeTable = BmsTimeTable.CreateFromBmsData(_bmsData);
		if (Flags.Player1LaneSwapOption != LaneSwapOption.None || Flags.Player2LaneSwapOption != LaneSwapOption.None || Flags.FlipSide)
		{
			_baseTimeTable = _originalTimeTable.SwapLanes(_bmsData.Keys, Flags.Player1LaneSwapOption, Flags.Player2LaneSwapOption, Flags.FlipSide, Program.Random.NextLong());
		}
		else
		{
			_baseTimeTable = _originalTimeTable;
		}
		Numbers.PlayStyle = _bmsData.Keys;
		Numbers.PlayLevel = _bmsData.PlayLevel;
		Numbers.TotalNotes = (Flags.LegacyLN ? _bmsData.TotalNotesWithoutLNEnd : _bmsData.TotalNotes);
		Numbers.TotalLongNotes = _bmsData.TotalNotes - _bmsData.TotalNotesWithoutLNEnd;
		Numbers.TotalNotesPerLane = _baseTimeTable.GetTotalNotesPerLane(!Flags.LegacyLN);
		Numbers.GaugeTotal = _bmsData.Total;
		Flags.IsTotalDefined = _bmsData.IsTotalDefined;
		Numbers.TotalBars = _baseTimeTable.BarIndex.Count - 1;
		Numbers.CurrentBars = ((StartBar > 0) ? (StartBar - 1) : 0);
		Numbers.GrooveGauge = ((StartBar <= 0) ? 20.0 : 2.0);
		Numbers.GaugeTotalIncrement = 0.0;
		Numbers.MaxBpm = _baseTimeTable.MaxBpm;
		Numbers.MinBpm = _baseTimeTable.MinBpm;
		Numbers.MainNotesBpm = _baseTimeTable.MainNotesBpm;
		Numbers.MainTimeBpm = _baseTimeTable.MainTimeBpm;
		_gaugeIncrements[JudgeType.PGreat] = Numbers.GaugeTotal / (double)Numbers.TotalNotes;
		_gaugeIncrements[JudgeType.Great] = _gaugeIncrements[JudgeType.PGreat];
		_gaugeIncrements[JudgeType.Good] = _gaugeIncrements[JudgeType.PGreat] / 2.0;
		Flags.ContainsLN = Numbers.TotalLongNotes > 0;
		Flags.JudgeRank = (JudgeRank)_bmsData.JudgeRank;
		if (JudgeRangeList.ContainsKey(Flags.JudgeRank))
		{
			_judgeRange = JudgeRangeList[Flags.JudgeRank];
		}
		else
		{
			_judgeRange = JudgeRangeList[JudgeRank.Easy];
		}
		Flags.IsJudgeRankDefined = _bmsData.IsJudgeRankDefined;
		Flags.IsDifficultyDefined = _bmsData.IsDifficultyDefined;
		Numbers.Difficulty = _bmsData.Difficulty;
		Texts.Title = _bmsData.Title;
		Texts.Subtitle = _bmsData.Subtitle;
		Texts.Genre = _bmsData.Genre;
		Texts.Artist = _bmsData.Artist;
		Texts.Subartist = _bmsData.Subartist;
		_hasPoorBga = false;
		foreach (KeyValuePair<string, string> bmpFile in _bmsData.BmpFiles)
		{
			if (bmpFile.Key == "00")
			{
				_hasPoorBga = true;
				break;
			}
		}
		for (int i = 0; i < _bmsData.Bars.Count; i++)
		{
			if (_bmsData.Bars[i].Poor.Count > 0)
			{
				_hasPoorBga = true;
				break;
			}
		}
		Numbers.TotalLoadCount = (Flags.DisableBga ? _bmsData.WavFiles.Count : (_bmsData.WavFiles.Count + _bmsData.BmpFiles.Count));
		Numbers.CurrentLoadCount = 0;
		_isBmsReady = true;
		_lastKeySounds.Clear();
		_laneIndex.Clear();
		_lnJudge.Clear();
		_lnJudgeTime.Clear();
		for (int j = 0; j < _baseTimeTable.LaneCount; j++)
		{
			_notePos.Add(new NotePositionLaneCollection());
			_lastKeySounds.Add("");
			_laneIndex.Add(j);
			_lnJudge.Add(JudgeType.None);
			_lnJudgeTime.Add(0.0);
		}
		_isBarlineCountAdded = false;
		_isBarlineReached = false;
		switch (Numbers.PlayStyle)
		{
		case 5:
			PlayingKeyState.KeyConfig = Program.KeyConfig.PlayingKeys.FiveKeys ?? Program.KeyConfig.PlayingKeys.SevenKeys ?? Program.KeyConfig.PlayingKeys.FourteenKeys.ConvertToSinglePlay();
			break;
		default:
			PlayingKeyState.KeyConfig = Program.KeyConfig.PlayingKeys.SevenKeys ?? Program.KeyConfig.PlayingKeys.FourteenKeys.ConvertToSinglePlay();
			break;
		case 10:
			PlayingKeyState.KeyConfig = Program.KeyConfig.PlayingKeys.TenKeys ?? Program.KeyConfig.PlayingKeys.FourteenKeys;
			break;
		case 14:
			PlayingKeyState.KeyConfig = Program.KeyConfig.PlayingKeys.FourteenKeys;
			break;
		case 9:
			PlayingKeyState.KeyConfig = Program.KeyConfig.PlayingKeys.NineButtons;
			break;
		}
		if (!Flags.IsPlayStyle7 && !Flags.IsPlayStyle5 && Flags.Player1LaneCoverEnabled != Flags.Player2LaneCoverEnabled)
		{
			Flags.Player2LaneCoverEnabled = Flags.Player1LaneCoverEnabled;
		}
		_skin = Skin.Load(Utils.GetPath(string.Concat(str2: Flags.IsPlayStyle14 ? Program.Settings.SkinSettings.Play14 : (Flags.IsPlayStyle10 ? (string.IsNullOrEmpty(Program.Settings.SkinSettings.Play10) ? Program.Settings.SkinSettings.Play14 : Program.Settings.SkinSettings.Play10) : (Flags.IsPlayStyle9 ? Program.Settings.SkinSettings.Play9 : ((!Flags.IsPlayStyle5) ? Program.Settings.SkinSettings.Play7 : (string.IsNullOrEmpty(Program.Settings.SkinSettings.Play5) ? Program.Settings.SkinSettings.Play7 : Program.Settings.SkinSettings.Play5)))), str0: Program.StartupPath, str1: Path.DirectorySeparatorChar.ToString())), this);
		foreach (ISkinItem item in _skin.Items)
		{
			CheckSkinItem(item);
		}
		foreach (KeyValuePair<string, string> font in _skin.Fonts)
		{
			_fonts.Add(font.Key, new Font(Utils.GetPath(font.Value), Graphics));
		}
		Program.InvokeMainThreadAndWait(() =>
		{
			lock (Program.GraphicsLockObj)
			{
				_skinTextures = new Dictionary<string, TextureInfo>
				{
					{
						"#black",
						Skin.GenerateSolidColorTexture(Graphics, Color.Black)
					},
					{
						"#white",
						Skin.GenerateSolidColorTexture(Graphics, Color.White)
					}
				};
				foreach (KeyValuePair<string, string> texture in _skin.Textures)
				{
					_skinTextures.Add(texture.Key, Graphics.LoadTextureFromFile(Utils.GetPath(texture.Value)));
				}
			}
			Window.Title = "로딩 중... - " + Program.WindowTitle + " " + Program.GraphicsAudioTypeWindowTitle + (Flags.LegacyLN ? (" " + Program.LegacyLNModeWindowTitle) : "");
		});
		_isSkinReady = true;
		CheckScratchSideForAllScratch();
		EventTimers.SceneStart = EventTimers.GetNowTime();
	}

	private void CheckSkinItem(ISkinItem item)
	{
		if (item is SkinItemGroup skinItemGroup)
		{
			foreach (ISkinItem item2 in skinItemGroup.Items)
			{
				CheckSkinItem(item2);
			}
		}
		if (item is GaugeSkinItem gaugeSkinItem)
		{
			Numbers.GaugeDivision = gaugeSkinItem.GaugeDivision;
			Numbers.GaugeBorderIndex = gaugeSkinItem.Border;
		}
	}

	private void CheckScratchSideForAllScratch()
	{
		if (Flags.IsPlayStyle14 || Flags.IsPlayStyle10)
		{
			bool flag = Flags.Player1LaneSwapOption == LaneSwapOption.AllScratch && _skin.Player1ScratchSide == ScratchSide.Right;
			bool flag2 = Flags.Player2LaneSwapOption == LaneSwapOption.AllScratch && _skin.Player2ScratchSide == ScratchSide.Right;
			if (flag | flag2)
			{
				_baseTimeTable = _baseTimeTable.SwapLanes(_bmsData.Keys, flag ? LaneSwapOption.Mirror : LaneSwapOption.None, flag2 ? LaneSwapOption.Mirror : LaneSwapOption.None, flip: false, 0L);
			}
		}
	}

	private void LoadBmsResources()
	{
		Flags.Loading = true;
		EventTimers.LoadStart = EventTimers.NowTime;
		_keySounds = new Dictionary<string, AudioSource>();
		_bgaBmps = new Dictionary<string, BitmapImage>();
		_movies = new Dictionary<string, IMovie>();
		Task.Factory.StartNew(() =>
		{
			lock (_loadWavLockObj)
			{
				foreach (KeyValuePair<string, string> wavFile in _bmsData.WavFiles)
				{
					if (Flags.Exiting)
					{
						return;
					}
					string path = Utils.GetPath(Path.GetDirectoryName(BmsPath) + Path.DirectorySeparatorChar + wavFile.Value);
					try
					{
						if (File.Exists(path))
						{
							goto IL_008d;
						}
						List<string> extensions = new List<string>(SoundExtension);
						if (FindAlternativeFile(ref path, extensions))
						{
							goto IL_008d;
						}
						goto end_IL_0067;
						IL_008d:
						AudioSource value = Audio.LoadSound(path);
						if (Flags.Exiting)
						{
							return;
						}
						_keySounds.Add(wavFile.Key, value);
						goto IL_0103;
						end_IL_0067:;
					}
					catch (Exception)
					{
						if (!Audio.ForceResampling)
						{
							try
							{
								AudioSource value2 = Audio.LoadSound(path, repeat: false, resampling: true);
								if (Flags.Exiting)
								{
									return;
								}
								_keySounds.Add(wavFile.Key, value2);
							}
							catch (Exception)
							{
								goto end_IL_00be;
							}
							goto IL_0103;
						}
						end_IL_00be:;
					}
					continue;
					IL_0103:
					Numbers.CurrentLoadCount++;
				}
				_isWavReady = true;
			}
			if (_isWavReady && _isBmpReady)
			{
				Flags.LoadComplete = true;
			}
		});
		if (!Flags.DisableBga)
		{
			Task.Factory.StartNew(() =>
			{
				lock (_loadBgaLockObj)
				{
					foreach (KeyValuePair<string, string> bmpFile in _bmsData.BmpFiles)
					{
						if (Flags.Exiting)
						{
							return;
						}
						string path = Utils.GetPath(Path.GetDirectoryName(BmsPath) + Path.DirectorySeparatorChar + bmpFile.Value);
						if (!File.Exists(path))
						{
							List<string> list = new List<string>();
							if (ImageExtension.Contains(Path.GetExtension(path).ToLower()))
							{
								list.AddRange(ImageExtension);
								list.AddRange(MovieExtension);
							}
							else if (MovieExtension.Contains(Path.GetExtension(path).ToLower()))
							{
								list.AddRange(MovieExtension);
								list.AddRange(ImageExtension);
							}
							if (!FindAlternativeFile(ref path, list))
							{
								continue;
							}
						}
						if (ImageExtension.Contains(Path.GetExtension(path).ToLower()))
						{
							try
							{
								BitmapImage bitmapImage = LoadBgaBmp(path);
								if (Flags.Exiting)
								{
									return;
								}
								_bgaBmps.Add(bmpFile.Key, bitmapImage);
								_maxBgaWidth = Math.Max(_maxBgaWidth, bitmapImage.Width);
								_maxBgaHeight = Math.Max(_maxBgaHeight, bitmapImage.Height);
							}
							catch (Exception)
							{
							}
						}
						else if (MovieExtension.Contains(Path.GetExtension(path).ToLower()))
						{
							IMovie movie = PlatformGeneral.LoadMovie(path);
							if (movie != null)
							{
								if (Flags.Exiting)
								{
									return;
								}
								_movies.Add(bmpFile.Key, movie);
								_maxBgaWidth = Math.Max(_maxBgaWidth, movie.Width);
								_maxBgaHeight = Math.Max(_maxBgaHeight, movie.Height);
							}
						}
						_bmpLoadCount++;
						Numbers.CurrentLoadCount++;
					}
					_isBmpReady = true;
				}
				if (_isWavReady && _isBmpReady)
				{
					Flags.LoadComplete = true;
				}
			});
		}
		else
		{
			_isBmpReady = true;
			if (_isWavReady && _isBmpReady)
			{
				Flags.LoadComplete = true;
			}
		}
	}

	private bool FindAlternativeFile(ref string path, List<string> extensions)
	{
		List<string> list = new List<string>(extensions);
		list.Remove(Path.GetExtension(path).ToLower());
		while (list.Count > 0)
		{
			string candidate = Path.ChangeExtension(path, list[0]);
			if (File.Exists(candidate))
			{
				path = candidate;
				return true;
			}
			list.RemoveAt(0);
		}

		// bms editer 호환: 하위/동일 디렉터리 내 재귀 탐색 및 대소문자 무시 매칭
		try
		{
			string dir = Path.GetDirectoryName(BmsPath);
			if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir))
			{
				string fileNameWithoutExt = Path.GetFileNameWithoutExtension(path);
				foreach (string file in Directory.EnumerateFiles(dir, "*.*", SearchOption.AllDirectories))
				{
					string curExt = Path.GetExtension(file).ToLower();
					if (extensions.Contains(curExt) && string.Equals(Path.GetFileNameWithoutExtension(file), fileNameWithoutExt, StringComparison.OrdinalIgnoreCase))
					{
						path = file;
						return true;
					}
				}
			}
		}
		catch
		{
		}

		return false;
	}

	private void InitBarInfo(BmsTimeTable timeTable)
	{
		_barInfo = new List<BarStartInfo>();
		for (int i = 0; i < timeTable.BarIndex.Count; i++)
		{
			_barInfo.Add(new BarStartInfo());
			_barInfo[i].Time = ((timeTable.Rows.Count > timeTable.BarIndex[i]) ? timeTable.Rows[timeTable.BarIndex[i]].Time : 0.0);
		}
		_barInfo[0].Bpm = _bmsData.StartBpm;
		int num = 1;
		double num2 = _bmsData.StartBpm;
		string bga = "";
		string layer = "";
		string poor = "";
		double num3 = 0.0;
		double num4 = 0.0;
		double num5 = 0.0;
		List<int> list = new List<int>();
		Dictionary<string, double> dictionary = new Dictionary<string, double>();
		Dictionary<string, double> dictionary2 = new Dictionary<string, double>();
		for (int j = 0; j < timeTable.Rows.Count; j++)
		{
			foreach (string item in dictionary.Keys.ToList())
			{
				if (_keySounds.ContainsKey(item) && timeTable.Rows[j].Time - dictionary[item] > _keySounds[item].TotalTime * 1000.0)
				{
					dictionary.Remove(item);
				}
			}
			foreach (string item2 in dictionary2.Keys.ToList())
			{
				if (_keySounds.ContainsKey(item2) && timeTable.Rows[j].Time - dictionary2[item2] > _keySounds[item2].TotalTime * 1000.0)
				{
					dictionary2.Remove(item2);
				}
			}
			if (num < _barInfo.Count && Extensions.EpsilonEquals(timeTable.Rows[j].Time, _barInfo[num].Time))
			{
				_barInfo[num].Bpm = num2;
				_barInfo[num].Bga = bga;
				_barInfo[num].Layer = layer;
				_barInfo[num].Poor = poor;
				_barInfo[num].BgaPosition = _barInfo[num].Time - num3;
				_barInfo[num].LayerPosition = _barInfo[num].Time - num4;
				_barInfo[num].PoorPosition = _barInfo[num].Time - num5;
				foreach (KeyValuePair<string, double> item3 in dictionary)
				{
					_barInfo[num].Keys.Add(item3.Key, _barInfo[num].Time - item3.Value);
				}
				foreach (KeyValuePair<string, double> item4 in dictionary2)
				{
					_barInfo[num].Bgm.Add(item4.Key, _barInfo[num].Time - item4.Value);
				}
				_barInfo[num].LNHolding.AddRange(list);
				num++;
			}
			for (int k = 0; k < timeTable.Rows[j].Lanes.Count; k++)
			{
				int num6 = 0;
				for (int l = 0; l < timeTable.Rows[j].Lanes[k].Count; l++)
				{
					if (timeTable.Rows[j].Lanes[k][l].ObjectType == BmsObjectType.Note || timeTable.Rows[j].Lanes[k][l].ObjectType == BmsObjectType.LNStart)
					{
						if (dictionary.ContainsKey(timeTable.Rows[j].Lanes[k][l].Value))
						{
							dictionary[timeTable.Rows[j].Lanes[k][l].Value] = timeTable.Rows[j].Lanes[k][l].Time;
						}
						else
						{
							dictionary.Add(timeTable.Rows[j].Lanes[k][l].Value, timeTable.Rows[j].Lanes[k][l].Time);
						}
						if (_reverseBar < 0 && _keySounds.ContainsKey(timeTable.Rows[j].Lanes[k][l].Value) && _totalMusicTime < timeTable.Rows[j].Lanes[k][l].Time + _keySounds[timeTable.Rows[j].Lanes[k][l].Value].TotalTime * 1000.0)
						{
							_totalMusicTime = timeTable.Rows[j].Lanes[k][l].Time + _keySounds[timeTable.Rows[j].Lanes[k][l].Value].TotalTime * 1000.0;
						}
						if (timeTable.Rows[j].Lanes[k][l].ObjectType == BmsObjectType.LNStart)
						{
							list.Add(k);
						}
					}
					else if (timeTable.Rows[j].Lanes[k][l].ObjectType == BmsObjectType.LNEnd)
					{
						list.Remove(k);
					}
					if (_reverseBar < 0 && (timeTable.Rows[j].Lanes[k][l].ObjectType == BmsObjectType.Note || timeTable.Rows[j].Lanes[k][l].ObjectType == BmsObjectType.LNEnd))
					{
						num6++;
					}
				}
				if (num6 > 0)
				{
					_lastNoteIndex = j;
					_lastNoteCount = num6;
				}
			}
			for (int m = 0; m < timeTable.Rows[j].Channels[BmsChannelType.Bgm].Count; m++)
			{
				if (dictionary2.ContainsKey(timeTable.Rows[j].Channels[BmsChannelType.Bgm][m].Value))
				{
					dictionary2[timeTable.Rows[j].Channels[BmsChannelType.Bgm][m].Value] = timeTable.Rows[j].Channels[BmsChannelType.Bgm][m].Time;
				}
				else
				{
					dictionary2.Add(timeTable.Rows[j].Channels[BmsChannelType.Bgm][m].Value, timeTable.Rows[j].Channels[BmsChannelType.Bgm][m].Time);
				}
				if (_reverseBar < 0 && _keySounds.ContainsKey(timeTable.Rows[j].Channels[BmsChannelType.Bgm][m].Value) && _totalMusicTime < timeTable.Rows[j].Channels[BmsChannelType.Bgm][m].Time + _keySounds[timeTable.Rows[j].Channels[BmsChannelType.Bgm][m].Value].TotalTime * 1000.0)
				{
					_totalMusicTime = timeTable.Rows[j].Channels[BmsChannelType.Bgm][m].Time + _keySounds[timeTable.Rows[j].Channels[BmsChannelType.Bgm][m].Value].TotalTime * 1000.0;
				}
			}
			for (int n = 0; n < timeTable.Rows[j].Channels[BmsChannelType.Bga].Count; n++)
			{
				bga = timeTable.Rows[j].Channels[BmsChannelType.Bga][n].Value;
				num3 = timeTable.Rows[j].Channels[BmsChannelType.Bga][n].Time;
			}
			for (int num7 = 0; num7 < timeTable.Rows[j].Channels[BmsChannelType.Layer].Count; num7++)
			{
				layer = timeTable.Rows[j].Channels[BmsChannelType.Layer][num7].Value;
				num4 = timeTable.Rows[j].Channels[BmsChannelType.Layer][num7].Time;
			}
			for (int num8 = 0; num8 < timeTable.Rows[j].Channels[BmsChannelType.Poor].Count; num8++)
			{
				poor = timeTable.Rows[j].Channels[BmsChannelType.Poor][num8].Value;
				num5 = timeTable.Rows[j].Channels[BmsChannelType.Poor][num8].Time;
			}
			for (int num9 = 0; num9 < timeTable.Rows[j].Channels[BmsChannelType.Bpm].Count; num9++)
			{
				num2 = double.Parse(timeTable.Rows[j].Channels[BmsChannelType.Bpm][num9].Value);
				if (num2 < 0.0 && _reverseBar < 0)
				{
					_reverseBar = num - 1;
				}
			}
		}
		_lastIndex = _lastNoteIndex;
		Numbers.TotalMusicTime = TimeSpan.FromMilliseconds(_totalMusicTime);
	}

	private void InitBarInfo()
	{
		InitBarInfo(_baseTimeTable);
	}

	public override void UnloadContent()
	{
		if (_isContentLoaded)
		{
			Program.Settings.MasterVolume = Numbers.MasterVolume;
			Program.Settings.KeyVolume = Numbers.KeyVolume;
			Program.Settings.BgmVolume = Numbers.BgmVolume;
			Program.Settings.HiSpeed = Numbers.Player1HiSpeed;
			Program.Settings.HiSpeedFixType = Flags.HiSpeedFixType;
			Program.Settings.LegacyLNMode = Flags.LegacyLN;
			Program.Settings.Player1LaneCoverEnabled = Flags.Player1LaneCoverEnabled;
			Program.Settings.Player2LaneCoverEnabled = Flags.Player2LaneCoverEnabled;
			Program.Settings.LaneCoverRange = (decimal)Numbers.Player1LaneCover / 100m;
			Program.Settings.JudgeLineLift = (decimal)Numbers.Player1Lift / 100m;
			ClearResources();
			_isContentLoaded = false;
		}
	}

	private void ClearResources()
	{
		Program.SkipDraw = true;
		_isSkinReady = false;
		_isBmsReady = false;
		_isWavReady = false;
		_isBmpReady = false;
		Flags.Loading = false;
		Flags.LoadComplete = false;
		EventTimers.LoadStart = 0.0;
		EventTimers.LoadComplete = 0.0;
		_isStartBarProcessed = false;
		PlayingKeyState.ClearKeyboardAssign();
		ResetBmsState();
		Numbers.Bpm = 130.0;
		Numbers.PlayLevel = 0;
		Numbers.GrooveGauge = 0.0;
		Numbers.GaugeDivision = 50;
		Numbers.GaugeBorderIndex = 40;
		Numbers.GaugeTotalIncrement = 0.0;
		Numbers.CurrentBars = 0;
		Numbers.TotalMusicTime = TimeSpan.Zero;
		_reverseBar = -1;
		StartBar = 0;
		_totalMusicTime = 0.0;
		_lastNoteIndex = 0;
		_lastNoteCount = 0;
		_gaugeIncrements[JudgeType.PGreat] = 0.0;
		_gaugeIncrements[JudgeType.Great] = 0.0;
		_gaugeIncrements[JudgeType.Good] = 0.0;
		Texts.Title = "";
		Texts.Subtitle = "";
		Texts.Genre = "";
		Texts.Artist = "";
		Texts.Subartist = "";
		_bmsData = null;
		_baseTimeTable = null;
		_originalTimeTable = null;
		_timeTable = null;
		if (_barInfo != null)
		{
			_barInfo.Clear();
		}
		foreach (string item in _keySounds.Keys.ToList())
		{
			Utils.Free(_keySounds[item]);
			_keySounds.Remove(item);
		}
		Utils.Free(_bgaBmps);
		foreach (string item2 in _movies.Keys.ToList())
		{
			Utils.Free(_movies[item2]);
			_movies.Remove(item2);
		}
		Program.InvokeMainThreadAndWait(() =>
		{
			lock (Program.GraphicsLockObj)
			{
				Utils.Free(_bgaTexture);
				Utils.Free(_layerTexture);
				Utils.Free(_poorTexture);
				_tempBgaData?.Dispose();
				_tempBgaData = null;
				_bmpLoadCount = 0;
				_maxBgaWidth = 1;
				_maxBgaHeight = 1;
				_skin = null;
				foreach (string item3 in _skinTextures.Keys.ToList())
				{
					Utils.Free(_skinTextures[item3]);
					_skinTextures.Remove(item3);
				}
				foreach (string item4 in _fonts.Keys.ToList())
				{
					Utils.Free(_fonts[item4]);
					_fonts.Remove(item4);
				}
			}
		});
		while (_notePos.Count > 0)
		{
			_notePos[0].Clear();
			_notePos.RemoveAt(0);
		}
		_barlinePos1.Clear();
		_barlinePos2.Clear();
	}

	private void ResetBmsState()
	{
		EventTimers.PlayStart = 0.0;
		lock (_loadWavLockObj)
		{
			foreach (KeyValuePair<string, AudioSource> keySound in _keySounds)
			{
				keySound.Value.Stop();
			}
		}
		lock (_loadBgaLockObj)
		{
			foreach (KeyValuePair<string, IMovie> movie in _movies)
			{
				if (movie.Value.IsPlaying)
				{
					movie.Value.Stop();
				}
			}
		}
		EventTimers.ResetPlayingTimers();
		EventTimers.StopBpmTimer();
		Flags.ResetPlayingFlags();
		AutoPlayKeyState.ResetKeyState();
		Numbers.ElapsedMusicTime = TimeSpan.Zero;
		Numbers.ResetNotesPerSecond();
		Numbers.ClearJudgeCount();
		_nowBga = "";
		_nowLayer = "";
		_nowPoor = "";
		_prevBga = "";
		_prevLayer = "";
		_prevPoor = "";
		_reverseTime = 0.0;
		_lastCount = 0;
		_startLNHolding.Clear();
		_startLNHoldingProcessed = false;
		_laneIndex.Clear();
		_lnJudge.Clear();
		_lnJudgeTime.Clear();
		for (int i = 0; i < _lastKeySounds.Count; i++)
		{
			_lastKeySounds[i] = "";
			_laneIndex.Add(i);
			_lnJudge.Add(JudgeType.None);
			_lnJudgeTime.Add(0.0);
		}
		_isBarlineCountAdded = false;
		_isBarlineReached = false;
		Program.InvokeMainThreadAndWait(() =>
		{
			lock (Program.GraphicsLockObj)
			{
				if (_bgaTexture != null)
				{
					((IDynamicTextureInfo)_bgaTexture).Clear();
				}
				if (_layerTexture != null)
				{
					((IDynamicTextureInfo)_layerTexture).Clear();
				}
				if (_poorTexture != null)
				{
					((IDynamicTextureInfo)_poorTexture).Clear();
				}
			}
		});
	}

	private void Restart(int restartBar, bool swapLanes)
	{
		if (restartBar < 0 || restartBar > Numbers.TotalBars)
		{
			return;
		}
		ResetBmsState();
		if (swapLanes)
		{
			if (Flags.Player1LaneSwapOption != LaneSwapOption.None || Flags.Player2LaneSwapOption != LaneSwapOption.None || Flags.FlipSide)
			{
				_baseTimeTable = _originalTimeTable.SwapLanes(_bmsData.Keys, Flags.Player1LaneSwapOption, Flags.Player2LaneSwapOption, Flags.FlipSide, Program.Random.NextLong());
				CheckScratchSideForAllScratch();
			}
			else
			{
				_baseTimeTable = _originalTimeTable;
			}
			InitBarInfo();
			GC.Collect();
		}
		_timeTable = new BmsTimeTable(_baseTimeTable, Numbers.MusicSpeed);
		Numbers.Bpm = _barInfo[restartBar].Bpm * Numbers.MusicSpeed;
		Numbers.GrooveGauge = ((restartBar <= 0) ? 20.0 : 2.0);
		Numbers.GaugeTotalIncrement = 0.0;
		Numbers.CurrentBars = ((restartBar > 0) ? (restartBar - 1) : 0);
		Numbers.MaxBpm = _timeTable.MaxBpm;
		Numbers.MinBpm = _timeTable.MinBpm;
		Numbers.MainNotesBpm = _timeTable.MainNotesBpm;
		Numbers.MainTimeBpm = _timeTable.MainTimeBpm;
		Numbers.TotalNotes = (Flags.LegacyLN ? _bmsData.TotalNotesWithoutLNEnd : _bmsData.TotalNotes);
		Numbers.TotalNotesPerLane = _baseTimeTable.GetTotalNotesPerLane(!Flags.LegacyLN);
		_gaugeIncrements[JudgeType.PGreat] = Numbers.GaugeTotal / (double)Numbers.TotalNotes;
		_gaugeIncrements[JudgeType.Great] = _gaugeIncrements[JudgeType.PGreat];
		_gaugeIncrements[JudgeType.Good] = _gaugeIncrements[JudgeType.PGreat] / 2.0;
		_nowBga = _barInfo[restartBar].Bga;
		_nowLayer = _barInfo[restartBar].Layer;
		_nowPoor = _barInfo[restartBar].Poor;
		if (_hasPoorBga && _nowPoor == "")
		{
			_nowPoor = "00";
		}
		_prevBga = "*";
		_prevLayer = "*";
		_prevPoor = "*";
		_lastIndex = _lastNoteIndex;
		_startLNHolding.AddRange(_barInfo[restartBar].LNHolding);
		EventTimers.UpdateNowTime();
		EventTimers.PlayStart = ((_baseTimeTable.Rows.Count > _baseTimeTable.BarIndex[restartBar]) ? (EventTimers.NowTime - _baseTimeTable.Rows[_baseTimeTable.BarIndex[restartBar]].Time * (1.0 / Numbers.MusicSpeed)) : EventTimers.NowTime);
		EventTimers.ResetBpmTimer();
		if (Numbers.MusicSpeedLevel != _prevMusicSpeedLevel)
		{
			foreach (KeyValuePair<string, AudioSource> keySound in _keySounds)
			{
				keySound.Value.Speed = (float)Numbers.MusicSpeed;
			}
		}
		foreach (KeyValuePair<string, double> item in _barInfo[restartBar].Bgm)
		{
			if (_keySounds.ContainsKey(item.Key))
			{
				_keySounds[item.Key].Volume = (Flags.IsBgmVolumeMute ? 0f : ((float)Numbers.BgmVolume / 100f));
				if (item.Value / 1000.0 < _keySounds[item.Key].TotalTime && (!Flags.IsBgmVolumeMute || Numbers.BgmVolume > 0))
				{
					_keySounds[item.Key].Play(_keySounds[item.Key].GetPositionFromSeconds(item.Value / 1000.0));
				}
			}
		}
		if (Flags.AutoPlay)
		{
			foreach (KeyValuePair<string, double> key in _barInfo[restartBar].Keys)
			{
				if (_keySounds.ContainsKey(key.Key))
				{
					_keySounds[key.Key].Volume = (Flags.IsKeyVolumeMute ? 0f : ((float)Numbers.KeyVolume / 100f));
					if (key.Value / 1000.0 < _keySounds[key.Key].TotalTime && (!Flags.IsKeyVolumeMute || Numbers.KeyVolume > 0))
					{
						_keySounds[key.Key].Play(_keySounds[key.Key].GetPositionFromSeconds(key.Value / 1000.0));
					}
				}
			}
		}
		RestartMovie(_nowBga, _barInfo[restartBar].BgaPosition);
		RestartMovie(_nowLayer, _barInfo[restartBar].LayerPosition);
		RestartMovie(_nowPoor, _barInfo[restartBar].PoorPosition);
		double num = (EventTimers.NowTime - EventTimers.PlayStart) * Numbers.MusicSpeed;
		Numbers.ElapsedMusicTime = ((num < _totalMusicTime) ? TimeSpan.FromMilliseconds(num) : Numbers.TotalMusicTime);
	}

	private void Restart(int restartBar)
	{
		Restart(restartBar, swapLanes: false);
	}

	private void Restart(bool swapLanes)
	{
		Restart(StartBar, swapLanes);
	}

	private void Restart()
	{
		Restart(StartBar, swapLanes: false);
	}

	private bool ProcessKeyInput()
	{
		foreach (KeyValuePair<string, KeyRepeatManager> keyRepeatManager in _keyRepeatManagers)
		{
			keyRepeatManager.Value.Update(EventTimers.NowTime);
		}
		if (ProcessCommonKeyInput())
		{
			return true;
		}
		if (!Flags.Loading && Flags.LoadComplete)
		{
			if (!Program.KeyConfig.PlayScene.BackBar.IsKeyDown() || !Program.KeyConfig.PlayScene.ForwardBar.IsKeyDown())
			{
				if (_keyRepeatManagers["BackBarTime"].IsKeyDownOnce())
				{
					double num = Numbers.ElapsedMusicTime.TotalMilliseconds - 5000.0;
					if (num <= 0.0 || Numbers.CurrentBars < 1)
					{
						StartBar = 0;
					}
					else
					{
						StartBar = Numbers.CurrentBars - 1;
						for (int num2 = Numbers.CurrentBars - 1; num2 >= 0; num2--)
						{
							if (_baseTimeTable.Rows[_baseTimeTable.BarIndex[num2]].Time <= num)
							{
								StartBar = num2;
								break;
							}
							if (num2 == 0)
							{
								StartBar = 0;
							}
						}
					}
					Restart();
				}
				else if (_keyRepeatManagers["BackBar4"].IsKeyDownOnce())
				{
					StartBar = ((Numbers.CurrentBars - 4 > 0) ? (Numbers.CurrentBars - 4) : 0);
					Restart();
				}
				else if (_keyRepeatManagers["BackBar"].IsKeyDownOnce())
				{
					StartBar = ((Numbers.CurrentBars - 1 > 0) ? (Numbers.CurrentBars - 1) : 0);
					Restart();
				}
				else if (_keyRepeatManagers["ForwardBarTime"].IsKeyDownOnce())
				{
					int num3 = ((_reverseBar >= 0) ? _reverseBar : Numbers.TotalBars);
					double num4 = Numbers.ElapsedMusicTime.TotalMilliseconds + 5000.0;
					if (num4 >= Numbers.TotalMusicTime.TotalMilliseconds || Numbers.CurrentBars >= num3)
					{
						StartBar = num3;
					}
					else
					{
						StartBar = Numbers.CurrentBars + 1;
						for (int i = Numbers.CurrentBars + 1; i <= num3; i++)
						{
							if (_baseTimeTable.Rows[_baseTimeTable.BarIndex[i]].Time >= num4)
							{
								StartBar = i;
								break;
							}
							if (i == num3)
							{
								StartBar = num3;
							}
						}
					}
					Restart();
				}
				else if (_keyRepeatManagers["ForwardBar4"].IsKeyDownOnce())
				{
					int num5 = ((_reverseBar >= 0) ? _reverseBar : Numbers.TotalBars);
					StartBar = ((Numbers.CurrentBars + 4 >= num5) ? num5 : (Numbers.CurrentBars + 4));
					Restart();
				}
				else if (_keyRepeatManagers["ForwardBar"].IsKeyDownOnce())
				{
					int num6 = ((_reverseBar >= 0) ? _reverseBar : Numbers.TotalBars);
					StartBar = ((Numbers.CurrentBars + 1 >= num6) ? num6 : (Numbers.CurrentBars + 1));
					Restart();
				}
			}
			if (Program.KeyConfig.PlayScene.ResetMusicSpeed.IsKeyDown())
			{
				if (Program.KeyConfig.PlayScene.ResetMusicSpeed.IsKeyDownOnce())
				{
					ChangeMusicSpeed(0);
				}
			}
			else if (_keyRepeatManagers["IncreaseMusicSpeed"].IsKeyDownOnce())
			{
				ChangeMusicSpeed(Numbers.MusicSpeedLevel + 1);
			}
			else if (_keyRepeatManagers["DecreaseMusicSpeed"].IsKeyDownOnce())
			{
				ChangeMusicSpeed(Numbers.MusicSpeedLevel - 1);
			}
			if (Program.KeyConfig.PlayScene.ToggleAutoplay.IsKeyDownOnce())
			{
				Flags.AutoPlay = !Flags.AutoPlay;
				StartBar = 0;
				Restart();
			}
			if (Program.KeyConfig.PlayScene.ChangeLNMode.IsKeyDownOnce())
			{
				Flags.LegacyLN = !Flags.LegacyLN;
				Program.InvokeMainThread(() =>
				{
					Window.Title = PlatformGeneral.GetPlayingWindowTitle();
				});
				StartBar = 0;
				Restart();
			}
			else
			{
				if (Program.KeyConfig.PlayScene.Reload.IsKeyDownOnce())
				{
					ClearResources();
					LoadBms();
					return true;
				}
				if (Program.KeyConfig.PlayScene.Flip.IsKeyDownOnce())
				{
					if (Flags.IsPlayStyle14 || Flags.IsPlayStyle10)
					{
						Flags.FlipSide = !Flags.FlipSide;
						StartBar = 0;
						Restart(swapLanes: true);
					}
				}
				else if (Program.KeyConfig.PlayScene.Random.IsKeyDownOnce() || Program.KeyConfig.PlayScene.Mirror.IsKeyDownOnce() || Program.KeyConfig.PlayScene.SRandom.IsKeyDownOnce())
				{
					LaneSwapOption laneSwapOption;
					if (Program.KeyConfig.PlayScene.Mirror.IsKeyDownOnce())
					{
						laneSwapOption = LaneSwapOption.Mirror;
					}
					else if (Program.KeyConfig.PlayScene.SRandom.IsKeyDownOnce())
					{
						if (PlayingKeyState.IsKeyDownAny(PlayingKeys.Player1Key2, PlayingKeys.Player2Key2))
						{
							laneSwapOption = LaneSwapOption.HRandom;
						}
						else if (PlayingKeyState.IsKeyDownAny(PlayingKeys.Player1Key3, PlayingKeys.Player2Key3))
						{
							laneSwapOption = LaneSwapOption.NonCorrectedSRandom;
						}
						else if (PlayingKeyState.IsKeyDownAny(PlayingKeys.Player1Key4, PlayingKeys.Player2Key4))
						{
							laneSwapOption = LaneSwapOption.PeriodicSRandom;
						}
						else
						{
							laneSwapOption = ((!PlayingKeyState.IsKeyDownAny(PlayingKeys.Player1Key5, PlayingKeys.Player2Key5) || Flags.IsPlayStyle9) ? LaneSwapOption.SRandom : LaneSwapOption.AllScratch);
						}
					}
					else if (PlayingKeyState.IsKeyDownAny(PlayingKeys.Player1Key2, PlayingKeys.Player2Key2))
					{
						laneSwapOption = ((!PlayingKeyState.IsKeyDownAny(PlayingKeys.Player1Key3, PlayingKeys.Player2Key3)) ? LaneSwapOption.Rotate : LaneSwapOption.CrossRotate);
					}
					else if (PlayingKeyState.IsKeyDownAny(PlayingKeys.Player1Key3, PlayingKeys.Player2Key3))
					{
						laneSwapOption = LaneSwapOption.Cross;
					}
					else if (PlayingKeyState.IsKeyDownAny(PlayingKeys.Player1Key4, PlayingKeys.Player2Key4))
					{
						laneSwapOption = ((!PlayingKeyState.IsKeyDownAny(PlayingKeys.Player1Key5, PlayingKeys.Player2Key5)) ? LaneSwapOption.Split : LaneSwapOption.Spread);
					}
					else
					{
						laneSwapOption = ((!PlayingKeyState.IsKeyDownAny(PlayingKeys.Player1Key5, PlayingKeys.Player2Key5)) ? LaneSwapOption.Random : LaneSwapOption.OddEven);
					}
					if (Flags.IsPlayStyle14 || Flags.IsPlayStyle10)
					{
						if (PlayingKeyState.IsKeyDownAny(PlayingKeys.Player1Key1, PlayingKeys.Player2Key1))
						{
							if (PlayingKeyState.IsKeyDown(PlayingKeys.Player1Key1))
							{
								Flags.Player1LaneSwapOption = ((Flags.Player1LaneSwapOption != laneSwapOption) ? laneSwapOption : LaneSwapOption.None);
							}
							if (PlayingKeyState.IsKeyDown(PlayingKeys.Player2Key1))
							{
								Flags.Player2LaneSwapOption = ((Flags.Player2LaneSwapOption != laneSwapOption) ? laneSwapOption : LaneSwapOption.None);
							}
						}
						else
						{
							if (Flags.Player1LaneSwapOption == Flags.Player2LaneSwapOption && Flags.Player1LaneSwapOption == laneSwapOption)
							{
								Flags.Player1LaneSwapOption = LaneSwapOption.None;
							}
							else
							{
								Flags.Player1LaneSwapOption = laneSwapOption;
							}
							Flags.Player2LaneSwapOption = Flags.Player1LaneSwapOption;
						}
					}
					else
					{
						Flags.Player1LaneSwapOption = ((Flags.Player1LaneSwapOption != laneSwapOption) ? laneSwapOption : LaneSwapOption.None);
						Flags.Player2LaneSwapOption = LaneSwapOption.None;
					}
					StartBar = 0;
					Restart(swapLanes: true);
				}
				else if (Program.KeyConfig.PlayScene.ClearRandomMirror.IsKeyDownOnce())
				{
					bool flag = false;
					if (Flags.Player1LaneSwapOption != LaneSwapOption.None)
					{
						flag = true;
					}
					if ((Flags.IsPlayStyle14 || Flags.IsPlayStyle10) && (Flags.Player2LaneSwapOption != LaneSwapOption.None || Flags.FlipSide))
					{
						flag = true;
					}
					if (flag)
					{
						Flags.Player1LaneSwapOption = LaneSwapOption.None;
						if (Flags.IsPlayStyle14 || Flags.IsPlayStyle10)
						{
							Flags.Player2LaneSwapOption = LaneSwapOption.None;
							Flags.FlipSide = false;
						}
						StartBar = 0;
						Restart(swapLanes: true);
					}
				}
				else if (Program.KeyConfig.PlayScene.Rerandomize.IsKeyDownOnce())
				{
					StartBar = 0;
					Restart(Flags.Player1LaneSwapOption.IsRandomize() || Flags.Player2LaneSwapOption.IsRandomize());
				}
				else if (((Flags.AutoPlay || Flags.IsMusicFinished) && Program.KeyConfig.PlayScene.BackToStart.IsKeyDownOnce()) || Program.KeyConfig.PlayScene.BackToStartInTestPlay.IsKeyDownOnce())
				{
					StartBar = 0;
					Restart();
				}
			}
			if (!Flags.AutoPlay)
			{
				if (Program.KeyConfig.PlayScene.JudgeTimingReset.IsKeyDown())
				{
					if (Program.KeyConfig.PlayScene.JudgeTimingReset.IsKeyDownOnce())
					{
						Numbers.JudgeTiming = 0.0;
					}
				}
				else if (_keyRepeatManagers["JudgeTimingUp10"].IsKeyDownOnce())
				{
					Numbers.JudgeTiming = (int)Numbers.JudgeTiming + 10;
				}
				else if (_keyRepeatManagers["JudgeTimingUp"].IsKeyDownOnce())
				{
					Numbers.JudgeTiming = (int)Numbers.JudgeTiming + 1;
				}
				else if (_keyRepeatManagers["JudgeTimingDown10"].IsKeyDownOnce())
				{
					Numbers.JudgeTiming = (int)Numbers.JudgeTiming - 10;
				}
				else if (_keyRepeatManagers["JudgeTimingDown"].IsKeyDownOnce())
				{
					Numbers.JudgeTiming = (int)Numbers.JudgeTiming - 1;
				}
			}
		}
		if (Program.KeyConfig.PlayScene.MasterVolumeMute.IsKeyDown() || Program.KeyConfig.PlayScene.MasterVolumeMax.IsKeyDown())
		{
			if (Program.KeyConfig.PlayScene.MasterVolumeMute.IsKeyDownOnce())
			{
				Numbers.MasterVolume = 0;
			}
			else if (Program.KeyConfig.PlayScene.MasterVolumeMax.IsKeyDownOnce())
			{
				Numbers.MasterVolume = 100;
			}
		}
		else if (_keyRepeatManagers["MasterVolumeDown10"].IsKeyDownOnce())
		{
			Numbers.MasterVolume -= 10;
		}
		else if (_keyRepeatManagers["MasterVolumeDown1"].IsKeyDownOnce())
		{
			Numbers.MasterVolume--;
		}
		else if (_keyRepeatManagers["MasterVolumeDown"].IsKeyDownOnce())
		{
			Numbers.MasterVolume -= 5;
		}
		else if (_keyRepeatManagers["MasterVolumeUp10"].IsKeyDownOnce())
		{
			Numbers.MasterVolume += 10;
		}
		else if (_keyRepeatManagers["MasterVolumeUp1"].IsKeyDownOnce())
		{
			Numbers.MasterVolume++;
		}
		else if (_keyRepeatManagers["MasterVolumeUp"].IsKeyDownOnce())
		{
			Numbers.MasterVolume += 5;
		}
		if (Program.KeyConfig.PlayScene.KeyVolumeMute.IsKeyDown() || Program.KeyConfig.PlayScene.KeyVolumeMax.IsKeyDown())
		{
			if (Program.KeyConfig.PlayScene.KeyVolumeMute.IsKeyDownOnce())
			{
				Numbers.KeyVolume = 0;
			}
			else if (Program.KeyConfig.PlayScene.KeyVolumeMax.IsKeyDownOnce())
			{
				Numbers.KeyVolume = 100;
			}
		}
		else if (_keyRepeatManagers["KeyVolumeDown10"].IsKeyDownOnce())
		{
			Numbers.KeyVolume -= 10;
		}
		else if (_keyRepeatManagers["KeyVolumeDown1"].IsKeyDownOnce())
		{
			Numbers.KeyVolume--;
		}
		else if (_keyRepeatManagers["KeyVolumeDown"].IsKeyDownOnce())
		{
			Numbers.KeyVolume -= 5;
		}
		else if (_keyRepeatManagers["KeyVolumeUp10"].IsKeyDownOnce())
		{
			Numbers.KeyVolume += 10;
		}
		else if (_keyRepeatManagers["KeyVolumeUp1"].IsKeyDownOnce())
		{
			Numbers.KeyVolume++;
		}
		else if (_keyRepeatManagers["KeyVolumeUp"].IsKeyDownOnce())
		{
			Numbers.KeyVolume += 5;
		}
		if (Program.KeyConfig.PlayScene.BgmVolumeMute.IsKeyDown() || Program.KeyConfig.PlayScene.BgmVolumeMax.IsKeyDown())
		{
			if (Program.KeyConfig.PlayScene.BgmVolumeMute.IsKeyDownOnce())
			{
				Numbers.BgmVolume = 0;
			}
			else if (Program.KeyConfig.PlayScene.BgmVolumeMax.IsKeyDownOnce())
			{
				Numbers.BgmVolume = 100;
			}
		}
		else if (_keyRepeatManagers["BgmVolumeDown10"].IsKeyDownOnce())
		{
			Numbers.BgmVolume -= 10;
		}
		else if (_keyRepeatManagers["BgmVolumeDown1"].IsKeyDownOnce())
		{
			Numbers.BgmVolume--;
		}
		else if (_keyRepeatManagers["BgmVolumeDown"].IsKeyDownOnce())
		{
			Numbers.BgmVolume -= 5;
		}
		else if (_keyRepeatManagers["BgmVolumeUp10"].IsKeyDownOnce())
		{
			Numbers.BgmVolume += 10;
		}
		else if (_keyRepeatManagers["BgmVolumeUp1"].IsKeyDownOnce())
		{
			Numbers.BgmVolume++;
		}
		else if (_keyRepeatManagers["BgmVolumeUp"].IsKeyDownOnce())
		{
			Numbers.BgmVolume += 5;
		}
		if (Program.KeyConfig.PlayScene.HiSpeedReset.IsKeyDown())
		{
			if (Program.KeyConfig.PlayScene.HiSpeedReset.IsKeyDownOnce())
			{
				Numbers.Player1HiSpeed = 1.0;
				Numbers.Player2HiSpeed = 1.0;
			}
		}
		else if (_keyRepeatManagers["HiSpeedDown1"].IsKeyDownOnce())
		{
			HiSpeedDown(1.0);
		}
		else if (_keyRepeatManagers["HiSpeedDown005"].IsKeyDownOnce())
		{
			HiSpeedDown(0.05);
		}
		else if (_keyRepeatManagers["HiSpeedDown"].IsKeyDownOnce())
		{
			HiSpeedDown(0.25);
		}
		else if (_keyRepeatManagers["HiSpeedUp1"].IsKeyDownOnce())
		{
			HiSpeedUp(1.0);
		}
		else if (_keyRepeatManagers["HiSpeedUp005"].IsKeyDownOnce())
		{
			HiSpeedUp(0.05);
		}
		else if (_keyRepeatManagers["HiSpeedUp"].IsKeyDownOnce())
		{
			HiSpeedUp(0.25);
		}
		if (Program.KeyConfig.PlayScene.ChangeHSFixBack.IsKeyDownOnce())
		{
			int length = Enum.GetValues(typeof(HiSpeedFixType)).Length;
			Flags.HiSpeedFixType = (HiSpeedFixType)((int)(Flags.HiSpeedFixType + length - 1) % length);
		}
		else if (Program.KeyConfig.PlayScene.ChangeHSFix.IsKeyDownOnce())
		{
			Flags.HiSpeedFixType = (HiSpeedFixType)((int)(Flags.HiSpeedFixType + 1) % Enum.GetValues(typeof(HiSpeedFixType)).Length);
		}
		if (Program.KeyConfig.PlayScene.ChangePlaySide.IsKeyDownOnce())
		{
			Flags.PlaySide = (PlaySide)((int)(Flags.PlaySide + 1) % Enum.GetValues(typeof(PlaySide)).Length);
		}
		if (Program.KeyConfig.PlayScene.DisplayNoteCount.IsKeyDownOnce())
		{
			Flags.DisplayNoteCount = !Flags.DisplayNoteCount;
		}
		if (!Program.KeyConfig.PlayScene.LiftUp.IsKeyDown() || !Program.KeyConfig.PlayScene.LiftDown.IsKeyDown())
		{
			if (_keyRepeatManagers["LiftUp005"].IsKeyDownOnce())
			{
				SetLiftValue(5);
			}
			else if (_keyRepeatManagers["LiftUp10"].IsKeyDownOnce())
			{
				SetLiftValue(1000);
			}
			else if (_keyRepeatManagers["LiftUp01"].IsKeyDownOnce())
			{
				SetLiftValue(10);
			}
			else if (_keyRepeatManagers["LiftUp"].IsKeyDownOnce())
			{
				SetLiftValue(100);
			}
			else if (_keyRepeatManagers["LiftDown005"].IsKeyDownOnce())
			{
				SetLiftValue(-5);
			}
			else if (_keyRepeatManagers["LiftDown10"].IsKeyDownOnce())
			{
				SetLiftValue(-1000);
			}
			else if (_keyRepeatManagers["LiftDown01"].IsKeyDownOnce())
			{
				SetLiftValue(-10);
			}
			else if (_keyRepeatManagers["LiftDown"].IsKeyDownOnce())
			{
				SetLiftValue(-100);
			}
		}
		if (!Program.KeyConfig.PlayScene.LaneCoverUp.IsKeyDown() || !Program.KeyConfig.PlayScene.LaneCoverDown.IsKeyDown())
		{
			if (_keyRepeatManagers["LaneCoverUp005"].IsKeyDownOnce())
			{
				SetLaneCoverValue(-5);
			}
			else if (_keyRepeatManagers["LaneCoverUp10"].IsKeyDownOnce())
			{
				SetLaneCoverValue(-1000);
			}
			else if (_keyRepeatManagers["LaneCoverUp01"].IsKeyDownOnce())
			{
				SetLaneCoverValue(-10);
			}
			else if (_keyRepeatManagers["LaneCoverUp"].IsKeyDownOnce())
			{
				SetLaneCoverValue(-100);
			}
			else if (_keyRepeatManagers["LaneCoverDown005"].IsKeyDownOnce())
			{
				SetLaneCoverValue(5);
			}
			else if (_keyRepeatManagers["LaneCoverDown10"].IsKeyDownOnce())
			{
				SetLaneCoverValue(1000);
			}
			else if (_keyRepeatManagers["LaneCoverDown01"].IsKeyDownOnce())
			{
				SetLaneCoverValue(10);
			}
			else if (_keyRepeatManagers["LaneCoverDown"].IsKeyDownOnce())
			{
				SetLaneCoverValue(100);
			}
		}
		if (Program.KeyConfig.PlayScene.Player1LaneCoverToggle.IsKeyDownOnce())
		{
			Flags.Player1LaneCoverEnabled = !Flags.Player1LaneCoverEnabled;
			if (!Flags.IsPlayStyle7 && !Flags.IsPlayStyle5)
			{
				Flags.Player2LaneCoverEnabled = Flags.Player1LaneCoverEnabled;
			}
		}
		if (Program.KeyConfig.PlayScene.Player2LaneCoverToggle.IsKeyDownOnce())
		{
			Flags.Player2LaneCoverEnabled = !Flags.Player2LaneCoverEnabled;
			if (!Flags.IsPlayStyle7 && !Flags.IsPlayStyle5)
			{
				Flags.Player1LaneCoverEnabled = Flags.Player2LaneCoverEnabled;
			}
		}
		_skin.UpdateControls(EventTimers.NowTime);
		return false;
	}

	private void ChangeMusicSpeed(int value)
	{
		_prevMusicSpeedLevel = Numbers.MusicSpeedLevel;
		Numbers.MusicSpeedLevel = value;
		if (_prevMusicSpeedLevel != Numbers.MusicSpeedLevel)
		{
			StartBar = Numbers.CurrentBars;
			Restart();
		}
	}

	private static void SetLaneCoverValue(int value)
	{
		Numbers.Player1LaneCover += value;
		Numbers.Player2LaneCover = Numbers.Player1LaneCover;
		EventTimers.Player1LaneCoverChanged = EventTimers.NowTime;
		EventTimers.Player2LaneCoverChanged = EventTimers.NowTime;
	}

	private static void SetLiftValue(int value)
	{
		Numbers.Player1Lift += value;
		Numbers.Player2Lift = Numbers.Player1Lift;
		EventTimers.Player1LiftChanged = EventTimers.NowTime;
		EventTimers.Player2LiftChanged = EventTimers.NowTime;
	}

	private static void HiSpeedUp(double amount)
	{
		Numbers.Player1HiSpeed = (double)((int)(Numbers.Player1HiSpeed * 10000.0) + (int)(amount * 10000.0)) / 10000.0;
		Numbers.Player2HiSpeed = Numbers.Player1HiSpeed;
	}

	private static void HiSpeedDown(double amount)
	{
		Numbers.Player1HiSpeed = (double)((int)(Numbers.Player1HiSpeed * 10000.0) - (int)(amount * 10000.0)) / 10000.0;
		if (Numbers.Player1HiSpeed < 0.25)
		{
			Numbers.Player1HiSpeed = 0.25;
		}
		Numbers.Player2HiSpeed = Numbers.Player1HiSpeed;
	}

	private bool ProcessRemoteProxy()
	{
		if (Program.RemoteProxy != null)
		{
			if (!string.IsNullOrEmpty(Program.RemoteProxy.BmsPath))
			{
				if (File.Exists(Program.RemoteProxy.BmsPath) && Bms.CheckBmsFile(Program.RemoteProxy.BmsPath))
				{
					ClearResources();
					BmsPath = Program.RemoteProxy.BmsPath;
					StartBar = Program.RemoteProxy.StartBar;
					SetLaneSwapOptionsFromRemoteProxy();
					Program.RemoteProxy.Reset();
					LoadBms();
					Program.InvokeMainThread(() =>
					{
						Window.SetActive();
					});
					return true;
				}
			}
			else if (Program.RemoteProxy.Player1LaneSwapOption.HasValue || Program.RemoteProxy.Player2LaneSwapOption.HasValue || Program.RemoteProxy.FlipSide.HasValue)
			{
				SetLaneSwapOptionsFromRemoteProxy();
				StartBar = Program.RemoteProxy.StartBar;
				Program.RemoteProxy.Reset();
				Restart(swapLanes: true);
				Program.InvokeMainThread(() =>
				{
					Window.SetActive();
				});
				return false;
			}
			if (Program.RemoteProxy.IsStopRequested)
			{
				Program.RemoteProxy.Reset();
				Window.Exit();
				return true;
			}
			Program.RemoteProxy.Reset();
		}
		return false;
	}

	private void SetLaneSwapOptionsFromRemoteProxy()
	{
		if (Program.RemoteProxy.Player1LaneSwapOption.HasValue)
		{
			Program.InitialPlayer1LaneSwapOption = Program.RemoteProxy.Player1LaneSwapOption;
		}
		if (Program.RemoteProxy.Player2LaneSwapOption.HasValue)
		{
			Program.InitialPlayer2LaneSwapOption = Program.RemoteProxy.Player2LaneSwapOption;
		}
		if (Program.RemoteProxy.FlipSide.HasValue)
		{
			Program.InitialFlipSide = Program.RemoteProxy.FlipSide;
		}
		Flags.Player1LaneSwapOption = Program.InitialPlayer1LaneSwapOption ?? LaneSwapOption.None;
		Flags.Player2LaneSwapOption = Program.InitialPlayer2LaneSwapOption ?? LaneSwapOption.None;
		Flags.FlipSide = Program.InitialFlipSide ?? false;
	}

	public override void Update()
	{
		int prevKeyValue;
		if (_isBmsReady)
		{
			if (ProcessRemoteProxy())
			{
				return;
			}
			if (_dropFileList.Count > 0)
			{
				if (!Flags.Loading)
				{
					try
					{
						string fullPath = Path.GetFullPath(_dropFileList[0]);
						if (File.Exists(fullPath) && Bms.CheckBmsFile(fullPath))
						{
							ClearResources();
							BmsPath = fullPath;
							StartBar = 0;
							Flags.Player1LaneSwapOption = Program.InitialPlayer1LaneSwapOption ?? LaneSwapOption.None;
							Flags.Player2LaneSwapOption = Program.InitialPlayer2LaneSwapOption ?? LaneSwapOption.None;
							Flags.FlipSide = Program.InitialFlipSide ?? false;
							LoadBms();
							_dropFileList.Clear();
							Program.InvokeMainThread(() =>
							{
								Window.SetActive();
							});
							return;
						}
					}
					catch (Exception)
					{
					}
				}
				_dropFileList.ForEach((string s) =>
				{
				});
				_dropFileList.Clear();
			}
			if (!Flags.Loading && !Flags.LoadComplete)
			{
				LoadBmsResources();
			}
			else if (Flags.Loading && Flags.LoadComplete)
			{
				InitBarInfo();
				GC.Collect();
				Flags.Loading = false;
				EventTimers.LoadComplete = EventTimers.NowTime;
				EventTimers.LoadStart = 0.0;
				_timeTable = new BmsTimeTable(_baseTimeTable);
				Numbers.Bpm = _barInfo[StartBar].Bpm;
				_nowBga = _barInfo[StartBar].Bga;
				_nowLayer = _barInfo[StartBar].Layer;
				_nowPoor = _barInfo[StartBar].Poor;
				if (_hasPoorBga && _nowPoor == "")
				{
					_nowPoor = "00";
				}
				_prevBga = "*";
				_prevLayer = "*";
				_prevPoor = "*";
				_startLNHolding.AddRange(_barInfo[StartBar].LNHolding);
				Program.InvokeMainThreadAndWait(() =>
				{
					lock (Program.GraphicsLockObj)
					{
						_bgaTexture = Graphics.CreateDynamicTexture(_maxBgaWidth, _maxBgaHeight);
						((IDynamicTextureInfo)_bgaTexture).Clear();
						_layerTexture = Graphics.CreateDynamicTexture(_maxBgaWidth, _maxBgaHeight);
						((IDynamicTextureInfo)_layerTexture).Clear();
						_poorTexture = Graphics.CreateDynamicTexture(_maxBgaWidth, _maxBgaHeight);
						((IDynamicTextureInfo)_poorTexture).Clear();
					}
					Window.Title = PlatformGeneral.GetPlayingWindowTitle();
				});
				EventTimers.UpdateNowTime();
				EventTimers.PlayStart = ((_baseTimeTable.Rows.Count > _baseTimeTable.BarIndex[StartBar]) ? (EventTimers.NowTime - _baseTimeTable.Rows[_baseTimeTable.BarIndex[StartBar]].Time) : EventTimers.NowTime);
				EventTimers.ResetBpmTimer();
			}
			if (EnableInput && ProcessKeyInput())
			{
				return;
			}
			if (!Flags.Loading && Flags.LoadComplete)
			{
				double num = EventTimers.NowTime - EventTimers.PlayStart;
				double num2 = num * Numbers.MusicSpeed;
				Numbers.ElapsedMusicTime = ((num2 < _totalMusicTime) ? TimeSpan.FromMilliseconds(num2) : Numbers.TotalMusicTime);
				double num3 = Math.Abs(Numbers.Bpm);
				double num4 = 0.0;
				double num5 = 0.0;
				double num6 = 0.0;
				double num7 = 0.0;
				double num8 = 0.0;
				double num9 = 0.0;
				double num10 = 0.0;
				bool flag = false;
				bool flag2 = false;
				double num11 = Flags.HiSpeedFixType switch
				{
					HiSpeedFixType.Max => 150.0 / _timeTable.MaxNotesBpm, 
					HiSpeedFixType.Min => 150.0 / _timeTable.MinNotesBpm, 
					HiSpeedFixType.MainNotes => 150.0 / _timeTable.MainNotesBpm, 
					HiSpeedFixType.MainTime => 150.0 / _timeTable.MainTimeBpm, 
					_ => 1.0, 
				};
				List<List<Rectangle>> list = new List<List<Rectangle>>();
				for (int num12 = 0; num12 < _timeTable.LaneCount; num12++)
				{
					_notePos[num12].Clear();
					List<Rectangle> laneDestinations = _skin.GetLaneDestinations(num12, EventTimers.NowTime);
					foreach (Rectangle item in laneDestinations)
					{
						_ = item;
						_notePos[num12].Add(new NotePositionCollection());
					}
					list.Add(laneDestinations);
				}
				prevKeyValue = -1;
				List<Rectangle> barlineDestinations = _skin.GetBarlineDestinations(1, EventTimers.NowTime);
				List<Rectangle> barlineDestinations2 = _skin.GetBarlineDestinations(2, EventTimers.NowTime);
				_barlinePos1.Clear();
				foreach (Rectangle item2 in barlineDestinations)
				{
					_ = item2;
					_barlinePos1.Add(new BarlinePositionCollection());
				}
				_barlinePos2.Clear();
				foreach (Rectangle item3 in barlineDestinations2)
				{
					_ = item3;
					_barlinePos2.Add(new BarlinePositionCollection());
				}
				Dictionary<int, List<NotePosition>> dictionary = new Dictionary<int, List<NotePosition>>();
				foreach (int item4 in _startLNHolding)
				{
					if (Numbers.Bpm > 0.0)
					{
						if (!_startLNHoldingProcessed)
						{
							if (Flags.AutoPlay)
							{
								AutoPlayKeyState.SetKeyDown(KeyList[item4]);
								Flags.LNHolding[item4] = true;
								Flags.LNReleased[item4] = false;
								EventTimers.LaneLNHolding[item4] = EventTimers.NowTime;
								if (!Flags.LegacyLN)
								{
									EventTimers.SetLNJudge(item4);
									Flags.SetJudge(item4, JudgeType.PGreat);
									Flags.SetLastJudged(item4);
									double grooveGauge = Numbers.GrooveGauge;
									int grooveGaugeDivideCount = Numbers.GrooveGaugeDivideCount;
									Numbers.GrooveGauge += _gaugeIncrements[JudgeType.PGreat];
									if (grooveGauge < Numbers.GrooveGauge && Numbers.GrooveGauge >= 100.0)
									{
										EventTimers.GrooveGaugeIncrease = EventTimers.NowTime;
										EventTimers.GrooveGaugeMax = EventTimers.NowTime;
									}
									else if (grooveGaugeDivideCount < Numbers.GrooveGaugeDivideCount)
									{
										EventTimers.GrooveGaugeIncrease = EventTimers.NowTime;
									}
									Numbers.GaugeTotalIncrement += Numbers.GaugeTotal / (double)Numbers.TotalNotes;
									Numbers.SetJudge(item4, JudgeType.PGreat, 0.0);
								}
							}
							else
							{
								Flags.LNHolding[item4] = false;
								Flags.LNReleased[item4] = true;
								EventTimers.LaneLNHolding[item4] = 0.0;
								bool flag3 = false;
								for (int num13 = _timeTable.BarIndex[StartBar]; num13 < _timeTable.Rows.Count; num13++)
								{
									for (int num14 = 0; num14 < _timeTable.Rows[num13].Lanes[item4].Count; num14++)
									{
										BmsObjectType objectType = _timeTable.Rows[num13].Lanes[item4][num14].ObjectType;
										if (objectType == BmsObjectType.LNEnd)
										{
											_timeTable.Rows[num13].Lanes[item4][num14].IsJudged = true;
											flag3 = true;
										}
										if (flag3)
										{
											break;
										}
									}
									if (flag3)
									{
										break;
									}
								}
								EventTimers.PoorBga = EventTimers.NowTime;
								EventTimers.SetJudge(item4, JudgeType.PassedPoor);
								Flags.SetJudge(item4, JudgeType.PassedPoor);
								Flags.SetLastJudged(item4);
								Numbers.SetJudge(item4, JudgeType.PassedPoor, 0.0);
								if (!Flags.LegacyLN)
								{
									Numbers.SetJudge(item4, JudgeType.PassedPoor, 0.0);
								}
							}
						}
						dictionary.Add(item4, new List<NotePosition>());
						for (int num15 = 0; num15 < list[item4].Count; num15++)
						{
							dictionary[item4].Add(new NotePosition(BmsObjectType.LNStart, list[item4][num15].X, list[item4][num15].Y, Flags.AutoPlay ? NoteStatus.LNHolding : NoteStatus.LNReleased));
						}
						continue;
					}
					if (!_startLNHoldingProcessed)
					{
						if (Flags.AutoPlay)
						{
							AutoPlayKeyState.SetKeyUp(KeyList[item4]);
						}
						Flags.LNHolding[item4] = false;
						Flags.LNReleased[item4] = false;
						EventTimers.LaneLNHolding[item4] = 0.0;
					}
					double num16 = _timeTable.Rows[_timeTable.BarIndex[StartBar]].Time;
					if (num16 < _reverseTime)
					{
						num16 = _reverseTime;
					}
					int num17 = 0;
					num17 = ((item4 < 10) ? GetLanePosition(Numbers.Player1HiSpeed, num11, num3, _reverseTime - (num - _reverseTime), num16) : GetLanePosition(Numbers.Player2HiSpeed, num11, num3, _reverseTime - (num - _reverseTime), num16));
					if (num17 < 0)
					{
						num17 = 0;
					}
					dictionary.Add(item4, new List<NotePosition>());
					for (int num18 = 0; num18 < list[item4].Count; num18++)
					{
						dictionary[item4].Add(new NotePosition(BmsObjectType.LNStart, list[item4][num18].X, list[item4][num18].Y - num17, NoteStatus.Normal));
					}
				}
				_startLNHoldingProcessed = true;
				if (!_isStartBarProcessed)
				{
					foreach (KeyValuePair<string, double> item5 in _barInfo[StartBar].Bgm)
					{
						if (_keySounds.ContainsKey(item5.Key))
						{
							_keySounds[item5.Key].Volume = (Flags.IsBgmVolumeMute ? 0f : ((float)Numbers.BgmVolume / 100f));
							if (item5.Value / 1000.0 < _keySounds[item5.Key].TotalTime && (!Flags.IsBgmVolumeMute || Numbers.BgmVolume > 0))
							{
								_keySounds[item5.Key].Play(_keySounds[item5.Key].GetPositionFromSeconds(item5.Value / 1000.0));
							}
						}
					}
					if (Flags.AutoPlay)
					{
						foreach (KeyValuePair<string, double> key2 in _barInfo[StartBar].Keys)
						{
							if (_keySounds.ContainsKey(key2.Key))
							{
								_keySounds[key2.Key].Volume = (Flags.IsKeyVolumeMute ? 0f : ((float)Numbers.KeyVolume / 100f));
								if (key2.Value / 1000.0 < _keySounds[key2.Key].TotalTime && (!Flags.IsKeyVolumeMute || Numbers.KeyVolume > 0))
								{
									_keySounds[key2.Key].Play(_keySounds[key2.Key].GetPositionFromSeconds(key2.Value / 1000.0));
								}
							}
						}
					}
					_isStartBarProcessed = true;
				}
				if (!Flags.AutoPlay)
				{
					List<int> list2 = new List<int>(_laneIndex);
					List<int> list3 = new List<int>();
					int num19 = _timeTable.BarIndex[StartBar];
					List<(int, int)> list4 = new List<(int, int)>(_timeTable.LaneCount);
					for (int num20 = 0; num20 < _timeTable.LaneCount; num20++)
					{
						list4.Add((-1, -1));
					}
					foreach (int item6 in list2)
					{
						bool flag4 = false;
						int num21 = -1;
						int num22 = -1;
						int num23 = -1;
						int index = -1;
						bool flag5 = false;
						double num24 = ((item6 == 0 || item6 == 10) ? _judgeRange.ScratchRangeScale : 1.0);
						for (int num25 = num19; num25 < _timeTable.Rows.Count && (!(_reverseTime > 0.0) || !(_timeTable.Rows[num25].Time > _reverseTime + Numbers.NoteTiming)); num25++)
						{
							for (int num26 = 0; num26 < _timeTable.Rows[num25].Lanes[item6].Count; num26++)
							{
								BmsLaneTime bmsLaneTime = _timeTable.Rows[num25].Lanes[item6][num26];
								switch (bmsLaneTime.ObjectType)
								{
								case BmsObjectType.Note:
								case BmsObjectType.LNStart:
								{
									double num28 = bmsLaneTime.Time - num - Numbers.JudgeTiming;
									double num29 = _judgeRange.Good * num24 * -1.0;
									if (num21 < 0)
									{
										num21 = num25;
										num22 = num26;
										num23 = num25;
										index = num26;
										if (!bmsLaneTime.IsJudged)
										{
											if (bmsLaneTime.ObjectType == BmsObjectType.LNStart || num28 >= num29)
											{
												flag4 = true;
											}
											else if (num28 < num29)
											{
												flag5 = true;
											}
										}
										break;
									}
									BmsLaneTime bmsLaneTime3 = _timeTable.Rows[num21].Lanes[item6][num22];
									if (!((bmsLaneTime3.IsJudged || bmsLaneTime3.ObjectType == BmsObjectType.InvisibleNote) | flag5))
									{
										break;
									}
									if (!bmsLaneTime.IsJudged)
									{
										if (flag5)
										{
											if (Math.Abs(num28) <= _judgeRange.Good * num24 || num28 < num29)
											{
												if (bmsLaneTime3.ObjectType != BmsObjectType.InvisibleNote || num28 <= _judgeRange.Bad)
												{
													num23 = num25;
													index = num26;
												}
												num21 = num25;
												num22 = num26;
												if (bmsLaneTime.ObjectType == BmsObjectType.LNStart || num28 >= num29)
												{
													flag4 = true;
												}
												else if (num28 < num29)
												{
													flag5 = true;
												}
											}
											else
											{
												flag4 = true;
											}
										}
										else if (num28 <= _judgeRange.FastPoor)
										{
											if (bmsLaneTime3.ObjectType != BmsObjectType.InvisibleNote || num28 <= _judgeRange.Bad)
											{
												num23 = num25;
												index = num26;
											}
											num21 = num25;
											num22 = num26;
											if (bmsLaneTime.ObjectType == BmsObjectType.LNStart || num28 >= num29)
											{
												flag4 = true;
											}
											else if (num28 < num29)
											{
												flag5 = true;
											}
										}
										else
										{
											flag4 = true;
										}
									}
									else if (flag5)
									{
										flag4 = true;
									}
									else
									{
										num21 = num25;
										num22 = num26;
										num23 = num25;
										index = num26;
									}
									break;
								}
								case BmsObjectType.InvisibleNote:
								{
									if (num21 < 0)
									{
										num21 = num25;
										num22 = num26;
										num23 = num25;
										index = num26;
										break;
									}
									BmsLaneTime bmsLaneTime2 = _timeTable.Rows[num21].Lanes[item6][num22];
									double num27 = bmsLaneTime.Time - num - Numbers.JudgeTiming;
									if ((bmsLaneTime2.IsJudged || bmsLaneTime2.ObjectType == BmsObjectType.InvisibleNote) && num27 <= _judgeRange.FastPoor)
									{
										if (bmsLaneTime2.ObjectType != BmsObjectType.InvisibleNote || bmsLaneTime2.Time - num - Numbers.JudgeTiming < 0.0)
										{
											num23 = num25;
											index = num26;
										}
										num21 = num25;
										num22 = num26;
									}
									break;
								}
								case BmsObjectType.LNEnd:
									if (num21 < 0)
									{
										num21 = num25;
										num22 = num26;
										num23 = num25;
										index = num26;
									}
									else if (_timeTable.Rows[num21].Lanes[item6][num22].ObjectType == BmsObjectType.LNStart && Flags.LNHolding[item6])
									{
										num21 = num25;
										num22 = num26;
										num23 = num25;
										index = num26;
										flag4 = true;
									}
									break;
								}
								if (flag4)
								{
									break;
								}
							}
							if (flag4)
							{
								break;
							}
						}
						if (num21 < 0)
						{
							continue;
						}
						if (num23 < 0)
						{
							num23 = num21;
							index = num22;
						}
						if (_timeTable.Rows[num21].Lanes[item6][num22].ObjectType != BmsObjectType.LNEnd)
						{
							double num30 = _timeTable.Rows[num21].Time - num - Numbers.JudgeTiming;
							if (_lastKeySounds[item6] == "" || num30 <= _judgeRange.FastPoor)
							{
								_lastKeySounds[item6] = _timeTable.Rows[num23].Lanes[item6][index].Value;
							}
						}
						if (_timeTable.Rows[num21].Lanes[item6][num22].ObjectType != BmsObjectType.InvisibleNote)
						{
							list4[item6] = (num21, num22);
						}
						list3.Add(item6);
					}
					_laneIndex = list3;
					for (int num31 = num19; num31 < _timeTable.Rows.Count && (!(_reverseTime > 0.0) || !(_timeTable.Rows[num31].Time > _reverseTime + Numbers.NoteTiming)); num31++)
					{
						double num32 = _timeTable.Rows[num31].Time - num - Numbers.JudgeTiming;
						if (num32 >= _judgeRange.Bad * -1.0 && (!Flags.LegacyLN || num32 > 0.0))
						{
							break;
						}
						JudgeType judgeType = JudgeType.None;
						for (int num33 = 0; num33 < _timeTable.Rows[num31].Lanes.Count; num33++)
						{
							if ((!Flags.LegacyLN || !Flags.LNHolding[num33]) && num32 >= _judgeRange.Bad * -1.0)
							{
								continue;
							}
							for (int num34 = 0; num34 < _timeTable.Rows[num31].Lanes[num33].Count; num34++)
							{
								switch (_timeTable.Rows[num31].Lanes[num33][num34].ObjectType)
								{
								case BmsObjectType.Note:
									if (!_timeTable.Rows[num31].Lanes[num33][num34].IsJudged && num32 < _judgeRange.Bad * -1.0)
									{
										judgeType = JudgeType.PassedPoor;
										EventTimers.PoorBga = EventTimers.NowTime;
										EventTimers.SetJudge(num33, judgeType);
										Flags.SetJudge(num33, judgeType);
										SetLastJudged(num33, _timeTable.Rows[num31].Lanes[num33][num34].Value, judgeType);
										Numbers.SetJudge(num33, judgeType, num32);
										Numbers.GrooveGauge += _gaugeIncrements[judgeType];
										if (Numbers.GrooveGauge <= 2.0)
										{
											Numbers.GrooveGauge = 2.0;
										}
										_timeTable.Rows[num31].Lanes[num33][num34].IsJudged = true;
									}
									break;
								case BmsObjectType.LNStart:
								{
									if (_timeTable.Rows[num31].Lanes[num33][num34].IsJudged || !(num32 < _judgeRange.Bad * -1.0))
									{
										break;
									}
									judgeType = JudgeType.PassedPoor;
									EventTimers.PoorBga = EventTimers.NowTime;
									Flags.LNHolding[num33] = false;
									Flags.LNReleased[num33] = true;
									EventTimers.LaneLNHolding[num33] = 0.0;
									bool flag6 = false;
									for (int num36 = num31; num36 < _timeTable.Rows.Count; num36++)
									{
										for (int num37 = 0; num37 < _timeTable.Rows[num36].Lanes[num33].Count; num37++)
										{
											if (_timeTable.Rows[num36].Lanes[num33][num37].ObjectType == BmsObjectType.LNEnd)
											{
												_timeTable.Rows[num36].Lanes[num33][num37].IsJudged = true;
												if (num33 == _lastIndex)
												{
													_lastCount++;
												}
												flag6 = true;
												break;
											}
										}
										if (flag6)
										{
											break;
										}
									}
									EventTimers.SetJudge(num33, judgeType);
									Flags.SetJudge(num33, judgeType);
									SetLastJudged(num33, _timeTable.Rows[num31].Lanes[num33][num34].Value, judgeType);
									Numbers.SetJudge(num33, judgeType, num32);
									if (!Flags.LegacyLN)
									{
										Numbers.SetJudge(num33, judgeType, num32);
									}
									Numbers.GrooveGauge += (Flags.LegacyLN ? _gaugeIncrements[judgeType] : (_gaugeIncrements[judgeType] * 2.0));
									if (Numbers.GrooveGauge <= 2.0)
									{
										Numbers.GrooveGauge = 2.0;
									}
									_timeTable.Rows[num31].Lanes[num33][num34].IsJudged = true;
									break;
								}
								case BmsObjectType.LNEnd:
								{
									if (_timeTable.Rows[num31].Lanes[num33][num34].IsJudged || !Flags.LNHolding[num33])
									{
										break;
									}
									if (Flags.LegacyLN)
									{
										if (num32 <= 0.0)
										{
											judgeType = _lnJudge[num33];
											double time = _lnJudgeTime[num33];
											Flags.LNHolding[num33] = false;
											Flags.LNReleased[num33] = true;
											Flags.LNHoldOnce[num33] = false;
											EventTimers.LaneLNHolding[num33] = 0.0;
											EventTimers.SetJudge(num33, judgeType);
											Flags.SetJudge(num33, judgeType);
											SetLastJudged(num33, _timeTable.Rows[num31].Lanes[num33][num34].Value, judgeType);
											Numbers.SetJudge(num33, judgeType, time);
											double grooveGauge2 = Numbers.GrooveGauge;
											int grooveGaugeDivideCount2 = Numbers.GrooveGaugeDivideCount;
											Numbers.GrooveGauge += _gaugeIncrements[judgeType];
											if (grooveGauge2 < Numbers.GrooveGauge && Numbers.GrooveGauge >= 100.0)
											{
												EventTimers.GrooveGaugeIncrease = EventTimers.NowTime;
												EventTimers.GrooveGaugeMax = EventTimers.NowTime;
											}
											else if (grooveGaugeDivideCount2 < Numbers.GrooveGaugeDivideCount)
											{
												EventTimers.GrooveGaugeIncrease = EventTimers.NowTime;
											}
											Numbers.GaugeTotalIncrement += _gaugeIncrements[judgeType];
											if (num33 == _lastIndex)
											{
												_lastCount++;
											}
											_timeTable.Rows[num31].Lanes[num33][num34].IsJudged = true;
										}
										break;
									}
									double num35 = ((num33 == 0 || num33 == 10) ? Math.Max(_judgeRange.ScratchRangeScale, _judgeRange.LNEndRangeScale) : _judgeRange.LNEndRangeScale);
									if (num32 < _judgeRange.Good * num35 * -1.0)
									{
										judgeType = JudgeType.Bad;
										EventTimers.PoorBga = EventTimers.NowTime;
										Flags.LNHolding[num33] = false;
										Flags.LNReleased[num33] = true;
										Flags.LNHoldOnce[num33] = false;
										EventTimers.LaneLNHolding[num33] = 0.0;
										EventTimers.SetJudge(num33, judgeType);
										Flags.SetJudge(num33, judgeType);
										SetLastJudged(num33, _timeTable.Rows[num31].Lanes[num33][num34].Value, judgeType);
										Numbers.SetJudge(num33, judgeType, num32);
										Numbers.GrooveGauge += _gaugeIncrements[judgeType];
										if (Numbers.GrooveGauge <= 2.0)
										{
											Numbers.GrooveGauge = 2.0;
										}
										if (!Flags.LegacyLN)
										{
											Numbers.NotesPerSecondCount++;
										}
										if (num33 == _lastIndex)
										{
											_lastCount++;
										}
										_timeTable.Rows[num31].Lanes[num33][num34].IsJudged = true;
									}
									break;
								}
								}
							}
						}
					}
					for (int num38 = 0; num38 < _timeTable.LaneCount; num38++)
					{
						(int, int) tuple = list4[num38];
						int num39 = tuple.Item1;
						int index2 = tuple.Item2;
						JudgeType judgeType2 = JudgeType.None;
						double num40 = ((num38 == 0 || num38 == 10) ? _judgeRange.ScratchRangeScale : 1.0);
						bool flag7;
						if (KeyList[num38] == PlayingKeys.Player1ScratchToggle)
						{
							flag7 = Flags.IsPlayer1ScratchDownOnce;
						}
						else
						{
							flag7 = ((KeyList[num38] != PlayingKeys.Player2ScratchToggle) ? Flags.IsKeyDownOnce(KeyList[num38]) : Flags.IsPlayer2ScratchDownOnce);
						}
						bool flag8;
						if (KeyList[num38] == PlayingKeys.Player1ScratchToggle)
						{
							flag8 = Flags.IsPlayer1ScratchUpOnce;
						}
						else
						{
							flag8 = ((KeyList[num38] != PlayingKeys.Player2ScratchToggle) ? Flags.IsKeyUpOnce(KeyList[num38]) : Flags.IsPlayer2ScratchUpOnce);
						}
						if (flag8 && num39 >= 0)
						{
							double num41 = ((num38 == 0 || num38 == 10) ? Math.Max(_judgeRange.ScratchRangeScale, _judgeRange.LNEndRangeScale) : _judgeRange.LNEndRangeScale);
							double num42 = _timeTable.Rows[num39].Time - num - Numbers.JudgeTiming;
							BmsLaneTime bmsLaneTime4 = _timeTable.Rows[num39].Lanes[num38][index2];
							BmsObjectType objectType = bmsLaneTime4.ObjectType;
							if (objectType == BmsObjectType.LNEnd && !bmsLaneTime4.IsJudged && Flags.LNHolding[num38] && num42 >= _judgeRange.Good * num41 * -1.0)
							{
								if (Math.Abs(num42) <= _judgeRange.PGreat * num41)
								{
									judgeType2 = JudgeType.PGreat;
								}
								else if (Math.Abs(num42) <= _judgeRange.Great * num41)
								{
									judgeType2 = JudgeType.Great;
								}
								else
								{
									judgeType2 = ((!(Math.Abs(num42) <= _judgeRange.Good * num41)) ? JudgeType.Bad : JudgeType.Good);
								}
								double time2 = num42;
								if (Flags.LegacyLN && judgeType2 > _lnJudge[num38])
								{
									judgeType2 = _lnJudge[num38];
									time2 = _lnJudgeTime[num38];
								}
								if (judgeType2 <= JudgeType.Bad)
								{
									EventTimers.PoorBga = EventTimers.NowTime;
								}
								Flags.LNHolding[num38] = false;
								Flags.LNReleased[num38] = true;
								EventTimers.LaneLNHolding[num38] = 0.0;
								EventTimers.SetJudge(num38, judgeType2);
								Flags.SetJudge(num38, judgeType2);
								SetLastJudged(num38, bmsLaneTime4.Value, judgeType2);
								Numbers.SetJudge(num38, judgeType2, time2);
								double grooveGauge3 = Numbers.GrooveGauge;
								int grooveGaugeDivideCount3 = Numbers.GrooveGaugeDivideCount;
								if (judgeType2 >= JudgeType.Good)
								{
									Numbers.GrooveGauge += _gaugeIncrements[judgeType2];
									if (grooveGauge3 < Numbers.GrooveGauge && Numbers.GrooveGauge >= 100.0)
									{
										EventTimers.GrooveGaugeIncrease = EventTimers.NowTime;
										EventTimers.GrooveGaugeMax = EventTimers.NowTime;
									}
									else if (grooveGaugeDivideCount3 < Numbers.GrooveGaugeDivideCount)
									{
										EventTimers.GrooveGaugeIncrease = EventTimers.NowTime;
									}
									Numbers.GaugeTotalIncrement += _gaugeIncrements[judgeType2];
								}
								else
								{
									Numbers.GrooveGauge += _gaugeIncrements[judgeType2];
									if (Numbers.GrooveGauge <= 2.0)
									{
										Numbers.GrooveGauge = 2.0;
									}
								}
								if (!Flags.LegacyLN)
								{
									Numbers.NotesPerSecondCount++;
								}
								if (num38 == _lastIndex)
								{
									_lastCount++;
								}
								bmsLaneTime4.IsJudged = true;
								if (judgeType2 <= JudgeType.Bad && _keySounds.ContainsKey(_lastKeySounds[num38]))
								{
									_keySounds[_lastKeySounds[num38]].Stop();
								}
								if (Flags.LegacyLN)
								{
									double num43 = _judgeRange.Good * num40 * -1.0;
									bool flag9 = false;
									for (int num44 = num39; num44 < _timeTable.Rows.Count && (!(_reverseTime > 0.0) || !(_timeTable.Rows[num44].Time > _reverseTime + Numbers.NoteTiming)); num44++)
									{
										double num45 = _timeTable.Rows[num44].Time - num - Numbers.JudgeTiming;
										if (num45 < num43)
										{
											break;
										}
										if (!(num45 < _judgeRange.Good * num40))
										{
											continue;
										}
										for (int num46 = 0; num46 < _timeTable.Rows[num44].Lanes[num38].Count; num46++)
										{
											objectType = _timeTable.Rows[num44].Lanes[num38][num46].ObjectType;
											if ((uint)objectType <= 1u)
											{
												num39 = num44;
												index2 = num46;
												_lastKeySounds[num38] = _timeTable.Rows[num44].Lanes[num38][num46].Value;
												flag9 = true;
											}
											if (flag9)
											{
												break;
											}
										}
										if (flag9)
										{
											break;
										}
									}
									if (!flag9)
									{
										flag7 = false;
									}
								}
								else
								{
									flag7 = false;
								}
							}
						}
						if (!flag7)
						{
							continue;
						}
						if (num39 >= 0)
						{
							double num47 = _timeTable.Rows[num39].Time - num - Numbers.JudgeTiming;
							BmsLaneTime bmsLaneTime5 = _timeTable.Rows[num39].Lanes[num38][index2];
							switch (bmsLaneTime5.ObjectType)
							{
							case BmsObjectType.Note:
								if (Math.Abs(num47) <= _judgeRange.Bad || num47 <= _judgeRange.FastPoor)
								{
									if (!bmsLaneTime5.IsJudged)
									{
										if (Math.Abs(num47) <= _judgeRange.PGreat * num40)
										{
											judgeType2 = JudgeType.PGreat;
										}
										else if (Math.Abs(num47) <= _judgeRange.Great * num40)
										{
											judgeType2 = JudgeType.Great;
										}
										else
										{
											judgeType2 = ((Math.Abs(num47) <= _judgeRange.Good * num40) ? JudgeType.Good : ((Math.Abs(num47) <= _judgeRange.Bad) ? JudgeType.Bad : JudgeType.WastedPoor));
										}
									}
									else if (num47 >= _judgeRange.SlowPoor)
									{
										judgeType2 = JudgeType.WastedPoor;
									}
									if (judgeType2 == JudgeType.None)
									{
										Flags.SetJudge(num38, judgeType2);
										break;
									}
									if (judgeType2 <= JudgeType.Bad)
									{
										EventTimers.PoorBga = EventTimers.NowTime;
									}
									Flags.LNHolding[num38] = false;
									Flags.LNReleased[num38] = false;
									Flags.LNHoldOnce[num38] = false;
									EventTimers.LaneLNHolding[num38] = 0.0;
									EventTimers.SetJudge(num38, judgeType2);
									Numbers.SetJudge(num38, judgeType2, num47);
									Flags.SetJudge(num38, judgeType2);
									SetLastJudged(num38, bmsLaneTime5.Value, judgeType2);
									double grooveGauge5 = Numbers.GrooveGauge;
									int grooveGaugeDivideCount5 = Numbers.GrooveGaugeDivideCount;
									if (judgeType2 >= JudgeType.Good)
									{
										Numbers.GrooveGauge += _gaugeIncrements[judgeType2];
										if (grooveGauge5 < Numbers.GrooveGauge && Numbers.GrooveGauge >= 100.0)
										{
											EventTimers.GrooveGaugeIncrease = EventTimers.NowTime;
											EventTimers.GrooveGaugeMax = EventTimers.NowTime;
										}
										else if (grooveGaugeDivideCount5 < Numbers.GrooveGaugeDivideCount)
										{
											EventTimers.GrooveGaugeIncrease = EventTimers.NowTime;
										}
										Numbers.GaugeTotalIncrement += _gaugeIncrements[judgeType2];
										Numbers.NotesPerSecondCount++;
									}
									else
									{
										Numbers.GrooveGauge += _gaugeIncrements[judgeType2];
										if (Numbers.GrooveGauge <= 2.0)
										{
											Numbers.GrooveGauge = 2.0;
										}
									}
									if (judgeType2 != JudgeType.WastedPoor)
									{
										bmsLaneTime5.IsJudged = true;
									}
								}
								else
								{
									Flags.SetJudge(num38, JudgeType.None);
								}
								break;
							case BmsObjectType.LNStart:
								if (!(Numbers.Bpm > 0.0))
								{
									break;
								}
								if (Math.Abs(num47) <= _judgeRange.Bad || num47 <= _judgeRange.FastPoor)
								{
									if (!bmsLaneTime5.IsJudged)
									{
										if (Math.Abs(num47) <= _judgeRange.PGreat * num40)
										{
											judgeType2 = JudgeType.PGreat;
										}
										else if (Math.Abs(num47) <= _judgeRange.Great * num40)
										{
											judgeType2 = JudgeType.Great;
										}
										else
										{
											judgeType2 = ((Math.Abs(num47) <= _judgeRange.Good * num40) ? JudgeType.Good : ((Math.Abs(num47) <= _judgeRange.Bad) ? JudgeType.Bad : JudgeType.WastedPoor));
										}
									}
									if (judgeType2 == JudgeType.None)
									{
										Flags.SetJudge(num38, judgeType2);
										Flags.LNHoldOnce[num38] = false;
										break;
									}
									if (judgeType2 <= JudgeType.Bad)
									{
										EventTimers.PoorBga = EventTimers.NowTime;
									}
									switch (judgeType2)
									{
									case JudgeType.Bad:
									{
										Flags.LNHolding[num38] = false;
										Flags.LNReleased[num38] = true;
										Flags.LNHoldOnce[num38] = false;
										EventTimers.LaneLNHolding[num38] = 0.0;
										bool flag10 = false;
										for (int num48 = num39; num48 < _timeTable.Rows.Count; num48++)
										{
											for (int num49 = 0; num49 < _timeTable.Rows[num48].Lanes[num38].Count; num49++)
											{
												if (_timeTable.Rows[num48].Lanes[num38][num49].ObjectType == BmsObjectType.LNEnd)
												{
													_timeTable.Rows[num48].Lanes[num38][num49].IsJudged = true;
													flag10 = true;
													break;
												}
											}
											if (flag10)
											{
												break;
											}
										}
										break;
									}
									case JudgeType.WastedPoor:
										Flags.LNHolding[num38] = false;
										Flags.LNReleased[num38] = false;
										Flags.LNHoldOnce[num38] = false;
										EventTimers.LaneLNHolding[num38] = 0.0;
										break;
									default:
										Flags.LNHolding[num38] = true;
										Flags.LNReleased[num38] = false;
										Flags.LNHoldOnce[num38] = true;
										EventTimers.LaneLNHolding[num38] = EventTimers.NowTime;
										break;
									}
									Numbers.NotesPerSecondCount++;
									if (EventTimers.FirstNoteJudge <= 0.0)
									{
										EventTimers.FirstNoteJudge = EventTimers.NowTime;
									}
									if (!Flags.LegacyLN)
									{
										if (judgeType2 <= JudgeType.Bad)
										{
											EventTimers.SetJudge(num38, judgeType2);
										}
										else
										{
											EventTimers.SetLNJudge(num38);
										}
										Flags.SetJudge(num38, judgeType2);
										SetLastJudged(num38, bmsLaneTime5.Value, judgeType2);
										Numbers.SetJudge(num38, judgeType2, num47);
										double grooveGauge4 = Numbers.GrooveGauge;
										int grooveGaugeDivideCount4 = Numbers.GrooveGaugeDivideCount;
										if (judgeType2 >= JudgeType.Good)
										{
											Numbers.GrooveGauge += _gaugeIncrements[judgeType2];
											if (grooveGauge4 < Numbers.GrooveGauge && Numbers.GrooveGauge >= 100.0)
											{
												EventTimers.GrooveGaugeIncrease = EventTimers.NowTime;
												EventTimers.GrooveGaugeMax = EventTimers.NowTime;
											}
											else if (grooveGaugeDivideCount4 < Numbers.GrooveGaugeDivideCount)
											{
												EventTimers.GrooveGaugeIncrease = EventTimers.NowTime;
											}
											Numbers.GaugeTotalIncrement += _gaugeIncrements[judgeType2];
										}
										else
										{
											Numbers.GrooveGauge += ((judgeType2 == JudgeType.Bad) ? (_gaugeIncrements[JudgeType.Bad] + _gaugeIncrements[JudgeType.PassedPoor]) : _gaugeIncrements[judgeType2]);
											if (Numbers.GrooveGauge <= 2.0)
											{
												Numbers.GrooveGauge = 2.0;
											}
											if (judgeType2 == JudgeType.Bad)
											{
												Numbers.SetJudge(num38, JudgeType.PassedPoor, num47);
											}
										}
									}
									else if (judgeType2 <= JudgeType.Bad)
									{
										EventTimers.SetJudge(num38, judgeType2);
										Flags.SetJudge(num38, judgeType2);
										SetLastJudged(num38, bmsLaneTime5.Value, judgeType2);
										Numbers.SetJudge(num38, judgeType2, num47);
										_ = Numbers.GrooveGauge;
										_ = Numbers.GrooveGaugeDivideCount;
										Numbers.GrooveGauge += _gaugeIncrements[judgeType2];
										if (Numbers.GrooveGauge <= 2.0)
										{
											Numbers.GrooveGauge = 2.0;
										}
									}
									else
									{
										_lnJudge[num38] = judgeType2;
										_lnJudgeTime[num38] = num47;
									}
									if (judgeType2 != JudgeType.WastedPoor)
									{
										bmsLaneTime5.IsJudged = true;
									}
								}
								else
								{
									Flags.SetJudge(num38, JudgeType.None);
								}
								break;
							}
						}
						else
						{
							Flags.SetJudge(num38, JudgeType.None);
							Flags.LNHoldOnce[num38] = false;
						}
						if (_keySounds.ContainsKey(_lastKeySounds[num38]))
						{
							_keySounds[_lastKeySounds[num38]].Volume = (Flags.IsKeyVolumeMute ? 0f : ((float)Numbers.KeyVolume / 100f));
							if (!Flags.IsKeyVolumeMute || Numbers.KeyVolume > 0)
							{
								_keySounds[_lastKeySounds[num38]].Play();
							}
						}
					}
				}
				for (int num50 = _timeTable.BarIndex[StartBar]; num50 < _timeTable.Rows.Count; num50++)
				{
					if (Flags.HiSpeedFixType == HiSpeedFixType.RegularSpeed)
					{
						num7 = ((!(Numbers.Bpm <= 0.0)) ? num : (_reverseTime - (num - _reverseTime)));
					}
					else if (num6 > 0.0)
					{
						num7 = num6;
					}
					else if (Numbers.Bpm <= 0.0)
					{
						num7 = _reverseTime - (num - _reverseTime);
					}
					else
					{
						num7 = ((!flag) ? num : num10);
					}
					bool flag11 = _reverseTime > 0.0 && _timeTable.Rows[num50].Time > _reverseTime + Numbers.NoteTiming;
					bool flag12 = true;
					double num51 = ((Numbers.NoteTiming > 0.0) ? 0.0 : Numbers.NoteTiming);
					foreach (Rectangle item7 in list[1])
					{
						if (item7.Y - GetLanePosition(Numbers.Player1HiSpeed, num11, num3, num7, _timeTable.Rows[num50].Time + num51, num4, num8) >= -item7.Height)
						{
							flag12 = false;
						}
					}
					foreach (Rectangle item8 in list[1])
					{
						if (item8.Y - GetLanePosition(Numbers.Player2HiSpeed, num11, num3, num7, _timeTable.Rows[num50].Time + num51, num5, num9) >= -item8.Height)
						{
							flag12 = false;
						}
					}
					if (flag12)
					{
						break;
					}
					if (_timeTable.Rows[num50].Barline != null)
					{
						if (!_isBarlineCountAdded && _timeTable.Rows[num50].Barline.Time <= num && Numbers.Bpm > 0.0)
						{
							Numbers.CurrentBars++;
							_isBarlineCountAdded = true;
						}
						if (!_isBarlineReached && _timeTable.Rows[num50].Barline.Time - Numbers.NoteTiming <= num && Numbers.Bpm > 0.0)
						{
							flag2 = true;
							_isBarlineReached = true;
						}
						if (_isBarlineCountAdded && _isBarlineReached)
						{
							_isBarlineCountAdded = false;
							_isBarlineReached = false;
							_timeTable.Rows[num50].Barline = null;
						}
						else
						{
							for (int num52 = 0; num52 < barlineDestinations.Count; num52++)
							{
								int lanePosition = GetLanePosition(Numbers.Player1HiSpeed, num11, num3, num7, _timeTable.Rows[num50].Barline.Time, num4, num8);
								if (lanePosition >= 0)
								{
									_barlinePos1[num52].Add(new Point(barlineDestinations[num52].X, barlineDestinations[num52].Y - lanePosition));
								}
							}
							for (int num53 = 0; num53 < barlineDestinations2.Count; num53++)
							{
								int lanePosition2 = GetLanePosition(Numbers.Player2HiSpeed, num11, num3, num7, _timeTable.Rows[num50].Barline.Time, num5, num9);
								if (lanePosition2 >= 0)
								{
									_barlinePos2[num53].Add(new Point(barlineDestinations2[num53].X, barlineDestinations2[num53].Y - lanePosition2));
								}
							}
						}
					}
					for (int num54 = 0; num54 < _timeTable.Rows[num50].Channels[BmsChannelType.Bgm].Count; num54++)
					{
						if (!(_timeTable.Rows[num50].Channels[BmsChannelType.Bgm][num54].Time <= num) || flag11)
						{
							continue;
						}
						if (_keySounds.ContainsKey(_timeTable.Rows[num50].Channels[BmsChannelType.Bgm][num54].Value))
						{
							_keySounds[_timeTable.Rows[num50].Channels[BmsChannelType.Bgm][num54].Value].Volume = (Flags.IsBgmVolumeMute ? 0f : ((float)Numbers.BgmVolume / 100f));
							if (!Flags.IsBgmVolumeMute || Numbers.BgmVolume > 0)
							{
								_keySounds[_timeTable.Rows[num50].Channels[BmsChannelType.Bgm][num54].Value].Play();
							}
						}
						_timeTable.Rows[num50].Channels[BmsChannelType.Bgm].RemoveAt(num54);
						num54--;
					}
					for (int num55 = 0; num55 < _timeTable.Rows[num50].Lanes.Count; num55++)
					{
						for (int num56 = 0; num56 < _timeTable.Rows[num50].Lanes[num55].Count; num56++)
						{
							switch (_timeTable.Rows[num50].Lanes[num55][num56].ObjectType)
							{
							case BmsObjectType.Note:
								if (Flags.AutoPlay && _timeTable.Rows[num50].Lanes[num55][num56].Time <= num && !flag11 && !_timeTable.Rows[num50].Lanes[num55][num56].IsJudged && num50 <= _lastIndex)
								{
									if (_keySounds.ContainsKey(_timeTable.Rows[num50].Lanes[num55][num56].Value))
									{
										_keySounds[_timeTable.Rows[num50].Lanes[num55][num56].Value].Volume = (Flags.IsKeyVolumeMute ? 0f : ((float)Numbers.KeyVolume / 100f));
										if (!Flags.IsKeyVolumeMute || Numbers.KeyVolume > 0)
										{
											_keySounds[_timeTable.Rows[num50].Lanes[num55][num56].Value].Play();
										}
									}
									AutoPlayKeyState.SetKeyDownOnce(KeyList[num55]);
									Flags.LNHolding[num55] = false;
									Flags.LNReleased[num55] = false;
									Flags.LNHoldOnce[num55] = false;
									EventTimers.LaneLNHolding[num55] = 0.0;
									EventTimers.SetJudge(num55, JudgeType.PGreat);
									Flags.SetJudge(num55, JudgeType.PGreat);
									if (prevKeyValue < KeyNameToInt(_timeTable.Rows[num50].Lanes[num55][num56].Value))
									{
										prevKeyValue = KeyNameToInt(_timeTable.Rows[num50].Lanes[num55][num56].Value);
										Flags.SetLastJudged(num55);
									}
									Numbers.SetJudge(num55, JudgeType.PGreat, 0.0);
									double grooveGauge7 = Numbers.GrooveGauge;
									int grooveGaugeDivideCount7 = Numbers.GrooveGaugeDivideCount;
									Numbers.GrooveGauge += _gaugeIncrements[JudgeType.PGreat];
									if (grooveGauge7 < Numbers.GrooveGauge && Numbers.GrooveGauge >= 100.0)
									{
										EventTimers.GrooveGaugeIncrease = EventTimers.NowTime;
										EventTimers.GrooveGaugeMax = EventTimers.NowTime;
									}
									else if (grooveGaugeDivideCount7 < Numbers.GrooveGaugeDivideCount)
									{
										EventTimers.GrooveGaugeIncrease = EventTimers.NowTime;
									}
									Numbers.GaugeTotalIncrement += Numbers.GaugeTotal / (double)Numbers.TotalNotes;
									_timeTable.Rows[num50].Lanes[num55][num56].IsJudged = true;
									Numbers.NotesPerSecondCount++;
								}
								if (_timeTable.Rows[num50].Lanes[num55][num56].IsJudged && num - _timeTable.Rows[num50].Lanes[num55][num56].Time + Numbers.JudgeTiming > _judgeRange.Timeout * -1.0)
								{
									if (num50 == _lastIndex)
									{
										_lastCount++;
									}
									_timeTable.Rows[num50].Lanes[num55].RemoveAt(num56);
									num56--;
								}
								else if (_timeTable.Rows[num50].Lanes[num55][num56].Time - Numbers.NoteTiming <= num && Numbers.Bpm > 0.0)
								{
									if (!_timeTable.Rows[num50].Lanes[num55][num56].IsJudged)
									{
										for (int num62 = 0; num62 < list[num55].Count; num62++)
										{
											_notePos[num55][num62].Add(new NotePosition(_timeTable.Rows[num50].Lanes[num55][num56].ObjectType, list[num55][num62].X, list[num55][num62].Y, NoteStatus.Normal));
										}
									}
								}
								else if (_reverseTime > 0.0 && _timeTable.Rows[num50].Time <= _reverseTime + Numbers.NoteTiming)
								{
									if (_timeTable.Rows[num50].Lanes[num55][num56].IsJudged)
									{
										if (num50 == _lastIndex)
										{
											_lastCount++;
										}
										_timeTable.Rows[num50].Lanes[num55].RemoveAt(num56);
										num56--;
									}
									else
									{
										for (int num63 = 0; num63 < list[num55].Count; num63++)
										{
											_notePos[num55][num63].Add(new NotePosition(_timeTable.Rows[num50].Lanes[num55][num56].ObjectType, list[num55][num63].X, list[num55][num63].Y, NoteStatus.Normal));
										}
									}
								}
								else if (!_timeTable.Rows[num50].Lanes[num55][num56].IsJudged)
								{
									for (int num64 = 0; num64 < list[num55].Count; num64++)
									{
										int num65 = 0;
										num65 = ((num55 < 10) ? (list[num55][num64].Y - GetLanePosition(Numbers.Player1HiSpeed, num11, num3, num7, _timeTable.Rows[num50].Lanes[num55][num56].Time, num4, num8)) : (list[num55][num64].Y - GetLanePosition(Numbers.Player2HiSpeed, num11, num3, num7, _timeTable.Rows[num50].Lanes[num55][num56].Time, num5, num9)));
										_notePos[num55][num64].Add(new NotePosition(_timeTable.Rows[num50].Lanes[num55][num56].ObjectType, list[num55][num64].X, num65, NoteStatus.Normal));
									}
								}
								break;
							case BmsObjectType.LNStart:
							{
								NoteStatus status = NoteStatus.Normal;
								if (Flags.AutoPlay && _timeTable.Rows[num50].Lanes[num55][num56].Time <= num && Numbers.Bpm > 0.0 && !_timeTable.Rows[num50].Lanes[num55][num56].IsJudged && num50 <= _lastIndex)
								{
									if (_keySounds.ContainsKey(_timeTable.Rows[num50].Lanes[num55][num56].Value))
									{
										_keySounds[_timeTable.Rows[num50].Lanes[num55][num56].Value].Volume = (Flags.IsKeyVolumeMute ? 0f : ((float)Numbers.KeyVolume / 100f));
										if (!Flags.IsKeyVolumeMute || Numbers.KeyVolume > 0)
										{
											_keySounds[_timeTable.Rows[num50].Lanes[num55][num56].Value].Play();
										}
									}
									AutoPlayKeyState.SetKeyDown(KeyList[num55]);
									Flags.LNHolding[num55] = true;
									Flags.LNReleased[num55] = false;
									Flags.LNHoldOnce[num55] = true;
									EventTimers.LaneLNHolding[num55] = EventTimers.NowTime;
									Numbers.NotesPerSecondCount++;
									if (EventTimers.FirstNoteJudge <= 0.0)
									{
										EventTimers.FirstNoteJudge = EventTimers.NowTime;
									}
									if (!Flags.LegacyLN)
									{
										EventTimers.SetLNJudge(num55);
										Flags.SetJudge(num55, JudgeType.PGreat);
										if (prevKeyValue < KeyNameToInt(_timeTable.Rows[num50].Lanes[num55][num56].Value))
										{
											prevKeyValue = KeyNameToInt(_timeTable.Rows[num50].Lanes[num55][num56].Value);
											Flags.SetLastJudged(num55);
										}
										Numbers.SetJudge(num55, JudgeType.PGreat, 0.0);
										double grooveGauge8 = Numbers.GrooveGauge;
										int grooveGaugeDivideCount8 = Numbers.GrooveGaugeDivideCount;
										Numbers.GrooveGauge += _gaugeIncrements[JudgeType.PGreat];
										if (grooveGauge8 < Numbers.GrooveGauge && Numbers.GrooveGauge >= 100.0)
										{
											EventTimers.GrooveGaugeIncrease = EventTimers.NowTime;
											EventTimers.GrooveGaugeMax = EventTimers.NowTime;
										}
										else if (grooveGaugeDivideCount8 < Numbers.GrooveGaugeDivideCount)
										{
											EventTimers.GrooveGaugeIncrease = EventTimers.NowTime;
										}
										Numbers.GaugeTotalIncrement += Numbers.GaugeTotal / (double)Numbers.TotalNotes;
									}
									_timeTable.Rows[num50].Lanes[num55][num56].IsJudged = true;
								}
								if (_timeTable.Rows[num50].Lanes[num55][num56].IsJudged)
								{
									status = (Flags.LNHolding[num55] ? NoteStatus.LNHolding : NoteStatus.LNReleased);
								}
								if (_timeTable.Rows[num50].Lanes[num55][num56].Time - Numbers.NoteTiming <= num && Numbers.Bpm > 0.0)
								{
									dictionary.Add(num55, new List<NotePosition>());
									for (int num66 = 0; num66 < list[num55].Count; num66++)
									{
										dictionary[num55].Add(new NotePosition(_timeTable.Rows[num50].Lanes[num55][num56].ObjectType, list[num55][num66].X, list[num55][num66].Y, status));
									}
									break;
								}
								double num67 = _timeTable.Rows[num50].Lanes[num55][num56].Time;
								if (num67 < _reverseTime + Numbers.NoteTiming)
								{
									num67 = _reverseTime + Numbers.NoteTiming;
								}
								int num68 = 0;
								num68 = ((num55 < 10) ? GetLanePosition(Numbers.Player1HiSpeed, num11, num3, num7, num67, num4, num8) : GetLanePosition(Numbers.Player2HiSpeed, num11, num3, num7, num67, num5, num9));
								if (num68 < 0)
								{
									num68 = 0;
								}
								dictionary.Add(num55, new List<NotePosition>());
								for (int num69 = 0; num69 < list[num55].Count; num69++)
								{
									dictionary[num55].Add(new NotePosition(_timeTable.Rows[num50].Lanes[num55][num56].ObjectType, list[num55][num69].X, list[num55][num69].Y - num68, status));
								}
								break;
							}
							case BmsObjectType.LNEnd:
								if (Flags.AutoPlay && _timeTable.Rows[num50].Lanes[num55][num56].Time <= num && !flag11 && !_timeTable.Rows[num50].Lanes[num55][num56].IsJudged && num50 <= _lastIndex)
								{
									AutoPlayKeyState.SetKeyUp(KeyList[num55]);
									Flags.LNHolding[num55] = false;
									Flags.LNReleased[num55] = true;
									EventTimers.LaneLNHolding[num55] = 0.0;
									EventTimers.SetJudge(num55, JudgeType.PGreat);
									Flags.SetJudge(num55, JudgeType.PGreat);
									if (prevKeyValue < KeyNameToInt(_timeTable.Rows[num50].Lanes[num55][num56].Value))
									{
										prevKeyValue = KeyNameToInt(_timeTable.Rows[num50].Lanes[num55][num56].Value);
										Flags.SetLastJudged(num55);
									}
									Numbers.SetJudge(num55, JudgeType.PGreat, 0.0);
									double grooveGauge6 = Numbers.GrooveGauge;
									int grooveGaugeDivideCount6 = Numbers.GrooveGaugeDivideCount;
									Numbers.GrooveGauge += _gaugeIncrements[JudgeType.PGreat];
									if (grooveGauge6 < Numbers.GrooveGauge && Numbers.GrooveGauge >= 100.0)
									{
										EventTimers.GrooveGaugeIncrease = EventTimers.NowTime;
										EventTimers.GrooveGaugeMax = EventTimers.NowTime;
									}
									else if (grooveGaugeDivideCount6 < Numbers.GrooveGaugeDivideCount)
									{
										EventTimers.GrooveGaugeIncrease = EventTimers.NowTime;
									}
									Numbers.GaugeTotalIncrement += Numbers.GaugeTotal / (double)Numbers.TotalNotes;
									if (!Flags.LegacyLN)
									{
										Numbers.NotesPerSecondCount++;
									}
									if (num50 == _lastIndex)
									{
										_lastCount++;
									}
									_timeTable.Rows[num50].Lanes[num55][num56].IsJudged = true;
								}
								if (_timeTable.Rows[num50].Lanes[num55][num56].IsJudged && Numbers.Bpm > 0.0 && (Flags.AutoPlay || num - _timeTable.Rows[num50].Lanes[num55][num56].Time + Numbers.JudgeTiming > 0.0))
								{
									if (_startLNHolding.Contains(num55))
									{
										_startLNHolding.Remove(num55);
									}
									else
									{
										for (int num57 = num50; num57 >= 0; num57--)
										{
											bool flag13 = false;
											for (int num58 = _timeTable.Rows[num57].Lanes[num55].Count - 1; num58 >= 0; num58--)
											{
												if ((num50 != num57 || num58 >= num56) && _timeTable.Rows[num57].Lanes[num55][num58].ObjectType == BmsObjectType.LNStart)
												{
													flag13 = true;
													_timeTable.Rows[num57].Lanes[num55].RemoveAt(num58);
													break;
												}
											}
											if (flag13)
											{
												break;
											}
										}
									}
									_timeTable.Rows[num50].Lanes[num55].RemoveAt(num56);
									num56--;
								}
								else if (!_timeTable.Rows[num50].Lanes[num55][num56].IsJudged && _timeTable.Rows[num50].Lanes[num55][num56].Time - Numbers.NoteTiming <= num && Numbers.Bpm > 0.0)
								{
									for (int num59 = 0; num59 < list[num55].Count; num59++)
									{
										if (dictionary.ContainsKey(num55))
										{
											_notePos[num55][num59].Add(dictionary[num55][num59]);
											_notePos[num55][num59].Add(new NotePosition(_timeTable.Rows[num50].Lanes[num55][num56].ObjectType, list[num55][num59].X, list[num55][num59].Y, dictionary[num55][num59].Status));
										}
									}
								}
								else
								{
									for (int num60 = 0; num60 < list[num55].Count; num60++)
									{
										if (dictionary.ContainsKey(num55))
										{
											_notePos[num55][num60].Add(dictionary[num55][num60]);
											int num61 = 0;
											num61 = ((num55 < 10) ? GetLanePosition(Numbers.Player1HiSpeed, num11, num3, num7, _timeTable.Rows[num50].Lanes[num55][num56].Time, num4, num8) : GetLanePosition(Numbers.Player2HiSpeed, num11, num3, num7, _timeTable.Rows[num50].Lanes[num55][num56].Time, num5, num9));
											if (num61 < 0)
											{
												num61 = 0;
											}
											_notePos[num55][num60].Add(new NotePosition(_timeTable.Rows[num50].Lanes[num55][num56].ObjectType, list[num55][num60].X, list[num55][num60].Y - num61, dictionary[num55][num60].Status));
										}
									}
								}
								if (dictionary.ContainsKey(num55))
								{
									dictionary.Remove(num55);
								}
								break;
							case BmsObjectType.InvisibleNote:
								if (Flags.AutoPlay || num - _timeTable.Rows[num50].Lanes[num55][num56].Time + Numbers.JudgeTiming > _judgeRange.Timeout * -1.0)
								{
									if (num50 == _lastIndex)
									{
										_lastCount++;
									}
									_timeTable.Rows[num50].Lanes[num55].RemoveAt(num56);
									num56--;
								}
								break;
							}
						}
					}
					for (int num70 = 0; num70 < _timeTable.Rows[num50].Channels[BmsChannelType.Bga].Count; num70++)
					{
						if (_timeTable.Rows[num50].Channels[BmsChannelType.Bga][num70].Time <= num && !flag11)
						{
							_nowBga = _timeTable.Rows[num50].Channels[BmsChannelType.Bga][num70].Value;
							PlayMovie(_nowBga);
							_timeTable.Rows[num50].Channels[BmsChannelType.Bga].RemoveAt(num70);
							num70--;
						}
					}
					for (int num71 = 0; num71 < _timeTable.Rows[num50].Channels[BmsChannelType.Layer].Count; num71++)
					{
						if (_timeTable.Rows[num50].Channels[BmsChannelType.Layer][num71].Time <= num && !flag11)
						{
							_nowLayer = _timeTable.Rows[num50].Channels[BmsChannelType.Layer][num71].Value;
							PlayMovie(_nowLayer);
							_timeTable.Rows[num50].Channels[BmsChannelType.Layer].RemoveAt(num71);
							num71--;
						}
					}
					for (int num72 = 0; num72 < _timeTable.Rows[num50].Channels[BmsChannelType.Poor].Count; num72++)
					{
						if (_timeTable.Rows[num50].Channels[BmsChannelType.Poor][num72].Time <= num && !flag11)
						{
							_nowPoor = _timeTable.Rows[num50].Channels[BmsChannelType.Poor][num72].Value;
							PlayMovie(_nowPoor);
							_timeTable.Rows[num50].Channels[BmsChannelType.Poor].RemoveAt(num72);
							num72--;
						}
					}
					double num73 = 0.0;
					double num74 = 0.0;
					double num75 = num3;
					for (int num76 = 0; num76 < _timeTable.Rows[num50].Channels[BmsChannelType.Bpm].Count; num76++)
					{
						double num77 = double.Parse(_timeTable.Rows[num50].Channels[BmsChannelType.Bpm][num76].Value);
						if (_timeTable.Rows[num50].Channels[BmsChannelType.Bpm][num76].Time - Numbers.NoteTiming <= num && Numbers.Bpm > 0.0)
						{
							Numbers.Bpm = num77;
							num75 = Math.Abs(num77);
							if (Numbers.Bpm > 0.0)
							{
								_timeTable.Rows[num50].Channels[BmsChannelType.Bpm].RemoveAt(num76);
								num76--;
								continue;
							}
							if (_reverseTime <= 0.0)
							{
								_reverseTime = _timeTable.Rows[num50].Channels[BmsChannelType.Bpm][num76].Time - Numbers.NoteTiming;
							}
							for (int num78 = 0; num78 < 20; num78++)
							{
								if (Flags.LNHolding[num78])
								{
									Flags.LNHolding[num78] = false;
									Flags.LNReleased[num78] = true;
									EventTimers.LaneLNHolding[num78] = 0.0;
									if (AutoPlayKeyState.IsKeyDown(KeyList[num78]))
									{
										AutoPlayKeyState.SetKeyUp(KeyList[num78]);
									}
								}
							}
						}
						else
						{
							num73 = num3 * Numbers.Player1HiSpeed * num11 * (_timeTable.Rows[num50].Channels[BmsChannelType.Bpm][num76].Time - num7 - Numbers.NoteTiming) / 1000.0;
							num74 = num3 * Numbers.Player2HiSpeed * num11 * (_timeTable.Rows[num50].Channels[BmsChannelType.Bpm][num76].Time - num7 - Numbers.NoteTiming) / 1000.0;
							num75 = Math.Abs(double.Parse(_timeTable.Rows[num50].Channels[BmsChannelType.Bpm][num76].Value));
							num6 = _timeTable.Rows[num50].Channels[BmsChannelType.Bpm][num76].Time - Numbers.NoteTiming;
						}
					}
					num3 = num75;
					num4 += num73;
					num5 += num74;
					double num79 = 0.0;
					double num80 = 0.0;
					for (int num81 = 0; num81 < _timeTable.Rows[num50].Channels[BmsChannelType.Stop].Count; num81++)
					{
						num79 += 60000.0 / (num3 * 48.0) * double.Parse(_timeTable.Rows[num50].Channels[BmsChannelType.Stop][num81].Value);
					}
					if (num79 > 0.0)
					{
						bool flag14 = false;
						if (Numbers.Bpm > 0.0)
						{
							for (int num82 = 0; num82 < _timeTable.Rows[num50].Channels[BmsChannelType.Stop].Count; num82++)
							{
								if (_timeTable.Rows[num50].Channels[BmsChannelType.Stop][num82].Time - Numbers.NoteTiming + num79 <= num)
								{
									_timeTable.Rows[num50].Channels[BmsChannelType.Stop].RemoveAt(num82);
									num82--;
									flag = false;
									flag14 = true;
								}
								else if (_timeTable.Rows[num50].Channels[BmsChannelType.Stop][num82].Time - Numbers.NoteTiming <= num)
								{
									flag = true;
									num80 = _timeTable.Rows[num50].Channels[BmsChannelType.Stop][num82].Time - Numbers.NoteTiming;
								}
							}
						}
						if (!flag14)
						{
							num8 += num3 * Numbers.Player1HiSpeed * num11 * num79 / 1000.0;
							num9 += num3 * Numbers.Player2HiSpeed * num11 * num79 / 1000.0;
						}
						if (flag)
						{
							num10 += num80;
						}
					}
					if (num50 == _lastIndex && !Flags.IsLastNoteJudged && _lastNoteCount <= _lastCount)
					{
						Flags.IsLastNoteJudged = true;
					}
					if (_timeTable.Rows[num50].Count <= 0)
					{
						_timeTable.Rows.RemoveAt(num50);
						if (num50 > _lastIndex)
						{
							num50--;
							continue;
						}
						num50--;
						_lastIndex--;
					}
				}
				if (dictionary.Count > 0)
				{
					foreach (KeyValuePair<int, List<NotePosition>> item9 in dictionary)
					{
						for (int num83 = 0; num83 < item9.Value.Count; num83++)
						{
							_notePos[item9.Key][num83].Add(item9.Value[num83]);
						}
					}
					dictionary.Clear();
				}
				if (flag2)
				{
					EventTimers.ResetBpmTimer();
				}
				else if (!flag)
				{
					EventTimers.AddBpmTimer(EventTimers.ElapsedTime, Math.Abs(Numbers.Bpm));
				}
				foreach (PlayingKeys value in Enum.GetValues(typeof(PlayingKeys)))
				{
					if (Flags.IsKeyDownOnce(value))
					{
						EventTimers.KeyDown[value] = EventTimers.NowTime;
						EventTimers.KeyDownAlt[value] = EventTimers.NowTime;
						EventTimers.KeyUp[value] = 0.0;
					}
					if (Flags.IsKeyUpOnce(value))
					{
						EventTimers.KeyUp[value] = EventTimers.NowTime;
						EventTimers.KeyDown[value] = 0.0;
					}
				}
				Numbers.UpdateNotesPerSecond();
				if (Flags.PoorBga && EventTimers.NowTime - EventTimers.PoorBga > 500.0)
				{
					EventTimers.PoorBga = 0.0;
				}
			}
		}
		if (Program.SkipDraw && _isSkinReady)
		{
			Program.SkipDraw = false;
		}
		void SetLastJudged(int lane, string keyName, JudgeType judge)
		{
			int num84 = KeyNameToInt(keyName);
			if (judge == JudgeType.Bad || judge == JudgeType.PassedPoor || prevKeyValue < num84)
			{
				prevKeyValue = num84;
				Flags.SetLastJudged(lane);
			}
		}
	}

	private int GetLanePosition(double hiSpeed, double hsFix, double bpm, double time, double objectTime, double bpmPos = 0.0, double stopPos = 0.0)
	{
		if (Flags.HiSpeedFixType == HiSpeedFixType.RegularSpeed)
		{
			return (int)Math.Ceiling(150.0 * hiSpeed * hsFix * (objectTime - time - Numbers.NoteTiming) / 1000.0);
		}
		return (int)Math.Ceiling(bpmPos + bpm * hiSpeed * hsFix * (objectTime - time - Numbers.NoteTiming) / 1000.0 - stopPos);
	}

	private BitmapImage LoadBgaBmp(string path)
	{
		BitmapImage bitmapImage2;
		using (BitmapImage bitmapImage = PlatformGeneral.LoadBitmapImage(path))
		{
			bitmapImage2 = bitmapImage.ToRawBitmapImage();
		}
		for (int i = 0; i < bitmapImage2.Width; i++)
		{
			for (int j = 0; j < bitmapImage2.Height; j++)
			{
				Span<byte> pixelAsSpan = bitmapImage2.GetPixelAsSpan(i, j);
				if (pixelAsSpan[0] == 0 && pixelAsSpan[1] == 0 && pixelAsSpan[2] == 0 && pixelAsSpan[3] == byte.MaxValue)
				{
					pixelAsSpan[3] = 0;
				}
			}
		}
		return bitmapImage2;
	}

	// #BMP/BGA 채널 없이 동영상 파일만 있는 차트(식스타 게이트 커스텀 등)는 마디 0에 동영상 BGA를 넣는다
	private void AddFallbackBgaMovie()
	{
		if (Flags.DisableBga || _bmsData.Bars.Count == 0 || _bmsData.Bars.Any(b => b.Bga.Count > 0))
		{
			return;
		}
		string bgaKey = _bmsData.BmpFiles.FirstOrDefault(kvp => MovieExtension.Contains(Path.GetExtension(kvp.Value).ToLowerInvariant())).Key;
		if (bgaKey == null)
		{
			string movieFile = FindFallbackBgaMovie();
			if (movieFile == null)
			{
				return;
			}
			bgaKey = FallbackBgaKey;
			_bmsData.BmpFiles[bgaKey] = movieFile;
		}
		_bmsData.Bars[0].AddBmsObject(BmsChannelType.Bga, new BmsObject(0.0, bgaKey, BmsObjectType.Bga));
	}

	private string FindFallbackBgaMovie()
	{
		string dir = string.IsNullOrEmpty(BmsPath) ? null : Path.GetDirectoryName(BmsPath);
		if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
		{
			return null;
		}
		return FallbackBgaMovieNames.FirstOrDefault(name => File.Exists(Path.Combine(dir, name))) ?? Directory.GetFiles(dir, "*.mp4").Select(Path.GetFileName).FirstOrDefault();
	}

	private void PlayMovie(string index)
	{
		if (_movies.TryGetValue(index, out IMovie movie))
		{
			movie.Play();
		}
	}

	private void RestartMovie(string index, double position)
	{
		if (_movies.TryGetValue(index, out IMovie movie))
		{
			if (!movie.IsStopped)
			{
				movie.Stop();
			}
			movie.CurrentPosition = Math.Min(position, movie.TotalTime);
			movie.Play();
		}
	}

	private void UpdateBga(TextureInfo texture, string prevIndex, string nowIndex)
	{
		if (_movies.TryGetValue(nowIndex, out IMovie movie))
		{
			if (!movie.NeedsCapture)
			{
				return;
			}
			UnmanagedArray<byte> frameData = movie.GetFrameData();
			if (frameData != null)
			{
				try
				{
					UpdateBgaTexture(texture, frameData.DataPtr, movie.Width, movie.Height, movie.Width * 4);
				}
				finally
				{
					frameData.Dispose();
				}
			}
		}
		else if (prevIndex != nowIndex)
		{
			if (_bgaBmps.TryGetValue(nowIndex, out BitmapImage bitmapImage))
			{
				UpdateBgaTexture(texture, bitmapImage);
			}
			else
			{
				((IDynamicTextureInfo)texture).Clear();
			}
		}
	}

	private void UpdateBgaTexture(TextureInfo texture, BitmapImage rawTexture)
	{
		UpdateBgaTexture(texture, rawTexture.DataPtr, rawTexture.Width, rawTexture.Height, rawTexture.Stride);
	}

	private void UpdateBgaTexture(TextureInfo texture, IntPtr dataPtr, int width, int height, int stride)
	{
		int num = stride / width;
		if (_tempBgaData == null || _tempBgaData.Count != _maxBgaWidth * _maxBgaHeight * num)
		{
			_tempBgaData?.Dispose();
			_tempBgaData = new UnmanagedArray<byte>(_maxBgaWidth * _maxBgaHeight * num);
		}
		for (int i = 0; i < height; i++)
		{
			_tempBgaData.CopyFromPtr(dataPtr + stride * i, _maxBgaWidth * num * i, stride);
		}
		((IDynamicTextureInfo)texture).UpdateTextureData(_tempBgaData.DataPtr, _tempBgaData.Count);
	}

	public override void Draw()
	{
		if (!_isSkinReady)
		{
			return;
		}
		double nowTime = EventTimers.NowTime;
		BlendType prevBlend = BlendType.Alpha;
		SamplerType prevSampler = SamplerType.Point;
		if (_isBmsReady && !Flags.Loading && Flags.LoadComplete)
		{
			if (!Flags.DisableBga)
			{
				if (_bgaTexture != null)
				{
					UpdateBga(_bgaTexture, _prevBga, _nowBga);
				}
				if (_layerTexture != null)
				{
					UpdateBga(_layerTexture, _prevLayer, _nowLayer);
				}
				if (_poorTexture != null)
				{
					UpdateBga(_poorTexture, _prevPoor, _nowPoor);
				}
			}
			_prevBga = _nowBga;
			_prevLayer = _nowLayer;
			_prevPoor = _nowPoor;
		}
		foreach (ISkinItem item in _skin.Items)
		{
			DrawSkinItem(item, nowTime, ref prevBlend, ref prevSampler, Point.Empty, in ColorVector.White);
		}
	}

	protected override void DrawSkinItem(ISkinItem item, double now, ref BlendType prevBlend, ref SamplerType prevSampler, Point offset, in ColorVector color)
	{
		if (!item.ItemType.Contains(SkinItemType.Drawable))
		{
			return;
		}
		if (item is SingleDestinationSkinItem singleDestinationSkinItem)
		{
			if (singleDestinationSkinItem.CheckDisplayed(now))
			{
				SetBlendState(singleDestinationSkinItem.Destination.Blend, ref prevBlend);
				SetSamplerState(singleDestinationSkinItem.Destination.Filter, ref prevSampler);
				SingleDestinationSkinItem singleDestinationSkinItem2 = singleDestinationSkinItem;
				if (singleDestinationSkinItem2 != null)
				{
					if (singleDestinationSkinItem2 is PlayAreaSkinItem playAreaSkinItem)
					{
						Rectangle destinationRectangle = playAreaSkinItem.GetDestinationRectangle(now, offset);
						ColorVector color2 = playAreaSkinItem.GetDestinationColorVector(now) * color;
						{
							foreach (ISkinItem item2 in playAreaSkinItem.Items)
							{
								DrawSkinItem(item2, now, ref prevBlend, ref prevSampler, destinationRectangle.Location, in color2);
							}
							return;
						}
					}
					if (singleDestinationSkinItem2 is BarlineSkinItem barlineSkinItem)
					{
						BarlineSkinItem barlineSkinItem2 = barlineSkinItem;
						BarlinePositionCollection barlinePositionCollection = null;
						if (barlineSkinItem2.Side == 2)
						{
							if (_barlinePos2.Count > 0)
							{
								barlinePositionCollection = _barlinePos2[0];
								_barlinePos2.RemoveAt(0);
							}
						}
						else if (_barlinePos1.Count > 0)
						{
							barlinePositionCollection = _barlinePos1[0];
							_barlinePos1.RemoveAt(0);
						}
						if (barlinePositionCollection != null)
						{
							barlineSkinItem2.Draw(Graphics, _skinTextures[barlineSkinItem2.TextureID], now, barlinePositionCollection, offset, in color);
						}
						return;
					}
					if (singleDestinationSkinItem2 is LaneSkinItem laneSkinItem)
					{
						LaneSkinItem laneSkinItem2 = laneSkinItem;
						if (_notePos.Count <= laneSkinItem2.LaneIndex || _notePos[laneSkinItem2.LaneIndex].Count <= 0)
						{
							return;
						}
						for (int i = 0; i < _notePos[laneSkinItem2.LaneIndex][0].Count; i++)
						{
							ColorVector colorVector = laneSkinItem2.GetDestinationColorVector(now) * color;
							Rectangle destinationRectangle2 = laneSkinItem2.GetDestinationRectangle(now, offset);
							switch (_notePos[laneSkinItem2.LaneIndex][0][i].ObjectType)
							{
							case BmsObjectType.Note:
							{
								ColorVector color6 = colorVector * laneSkinItem2.NoteSources["note"].ColorVector;
								Rectangle dstRectangle4 = destinationRectangle2;
								dstRectangle4.X = _notePos[laneSkinItem2.LaneIndex][0][i].X + offset.X;
								dstRectangle4.Y = _notePos[laneSkinItem2.LaneIndex][0][i].Y + offset.Y;
								Graphics.Draw(_skinTextures[laneSkinItem2.NoteSources["note"].TextureID], laneSkinItem2.NoteSources["note"].GetSourceRectangle(now), in dstRectangle4, in color6);
								break;
							}
							case BmsObjectType.LNStart:
							{
								string text = "";
								if (_notePos[laneSkinItem2.LaneIndex][0][i].Status == NoteStatus.LNHolding)
								{
									text = "_holding";
								}
								else if (_notePos[laneSkinItem2.LaneIndex][0][i].Status == NoteStatus.LNReleased)
								{
									text = "_released";
								}
								bool flag = false;
								Rectangle dstRectangle = default;
								Rectangle dstRectangle2 = default;
								ColorVector color3 = colorVector * laneSkinItem2.NoteSources["ln_body" + text].ColorVector;
								ColorVector color4 = ColorVector.Zero;
								for (int j = i; j < _notePos[laneSkinItem2.LaneIndex][0].Count; j++)
								{
									if (_notePos[laneSkinItem2.LaneIndex][0][j].ObjectType == BmsObjectType.LNEnd)
									{
										flag = true;
										color4 = colorVector * laneSkinItem2.NoteSources["ln_end" + text].ColorVector;
										dstRectangle = destinationRectangle2;
										dstRectangle.X = _notePos[laneSkinItem2.LaneIndex][0][i].X + offset.X;
										dstRectangle.Y = _notePos[laneSkinItem2.LaneIndex][0][j].Y + laneSkinItem2.NoteSources["ln_end" + text].Height + offset.Y;
										dstRectangle.Height = _notePos[laneSkinItem2.LaneIndex][0][i].Y - _notePos[laneSkinItem2.LaneIndex][0][j].Y - laneSkinItem2.NoteSources["ln_end" + text].Height;
										dstRectangle2 = destinationRectangle2;
										dstRectangle2.X = _notePos[laneSkinItem2.LaneIndex][0][j].X + offset.X;
										dstRectangle2.Y = _notePos[laneSkinItem2.LaneIndex][0][j].Y + offset.Y;
										break;
									}
								}
								if (!flag)
								{
									dstRectangle = destinationRectangle2;
									dstRectangle.X = _notePos[laneSkinItem2.LaneIndex][0][i].X + offset.X;
									dstRectangle.Y = offset.Y;
									dstRectangle.Height = _notePos[laneSkinItem2.LaneIndex][0][i].Y;
								}
								Graphics.Draw(_skinTextures[laneSkinItem2.NoteSources["ln_body" + text].TextureID], laneSkinItem2.NoteSources["ln_body" + text].GetSourceRectangle(now), in dstRectangle, in color3);
								ColorVector color5 = colorVector * laneSkinItem2.NoteSources["ln_start" + text].ColorVector;
								Rectangle dstRectangle3 = destinationRectangle2;
								dstRectangle3.X = _notePos[laneSkinItem2.LaneIndex][0][i].X + offset.X;
								dstRectangle3.Y = _notePos[laneSkinItem2.LaneIndex][0][i].Y + offset.Y;
								Graphics.Draw(_skinTextures[laneSkinItem2.NoteSources["ln_start" + text].TextureID], laneSkinItem2.NoteSources["ln_start" + text].GetSourceRectangle(now), in dstRectangle3, in color5);
								if (flag)
								{
									Graphics.Draw(_skinTextures[laneSkinItem2.NoteSources["ln_end" + text].TextureID], laneSkinItem2.NoteSources["ln_end" + text].GetSourceRectangle(now), in dstRectangle2, in color4);
								}
								break;
							}
							}
						}
						_notePos[laneSkinItem2.LaneIndex].RemoveAt(0);
						return;
					}
					if (singleDestinationSkinItem2 is BgaSkinItem bgaSkinItem)
					{
						Rectangle dstRectangle5 = bgaSkinItem.GetDestinationRectangle(now, offset);
						ColorVector color7 = bgaSkinItem.GetDestinationColorVector(now) * color;
						Graphics.Draw(_skinTextures["#black"], new Rectangle(0, 0, 1, 1), in dstRectangle5, in color7);
						double num = 1.0;
						double num2 = 1.0;
						num = ((_maxBgaWidth <= 256) ? ((double)dstRectangle5.Width / 256.0) : ((double)dstRectangle5.Width / 256.0 * (256.0 / (double)_maxBgaWidth)));
						num2 = ((_maxBgaHeight <= 256) ? ((double)dstRectangle5.Height / 256.0) : ((double)dstRectangle5.Height / 256.0 * (256.0 / (double)_maxBgaHeight)));
						if (_hasPoorBga && Flags.PoorBga)
						{
							DrawBga(_poorTexture, _nowPoor, in dstRectangle5, in color7, num, num2);
							return;
						}
						DrawBga(_bgaTexture, _nowBga, in dstRectangle5, in color7, num, num2);
						DrawBga(_layerTexture, _nowLayer, in dstRectangle5, in color7, num, num2);
						return;
					}
					if (singleDestinationSkinItem2 is JudgeSkinItem judgeSkinItem)
					{
						JudgeSkinItem judgeSkinItem2 = judgeSkinItem;
						Rectangle dstRectangle6 = judgeSkinItem2.GetDestinationRectangle(now, offset);
						ColorVector color8 = judgeSkinItem2.GetDestinationColorVector(now) * color;
						ComboSkinItem comboItem = judgeSkinItem2.ComboItem;
						if (comboItem != null)
						{
							int num3 = (int)Math.Ceiling((double)comboItem.GetDestinationRectangle(now, offset).Width / 2.0);
							dstRectangle6.X -= num3;
						}
						Graphics.Draw(_skinTextures[judgeSkinItem2.TextureID], judgeSkinItem2.GetSourceRectangle(now), in dstRectangle6, in color8);
						return;
					}
					if (singleDestinationSkinItem2 is ComboSkinItem comboSkinItem)
					{
						ComboSkinItem comboSkinItem2 = comboSkinItem;
						Point empty = Point.Empty;
						JudgeSkinItem judgeItem = comboSkinItem2.JudgeItem;
						if (judgeItem != null)
						{
							int num4 = (int)Math.Ceiling((double)comboSkinItem2.GetDestinationRectangle(now, offset).Width / 2.0);
							Rectangle destinationRectangle3 = judgeItem.GetDestinationRectangle(now, offset);
							empty.X = destinationRectangle3.X - num4;
							empty.Y = destinationRectangle3.Y;
						}
						ColorVector color9 = comboSkinItem2.GetDestinationColorVector(now) * color;
						int value = comboSkinItem2.Value;
						int numberDigit = comboSkinItem2.GetNumberDigit();
						int num5;
						if (comboSkinItem2.NumberType == 11 && comboSkinItem2.Digit != 0)
						{
							num5 = Math.Abs(comboSkinItem2.Digit);
						}
						else if (comboSkinItem2.NumberType == 24 && comboSkinItem2.Digit != 0)
						{
							num5 = Math.Abs(comboSkinItem2.Digit) + 1;
						}
						else
						{
							num5 = ((comboSkinItem2.NumberType != 24 || comboSkinItem2.Digit != 0 || value >= 0) ? numberDigit : (numberDigit + 1));
						}
						int num6 = Math.Abs(value);
						for (int k = 0; k < num5; k++)
						{
							int num7;
							if (comboSkinItem2.NumberType == 24 && (comboSkinItem2.Digit != 0 || (comboSkinItem2.Digit == 0 && value < 0)) && k == num5 - 1)
							{
								num7 = 11;
							}
							else if (k >= numberDigit)
							{
								num7 = 10;
							}
							else
							{
								num7 = Math.Abs(num6) % 10;
								num6 = (num6 - num7) / 10;
							}
							if (comboSkinItem2.NumberType == 24 && value < 0)
							{
								num7 += 12;
							}
							Rectangle dstRectangle7 = comboSkinItem2.GetDestinationRectangle(now, k, empty);
							Graphics.Draw(_skinTextures[comboSkinItem2.TextureID], comboSkinItem2.GetSourceRectangle(now, num7), in dstRectangle7, in color9);
						}
						return;
					}
					if (singleDestinationSkinItem2 is LaneCoverSkinItem laneCoverSkinItem)
					{
						LaneCoverSkinItem laneCoverSkinItem2 = laneCoverSkinItem;
						laneCoverSkinItem2.Draw(Graphics, _skinTextures[laneCoverSkinItem2.TextureID], now, offset, in color);
						return;
					}
				}
				DrawCommonSkinItem(singleDestinationSkinItem, now, ref prevBlend, ref prevSampler, _skinTextures, _fonts, offset, in color);
			}
			else if (singleDestinationSkinItem is BarlineSkinItem barlineSkinItem3)
			{
				if (barlineSkinItem3.Side == 2)
				{
					if (_barlinePos2.Count > 0)
					{
						_barlinePos2.RemoveAt(0);
					}
				}
				else if (_barlinePos1.Count > 0)
				{
					_barlinePos1.RemoveAt(0);
				}
			}
			else if (singleDestinationSkinItem is LaneSkinItem laneSkinItem3 && _notePos.Count > laneSkinItem3.LaneIndex && _notePos[laneSkinItem3.LaneIndex].Count > 0)
			{
				_notePos[laneSkinItem3.LaneIndex].RemoveAt(0);
			}
		}
		else
		{
			if (!(item is MultipleDestinationSkinItem multipleDestinationSkinItem))
			{
				return;
			}
			if (multipleDestinationSkinItem is GaugeSkinItem gaugeSkinItem)
			{
				int totalIndex = gaugeSkinItem.GetTotalIndex();
				for (int l = 0; l < totalIndex; l++)
				{
					int containerIndex = gaugeSkinItem.GetContainerIndex(l, totalIndex);
					if (gaugeSkinItem.Destinations.ContainsKey(containerIndex))
					{
						SetBlendState(gaugeSkinItem.Destinations[containerIndex].Blend, ref prevBlend);
						SetSamplerState(gaugeSkinItem.Destinations[containerIndex].Filter, ref prevSampler);
						ColorVector color10 = gaugeSkinItem.GetDestinationColorVector(containerIndex, now) * color;
						Graphics.Draw(_skinTextures[gaugeSkinItem.TextureID], gaugeSkinItem.GetSourceRectangle(l, now), gaugeSkinItem.GetDestinationRectangle(l, totalIndex, now, offset), in color10);
					}
				}
			}
			else
			{
				DrawCommonMultipleDestinationSkinItem(multipleDestinationSkinItem, now, ref prevBlend, ref prevSampler, _skinTextures, _fonts, offset, in color);
			}
		}
	}

	private void DrawBga(TextureInfo texture, string index, in Rectangle baseDst, in ColorVector color, double widthRatio, double heightRatio)
	{
		if (texture == null || !TryGetBgaSize(index, out int width, out int height))
		{
			return;
		}
		Rectangle dstRectangle = Rectangle.Empty;
		dstRectangle.Width = (int)Math.Ceiling((double)width * widthRatio);
		dstRectangle.Height = (int)Math.Ceiling((double)height * heightRatio);
		dstRectangle.X = baseDst.X + (baseDst.Width - dstRectangle.Width) / 2;
		dstRectangle.Y = baseDst.Y;
		Graphics.Draw(texture, new Rectangle(0, 0, width, height), in dstRectangle, in color);
	}

	private bool TryGetBgaSize(string index, out int width, out int height)
	{
		if (_movies.TryGetValue(index, out IMovie movie) && movie.IsPlaying)
		{
			width = movie.Width;
			height = movie.Height;
			return true;
		}
		if (_bgaBmps.TryGetValue(index, out BitmapImage bitmapImage) && bitmapImage != null)
		{
			width = bitmapImage.Width;
			height = bitmapImage.Height;
			return true;
		}
		width = 0;
		height = 0;
		return false;
	}

	private int KeyNameToInt(string keyname)
	{
		if (string.IsNullOrEmpty(keyname))
		{
			return 0;
		}
		int num = 0;
		if (_bmsData.IsBase62Defined)
		{
			string text = keyname;
			for (int i = 0; i < text.Length; i++)
			{
				int num2 = KeyNameChars62.IndexOf(text[text.Length - (i + 1)]);
				if (num2 < 0)
				{
					return 0;
				}
				num += num2 * (int)Math.Pow(62.0, i);
			}
		}
		else
		{
			string text = keyname.ToUpper();
			for (int j = 0; j < text.Length; j++)
			{
				int num3 = KeyNameChars.IndexOf(text[text.Length - (j + 1)]);
				if (num3 < 0)
				{
					return 0;
				}
				num += num3 * (int)Math.Pow(36.0, j);
			}
		}
		return num;
	}

	public void OnFileDrop(object sender, FileDropEventArgs e)
	{
		_dropFileList.Add(e.FileName);
	}

	public override void Dispose()
	{
		UnloadContent();
	}
}
