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
	private static bool _masterVolMute = false;

	private static readonly ReadOnlyDictionary<string, Func<bool>> FlagDictionary = CreateFlagDictionary();

	public static bool Exiting { get; set; }

	public static bool EnableOpenFileDialog { get; } = PlatformGeneral.EnableOpenFileDialog;

	public static bool AutoPlay { get; set; }

	public static bool Loading { get; set; }

	public static bool LoadComplete { get; set; }

	public static bool LegacyLN { get; set; } = true;

	public static bool DisableBga { get; set; } = false;

	public static bool DisplayFps { get; set; } = false;

	public static bool DisplayNoteCount { get; set; } = true;

	public static bool ContainsLN { get; set; }

	public static bool IsTotalDefined { get; set; }

	public static bool IsJudgeRankDefined { get; set; }

	public static bool IsDifficultyDefined { get; set; }

	public static bool IsPlayer1ScratchDown
	{
		get
		{
			if (!IsKeyDownAny(PlayingKeys.Player1ScratchLeft, PlayingKeys.Player1ScratchRight, PlayingKeys.Player1ScratchToggle))
			{
				if (!AutoPlay)
				{
					return PlayingKeyState.IsPlayer1PlayingScratchRotaryDown;
				}
				return false;
			}
			return true;
		}
	}

	public static bool IsPlayer1ScratchDownOnce
	{
		get
		{
			if (!IsKeyDownOnceAny(PlayingKeys.Player1ScratchLeft, PlayingKeys.Player1ScratchRight, PlayingKeys.Player1ScratchToggle))
			{
				if (!AutoPlay)
				{
					return PlayingKeyState.IsPlayer1PlayingScratchRotaryDownOnce;
				}
				return false;
			}
			return true;
		}
	}

	public static bool IsPlayer1ScratchUp
	{
		get
		{
			if (!IsKeyUpAny(PlayingKeys.Player1ScratchLeft, PlayingKeys.Player1ScratchRight, PlayingKeys.Player1ScratchToggle))
			{
				if (!AutoPlay)
				{
					return PlayingKeyState.IsPlayer1PlayingScratchRotaryUp;
				}
				return false;
			}
			return true;
		}
	}

	public static bool IsPlayer1ScratchUpOnce
	{
		get
		{
			if (!IsKeyUpOnceAny(PlayingKeys.Player1ScratchLeft, PlayingKeys.Player1ScratchRight, PlayingKeys.Player1ScratchToggle))
			{
				if (!AutoPlay)
				{
					return PlayingKeyState.IsPlayer1PlayingScratchRotaryUpOnce;
				}
				return false;
			}
			return true;
		}
	}

	public static bool IsPlayer2ScratchDown
	{
		get
		{
			if (!IsKeyDownAny(PlayingKeys.Player2ScratchLeft, PlayingKeys.Player2ScratchRight, PlayingKeys.Player2ScratchToggle))
			{
				if (!AutoPlay)
				{
					return PlayingKeyState.IsPlayer2PlayingScratchRotaryDown;
				}
				return false;
			}
			return true;
		}
	}

	public static bool IsPlayer2ScratchDownOnce
	{
		get
		{
			if (!IsKeyDownOnceAny(PlayingKeys.Player2ScratchLeft, PlayingKeys.Player2ScratchRight, PlayingKeys.Player2ScratchToggle))
			{
				if (!AutoPlay)
				{
					return PlayingKeyState.IsPlayer2PlayingScratchRotaryDownOnce;
				}
				return false;
			}
			return true;
		}
	}

	public static bool IsPlayer2ScratchUp
	{
		get
		{
			if (!IsKeyUpAny(PlayingKeys.Player2ScratchLeft, PlayingKeys.Player2ScratchRight, PlayingKeys.Player2ScratchToggle))
			{
				if (!AutoPlay)
				{
					return PlayingKeyState.IsPlayer2PlayingScratchRotaryUp;
				}
				return false;
			}
			return true;
		}
	}

	public static bool IsPlayer2ScratchUpOnce
	{
		get
		{
			if (!IsKeyUpOnceAny(PlayingKeys.Player2ScratchLeft, PlayingKeys.Player2ScratchRight, PlayingKeys.Player2ScratchToggle))
			{
				if (!AutoPlay)
				{
					return PlayingKeyState.IsPlayer2PlayingScratchRotaryUpOnce;
				}
				return false;
			}
			return true;
		}
	}

	public static ScratchDirection Player1ScratchToggleDirection
	{
		get
		{
			if (!AutoPlay)
			{
				return PlayingKeyState.Player1ScratchToggleDirection;
			}
			return AutoPlayKeyState.Player1ScratchToggleDirection;
		}
	}

	public static ScratchDirection Player2ScratchToggleDirection
	{
		get
		{
			if (!AutoPlay)
			{
				return PlayingKeyState.Player2ScratchToggleDirection;
			}
			return AutoPlayKeyState.Player2ScratchToggleDirection;
		}
	}

	public static bool Player1LaneCoverEnabled { get; set; }

	public static bool Player2LaneCoverEnabled { get; set; }

	public static List<JudgeType> LaneJudge { get; private set; } = InitLaneJudge();

	public static List<JudgeType> LaneLaserJudge { get; private set; } = InitLaneJudge();

	public static JudgeType Player1Judge { get; private set; } = JudgeType.None;

	public static JudgeType Player2Judge { get; private set; } = JudgeType.None;

	public static JudgeType Player1JudgeLeft { get; private set; } = JudgeType.None;

	public static JudgeType Player1JudgeCenter { get; private set; } = JudgeType.None;

	public static JudgeType Player1JudgeRight { get; private set; } = JudgeType.None;

	public static JudgeType Player2JudgeLeft { get; private set; } = JudgeType.None;

	public static JudgeType Player2JudgeCenter { get; private set; } = JudgeType.None;

	public static JudgeType Player2JudgeRight { get; private set; } = JudgeType.None;

	public static List<bool> LastJudged { get; private set; } = InitLaneFlag();

	public static bool Player1LastJudgedLeft { get; private set; }

	public static bool Player1LastJudgedCenter { get; private set; }

	public static bool Player1LastJudgedRight { get; private set; }

	public static bool Player2LastJudgedLeft { get; private set; }

	public static bool Player2LastJudgedCenter { get; private set; }

	public static bool Player2LastJudgedRight { get; private set; }

	public static List<bool> LNHolding { get; private set; } = InitLaneFlag();

	public static List<bool> LNReleased { get; private set; } = InitLaneFlag();

	public static List<bool> LNHoldOnce { get; private set; } = InitLaneFlag();

	public static bool IsLastNoteJudged { get; set; }

	public static bool IsMusicFinished => Numbers.TotalMusicTime <= Numbers.ElapsedMusicTime;

	public static bool IsPlayStyle5 => Numbers.PlayStyle == 5;

	public static bool IsPlayStyle7 => Numbers.PlayStyle == 7;

	public static bool IsPlayStyle9 => Numbers.PlayStyle == 9;

	public static bool IsPlayStyle10 => Numbers.PlayStyle == 10;

	public static bool IsPlayStyle14 => Numbers.PlayStyle == 14;

	public static bool IsBpmVariable => !Extensions.EpsilonEquals(Numbers.MaxBpm, Numbers.MinBpm);

	public static bool IsGaugeMax => Numbers.GrooveGauge >= 100.0;

	public static bool IsGaugeUnder9 => Numbers.GrooveGauge < 10.0;

	public static bool IsGaugeZero => Numbers.GrooveGauge <= 0.0;

	public static bool IsGaugeOverBorder => Numbers.GrooveGauge >= Numbers.GaugeBorder;

	public static HiSpeedFixType HiSpeedFixType { get; set; } = HiSpeedFixType.None;

	public static JudgeRank JudgeRank { get; set; } = JudgeRank.Easy;

	public static PlaySide PlaySide { get; set; } = PlaySide.Left;

	public static bool IsMasterVolumeMute
	{
		get
		{
			return _masterVolMute;
		}
		set
		{
			if (_masterVolMute != value)
			{
				_masterVolMute = value;
				Numbers.OnMasterVolumeChanged();
			}
		}
	}

	public static bool IsKeyVolumeMute { get; set; } = false;

	public static bool IsBgmVolumeMute { get; set; } = false;

	public static bool IsMasterVolumeMax => (double)Numbers.MasterVolume >= 100.0;

	public static bool IsMasterVolumeUnder9
	{
		get
		{
			if ((double)Numbers.MasterVolume < 10.0)
			{
				return (double)Numbers.MasterVolume > 0.0;
			}
			return false;
		}
	}

	public static bool IsMasterVolumeZero => (double)Numbers.MasterVolume <= 0.0;

	public static bool IsKeyVolumeMax => (double)Numbers.KeyVolume >= 100.0;

	public static bool IsKeyVolumeUnder9
	{
		get
		{
			if ((double)Numbers.KeyVolume < 10.0)
			{
				return (double)Numbers.KeyVolume > 0.0;
			}
			return false;
		}
	}

	public static bool IsKeyVolumeZero => (double)Numbers.KeyVolume <= 0.0;

	public static bool IsBgmVolumeMax => (double)Numbers.BgmVolume >= 100.0;

	public static bool IsBgmVolumeUnder9
	{
		get
		{
			if ((double)Numbers.BgmVolume < 10.0)
			{
				return (double)Numbers.BgmVolume > 0.0;
			}
			return false;
		}
	}

	public static bool IsBgmVolumeZero => (double)Numbers.BgmVolume <= 0.0;

	public static bool PoorBga => EventTimers.PoorBga > 0.0;

	public static LaneSwapOption Player1LaneSwapOption { get; set; } = LaneSwapOption.None;

	public static LaneSwapOption Player2LaneSwapOption { get; set; } = LaneSwapOption.None;

	public static bool FlipSide { get; set; } = false;

	public static bool CheckDifficulty(int difficulty)
	{
		if (!IsDifficultyDefined)
		{
			return false;
		}
		return Numbers.Difficulty == difficulty;
	}

	public static bool IsLaneContainsNotes(int lane)
	{
		return Numbers.GetLaneTotalNotes(lane) > 0;
	}

	private static List<JudgeType> InitLaneJudge()
	{
		List<JudgeType> list = new List<JudgeType>();
		for (int i = 0; i < 20; i++)
		{
			list.Add(JudgeType.None);
		}
		return list;
	}

	private static bool IsJudgeTimingFast(double time)
	{
		return time > 0.0;
	}

	private static bool IsJudgeTimingSlow(double time)
	{
		return time <= 0.0;
	}

	private static List<bool> InitLaneFlag()
	{
		List<bool> list = new List<bool>();
		for (int i = 0; i < 20; i++)
		{
			list.Add(item: false);
		}
		return list;
	}

	private static bool IsNumberBetween(int num, int max, int min)
	{
		if (num < max)
		{
			return num >= min;
		}
		return false;
	}

	private static bool IsNumberBetween(double num, double max, double min)
	{
		if (num < max)
		{
			return num >= min;
		}
		return false;
	}
}
