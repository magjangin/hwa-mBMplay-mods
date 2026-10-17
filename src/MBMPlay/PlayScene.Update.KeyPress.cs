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
	// 레인별 키 입력(누름·롱노트 놓음)을 판정 대상 노트에 대해 판정한다
	private void JudgeKeyPresses(List<(int, int)> list4, double num)
	{
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
}
