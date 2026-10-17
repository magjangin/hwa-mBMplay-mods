using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MistUtils;

namespace MBMPlay;

public static partial class Numbers
{
	private static int masterVolume = 100;

	private static int keyVolume = 100;

	private static int bgmVolume = 100;

	private static readonly ReadOnlyCollection<int> playStyleList = new List<int> { 5, 7, 9, 10, 14 }.AsReadOnly();

	private static int playStyle = 7;

	private static int musicSpeedLevel = 0;

	private static readonly int LaneCoverMax = 10000;

	private static readonly int LaneCoverMin = 0;

	private static int _player1LaneCover = 0;

	private static int _player2LaneCover = 0;

	private static readonly int LiftMax = 5000;

	private static readonly int LiftMin = 0;

	private static int _player1Lift = 0;

	private static int _player2Lift = 0;

	private static double gauge = 0.0;

	public static Dictionary<JudgeType, JudgeCount> JudgeCount = new Dictionary<JudgeType, JudgeCount>
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
	};

	public static Dictionary<JudgeType, JudgeCount> Player1JudgeCount = new Dictionary<JudgeType, JudgeCount>
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
	};

	public static Dictionary<JudgeType, JudgeCount> Player2JudgeCount = new Dictionary<JudgeType, JudgeCount>
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
	};

	public static List<Dictionary<JudgeType, JudgeCount>> LaneJudgeCount = InitLaneJudgeCount();

	public static List<int> LaneNoteCount = InitLaneList<int>();

	private static int _comboCount;

	private static double _judgeTimingSum = 0.0;

	private static int _judgeTimingCount = 0;

	private static int notesPerSecondSum = 0;

	private static int notesPerSecondSumCount = 0;

	private static double notesPerSecondTime = 0.0;

	private static bool prevLastJudged = false;

	private static int frames = 0;

	private static double fpsStartTime = 0.0;

	private static int updateCount = 0;

	private static double updateStartTime = 0.0;

	private static readonly ReadOnlyDictionary<string, Func<int, int>> IntValueDictionary = CreateIntValueDictionary();

	private static readonly ReadOnlyDictionary<string, Func<double>> DoubleValueDictionary = Extensions.AsReadOnly(new Dictionary<string, Func<double>>
	{
		{ "test", () => TestValue },
		{ "bpm", () => Bpm },
		{ "max_bpm", () => MaxBpm },
		{ "min_bpm", () => MinBpm },
		{ "main_notes_bpm", () => MainNotesBpm },
		{ "main_time_bpm", () => MainTimeBpm },
		{ "1p_hs", () => Player1HiSpeed },
		{ "2p_hs", () => Player2HiSpeed },
		{ "1p_lanecover", () => (double)Player1LaneCover / 100.0 },
		{ "2p_lanecover", () => (double)Player2LaneCover / 100.0 },
		{ "1p_lift", () => (double)Player1Lift / 100.0 },
		{ "2p_lift", () => (double)Player2Lift / 100.0 },
		{ "music_speed", () => MusicSpeed },
		{ "gauge_total", () => GaugeTotal },
		{ "groove_gauge", () => GrooveGauge },
		{ "gauge_total_increment", () => GaugeTotalIncrement },
		{ "load_percentage", () => LoadPercentage },
		{ "music_time_position", () => MusicTimePosition },
		{ "judge_timing", () => JudgeTiming },
		{ "average_judge_timing", () => AverageJudgeTiming },
		{ "average_notes_per_second", () => AverageNotesPerSecond },
		{ "score_rate", () => ScoreRate }
	});

	public static double TestValue { get; set; } = 0.0;

	public static double JudgeTiming { get; set; } = 0.0;

	public static double NoteTiming { get; set; } = 0.0;

	public static int MasterVolume
	{
		get
		{
			return masterVolume;
		}
		set
		{
			if (value < 0)
			{
				masterVolume = 0;
			}
			else if (value > 100)
			{
				masterVolume = 100;
			}
			else
			{
				masterVolume = value;
			}
			OnMasterVolumeChanged();
		}
	}

	public static int KeyVolume
	{
		get
		{
			return keyVolume;
		}
		set
		{
			if (value < 0)
			{
				keyVolume = 0;
			}
			else if (value > 100)
			{
				keyVolume = 100;
			}
			else
			{
				keyVolume = value;
			}
		}
	}

	public static int BgmVolume
	{
		get
		{
			return bgmVolume;
		}
		set
		{
			if (value < 0)
			{
				bgmVolume = 0;
			}
			else if (value > 100)
			{
				bgmVolume = 100;
			}
			else
			{
				bgmVolume = value;
			}
		}
	}

	public static double Bpm { get; set; } = 130.0;

	public static double MaxBpm { get; set; } = 130.0;

	public static double MinBpm { get; set; } = 130.0;

	public static double MainNotesBpm { get; set; } = 130.0;

	public static double MainTimeBpm { get; set; } = 130.0;

	public static int PlayStyle
	{
		get
		{
			return playStyle;
		}
		set
		{
			if (playStyleList.Contains(value))
			{
				playStyle = value;
			}
		}
	}

	public static int Difficulty { get; set; }

	public static int PlayLevel { get; set; }

	public static double Player1HiSpeed { get; set; } = 1.0;

	public static double Player2HiSpeed { get; set; } = 1.0;

	public static double MusicSpeed
	{
		get
		{
			if (MusicSpeedLevel == 0)
			{
				return 1.0;
			}
			return Math.Pow(2.0, (double)MusicSpeedLevel / 12.0);
		}
	}

	public static int MusicSpeedLevel
	{
		get
		{
			return musicSpeedLevel;
		}
		set
		{
			if (value < -12)
			{
				musicSpeedLevel = -12;
			}
			else if (value > 12)
			{
				musicSpeedLevel = 12;
			}
			else
			{
				musicSpeedLevel = value;
			}
		}
	}

	public static int Player1LaneCover
	{
		get
		{
			return _player1LaneCover;
		}
		set
		{
			if (value > LaneCoverMax)
			{
				_player1LaneCover = LaneCoverMax;
			}
			else if (value < LaneCoverMin)
			{
				_player1LaneCover = LaneCoverMin;
			}
			else
			{
				_player1LaneCover = value;
			}
		}
	}

	public static int Player2LaneCover
	{
		get
		{
			return _player2LaneCover;
		}
		set
		{
			if (value > LaneCoverMax)
			{
				_player2LaneCover = LaneCoverMax;
			}
			else if (value < LaneCoverMin)
			{
				_player2LaneCover = LaneCoverMin;
			}
			else
			{
				_player2LaneCover = value;
			}
		}
	}

	public static int Player1Lift
	{
		get
		{
			return _player1Lift;
		}
		set
		{
			if (value > LiftMax)
			{
				_player1Lift = LiftMax;
			}
			else if (value < LiftMin)
			{
				_player1Lift = LiftMin;
			}
			else
			{
				_player1Lift = value;
			}
		}
	}

	public static int Player2Lift
	{
		get
		{
			return _player2Lift;
		}
		set
		{
			if (value > LiftMax)
			{
				_player2Lift = LiftMax;
			}
			else if (value < LiftMin)
			{
				_player2Lift = LiftMin;
			}
			else
			{
				_player2Lift = value;
			}
		}
	}

	public static int TotalNotes { get; set; }

	public static int TotalLongNotes { get; set; }

	public static List<int> TotalNotesPerLane { get; set; }

	public static double GaugeTotal { get; set; }

	public static double GrooveGauge
	{
		get
		{
			return gauge;
		}
		set
		{
			if (value < 0.0)
			{
				gauge = 0.0;
			}
			else if (value > 100.0)
			{
				gauge = 100.0;
			}
			else
			{
				gauge = value;
			}
		}
	}

	public static double GrooveGauge2PerStep => Math.Floor(GrooveGauge / 2.0) * 2.0;

	public static double GaugeBorder { get; set; }

	public static int GaugeDivision { get; set; } = 50;

	public static int GaugeBorderIndex { get; set; } = 40;

	public static int GrooveGaugeDivideCount
	{
		get
		{
			double num = 0.0;
			num = ((!(GrooveGauge >= GaugeBorder)) ? (GrooveGauge * ((double)GaugeBorderIndex / GaugeBorder)) : ((double)GaugeBorderIndex + (GrooveGauge - GaugeBorder) * ((double)(GaugeDivision - GaugeBorderIndex) / (100.0 - GaugeBorder))));
			return (int)Math.Floor(num);
		}
	}

	public static double GaugeTotalIncrement { get; set; }

	public static int TotalBars { get; set; }

	public static int CurrentBars { get; set; }

	public static int TotalLoadCount { get; set; }

	public static int CurrentLoadCount { get; set; }

	public static double LoadPercentage
	{
		get
		{
			if (TotalLoadCount == 0)
			{
				return 0.0;
			}
			return (double)CurrentLoadCount / (double)TotalLoadCount * 100.0;
		}
	}

	public static TimeSpan TotalMusicTime { get; set; }

	public static TimeSpan ElapsedMusicTime { get; set; }

	public static double MusicTimePosition
	{
		get
		{
			if (TotalMusicTime.TotalMilliseconds == 0.0)
			{
				return 0.0;
			}
			return ElapsedMusicTime.TotalMilliseconds / TotalMusicTime.TotalMilliseconds * 100.0;
		}
	}

	public static double LastJudgeTime { get; set; } = 0.0;

	public static double Player1LastJudgeTime { get; set; } = 0.0;

	public static double Player2LastJudgeTime { get; set; } = 0.0;

	public static double Player1LastJudgeTimeLeft { get; set; } = 0.0;

	public static double Player1LastJudgeTimeCenter { get; set; } = 0.0;

	public static double Player1LastJudgeTimeRight { get; set; } = 0.0;

	public static double Player2LastJudgeTimeLeft { get; set; } = 0.0;

	public static double Player2LastJudgeTimeCenter { get; set; } = 0.0;

	public static double Player2LastJudgeTimeRight { get; set; } = 0.0;

	public static List<double> LaneLastJudgeTime { get; set; } = InitLaneList<double>();

	public static int Player1NoteCount { get; private set; } = 0;

	public static int Player2NoteCount { get; private set; } = 0;

	public static int Player1MissCount => Player1JudgeCount[JudgeType.Bad].Total + Player1PoorCount;

	public static int Player1MissFastCount => Player1JudgeCount[JudgeType.Bad].Fast + Player1PoorFastCount;

	public static int Player1MissSlowCount => Player1JudgeCount[JudgeType.Bad].Slow + Player1PoorSlowCount;

	public static int Player2MissCount => Player2JudgeCount[JudgeType.Bad].Total + Player2PoorCount;

	public static int Player2MissFastCount => Player2JudgeCount[JudgeType.Bad].Fast + Player2PoorFastCount;

	public static int Player2MissSlowCount => Player2JudgeCount[JudgeType.Bad].Slow + Player2PoorSlowCount;

	public static int Player1PoorCount => Player1JudgeCount[JudgeType.PassedPoor].Total + Player1JudgeCount[JudgeType.WastedPoor].Total;

	public static int Player1PoorFastCount => Player1JudgeCount[JudgeType.WastedPoor].Fast;

	public static int Player1PoorSlowCount => Player1JudgeCount[JudgeType.PassedPoor].Total + Player1JudgeCount[JudgeType.WastedPoor].Slow;

	public static int Player2PoorCount => Player2JudgeCount[JudgeType.PassedPoor].Total + Player2JudgeCount[JudgeType.WastedPoor].Total;

	public static int Player2PoorFastCount => Player2JudgeCount[JudgeType.WastedPoor].Fast;

	public static int Player2PoorSlowCount => Player2JudgeCount[JudgeType.PassedPoor].Total + Player2JudgeCount[JudgeType.WastedPoor].Slow;

	public static int Player1TotalFastCount { get; private set; } = 0;

	public static int Player1TotalSlowCount { get; private set; } = 0;

	public static int Player1TotalFastCountWithoutPGreat { get; private set; } = 0;

	public static int Player1TotalSlowCountWithoutPGreat { get; private set; } = 0;

	public static int Player2TotalFastCount { get; private set; } = 0;

	public static int Player2TotalSlowCount { get; private set; } = 0;

	public static int Player2TotalFastCountWithoutPGreat { get; private set; } = 0;

	public static int Player2TotalSlowCountWithoutPGreat { get; private set; } = 0;

	public static int ExScore => JudgeCount[JudgeType.PGreat].Total * 2 + JudgeCount[JudgeType.Great].Total;

	public static double ScoreRate
	{
		get
		{
			if (TotalNotes == 0)
			{
				return 0.0;
			}
			return (double)ExScore / ((double)TotalNotes * 2.0) * 100.0;
		}
	}

	public static int MissCount => JudgeCount[JudgeType.Bad].Total + PoorCount;

	public static int MissFastCount => JudgeCount[JudgeType.Bad].Fast + PoorFastCount;

	public static int MissSlowCount => JudgeCount[JudgeType.Bad].Slow + PoorSlowCount;

	public static int PoorCount => JudgeCount[JudgeType.PassedPoor].Total + JudgeCount[JudgeType.WastedPoor].Total;

	public static int PoorFastCount => JudgeCount[JudgeType.WastedPoor].Fast;

	public static int PoorSlowCount => JudgeCount[JudgeType.PassedPoor].Total + JudgeCount[JudgeType.WastedPoor].Slow;

	public static int FastCount => Player1TotalFastCount + Player2TotalFastCount;

	public static int SlowCount => Player1TotalSlowCount + Player2TotalSlowCount;

	public static int FastCountWithoutPGreat => Player1TotalFastCountWithoutPGreat + Player2TotalFastCountWithoutPGreat;

	public static int SlowCountWithoutPGreat => Player1TotalSlowCountWithoutPGreat + Player2TotalSlowCountWithoutPGreat;

	public static int ComboCount
	{
		get
		{
			return _comboCount;
		}
		private set
		{
			_comboCount = value;
			if (MaxCombo < value)
			{
				MaxCombo = value;
			}
		}
	}

	public static int Player1ComboCount { get; private set; } = 0;

	public static int Player2ComboCount { get; private set; } = 0;

	public static int MaxCombo { get; private set; } = 0;

	public static int ComboBreakCount { get; private set; } = 0;

	public static double AverageJudgeTiming
	{
		get
		{
			if (_judgeTimingCount != 0)
			{
				return _judgeTimingSum / (double)_judgeTimingCount;
			}
			return 0.0;
		}
	}

	public static int NotesPerSecond { get; private set; }

	public static int MaxNotesPerSecond { get; private set; }

	public static double AverageNotesPerSecond { get; private set; }

	public static int NotesPerSecondCount { get; set; }

	public static int Fps { get; private set; }

	public static int UpdatePerSecond { get; private set; }

	public static event EventHandler MasterVolumeChanged;

	public static int GetValue(string name, int digit)
	{
		if (string.IsNullOrEmpty(name))
		{
			return 0;
		}
		if (IntValueDictionary.ContainsKey(name))
		{
			return IntValueDictionary[name](digit);
		}
		return 0;
	}

	public static double GetValueDouble(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return 0.0;
		}
		if (DoubleValueDictionary.ContainsKey(name))
		{
			return DoubleValueDictionary[name]();
		}
		return GetValue(name, 0);
	}

	public static bool TryGetValueDouble(string name, out double value)
	{
		if (!string.IsNullOrEmpty(name))
		{
			if (DoubleValueDictionary.ContainsKey(name))
			{
				value = DoubleValueDictionary[name]();
				return true;
			}
			if (IntValueDictionary.ContainsKey(name))
			{
				value = IntValueDictionary[name](0);
				return true;
			}
		}
		value = double.NaN;
		return false;
	}

	public static bool TryGetCommandDouble(string name, out Func<double> func)
	{
		if (!string.IsNullOrEmpty(name))
		{
			if (DoubleValueDictionary.ContainsKey(name))
			{
				func = DoubleValueDictionary[name];
				return true;
			}
			if (IntValueDictionary.ContainsKey(name))
			{
				func = () => IntValueDictionary[name](0);
				return true;
			}
		}
		func = null;
		return false;
	}

	internal static int GetIntValue(int num, int digit)
	{
		if (digit < 0)
		{
			int num2 = ((num == 0) ? 1 : ((int)Math.Floor(Math.Log10(Math.Abs(num))) + 1));
			if (Math.Abs(digit) >= num2)
			{
				return num;
			}
			return (int)Math.Truncate((double)num / Math.Pow(10.0, num2 - Math.Abs(digit)));
		}
		return num;
	}

	internal static int GetBeforePointValue(double num, int digit)
	{
		double num2 = num;
		if (num2 > 2147483647.0 || num2 < -2147483648.0)
		{
			num2 -= Math.Truncate(num2 / 1000000000.0) * 1000000000.0;
		}
		return GetIntValue((int)Math.Truncate(new decimal(num2)), digit);
	}

	internal static int GetAfterPointValue(double num, int digit)
	{
		if (num == 0.0)
		{
			return 0;
		}
		double num2 = num;
		if (num2 > 2147483647.0 || num2 < -2147483648.0)
		{
			num2 -= Math.Truncate(num2 / 1000000000.0) * 1000000000.0;
		}
		decimal num3 = new decimal(num2);
		num3 %= 1m;
		if (num3 == 0m)
		{
			return 0;
		}
		if (digit == 0)
		{
			for (int i = 0; i < 9; i++)
			{
				num3 *= 10m;
				if (num3 == Math.Truncate(num3))
				{
					break;
				}
			}
		}
		else
		{
			num3 *= (decimal)Math.Pow(10.0, digit);
		}
		return (int)Math.Truncate(num3);
	}

	internal static void OnMasterVolumeChanged()
	{
		MasterVolumeChanged?.Invoke(null, EventArgs.Empty);
	}

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

	private static List<T> InitLaneList<T>()
	{
		List<T> list = new List<T>();
		for (int i = 0; i < 20; i++)
		{
			list.Add(default);
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

	public static void UpdateNotesPerSecond()
	{
		if (!(EventTimers.FirstNoteJudge > 0.0) || (prevLastJudged && notesPerSecondTime <= 0.0))
		{
			return;
		}
		if (notesPerSecondTime <= 0.0)
		{
			notesPerSecondTime = EventTimers.FirstNoteJudge;
		}
		double nowTime = EventTimers.GetNowTime();
		if (!(nowTime - notesPerSecondTime >= 1000.0))
		{
			return;
		}
		if (!prevLastJudged)
		{
			NotesPerSecond = NotesPerSecondCount;
			NotesPerSecondCount = 0;
			notesPerSecondTime = nowTime;
			int num = Math.Max(NotesPerSecond, MaxNotesPerSecond);
			if (MaxNotesPerSecond < num)
			{
				MaxNotesPerSecond = num;
				EventTimers.MaxNotesPerSecondUpdate = EventTimers.NowTime;
			}
			notesPerSecondSum += NotesPerSecond;
			notesPerSecondSumCount++;
			AverageNotesPerSecond = (double)notesPerSecondSum / (double)notesPerSecondSumCount;
			if (Flags.IsLastNoteJudged)
			{
				prevLastJudged = true;
			}
		}
		else
		{
			NotesPerSecond = 0;
			NotesPerSecondCount = 0;
			notesPerSecondTime = 0.0;
		}
	}

	public static void ResetNotesPerSecond()
	{
		NotesPerSecond = 0;
		MaxNotesPerSecond = 0;
		AverageNotesPerSecond = 0.0;
		NotesPerSecondCount = 0;
		notesPerSecondSum = 0;
		notesPerSecondSumCount = 0;
		notesPerSecondTime = 0.0;
		prevLastJudged = false;
		EventTimers.MaxNotesPerSecondUpdate = 0.0;
	}

	public static void UpdateFps()
	{
		double nowTime = EventTimers.GetNowTime();
		if (fpsStartTime <= 0.0)
		{
			fpsStartTime = nowTime;
		}
		frames++;
		if (nowTime - fpsStartTime >= 1000.0)
		{
			Fps = frames;
			frames = 0;
			fpsStartTime = nowTime;
		}
	}

	public static void UpdateUps()
	{
		double nowTime = EventTimers.GetNowTime();
		if (updateStartTime <= 0.0)
		{
			updateStartTime = nowTime;
		}
		updateCount++;
		if (nowTime - updateStartTime >= 1000.0)
		{
			UpdatePerSecond = updateCount;
			updateCount = 0;
			updateStartTime = nowTime;
		}
	}
}
