using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BeMusic;
using MBMPlay.Common;
using MBMPlay.Platform;
using MistUtils;

namespace MBMPlay;

public static partial class Flags
{
	public static void SetJudge(int lane, JudgeType judge)
	{
		if (judge != JudgeType.PassedPoor)
		{
			LaneLaserJudge[lane] = judge;
		}
		if (judge == JudgeType.None)
		{
			return;
		}
		LaneJudge[lane] = judge;
		if (lane >= 10)
		{
			Player2Judge = judge;
			if (lane >= 11 && lane <= 13)
			{
				Player2JudgeLeft = judge;
			}
			else if (lane >= 14 && lane <= 16)
			{
				Player2JudgeCenter = judge;
			}
			else if (lane >= 17 && lane <= 19)
			{
				Player2JudgeRight = judge;
			}
		}
		else
		{
			Player1Judge = judge;
			if (lane >= 1 && lane <= 3)
			{
				Player1JudgeLeft = judge;
			}
			else if (lane >= 4 && lane <= 6)
			{
				Player1JudgeCenter = judge;
			}
			else if (lane >= 7 && lane <= 9)
			{
				Player1JudgeRight = judge;
			}
		}
	}

	public static void SetLastJudged(int lane)
	{
		if (lane >= 10)
		{
			for (int i = 10; i < 20; i++)
			{
				if (i == lane)
				{
					LastJudged[i] = true;
				}
				else
				{
					LastJudged[i] = false;
				}
			}
			if (lane >= 11 && lane <= 13)
			{
				Player2LastJudgedLeft = true;
				Player2LastJudgedCenter = false;
				Player2LastJudgedRight = false;
			}
			else if (lane >= 14 && lane <= 16)
			{
				Player2LastJudgedLeft = false;
				Player2LastJudgedCenter = true;
				Player2LastJudgedRight = false;
			}
			else if (lane >= 17 && lane <= 19)
			{
				Player2LastJudgedLeft = false;
				Player2LastJudgedCenter = false;
				Player2LastJudgedRight = true;
			}
			return;
		}
		for (int j = 0; j < 10; j++)
		{
			if (j == lane)
			{
				LastJudged[j] = true;
			}
			else
			{
				LastJudged[j] = false;
			}
		}
		if (lane >= 1 && lane <= 3)
		{
			Player1LastJudgedLeft = true;
			Player1LastJudgedCenter = false;
			Player1LastJudgedRight = false;
		}
		else if (lane >= 4 && lane <= 6)
		{
			Player1LastJudgedLeft = false;
			Player1LastJudgedCenter = true;
			Player1LastJudgedRight = false;
		}
		else if (lane >= 7 && lane <= 9)
		{
			Player1LastJudgedLeft = false;
			Player1LastJudgedCenter = false;
			Player1LastJudgedRight = true;
		}
	}

	public static void ResetPlayingFlags()
	{
		for (int i = 0; i < 20; i++)
		{
			LaneJudge[i] = JudgeType.None;
			LaneLaserJudge[i] = JudgeType.None;
			LastJudged[i] = false;
			LNHolding[i] = false;
			LNReleased[i] = false;
			LNHoldOnce[i] = false;
		}
		Player1Judge = JudgeType.None;
		Player2Judge = JudgeType.None;
		Player1JudgeLeft = JudgeType.None;
		Player1JudgeCenter = JudgeType.None;
		Player1JudgeRight = JudgeType.None;
		Player2JudgeLeft = JudgeType.None;
		Player2JudgeCenter = JudgeType.None;
		Player2JudgeRight = JudgeType.None;
		Player1LastJudgedLeft = false;
		Player1LastJudgedCenter = false;
		Player1LastJudgedRight = false;
		Player2LastJudgedLeft = false;
		Player2LastJudgedCenter = false;
		Player2LastJudgedRight = false;
		IsLastNoteJudged = false;
	}
}
