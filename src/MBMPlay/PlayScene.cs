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

public sealed partial class PlayScene : Scene
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

	public PlayScene(GraphicsBase graphics, GraphicsWindow window, AudioInstance audio)
		: base(graphics, window, audio)
	{
		ButtonCommandDictionary = CreateButtonCommandDictionary();
		TrackBarCommandDictionary = CreateTrackBarCommandDictionary();
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

	private int GetLanePosition(double hiSpeed, double hsFix, double bpm, double time, double objectTime, double bpmPos = 0.0, double stopPos = 0.0)
	{
		if (Flags.HiSpeedFixType == HiSpeedFixType.RegularSpeed)
		{
			return (int)Math.Ceiling(150.0 * hiSpeed * hsFix * (objectTime - time - Numbers.NoteTiming) / 1000.0);
		}
		return (int)Math.Ceiling(bpmPos + bpm * hiSpeed * hsFix * (objectTime - time - Numbers.NoteTiming) / 1000.0 - stopPos);
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
