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
	// SetLastJudged와 오토플레이 처리가 한 프레임 안에서 공유하는 값 (원래 Update의 지역 변수)
	private int _prevKeyValue;

	public override void Update()
	{
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
				_prevKeyValue = -1;
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
				UpdateStartLNHolding(num, num3, num11, list, dictionary);
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
					JudgeManualPlay(num);
				}
				UpdateRows(num, num3, num11, list, dictionary, barlineDestinations, barlineDestinations2, ref flag, ref flag2);
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
	}

	private void SetLastJudged(int lane, string keyName, JudgeType judge)
	{
		int num84 = KeyNameToInt(keyName);
		if (judge == JudgeType.Bad || judge == JudgeType.PassedPoor || _prevKeyValue < num84)
		{
			_prevKeyValue = num84;
			Flags.SetLastJudged(lane);
		}
	}

	// 시작 마디에 걸쳐 있는 롱노트를 처리하고(오토플레이면 누른 상태, 아니면 놓침 판정) 그 노트 위치를 기록한다
	private void UpdateStartLNHolding(double num, double num3, double num11, List<List<Rectangle>> list, Dictionary<int, List<NotePosition>> dictionary)
	{
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
	}
}
