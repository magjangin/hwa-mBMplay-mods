using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BeMusic;
using MBMPlay.Common;
using MBMPlay.Platform;
using MistUtils;

namespace MBMPlay;

public static class Flags
{
	private static bool _masterVolMute = false;

	private static readonly ReadOnlyDictionary<string, Func<bool>> FlagDictionary = Extensions.AsReadOnly(new Dictionary<string, Func<bool>>
	{
		{
			"autoplay",
			() => AutoPlay
		},
		{
			"loading",
			() => Loading
		},
		{
			"load_complete",
			() => LoadComplete
		},
		{
			"legacy_ln_mode",
			() => LegacyLN
		},
		{
			"display_fps",
			() => DisplayFps
		},
		{
			"display_note_count",
			() => DisplayNoteCount
		},
		{
			"1p_lanecover_on",
			() => Player1LaneCoverEnabled
		},
		{
			"2p_lanecover_on",
			() => Player2LaneCoverEnabled
		},
		{
			"lanecover_on",
			() => Player1LaneCoverEnabled || Player2LaneCoverEnabled
		},
		{
			"total_defined",
			() => IsTotalDefined
		},
		{
			"1p_key1_contains_notes",
			() => IsLaneContainsNotes(1)
		},
		{
			"1p_key2_contains_notes",
			() => IsLaneContainsNotes(2)
		},
		{
			"1p_key3_contains_notes",
			() => IsLaneContainsNotes(3)
		},
		{
			"1p_key4_contains_notes",
			() => IsLaneContainsNotes(4)
		},
		{
			"1p_key5_contains_notes",
			() => IsLaneContainsNotes(5)
		},
		{
			"1p_key6_contains_notes",
			() => IsLaneContainsNotes(6)
		},
		{
			"1p_key7_contains_notes",
			() => IsLaneContainsNotes(7)
		},
		{
			"1p_key8_contains_notes",
			() => IsLaneContainsNotes(8)
		},
		{
			"1p_key9_contains_notes",
			() => IsLaneContainsNotes(9)
		},
		{
			"1p_scratch_contains_notes",
			() => IsLaneContainsNotes(0)
		},
		{
			"2p_key1_contains_notes",
			() => IsLaneContainsNotes(11)
		},
		{
			"2p_key2_contains_notes",
			() => IsLaneContainsNotes(12)
		},
		{
			"2p_key3_contains_notes",
			() => IsLaneContainsNotes(13)
		},
		{
			"2p_key4_contains_notes",
			() => IsLaneContainsNotes(14)
		},
		{
			"2p_key5_contains_notes",
			() => IsLaneContainsNotes(15)
		},
		{
			"2p_key6_contains_notes",
			() => IsLaneContainsNotes(16)
		},
		{
			"2p_key7_contains_notes",
			() => IsLaneContainsNotes(17)
		},
		{
			"2p_key8_contains_notes",
			() => IsLaneContainsNotes(18)
		},
		{
			"2p_key9_contains_notes",
			() => IsLaneContainsNotes(19)
		},
		{
			"2p_scratch_contains_notes",
			() => IsLaneContainsNotes(10)
		},
		{
			"1p_key1_on",
			() => IsKeyDown(PlayingKeys.Player1Key1)
		},
		{
			"1p_key2_on",
			() => IsKeyDown(PlayingKeys.Player1Key2)
		},
		{
			"1p_key3_on",
			() => IsKeyDown(PlayingKeys.Player1Key3)
		},
		{
			"1p_key4_on",
			() => IsKeyDown(PlayingKeys.Player1Key4)
		},
		{
			"1p_key5_on",
			() => IsKeyDown(PlayingKeys.Player1Key5)
		},
		{
			"1p_key6_on",
			() => IsKeyDown(PlayingKeys.Player1Key6)
		},
		{
			"1p_key7_on",
			() => IsKeyDown(PlayingKeys.Player1Key7)
		},
		{
			"1p_key8_on",
			() => IsKeyDown(PlayingKeys.Player1Key8)
		},
		{
			"1p_key9_on",
			() => IsKeyDown(PlayingKeys.Player1Key9)
		},
		{
			"1p_scratch_on",
			() => IsPlayer1ScratchDown
		},
		{
			"1p_start_on",
			() => IsKeyDown(PlayingKeys.Player1Start)
		},
		{
			"1p_select_on",
			() => IsKeyDown(PlayingKeys.Player1Select)
		},
		{
			"2p_key1_on",
			() => IsKeyDown(PlayingKeys.Player2Key1)
		},
		{
			"2p_key2_on",
			() => IsKeyDown(PlayingKeys.Player2Key2)
		},
		{
			"2p_key3_on",
			() => IsKeyDown(PlayingKeys.Player2Key3)
		},
		{
			"2p_key4_on",
			() => IsKeyDown(PlayingKeys.Player2Key4)
		},
		{
			"2p_key5_on",
			() => IsKeyDown(PlayingKeys.Player2Key5)
		},
		{
			"2p_key6_on",
			() => IsKeyDown(PlayingKeys.Player2Key6)
		},
		{
			"2p_key7_on",
			() => IsKeyDown(PlayingKeys.Player2Key7)
		},
		{
			"2p_key8_on",
			() => IsKeyDown(PlayingKeys.Player2Key8)
		},
		{
			"2p_key9_on",
			() => IsKeyDown(PlayingKeys.Player2Key9)
		},
		{
			"2p_scratch_on",
			() => IsPlayer2ScratchDown
		},
		{
			"2p_start_on",
			() => IsKeyDown(PlayingKeys.Player2Start)
		},
		{
			"2p_select_on",
			() => IsKeyDown(PlayingKeys.Player2Select)
		},
		{
			"1p_judge_pgreat",
			() => Player1Judge == JudgeType.PGreat
		},
		{
			"1p_key1_judge_pgreat",
			() => LaneJudge[1] == JudgeType.PGreat
		},
		{
			"1p_key2_judge_pgreat",
			() => LaneJudge[2] == JudgeType.PGreat
		},
		{
			"1p_key3_judge_pgreat",
			() => LaneJudge[3] == JudgeType.PGreat
		},
		{
			"1p_key4_judge_pgreat",
			() => LaneJudge[4] == JudgeType.PGreat
		},
		{
			"1p_key5_judge_pgreat",
			() => LaneJudge[5] == JudgeType.PGreat
		},
		{
			"1p_key6_judge_pgreat",
			() => LaneJudge[6] == JudgeType.PGreat
		},
		{
			"1p_key7_judge_pgreat",
			() => LaneJudge[7] == JudgeType.PGreat
		},
		{
			"1p_key8_judge_pgreat",
			() => LaneJudge[8] == JudgeType.PGreat
		},
		{
			"1p_key9_judge_pgreat",
			() => LaneJudge[9] == JudgeType.PGreat
		},
		{
			"1p_scratch_judge_pgreat",
			() => LaneJudge[0] == JudgeType.PGreat
		},
		{
			"1p_judge_left_pgreat",
			() => Player1JudgeLeft == JudgeType.PGreat
		},
		{
			"1p_judge_center_pgreat",
			() => Player1JudgeCenter == JudgeType.PGreat
		},
		{
			"1p_judge_right_pgreat",
			() => Player1JudgeRight == JudgeType.PGreat
		},
		{
			"2p_judge_pgreat",
			() => Player2Judge == JudgeType.PGreat
		},
		{
			"2p_key1_judge_pgreat",
			() => LaneJudge[11] == JudgeType.PGreat
		},
		{
			"2p_key2_judge_pgreat",
			() => LaneJudge[12] == JudgeType.PGreat
		},
		{
			"2p_key3_judge_pgreat",
			() => LaneJudge[13] == JudgeType.PGreat
		},
		{
			"2p_key4_judge_pgreat",
			() => LaneJudge[14] == JudgeType.PGreat
		},
		{
			"2p_key5_judge_pgreat",
			() => LaneJudge[15] == JudgeType.PGreat
		},
		{
			"2p_key6_judge_pgreat",
			() => LaneJudge[16] == JudgeType.PGreat
		},
		{
			"2p_key7_judge_pgreat",
			() => LaneJudge[17] == JudgeType.PGreat
		},
		{
			"2p_key8_judge_pgreat",
			() => LaneJudge[18] == JudgeType.PGreat
		},
		{
			"2p_key9_judge_pgreat",
			() => LaneJudge[19] == JudgeType.PGreat
		},
		{
			"2p_scratch_judge_pgreat",
			() => LaneJudge[10] == JudgeType.PGreat
		},
		{
			"2p_judge_left_pgreat",
			() => Player2JudgeLeft == JudgeType.PGreat
		},
		{
			"2p_judge_center_pgreat",
			() => Player2JudgeCenter == JudgeType.PGreat
		},
		{
			"2p_judge_right_pgreat",
			() => Player2JudgeRight == JudgeType.PGreat
		},
		{
			"1p_judge_great",
			() => Player1Judge == JudgeType.Great
		},
		{
			"1p_key1_judge_great",
			() => LaneJudge[1] == JudgeType.Great
		},
		{
			"1p_key2_judge_great",
			() => LaneJudge[2] == JudgeType.Great
		},
		{
			"1p_key3_judge_great",
			() => LaneJudge[3] == JudgeType.Great
		},
		{
			"1p_key4_judge_great",
			() => LaneJudge[4] == JudgeType.Great
		},
		{
			"1p_key5_judge_great",
			() => LaneJudge[5] == JudgeType.Great
		},
		{
			"1p_key6_judge_great",
			() => LaneJudge[6] == JudgeType.Great
		},
		{
			"1p_key7_judge_great",
			() => LaneJudge[7] == JudgeType.Great
		},
		{
			"1p_key8_judge_great",
			() => LaneJudge[8] == JudgeType.Great
		},
		{
			"1p_key9_judge_great",
			() => LaneJudge[9] == JudgeType.Great
		},
		{
			"1p_scratch_judge_great",
			() => LaneJudge[0] == JudgeType.Great
		},
		{
			"1p_judge_left_great",
			() => Player1JudgeLeft == JudgeType.Great
		},
		{
			"1p_judge_center_great",
			() => Player1JudgeCenter == JudgeType.Great
		},
		{
			"1p_judge_right_great",
			() => Player1JudgeRight == JudgeType.Great
		},
		{
			"2p_judge_great",
			() => Player2Judge == JudgeType.Great
		},
		{
			"2p_key1_judge_great",
			() => LaneJudge[11] == JudgeType.Great
		},
		{
			"2p_key2_judge_great",
			() => LaneJudge[12] == JudgeType.Great
		},
		{
			"2p_key3_judge_great",
			() => LaneJudge[13] == JudgeType.Great
		},
		{
			"2p_key4_judge_great",
			() => LaneJudge[14] == JudgeType.Great
		},
		{
			"2p_key5_judge_great",
			() => LaneJudge[15] == JudgeType.Great
		},
		{
			"2p_key6_judge_great",
			() => LaneJudge[16] == JudgeType.Great
		},
		{
			"2p_key7_judge_great",
			() => LaneJudge[17] == JudgeType.Great
		},
		{
			"2p_key8_judge_great",
			() => LaneJudge[18] == JudgeType.Great
		},
		{
			"2p_key9_judge_great",
			() => LaneJudge[19] == JudgeType.Great
		},
		{
			"2p_scratch_judge_great",
			() => LaneJudge[10] == JudgeType.Great
		},
		{
			"2p_judge_left_great",
			() => Player2JudgeLeft == JudgeType.Great
		},
		{
			"2p_judge_center_great",
			() => Player2JudgeCenter == JudgeType.Great
		},
		{
			"2p_judge_right_great",
			() => Player2JudgeRight == JudgeType.Great
		},
		{
			"1p_judge_good",
			() => Player1Judge == JudgeType.Good
		},
		{
			"1p_key1_judge_good",
			() => LaneJudge[1] == JudgeType.Good
		},
		{
			"1p_key2_judge_good",
			() => LaneJudge[2] == JudgeType.Good
		},
		{
			"1p_key3_judge_good",
			() => LaneJudge[3] == JudgeType.Good
		},
		{
			"1p_key4_judge_good",
			() => LaneJudge[4] == JudgeType.Good
		},
		{
			"1p_key5_judge_good",
			() => LaneJudge[5] == JudgeType.Good
		},
		{
			"1p_key6_judge_good",
			() => LaneJudge[6] == JudgeType.Good
		},
		{
			"1p_key7_judge_good",
			() => LaneJudge[7] == JudgeType.Good
		},
		{
			"1p_key8_judge_good",
			() => LaneJudge[8] == JudgeType.Good
		},
		{
			"1p_key9_judge_good",
			() => LaneJudge[9] == JudgeType.Good
		},
		{
			"1p_scratch_judge_good",
			() => LaneJudge[0] == JudgeType.Good
		},
		{
			"1p_judge_left_good",
			() => Player1JudgeLeft == JudgeType.Good
		},
		{
			"1p_judge_center_good",
			() => Player1JudgeCenter == JudgeType.Good
		},
		{
			"1p_judge_right_good",
			() => Player1JudgeRight == JudgeType.Good
		},
		{
			"2p_judge_good",
			() => Player2Judge == JudgeType.Good
		},
		{
			"2p_key1_judge_good",
			() => LaneJudge[11] == JudgeType.Good
		},
		{
			"2p_key2_judge_good",
			() => LaneJudge[12] == JudgeType.Good
		},
		{
			"2p_key3_judge_good",
			() => LaneJudge[13] == JudgeType.Good
		},
		{
			"2p_key4_judge_good",
			() => LaneJudge[14] == JudgeType.Good
		},
		{
			"2p_key5_judge_good",
			() => LaneJudge[15] == JudgeType.Good
		},
		{
			"2p_key6_judge_good",
			() => LaneJudge[16] == JudgeType.Good
		},
		{
			"2p_key7_judge_good",
			() => LaneJudge[17] == JudgeType.Good
		},
		{
			"2p_key8_judge_good",
			() => LaneJudge[18] == JudgeType.Good
		},
		{
			"2p_key9_judge_good",
			() => LaneJudge[19] == JudgeType.Good
		},
		{
			"2p_scratch_judge_good",
			() => LaneJudge[10] == JudgeType.Good
		},
		{
			"2p_judge_left_good",
			() => Player2JudgeLeft == JudgeType.Good
		},
		{
			"2p_judge_center_good",
			() => Player2JudgeCenter == JudgeType.Good
		},
		{
			"2p_judge_right_good",
			() => Player2JudgeRight == JudgeType.Good
		},
		{
			"1p_judge_bad",
			() => Player1Judge == JudgeType.Bad
		},
		{
			"1p_key1_judge_bad",
			() => LaneJudge[1] == JudgeType.Bad
		},
		{
			"1p_key2_judge_bad",
			() => LaneJudge[2] == JudgeType.Bad
		},
		{
			"1p_key3_judge_bad",
			() => LaneJudge[3] == JudgeType.Bad
		},
		{
			"1p_key4_judge_bad",
			() => LaneJudge[4] == JudgeType.Bad
		},
		{
			"1p_key5_judge_bad",
			() => LaneJudge[5] == JudgeType.Bad
		},
		{
			"1p_key6_judge_bad",
			() => LaneJudge[6] == JudgeType.Bad
		},
		{
			"1p_key7_judge_bad",
			() => LaneJudge[7] == JudgeType.Bad
		},
		{
			"1p_key8_judge_bad",
			() => LaneJudge[8] == JudgeType.Bad
		},
		{
			"1p_key9_judge_bad",
			() => LaneJudge[9] == JudgeType.Bad
		},
		{
			"1p_scratch_judge_bad",
			() => LaneJudge[0] == JudgeType.Bad
		},
		{
			"1p_judge_left_bad",
			() => Player1JudgeLeft == JudgeType.Bad
		},
		{
			"1p_judge_center_bad",
			() => Player1JudgeCenter == JudgeType.Bad
		},
		{
			"1p_judge_right_bad",
			() => Player1JudgeRight == JudgeType.Bad
		},
		{
			"2p_judge_bad",
			() => Player2Judge == JudgeType.Bad
		},
		{
			"2p_key1_judge_bad",
			() => LaneJudge[11] == JudgeType.Bad
		},
		{
			"2p_key2_judge_bad",
			() => LaneJudge[12] == JudgeType.Bad
		},
		{
			"2p_key3_judge_bad",
			() => LaneJudge[13] == JudgeType.Bad
		},
		{
			"2p_key4_judge_bad",
			() => LaneJudge[14] == JudgeType.Bad
		},
		{
			"2p_key5_judge_bad",
			() => LaneJudge[15] == JudgeType.Bad
		},
		{
			"2p_key6_judge_bad",
			() => LaneJudge[16] == JudgeType.Bad
		},
		{
			"2p_key7_judge_bad",
			() => LaneJudge[17] == JudgeType.Bad
		},
		{
			"2p_key8_judge_bad",
			() => LaneJudge[18] == JudgeType.Bad
		},
		{
			"2p_key9_judge_bad",
			() => LaneJudge[19] == JudgeType.Bad
		},
		{
			"2p_scratch_judge_bad",
			() => LaneJudge[10] == JudgeType.Bad
		},
		{
			"2p_judge_left_bad",
			() => Player2JudgeLeft == JudgeType.Bad
		},
		{
			"2p_judge_center_bad",
			() => Player2JudgeCenter == JudgeType.Bad
		},
		{
			"2p_judge_right_bad",
			() => Player2JudgeRight == JudgeType.Bad
		},
		{
			"1p_judge_passed_poor",
			() => Player1Judge == JudgeType.PassedPoor
		},
		{
			"1p_key1_judge_passed_poor",
			() => LaneJudge[1] == JudgeType.PassedPoor
		},
		{
			"1p_key2_judge_passed_poor",
			() => LaneJudge[2] == JudgeType.PassedPoor
		},
		{
			"1p_key3_judge_passed_poor",
			() => LaneJudge[3] == JudgeType.PassedPoor
		},
		{
			"1p_key4_judge_passed_poor",
			() => LaneJudge[4] == JudgeType.PassedPoor
		},
		{
			"1p_key5_judge_passed_poor",
			() => LaneJudge[5] == JudgeType.PassedPoor
		},
		{
			"1p_key6_judge_passed_poor",
			() => LaneJudge[6] == JudgeType.PassedPoor
		},
		{
			"1p_key7_judge_passed_poor",
			() => LaneJudge[7] == JudgeType.PassedPoor
		},
		{
			"1p_key8_judge_passed_poor",
			() => LaneJudge[8] == JudgeType.PassedPoor
		},
		{
			"1p_key9_judge_passed_poor",
			() => LaneJudge[9] == JudgeType.PassedPoor
		},
		{
			"1p_scratch_judge_passed_poor",
			() => LaneJudge[0] == JudgeType.PassedPoor
		},
		{
			"1p_judge_left_passed_poor",
			() => Player1JudgeLeft == JudgeType.PassedPoor
		},
		{
			"1p_judge_center_passed_poor",
			() => Player1JudgeCenter == JudgeType.PassedPoor
		},
		{
			"1p_judge_right_passed_poor",
			() => Player1JudgeRight == JudgeType.PassedPoor
		},
		{
			"2p_judge_passed_poor",
			() => Player2Judge == JudgeType.PassedPoor
		},
		{
			"2p_key1_judge_passed_poor",
			() => LaneJudge[11] == JudgeType.PassedPoor
		},
		{
			"2p_key2_judge_passed_poor",
			() => LaneJudge[12] == JudgeType.PassedPoor
		},
		{
			"2p_key3_judge_passed_poor",
			() => LaneJudge[13] == JudgeType.PassedPoor
		},
		{
			"2p_key4_judge_passed_poor",
			() => LaneJudge[14] == JudgeType.PassedPoor
		},
		{
			"2p_key5_judge_passed_poor",
			() => LaneJudge[15] == JudgeType.PassedPoor
		},
		{
			"2p_key6_judge_passed_poor",
			() => LaneJudge[16] == JudgeType.PassedPoor
		},
		{
			"2p_key7_judge_passed_poor",
			() => LaneJudge[17] == JudgeType.PassedPoor
		},
		{
			"2p_key8_judge_passed_poor",
			() => LaneJudge[18] == JudgeType.PassedPoor
		},
		{
			"2p_key9_judge_passed_poor",
			() => LaneJudge[19] == JudgeType.PassedPoor
		},
		{
			"2p_scratch_judge_passed_poor",
			() => LaneJudge[10] == JudgeType.PassedPoor
		},
		{
			"2p_judge_left_passed_poor",
			() => Player2JudgeLeft == JudgeType.PassedPoor
		},
		{
			"2p_judge_center_passed_poor",
			() => Player2JudgeCenter == JudgeType.PassedPoor
		},
		{
			"2p_judge_right_passed_poor",
			() => Player2JudgeRight == JudgeType.PassedPoor
		},
		{
			"1p_judge_wasted_poor",
			() => Player1Judge == JudgeType.WastedPoor
		},
		{
			"1p_key1_judge_wasted_poor",
			() => LaneJudge[1] == JudgeType.WastedPoor
		},
		{
			"1p_key2_judge_wasted_poor",
			() => LaneJudge[2] == JudgeType.WastedPoor
		},
		{
			"1p_key3_judge_wasted_poor",
			() => LaneJudge[3] == JudgeType.WastedPoor
		},
		{
			"1p_key4_judge_wasted_poor",
			() => LaneJudge[4] == JudgeType.WastedPoor
		},
		{
			"1p_key5_judge_wasted_poor",
			() => LaneJudge[5] == JudgeType.WastedPoor
		},
		{
			"1p_key6_judge_wasted_poor",
			() => LaneJudge[6] == JudgeType.WastedPoor
		},
		{
			"1p_key7_judge_wasted_poor",
			() => LaneJudge[7] == JudgeType.WastedPoor
		},
		{
			"1p_key8_judge_wasted_poor",
			() => LaneJudge[8] == JudgeType.WastedPoor
		},
		{
			"1p_key9_judge_wasted_poor",
			() => LaneJudge[9] == JudgeType.WastedPoor
		},
		{
			"1p_scratch_judge_wasted_poor",
			() => LaneJudge[0] == JudgeType.WastedPoor
		},
		{
			"1p_judge_left_wasted_poor",
			() => Player1JudgeLeft == JudgeType.WastedPoor
		},
		{
			"1p_judge_center_wasted_poor",
			() => Player1JudgeCenter == JudgeType.WastedPoor
		},
		{
			"1p_judge_right_wasted_poor",
			() => Player1JudgeRight == JudgeType.WastedPoor
		},
		{
			"2p_judge_wasted_poor",
			() => Player2Judge == JudgeType.WastedPoor
		},
		{
			"2p_key1_judge_wasted_poor",
			() => LaneJudge[11] == JudgeType.WastedPoor
		},
		{
			"2p_key2_judge_wasted_poor",
			() => LaneJudge[12] == JudgeType.WastedPoor
		},
		{
			"2p_key3_judge_wasted_poor",
			() => LaneJudge[13] == JudgeType.WastedPoor
		},
		{
			"2p_key4_judge_wasted_poor",
			() => LaneJudge[14] == JudgeType.WastedPoor
		},
		{
			"2p_key5_judge_wasted_poor",
			() => LaneJudge[15] == JudgeType.WastedPoor
		},
		{
			"2p_key6_judge_wasted_poor",
			() => LaneJudge[16] == JudgeType.WastedPoor
		},
		{
			"2p_key7_judge_wasted_poor",
			() => LaneJudge[17] == JudgeType.WastedPoor
		},
		{
			"2p_key8_judge_wasted_poor",
			() => LaneJudge[18] == JudgeType.WastedPoor
		},
		{
			"2p_key9_judge_wasted_poor",
			() => LaneJudge[19] == JudgeType.WastedPoor
		},
		{
			"2p_scratch_judge_wasted_poor",
			() => LaneJudge[10] == JudgeType.WastedPoor
		},
		{
			"2p_judge_left_wasted_poor",
			() => Player2JudgeLeft == JudgeType.WastedPoor
		},
		{
			"2p_judge_center_wasted_poor",
			() => Player2JudgeCenter == JudgeType.WastedPoor
		},
		{
			"2p_judge_right_wasted_poor",
			() => Player2JudgeRight == JudgeType.WastedPoor
		},
		{
			"1p_judge_poor",
			() => Player1Judge <= JudgeType.PassedPoor
		},
		{
			"1p_key1_judge_poor",
			() => LaneJudge[1] <= JudgeType.PassedPoor
		},
		{
			"1p_key2_judge_poor",
			() => LaneJudge[2] <= JudgeType.PassedPoor
		},
		{
			"1p_key3_judge_poor",
			() => LaneJudge[3] <= JudgeType.PassedPoor
		},
		{
			"1p_key4_judge_poor",
			() => LaneJudge[4] <= JudgeType.PassedPoor
		},
		{
			"1p_key5_judge_poor",
			() => LaneJudge[5] <= JudgeType.PassedPoor
		},
		{
			"1p_key6_judge_poor",
			() => LaneJudge[6] <= JudgeType.PassedPoor
		},
		{
			"1p_key7_judge_poor",
			() => LaneJudge[7] <= JudgeType.PassedPoor
		},
		{
			"1p_key8_judge_poor",
			() => LaneJudge[8] <= JudgeType.PassedPoor
		},
		{
			"1p_key9_judge_poor",
			() => LaneJudge[9] <= JudgeType.PassedPoor
		},
		{
			"1p_scratch_judge_poor",
			() => LaneJudge[0] <= JudgeType.PassedPoor
		},
		{
			"1p_judge_left_poor",
			() => Player1JudgeLeft <= JudgeType.PassedPoor
		},
		{
			"1p_judge_center_poor",
			() => Player1JudgeCenter <= JudgeType.PassedPoor
		},
		{
			"1p_judge_right_poor",
			() => Player1JudgeRight <= JudgeType.PassedPoor
		},
		{
			"2p_judge_poor",
			() => Player2Judge <= JudgeType.PassedPoor
		},
		{
			"2p_key1_judge_poor",
			() => LaneJudge[11] <= JudgeType.PassedPoor
		},
		{
			"2p_key2_judge_poor",
			() => LaneJudge[12] <= JudgeType.PassedPoor
		},
		{
			"2p_key3_judge_poor",
			() => LaneJudge[13] <= JudgeType.PassedPoor
		},
		{
			"2p_key4_judge_poor",
			() => LaneJudge[14] <= JudgeType.PassedPoor
		},
		{
			"2p_key5_judge_poor",
			() => LaneJudge[15] <= JudgeType.PassedPoor
		},
		{
			"2p_key6_judge_poor",
			() => LaneJudge[16] <= JudgeType.PassedPoor
		},
		{
			"2p_key7_judge_poor",
			() => LaneJudge[17] <= JudgeType.PassedPoor
		},
		{
			"2p_key8_judge_poor",
			() => LaneJudge[18] <= JudgeType.PassedPoor
		},
		{
			"2p_key9_judge_poor",
			() => LaneJudge[19] <= JudgeType.PassedPoor
		},
		{
			"2p_scratch_judge_poor",
			() => LaneJudge[10] <= JudgeType.PassedPoor
		},
		{
			"2p_judge_left_poor",
			() => Player2JudgeLeft <= JudgeType.PassedPoor
		},
		{
			"2p_judge_center_poor",
			() => Player2JudgeCenter <= JudgeType.PassedPoor
		},
		{
			"2p_judge_right_poor",
			() => Player2JudgeRight <= JudgeType.PassedPoor
		},
		{
			"1p_judge_great_or_better",
			() => Player1Judge >= JudgeType.Great
		},
		{
			"1p_key1_judge_great_or_better",
			() => LaneJudge[1] >= JudgeType.Great
		},
		{
			"1p_key2_judge_great_or_better",
			() => LaneJudge[2] >= JudgeType.Great
		},
		{
			"1p_key3_judge_great_or_better",
			() => LaneJudge[3] >= JudgeType.Great
		},
		{
			"1p_key4_judge_great_or_better",
			() => LaneJudge[4] >= JudgeType.Great
		},
		{
			"1p_key5_judge_great_or_better",
			() => LaneJudge[5] >= JudgeType.Great
		},
		{
			"1p_key6_judge_great_or_better",
			() => LaneJudge[6] >= JudgeType.Great
		},
		{
			"1p_key7_judge_great_or_better",
			() => LaneJudge[7] >= JudgeType.Great
		},
		{
			"1p_key8_judge_great_or_better",
			() => LaneJudge[8] >= JudgeType.Great
		},
		{
			"1p_key9_judge_great_or_better",
			() => LaneJudge[9] >= JudgeType.Great
		},
		{
			"1p_scratch_judge_great_or_better",
			() => LaneJudge[0] >= JudgeType.Great
		},
		{
			"1p_judge_left_great_or_better",
			() => Player1JudgeLeft >= JudgeType.Great
		},
		{
			"1p_judge_center_great_or_better",
			() => Player1JudgeCenter >= JudgeType.Great
		},
		{
			"1p_judge_right_great_or_better",
			() => Player1JudgeRight >= JudgeType.Great
		},
		{
			"2p_judge_great_or_better",
			() => Player2Judge >= JudgeType.Great
		},
		{
			"2p_key1_judge_great_or_better",
			() => LaneJudge[11] >= JudgeType.Great
		},
		{
			"2p_key2_judge_great_or_better",
			() => LaneJudge[12] >= JudgeType.Great
		},
		{
			"2p_key3_judge_great_or_better",
			() => LaneJudge[13] >= JudgeType.Great
		},
		{
			"2p_key4_judge_great_or_better",
			() => LaneJudge[14] >= JudgeType.Great
		},
		{
			"2p_key5_judge_great_or_better",
			() => LaneJudge[15] >= JudgeType.Great
		},
		{
			"2p_key6_judge_great_or_better",
			() => LaneJudge[16] >= JudgeType.Great
		},
		{
			"2p_key7_judge_great_or_better",
			() => LaneJudge[17] >= JudgeType.Great
		},
		{
			"2p_key8_judge_great_or_better",
			() => LaneJudge[18] >= JudgeType.Great
		},
		{
			"2p_key9_judge_great_or_better",
			() => LaneJudge[19] >= JudgeType.Great
		},
		{
			"2p_scratch_judge_great_or_better",
			() => LaneJudge[10] >= JudgeType.Great
		},
		{
			"2p_judge_left_great_or_better",
			() => Player2JudgeLeft >= JudgeType.Great
		},
		{
			"2p_judge_center_great_or_better",
			() => Player2JudgeCenter >= JudgeType.Great
		},
		{
			"2p_judge_right_great_or_better",
			() => Player2JudgeRight >= JudgeType.Great
		},
		{
			"1p_judge_good_or_better",
			() => Player1Judge >= JudgeType.Good
		},
		{
			"1p_key1_judge_good_or_better",
			() => LaneJudge[1] >= JudgeType.Good
		},
		{
			"1p_key2_judge_good_or_better",
			() => LaneJudge[2] >= JudgeType.Good
		},
		{
			"1p_key3_judge_good_or_better",
			() => LaneJudge[3] >= JudgeType.Good
		},
		{
			"1p_key4_judge_good_or_better",
			() => LaneJudge[4] >= JudgeType.Good
		},
		{
			"1p_key5_judge_good_or_better",
			() => LaneJudge[5] >= JudgeType.Good
		},
		{
			"1p_key6_judge_good_or_better",
			() => LaneJudge[6] >= JudgeType.Good
		},
		{
			"1p_key7_judge_good_or_better",
			() => LaneJudge[7] >= JudgeType.Good
		},
		{
			"1p_key8_judge_good_or_better",
			() => LaneJudge[8] >= JudgeType.Good
		},
		{
			"1p_key9_judge_good_or_better",
			() => LaneJudge[9] >= JudgeType.Good
		},
		{
			"1p_scratch_judge_good_or_better",
			() => LaneJudge[0] >= JudgeType.Good
		},
		{
			"1p_judge_left_good_or_better",
			() => Player1JudgeLeft >= JudgeType.Good
		},
		{
			"1p_judge_center_good_or_better",
			() => Player1JudgeCenter >= JudgeType.Good
		},
		{
			"1p_judge_right_good_or_better",
			() => Player1JudgeRight >= JudgeType.Good
		},
		{
			"2p_judge_good_or_better",
			() => Player2Judge >= JudgeType.Good
		},
		{
			"2p_key1_judge_good_or_better",
			() => LaneJudge[11] >= JudgeType.Good
		},
		{
			"2p_key2_judge_good_or_better",
			() => LaneJudge[12] >= JudgeType.Good
		},
		{
			"2p_key3_judge_good_or_better",
			() => LaneJudge[13] >= JudgeType.Good
		},
		{
			"2p_key4_judge_good_or_better",
			() => LaneJudge[14] >= JudgeType.Good
		},
		{
			"2p_key5_judge_good_or_better",
			() => LaneJudge[15] >= JudgeType.Good
		},
		{
			"2p_key6_judge_good_or_better",
			() => LaneJudge[16] >= JudgeType.Good
		},
		{
			"2p_key7_judge_good_or_better",
			() => LaneJudge[17] >= JudgeType.Good
		},
		{
			"2p_key8_judge_good_or_better",
			() => LaneJudge[18] >= JudgeType.Good
		},
		{
			"2p_key9_judge_good_or_better",
			() => LaneJudge[19] >= JudgeType.Good
		},
		{
			"2p_scratch_judge_good_or_better",
			() => LaneJudge[10] >= JudgeType.Good
		},
		{
			"2p_judge_left_good_or_better",
			() => Player2JudgeLeft >= JudgeType.Good
		},
		{
			"2p_judge_center_good_or_better",
			() => Player2JudgeCenter >= JudgeType.Good
		},
		{
			"2p_judge_right_good_or_better",
			() => Player2JudgeRight >= JudgeType.Good
		},
		{
			"1p_judge_bad_or_worse",
			() => Player1Judge <= JudgeType.Bad
		},
		{
			"1p_key1_judge_bad_or_worse",
			() => LaneJudge[1] <= JudgeType.Bad
		},
		{
			"1p_key2_judge_bad_or_worse",
			() => LaneJudge[2] <= JudgeType.Bad
		},
		{
			"1p_key3_judge_bad_or_worse",
			() => LaneJudge[3] <= JudgeType.Bad
		},
		{
			"1p_key4_judge_bad_or_worse",
			() => LaneJudge[4] <= JudgeType.Bad
		},
		{
			"1p_key5_judge_bad_or_worse",
			() => LaneJudge[5] <= JudgeType.Bad
		},
		{
			"1p_key6_judge_bad_or_worse",
			() => LaneJudge[6] <= JudgeType.Bad
		},
		{
			"1p_key7_judge_bad_or_worse",
			() => LaneJudge[7] <= JudgeType.Bad
		},
		{
			"1p_key8_judge_bad_or_worse",
			() => LaneJudge[8] <= JudgeType.Bad
		},
		{
			"1p_key9_judge_bad_or_worse",
			() => LaneJudge[9] <= JudgeType.Bad
		},
		{
			"1p_scratch_judge_bad_or_worse",
			() => LaneJudge[0] <= JudgeType.Bad
		},
		{
			"1p_judge_left_bad_or_worse",
			() => Player1JudgeLeft <= JudgeType.Bad
		},
		{
			"1p_judge_center_bad_or_worse",
			() => Player1JudgeCenter <= JudgeType.Bad
		},
		{
			"1p_judge_right_bad_or_worse",
			() => Player1JudgeRight <= JudgeType.Bad
		},
		{
			"2p_judge_bad_or_worse",
			() => Player2Judge <= JudgeType.Bad
		},
		{
			"2p_key1_judge_bad_or_worse",
			() => LaneJudge[11] <= JudgeType.Bad
		},
		{
			"2p_key2_judge_bad_or_worse",
			() => LaneJudge[12] <= JudgeType.Bad
		},
		{
			"2p_key3_judge_bad_or_worse",
			() => LaneJudge[13] <= JudgeType.Bad
		},
		{
			"2p_key4_judge_bad_or_worse",
			() => LaneJudge[14] <= JudgeType.Bad
		},
		{
			"2p_key5_judge_bad_or_worse",
			() => LaneJudge[15] <= JudgeType.Bad
		},
		{
			"2p_key6_judge_bad_or_worse",
			() => LaneJudge[16] <= JudgeType.Bad
		},
		{
			"2p_key7_judge_bad_or_worse",
			() => LaneJudge[17] <= JudgeType.Bad
		},
		{
			"2p_key8_judge_bad_or_worse",
			() => LaneJudge[18] <= JudgeType.Bad
		},
		{
			"2p_key9_judge_bad_or_worse",
			() => LaneJudge[19] <= JudgeType.Bad
		},
		{
			"2p_scratch_judge_bad_or_worse",
			() => LaneJudge[10] <= JudgeType.Bad
		},
		{
			"2p_judge_left_bad_or_worse",
			() => Player2JudgeLeft <= JudgeType.Bad
		},
		{
			"2p_judge_center_bad_or_worse",
			() => Player2JudgeCenter <= JudgeType.Bad
		},
		{
			"2p_judge_right_bad_or_worse",
			() => Player2JudgeRight <= JudgeType.Bad
		},
		{
			"1p_key1_laser_judge_pgreat",
			() => LaneLaserJudge[1] == JudgeType.PGreat
		},
		{
			"1p_key2_laser_judge_pgreat",
			() => LaneLaserJudge[2] == JudgeType.PGreat
		},
		{
			"1p_key3_laser_judge_pgreat",
			() => LaneLaserJudge[3] == JudgeType.PGreat
		},
		{
			"1p_key4_laser_judge_pgreat",
			() => LaneLaserJudge[4] == JudgeType.PGreat
		},
		{
			"1p_key5_laser_judge_pgreat",
			() => LaneLaserJudge[5] == JudgeType.PGreat
		},
		{
			"1p_key6_laser_judge_pgreat",
			() => LaneLaserJudge[6] == JudgeType.PGreat
		},
		{
			"1p_key7_laser_judge_pgreat",
			() => LaneLaserJudge[7] == JudgeType.PGreat
		},
		{
			"1p_key8_laser_judge_pgreat",
			() => LaneLaserJudge[8] == JudgeType.PGreat
		},
		{
			"1p_key9_laser_judge_pgreat",
			() => LaneLaserJudge[9] == JudgeType.PGreat
		},
		{
			"1p_scratch_laser_judge_pgreat",
			() => LaneLaserJudge[0] == JudgeType.PGreat
		},
		{
			"2p_key1_laser_judge_pgreat",
			() => LaneLaserJudge[11] == JudgeType.PGreat
		},
		{
			"2p_key2_laser_judge_pgreat",
			() => LaneLaserJudge[12] == JudgeType.PGreat
		},
		{
			"2p_key3_laser_judge_pgreat",
			() => LaneLaserJudge[13] == JudgeType.PGreat
		},
		{
			"2p_key4_laser_judge_pgreat",
			() => LaneLaserJudge[14] == JudgeType.PGreat
		},
		{
			"2p_key5_laser_judge_pgreat",
			() => LaneLaserJudge[15] == JudgeType.PGreat
		},
		{
			"2p_key6_laser_judge_pgreat",
			() => LaneLaserJudge[16] == JudgeType.PGreat
		},
		{
			"2p_key7_laser_judge_pgreat",
			() => LaneLaserJudge[17] == JudgeType.PGreat
		},
		{
			"2p_key8_laser_judge_pgreat",
			() => LaneLaserJudge[18] == JudgeType.PGreat
		},
		{
			"2p_key9_laser_judge_pgreat",
			() => LaneLaserJudge[19] == JudgeType.PGreat
		},
		{
			"2p_scratch_laser_judge_pgreat",
			() => LaneLaserJudge[10] == JudgeType.PGreat
		},
		{
			"1p_key1_laser_judge_great",
			() => LaneLaserJudge[1] == JudgeType.Great
		},
		{
			"1p_key2_laser_judge_great",
			() => LaneLaserJudge[2] == JudgeType.Great
		},
		{
			"1p_key3_laser_judge_great",
			() => LaneLaserJudge[3] == JudgeType.Great
		},
		{
			"1p_key4_laser_judge_great",
			() => LaneLaserJudge[4] == JudgeType.Great
		},
		{
			"1p_key5_laser_judge_great",
			() => LaneLaserJudge[5] == JudgeType.Great
		},
		{
			"1p_key6_laser_judge_great",
			() => LaneLaserJudge[6] == JudgeType.Great
		},
		{
			"1p_key7_laser_judge_great",
			() => LaneLaserJudge[7] == JudgeType.Great
		},
		{
			"1p_key8_laser_judge_great",
			() => LaneLaserJudge[8] == JudgeType.Great
		},
		{
			"1p_key9_laser_judge_great",
			() => LaneLaserJudge[9] == JudgeType.Great
		},
		{
			"1p_scratch_laser_judge_great",
			() => LaneLaserJudge[0] == JudgeType.Great
		},
		{
			"2p_key1_laser_judge_great",
			() => LaneLaserJudge[11] == JudgeType.Great
		},
		{
			"2p_key2_laser_judge_great",
			() => LaneLaserJudge[12] == JudgeType.Great
		},
		{
			"2p_key3_laser_judge_great",
			() => LaneLaserJudge[13] == JudgeType.Great
		},
		{
			"2p_key4_laser_judge_great",
			() => LaneLaserJudge[14] == JudgeType.Great
		},
		{
			"2p_key5_laser_judge_great",
			() => LaneLaserJudge[15] == JudgeType.Great
		},
		{
			"2p_key6_laser_judge_great",
			() => LaneLaserJudge[16] == JudgeType.Great
		},
		{
			"2p_key7_laser_judge_great",
			() => LaneLaserJudge[17] == JudgeType.Great
		},
		{
			"2p_key8_laser_judge_great",
			() => LaneLaserJudge[18] == JudgeType.Great
		},
		{
			"2p_key9_laser_judge_great",
			() => LaneLaserJudge[19] == JudgeType.Great
		},
		{
			"2p_scratch_laser_judge_great",
			() => LaneLaserJudge[10] == JudgeType.Great
		},
		{
			"1p_key1_laser_judge_good",
			() => LaneLaserJudge[1] == JudgeType.Good
		},
		{
			"1p_key2_laser_judge_good",
			() => LaneLaserJudge[2] == JudgeType.Good
		},
		{
			"1p_key3_laser_judge_good",
			() => LaneLaserJudge[3] == JudgeType.Good
		},
		{
			"1p_key4_laser_judge_good",
			() => LaneLaserJudge[4] == JudgeType.Good
		},
		{
			"1p_key5_laser_judge_good",
			() => LaneLaserJudge[5] == JudgeType.Good
		},
		{
			"1p_key6_laser_judge_good",
			() => LaneLaserJudge[6] == JudgeType.Good
		},
		{
			"1p_key7_laser_judge_good",
			() => LaneLaserJudge[7] == JudgeType.Good
		},
		{
			"1p_key8_laser_judge_good",
			() => LaneLaserJudge[8] == JudgeType.Good
		},
		{
			"1p_key9_laser_judge_good",
			() => LaneLaserJudge[9] == JudgeType.Good
		},
		{
			"1p_scratch_laser_judge_good",
			() => LaneLaserJudge[0] == JudgeType.Good
		},
		{
			"2p_key1_laser_judge_good",
			() => LaneLaserJudge[11] == JudgeType.Good
		},
		{
			"2p_key2_laser_judge_good",
			() => LaneLaserJudge[12] == JudgeType.Good
		},
		{
			"2p_key3_laser_judge_good",
			() => LaneLaserJudge[13] == JudgeType.Good
		},
		{
			"2p_key4_laser_judge_good",
			() => LaneLaserJudge[14] == JudgeType.Good
		},
		{
			"2p_key5_laser_judge_good",
			() => LaneLaserJudge[15] == JudgeType.Good
		},
		{
			"2p_key6_laser_judge_good",
			() => LaneLaserJudge[16] == JudgeType.Good
		},
		{
			"2p_key7_laser_judge_good",
			() => LaneLaserJudge[17] == JudgeType.Good
		},
		{
			"2p_key8_laser_judge_good",
			() => LaneLaserJudge[18] == JudgeType.Good
		},
		{
			"2p_key9_laser_judge_good",
			() => LaneLaserJudge[19] == JudgeType.Good
		},
		{
			"2p_scratch_laser_judge_good",
			() => LaneLaserJudge[10] == JudgeType.Good
		},
		{
			"1p_key1_laser_judge_bad",
			() => LaneLaserJudge[1] == JudgeType.Bad
		},
		{
			"1p_key2_laser_judge_bad",
			() => LaneLaserJudge[2] == JudgeType.Bad
		},
		{
			"1p_key3_laser_judge_bad",
			() => LaneLaserJudge[3] == JudgeType.Bad
		},
		{
			"1p_key4_laser_judge_bad",
			() => LaneLaserJudge[4] == JudgeType.Bad
		},
		{
			"1p_key5_laser_judge_bad",
			() => LaneLaserJudge[5] == JudgeType.Bad
		},
		{
			"1p_key6_laser_judge_bad",
			() => LaneLaserJudge[6] == JudgeType.Bad
		},
		{
			"1p_key7_laser_judge_bad",
			() => LaneLaserJudge[7] == JudgeType.Bad
		},
		{
			"1p_key8_laser_judge_bad",
			() => LaneLaserJudge[8] == JudgeType.Bad
		},
		{
			"1p_key9_laser_judge_bad",
			() => LaneLaserJudge[9] == JudgeType.Bad
		},
		{
			"1p_scratch_laser_judge_bad",
			() => LaneLaserJudge[0] == JudgeType.Bad
		},
		{
			"2p_key1_laser_judge_bad",
			() => LaneLaserJudge[11] == JudgeType.Bad
		},
		{
			"2p_key2_laser_judge_bad",
			() => LaneLaserJudge[12] == JudgeType.Bad
		},
		{
			"2p_key3_laser_judge_bad",
			() => LaneLaserJudge[13] == JudgeType.Bad
		},
		{
			"2p_key4_laser_judge_bad",
			() => LaneLaserJudge[14] == JudgeType.Bad
		},
		{
			"2p_key5_laser_judge_bad",
			() => LaneLaserJudge[15] == JudgeType.Bad
		},
		{
			"2p_key6_laser_judge_bad",
			() => LaneLaserJudge[16] == JudgeType.Bad
		},
		{
			"2p_key7_laser_judge_bad",
			() => LaneLaserJudge[17] == JudgeType.Bad
		},
		{
			"2p_key8_laser_judge_bad",
			() => LaneLaserJudge[18] == JudgeType.Bad
		},
		{
			"2p_key9_laser_judge_bad",
			() => LaneLaserJudge[19] == JudgeType.Bad
		},
		{
			"2p_scratch_laser_judge_bad",
			() => LaneLaserJudge[10] == JudgeType.Bad
		},
		{
			"1p_key1_laser_judge_passed_poor",
			() => LaneLaserJudge[1] == JudgeType.PassedPoor
		},
		{
			"1p_key2_laser_judge_passed_poor",
			() => LaneLaserJudge[2] == JudgeType.PassedPoor
		},
		{
			"1p_key3_laser_judge_passed_poor",
			() => LaneLaserJudge[3] == JudgeType.PassedPoor
		},
		{
			"1p_key4_laser_judge_passed_poor",
			() => LaneLaserJudge[4] == JudgeType.PassedPoor
		},
		{
			"1p_key5_laser_judge_passed_poor",
			() => LaneLaserJudge[5] == JudgeType.PassedPoor
		},
		{
			"1p_key6_laser_judge_passed_poor",
			() => LaneLaserJudge[6] == JudgeType.PassedPoor
		},
		{
			"1p_key7_laser_judge_passed_poor",
			() => LaneLaserJudge[7] == JudgeType.PassedPoor
		},
		{
			"1p_key8_laser_judge_passed_poor",
			() => LaneLaserJudge[8] == JudgeType.PassedPoor
		},
		{
			"1p_key9_laser_judge_passed_poor",
			() => LaneLaserJudge[9] == JudgeType.PassedPoor
		},
		{
			"1p_scratch_laser_judge_passed_poor",
			() => LaneLaserJudge[0] == JudgeType.PassedPoor
		},
		{
			"2p_key1_laser_judge_passed_poor",
			() => LaneLaserJudge[11] == JudgeType.PassedPoor
		},
		{
			"2p_key2_laser_judge_passed_poor",
			() => LaneLaserJudge[12] == JudgeType.PassedPoor
		},
		{
			"2p_key3_laser_judge_passed_poor",
			() => LaneLaserJudge[13] == JudgeType.PassedPoor
		},
		{
			"2p_key4_laser_judge_passed_poor",
			() => LaneLaserJudge[14] == JudgeType.PassedPoor
		},
		{
			"2p_key5_laser_judge_passed_poor",
			() => LaneLaserJudge[15] == JudgeType.PassedPoor
		},
		{
			"2p_key6_laser_judge_passed_poor",
			() => LaneLaserJudge[16] == JudgeType.PassedPoor
		},
		{
			"2p_key7_laser_judge_passed_poor",
			() => LaneLaserJudge[17] == JudgeType.PassedPoor
		},
		{
			"2p_key8_laser_judge_passed_poor",
			() => LaneLaserJudge[18] == JudgeType.PassedPoor
		},
		{
			"2p_key9_laser_judge_passed_poor",
			() => LaneLaserJudge[19] == JudgeType.PassedPoor
		},
		{
			"2p_scratch_laser_judge_passed_poor",
			() => LaneLaserJudge[10] == JudgeType.PassedPoor
		},
		{
			"1p_key1_laser_judge_wasted_poor",
			() => LaneLaserJudge[1] == JudgeType.WastedPoor
		},
		{
			"1p_key2_laser_judge_wasted_poor",
			() => LaneLaserJudge[2] == JudgeType.WastedPoor
		},
		{
			"1p_key3_laser_judge_wasted_poor",
			() => LaneLaserJudge[3] == JudgeType.WastedPoor
		},
		{
			"1p_key4_laser_judge_wasted_poor",
			() => LaneLaserJudge[4] == JudgeType.WastedPoor
		},
		{
			"1p_key5_laser_judge_wasted_poor",
			() => LaneLaserJudge[5] == JudgeType.WastedPoor
		},
		{
			"1p_key6_laser_judge_wasted_poor",
			() => LaneLaserJudge[6] == JudgeType.WastedPoor
		},
		{
			"1p_key7_laser_judge_wasted_poor",
			() => LaneLaserJudge[7] == JudgeType.WastedPoor
		},
		{
			"1p_key8_laser_judge_wasted_poor",
			() => LaneLaserJudge[8] == JudgeType.WastedPoor
		},
		{
			"1p_key9_laser_judge_wasted_poor",
			() => LaneLaserJudge[9] == JudgeType.WastedPoor
		},
		{
			"1p_scratch_laser_judge_wasted_poor",
			() => LaneLaserJudge[0] == JudgeType.WastedPoor
		},
		{
			"2p_key1_laser_judge_wasted_poor",
			() => LaneLaserJudge[11] == JudgeType.WastedPoor
		},
		{
			"2p_key2_laser_judge_wasted_poor",
			() => LaneLaserJudge[12] == JudgeType.WastedPoor
		},
		{
			"2p_key3_laser_judge_wasted_poor",
			() => LaneLaserJudge[13] == JudgeType.WastedPoor
		},
		{
			"2p_key4_laser_judge_wasted_poor",
			() => LaneLaserJudge[14] == JudgeType.WastedPoor
		},
		{
			"2p_key5_laser_judge_wasted_poor",
			() => LaneLaserJudge[15] == JudgeType.WastedPoor
		},
		{
			"2p_key6_laser_judge_wasted_poor",
			() => LaneLaserJudge[16] == JudgeType.WastedPoor
		},
		{
			"2p_key7_laser_judge_wasted_poor",
			() => LaneLaserJudge[17] == JudgeType.WastedPoor
		},
		{
			"2p_key8_laser_judge_wasted_poor",
			() => LaneLaserJudge[18] == JudgeType.WastedPoor
		},
		{
			"2p_key9_laser_judge_wasted_poor",
			() => LaneLaserJudge[19] == JudgeType.WastedPoor
		},
		{
			"2p_scratch_laser_judge_wasted_poor",
			() => LaneLaserJudge[10] == JudgeType.WastedPoor
		},
		{
			"1p_key1_laser_judge_poor",
			() => LaneLaserJudge[1] <= JudgeType.PassedPoor
		},
		{
			"1p_key2_laser_judge_poor",
			() => LaneLaserJudge[2] <= JudgeType.PassedPoor
		},
		{
			"1p_key3_laser_judge_poor",
			() => LaneLaserJudge[3] <= JudgeType.PassedPoor
		},
		{
			"1p_key4_laser_judge_poor",
			() => LaneLaserJudge[4] <= JudgeType.PassedPoor
		},
		{
			"1p_key5_laser_judge_poor",
			() => LaneLaserJudge[5] <= JudgeType.PassedPoor
		},
		{
			"1p_key6_laser_judge_poor",
			() => LaneLaserJudge[6] <= JudgeType.PassedPoor
		},
		{
			"1p_key7_laser_judge_poor",
			() => LaneLaserJudge[7] <= JudgeType.PassedPoor
		},
		{
			"1p_key8_laser_judge_poor",
			() => LaneLaserJudge[8] <= JudgeType.PassedPoor
		},
		{
			"1p_key9_laser_judge_poor",
			() => LaneLaserJudge[9] <= JudgeType.PassedPoor
		},
		{
			"1p_scratch_laser_judge_poor",
			() => LaneLaserJudge[0] <= JudgeType.PassedPoor
		},
		{
			"2p_key1_laser_judge_poor",
			() => LaneLaserJudge[11] <= JudgeType.PassedPoor
		},
		{
			"2p_key2_laser_judge_poor",
			() => LaneLaserJudge[12] <= JudgeType.PassedPoor
		},
		{
			"2p_key3_laser_judge_poor",
			() => LaneLaserJudge[13] <= JudgeType.PassedPoor
		},
		{
			"2p_key4_laser_judge_poor",
			() => LaneLaserJudge[14] <= JudgeType.PassedPoor
		},
		{
			"2p_key5_laser_judge_poor",
			() => LaneLaserJudge[15] <= JudgeType.PassedPoor
		},
		{
			"2p_key6_laser_judge_poor",
			() => LaneLaserJudge[16] <= JudgeType.PassedPoor
		},
		{
			"2p_key7_laser_judge_poor",
			() => LaneLaserJudge[17] <= JudgeType.PassedPoor
		},
		{
			"2p_key8_laser_judge_poor",
			() => LaneLaserJudge[18] <= JudgeType.PassedPoor
		},
		{
			"2p_key9_laser_judge_poor",
			() => LaneLaserJudge[19] <= JudgeType.PassedPoor
		},
		{
			"2p_scratch_laser_judge_poor",
			() => LaneLaserJudge[10] <= JudgeType.PassedPoor
		},
		{
			"1p_key1_laser_judge_great_or_better",
			() => LaneLaserJudge[1] >= JudgeType.Great
		},
		{
			"1p_key2_laser_judge_great_or_better",
			() => LaneLaserJudge[2] >= JudgeType.Great
		},
		{
			"1p_key3_laser_judge_great_or_better",
			() => LaneLaserJudge[3] >= JudgeType.Great
		},
		{
			"1p_key4_laser_judge_great_or_better",
			() => LaneLaserJudge[4] >= JudgeType.Great
		},
		{
			"1p_key5_laser_judge_great_or_better",
			() => LaneLaserJudge[5] >= JudgeType.Great
		},
		{
			"1p_key6_laser_judge_great_or_better",
			() => LaneLaserJudge[6] >= JudgeType.Great
		},
		{
			"1p_key7_laser_judge_great_or_better",
			() => LaneLaserJudge[7] >= JudgeType.Great
		},
		{
			"1p_key8_laser_judge_great_or_better",
			() => LaneLaserJudge[8] >= JudgeType.Great
		},
		{
			"1p_key9_laser_judge_great_or_better",
			() => LaneLaserJudge[9] >= JudgeType.Great
		},
		{
			"1p_scratch_laser_judge_great_or_better",
			() => LaneLaserJudge[0] >= JudgeType.Great
		},
		{
			"2p_key1_laser_judge_great_or_better",
			() => LaneLaserJudge[11] >= JudgeType.Great
		},
		{
			"2p_key2_laser_judge_great_or_better",
			() => LaneLaserJudge[12] >= JudgeType.Great
		},
		{
			"2p_key3_laser_judge_great_or_better",
			() => LaneLaserJudge[13] >= JudgeType.Great
		},
		{
			"2p_key4_laser_judge_great_or_better",
			() => LaneLaserJudge[14] >= JudgeType.Great
		},
		{
			"2p_key5_laser_judge_great_or_better",
			() => LaneLaserJudge[15] >= JudgeType.Great
		},
		{
			"2p_key6_laser_judge_great_or_better",
			() => LaneLaserJudge[16] >= JudgeType.Great
		},
		{
			"2p_key7_laser_judge_great_or_better",
			() => LaneLaserJudge[17] >= JudgeType.Great
		},
		{
			"2p_key8_laser_judge_great_or_better",
			() => LaneLaserJudge[18] >= JudgeType.Great
		},
		{
			"2p_key9_laser_judge_great_or_better",
			() => LaneLaserJudge[19] >= JudgeType.Great
		},
		{
			"2p_scratch_laser_judge_great_or_better",
			() => LaneLaserJudge[10] >= JudgeType.Great
		},
		{
			"1p_key1_laser_judge_good_or_better",
			() => LaneLaserJudge[1] >= JudgeType.Good
		},
		{
			"1p_key2_laser_judge_good_or_better",
			() => LaneLaserJudge[2] >= JudgeType.Good
		},
		{
			"1p_key3_laser_judge_good_or_better",
			() => LaneLaserJudge[3] >= JudgeType.Good
		},
		{
			"1p_key4_laser_judge_good_or_better",
			() => LaneLaserJudge[4] >= JudgeType.Good
		},
		{
			"1p_key5_laser_judge_good_or_better",
			() => LaneLaserJudge[5] >= JudgeType.Good
		},
		{
			"1p_key6_laser_judge_good_or_better",
			() => LaneLaserJudge[6] >= JudgeType.Good
		},
		{
			"1p_key7_laser_judge_good_or_better",
			() => LaneLaserJudge[7] >= JudgeType.Good
		},
		{
			"1p_key8_laser_judge_good_or_better",
			() => LaneLaserJudge[8] >= JudgeType.Good
		},
		{
			"1p_key9_laser_judge_good_or_better",
			() => LaneLaserJudge[9] >= JudgeType.Good
		},
		{
			"1p_scratch_laser_judge_good_or_better",
			() => LaneLaserJudge[0] >= JudgeType.Good
		},
		{
			"2p_key1_laser_judge_good_or_better",
			() => LaneLaserJudge[11] >= JudgeType.Good
		},
		{
			"2p_key2_laser_judge_good_or_better",
			() => LaneLaserJudge[12] >= JudgeType.Good
		},
		{
			"2p_key3_laser_judge_good_or_better",
			() => LaneLaserJudge[13] >= JudgeType.Good
		},
		{
			"2p_key4_laser_judge_good_or_better",
			() => LaneLaserJudge[14] >= JudgeType.Good
		},
		{
			"2p_key5_laser_judge_good_or_better",
			() => LaneLaserJudge[15] >= JudgeType.Good
		},
		{
			"2p_key6_laser_judge_good_or_better",
			() => LaneLaserJudge[16] >= JudgeType.Good
		},
		{
			"2p_key7_laser_judge_good_or_better",
			() => LaneLaserJudge[17] >= JudgeType.Good
		},
		{
			"2p_key8_laser_judge_good_or_better",
			() => LaneLaserJudge[18] >= JudgeType.Good
		},
		{
			"2p_key9_laser_judge_good_or_better",
			() => LaneLaserJudge[19] >= JudgeType.Good
		},
		{
			"2p_scratch_laser_judge_good_or_better",
			() => LaneLaserJudge[10] >= JudgeType.Good
		},
		{
			"1p_key1_laser_judge_bad_or_worse",
			() => LaneLaserJudge[1] <= JudgeType.Bad
		},
		{
			"1p_key2_laser_judge_bad_or_worse",
			() => LaneLaserJudge[2] <= JudgeType.Bad
		},
		{
			"1p_key3_laser_judge_bad_or_worse",
			() => LaneLaserJudge[3] <= JudgeType.Bad
		},
		{
			"1p_key4_laser_judge_bad_or_worse",
			() => LaneLaserJudge[4] <= JudgeType.Bad
		},
		{
			"1p_key5_laser_judge_bad_or_worse",
			() => LaneLaserJudge[5] <= JudgeType.Bad
		},
		{
			"1p_key6_laser_judge_bad_or_worse",
			() => LaneLaserJudge[6] <= JudgeType.Bad
		},
		{
			"1p_key7_laser_judge_bad_or_worse",
			() => LaneLaserJudge[7] <= JudgeType.Bad
		},
		{
			"1p_key8_laser_judge_bad_or_worse",
			() => LaneLaserJudge[8] <= JudgeType.Bad
		},
		{
			"1p_key9_laser_judge_bad_or_worse",
			() => LaneLaserJudge[9] <= JudgeType.Bad
		},
		{
			"1p_scratch_laser_judge_bad_or_worse",
			() => LaneLaserJudge[0] <= JudgeType.Bad
		},
		{
			"2p_key1_laser_judge_bad_or_worse",
			() => LaneLaserJudge[11] <= JudgeType.Bad
		},
		{
			"2p_key2_laser_judge_bad_or_worse",
			() => LaneLaserJudge[12] <= JudgeType.Bad
		},
		{
			"2p_key3_laser_judge_bad_or_worse",
			() => LaneLaserJudge[13] <= JudgeType.Bad
		},
		{
			"2p_key4_laser_judge_bad_or_worse",
			() => LaneLaserJudge[14] <= JudgeType.Bad
		},
		{
			"2p_key5_laser_judge_bad_or_worse",
			() => LaneLaserJudge[15] <= JudgeType.Bad
		},
		{
			"2p_key6_laser_judge_bad_or_worse",
			() => LaneLaserJudge[16] <= JudgeType.Bad
		},
		{
			"2p_key7_laser_judge_bad_or_worse",
			() => LaneLaserJudge[17] <= JudgeType.Bad
		},
		{
			"2p_key8_laser_judge_bad_or_worse",
			() => LaneLaserJudge[18] <= JudgeType.Bad
		},
		{
			"2p_key9_laser_judge_bad_or_worse",
			() => LaneLaserJudge[19] <= JudgeType.Bad
		},
		{
			"2p_scratch_laser_judge_bad_or_worse",
			() => LaneLaserJudge[10] <= JudgeType.Bad
		},
		{
			"1p_last_judged_left",
			() => Player1LastJudgedLeft
		},
		{
			"1p_last_judged_center",
			() => Player1LastJudgedCenter
		},
		{
			"1p_last_judged_right",
			() => Player1LastJudgedRight
		},
		{
			"2p_last_judged_left",
			() => Player2LastJudgedLeft
		},
		{
			"2p_last_judged_center",
			() => Player2LastJudgedCenter
		},
		{
			"2p_last_judged_right",
			() => Player2LastJudgedRight
		},
		{
			"1p_key1_last_judged",
			() => LastJudged[1]
		},
		{
			"1p_key2_last_judged",
			() => LastJudged[2]
		},
		{
			"1p_key3_last_judged",
			() => LastJudged[3]
		},
		{
			"1p_key4_last_judged",
			() => LastJudged[4]
		},
		{
			"1p_key5_last_judged",
			() => LastJudged[5]
		},
		{
			"1p_key6_last_judged",
			() => LastJudged[6]
		},
		{
			"1p_key7_last_judged",
			() => LastJudged[7]
		},
		{
			"1p_key8_last_judged",
			() => LastJudged[8]
		},
		{
			"1p_key9_last_judged",
			() => LastJudged[9]
		},
		{
			"1p_scratch_last_judged",
			() => LastJudged[0]
		},
		{
			"2p_key1_last_judged",
			() => LastJudged[11]
		},
		{
			"2p_key2_last_judged",
			() => LastJudged[12]
		},
		{
			"2p_key3_last_judged",
			() => LastJudged[13]
		},
		{
			"2p_key4_last_judged",
			() => LastJudged[14]
		},
		{
			"2p_key5_last_judged",
			() => LastJudged[15]
		},
		{
			"2p_key6_last_judged",
			() => LastJudged[16]
		},
		{
			"2p_key7_last_judged",
			() => LastJudged[17]
		},
		{
			"2p_key8_last_judged",
			() => LastJudged[18]
		},
		{
			"2p_key9_last_judged",
			() => LastJudged[19]
		},
		{
			"2p_scratch_last_judged",
			() => LastJudged[10]
		},
		{
			"1p_key1_ln_holding",
			() => LNHolding[1]
		},
		{
			"1p_key2_ln_holding",
			() => LNHolding[2]
		},
		{
			"1p_key3_ln_holding",
			() => LNHolding[3]
		},
		{
			"1p_key4_ln_holding",
			() => LNHolding[4]
		},
		{
			"1p_key5_ln_holding",
			() => LNHolding[5]
		},
		{
			"1p_key6_ln_holding",
			() => LNHolding[6]
		},
		{
			"1p_key7_ln_holding",
			() => LNHolding[7]
		},
		{
			"1p_key8_ln_holding",
			() => LNHolding[8]
		},
		{
			"1p_key9_ln_holding",
			() => LNHolding[9]
		},
		{
			"1p_scratch_ln_holding",
			() => LNHolding[0]
		},
		{
			"2p_key1_ln_holding",
			() => LNHolding[11]
		},
		{
			"2p_key2_ln_holding",
			() => LNHolding[12]
		},
		{
			"2p_key3_ln_holding",
			() => LNHolding[13]
		},
		{
			"2p_key4_ln_holding",
			() => LNHolding[14]
		},
		{
			"2p_key5_ln_holding",
			() => LNHolding[15]
		},
		{
			"2p_key6_ln_holding",
			() => LNHolding[16]
		},
		{
			"2p_key7_ln_holding",
			() => LNHolding[17]
		},
		{
			"2p_key8_ln_holding",
			() => LNHolding[18]
		},
		{
			"2p_key9_ln_holding",
			() => LNHolding[19]
		},
		{
			"2p_scratch_ln_holding",
			() => LNHolding[10]
		},
		{
			"1p_key1_ln_released",
			() => LNReleased[1]
		},
		{
			"1p_key2_ln_released",
			() => LNReleased[2]
		},
		{
			"1p_key3_ln_released",
			() => LNReleased[3]
		},
		{
			"1p_key4_ln_released",
			() => LNReleased[4]
		},
		{
			"1p_key5_ln_released",
			() => LNReleased[5]
		},
		{
			"1p_key6_ln_released",
			() => LNReleased[6]
		},
		{
			"1p_key7_ln_released",
			() => LNReleased[7]
		},
		{
			"1p_key8_ln_released",
			() => LNReleased[8]
		},
		{
			"1p_key9_ln_released",
			() => LNReleased[9]
		},
		{
			"1p_scratch_ln_released",
			() => LNReleased[0]
		},
		{
			"2p_key1_ln_released",
			() => LNReleased[11]
		},
		{
			"2p_key2_ln_released",
			() => LNReleased[12]
		},
		{
			"2p_key3_ln_released",
			() => LNReleased[13]
		},
		{
			"2p_key4_ln_released",
			() => LNReleased[14]
		},
		{
			"2p_key5_ln_released",
			() => LNReleased[15]
		},
		{
			"2p_key6_ln_released",
			() => LNReleased[16]
		},
		{
			"2p_key7_ln_released",
			() => LNReleased[17]
		},
		{
			"2p_key8_ln_released",
			() => LNReleased[18]
		},
		{
			"2p_key9_ln_released",
			() => LNReleased[19]
		},
		{
			"2p_scratch_ln_released",
			() => LNReleased[10]
		},
		{
			"1p_key1_ln_hold_once",
			() => LNHoldOnce[1]
		},
		{
			"1p_key2_ln_hold_once",
			() => LNHoldOnce[2]
		},
		{
			"1p_key3_ln_hold_once",
			() => LNHoldOnce[3]
		},
		{
			"1p_key4_ln_hold_once",
			() => LNHoldOnce[4]
		},
		{
			"1p_key5_ln_hold_once",
			() => LNHoldOnce[5]
		},
		{
			"1p_key6_ln_hold_once",
			() => LNHoldOnce[6]
		},
		{
			"1p_key7_ln_hold_once",
			() => LNHoldOnce[7]
		},
		{
			"1p_key8_ln_hold_once",
			() => LNHoldOnce[8]
		},
		{
			"1p_key9_ln_hold_once",
			() => LNHoldOnce[9]
		},
		{
			"1p_scratch_ln_hold_once",
			() => LNHoldOnce[0]
		},
		{
			"2p_key1_ln_hold_once",
			() => LNHoldOnce[11]
		},
		{
			"2p_key2_ln_hold_once",
			() => LNHoldOnce[12]
		},
		{
			"2p_key3_ln_hold_once",
			() => LNHoldOnce[13]
		},
		{
			"2p_key4_ln_hold_once",
			() => LNHoldOnce[14]
		},
		{
			"2p_key5_ln_hold_once",
			() => LNHoldOnce[15]
		},
		{
			"2p_key6_ln_hold_once",
			() => LNHoldOnce[16]
		},
		{
			"2p_key7_ln_hold_once",
			() => LNHoldOnce[17]
		},
		{
			"2p_key8_ln_hold_once",
			() => LNHoldOnce[18]
		},
		{
			"2p_key9_ln_hold_once",
			() => LNHoldOnce[19]
		},
		{
			"2p_scratch_ln_hold_once",
			() => LNHoldOnce[10]
		},
		{
			"judge_timing_fast",
			() => IsJudgeTimingFast(Numbers.LastJudgeTime)
		},
		{
			"1p_judge_timing_fast",
			() => IsJudgeTimingFast(Numbers.Player1LastJudgeTime)
		},
		{
			"1p_key1_judge_timing_fast",
			() => IsJudgeTimingFast(Numbers.LaneLastJudgeTime[1])
		},
		{
			"1p_key2_judge_timing_fast",
			() => IsJudgeTimingFast(Numbers.LaneLastJudgeTime[2])
		},
		{
			"1p_key3_judge_timing_fast",
			() => IsJudgeTimingFast(Numbers.LaneLastJudgeTime[3])
		},
		{
			"1p_key4_judge_timing_fast",
			() => IsJudgeTimingFast(Numbers.LaneLastJudgeTime[4])
		},
		{
			"1p_key5_judge_timing_fast",
			() => IsJudgeTimingFast(Numbers.LaneLastJudgeTime[5])
		},
		{
			"1p_key6_judge_timing_fast",
			() => IsJudgeTimingFast(Numbers.LaneLastJudgeTime[6])
		},
		{
			"1p_key7_judge_timing_fast",
			() => IsJudgeTimingFast(Numbers.LaneLastJudgeTime[7])
		},
		{
			"1p_key8_judge_timing_fast",
			() => IsJudgeTimingFast(Numbers.LaneLastJudgeTime[8])
		},
		{
			"1p_key9_judge_timing_fast",
			() => IsJudgeTimingFast(Numbers.LaneLastJudgeTime[9])
		},
		{
			"1p_scratch_judge_timing_fast",
			() => IsJudgeTimingFast(Numbers.LaneLastJudgeTime[0])
		},
		{
			"1p_judge_timing_left_fast",
			() => IsJudgeTimingFast(Numbers.Player1LastJudgeTimeLeft)
		},
		{
			"1p_judge_timing_center_fast",
			() => IsJudgeTimingFast(Numbers.Player1LastJudgeTimeCenter)
		},
		{
			"1p_judge_timing_right_fast",
			() => IsJudgeTimingFast(Numbers.Player1LastJudgeTimeRight)
		},
		{
			"2p_judge_timing_fast",
			() => IsJudgeTimingFast(Numbers.Player2LastJudgeTime)
		},
		{
			"2p_key1_judge_timing_fast",
			() => IsJudgeTimingFast(Numbers.LaneLastJudgeTime[11])
		},
		{
			"2p_key2_judge_timing_fast",
			() => IsJudgeTimingFast(Numbers.LaneLastJudgeTime[12])
		},
		{
			"2p_key3_judge_timing_fast",
			() => IsJudgeTimingFast(Numbers.LaneLastJudgeTime[13])
		},
		{
			"2p_key4_judge_timing_fast",
			() => IsJudgeTimingFast(Numbers.LaneLastJudgeTime[14])
		},
		{
			"2p_key5_judge_timing_fast",
			() => IsJudgeTimingFast(Numbers.LaneLastJudgeTime[15])
		},
		{
			"2p_key6_judge_timing_fast",
			() => IsJudgeTimingFast(Numbers.LaneLastJudgeTime[16])
		},
		{
			"2p_key7_judge_timing_fast",
			() => IsJudgeTimingFast(Numbers.LaneLastJudgeTime[17])
		},
		{
			"2p_key8_judge_timing_fast",
			() => IsJudgeTimingFast(Numbers.LaneLastJudgeTime[18])
		},
		{
			"2p_key9_judge_timing_fast",
			() => IsJudgeTimingFast(Numbers.LaneLastJudgeTime[19])
		},
		{
			"2p_scratch_judge_timing_fast",
			() => IsJudgeTimingFast(Numbers.LaneLastJudgeTime[10])
		},
		{
			"2p_judge_timing_left_fast",
			() => IsJudgeTimingFast(Numbers.Player1LastJudgeTimeLeft)
		},
		{
			"2p_judge_timing_center_fast",
			() => IsJudgeTimingFast(Numbers.Player1LastJudgeTimeCenter)
		},
		{
			"2p_judge_timing_right_fast",
			() => IsJudgeTimingFast(Numbers.Player1LastJudgeTimeRight)
		},
		{
			"judge_timing_slow",
			() => IsJudgeTimingSlow(Numbers.LastJudgeTime)
		},
		{
			"1p_judge_timing_slow",
			() => IsJudgeTimingSlow(Numbers.Player1LastJudgeTime)
		},
		{
			"1p_key1_judge_timing_slow",
			() => IsJudgeTimingSlow(Numbers.LaneLastJudgeTime[1])
		},
		{
			"1p_key2_judge_timing_slow",
			() => IsJudgeTimingSlow(Numbers.LaneLastJudgeTime[2])
		},
		{
			"1p_key3_judge_timing_slow",
			() => IsJudgeTimingSlow(Numbers.LaneLastJudgeTime[3])
		},
		{
			"1p_key4_judge_timing_slow",
			() => IsJudgeTimingSlow(Numbers.LaneLastJudgeTime[4])
		},
		{
			"1p_key5_judge_timing_slow",
			() => IsJudgeTimingSlow(Numbers.LaneLastJudgeTime[5])
		},
		{
			"1p_key6_judge_timing_slow",
			() => IsJudgeTimingSlow(Numbers.LaneLastJudgeTime[6])
		},
		{
			"1p_key7_judge_timing_slow",
			() => IsJudgeTimingSlow(Numbers.LaneLastJudgeTime[7])
		},
		{
			"1p_key8_judge_timing_slow",
			() => IsJudgeTimingSlow(Numbers.LaneLastJudgeTime[8])
		},
		{
			"1p_key9_judge_timing_slow",
			() => IsJudgeTimingSlow(Numbers.LaneLastJudgeTime[9])
		},
		{
			"1p_scratch_judge_timing_slow",
			() => IsJudgeTimingSlow(Numbers.LaneLastJudgeTime[0])
		},
		{
			"1p_judge_timing_left_slow",
			() => IsJudgeTimingSlow(Numbers.Player1LastJudgeTimeLeft)
		},
		{
			"1p_judge_timing_center_slow",
			() => IsJudgeTimingSlow(Numbers.Player1LastJudgeTimeCenter)
		},
		{
			"1p_judge_timing_right_slow",
			() => IsJudgeTimingSlow(Numbers.Player1LastJudgeTimeRight)
		},
		{
			"2p_judge_timing_slow",
			() => IsJudgeTimingSlow(Numbers.Player2LastJudgeTime)
		},
		{
			"2p_key1_judge_timing_slow",
			() => IsJudgeTimingSlow(Numbers.LaneLastJudgeTime[11])
		},
		{
			"2p_key2_judge_timing_slow",
			() => IsJudgeTimingSlow(Numbers.LaneLastJudgeTime[12])
		},
		{
			"2p_key3_judge_timing_slow",
			() => IsJudgeTimingSlow(Numbers.LaneLastJudgeTime[13])
		},
		{
			"2p_key4_judge_timing_slow",
			() => IsJudgeTimingSlow(Numbers.LaneLastJudgeTime[14])
		},
		{
			"2p_key5_judge_timing_slow",
			() => IsJudgeTimingSlow(Numbers.LaneLastJudgeTime[15])
		},
		{
			"2p_key6_judge_timing_slow",
			() => IsJudgeTimingSlow(Numbers.LaneLastJudgeTime[16])
		},
		{
			"2p_key7_judge_timing_slow",
			() => IsJudgeTimingSlow(Numbers.LaneLastJudgeTime[17])
		},
		{
			"2p_key8_judge_timing_slow",
			() => IsJudgeTimingSlow(Numbers.LaneLastJudgeTime[18])
		},
		{
			"2p_key9_judge_timing_slow",
			() => IsJudgeTimingSlow(Numbers.LaneLastJudgeTime[19])
		},
		{
			"2p_scratch_judge_timing_slow",
			() => IsJudgeTimingSlow(Numbers.LaneLastJudgeTime[10])
		},
		{
			"2p_judge_timing_left_slow",
			() => IsJudgeTimingSlow(Numbers.Player1LastJudgeTimeLeft)
		},
		{
			"2p_judge_timing_center_slow",
			() => IsJudgeTimingSlow(Numbers.Player1LastJudgeTimeCenter)
		},
		{
			"2p_judge_timing_right_slow",
			() => IsJudgeTimingSlow(Numbers.Player1LastJudgeTimeRight)
		},
		{
			"1p_ran_mir_off",
			() => Player1LaneSwapOption == LaneSwapOption.None
		},
		{
			"1p_ran_mir_random",
			() => Player1LaneSwapOption == LaneSwapOption.Random
		},
		{
			"1p_ran_mir_mirror",
			() => Player1LaneSwapOption == LaneSwapOption.Mirror
		},
		{
			"1p_ran_mir_rotate",
			() => Player1LaneSwapOption == LaneSwapOption.Rotate
		},
		{
			"1p_ran_mir_cross",
			() => Player1LaneSwapOption == LaneSwapOption.Cross
		},
		{
			"1p_ran_mir_xrotate",
			() => Player1LaneSwapOption == LaneSwapOption.CrossRotate
		},
		{
			"1p_ran_mir_split",
			() => Player1LaneSwapOption == LaneSwapOption.Split
		},
		{
			"1p_ran_mir_oddeven",
			() => Player1LaneSwapOption == LaneSwapOption.OddEven
		},
		{
			"1p_ran_mir_spread",
			() => Player1LaneSwapOption == LaneSwapOption.Spread
		},
		{
			"1p_ran_mir_srandom",
			() => Player1LaneSwapOption == LaneSwapOption.SRandom
		},
		{
			"1p_ran_mir_hrandom",
			() => Player1LaneSwapOption == LaneSwapOption.HRandom
		},
		{
			"1p_ran_mir_srandom_n",
			() => Player1LaneSwapOption == LaneSwapOption.NonCorrectedSRandom
		},
		{
			"1p_ran_mir_srandom_p",
			() => Player1LaneSwapOption == LaneSwapOption.PeriodicSRandom
		},
		{
			"1p_ran_mir_allscr",
			() => Player1LaneSwapOption == LaneSwapOption.AllScratch
		},
		{
			"2p_ran_mir_off",
			() => Player2LaneSwapOption == LaneSwapOption.None
		},
		{
			"2p_ran_mir_random",
			() => Player2LaneSwapOption == LaneSwapOption.Random
		},
		{
			"2p_ran_mir_mirror",
			() => Player2LaneSwapOption == LaneSwapOption.Mirror
		},
		{
			"2p_ran_mir_rotate",
			() => Player2LaneSwapOption == LaneSwapOption.Rotate
		},
		{
			"2p_ran_mir_cross",
			() => Player2LaneSwapOption == LaneSwapOption.Cross
		},
		{
			"2p_ran_mir_xrotate",
			() => Player2LaneSwapOption == LaneSwapOption.CrossRotate
		},
		{
			"2p_ran_mir_split",
			() => Player2LaneSwapOption == LaneSwapOption.Split
		},
		{
			"2p_ran_mir_oddeven",
			() => Player2LaneSwapOption == LaneSwapOption.OddEven
		},
		{
			"2p_ran_mir_spread",
			() => Player2LaneSwapOption == LaneSwapOption.Spread
		},
		{
			"2p_ran_mir_srandom",
			() => Player2LaneSwapOption == LaneSwapOption.SRandom
		},
		{
			"2p_ran_mir_hrandom",
			() => Player2LaneSwapOption == LaneSwapOption.HRandom
		},
		{
			"2p_ran_mir_srandom_n",
			() => Player2LaneSwapOption == LaneSwapOption.NonCorrectedSRandom
		},
		{
			"2p_ran_mir_srandom_p",
			() => Player2LaneSwapOption == LaneSwapOption.PeriodicSRandom
		},
		{
			"2p_ran_mir_allscr",
			() => Player2LaneSwapOption == LaneSwapOption.AllScratch
		},
		{
			"side_flip",
			() => FlipSide
		},
		{
			"playstyle_5",
			() => IsPlayStyle5
		},
		{
			"playstyle_7",
			() => IsPlayStyle7
		},
		{
			"playstyle_9",
			() => IsPlayStyle9
		},
		{
			"playstyle_10",
			() => IsPlayStyle10
		},
		{
			"playstyle_14",
			() => IsPlayStyle14
		},
		{
			"bpm_variable",
			() => IsBpmVariable
		},
		{
			"gauge_100",
			() => IsGaugeMax
		},
		{
			"gauge_90",
			() => IsNumberBetween(Numbers.GrooveGauge, 100.0, 90.0)
		},
		{
			"gauge_80",
			() => IsNumberBetween(Numbers.GrooveGauge, 90.0, 80.0)
		},
		{
			"gauge_70",
			() => IsNumberBetween(Numbers.GrooveGauge, 80.0, 70.0)
		},
		{
			"gauge_60",
			() => IsNumberBetween(Numbers.GrooveGauge, 70.0, 60.0)
		},
		{
			"gauge_50",
			() => IsNumberBetween(Numbers.GrooveGauge, 60.0, 50.0)
		},
		{
			"gauge_40",
			() => IsNumberBetween(Numbers.GrooveGauge, 50.0, 40.0)
		},
		{
			"gauge_30",
			() => IsNumberBetween(Numbers.GrooveGauge, 40.0, 30.0)
		},
		{
			"gauge_20",
			() => IsNumberBetween(Numbers.GrooveGauge, 30.0, 20.0)
		},
		{
			"gauge_10",
			() => IsNumberBetween(Numbers.GrooveGauge, 20.0, 10.0)
		},
		{
			"gauge_0",
			() => IsGaugeUnder9
		},
		{
			"gauge_empty",
			() => IsGaugeZero
		},
		{
			"gauge_over_border",
			() => IsGaugeOverBorder
		},
		{
			"hs_fix_off",
			() => HiSpeedFixType == HiSpeedFixType.None
		},
		{
			"hs_fix_max",
			() => HiSpeedFixType == HiSpeedFixType.Max
		},
		{
			"hs_fix_min",
			() => HiSpeedFixType == HiSpeedFixType.Min
		},
		{
			"hs_fix_main_notes",
			() => HiSpeedFixType == HiSpeedFixType.MainNotes
		},
		{
			"hs_fix_main_time",
			() => HiSpeedFixType == HiSpeedFixType.MainTime
		},
		{
			"hs_fix_regular_speed",
			() => HiSpeedFixType == HiSpeedFixType.RegularSpeed
		},
		{
			"judge_rank_defined",
			() => IsJudgeRankDefined
		},
		{
			"judge_rank_easy",
			() => JudgeRank == JudgeRank.Easy
		},
		{
			"judge_rank_normal",
			() => JudgeRank == JudgeRank.Normal
		},
		{
			"judge_rank_hard",
			() => JudgeRank == JudgeRank.Hard
		},
		{
			"judge_rank_veryhard",
			() => JudgeRank == JudgeRank.VeryHard
		},
		{
			"difficulty_defined",
			() => IsDifficultyDefined && Numbers.Difficulty >= 1 && Numbers.Difficulty <= 5
		},
		{
			"difficulty_1",
			() => CheckDifficulty(1)
		},
		{
			"difficulty_2",
			() => CheckDifficulty(2)
		},
		{
			"difficulty_3",
			() => CheckDifficulty(3)
		},
		{
			"difficulty_4",
			() => CheckDifficulty(4)
		},
		{
			"difficulty_5",
			() => CheckDifficulty(5)
		},
		{
			"playside_left",
			() => PlaySide == PlaySide.Left || PlaySide == PlaySide.Both
		},
		{
			"playside_right",
			() => PlaySide == PlaySide.Right || PlaySide == PlaySide.Both
		},
		{
			"contains_ln",
			() => ContainsLN
		},
		{
			"volume_master_100",
			() => IsMasterVolumeMax
		},
		{
			"volume_master_90",
			() => IsNumberBetween(Numbers.MasterVolume, 100, 90)
		},
		{
			"volume_master_80",
			() => IsNumberBetween(Numbers.MasterVolume, 90, 80)
		},
		{
			"volume_master_70",
			() => IsNumberBetween(Numbers.MasterVolume, 80, 70)
		},
		{
			"volume_master_60",
			() => IsNumberBetween(Numbers.MasterVolume, 70, 60)
		},
		{
			"volume_master_50",
			() => IsNumberBetween(Numbers.MasterVolume, 60, 50)
		},
		{
			"volume_master_40",
			() => IsNumberBetween(Numbers.MasterVolume, 50, 40)
		},
		{
			"volume_master_30",
			() => IsNumberBetween(Numbers.MasterVolume, 40, 30)
		},
		{
			"volume_master_20",
			() => IsNumberBetween(Numbers.MasterVolume, 30, 20)
		},
		{
			"volume_master_10",
			() => IsNumberBetween(Numbers.MasterVolume, 20, 10)
		},
		{
			"volume_master_1",
			() => IsMasterVolumeUnder9
		},
		{
			"volume_master_0",
			() => IsMasterVolumeZero
		},
		{
			"volume_master_mute",
			() => IsMasterVolumeMute
		},
		{
			"volume_key_100",
			() => IsKeyVolumeMax
		},
		{
			"volume_key_90",
			() => IsNumberBetween(Numbers.KeyVolume, 100, 90)
		},
		{
			"volume_key_80",
			() => IsNumberBetween(Numbers.KeyVolume, 90, 80)
		},
		{
			"volume_key_70",
			() => IsNumberBetween(Numbers.KeyVolume, 80, 70)
		},
		{
			"volume_key_60",
			() => IsNumberBetween(Numbers.KeyVolume, 70, 60)
		},
		{
			"volume_key_50",
			() => IsNumberBetween(Numbers.KeyVolume, 60, 50)
		},
		{
			"volume_key_40",
			() => IsNumberBetween(Numbers.KeyVolume, 50, 40)
		},
		{
			"volume_key_30",
			() => IsNumberBetween(Numbers.KeyVolume, 40, 30)
		},
		{
			"volume_key_20",
			() => IsNumberBetween(Numbers.KeyVolume, 30, 20)
		},
		{
			"volume_key_10",
			() => IsNumberBetween(Numbers.KeyVolume, 20, 10)
		},
		{
			"volume_key_1",
			() => IsKeyVolumeUnder9
		},
		{
			"volume_key_0",
			() => IsKeyVolumeZero
		},
		{
			"volume_key_mute",
			() => IsKeyVolumeMute
		},
		{
			"volume_bgm_100",
			() => IsBgmVolumeMax
		},
		{
			"volume_bgm_90",
			() => IsNumberBetween(Numbers.BgmVolume, 100, 90)
		},
		{
			"volume_bgm_80",
			() => IsNumberBetween(Numbers.BgmVolume, 90, 80)
		},
		{
			"volume_bgm_70",
			() => IsNumberBetween(Numbers.BgmVolume, 80, 70)
		},
		{
			"volume_bgm_60",
			() => IsNumberBetween(Numbers.BgmVolume, 70, 60)
		},
		{
			"volume_bgm_50",
			() => IsNumberBetween(Numbers.BgmVolume, 60, 50)
		},
		{
			"volume_bgm_40",
			() => IsNumberBetween(Numbers.BgmVolume, 50, 40)
		},
		{
			"volume_bgm_30",
			() => IsNumberBetween(Numbers.BgmVolume, 40, 30)
		},
		{
			"volume_bgm_20",
			() => IsNumberBetween(Numbers.BgmVolume, 30, 20)
		},
		{
			"volume_bgm_10",
			() => IsNumberBetween(Numbers.BgmVolume, 20, 10)
		},
		{
			"volume_bgm_1",
			() => IsBgmVolumeUnder9
		},
		{
			"volume_bgm_0",
			() => IsBgmVolumeZero
		},
		{
			"volume_bgm_mute",
			() => IsBgmVolumeMute
		},
		{
			"enable_open_file_dialog",
			() => EnableOpenFileDialog
		}
	});

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

	public static bool GetValue(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return false;
		}
		if (FlagDictionary.ContainsKey(name))
		{
			return FlagDictionary[name]();
		}
		return false;
	}

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

	public static bool IsKeyDown(PlayingKeys key)
	{
		if (!AutoPlay)
		{
			return PlayingKeyState.IsKeyDown(key);
		}
		return AutoPlayKeyState.IsKeyDown(key);
	}

	public static bool IsKeyDownAll(params PlayingKeys[] keys)
	{
		if (keys == null)
		{
			return false;
		}
		for (int i = 0; i < keys.Length; i++)
		{
			if (!IsKeyDown(keys[i]))
			{
				return false;
			}
		}
		return true;
	}

	public static bool IsKeyDownAny(params PlayingKeys[] keys)
	{
		if (keys == null)
		{
			return false;
		}
		for (int i = 0; i < keys.Length; i++)
		{
			if (IsKeyDown(keys[i]))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsKeyDownOnce(PlayingKeys key)
	{
		if (!AutoPlay)
		{
			return PlayingKeyState.IsKeyDownOnce(key);
		}
		return AutoPlayKeyState.IsKeyDownOnce(key);
	}

	public static bool IsKeyDownOnceAny(params PlayingKeys[] keys)
	{
		if (keys == null)
		{
			return false;
		}
		for (int i = 0; i < keys.Length; i++)
		{
			if (IsKeyDownOnce(keys[i]))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsKeyUp(PlayingKeys key)
	{
		if (!AutoPlay)
		{
			return PlayingKeyState.IsKeyUp(key);
		}
		return AutoPlayKeyState.IsKeyUp(key);
	}

	public static bool IsKeyUpAll(params PlayingKeys[] keys)
	{
		if (keys == null)
		{
			return false;
		}
		for (int i = 0; i < keys.Length; i++)
		{
			if (!IsKeyUp(keys[i]))
			{
				return false;
			}
		}
		return true;
	}

	public static bool IsKeyUpAny(params PlayingKeys[] keys)
	{
		if (keys == null)
		{
			return false;
		}
		for (int i = 0; i < keys.Length; i++)
		{
			if (IsKeyUp(keys[i]))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsKeyUpOnce(PlayingKeys key)
	{
		if (!AutoPlay)
		{
			return PlayingKeyState.IsKeyUpOnce(key);
		}
		return AutoPlayKeyState.IsKeyUpOnce(key);
	}

	public static bool IsKeyUpOnceAny(params PlayingKeys[] keys)
	{
		if (keys == null)
		{
			return false;
		}
		for (int i = 0; i < keys.Length; i++)
		{
			if (IsKeyUpOnce(keys[i]))
			{
				return true;
			}
		}
		return false;
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
