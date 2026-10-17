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
}
