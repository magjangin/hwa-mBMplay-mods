using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MistUtils;

namespace MBMPlay;

public static class Numbers
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

	private static readonly ReadOnlyDictionary<string, Func<int, int>> IntValueDictionary = Extensions.AsReadOnly(new Dictionary<string, Func<int, int>>
	{
		{
			"volume_master",
			(int digit) => GetIntValue(MasterVolume, digit)
		},
		{
			"volume_key",
			(int digit) => GetIntValue(KeyVolume, digit)
		},
		{
			"volume_bgm",
			(int digit) => GetIntValue(BgmVolume, digit)
		},
		{
			"bpm",
			(int digit) => GetBeforePointValue(Bpm, digit)
		},
		{
			"bpm_point",
			(int digit) => GetAfterPointValue(Bpm, Math.Abs(digit))
		},
		{
			"max_bpm",
			(int digit) => GetBeforePointValue(MaxBpm, digit)
		},
		{
			"max_bpm_point",
			(int digit) => GetAfterPointValue(MaxBpm, Math.Abs(digit))
		},
		{
			"min_bpm",
			(int digit) => GetBeforePointValue(MinBpm, digit)
		},
		{
			"min_bpm_point",
			(int digit) => GetAfterPointValue(MinBpm, Math.Abs(digit))
		},
		{
			"main_notes_bpm",
			(int digit) => GetBeforePointValue(MainNotesBpm, digit)
		},
		{
			"main_notes_bpm_point",
			(int digit) => GetAfterPointValue(MainNotesBpm, Math.Abs(digit))
		},
		{
			"main_time_bpm",
			(int digit) => GetBeforePointValue(MainTimeBpm, digit)
		},
		{
			"main_time_bpm_point",
			(int digit) => GetAfterPointValue(MainTimeBpm, Math.Abs(digit))
		},
		{
			"playlevel",
			(int digit) => GetIntValue(PlayLevel, digit)
		},
		{
			"music_speed_level",
			(int digit) => GetIntValue(MusicSpeedLevel, digit)
		},
		{
			"music_speed",
			(int digit) => GetBeforePointValue(MusicSpeed, digit)
		},
		{
			"music_speed_point",
			(int digit) => GetAfterPointValue(MusicSpeed, Math.Abs(digit))
		},
		{
			"combo_count",
			(int digit) => GetIntValue(ComboCount, digit)
		},
		{
			"max_combo",
			(int digit) => GetIntValue(MaxCombo, digit)
		},
		{
			"total_notes",
			(int digit) => GetIntValue(TotalNotes, digit)
		},
		{
			"total_ln",
			(int digit) => GetIntValue(TotalLongNotes, digit)
		},
		{
			"1p_key1_total_notes",
			(int digit) => GetIntValue(GetLaneTotalNotes(1), digit)
		},
		{
			"1p_key2_total_notes",
			(int digit) => GetIntValue(GetLaneTotalNotes(2), digit)
		},
		{
			"1p_key3_total_notes",
			(int digit) => GetIntValue(GetLaneTotalNotes(3), digit)
		},
		{
			"1p_key4_total_notes",
			(int digit) => GetIntValue(GetLaneTotalNotes(4), digit)
		},
		{
			"1p_key5_total_notes",
			(int digit) => GetIntValue(GetLaneTotalNotes(5), digit)
		},
		{
			"1p_key6_total_notes",
			(int digit) => GetIntValue(GetLaneTotalNotes(6), digit)
		},
		{
			"1p_key7_total_notes",
			(int digit) => GetIntValue(GetLaneTotalNotes(7), digit)
		},
		{
			"1p_key8_total_notes",
			(int digit) => GetIntValue(GetLaneTotalNotes(8), digit)
		},
		{
			"1p_key9_total_notes",
			(int digit) => GetIntValue(GetLaneTotalNotes(9), digit)
		},
		{
			"1p_scratch_total_notes",
			(int digit) => GetIntValue(GetLaneTotalNotes(0), digit)
		},
		{
			"2p_key1_total_notes",
			(int digit) => GetIntValue(GetLaneTotalNotes(11), digit)
		},
		{
			"2p_key2_total_notes",
			(int digit) => GetIntValue(GetLaneTotalNotes(12), digit)
		},
		{
			"2p_key3_total_notes",
			(int digit) => GetIntValue(GetLaneTotalNotes(13), digit)
		},
		{
			"2p_key4_total_notes",
			(int digit) => GetIntValue(GetLaneTotalNotes(14), digit)
		},
		{
			"2p_key5_total_notes",
			(int digit) => GetIntValue(GetLaneTotalNotes(15), digit)
		},
		{
			"2p_key6_total_notes",
			(int digit) => GetIntValue(GetLaneTotalNotes(16), digit)
		},
		{
			"2p_key7_total_notes",
			(int digit) => GetIntValue(GetLaneTotalNotes(17), digit)
		},
		{
			"2p_key8_total_notes",
			(int digit) => GetIntValue(GetLaneTotalNotes(18), digit)
		},
		{
			"2p_key9_total_notes",
			(int digit) => GetIntValue(GetLaneTotalNotes(19), digit)
		},
		{
			"2p_scratch_total_notes",
			(int digit) => GetIntValue(GetLaneTotalNotes(10), digit)
		},
		{
			"gauge_total",
			(int digit) => GetBeforePointValue(GaugeTotal, digit)
		},
		{
			"gauge_total_point",
			(int digit) => GetAfterPointValue(GaugeTotal, digit)
		},
		{
			"groove_gauge",
			(int digit) => GetBeforePointValue(GrooveGauge, digit)
		},
		{
			"groove_gauge_point",
			(int digit) => GetAfterPointValue(GrooveGauge, digit)
		},
		{
			"groove_gauge_2per",
			(int digit) => GetBeforePointValue(GrooveGauge2PerStep, digit)
		},
		{
			"gauge_total_increment",
			(int digit) => GetBeforePointValue(GaugeTotalIncrement, digit)
		},
		{
			"gauge_total_increment_point",
			(int digit) => GetAfterPointValue(GaugeTotalIncrement, digit)
		},
		{
			"total_bars",
			(int digit) => GetIntValue(TotalBars, digit)
		},
		{
			"current_bars",
			(int digit) => GetIntValue(CurrentBars, digit)
		},
		{
			"total_load_count",
			(int digit) => GetIntValue(TotalLoadCount, digit)
		},
		{
			"current_load_count",
			(int digit) => GetIntValue(CurrentLoadCount, digit)
		},
		{
			"load_percentage",
			(int digit) => GetBeforePointValue(LoadPercentage, digit)
		},
		{
			"load_percentage_point",
			(int digit) => GetAfterPointValue(LoadPercentage, digit)
		},
		{
			"total_music_time_minutes",
			(int digit) => GetIntValue((int)Math.Floor(TotalMusicTime.TotalMinutes), digit)
		},
		{
			"total_music_time_seconds",
			(int digit) => GetIntValue(TotalMusicTime.Seconds, digit)
		},
		{
			"elapsed_music_time_minutes",
			(int digit) => GetIntValue((int)Math.Floor(ElapsedMusicTime.TotalMinutes), digit)
		},
		{
			"elapsed_music_time_seconds",
			(int digit) => GetIntValue(ElapsedMusicTime.Seconds, digit)
		},
		{
			"music_time_position",
			(int digit) => GetBeforePointValue(MusicTimePosition, digit)
		},
		{
			"music_time_position_point",
			(int digit) => GetAfterPointValue(MusicTimePosition, digit)
		},
		{
			"1p_hs",
			(int digit) => GetBeforePointValue(Player1HiSpeed, digit)
		},
		{
			"1p_hs_point",
			(int digit) => GetAfterPointValue(Player1HiSpeed, Math.Abs(digit))
		},
		{
			"2p_hs",
			(int digit) => GetBeforePointValue(Player2HiSpeed, digit)
		},
		{
			"2p_hs_point",
			(int digit) => GetAfterPointValue(Player2HiSpeed, Math.Abs(digit))
		},
		{
			"1p_lanecover",
			(int digit) => GetIntValue(Player1LaneCover / 100, digit)
		},
		{
			"1p_lanecover_point",
			(int digit) => GetIntValue(Player1LaneCover % 100, digit * -1)
		},
		{
			"2p_lanecover",
			(int digit) => GetIntValue(Player2LaneCover / 100, digit)
		},
		{
			"2p_lanecover_point",
			(int digit) => GetIntValue(Player2LaneCover % 100, digit * -1)
		},
		{
			"1p_lift",
			(int digit) => GetIntValue(Player1Lift / 100, digit)
		},
		{
			"1p_lift_point",
			(int digit) => GetIntValue(Player1Lift % 100, digit * -1)
		},
		{
			"2p_lift",
			(int digit) => GetIntValue(Player2Lift / 100, digit)
		},
		{
			"2p_lift_point",
			(int digit) => GetIntValue(Player2Lift % 100, digit * -1)
		},
		{
			"1p_combo_count",
			(int digit) => GetIntValue(Player1ComboCount, digit)
		},
		{
			"2p_combo_count",
			(int digit) => GetIntValue(Player2ComboCount, digit)
		},
		{
			"1p_misscount",
			(int digit) => GetIntValue(Player1MissCount, digit)
		},
		{
			"1p_misscount_fast",
			(int digit) => GetIntValue(Player1MissFastCount, digit)
		},
		{
			"1p_misscount_slow",
			(int digit) => GetIntValue(Player1MissSlowCount, digit)
		},
		{
			"2p_misscount",
			(int digit) => GetIntValue(Player2MissCount, digit)
		},
		{
			"2p_misscount_fast",
			(int digit) => GetIntValue(Player2MissFastCount, digit)
		},
		{
			"2p_misscount_slow",
			(int digit) => GetIntValue(Player2MissSlowCount, digit)
		},
		{
			"1p_note_count",
			(int digit) => GetIntValue(Player1NoteCount, digit)
		},
		{
			"2p_note_count",
			(int digit) => GetIntValue(Player2NoteCount, digit)
		},
		{
			"1p_key1_note_count",
			(int digit) => GetIntValue(LaneNoteCount[1], digit)
		},
		{
			"1p_key2_note_count",
			(int digit) => GetIntValue(LaneNoteCount[2], digit)
		},
		{
			"1p_key3_note_count",
			(int digit) => GetIntValue(LaneNoteCount[3], digit)
		},
		{
			"1p_key4_note_count",
			(int digit) => GetIntValue(LaneNoteCount[4], digit)
		},
		{
			"1p_key5_note_count",
			(int digit) => GetIntValue(LaneNoteCount[5], digit)
		},
		{
			"1p_key6_note_count",
			(int digit) => GetIntValue(LaneNoteCount[6], digit)
		},
		{
			"1p_key7_note_count",
			(int digit) => GetIntValue(LaneNoteCount[7], digit)
		},
		{
			"1p_key8_note_count",
			(int digit) => GetIntValue(LaneNoteCount[8], digit)
		},
		{
			"1p_key9_note_count",
			(int digit) => GetIntValue(LaneNoteCount[9], digit)
		},
		{
			"1p_scratch_note_count",
			(int digit) => GetIntValue(LaneNoteCount[0], digit)
		},
		{
			"2p_key1_note_count",
			(int digit) => GetIntValue(LaneNoteCount[11], digit)
		},
		{
			"2p_key2_note_count",
			(int digit) => GetIntValue(LaneNoteCount[12], digit)
		},
		{
			"2p_key3_note_count",
			(int digit) => GetIntValue(LaneNoteCount[13], digit)
		},
		{
			"2p_key4_note_count",
			(int digit) => GetIntValue(LaneNoteCount[14], digit)
		},
		{
			"2p_key5_note_count",
			(int digit) => GetIntValue(LaneNoteCount[15], digit)
		},
		{
			"2p_key6_note_count",
			(int digit) => GetIntValue(LaneNoteCount[16], digit)
		},
		{
			"2p_key7_note_count",
			(int digit) => GetIntValue(LaneNoteCount[17], digit)
		},
		{
			"2p_key8_note_count",
			(int digit) => GetIntValue(LaneNoteCount[18], digit)
		},
		{
			"2p_key9_note_count",
			(int digit) => GetIntValue(LaneNoteCount[19], digit)
		},
		{
			"2p_scratch_note_count",
			(int digit) => GetIntValue(LaneNoteCount[10], digit)
		},
		{
			"1p_pgreat",
			(int digit) => GetIntValue(Player1JudgeCount[JudgeType.PGreat].Total, digit)
		},
		{
			"1p_pgreat_fast",
			(int digit) => GetIntValue(Player1JudgeCount[JudgeType.PGreat].Fast, digit)
		},
		{
			"1p_pgreat_slow",
			(int digit) => GetIntValue(Player1JudgeCount[JudgeType.PGreat].Slow, digit)
		},
		{
			"2p_pgreat",
			(int digit) => GetIntValue(Player2JudgeCount[JudgeType.PGreat].Total, digit)
		},
		{
			"2p_pgreat_fast",
			(int digit) => GetIntValue(Player2JudgeCount[JudgeType.PGreat].Fast, digit)
		},
		{
			"2p_pgreat_slow",
			(int digit) => GetIntValue(Player2JudgeCount[JudgeType.PGreat].Slow, digit)
		},
		{
			"1p_key1_pgreat_fast",
			(int digit) => GetIntValue(LaneJudgeCount[1][JudgeType.PGreat].Fast, digit)
		},
		{
			"1p_key2_pgreat_fast",
			(int digit) => GetIntValue(LaneJudgeCount[2][JudgeType.PGreat].Fast, digit)
		},
		{
			"1p_key3_pgreat_fast",
			(int digit) => GetIntValue(LaneJudgeCount[3][JudgeType.PGreat].Fast, digit)
		},
		{
			"1p_key4_pgreat_fast",
			(int digit) => GetIntValue(LaneJudgeCount[4][JudgeType.PGreat].Fast, digit)
		},
		{
			"1p_key5_pgreat_fast",
			(int digit) => GetIntValue(LaneJudgeCount[5][JudgeType.PGreat].Fast, digit)
		},
		{
			"1p_key6_pgreat_fast",
			(int digit) => GetIntValue(LaneJudgeCount[6][JudgeType.PGreat].Fast, digit)
		},
		{
			"1p_key7_pgreat_fast",
			(int digit) => GetIntValue(LaneJudgeCount[7][JudgeType.PGreat].Fast, digit)
		},
		{
			"1p_key8_pgreat_fast",
			(int digit) => GetIntValue(LaneJudgeCount[8][JudgeType.PGreat].Fast, digit)
		},
		{
			"1p_key9_pgreat_fast",
			(int digit) => GetIntValue(LaneJudgeCount[9][JudgeType.PGreat].Fast, digit)
		},
		{
			"1p_scratch_pgreat_fast",
			(int digit) => GetIntValue(LaneJudgeCount[0][JudgeType.PGreat].Fast, digit)
		},
		{
			"2p_key1_pgreat_fast",
			(int digit) => GetIntValue(LaneJudgeCount[11][JudgeType.PGreat].Fast, digit)
		},
		{
			"2p_key2_pgreat_fast",
			(int digit) => GetIntValue(LaneJudgeCount[12][JudgeType.PGreat].Fast, digit)
		},
		{
			"2p_key3_pgreat_fast",
			(int digit) => GetIntValue(LaneJudgeCount[13][JudgeType.PGreat].Fast, digit)
		},
		{
			"2p_key4_pgreat_fast",
			(int digit) => GetIntValue(LaneJudgeCount[14][JudgeType.PGreat].Fast, digit)
		},
		{
			"2p_key5_pgreat_fast",
			(int digit) => GetIntValue(LaneJudgeCount[15][JudgeType.PGreat].Fast, digit)
		},
		{
			"2p_key6_pgreat_fast",
			(int digit) => GetIntValue(LaneJudgeCount[16][JudgeType.PGreat].Fast, digit)
		},
		{
			"2p_key7_pgreat_fast",
			(int digit) => GetIntValue(LaneJudgeCount[17][JudgeType.PGreat].Fast, digit)
		},
		{
			"2p_key8_pgreat_fast",
			(int digit) => GetIntValue(LaneJudgeCount[18][JudgeType.PGreat].Fast, digit)
		},
		{
			"2p_key9_pgreat_fast",
			(int digit) => GetIntValue(LaneJudgeCount[19][JudgeType.PGreat].Fast, digit)
		},
		{
			"2p_scratch_pgreat_fast",
			(int digit) => GetIntValue(LaneJudgeCount[10][JudgeType.PGreat].Fast, digit)
		},
		{
			"1p_key1_pgreat_slow",
			(int digit) => GetIntValue(LaneJudgeCount[1][JudgeType.PGreat].Slow, digit)
		},
		{
			"1p_key2_pgreat_slow",
			(int digit) => GetIntValue(LaneJudgeCount[2][JudgeType.PGreat].Slow, digit)
		},
		{
			"1p_key3_pgreat_slow",
			(int digit) => GetIntValue(LaneJudgeCount[3][JudgeType.PGreat].Slow, digit)
		},
		{
			"1p_key4_pgreat_slow",
			(int digit) => GetIntValue(LaneJudgeCount[4][JudgeType.PGreat].Slow, digit)
		},
		{
			"1p_key5_pgreat_slow",
			(int digit) => GetIntValue(LaneJudgeCount[5][JudgeType.PGreat].Slow, digit)
		},
		{
			"1p_key6_pgreat_slow",
			(int digit) => GetIntValue(LaneJudgeCount[6][JudgeType.PGreat].Slow, digit)
		},
		{
			"1p_key7_pgreat_slow",
			(int digit) => GetIntValue(LaneJudgeCount[7][JudgeType.PGreat].Slow, digit)
		},
		{
			"1p_key8_pgreat_slow",
			(int digit) => GetIntValue(LaneJudgeCount[8][JudgeType.PGreat].Slow, digit)
		},
		{
			"1p_key9_pgreat_slow",
			(int digit) => GetIntValue(LaneJudgeCount[9][JudgeType.PGreat].Slow, digit)
		},
		{
			"1p_scratch_pgreat_slow",
			(int digit) => GetIntValue(LaneJudgeCount[0][JudgeType.PGreat].Slow, digit)
		},
		{
			"2p_key1_pgreat_slow",
			(int digit) => GetIntValue(LaneJudgeCount[11][JudgeType.PGreat].Slow, digit)
		},
		{
			"2p_key2_pgreat_slow",
			(int digit) => GetIntValue(LaneJudgeCount[12][JudgeType.PGreat].Slow, digit)
		},
		{
			"2p_key3_pgreat_slow",
			(int digit) => GetIntValue(LaneJudgeCount[13][JudgeType.PGreat].Slow, digit)
		},
		{
			"2p_key4_pgreat_slow",
			(int digit) => GetIntValue(LaneJudgeCount[14][JudgeType.PGreat].Slow, digit)
		},
		{
			"2p_key5_pgreat_slow",
			(int digit) => GetIntValue(LaneJudgeCount[15][JudgeType.PGreat].Slow, digit)
		},
		{
			"2p_key6_pgreat_slow",
			(int digit) => GetIntValue(LaneJudgeCount[16][JudgeType.PGreat].Slow, digit)
		},
		{
			"2p_key7_pgreat_slow",
			(int digit) => GetIntValue(LaneJudgeCount[17][JudgeType.PGreat].Slow, digit)
		},
		{
			"2p_key8_pgreat_slow",
			(int digit) => GetIntValue(LaneJudgeCount[18][JudgeType.PGreat].Slow, digit)
		},
		{
			"2p_key9_pgreat_slow",
			(int digit) => GetIntValue(LaneJudgeCount[19][JudgeType.PGreat].Slow, digit)
		},
		{
			"2p_scratch_pgreat_slow",
			(int digit) => GetIntValue(LaneJudgeCount[10][JudgeType.PGreat].Slow, digit)
		},
		{
			"1p_key1_pgreat",
			(int digit) => GetIntValue(LaneJudgeCount[1][JudgeType.PGreat].Total, digit)
		},
		{
			"1p_key2_pgreat",
			(int digit) => GetIntValue(LaneJudgeCount[2][JudgeType.PGreat].Total, digit)
		},
		{
			"1p_key3_pgreat",
			(int digit) => GetIntValue(LaneJudgeCount[3][JudgeType.PGreat].Total, digit)
		},
		{
			"1p_key4_pgreat",
			(int digit) => GetIntValue(LaneJudgeCount[4][JudgeType.PGreat].Total, digit)
		},
		{
			"1p_key5_pgreat",
			(int digit) => GetIntValue(LaneJudgeCount[5][JudgeType.PGreat].Total, digit)
		},
		{
			"1p_key6_pgreat",
			(int digit) => GetIntValue(LaneJudgeCount[6][JudgeType.PGreat].Total, digit)
		},
		{
			"1p_key7_pgreat",
			(int digit) => GetIntValue(LaneJudgeCount[7][JudgeType.PGreat].Total, digit)
		},
		{
			"1p_key8_pgreat",
			(int digit) => GetIntValue(LaneJudgeCount[8][JudgeType.PGreat].Total, digit)
		},
		{
			"1p_key9_pgreat",
			(int digit) => GetIntValue(LaneJudgeCount[9][JudgeType.PGreat].Total, digit)
		},
		{
			"1p_scratch_pgreat",
			(int digit) => GetIntValue(LaneJudgeCount[0][JudgeType.PGreat].Total, digit)
		},
		{
			"2p_key1_pgreat",
			(int digit) => GetIntValue(LaneJudgeCount[11][JudgeType.PGreat].Total, digit)
		},
		{
			"2p_key2_pgreat",
			(int digit) => GetIntValue(LaneJudgeCount[12][JudgeType.PGreat].Total, digit)
		},
		{
			"2p_key3_pgreat",
			(int digit) => GetIntValue(LaneJudgeCount[13][JudgeType.PGreat].Total, digit)
		},
		{
			"2p_key4_pgreat",
			(int digit) => GetIntValue(LaneJudgeCount[14][JudgeType.PGreat].Total, digit)
		},
		{
			"2p_key5_pgreat",
			(int digit) => GetIntValue(LaneJudgeCount[15][JudgeType.PGreat].Total, digit)
		},
		{
			"2p_key6_pgreat",
			(int digit) => GetIntValue(LaneJudgeCount[16][JudgeType.PGreat].Total, digit)
		},
		{
			"2p_key7_pgreat",
			(int digit) => GetIntValue(LaneJudgeCount[17][JudgeType.PGreat].Total, digit)
		},
		{
			"2p_key8_pgreat",
			(int digit) => GetIntValue(LaneJudgeCount[18][JudgeType.PGreat].Total, digit)
		},
		{
			"2p_key9_pgreat",
			(int digit) => GetIntValue(LaneJudgeCount[19][JudgeType.PGreat].Total, digit)
		},
		{
			"2p_scratch_pgreat",
			(int digit) => GetIntValue(LaneJudgeCount[10][JudgeType.PGreat].Total, digit)
		},
		{
			"1p_great",
			(int digit) => GetIntValue(Player1JudgeCount[JudgeType.Great].Total, digit)
		},
		{
			"1p_great_fast",
			(int digit) => GetIntValue(Player1JudgeCount[JudgeType.Great].Fast, digit)
		},
		{
			"1p_great_slow",
			(int digit) => GetIntValue(Player1JudgeCount[JudgeType.Great].Slow, digit)
		},
		{
			"2p_great",
			(int digit) => GetIntValue(Player2JudgeCount[JudgeType.Great].Total, digit)
		},
		{
			"2p_great_fast",
			(int digit) => GetIntValue(Player2JudgeCount[JudgeType.Great].Fast, digit)
		},
		{
			"2p_great_slow",
			(int digit) => GetIntValue(Player2JudgeCount[JudgeType.Great].Slow, digit)
		},
		{
			"1p_key1_great_fast",
			(int digit) => GetIntValue(LaneJudgeCount[1][JudgeType.Great].Fast, digit)
		},
		{
			"1p_key2_great_fast",
			(int digit) => GetIntValue(LaneJudgeCount[2][JudgeType.Great].Fast, digit)
		},
		{
			"1p_key3_great_fast",
			(int digit) => GetIntValue(LaneJudgeCount[3][JudgeType.Great].Fast, digit)
		},
		{
			"1p_key4_great_fast",
			(int digit) => GetIntValue(LaneJudgeCount[4][JudgeType.Great].Fast, digit)
		},
		{
			"1p_key5_great_fast",
			(int digit) => GetIntValue(LaneJudgeCount[5][JudgeType.Great].Fast, digit)
		},
		{
			"1p_key6_great_fast",
			(int digit) => GetIntValue(LaneJudgeCount[6][JudgeType.Great].Fast, digit)
		},
		{
			"1p_key7_great_fast",
			(int digit) => GetIntValue(LaneJudgeCount[7][JudgeType.Great].Fast, digit)
		},
		{
			"1p_key8_great_fast",
			(int digit) => GetIntValue(LaneJudgeCount[8][JudgeType.Great].Fast, digit)
		},
		{
			"1p_key9_great_fast",
			(int digit) => GetIntValue(LaneJudgeCount[9][JudgeType.Great].Fast, digit)
		},
		{
			"1p_scratch_great_fast",
			(int digit) => GetIntValue(LaneJudgeCount[0][JudgeType.Great].Fast, digit)
		},
		{
			"2p_key1_great_fast",
			(int digit) => GetIntValue(LaneJudgeCount[11][JudgeType.Great].Fast, digit)
		},
		{
			"2p_key2_great_fast",
			(int digit) => GetIntValue(LaneJudgeCount[12][JudgeType.Great].Fast, digit)
		},
		{
			"2p_key3_great_fast",
			(int digit) => GetIntValue(LaneJudgeCount[13][JudgeType.Great].Fast, digit)
		},
		{
			"2p_key4_great_fast",
			(int digit) => GetIntValue(LaneJudgeCount[14][JudgeType.Great].Fast, digit)
		},
		{
			"2p_key5_great_fast",
			(int digit) => GetIntValue(LaneJudgeCount[15][JudgeType.Great].Fast, digit)
		},
		{
			"2p_key6_great_fast",
			(int digit) => GetIntValue(LaneJudgeCount[16][JudgeType.Great].Fast, digit)
		},
		{
			"2p_key7_great_fast",
			(int digit) => GetIntValue(LaneJudgeCount[17][JudgeType.Great].Fast, digit)
		},
		{
			"2p_key8_great_fast",
			(int digit) => GetIntValue(LaneJudgeCount[18][JudgeType.Great].Fast, digit)
		},
		{
			"2p_key9_great_fast",
			(int digit) => GetIntValue(LaneJudgeCount[19][JudgeType.Great].Fast, digit)
		},
		{
			"2p_scratch_great_fast",
			(int digit) => GetIntValue(LaneJudgeCount[10][JudgeType.Great].Fast, digit)
		},
		{
			"1p_key1_great_slow",
			(int digit) => GetIntValue(LaneJudgeCount[1][JudgeType.Great].Slow, digit)
		},
		{
			"1p_key2_great_slow",
			(int digit) => GetIntValue(LaneJudgeCount[2][JudgeType.Great].Slow, digit)
		},
		{
			"1p_key3_great_slow",
			(int digit) => GetIntValue(LaneJudgeCount[3][JudgeType.Great].Slow, digit)
		},
		{
			"1p_key4_great_slow",
			(int digit) => GetIntValue(LaneJudgeCount[4][JudgeType.Great].Slow, digit)
		},
		{
			"1p_key5_great_slow",
			(int digit) => GetIntValue(LaneJudgeCount[5][JudgeType.Great].Slow, digit)
		},
		{
			"1p_key6_great_slow",
			(int digit) => GetIntValue(LaneJudgeCount[6][JudgeType.Great].Slow, digit)
		},
		{
			"1p_key7_great_slow",
			(int digit) => GetIntValue(LaneJudgeCount[7][JudgeType.Great].Slow, digit)
		},
		{
			"1p_key8_great_slow",
			(int digit) => GetIntValue(LaneJudgeCount[8][JudgeType.Great].Slow, digit)
		},
		{
			"1p_key9_great_slow",
			(int digit) => GetIntValue(LaneJudgeCount[9][JudgeType.Great].Slow, digit)
		},
		{
			"1p_scratch_great_slow",
			(int digit) => GetIntValue(LaneJudgeCount[0][JudgeType.Great].Slow, digit)
		},
		{
			"2p_key1_great_slow",
			(int digit) => GetIntValue(LaneJudgeCount[11][JudgeType.Great].Slow, digit)
		},
		{
			"2p_key2_great_slow",
			(int digit) => GetIntValue(LaneJudgeCount[12][JudgeType.Great].Slow, digit)
		},
		{
			"2p_key3_great_slow",
			(int digit) => GetIntValue(LaneJudgeCount[13][JudgeType.Great].Slow, digit)
		},
		{
			"2p_key4_great_slow",
			(int digit) => GetIntValue(LaneJudgeCount[14][JudgeType.Great].Slow, digit)
		},
		{
			"2p_key5_great_slow",
			(int digit) => GetIntValue(LaneJudgeCount[15][JudgeType.Great].Slow, digit)
		},
		{
			"2p_key6_great_slow",
			(int digit) => GetIntValue(LaneJudgeCount[16][JudgeType.Great].Slow, digit)
		},
		{
			"2p_key7_great_slow",
			(int digit) => GetIntValue(LaneJudgeCount[17][JudgeType.Great].Slow, digit)
		},
		{
			"2p_key8_great_slow",
			(int digit) => GetIntValue(LaneJudgeCount[18][JudgeType.Great].Slow, digit)
		},
		{
			"2p_key9_great_slow",
			(int digit) => GetIntValue(LaneJudgeCount[19][JudgeType.Great].Slow, digit)
		},
		{
			"2p_scratch_great_slow",
			(int digit) => GetIntValue(LaneJudgeCount[10][JudgeType.Great].Slow, digit)
		},
		{
			"1p_key1_great",
			(int digit) => GetIntValue(LaneJudgeCount[1][JudgeType.Great].Total, digit)
		},
		{
			"1p_key2_great",
			(int digit) => GetIntValue(LaneJudgeCount[2][JudgeType.Great].Total, digit)
		},
		{
			"1p_key3_great",
			(int digit) => GetIntValue(LaneJudgeCount[3][JudgeType.Great].Total, digit)
		},
		{
			"1p_key4_great",
			(int digit) => GetIntValue(LaneJudgeCount[4][JudgeType.Great].Total, digit)
		},
		{
			"1p_key5_great",
			(int digit) => GetIntValue(LaneJudgeCount[5][JudgeType.Great].Total, digit)
		},
		{
			"1p_key6_great",
			(int digit) => GetIntValue(LaneJudgeCount[6][JudgeType.Great].Total, digit)
		},
		{
			"1p_key7_great",
			(int digit) => GetIntValue(LaneJudgeCount[7][JudgeType.Great].Total, digit)
		},
		{
			"1p_key8_great",
			(int digit) => GetIntValue(LaneJudgeCount[8][JudgeType.Great].Total, digit)
		},
		{
			"1p_key9_great",
			(int digit) => GetIntValue(LaneJudgeCount[9][JudgeType.Great].Total, digit)
		},
		{
			"1p_scratch_great",
			(int digit) => GetIntValue(LaneJudgeCount[0][JudgeType.Great].Total, digit)
		},
		{
			"2p_key1_great",
			(int digit) => GetIntValue(LaneJudgeCount[11][JudgeType.Great].Total, digit)
		},
		{
			"2p_key2_great",
			(int digit) => GetIntValue(LaneJudgeCount[12][JudgeType.Great].Total, digit)
		},
		{
			"2p_key3_great",
			(int digit) => GetIntValue(LaneJudgeCount[13][JudgeType.Great].Total, digit)
		},
		{
			"2p_key4_great",
			(int digit) => GetIntValue(LaneJudgeCount[14][JudgeType.Great].Total, digit)
		},
		{
			"2p_key5_great",
			(int digit) => GetIntValue(LaneJudgeCount[15][JudgeType.Great].Total, digit)
		},
		{
			"2p_key6_great",
			(int digit) => GetIntValue(LaneJudgeCount[16][JudgeType.Great].Total, digit)
		},
		{
			"2p_key7_great",
			(int digit) => GetIntValue(LaneJudgeCount[17][JudgeType.Great].Total, digit)
		},
		{
			"2p_key8_great",
			(int digit) => GetIntValue(LaneJudgeCount[18][JudgeType.Great].Total, digit)
		},
		{
			"2p_key9_great",
			(int digit) => GetIntValue(LaneJudgeCount[19][JudgeType.Great].Total, digit)
		},
		{
			"2p_scratch_great",
			(int digit) => GetIntValue(LaneJudgeCount[10][JudgeType.Great].Total, digit)
		},
		{
			"1p_good",
			(int digit) => GetIntValue(Player1JudgeCount[JudgeType.Good].Total, digit)
		},
		{
			"1p_good_fast",
			(int digit) => GetIntValue(Player1JudgeCount[JudgeType.Good].Fast, digit)
		},
		{
			"1p_good_slow",
			(int digit) => GetIntValue(Player1JudgeCount[JudgeType.Good].Slow, digit)
		},
		{
			"2p_good",
			(int digit) => GetIntValue(Player2JudgeCount[JudgeType.Good].Total, digit)
		},
		{
			"2p_good_fast",
			(int digit) => GetIntValue(Player2JudgeCount[JudgeType.Good].Fast, digit)
		},
		{
			"2p_good_slow",
			(int digit) => GetIntValue(Player2JudgeCount[JudgeType.Good].Slow, digit)
		},
		{
			"1p_key1_good_fast",
			(int digit) => GetIntValue(LaneJudgeCount[1][JudgeType.Good].Fast, digit)
		},
		{
			"1p_key2_good_fast",
			(int digit) => GetIntValue(LaneJudgeCount[2][JudgeType.Good].Fast, digit)
		},
		{
			"1p_key3_good_fast",
			(int digit) => GetIntValue(LaneJudgeCount[3][JudgeType.Good].Fast, digit)
		},
		{
			"1p_key4_good_fast",
			(int digit) => GetIntValue(LaneJudgeCount[4][JudgeType.Good].Fast, digit)
		},
		{
			"1p_key5_good_fast",
			(int digit) => GetIntValue(LaneJudgeCount[5][JudgeType.Good].Fast, digit)
		},
		{
			"1p_key6_good_fast",
			(int digit) => GetIntValue(LaneJudgeCount[6][JudgeType.Good].Fast, digit)
		},
		{
			"1p_key7_good_fast",
			(int digit) => GetIntValue(LaneJudgeCount[7][JudgeType.Good].Fast, digit)
		},
		{
			"1p_key8_good_fast",
			(int digit) => GetIntValue(LaneJudgeCount[8][JudgeType.Good].Fast, digit)
		},
		{
			"1p_key9_good_fast",
			(int digit) => GetIntValue(LaneJudgeCount[9][JudgeType.Good].Fast, digit)
		},
		{
			"1p_scratch_good_fast",
			(int digit) => GetIntValue(LaneJudgeCount[0][JudgeType.Good].Fast, digit)
		},
		{
			"2p_key1_good_fast",
			(int digit) => GetIntValue(LaneJudgeCount[11][JudgeType.Good].Fast, digit)
		},
		{
			"2p_key2_good_fast",
			(int digit) => GetIntValue(LaneJudgeCount[12][JudgeType.Good].Fast, digit)
		},
		{
			"2p_key3_good_fast",
			(int digit) => GetIntValue(LaneJudgeCount[13][JudgeType.Good].Fast, digit)
		},
		{
			"2p_key4_good_fast",
			(int digit) => GetIntValue(LaneJudgeCount[14][JudgeType.Good].Fast, digit)
		},
		{
			"2p_key5_good_fast",
			(int digit) => GetIntValue(LaneJudgeCount[15][JudgeType.Good].Fast, digit)
		},
		{
			"2p_key6_good_fast",
			(int digit) => GetIntValue(LaneJudgeCount[16][JudgeType.Good].Fast, digit)
		},
		{
			"2p_key7_good_fast",
			(int digit) => GetIntValue(LaneJudgeCount[17][JudgeType.Good].Fast, digit)
		},
		{
			"2p_key8_good_fast",
			(int digit) => GetIntValue(LaneJudgeCount[18][JudgeType.Good].Fast, digit)
		},
		{
			"2p_key9_good_fast",
			(int digit) => GetIntValue(LaneJudgeCount[19][JudgeType.Good].Fast, digit)
		},
		{
			"2p_scratch_good_fast",
			(int digit) => GetIntValue(LaneJudgeCount[10][JudgeType.Good].Fast, digit)
		},
		{
			"1p_key1_good_slow",
			(int digit) => GetIntValue(LaneJudgeCount[1][JudgeType.Good].Slow, digit)
		},
		{
			"1p_key2_good_slow",
			(int digit) => GetIntValue(LaneJudgeCount[2][JudgeType.Good].Slow, digit)
		},
		{
			"1p_key3_good_slow",
			(int digit) => GetIntValue(LaneJudgeCount[3][JudgeType.Good].Slow, digit)
		},
		{
			"1p_key4_good_slow",
			(int digit) => GetIntValue(LaneJudgeCount[4][JudgeType.Good].Slow, digit)
		},
		{
			"1p_key5_good_slow",
			(int digit) => GetIntValue(LaneJudgeCount[5][JudgeType.Good].Slow, digit)
		},
		{
			"1p_key6_good_slow",
			(int digit) => GetIntValue(LaneJudgeCount[6][JudgeType.Good].Slow, digit)
		},
		{
			"1p_key7_good_slow",
			(int digit) => GetIntValue(LaneJudgeCount[7][JudgeType.Good].Slow, digit)
		},
		{
			"1p_key8_good_slow",
			(int digit) => GetIntValue(LaneJudgeCount[8][JudgeType.Good].Slow, digit)
		},
		{
			"1p_key9_good_slow",
			(int digit) => GetIntValue(LaneJudgeCount[9][JudgeType.Good].Slow, digit)
		},
		{
			"1p_scratch_good_slow",
			(int digit) => GetIntValue(LaneJudgeCount[0][JudgeType.Good].Slow, digit)
		},
		{
			"2p_key1_good_slow",
			(int digit) => GetIntValue(LaneJudgeCount[11][JudgeType.Good].Slow, digit)
		},
		{
			"2p_key2_good_slow",
			(int digit) => GetIntValue(LaneJudgeCount[12][JudgeType.Good].Slow, digit)
		},
		{
			"2p_key3_good_slow",
			(int digit) => GetIntValue(LaneJudgeCount[13][JudgeType.Good].Slow, digit)
		},
		{
			"2p_key4_good_slow",
			(int digit) => GetIntValue(LaneJudgeCount[14][JudgeType.Good].Slow, digit)
		},
		{
			"2p_key5_good_slow",
			(int digit) => GetIntValue(LaneJudgeCount[15][JudgeType.Good].Slow, digit)
		},
		{
			"2p_key6_good_slow",
			(int digit) => GetIntValue(LaneJudgeCount[16][JudgeType.Good].Slow, digit)
		},
		{
			"2p_key7_good_slow",
			(int digit) => GetIntValue(LaneJudgeCount[17][JudgeType.Good].Slow, digit)
		},
		{
			"2p_key8_good_slow",
			(int digit) => GetIntValue(LaneJudgeCount[18][JudgeType.Good].Slow, digit)
		},
		{
			"2p_key9_good_slow",
			(int digit) => GetIntValue(LaneJudgeCount[19][JudgeType.Good].Slow, digit)
		},
		{
			"2p_scratch_good_slow",
			(int digit) => GetIntValue(LaneJudgeCount[10][JudgeType.Good].Slow, digit)
		},
		{
			"1p_key1_good",
			(int digit) => GetIntValue(LaneJudgeCount[1][JudgeType.Good].Total, digit)
		},
		{
			"1p_key2_good",
			(int digit) => GetIntValue(LaneJudgeCount[2][JudgeType.Good].Total, digit)
		},
		{
			"1p_key3_good",
			(int digit) => GetIntValue(LaneJudgeCount[3][JudgeType.Good].Total, digit)
		},
		{
			"1p_key4_good",
			(int digit) => GetIntValue(LaneJudgeCount[4][JudgeType.Good].Total, digit)
		},
		{
			"1p_key5_good",
			(int digit) => GetIntValue(LaneJudgeCount[5][JudgeType.Good].Total, digit)
		},
		{
			"1p_key6_good",
			(int digit) => GetIntValue(LaneJudgeCount[6][JudgeType.Good].Total, digit)
		},
		{
			"1p_key7_good",
			(int digit) => GetIntValue(LaneJudgeCount[7][JudgeType.Good].Total, digit)
		},
		{
			"1p_key8_good",
			(int digit) => GetIntValue(LaneJudgeCount[8][JudgeType.Good].Total, digit)
		},
		{
			"1p_key9_good",
			(int digit) => GetIntValue(LaneJudgeCount[9][JudgeType.Good].Total, digit)
		},
		{
			"1p_scratch_good",
			(int digit) => GetIntValue(LaneJudgeCount[0][JudgeType.Good].Total, digit)
		},
		{
			"2p_key1_good",
			(int digit) => GetIntValue(LaneJudgeCount[11][JudgeType.Good].Total, digit)
		},
		{
			"2p_key2_good",
			(int digit) => GetIntValue(LaneJudgeCount[12][JudgeType.Good].Total, digit)
		},
		{
			"2p_key3_good",
			(int digit) => GetIntValue(LaneJudgeCount[13][JudgeType.Good].Total, digit)
		},
		{
			"2p_key4_good",
			(int digit) => GetIntValue(LaneJudgeCount[14][JudgeType.Good].Total, digit)
		},
		{
			"2p_key5_good",
			(int digit) => GetIntValue(LaneJudgeCount[15][JudgeType.Good].Total, digit)
		},
		{
			"2p_key6_good",
			(int digit) => GetIntValue(LaneJudgeCount[16][JudgeType.Good].Total, digit)
		},
		{
			"2p_key7_good",
			(int digit) => GetIntValue(LaneJudgeCount[17][JudgeType.Good].Total, digit)
		},
		{
			"2p_key8_good",
			(int digit) => GetIntValue(LaneJudgeCount[18][JudgeType.Good].Total, digit)
		},
		{
			"2p_key9_good",
			(int digit) => GetIntValue(LaneJudgeCount[19][JudgeType.Good].Total, digit)
		},
		{
			"2p_scratch_good",
			(int digit) => GetIntValue(LaneJudgeCount[10][JudgeType.Good].Total, digit)
		},
		{
			"1p_bad",
			(int digit) => GetIntValue(Player1JudgeCount[JudgeType.Bad].Total, digit)
		},
		{
			"1p_bad_fast",
			(int digit) => GetIntValue(Player1JudgeCount[JudgeType.Bad].Fast, digit)
		},
		{
			"1p_bad_slow",
			(int digit) => GetIntValue(Player1JudgeCount[JudgeType.Bad].Slow, digit)
		},
		{
			"2p_bad",
			(int digit) => GetIntValue(Player2JudgeCount[JudgeType.Bad].Total, digit)
		},
		{
			"2p_bad_fast",
			(int digit) => GetIntValue(Player2JudgeCount[JudgeType.Bad].Fast, digit)
		},
		{
			"2p_bad_slow",
			(int digit) => GetIntValue(Player2JudgeCount[JudgeType.Bad].Slow, digit)
		},
		{
			"1p_key1_bad_fast",
			(int digit) => GetIntValue(LaneJudgeCount[1][JudgeType.Bad].Fast, digit)
		},
		{
			"1p_key2_bad_fast",
			(int digit) => GetIntValue(LaneJudgeCount[2][JudgeType.Bad].Fast, digit)
		},
		{
			"1p_key3_bad_fast",
			(int digit) => GetIntValue(LaneJudgeCount[3][JudgeType.Bad].Fast, digit)
		},
		{
			"1p_key4_bad_fast",
			(int digit) => GetIntValue(LaneJudgeCount[4][JudgeType.Bad].Fast, digit)
		},
		{
			"1p_key5_bad_fast",
			(int digit) => GetIntValue(LaneJudgeCount[5][JudgeType.Bad].Fast, digit)
		},
		{
			"1p_key6_bad_fast",
			(int digit) => GetIntValue(LaneJudgeCount[6][JudgeType.Bad].Fast, digit)
		},
		{
			"1p_key7_bad_fast",
			(int digit) => GetIntValue(LaneJudgeCount[7][JudgeType.Bad].Fast, digit)
		},
		{
			"1p_key8_bad_fast",
			(int digit) => GetIntValue(LaneJudgeCount[8][JudgeType.Bad].Fast, digit)
		},
		{
			"1p_key9_bad_fast",
			(int digit) => GetIntValue(LaneJudgeCount[9][JudgeType.Bad].Fast, digit)
		},
		{
			"1p_scratch_bad_fast",
			(int digit) => GetIntValue(LaneJudgeCount[0][JudgeType.Bad].Fast, digit)
		},
		{
			"2p_key1_bad_fast",
			(int digit) => GetIntValue(LaneJudgeCount[11][JudgeType.Bad].Fast, digit)
		},
		{
			"2p_key2_bad_fast",
			(int digit) => GetIntValue(LaneJudgeCount[12][JudgeType.Bad].Fast, digit)
		},
		{
			"2p_key3_bad_fast",
			(int digit) => GetIntValue(LaneJudgeCount[13][JudgeType.Bad].Fast, digit)
		},
		{
			"2p_key4_bad_fast",
			(int digit) => GetIntValue(LaneJudgeCount[14][JudgeType.Bad].Fast, digit)
		},
		{
			"2p_key5_bad_fast",
			(int digit) => GetIntValue(LaneJudgeCount[15][JudgeType.Bad].Fast, digit)
		},
		{
			"2p_key6_bad_fast",
			(int digit) => GetIntValue(LaneJudgeCount[16][JudgeType.Bad].Fast, digit)
		},
		{
			"2p_key7_bad_fast",
			(int digit) => GetIntValue(LaneJudgeCount[17][JudgeType.Bad].Fast, digit)
		},
		{
			"2p_key8_bad_fast",
			(int digit) => GetIntValue(LaneJudgeCount[18][JudgeType.Bad].Fast, digit)
		},
		{
			"2p_key9_bad_fast",
			(int digit) => GetIntValue(LaneJudgeCount[19][JudgeType.Bad].Fast, digit)
		},
		{
			"2p_scratch_bad_fast",
			(int digit) => GetIntValue(LaneJudgeCount[10][JudgeType.Bad].Fast, digit)
		},
		{
			"1p_key1_bad_slow",
			(int digit) => GetIntValue(LaneJudgeCount[1][JudgeType.Bad].Slow, digit)
		},
		{
			"1p_key2_bad_slow",
			(int digit) => GetIntValue(LaneJudgeCount[2][JudgeType.Bad].Slow, digit)
		},
		{
			"1p_key3_bad_slow",
			(int digit) => GetIntValue(LaneJudgeCount[3][JudgeType.Bad].Slow, digit)
		},
		{
			"1p_key4_bad_slow",
			(int digit) => GetIntValue(LaneJudgeCount[4][JudgeType.Bad].Slow, digit)
		},
		{
			"1p_key5_bad_slow",
			(int digit) => GetIntValue(LaneJudgeCount[5][JudgeType.Bad].Slow, digit)
		},
		{
			"1p_key6_bad_slow",
			(int digit) => GetIntValue(LaneJudgeCount[6][JudgeType.Bad].Slow, digit)
		},
		{
			"1p_key7_bad_slow",
			(int digit) => GetIntValue(LaneJudgeCount[7][JudgeType.Bad].Slow, digit)
		},
		{
			"1p_key8_bad_slow",
			(int digit) => GetIntValue(LaneJudgeCount[8][JudgeType.Bad].Slow, digit)
		},
		{
			"1p_key9_bad_slow",
			(int digit) => GetIntValue(LaneJudgeCount[9][JudgeType.Bad].Slow, digit)
		},
		{
			"1p_scratch_bad_slow",
			(int digit) => GetIntValue(LaneJudgeCount[0][JudgeType.Bad].Slow, digit)
		},
		{
			"2p_key1_bad_slow",
			(int digit) => GetIntValue(LaneJudgeCount[11][JudgeType.Bad].Slow, digit)
		},
		{
			"2p_key2_bad_slow",
			(int digit) => GetIntValue(LaneJudgeCount[12][JudgeType.Bad].Slow, digit)
		},
		{
			"2p_key3_bad_slow",
			(int digit) => GetIntValue(LaneJudgeCount[13][JudgeType.Bad].Slow, digit)
		},
		{
			"2p_key4_bad_slow",
			(int digit) => GetIntValue(LaneJudgeCount[14][JudgeType.Bad].Slow, digit)
		},
		{
			"2p_key5_bad_slow",
			(int digit) => GetIntValue(LaneJudgeCount[15][JudgeType.Bad].Slow, digit)
		},
		{
			"2p_key6_bad_slow",
			(int digit) => GetIntValue(LaneJudgeCount[16][JudgeType.Bad].Slow, digit)
		},
		{
			"2p_key7_bad_slow",
			(int digit) => GetIntValue(LaneJudgeCount[17][JudgeType.Bad].Slow, digit)
		},
		{
			"2p_key8_bad_slow",
			(int digit) => GetIntValue(LaneJudgeCount[18][JudgeType.Bad].Slow, digit)
		},
		{
			"2p_key9_bad_slow",
			(int digit) => GetIntValue(LaneJudgeCount[19][JudgeType.Bad].Slow, digit)
		},
		{
			"2p_scratch_bad_slow",
			(int digit) => GetIntValue(LaneJudgeCount[10][JudgeType.Bad].Slow, digit)
		},
		{
			"1p_key1_bad",
			(int digit) => GetIntValue(LaneJudgeCount[1][JudgeType.Bad].Total, digit)
		},
		{
			"1p_key2_bad",
			(int digit) => GetIntValue(LaneJudgeCount[2][JudgeType.Bad].Total, digit)
		},
		{
			"1p_key3_bad",
			(int digit) => GetIntValue(LaneJudgeCount[3][JudgeType.Bad].Total, digit)
		},
		{
			"1p_key4_bad",
			(int digit) => GetIntValue(LaneJudgeCount[4][JudgeType.Bad].Total, digit)
		},
		{
			"1p_key5_bad",
			(int digit) => GetIntValue(LaneJudgeCount[5][JudgeType.Bad].Total, digit)
		},
		{
			"1p_key6_bad",
			(int digit) => GetIntValue(LaneJudgeCount[6][JudgeType.Bad].Total, digit)
		},
		{
			"1p_key7_bad",
			(int digit) => GetIntValue(LaneJudgeCount[7][JudgeType.Bad].Total, digit)
		},
		{
			"1p_key8_bad",
			(int digit) => GetIntValue(LaneJudgeCount[8][JudgeType.Bad].Total, digit)
		},
		{
			"1p_key9_bad",
			(int digit) => GetIntValue(LaneJudgeCount[9][JudgeType.Bad].Total, digit)
		},
		{
			"1p_scratch_bad",
			(int digit) => GetIntValue(LaneJudgeCount[0][JudgeType.Bad].Total, digit)
		},
		{
			"2p_key1_bad",
			(int digit) => GetIntValue(LaneJudgeCount[11][JudgeType.Bad].Total, digit)
		},
		{
			"2p_key2_bad",
			(int digit) => GetIntValue(LaneJudgeCount[12][JudgeType.Bad].Total, digit)
		},
		{
			"2p_key3_bad",
			(int digit) => GetIntValue(LaneJudgeCount[13][JudgeType.Bad].Total, digit)
		},
		{
			"2p_key4_bad",
			(int digit) => GetIntValue(LaneJudgeCount[14][JudgeType.Bad].Total, digit)
		},
		{
			"2p_key5_bad",
			(int digit) => GetIntValue(LaneJudgeCount[15][JudgeType.Bad].Total, digit)
		},
		{
			"2p_key6_bad",
			(int digit) => GetIntValue(LaneJudgeCount[16][JudgeType.Bad].Total, digit)
		},
		{
			"2p_key7_bad",
			(int digit) => GetIntValue(LaneJudgeCount[17][JudgeType.Bad].Total, digit)
		},
		{
			"2p_key8_bad",
			(int digit) => GetIntValue(LaneJudgeCount[18][JudgeType.Bad].Total, digit)
		},
		{
			"2p_key9_bad",
			(int digit) => GetIntValue(LaneJudgeCount[19][JudgeType.Bad].Total, digit)
		},
		{
			"2p_scratch_bad",
			(int digit) => GetIntValue(LaneJudgeCount[10][JudgeType.Bad].Total, digit)
		},
		{
			"1p_poor",
			(int digit) => GetIntValue(Player1PoorCount, digit)
		},
		{
			"1p_poor_fast",
			(int digit) => GetIntValue(Player1PoorFastCount, digit)
		},
		{
			"1p_poor_slow",
			(int digit) => GetIntValue(Player1PoorSlowCount, digit)
		},
		{
			"2p_poor",
			(int digit) => GetIntValue(Player2PoorCount, digit)
		},
		{
			"2p_poor_fast",
			(int digit) => GetIntValue(Player2PoorFastCount, digit)
		},
		{
			"2p_poor_slow",
			(int digit) => GetIntValue(Player2PoorSlowCount, digit)
		},
		{
			"1p_key1_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[1][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"1p_key2_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[2][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"1p_key3_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[3][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"1p_key4_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[4][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"1p_key5_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[5][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"1p_key6_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[6][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"1p_key7_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[7][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"1p_key8_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[8][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"1p_key9_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[9][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"1p_scratch_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[0][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"2p_key1_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[11][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"2p_key2_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[12][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"2p_key3_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[13][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"2p_key4_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[14][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"2p_key5_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[15][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"2p_key6_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[16][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"2p_key7_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[17][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"2p_key8_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[18][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"2p_key9_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[19][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"2p_scratch_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[10][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"1p_key1_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[1][JudgeType.PassedPoor].Total + LaneJudgeCount[1][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"1p_key2_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[2][JudgeType.PassedPoor].Total + LaneJudgeCount[2][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"1p_key3_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[3][JudgeType.PassedPoor].Total + LaneJudgeCount[3][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"1p_key4_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[4][JudgeType.PassedPoor].Total + LaneJudgeCount[4][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"1p_key5_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[5][JudgeType.PassedPoor].Total + LaneJudgeCount[5][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"1p_key6_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[6][JudgeType.PassedPoor].Total + LaneJudgeCount[6][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"1p_key7_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[7][JudgeType.PassedPoor].Total + LaneJudgeCount[7][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"1p_key8_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[8][JudgeType.PassedPoor].Total + LaneJudgeCount[8][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"1p_key9_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[9][JudgeType.PassedPoor].Total + LaneJudgeCount[9][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"1p_scratch_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[0][JudgeType.PassedPoor].Total + LaneJudgeCount[0][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"2p_key1_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[11][JudgeType.PassedPoor].Total + LaneJudgeCount[11][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"2p_key2_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[12][JudgeType.PassedPoor].Total + LaneJudgeCount[12][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"2p_key3_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[13][JudgeType.PassedPoor].Total + LaneJudgeCount[13][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"2p_key4_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[14][JudgeType.PassedPoor].Total + LaneJudgeCount[14][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"2p_key5_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[15][JudgeType.PassedPoor].Total + LaneJudgeCount[15][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"2p_key6_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[16][JudgeType.PassedPoor].Total + LaneJudgeCount[16][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"2p_key7_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[17][JudgeType.PassedPoor].Total + LaneJudgeCount[17][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"2p_key8_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[18][JudgeType.PassedPoor].Total + LaneJudgeCount[18][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"2p_key9_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[19][JudgeType.PassedPoor].Total + LaneJudgeCount[19][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"2p_scratch_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[10][JudgeType.PassedPoor].Total + LaneJudgeCount[10][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"1p_key1_poor",
			(int digit) => GetIntValue(LaneJudgeCount[1][JudgeType.PassedPoor].Total + LaneJudgeCount[1][JudgeType.WastedPoor].Total, digit)
		},
		{
			"1p_key2_poor",
			(int digit) => GetIntValue(LaneJudgeCount[2][JudgeType.PassedPoor].Total + LaneJudgeCount[2][JudgeType.WastedPoor].Total, digit)
		},
		{
			"1p_key3_poor",
			(int digit) => GetIntValue(LaneJudgeCount[3][JudgeType.PassedPoor].Total + LaneJudgeCount[3][JudgeType.WastedPoor].Total, digit)
		},
		{
			"1p_key4_poor",
			(int digit) => GetIntValue(LaneJudgeCount[4][JudgeType.PassedPoor].Total + LaneJudgeCount[4][JudgeType.WastedPoor].Total, digit)
		},
		{
			"1p_key5_poor",
			(int digit) => GetIntValue(LaneJudgeCount[5][JudgeType.PassedPoor].Total + LaneJudgeCount[5][JudgeType.WastedPoor].Total, digit)
		},
		{
			"1p_key6_poor",
			(int digit) => GetIntValue(LaneJudgeCount[6][JudgeType.PassedPoor].Total + LaneJudgeCount[6][JudgeType.WastedPoor].Total, digit)
		},
		{
			"1p_key7_poor",
			(int digit) => GetIntValue(LaneJudgeCount[7][JudgeType.PassedPoor].Total + LaneJudgeCount[7][JudgeType.WastedPoor].Total, digit)
		},
		{
			"1p_key8_poor",
			(int digit) => GetIntValue(LaneJudgeCount[8][JudgeType.PassedPoor].Total + LaneJudgeCount[8][JudgeType.WastedPoor].Total, digit)
		},
		{
			"1p_key9_poor",
			(int digit) => GetIntValue(LaneJudgeCount[9][JudgeType.PassedPoor].Total + LaneJudgeCount[9][JudgeType.WastedPoor].Total, digit)
		},
		{
			"1p_scratch_poor",
			(int digit) => GetIntValue(LaneJudgeCount[0][JudgeType.PassedPoor].Total + LaneJudgeCount[0][JudgeType.WastedPoor].Total, digit)
		},
		{
			"2p_key1_poor",
			(int digit) => GetIntValue(LaneJudgeCount[11][JudgeType.PassedPoor].Total + LaneJudgeCount[11][JudgeType.WastedPoor].Total, digit)
		},
		{
			"2p_key2_poor",
			(int digit) => GetIntValue(LaneJudgeCount[12][JudgeType.PassedPoor].Total + LaneJudgeCount[12][JudgeType.WastedPoor].Total, digit)
		},
		{
			"2p_key3_poor",
			(int digit) => GetIntValue(LaneJudgeCount[13][JudgeType.PassedPoor].Total + LaneJudgeCount[13][JudgeType.WastedPoor].Total, digit)
		},
		{
			"2p_key4_poor",
			(int digit) => GetIntValue(LaneJudgeCount[14][JudgeType.PassedPoor].Total + LaneJudgeCount[14][JudgeType.WastedPoor].Total, digit)
		},
		{
			"2p_key5_poor",
			(int digit) => GetIntValue(LaneJudgeCount[15][JudgeType.PassedPoor].Total + LaneJudgeCount[15][JudgeType.WastedPoor].Total, digit)
		},
		{
			"2p_key6_poor",
			(int digit) => GetIntValue(LaneJudgeCount[16][JudgeType.PassedPoor].Total + LaneJudgeCount[16][JudgeType.WastedPoor].Total, digit)
		},
		{
			"2p_key7_poor",
			(int digit) => GetIntValue(LaneJudgeCount[17][JudgeType.PassedPoor].Total + LaneJudgeCount[17][JudgeType.WastedPoor].Total, digit)
		},
		{
			"2p_key8_poor",
			(int digit) => GetIntValue(LaneJudgeCount[18][JudgeType.PassedPoor].Total + LaneJudgeCount[18][JudgeType.WastedPoor].Total, digit)
		},
		{
			"2p_key9_poor",
			(int digit) => GetIntValue(LaneJudgeCount[19][JudgeType.PassedPoor].Total + LaneJudgeCount[19][JudgeType.WastedPoor].Total, digit)
		},
		{
			"2p_scratch_poor",
			(int digit) => GetIntValue(LaneJudgeCount[10][JudgeType.PassedPoor].Total + LaneJudgeCount[10][JudgeType.WastedPoor].Total, digit)
		},
		{
			"1p_passed_poor",
			(int digit) => GetIntValue(Player1JudgeCount[JudgeType.PassedPoor].Total, digit)
		},
		{
			"2p_passed_poor",
			(int digit) => GetIntValue(Player2JudgeCount[JudgeType.PassedPoor].Total, digit)
		},
		{
			"1p_key1_passed_poor",
			(int digit) => GetIntValue(LaneJudgeCount[1][JudgeType.PassedPoor].Total, digit)
		},
		{
			"1p_key2_passed_poor",
			(int digit) => GetIntValue(LaneJudgeCount[2][JudgeType.PassedPoor].Total, digit)
		},
		{
			"1p_key3_passed_poor",
			(int digit) => GetIntValue(LaneJudgeCount[3][JudgeType.PassedPoor].Total, digit)
		},
		{
			"1p_key4_passed_poor",
			(int digit) => GetIntValue(LaneJudgeCount[4][JudgeType.PassedPoor].Total, digit)
		},
		{
			"1p_key5_passed_poor",
			(int digit) => GetIntValue(LaneJudgeCount[5][JudgeType.PassedPoor].Total, digit)
		},
		{
			"1p_key6_passed_poor",
			(int digit) => GetIntValue(LaneJudgeCount[6][JudgeType.PassedPoor].Total, digit)
		},
		{
			"1p_key7_passed_poor",
			(int digit) => GetIntValue(LaneJudgeCount[7][JudgeType.PassedPoor].Total, digit)
		},
		{
			"1p_key8_passed_poor",
			(int digit) => GetIntValue(LaneJudgeCount[8][JudgeType.PassedPoor].Total, digit)
		},
		{
			"1p_key9_passed_poor",
			(int digit) => GetIntValue(LaneJudgeCount[9][JudgeType.PassedPoor].Total, digit)
		},
		{
			"1p_scratch_passed_poor",
			(int digit) => GetIntValue(LaneJudgeCount[0][JudgeType.PassedPoor].Total, digit)
		},
		{
			"2p_key1_passed_poor",
			(int digit) => GetIntValue(LaneJudgeCount[11][JudgeType.PassedPoor].Total, digit)
		},
		{
			"2p_key2_passed_poor",
			(int digit) => GetIntValue(LaneJudgeCount[12][JudgeType.PassedPoor].Total, digit)
		},
		{
			"2p_key3_passed_poor",
			(int digit) => GetIntValue(LaneJudgeCount[13][JudgeType.PassedPoor].Total, digit)
		},
		{
			"2p_key4_passed_poor",
			(int digit) => GetIntValue(LaneJudgeCount[14][JudgeType.PassedPoor].Total, digit)
		},
		{
			"2p_key5_passed_poor",
			(int digit) => GetIntValue(LaneJudgeCount[15][JudgeType.PassedPoor].Total, digit)
		},
		{
			"2p_key6_passed_poor",
			(int digit) => GetIntValue(LaneJudgeCount[16][JudgeType.PassedPoor].Total, digit)
		},
		{
			"2p_key7_passed_poor",
			(int digit) => GetIntValue(LaneJudgeCount[17][JudgeType.PassedPoor].Total, digit)
		},
		{
			"2p_key8_passed_poor",
			(int digit) => GetIntValue(LaneJudgeCount[18][JudgeType.PassedPoor].Total, digit)
		},
		{
			"2p_key9_passed_poor",
			(int digit) => GetIntValue(LaneJudgeCount[19][JudgeType.PassedPoor].Total, digit)
		},
		{
			"2p_scratch_passed_poor",
			(int digit) => GetIntValue(LaneJudgeCount[10][JudgeType.PassedPoor].Total, digit)
		},
		{
			"1p_wasted_poor",
			(int digit) => GetIntValue(Player1JudgeCount[JudgeType.WastedPoor].Total, digit)
		},
		{
			"1p_wasted_poor_fast",
			(int digit) => GetIntValue(Player1JudgeCount[JudgeType.WastedPoor].Fast, digit)
		},
		{
			"1p_wasted_poor_slow",
			(int digit) => GetIntValue(Player1JudgeCount[JudgeType.WastedPoor].Slow, digit)
		},
		{
			"2p_wasted_poor",
			(int digit) => GetIntValue(Player2JudgeCount[JudgeType.WastedPoor].Total, digit)
		},
		{
			"2p_wasted_poor_fast",
			(int digit) => GetIntValue(Player2JudgeCount[JudgeType.WastedPoor].Fast, digit)
		},
		{
			"2p_wasted_poor_slow",
			(int digit) => GetIntValue(Player2JudgeCount[JudgeType.WastedPoor].Slow, digit)
		},
		{
			"1p_key1_wasted_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[1][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"1p_key2_wasted_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[2][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"1p_key3_wasted_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[3][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"1p_key4_wasted_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[4][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"1p_key5_wasted_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[5][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"1p_key6_wasted_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[6][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"1p_key7_wasted_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[7][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"1p_key8_wasted_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[8][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"1p_key9_wasted_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[9][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"1p_scratch_wasted_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[0][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"2p_key1_wasted_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[11][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"2p_key2_wasted_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[12][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"2p_key3_wasted_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[13][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"2p_key4_wasted_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[14][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"2p_key5_wasted_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[15][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"2p_key6_wasted_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[16][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"2p_key7_wasted_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[17][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"2p_key8_wasted_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[18][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"2p_key9_wasted_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[19][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"2p_scratch_wasted_poor_fast",
			(int digit) => GetIntValue(LaneJudgeCount[10][JudgeType.WastedPoor].Fast, digit)
		},
		{
			"1p_key1_wasted_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[1][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"1p_key2_wasted_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[2][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"1p_key3_wasted_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[3][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"1p_key4_wasted_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[4][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"1p_key5_wasted_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[5][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"1p_key6_wasted_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[6][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"1p_key7_wasted_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[7][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"1p_key8_wasted_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[8][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"1p_key9_wasted_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[9][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"1p_scratch_wasted_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[0][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"2p_key1_wasted_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[11][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"2p_key2_wasted_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[12][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"2p_key3_wasted_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[13][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"2p_key4_wasted_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[14][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"2p_key5_wasted_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[15][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"2p_key6_wasted_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[16][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"2p_key7_wasted_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[17][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"2p_key8_wasted_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[18][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"2p_key9_wasted_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[19][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"2p_scratch_wasted_poor_slow",
			(int digit) => GetIntValue(LaneJudgeCount[10][JudgeType.WastedPoor].Slow, digit)
		},
		{
			"1p_key1_wasted_poor",
			(int digit) => GetIntValue(LaneJudgeCount[1][JudgeType.WastedPoor].Total, digit)
		},
		{
			"1p_key2_wasted_poor",
			(int digit) => GetIntValue(LaneJudgeCount[2][JudgeType.WastedPoor].Total, digit)
		},
		{
			"1p_key3_wasted_poor",
			(int digit) => GetIntValue(LaneJudgeCount[3][JudgeType.WastedPoor].Total, digit)
		},
		{
			"1p_key4_wasted_poor",
			(int digit) => GetIntValue(LaneJudgeCount[4][JudgeType.WastedPoor].Total, digit)
		},
		{
			"1p_key5_wasted_poor",
			(int digit) => GetIntValue(LaneJudgeCount[5][JudgeType.WastedPoor].Total, digit)
		},
		{
			"1p_key6_wasted_poor",
			(int digit) => GetIntValue(LaneJudgeCount[6][JudgeType.WastedPoor].Total, digit)
		},
		{
			"1p_key7_wasted_poor",
			(int digit) => GetIntValue(LaneJudgeCount[7][JudgeType.WastedPoor].Total, digit)
		},
		{
			"1p_key8_wasted_poor",
			(int digit) => GetIntValue(LaneJudgeCount[8][JudgeType.WastedPoor].Total, digit)
		},
		{
			"1p_key9_wasted_poor",
			(int digit) => GetIntValue(LaneJudgeCount[9][JudgeType.WastedPoor].Total, digit)
		},
		{
			"1p_scratch_wasted_poor",
			(int digit) => GetIntValue(LaneJudgeCount[0][JudgeType.WastedPoor].Total, digit)
		},
		{
			"2p_key1_wasted_poor",
			(int digit) => GetIntValue(LaneJudgeCount[11][JudgeType.WastedPoor].Total, digit)
		},
		{
			"2p_key2_wasted_poor",
			(int digit) => GetIntValue(LaneJudgeCount[12][JudgeType.WastedPoor].Total, digit)
		},
		{
			"2p_key3_wasted_poor",
			(int digit) => GetIntValue(LaneJudgeCount[13][JudgeType.WastedPoor].Total, digit)
		},
		{
			"2p_key4_wasted_poor",
			(int digit) => GetIntValue(LaneJudgeCount[14][JudgeType.WastedPoor].Total, digit)
		},
		{
			"2p_key5_wasted_poor",
			(int digit) => GetIntValue(LaneJudgeCount[15][JudgeType.WastedPoor].Total, digit)
		},
		{
			"2p_key6_wasted_poor",
			(int digit) => GetIntValue(LaneJudgeCount[16][JudgeType.WastedPoor].Total, digit)
		},
		{
			"2p_key7_wasted_poor",
			(int digit) => GetIntValue(LaneJudgeCount[17][JudgeType.WastedPoor].Total, digit)
		},
		{
			"2p_key8_wasted_poor",
			(int digit) => GetIntValue(LaneJudgeCount[18][JudgeType.WastedPoor].Total, digit)
		},
		{
			"2p_key9_wasted_poor",
			(int digit) => GetIntValue(LaneJudgeCount[19][JudgeType.WastedPoor].Total, digit)
		},
		{
			"2p_scratch_wasted_poor",
			(int digit) => GetIntValue(LaneJudgeCount[10][JudgeType.WastedPoor].Total, digit)
		},
		{
			"1p_fast_count",
			(int digit) => GetIntValue(Player1TotalFastCount, digit)
		},
		{
			"2p_fast_count",
			(int digit) => GetIntValue(Player2TotalFastCount, digit)
		},
		{
			"1p_key1_fast_count",
			(int digit) => GetIntValue(GetLaneFastCount(1), digit)
		},
		{
			"1p_key2_fast_count",
			(int digit) => GetIntValue(GetLaneFastCount(2), digit)
		},
		{
			"1p_key3_fast_count",
			(int digit) => GetIntValue(GetLaneFastCount(3), digit)
		},
		{
			"1p_key4_fast_count",
			(int digit) => GetIntValue(GetLaneFastCount(4), digit)
		},
		{
			"1p_key5_fast_count",
			(int digit) => GetIntValue(GetLaneFastCount(5), digit)
		},
		{
			"1p_key6_fast_count",
			(int digit) => GetIntValue(GetLaneFastCount(6), digit)
		},
		{
			"1p_key7_fast_count",
			(int digit) => GetIntValue(GetLaneFastCount(7), digit)
		},
		{
			"1p_key8_fast_count",
			(int digit) => GetIntValue(GetLaneFastCount(8), digit)
		},
		{
			"1p_key9_fast_count",
			(int digit) => GetIntValue(GetLaneFastCount(9), digit)
		},
		{
			"1p_scratch_fast_count",
			(int digit) => GetIntValue(GetLaneFastCount(0), digit)
		},
		{
			"2p_key1_fast_count",
			(int digit) => GetIntValue(GetLaneFastCount(11), digit)
		},
		{
			"2p_key2_fast_count",
			(int digit) => GetIntValue(GetLaneFastCount(12), digit)
		},
		{
			"2p_key3_fast_count",
			(int digit) => GetIntValue(GetLaneFastCount(13), digit)
		},
		{
			"2p_key4_fast_count",
			(int digit) => GetIntValue(GetLaneFastCount(14), digit)
		},
		{
			"2p_key5_fast_count",
			(int digit) => GetIntValue(GetLaneFastCount(15), digit)
		},
		{
			"2p_key6_fast_count",
			(int digit) => GetIntValue(GetLaneFastCount(16), digit)
		},
		{
			"2p_key7_fast_count",
			(int digit) => GetIntValue(GetLaneFastCount(17), digit)
		},
		{
			"2p_key8_fast_count",
			(int digit) => GetIntValue(GetLaneFastCount(18), digit)
		},
		{
			"2p_key9_fast_count",
			(int digit) => GetIntValue(GetLaneFastCount(19), digit)
		},
		{
			"2p_scratch_fast_count",
			(int digit) => GetIntValue(GetLaneFastCount(10), digit)
		},
		{
			"1p_slow_count",
			(int digit) => GetIntValue(Player1TotalSlowCount, digit)
		},
		{
			"2p_slow_count",
			(int digit) => GetIntValue(Player2TotalSlowCount, digit)
		},
		{
			"1p_key1_slow_count",
			(int digit) => GetIntValue(GetLaneSlowCount(1), digit)
		},
		{
			"1p_key2_slow_count",
			(int digit) => GetIntValue(GetLaneSlowCount(2), digit)
		},
		{
			"1p_key3_slow_count",
			(int digit) => GetIntValue(GetLaneSlowCount(3), digit)
		},
		{
			"1p_key4_slow_count",
			(int digit) => GetIntValue(GetLaneSlowCount(4), digit)
		},
		{
			"1p_key5_slow_count",
			(int digit) => GetIntValue(GetLaneSlowCount(5), digit)
		},
		{
			"1p_key6_slow_count",
			(int digit) => GetIntValue(GetLaneSlowCount(6), digit)
		},
		{
			"1p_key7_slow_count",
			(int digit) => GetIntValue(GetLaneSlowCount(7), digit)
		},
		{
			"1p_key8_slow_count",
			(int digit) => GetIntValue(GetLaneSlowCount(8), digit)
		},
		{
			"1p_key9_slow_count",
			(int digit) => GetIntValue(GetLaneSlowCount(9), digit)
		},
		{
			"1p_scratch_slow_count",
			(int digit) => GetIntValue(GetLaneSlowCount(0), digit)
		},
		{
			"2p_key1_slow_count",
			(int digit) => GetIntValue(GetLaneSlowCount(11), digit)
		},
		{
			"2p_key2_slow_count",
			(int digit) => GetIntValue(GetLaneSlowCount(12), digit)
		},
		{
			"2p_key3_slow_count",
			(int digit) => GetIntValue(GetLaneSlowCount(13), digit)
		},
		{
			"2p_key4_slow_count",
			(int digit) => GetIntValue(GetLaneSlowCount(14), digit)
		},
		{
			"2p_key5_slow_count",
			(int digit) => GetIntValue(GetLaneSlowCount(15), digit)
		},
		{
			"2p_key6_slow_count",
			(int digit) => GetIntValue(GetLaneSlowCount(16), digit)
		},
		{
			"2p_key7_slow_count",
			(int digit) => GetIntValue(GetLaneSlowCount(17), digit)
		},
		{
			"2p_key8_slow_count",
			(int digit) => GetIntValue(GetLaneSlowCount(18), digit)
		},
		{
			"2p_key9_slow_count",
			(int digit) => GetIntValue(GetLaneSlowCount(19), digit)
		},
		{
			"2p_scratch_slow_count",
			(int digit) => GetIntValue(GetLaneSlowCount(10), digit)
		},
		{
			"1p_fast_count_without_pgreat",
			(int digit) => GetIntValue(Player1TotalFastCountWithoutPGreat, digit)
		},
		{
			"2p_fast_count_without_pgreat",
			(int digit) => GetIntValue(Player2TotalFastCountWithoutPGreat, digit)
		},
		{
			"1p_key1_fast_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneFastCountWithoutPGreat(1), digit)
		},
		{
			"1p_key2_fast_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneFastCountWithoutPGreat(2), digit)
		},
		{
			"1p_key3_fast_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneFastCountWithoutPGreat(3), digit)
		},
		{
			"1p_key4_fast_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneFastCountWithoutPGreat(4), digit)
		},
		{
			"1p_key5_fast_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneFastCountWithoutPGreat(5), digit)
		},
		{
			"1p_key6_fast_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneFastCountWithoutPGreat(6), digit)
		},
		{
			"1p_key7_fast_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneFastCountWithoutPGreat(7), digit)
		},
		{
			"1p_key8_fast_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneFastCountWithoutPGreat(8), digit)
		},
		{
			"1p_key9_fast_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneFastCountWithoutPGreat(9), digit)
		},
		{
			"1p_scratch_fast_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneFastCountWithoutPGreat(0), digit)
		},
		{
			"2p_key1_fast_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneFastCountWithoutPGreat(11), digit)
		},
		{
			"2p_key2_fast_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneFastCountWithoutPGreat(12), digit)
		},
		{
			"2p_key3_fast_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneFastCountWithoutPGreat(13), digit)
		},
		{
			"2p_key4_fast_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneFastCountWithoutPGreat(14), digit)
		},
		{
			"2p_key5_fast_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneFastCountWithoutPGreat(15), digit)
		},
		{
			"2p_key6_fast_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneFastCountWithoutPGreat(16), digit)
		},
		{
			"2p_key7_fast_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneFastCountWithoutPGreat(17), digit)
		},
		{
			"2p_key8_fast_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneFastCountWithoutPGreat(18), digit)
		},
		{
			"2p_key9_fast_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneFastCountWithoutPGreat(19), digit)
		},
		{
			"2p_scratch_fast_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneFastCountWithoutPGreat(10), digit)
		},
		{
			"1p_slow_count_without_pgreat",
			(int digit) => GetIntValue(Player1TotalSlowCountWithoutPGreat, digit)
		},
		{
			"2p_slow_count_without_pgreat",
			(int digit) => GetIntValue(Player2TotalSlowCountWithoutPGreat, digit)
		},
		{
			"1p_key1_slow_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneSlowCountWithoutPGreat(1), digit)
		},
		{
			"1p_key2_slow_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneSlowCountWithoutPGreat(2), digit)
		},
		{
			"1p_key3_slow_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneSlowCountWithoutPGreat(3), digit)
		},
		{
			"1p_key4_slow_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneSlowCountWithoutPGreat(4), digit)
		},
		{
			"1p_key5_slow_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneSlowCountWithoutPGreat(5), digit)
		},
		{
			"1p_key6_slow_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneSlowCountWithoutPGreat(6), digit)
		},
		{
			"1p_key7_slow_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneSlowCountWithoutPGreat(7), digit)
		},
		{
			"1p_key8_slow_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneSlowCountWithoutPGreat(8), digit)
		},
		{
			"1p_key9_slow_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneSlowCountWithoutPGreat(9), digit)
		},
		{
			"1p_scratch_slow_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneSlowCountWithoutPGreat(0), digit)
		},
		{
			"2p_key1_slow_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneSlowCountWithoutPGreat(11), digit)
		},
		{
			"2p_key2_slow_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneSlowCountWithoutPGreat(12), digit)
		},
		{
			"2p_key3_slow_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneSlowCountWithoutPGreat(13), digit)
		},
		{
			"2p_key4_slow_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneSlowCountWithoutPGreat(14), digit)
		},
		{
			"2p_key5_slow_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneSlowCountWithoutPGreat(15), digit)
		},
		{
			"2p_key6_slow_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneSlowCountWithoutPGreat(16), digit)
		},
		{
			"2p_key7_slow_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneSlowCountWithoutPGreat(17), digit)
		},
		{
			"2p_key8_slow_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneSlowCountWithoutPGreat(18), digit)
		},
		{
			"2p_key9_slow_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneSlowCountWithoutPGreat(19), digit)
		},
		{
			"2p_scratch_slow_count_without_pgreat",
			(int digit) => GetIntValue(GetLaneSlowCountWithoutPGreat(10), digit)
		},
		{
			"1p_last_judge_time",
			(int digit) => GetBeforePointValue(Player1LastJudgeTime, digit)
		},
		{
			"1p_last_judge_time_point",
			(int digit) => GetAfterPointValue(Player1LastJudgeTime, digit)
		},
		{
			"2p_last_judge_time",
			(int digit) => GetBeforePointValue(Player2LastJudgeTime, digit)
		},
		{
			"2p_last_judge_time_point",
			(int digit) => GetAfterPointValue(Player2LastJudgeTime, digit)
		},
		{
			"1p_key1_last_judge_time",
			(int digit) => GetBeforePointValue(LaneLastJudgeTime[1], digit)
		},
		{
			"1p_key1_last_judge_time_point",
			(int digit) => GetAfterPointValue(LaneLastJudgeTime[1], digit)
		},
		{
			"1p_key2_last_judge_time",
			(int digit) => GetBeforePointValue(LaneLastJudgeTime[2], digit)
		},
		{
			"1p_key2_last_judge_time_point",
			(int digit) => GetAfterPointValue(LaneLastJudgeTime[2], digit)
		},
		{
			"1p_key3_last_judge_time",
			(int digit) => GetBeforePointValue(LaneLastJudgeTime[3], digit)
		},
		{
			"1p_key3_last_judge_time_point",
			(int digit) => GetAfterPointValue(LaneLastJudgeTime[3], digit)
		},
		{
			"1p_key4_last_judge_time",
			(int digit) => GetBeforePointValue(LaneLastJudgeTime[4], digit)
		},
		{
			"1p_key4_last_judge_time_point",
			(int digit) => GetAfterPointValue(LaneLastJudgeTime[4], digit)
		},
		{
			"1p_key5_last_judge_time",
			(int digit) => GetBeforePointValue(LaneLastJudgeTime[5], digit)
		},
		{
			"1p_key5_last_judge_time_point",
			(int digit) => GetAfterPointValue(LaneLastJudgeTime[5], digit)
		},
		{
			"1p_key6_last_judge_time",
			(int digit) => GetBeforePointValue(LaneLastJudgeTime[6], digit)
		},
		{
			"1p_key6_last_judge_time_point",
			(int digit) => GetAfterPointValue(LaneLastJudgeTime[6], digit)
		},
		{
			"1p_key7_last_judge_time",
			(int digit) => GetBeforePointValue(LaneLastJudgeTime[7], digit)
		},
		{
			"1p_key7_last_judge_time_point",
			(int digit) => GetAfterPointValue(LaneLastJudgeTime[7], digit)
		},
		{
			"1p_key8_last_judge_time",
			(int digit) => GetBeforePointValue(LaneLastJudgeTime[8], digit)
		},
		{
			"1p_key8_last_judge_time_point",
			(int digit) => GetAfterPointValue(LaneLastJudgeTime[8], digit)
		},
		{
			"1p_key9_last_judge_time",
			(int digit) => GetBeforePointValue(LaneLastJudgeTime[9], digit)
		},
		{
			"1p_key9_last_judge_time_point",
			(int digit) => GetAfterPointValue(LaneLastJudgeTime[9], digit)
		},
		{
			"1p_scratch_last_judge_time",
			(int digit) => GetBeforePointValue(LaneLastJudgeTime[0], digit)
		},
		{
			"1p_scratch_last_judge_time_point",
			(int digit) => GetAfterPointValue(LaneLastJudgeTime[0], digit)
		},
		{
			"1p_last_judge_time_left",
			(int digit) => GetBeforePointValue(Player1LastJudgeTimeLeft, digit)
		},
		{
			"1p_last_judge_time_left_point",
			(int digit) => GetAfterPointValue(Player1LastJudgeTimeLeft, digit)
		},
		{
			"1p_last_judge_time_center",
			(int digit) => GetBeforePointValue(Player1LastJudgeTimeCenter, digit)
		},
		{
			"1p_last_judge_time_center_point",
			(int digit) => GetAfterPointValue(Player1LastJudgeTimeCenter, digit)
		},
		{
			"1p_last_judge_time_right",
			(int digit) => GetBeforePointValue(Player1LastJudgeTimeRight, digit)
		},
		{
			"1p_last_judge_time_right_point",
			(int digit) => GetAfterPointValue(Player1LastJudgeTimeRight, digit)
		},
		{
			"2p_key1_last_judge_time",
			(int digit) => GetBeforePointValue(LaneLastJudgeTime[11], digit)
		},
		{
			"2p_key1_last_judge_time_point",
			(int digit) => GetAfterPointValue(LaneLastJudgeTime[11], digit)
		},
		{
			"2p_key2_last_judge_time",
			(int digit) => GetBeforePointValue(LaneLastJudgeTime[12], digit)
		},
		{
			"2p_key2_last_judge_time_point",
			(int digit) => GetAfterPointValue(LaneLastJudgeTime[12], digit)
		},
		{
			"2p_key3_last_judge_time",
			(int digit) => GetBeforePointValue(LaneLastJudgeTime[13], digit)
		},
		{
			"2p_key3_last_judge_time_point",
			(int digit) => GetAfterPointValue(LaneLastJudgeTime[13], digit)
		},
		{
			"2p_key4_last_judge_time",
			(int digit) => GetBeforePointValue(LaneLastJudgeTime[14], digit)
		},
		{
			"2p_key4_last_judge_time_point",
			(int digit) => GetAfterPointValue(LaneLastJudgeTime[14], digit)
		},
		{
			"2p_key5_last_judge_time",
			(int digit) => GetBeforePointValue(LaneLastJudgeTime[15], digit)
		},
		{
			"2p_key5_last_judge_time_point",
			(int digit) => GetAfterPointValue(LaneLastJudgeTime[15], digit)
		},
		{
			"2p_key6_last_judge_time",
			(int digit) => GetBeforePointValue(LaneLastJudgeTime[16], digit)
		},
		{
			"2p_key6_last_judge_time_point",
			(int digit) => GetAfterPointValue(LaneLastJudgeTime[16], digit)
		},
		{
			"2p_key7_last_judge_time",
			(int digit) => GetBeforePointValue(LaneLastJudgeTime[17], digit)
		},
		{
			"2p_key7_last_judge_time_point",
			(int digit) => GetAfterPointValue(LaneLastJudgeTime[17], digit)
		},
		{
			"2p_key8_last_judge_time",
			(int digit) => GetBeforePointValue(LaneLastJudgeTime[18], digit)
		},
		{
			"2p_key8_last_judge_time_point",
			(int digit) => GetAfterPointValue(LaneLastJudgeTime[18], digit)
		},
		{
			"2p_key9_last_judge_time",
			(int digit) => GetBeforePointValue(LaneLastJudgeTime[19], digit)
		},
		{
			"2p_key9_last_judge_time_point",
			(int digit) => GetAfterPointValue(LaneLastJudgeTime[19], digit)
		},
		{
			"2p_scratch_last_judge_time",
			(int digit) => GetBeforePointValue(LaneLastJudgeTime[10], digit)
		},
		{
			"2p_scratch_last_judge_time_point",
			(int digit) => GetAfterPointValue(LaneLastJudgeTime[10], digit)
		},
		{
			"2p_last_judge_time_left",
			(int digit) => GetBeforePointValue(Player2LastJudgeTimeLeft, digit)
		},
		{
			"2p_last_judge_time_left_point",
			(int digit) => GetAfterPointValue(Player2LastJudgeTimeLeft, digit)
		},
		{
			"2p_last_judge_time_center",
			(int digit) => GetBeforePointValue(Player2LastJudgeTimeCenter, digit)
		},
		{
			"2p_last_judge_time_center_point",
			(int digit) => GetAfterPointValue(Player2LastJudgeTimeCenter, digit)
		},
		{
			"2p_last_judge_time_right",
			(int digit) => GetBeforePointValue(Player2LastJudgeTimeRight, digit)
		},
		{
			"2p_last_judge_time_right_point",
			(int digit) => GetAfterPointValue(Player2LastJudgeTimeRight, digit)
		},
		{
			"exscore",
			(int digit) => GetIntValue(ExScore, digit)
		},
		{
			"score_rate",
			(int digit) => GetBeforePointValue(ScoreRate, digit)
		},
		{
			"score_rate_point",
			(int digit) => GetAfterPointValue(ScoreRate, digit)
		},
		{
			"misscount",
			(int digit) => GetIntValue(MissCount, digit)
		},
		{
			"misscount_fast",
			(int digit) => GetIntValue(MissFastCount, digit)
		},
		{
			"misscount_slow",
			(int digit) => GetIntValue(MissSlowCount, digit)
		},
		{
			"pgreat",
			(int digit) => GetIntValue(JudgeCount[JudgeType.PGreat].Total, digit)
		},
		{
			"pgreat_fast",
			(int digit) => GetIntValue(JudgeCount[JudgeType.PGreat].Fast, digit)
		},
		{
			"pgreat_slow",
			(int digit) => GetIntValue(JudgeCount[JudgeType.PGreat].Slow, digit)
		},
		{
			"great",
			(int digit) => GetIntValue(JudgeCount[JudgeType.Great].Total, digit)
		},
		{
			"great_fast",
			(int digit) => GetIntValue(JudgeCount[JudgeType.Great].Fast, digit)
		},
		{
			"great_slow",
			(int digit) => GetIntValue(JudgeCount[JudgeType.Great].Slow, digit)
		},
		{
			"good",
			(int digit) => GetIntValue(JudgeCount[JudgeType.Good].Total, digit)
		},
		{
			"good_fast",
			(int digit) => GetIntValue(JudgeCount[JudgeType.Good].Fast, digit)
		},
		{
			"good_slow",
			(int digit) => GetIntValue(JudgeCount[JudgeType.Good].Slow, digit)
		},
		{
			"bad",
			(int digit) => GetIntValue(JudgeCount[JudgeType.Bad].Total, digit)
		},
		{
			"bad_fast",
			(int digit) => GetIntValue(JudgeCount[JudgeType.Bad].Fast, digit)
		},
		{
			"bad_slow",
			(int digit) => GetIntValue(JudgeCount[JudgeType.Bad].Slow, digit)
		},
		{
			"poor",
			(int digit) => GetIntValue(PoorCount, digit)
		},
		{
			"poor_fast",
			(int digit) => GetIntValue(PoorFastCount, digit)
		},
		{
			"poor_slow",
			(int digit) => GetIntValue(PoorSlowCount, digit)
		},
		{
			"passed_poor",
			(int digit) => GetIntValue(JudgeCount[JudgeType.PassedPoor].Total, digit)
		},
		{
			"wasted_poor",
			(int digit) => GetIntValue(JudgeCount[JudgeType.WastedPoor].Total, digit)
		},
		{
			"wasted_poor_fast",
			(int digit) => GetIntValue(JudgeCount[JudgeType.WastedPoor].Fast, digit)
		},
		{
			"wasted_poor_slow",
			(int digit) => GetIntValue(JudgeCount[JudgeType.WastedPoor].Slow, digit)
		},
		{
			"fast_count",
			(int digit) => GetIntValue(FastCount, digit)
		},
		{
			"slow_count",
			(int digit) => GetIntValue(SlowCount, digit)
		},
		{
			"fast_count_without_pgreat",
			(int digit) => GetIntValue(FastCountWithoutPGreat, digit)
		},
		{
			"slow_count_without_pgreat",
			(int digit) => GetIntValue(SlowCountWithoutPGreat, digit)
		},
		{
			"last_judge_time",
			(int digit) => GetBeforePointValue(LastJudgeTime, digit)
		},
		{
			"last_judge_time_point",
			(int digit) => GetAfterPointValue(LastJudgeTime, digit)
		},
		{
			"combo_break",
			(int digit) => GetIntValue(ComboBreakCount, digit)
		},
		{
			"judge_timing",
			(int digit) => GetBeforePointValue(JudgeTiming, digit)
		},
		{
			"judge_timing_point",
			(int digit) => GetAfterPointValue(JudgeTiming, digit)
		},
		{
			"average_judge_timing",
			(int digit) => GetBeforePointValue(AverageJudgeTiming, digit)
		},
		{
			"average_judge_timing_point",
			(int digit) => GetAfterPointValue(AverageJudgeTiming, digit)
		},
		{
			"notes_per_second",
			(int digit) => GetIntValue(NotesPerSecond, digit)
		},
		{
			"max_notes_per_second",
			(int digit) => GetIntValue(MaxNotesPerSecond, digit)
		},
		{
			"average_notes_per_second",
			(int digit) => GetBeforePointValue(AverageNotesPerSecond, digit)
		},
		{
			"average_notes_per_second_point",
			(int digit) => GetAfterPointValue(AverageNotesPerSecond, digit)
		},
		{
			"fps",
			(int digit) => GetIntValue(Fps, digit)
		},
		{
			"ups",
			(int digit) => GetIntValue(UpdatePerSecond, digit)
		}
	});

	private static readonly ReadOnlyDictionary<string, Func<double>> DoubleValueDictionary = Extensions.AsReadOnly(new Dictionary<string, Func<double>>
	{
		{
			"test",
			() => TestValue
		},
		{
			"bpm",
			() => Bpm
		},
		{
			"max_bpm",
			() => MaxBpm
		},
		{
			"min_bpm",
			() => MinBpm
		},
		{
			"main_notes_bpm",
			() => MainNotesBpm
		},
		{
			"main_time_bpm",
			() => MainTimeBpm
		},
		{
			"1p_hs",
			() => Player1HiSpeed
		},
		{
			"2p_hs",
			() => Player2HiSpeed
		},
		{
			"1p_lanecover",
			() => (double)Player1LaneCover / 100.0
		},
		{
			"2p_lanecover",
			() => (double)Player2LaneCover / 100.0
		},
		{
			"1p_lift",
			() => (double)Player1Lift / 100.0
		},
		{
			"2p_lift",
			() => (double)Player2Lift / 100.0
		},
		{
			"music_speed",
			() => MusicSpeed
		},
		{
			"gauge_total",
			() => GaugeTotal
		},
		{
			"groove_gauge",
			() => GrooveGauge
		},
		{
			"gauge_total_increment",
			() => GaugeTotalIncrement
		},
		{
			"load_percentage",
			() => LoadPercentage
		},
		{
			"music_time_position",
			() => MusicTimePosition
		},
		{
			"judge_timing",
			() => JudgeTiming
		},
		{
			"average_judge_timing",
			() => AverageJudgeTiming
		},
		{
			"average_notes_per_second",
			() => AverageNotesPerSecond
		},
		{
			"score_rate",
			() => ScoreRate
		}
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
