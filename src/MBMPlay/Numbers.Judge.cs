using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MistUtils;

namespace MBMPlay;

public static partial class Numbers
{
	public static int GetLaneTotalNotes(int lane)
	{
		if (TotalNotesPerLane == null || lane >= TotalNotesPerLane.Count)
		{
			return 0;
		}
		return TotalNotesPerLane[lane];
	}

	private static List<Dictionary<JudgeType, JudgeCount>> InitLaneJudgeCount()
	{
		List<Dictionary<JudgeType, JudgeCount>> list = new List<Dictionary<JudgeType, JudgeCount>>();
		for (int i = 0; i < 20; i++)
		{
			list.Add(new Dictionary<JudgeType, JudgeCount>
			{
				{
					JudgeType.PGreat,
					new JudgeCount()
				},
				{
					JudgeType.Great,
					new JudgeCount()
				},
				{
					JudgeType.Good,
					new JudgeCount()
				},
				{
					JudgeType.Bad,
					new JudgeCount()
				},
				{
					JudgeType.PassedPoor,
					new JudgeCount()
				},
				{
					JudgeType.WastedPoor,
					new JudgeCount()
				}
			});
		}
		return list;
	}

	private static int GetLaneNoteCount(int lane)
	{
		return LaneJudgeCount[lane][JudgeType.PGreat].Total + LaneJudgeCount[lane][JudgeType.Great].Total + LaneJudgeCount[lane][JudgeType.Good].Total + LaneJudgeCount[lane][JudgeType.Bad].Total + LaneJudgeCount[lane][JudgeType.PassedPoor].Total;
	}

	private static int GetLaneMissCount(int lane)
	{
		return LaneJudgeCount[lane][JudgeType.Bad].Total + LaneJudgeCount[lane][JudgeType.PassedPoor].Total + LaneJudgeCount[lane][JudgeType.WastedPoor].Total;
	}

	private static int GetLaneFastCount(int lane)
	{
		return LaneJudgeCount[lane][JudgeType.PGreat].Fast + LaneJudgeCount[lane][JudgeType.Great].Fast + LaneJudgeCount[lane][JudgeType.Good].Fast + LaneJudgeCount[lane][JudgeType.Bad].Fast + LaneJudgeCount[lane][JudgeType.WastedPoor].Fast;
	}

	private static int GetLaneSlowCount(int lane)
	{
		return LaneJudgeCount[lane][JudgeType.PGreat].Slow + LaneJudgeCount[lane][JudgeType.Great].Slow + LaneJudgeCount[lane][JudgeType.Good].Slow + LaneJudgeCount[lane][JudgeType.Bad].Slow + LaneJudgeCount[lane][JudgeType.WastedPoor].Slow + LaneJudgeCount[lane][JudgeType.PassedPoor].Total;
	}

	private static int GetLaneFastCountWithoutPGreat(int lane)
	{
		return LaneJudgeCount[lane][JudgeType.Great].Fast + LaneJudgeCount[lane][JudgeType.Good].Fast + LaneJudgeCount[lane][JudgeType.Bad].Fast + LaneJudgeCount[lane][JudgeType.WastedPoor].Fast;
	}

	private static int GetLaneSlowCountWithoutPGreat(int lane)
	{
		return LaneJudgeCount[lane][JudgeType.Great].Slow + LaneJudgeCount[lane][JudgeType.Good].Slow + LaneJudgeCount[lane][JudgeType.Bad].Slow + LaneJudgeCount[lane][JudgeType.WastedPoor].Slow + LaneJudgeCount[lane][JudgeType.PassedPoor].Total;
	}

	public static void SetJudge(int lane, JudgeType judge, double time)
	{
		if (judge == JudgeType.None)
		{
			return;
		}
		if (judge != JudgeType.PassedPoor && time > 0.0)
		{
			JudgeCount[judge].Fast++;
			LaneJudgeCount[lane][judge].Fast++;
			if (lane >= 10)
			{
				if (judge != JudgeType.PGreat)
				{
					Player2TotalFastCountWithoutPGreat++;
				}
				Player2TotalFastCount++;
				Player2JudgeCount[judge].Fast++;
			}
			else
			{
				if (judge != JudgeType.PGreat)
				{
					Player1TotalFastCountWithoutPGreat++;
				}
				Player1TotalFastCount++;
				Player1JudgeCount[judge].Fast++;
			}
		}
		else
		{
			JudgeCount[judge].Slow++;
			LaneJudgeCount[lane][judge].Slow++;
			if (lane >= 10)
			{
				if (judge != JudgeType.PGreat)
				{
					Player2TotalSlowCountWithoutPGreat++;
				}
				Player2TotalSlowCount++;
				Player2JudgeCount[judge].Slow++;
			}
			else
			{
				if (judge != JudgeType.PGreat)
				{
					Player1TotalSlowCountWithoutPGreat++;
				}
				Player1TotalSlowCount++;
				Player1JudgeCount[judge].Slow++;
			}
		}
		if (judge != JudgeType.WastedPoor)
		{
			LaneNoteCount[lane]++;
			if (lane >= 10)
			{
				Player2NoteCount++;
			}
			else
			{
				Player1NoteCount++;
			}
			if (judge >= JudgeType.Good)
			{
				_judgeTimingSum += time;
				_judgeTimingCount++;
				ComboCount++;
			}
			else if (judge == JudgeType.Bad || judge == JudgeType.PassedPoor)
			{
				ComboCount = 0;
				ComboBreakCount++;
			}
			if (lane >= 10)
			{
				Player2ComboCount = ComboCount;
			}
			else
			{
				Player1ComboCount = ComboCount;
			}
		}
		LastJudgeTime = time;
		LaneLastJudgeTime[lane] = time;
		if (lane >= 10)
		{
			Player2LastJudgeTime = time;
			if (lane >= 11 && lane <= 13)
			{
				Player2LastJudgeTimeLeft = time;
			}
			else if (lane >= 14 && lane <= 16)
			{
				Player2LastJudgeTimeCenter = time;
			}
			else if (lane >= 17 && lane <= 19)
			{
				Player2LastJudgeTimeRight = time;
			}
		}
		else
		{
			Player1LastJudgeTime = time;
			if (lane >= 1 && lane <= 3)
			{
				Player1LastJudgeTimeLeft = time;
			}
			else if (lane >= 4 && lane <= 6)
			{
				Player1LastJudgeTimeCenter = time;
			}
			else if (lane >= 7 && lane <= 9)
			{
				Player1LastJudgeTimeRight = time;
			}
		}
	}

	public static void ClearJudgeCount()
	{
		foreach (JudgeType key in JudgeCount.Keys)
		{
			JudgeCount[key].Fast = 0;
			JudgeCount[key].Slow = 0;
			Player1JudgeCount[key].Fast = 0;
			Player1JudgeCount[key].Slow = 0;
			Player2JudgeCount[key].Fast = 0;
			Player2JudgeCount[key].Slow = 0;
		}
		for (int i = 0; i < 20; i++)
		{
			foreach (JudgeType key2 in LaneJudgeCount[i].Keys)
			{
				LaneJudgeCount[i][key2].Fast = 0;
				LaneJudgeCount[i][key2].Slow = 0;
			}
			LaneNoteCount[i] = 0;
			LaneLastJudgeTime[i] = 0.0;
		}
		Player1NoteCount = 0;
		Player2NoteCount = 0;
		Player1TotalFastCount = 0;
		Player1TotalSlowCount = 0;
		Player1TotalFastCountWithoutPGreat = 0;
		Player1TotalSlowCountWithoutPGreat = 0;
		Player2TotalFastCount = 0;
		Player2TotalSlowCount = 0;
		Player2TotalFastCountWithoutPGreat = 0;
		Player2TotalSlowCountWithoutPGreat = 0;
		Player1ComboCount = 0;
		Player2ComboCount = 0;
		ComboCount = 0;
		MaxCombo = 0;
		ComboBreakCount = 0;
		LastJudgeTime = 0.0;
		Player1LastJudgeTime = 0.0;
		Player1LastJudgeTimeLeft = 0.0;
		Player1LastJudgeTimeCenter = 0.0;
		Player1LastJudgeTimeRight = 0.0;
		Player2LastJudgeTime = 0.0;
		Player2LastJudgeTimeLeft = 0.0;
		Player2LastJudgeTimeCenter = 0.0;
		Player2LastJudgeTimeRight = 0.0;
		_judgeTimingSum = 0.0;
		_judgeTimingCount = 0;
	}
}
