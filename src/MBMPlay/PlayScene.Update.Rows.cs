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
	// 시간표의 각 행을 돌며 노트·마디선 위치를 계산하고, 오토플레이와 BGM/BGA/BPM/STOP 등 채널 이벤트를 처리한다
	private void UpdateRows(double num, double num3, double num11, List<List<Rectangle>> list, Dictionary<int, List<NotePosition>> dictionary, List<Rectangle> barlineDestinations, List<Rectangle> barlineDestinations2, ref bool flag, ref bool flag2)
	{
		double num4 = 0.0;
		double num5 = 0.0;
		double num6 = 0.0;
		double num7 = 0.0;
		double num8 = 0.0;
		double num9 = 0.0;
		double num10 = 0.0;
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
							if (_prevKeyValue < KeyNameToInt(_timeTable.Rows[num50].Lanes[num55][num56].Value))
							{
								_prevKeyValue = KeyNameToInt(_timeTable.Rows[num50].Lanes[num55][num56].Value);
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
								if (_prevKeyValue < KeyNameToInt(_timeTable.Rows[num50].Lanes[num55][num56].Value))
								{
									_prevKeyValue = KeyNameToInt(_timeTable.Rows[num50].Lanes[num55][num56].Value);
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
							if (_prevKeyValue < KeyNameToInt(_timeTable.Rows[num50].Lanes[num55][num56].Value))
							{
								_prevKeyValue = KeyNameToInt(_timeTable.Rows[num50].Lanes[num55][num56].Value);
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
	}
}
