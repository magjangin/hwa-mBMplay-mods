using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using MBMPlay.Common;
using MistUtils;

namespace MBMPlay;

public static partial class EventTimers
{
	public static void SetJudge(int lane, JudgeType judge)
	{
		if (LaneLNJudge[lane] > 0.0)
		{
			LaneLNJudge[lane] = 0.0;
			if (lane >= 10)
			{
				lnCount2P--;
				if (lane >= 11 && lane <= 13)
				{
					lnCount2PLeft--;
				}
				else if (lane >= 14 && lane <= 16)
				{
					lnCount2PCenter--;
				}
				else if (lane >= 17 && lane <= 19)
				{
					lnCount2PRight--;
				}
			}
			else
			{
				lnCount1P--;
				if (lane >= 1 && lane <= 3)
				{
					lnCount1PLeft--;
				}
				else if (lane >= 4 && lane <= 6)
				{
					lnCount1PCenter--;
				}
				else if (lane >= 7 && lane <= 9)
				{
					lnCount1PRight--;
				}
			}
		}
		LaneJudge[lane] = NowTime;
		if (judge >= JudgeType.Good)
		{
			Bomb[JudgeType.Good][lane] = NowTime;
		}
		if (judge >= JudgeType.Great)
		{
			Bomb[JudgeType.Great][lane] = NowTime;
		}
		if (judge == JudgeType.PGreat)
		{
			Bomb[JudgeType.PGreat][lane] = NowTime;
		}
		switch (judge)
		{
		case JudgeType.Good:
			BombExclusive[JudgeType.PGreat][lane] = 0.0;
			BombExclusive[JudgeType.Great][lane] = 0.0;
			BombExclusive[JudgeType.Good][lane] = NowTime;
			break;
		case JudgeType.Great:
			BombExclusive[JudgeType.PGreat][lane] = 0.0;
			BombExclusive[JudgeType.Great][lane] = NowTime;
			BombExclusive[JudgeType.Good][lane] = 0.0;
			BombExclusiveGreat[JudgeType.PGreat][lane] = 0.0;
			BombExclusiveGreat[JudgeType.Great][lane] = NowTime;
			break;
		case JudgeType.PGreat:
			BombExclusive[JudgeType.PGreat][lane] = NowTime;
			BombExclusive[JudgeType.Great][lane] = 0.0;
			BombExclusive[JudgeType.Good][lane] = 0.0;
			BombExclusiveGreat[JudgeType.PGreat][lane] = NowTime;
			BombExclusiveGreat[JudgeType.Great][lane] = 0.0;
			break;
		}
		if (lane >= 10)
		{
			if (lnCount2P <= 0)
			{
				Player2LNJudge = 0.0;
				Player2Judge = NowTime;
			}
			else
			{
				Player2LNJudge = NowTime;
			}
			if (lane >= 11 && lane <= 13)
			{
				if (lnCount2PLeft <= 0)
				{
					Player2LNJudgeLeft = 0.0;
					Player2JudgeLeft = NowTime;
				}
				else
				{
					Player2LNJudgeLeft = NowTime;
				}
			}
			else if (lane >= 14 && lane <= 16)
			{
				if (lnCount2PCenter <= 0)
				{
					Player2LNJudgeCenter = 0.0;
					Player2JudgeCenter = NowTime;
				}
				else
				{
					Player2LNJudgeCenter = NowTime;
				}
			}
			else if (lane >= 17 && lane <= 19)
			{
				if (lnCount2PRight <= 0)
				{
					Player2LNJudgeRight = 0.0;
					Player2JudgeRight = NowTime;
				}
				else
				{
					Player2LNJudgeRight = NowTime;
				}
			}
		}
		else
		{
			if (lnCount1P <= 0)
			{
				Player1LNJudge = 0.0;
				Player1Judge = NowTime;
			}
			else
			{
				Player1LNJudge = NowTime;
			}
			if (lane >= 1 && lane <= 3)
			{
				if (lnCount1PLeft <= 0)
				{
					Player1LNJudgeLeft = 0.0;
					Player1JudgeLeft = NowTime;
				}
				else
				{
					Player1LNJudgeLeft = NowTime;
				}
			}
			else if (lane >= 4 && lane <= 6)
			{
				if (lnCount1PCenter <= 0)
				{
					Player1LNJudgeCenter = 0.0;
					Player1JudgeCenter = NowTime;
				}
				else
				{
					Player1LNJudgeCenter = NowTime;
				}
			}
			else if (lane >= 7 && lane <= 9)
			{
				if (lnCount1PRight <= 0)
				{
					Player1LNJudgeRight = 0.0;
					Player1JudgeRight = NowTime;
				}
				else
				{
					Player1LNJudgeRight = NowTime;
				}
			}
		}
		if (FirstNoteJudge <= 0.0)
		{
			FirstNoteJudge = NowTime;
		}
	}

	public static void SetLNJudge(int lane)
	{
		LaneJudge[lane] = 0.0;
		LaneLNJudge[lane] = NowTime;
		if (lane >= 10)
		{
			Player2Judge = 0.0;
			Player2LNJudge = NowTime;
			lnCount2P++;
			if (lane >= 11 && lane <= 13)
			{
				Player2JudgeLeft = 0.0;
				Player2LNJudgeLeft = NowTime;
				lnCount2PLeft++;
			}
			else if (lane >= 14 && lane <= 16)
			{
				Player2JudgeCenter = 0.0;
				Player2LNJudgeCenter = NowTime;
				lnCount2PCenter++;
			}
			else if (lane >= 17 && lane <= 19)
			{
				Player2JudgeRight = 0.0;
				Player2LNJudgeRight = NowTime;
				lnCount2PRight++;
			}
		}
		else
		{
			Player1Judge = 0.0;
			Player1LNJudge = NowTime;
			lnCount1P++;
			if (lane >= 1 && lane <= 3)
			{
				Player1JudgeLeft = 0.0;
				Player1LNJudgeLeft = NowTime;
				lnCount1PLeft++;
			}
			else if (lane >= 4 && lane <= 6)
			{
				Player1JudgeCenter = 0.0;
				Player1LNJudgeCenter = NowTime;
				lnCount1PCenter++;
			}
			else if (lane >= 7 && lane <= 9)
			{
				Player1JudgeRight = 0.0;
				Player1LNJudgeRight = NowTime;
				lnCount1PRight++;
			}
		}
	}

	public static void UnsetLNJudge(int lane)
	{
		LaneLNJudge[lane] = 0.0;
		if (lane >= 10)
		{
			Player2LNJudge = 0.0;
			lnCount2P--;
			if (lane >= 11 && lane <= 13)
			{
				Player2LNJudgeLeft = 0.0;
				lnCount2PLeft--;
			}
			else if (lane >= 14 && lane <= 16)
			{
				Player2LNJudgeCenter = 0.0;
				lnCount2PCenter--;
			}
			else if (lane >= 17 && lane <= 19)
			{
				Player2LNJudgeRight = 0.0;
				lnCount2PRight--;
			}
		}
		else
		{
			Player1LNJudge = 0.0;
			lnCount1P--;
			if (lane >= 1 && lane <= 3)
			{
				Player1LNJudgeLeft = 0.0;
				lnCount1PLeft--;
			}
			else if (lane >= 4 && lane <= 6)
			{
				Player1LNJudgeCenter = 0.0;
				lnCount1PCenter--;
			}
			else if (lane >= 7 && lane <= 9)
			{
				Player1LNJudgeRight = 0.0;
				lnCount1PRight--;
			}
		}
	}

	public static void ResetPlayingTimers()
	{
		foreach (PlayingKeys value in Enum.GetValues(typeof(PlayingKeys)))
		{
			KeyDown[value] = 0.0;
			KeyDownAlt[value] = 0.0;
			KeyUp[value] = 0.0;
		}
		PlayingKeyState.ResetScratchTimers();
		for (int i = 0; i < 20; i++)
		{
			LaneJudge[i] = 0.0;
			LaneLNJudge[i] = 0.0;
			LaneLNHolding[i] = 0.0;
			foreach (JudgeType key in Bomb.Keys)
			{
				Bomb[key][i] = 0.0;
			}
			foreach (JudgeType key2 in BombExclusive.Keys)
			{
				BombExclusive[key2][i] = 0.0;
			}
			foreach (JudgeType key3 in BombExclusiveGreat.Keys)
			{
				BombExclusiveGreat[key3][i] = 0.0;
			}
		}
		Player1ScratchRotate.Reset();
		Player2ScratchRotate.Reset();
		Player1LaneCoverChanged = 0.0;
		Player2LaneCoverChanged = 0.0;
		Player1Judge = 0.0;
		Player1JudgeLeft = 0.0;
		Player1JudgeCenter = 0.0;
		Player1JudgeRight = 0.0;
		Player1LNJudge = 0.0;
		Player1LNJudgeLeft = 0.0;
		Player1LNJudgeCenter = 0.0;
		Player1LNJudgeRight = 0.0;
		Player2Judge = 0.0;
		Player2JudgeLeft = 0.0;
		Player2JudgeCenter = 0.0;
		Player2JudgeRight = 0.0;
		Player2LNJudge = 0.0;
		Player2LNJudgeLeft = 0.0;
		Player2LNJudgeCenter = 0.0;
		Player2LNJudgeRight = 0.0;
		FirstNoteJudge = 0.0;
		lnCount1P = 0;
		lnCount2P = 0;
		lnCount1PLeft = 0;
		lnCount1PCenter = 0;
		lnCount1PRight = 0;
		lnCount2PLeft = 0;
		lnCount2PCenter = 0;
		lnCount2PRight = 0;
		GrooveGaugeIncrease = 0.0;
		GrooveGaugeMax = 0.0;
		PoorBga = 0.0;
		MaxNotesPerSecondUpdate = 0.0;
	}
}
