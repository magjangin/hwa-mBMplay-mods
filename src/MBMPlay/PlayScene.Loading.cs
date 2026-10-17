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

public sealed partial class PlayScene
{
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
}
