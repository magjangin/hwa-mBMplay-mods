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
	// 수동 플레이 판정: 레인별 판정 대상 노트를 찾고, 판정 범위를 지난 노트를 놓침 처리한 뒤 키 입력을 판정한다
	private void JudgeManualPlay(double num)
	{
		List<int> list2 = new List<int>(_laneIndex);
		List<int> list3 = new List<int>();
		int num19 = _timeTable.BarIndex[StartBar];
		List<(int, int)> list4 = new List<(int, int)>(_timeTable.LaneCount);
		for (int num20 = 0; num20 < _timeTable.LaneCount; num20++)
		{
			list4.Add((-1, -1));
		}
		FindJudgeTargets(list2, list3, list4, num, num19);
		_laneIndex = list3;
		JudgePassedNotes(num, num19);
		JudgeKeyPresses(list4, num);
	}

	// 레인마다 다음에 판정할 노트를 찾아 list4에 기록하고, 키를 눌렀을 때 낼 키음(_lastKeySounds)을 정한다
	private void FindJudgeTargets(List<int> list2, List<int> list3, List<(int, int)> list4, double num, int num19)
	{
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
	}

	// 판정 범위를 지나쳐 버린 노트를 놓침 처리한다
	private void JudgePassedNotes(double num, int num19)
	{
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
	}
}
