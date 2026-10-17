using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using MBMPlay.Common;
using MistUtils;

namespace MBMPlay;

public static class EventTimers
{
	private static double prevTime = 0.0;

	private static int lnCount1P = 0;

	private static int lnCount2P = 0;

	private static int lnCount1PLeft = 0;

	private static int lnCount1PCenter = 0;

	private static int lnCount1PRight = 0;

	private static int lnCount2PLeft = 0;

	private static int lnCount2PCenter = 0;

	private static int lnCount2PRight = 0;

	private static double bpmTimer = -1.0;

	private static readonly ReadOnlyDictionary<string, Func<double>> EventTimerDictionary = Extensions.AsReadOnly(new Dictionary<string, Func<double>>
	{
		{ "scene_start", () => SceneStart },
		{ "load_start", () => LoadStart },
		{ "load_complete", () => LoadComplete },
		{ "play_start", () => PlayStart },
		{ "bpm", () => BpmTimer },
		{ "groove_gauge_increase", () => GrooveGaugeIncrease },
		{ "groove_gauge_max", () => GrooveGaugeMax },
		{ "bga_poor", () => PoorBga },
		{ "1p_lanecover_changed", () => Player1LaneCoverChanged },
		{ "2p_lanecover_changed", () => Player2LaneCoverChanged },
		{ "lanecover_changed", () => Math.Max(Player1LaneCoverChanged, Player2LaneCoverChanged) },
		{ "1p_lift_changed", () => Player1LiftChanged },
		{ "2p_lift_changed", () => Player2LiftChanged },
		{ "lift_changed", () => Math.Max(Player1LiftChanged, Player2LiftChanged) },
		{ "nps_max_update", () => MaxNotesPerSecondUpdate },
		{ "1p_key1_on", () => KeyDown[PlayingKeys.Player1Key1] },
		{ "1p_key2_on", () => KeyDown[PlayingKeys.Player1Key2] },
		{ "1p_key3_on", () => KeyDown[PlayingKeys.Player1Key3] },
		{ "1p_key4_on", () => KeyDown[PlayingKeys.Player1Key4] },
		{ "1p_key5_on", () => KeyDown[PlayingKeys.Player1Key5] },
		{ "1p_key6_on", () => KeyDown[PlayingKeys.Player1Key6] },
		{ "1p_key7_on", () => KeyDown[PlayingKeys.Player1Key7] },
		{ "1p_key8_on", () => KeyDown[PlayingKeys.Player1Key8] },
		{ "1p_key9_on", () => KeyDown[PlayingKeys.Player1Key9] },
		{ "1p_scratch_on", () => Player1ScratchDown },
		{ "2p_key1_on", () => KeyDown[PlayingKeys.Player2Key1] },
		{ "2p_key2_on", () => KeyDown[PlayingKeys.Player2Key2] },
		{ "2p_key3_on", () => KeyDown[PlayingKeys.Player2Key3] },
		{ "2p_key4_on", () => KeyDown[PlayingKeys.Player2Key4] },
		{ "2p_key5_on", () => KeyDown[PlayingKeys.Player2Key5] },
		{ "2p_key6_on", () => KeyDown[PlayingKeys.Player2Key6] },
		{ "2p_key7_on", () => KeyDown[PlayingKeys.Player2Key7] },
		{ "2p_key8_on", () => KeyDown[PlayingKeys.Player2Key8] },
		{ "2p_key9_on", () => KeyDown[PlayingKeys.Player2Key9] },
		{ "2p_scratch_on", () => Player2ScratchDown },
		{ "1p_key1_on_alt", () => KeyDownAlt[PlayingKeys.Player1Key1] },
		{ "1p_key2_on_alt", () => KeyDownAlt[PlayingKeys.Player1Key2] },
		{ "1p_key3_on_alt", () => KeyDownAlt[PlayingKeys.Player1Key3] },
		{ "1p_key4_on_alt", () => KeyDownAlt[PlayingKeys.Player1Key4] },
		{ "1p_key5_on_alt", () => KeyDownAlt[PlayingKeys.Player1Key5] },
		{ "1p_key6_on_alt", () => KeyDownAlt[PlayingKeys.Player1Key6] },
		{ "1p_key7_on_alt", () => KeyDownAlt[PlayingKeys.Player1Key7] },
		{ "1p_key8_on_alt", () => KeyDownAlt[PlayingKeys.Player1Key8] },
		{ "1p_key9_on_alt", () => KeyDownAlt[PlayingKeys.Player1Key9] },
		{ "1p_scratch_on_alt", () => Player1ScratchDownAlt },
		{ "2p_key1_on_alt", () => KeyDownAlt[PlayingKeys.Player2Key1] },
		{ "2p_key2_on_alt", () => KeyDownAlt[PlayingKeys.Player2Key2] },
		{ "2p_key3_on_alt", () => KeyDownAlt[PlayingKeys.Player2Key3] },
		{ "2p_key4_on_alt", () => KeyDownAlt[PlayingKeys.Player2Key4] },
		{ "2p_key5_on_alt", () => KeyDownAlt[PlayingKeys.Player2Key5] },
		{ "2p_key6_on_alt", () => KeyDownAlt[PlayingKeys.Player2Key6] },
		{ "2p_key7_on_alt", () => KeyDownAlt[PlayingKeys.Player2Key7] },
		{ "2p_key8_on_alt", () => KeyDownAlt[PlayingKeys.Player2Key8] },
		{ "2p_key9_on_alt", () => KeyDownAlt[PlayingKeys.Player2Key9] },
		{ "2p_scratch_on_alt", () => Player2ScratchDownAlt },
		{ "1p_key1_off", () => KeyUp[PlayingKeys.Player1Key1] },
		{ "1p_key2_off", () => KeyUp[PlayingKeys.Player1Key2] },
		{ "1p_key3_off", () => KeyUp[PlayingKeys.Player1Key3] },
		{ "1p_key4_off", () => KeyUp[PlayingKeys.Player1Key4] },
		{ "1p_key5_off", () => KeyUp[PlayingKeys.Player1Key5] },
		{ "1p_key6_off", () => KeyUp[PlayingKeys.Player1Key6] },
		{ "1p_key7_off", () => KeyUp[PlayingKeys.Player1Key7] },
		{ "1p_key8_off", () => KeyUp[PlayingKeys.Player1Key8] },
		{ "1p_key9_off", () => KeyUp[PlayingKeys.Player1Key9] },
		{ "1p_scratch_off", () => Player1ScratchUp },
		{ "2p_key1_off", () => KeyUp[PlayingKeys.Player2Key1] },
		{ "2p_key2_off", () => KeyUp[PlayingKeys.Player2Key2] },
		{ "2p_key3_off", () => KeyUp[PlayingKeys.Player2Key3] },
		{ "2p_key4_off", () => KeyUp[PlayingKeys.Player2Key4] },
		{ "2p_key5_off", () => KeyUp[PlayingKeys.Player2Key5] },
		{ "2p_key6_off", () => KeyUp[PlayingKeys.Player2Key6] },
		{ "2p_key7_off", () => KeyUp[PlayingKeys.Player2Key7] },
		{ "2p_key8_off", () => KeyUp[PlayingKeys.Player2Key8] },
		{ "2p_key9_off", () => KeyUp[PlayingKeys.Player2Key9] },
		{ "2p_scratch_off", () => Player2ScratchUp },
		{ "1p_scratch_rotate", () => Player1ScratchRotate.Value },
		{ "2p_scratch_rotate", () => Player2ScratchRotate.Value },
		{ "1p_judge", () => Player1Judge },
		{ "1p_judge_left", () => Player1JudgeLeft },
		{ "1p_judge_center", () => Player1JudgeCenter },
		{ "1p_judge_right", () => Player1JudgeRight },
		{ "2p_judge", () => Player2Judge },
		{ "2p_judge_left", () => Player2JudgeLeft },
		{ "2p_judge_center", () => Player2JudgeCenter },
		{ "2p_judge_right", () => Player2JudgeRight },
		{ "1p_ln_judge", () => Player1LNJudge },
		{ "1p_ln_judge_left", () => Player1LNJudgeLeft },
		{ "1p_ln_judge_center", () => Player1LNJudgeCenter },
		{ "1p_ln_judge_right", () => Player1LNJudgeRight },
		{ "2p_ln_judge", () => Player2LNJudge },
		{ "2p_ln_judge_left", () => Player2LNJudgeLeft },
		{ "2p_ln_judge_center", () => Player2LNJudgeCenter },
		{ "2p_ln_judge_right", () => Player2LNJudgeRight },
		{ "1p_key1_judge", () => LaneJudge[1] },
		{ "1p_key2_judge", () => LaneJudge[2] },
		{ "1p_key3_judge", () => LaneJudge[3] },
		{ "1p_key4_judge", () => LaneJudge[4] },
		{ "1p_key5_judge", () => LaneJudge[5] },
		{ "1p_key6_judge", () => LaneJudge[6] },
		{ "1p_key7_judge", () => LaneJudge[7] },
		{ "1p_key8_judge", () => LaneJudge[8] },
		{ "1p_key9_judge", () => LaneJudge[9] },
		{ "1p_scratch_judge", () => LaneJudge[0] },
		{ "2p_key1_judge", () => LaneJudge[11] },
		{ "2p_key2_judge", () => LaneJudge[12] },
		{ "2p_key3_judge", () => LaneJudge[13] },
		{ "2p_key4_judge", () => LaneJudge[14] },
		{ "2p_key5_judge", () => LaneJudge[15] },
		{ "2p_key6_judge", () => LaneJudge[16] },
		{ "2p_key7_judge", () => LaneJudge[17] },
		{ "2p_key8_judge", () => LaneJudge[18] },
		{ "2p_key9_judge", () => LaneJudge[19] },
		{ "2p_scratch_judge", () => LaneJudge[10] },
		{ "1p_key1_ln_judge", () => LaneLNJudge[1] },
		{ "1p_key2_ln_judge", () => LaneLNJudge[2] },
		{ "1p_key3_ln_judge", () => LaneLNJudge[3] },
		{ "1p_key4_ln_judge", () => LaneLNJudge[4] },
		{ "1p_key5_ln_judge", () => LaneLNJudge[5] },
		{ "1p_key6_ln_judge", () => LaneLNJudge[6] },
		{ "1p_key7_ln_judge", () => LaneLNJudge[7] },
		{ "1p_key8_ln_judge", () => LaneLNJudge[8] },
		{ "1p_key9_ln_judge", () => LaneLNJudge[9] },
		{ "1p_scratch_ln_judge", () => LaneLNJudge[0] },
		{ "2p_key1_ln_judge", () => LaneLNJudge[11] },
		{ "2p_key2_ln_judge", () => LaneLNJudge[12] },
		{ "2p_key3_ln_judge", () => LaneLNJudge[13] },
		{ "2p_key4_ln_judge", () => LaneLNJudge[14] },
		{ "2p_key5_ln_judge", () => LaneLNJudge[15] },
		{ "2p_key6_ln_judge", () => LaneLNJudge[16] },
		{ "2p_key7_ln_judge", () => LaneLNJudge[17] },
		{ "2p_key8_ln_judge", () => LaneLNJudge[18] },
		{ "2p_key9_ln_judge", () => LaneLNJudge[19] },
		{ "2p_scratch_ln_judge", () => LaneLNJudge[10] },
		{ "1p_key1_ln_holding", () => LaneLNHolding[1] },
		{ "1p_key2_ln_holding", () => LaneLNHolding[2] },
		{ "1p_key3_ln_holding", () => LaneLNHolding[3] },
		{ "1p_key4_ln_holding", () => LaneLNHolding[4] },
		{ "1p_key5_ln_holding", () => LaneLNHolding[5] },
		{ "1p_key6_ln_holding", () => LaneLNHolding[6] },
		{ "1p_key7_ln_holding", () => LaneLNHolding[7] },
		{ "1p_key8_ln_holding", () => LaneLNHolding[8] },
		{ "1p_key9_ln_holding", () => LaneLNHolding[9] },
		{ "1p_scratch_ln_holding", () => LaneLNHolding[0] },
		{ "2p_key1_ln_holding", () => LaneLNHolding[11] },
		{ "2p_key2_ln_holding", () => LaneLNHolding[12] },
		{ "2p_key3_ln_holding", () => LaneLNHolding[13] },
		{ "2p_key4_ln_holding", () => LaneLNHolding[14] },
		{ "2p_key5_ln_holding", () => LaneLNHolding[15] },
		{ "2p_key6_ln_holding", () => LaneLNHolding[16] },
		{ "2p_key7_ln_holding", () => LaneLNHolding[17] },
		{ "2p_key8_ln_holding", () => LaneLNHolding[18] },
		{ "2p_key9_ln_holding", () => LaneLNHolding[19] },
		{ "2p_scratch_ln_holding", () => LaneLNHolding[10] },
		{ "1p_key1_bomb_pgreat", () => Bomb[JudgeType.PGreat][1] },
		{ "1p_key2_bomb_pgreat", () => Bomb[JudgeType.PGreat][2] },
		{ "1p_key3_bomb_pgreat", () => Bomb[JudgeType.PGreat][3] },
		{ "1p_key4_bomb_pgreat", () => Bomb[JudgeType.PGreat][4] },
		{ "1p_key5_bomb_pgreat", () => Bomb[JudgeType.PGreat][5] },
		{ "1p_key6_bomb_pgreat", () => Bomb[JudgeType.PGreat][6] },
		{ "1p_key7_bomb_pgreat", () => Bomb[JudgeType.PGreat][7] },
		{ "1p_key8_bomb_pgreat", () => Bomb[JudgeType.PGreat][8] },
		{ "1p_key9_bomb_pgreat", () => Bomb[JudgeType.PGreat][9] },
		{ "1p_scratch_bomb_pgreat", () => Bomb[JudgeType.PGreat][0] },
		{ "2p_key1_bomb_pgreat", () => Bomb[JudgeType.PGreat][11] },
		{ "2p_key2_bomb_pgreat", () => Bomb[JudgeType.PGreat][12] },
		{ "2p_key3_bomb_pgreat", () => Bomb[JudgeType.PGreat][13] },
		{ "2p_key4_bomb_pgreat", () => Bomb[JudgeType.PGreat][14] },
		{ "2p_key5_bomb_pgreat", () => Bomb[JudgeType.PGreat][15] },
		{ "2p_key6_bomb_pgreat", () => Bomb[JudgeType.PGreat][16] },
		{ "2p_key7_bomb_pgreat", () => Bomb[JudgeType.PGreat][17] },
		{ "2p_key8_bomb_pgreat", () => Bomb[JudgeType.PGreat][18] },
		{ "2p_key9_bomb_pgreat", () => Bomb[JudgeType.PGreat][19] },
		{ "2p_scratch_bomb_pgreat", () => Bomb[JudgeType.PGreat][10] },
		{ "1p_key1_bomb_great", () => Bomb[JudgeType.Great][1] },
		{ "1p_key2_bomb_great", () => Bomb[JudgeType.Great][2] },
		{ "1p_key3_bomb_great", () => Bomb[JudgeType.Great][3] },
		{ "1p_key4_bomb_great", () => Bomb[JudgeType.Great][4] },
		{ "1p_key5_bomb_great", () => Bomb[JudgeType.Great][5] },
		{ "1p_key6_bomb_great", () => Bomb[JudgeType.Great][6] },
		{ "1p_key7_bomb_great", () => Bomb[JudgeType.Great][7] },
		{ "1p_key8_bomb_great", () => Bomb[JudgeType.Great][8] },
		{ "1p_key9_bomb_great", () => Bomb[JudgeType.Great][9] },
		{ "1p_scratch_bomb_great", () => Bomb[JudgeType.Great][0] },
		{ "2p_key1_bomb_great", () => Bomb[JudgeType.Great][11] },
		{ "2p_key2_bomb_great", () => Bomb[JudgeType.Great][12] },
		{ "2p_key3_bomb_great", () => Bomb[JudgeType.Great][13] },
		{ "2p_key4_bomb_great", () => Bomb[JudgeType.Great][14] },
		{ "2p_key5_bomb_great", () => Bomb[JudgeType.Great][15] },
		{ "2p_key6_bomb_great", () => Bomb[JudgeType.Great][16] },
		{ "2p_key7_bomb_great", () => Bomb[JudgeType.Great][17] },
		{ "2p_key8_bomb_great", () => Bomb[JudgeType.Great][18] },
		{ "2p_key9_bomb_great", () => Bomb[JudgeType.Great][19] },
		{ "2p_scratch_bomb_great", () => Bomb[JudgeType.Great][10] },
		{ "1p_key1_bomb_good", () => Bomb[JudgeType.Good][1] },
		{ "1p_key2_bomb_good", () => Bomb[JudgeType.Good][2] },
		{ "1p_key3_bomb_good", () => Bomb[JudgeType.Good][3] },
		{ "1p_key4_bomb_good", () => Bomb[JudgeType.Good][4] },
		{ "1p_key5_bomb_good", () => Bomb[JudgeType.Good][5] },
		{ "1p_key6_bomb_good", () => Bomb[JudgeType.Good][6] },
		{ "1p_key7_bomb_good", () => Bomb[JudgeType.Good][7] },
		{ "1p_key8_bomb_good", () => Bomb[JudgeType.Good][8] },
		{ "1p_key9_bomb_good", () => Bomb[JudgeType.Good][9] },
		{ "1p_scratch_bomb_good", () => Bomb[JudgeType.Good][0] },
		{ "2p_key1_bomb_good", () => Bomb[JudgeType.Good][11] },
		{ "2p_key2_bomb_good", () => Bomb[JudgeType.Good][12] },
		{ "2p_key3_bomb_good", () => Bomb[JudgeType.Good][13] },
		{ "2p_key4_bomb_good", () => Bomb[JudgeType.Good][14] },
		{ "2p_key5_bomb_good", () => Bomb[JudgeType.Good][15] },
		{ "2p_key6_bomb_good", () => Bomb[JudgeType.Good][16] },
		{ "2p_key7_bomb_good", () => Bomb[JudgeType.Good][17] },
		{ "2p_key8_bomb_good", () => Bomb[JudgeType.Good][18] },
		{ "2p_key9_bomb_good", () => Bomb[JudgeType.Good][19] },
		{ "2p_scratch_bomb_good", () => Bomb[JudgeType.Good][10] },
		{ "1p_key1_bomb_pgreat_exclusive", () => BombExclusive[JudgeType.PGreat][1] },
		{ "1p_key2_bomb_pgreat_exclusive", () => BombExclusive[JudgeType.PGreat][2] },
		{ "1p_key3_bomb_pgreat_exclusive", () => BombExclusive[JudgeType.PGreat][3] },
		{ "1p_key4_bomb_pgreat_exclusive", () => BombExclusive[JudgeType.PGreat][4] },
		{ "1p_key5_bomb_pgreat_exclusive", () => BombExclusive[JudgeType.PGreat][5] },
		{ "1p_key6_bomb_pgreat_exclusive", () => BombExclusive[JudgeType.PGreat][6] },
		{ "1p_key7_bomb_pgreat_exclusive", () => BombExclusive[JudgeType.PGreat][7] },
		{ "1p_key8_bomb_pgreat_exclusive", () => BombExclusive[JudgeType.PGreat][8] },
		{ "1p_key9_bomb_pgreat_exclusive", () => BombExclusive[JudgeType.PGreat][9] },
		{ "1p_scratch_bomb_pgreat_exclusive", () => BombExclusive[JudgeType.PGreat][0] },
		{ "2p_key1_bomb_pgreat_exclusive", () => BombExclusive[JudgeType.PGreat][11] },
		{ "2p_key2_bomb_pgreat_exclusive", () => BombExclusive[JudgeType.PGreat][12] },
		{ "2p_key3_bomb_pgreat_exclusive", () => BombExclusive[JudgeType.PGreat][13] },
		{ "2p_key4_bomb_pgreat_exclusive", () => BombExclusive[JudgeType.PGreat][14] },
		{ "2p_key5_bomb_pgreat_exclusive", () => BombExclusive[JudgeType.PGreat][15] },
		{ "2p_key6_bomb_pgreat_exclusive", () => BombExclusive[JudgeType.PGreat][16] },
		{ "2p_key7_bomb_pgreat_exclusive", () => BombExclusive[JudgeType.PGreat][17] },
		{ "2p_key8_bomb_pgreat_exclusive", () => BombExclusive[JudgeType.PGreat][18] },
		{ "2p_key9_bomb_pgreat_exclusive", () => BombExclusive[JudgeType.PGreat][19] },
		{ "2p_scratch_bomb_pgreat_exclusive", () => BombExclusive[JudgeType.PGreat][10] },
		{ "1p_key1_bomb_great_exclusive", () => BombExclusive[JudgeType.Great][1] },
		{ "1p_key2_bomb_great_exclusive", () => BombExclusive[JudgeType.Great][2] },
		{ "1p_key3_bomb_great_exclusive", () => BombExclusive[JudgeType.Great][3] },
		{ "1p_key4_bomb_great_exclusive", () => BombExclusive[JudgeType.Great][4] },
		{ "1p_key5_bomb_great_exclusive", () => BombExclusive[JudgeType.Great][5] },
		{ "1p_key6_bomb_great_exclusive", () => BombExclusive[JudgeType.Great][6] },
		{ "1p_key7_bomb_great_exclusive", () => BombExclusive[JudgeType.Great][7] },
		{ "1p_key8_bomb_great_exclusive", () => BombExclusive[JudgeType.Great][8] },
		{ "1p_key9_bomb_great_exclusive", () => BombExclusive[JudgeType.Great][9] },
		{ "1p_scratch_bomb_great_exclusive", () => BombExclusive[JudgeType.Great][0] },
		{ "2p_key1_bomb_great_exclusive", () => BombExclusive[JudgeType.Great][11] },
		{ "2p_key2_bomb_great_exclusive", () => BombExclusive[JudgeType.Great][12] },
		{ "2p_key3_bomb_great_exclusive", () => BombExclusive[JudgeType.Great][13] },
		{ "2p_key4_bomb_great_exclusive", () => BombExclusive[JudgeType.Great][14] },
		{ "2p_key5_bomb_great_exclusive", () => BombExclusive[JudgeType.Great][15] },
		{ "2p_key6_bomb_great_exclusive", () => BombExclusive[JudgeType.Great][16] },
		{ "2p_key7_bomb_great_exclusive", () => BombExclusive[JudgeType.Great][17] },
		{ "2p_key8_bomb_great_exclusive", () => BombExclusive[JudgeType.Great][18] },
		{ "2p_key9_bomb_great_exclusive", () => BombExclusive[JudgeType.Great][19] },
		{ "2p_scratch_bomb_great_exclusive", () => BombExclusive[JudgeType.Great][10] },
		{ "1p_key1_bomb_good_exclusive", () => BombExclusive[JudgeType.Good][1] },
		{ "1p_key2_bomb_good_exclusive", () => BombExclusive[JudgeType.Good][2] },
		{ "1p_key3_bomb_good_exclusive", () => BombExclusive[JudgeType.Good][3] },
		{ "1p_key4_bomb_good_exclusive", () => BombExclusive[JudgeType.Good][4] },
		{ "1p_key5_bomb_good_exclusive", () => BombExclusive[JudgeType.Good][5] },
		{ "1p_key6_bomb_good_exclusive", () => BombExclusive[JudgeType.Good][6] },
		{ "1p_key7_bomb_good_exclusive", () => BombExclusive[JudgeType.Good][7] },
		{ "1p_key8_bomb_good_exclusive", () => BombExclusive[JudgeType.Good][8] },
		{ "1p_key9_bomb_good_exclusive", () => BombExclusive[JudgeType.Good][9] },
		{ "1p_scratch_bomb_good_exclusive", () => BombExclusive[JudgeType.Good][0] },
		{ "2p_key1_bomb_good_exclusive", () => BombExclusive[JudgeType.Good][11] },
		{ "2p_key2_bomb_good_exclusive", () => BombExclusive[JudgeType.Good][12] },
		{ "2p_key3_bomb_good_exclusive", () => BombExclusive[JudgeType.Good][13] },
		{ "2p_key4_bomb_good_exclusive", () => BombExclusive[JudgeType.Good][14] },
		{ "2p_key5_bomb_good_exclusive", () => BombExclusive[JudgeType.Good][15] },
		{ "2p_key6_bomb_good_exclusive", () => BombExclusive[JudgeType.Good][16] },
		{ "2p_key7_bomb_good_exclusive", () => BombExclusive[JudgeType.Good][17] },
		{ "2p_key8_bomb_good_exclusive", () => BombExclusive[JudgeType.Good][18] },
		{ "2p_key9_bomb_good_exclusive", () => BombExclusive[JudgeType.Good][19] },
		{ "2p_scratch_bomb_good_exclusive", () => BombExclusive[JudgeType.Good][10] },
		{ "1p_key1_bomb_pgreat_exclusive_2", () => BombExclusiveGreat[JudgeType.PGreat][1] },
		{ "1p_key2_bomb_pgreat_exclusive_2", () => BombExclusiveGreat[JudgeType.PGreat][2] },
		{ "1p_key3_bomb_pgreat_exclusive_2", () => BombExclusiveGreat[JudgeType.PGreat][3] },
		{ "1p_key4_bomb_pgreat_exclusive_2", () => BombExclusiveGreat[JudgeType.PGreat][4] },
		{ "1p_key5_bomb_pgreat_exclusive_2", () => BombExclusiveGreat[JudgeType.PGreat][5] },
		{ "1p_key6_bomb_pgreat_exclusive_2", () => BombExclusiveGreat[JudgeType.PGreat][6] },
		{ "1p_key7_bomb_pgreat_exclusive_2", () => BombExclusiveGreat[JudgeType.PGreat][7] },
		{ "1p_key8_bomb_pgreat_exclusive_2", () => BombExclusiveGreat[JudgeType.PGreat][8] },
		{ "1p_key9_bomb_pgreat_exclusive_2", () => BombExclusiveGreat[JudgeType.PGreat][9] },
		{ "1p_scratch_bomb_pgreat_exclusive_2", () => BombExclusiveGreat[JudgeType.PGreat][0] },
		{ "2p_key1_bomb_pgreat_exclusive_2", () => BombExclusiveGreat[JudgeType.PGreat][11] },
		{ "2p_key2_bomb_pgreat_exclusive_2", () => BombExclusiveGreat[JudgeType.PGreat][12] },
		{ "2p_key3_bomb_pgreat_exclusive_2", () => BombExclusiveGreat[JudgeType.PGreat][13] },
		{ "2p_key4_bomb_pgreat_exclusive_2", () => BombExclusiveGreat[JudgeType.PGreat][14] },
		{ "2p_key5_bomb_pgreat_exclusive_2", () => BombExclusiveGreat[JudgeType.PGreat][15] },
		{ "2p_key6_bomb_pgreat_exclusive_2", () => BombExclusiveGreat[JudgeType.PGreat][16] },
		{ "2p_key7_bomb_pgreat_exclusive_2", () => BombExclusiveGreat[JudgeType.PGreat][17] },
		{ "2p_key8_bomb_pgreat_exclusive_2", () => BombExclusiveGreat[JudgeType.PGreat][18] },
		{ "2p_key9_bomb_pgreat_exclusive_2", () => BombExclusiveGreat[JudgeType.PGreat][19] },
		{ "2p_scratch_bomb_pgreat_exclusive_2", () => BombExclusiveGreat[JudgeType.PGreat][10] },
		{ "1p_key1_bomb_great_exclusive_2", () => BombExclusiveGreat[JudgeType.Great][1] },
		{ "1p_key2_bomb_great_exclusive_2", () => BombExclusiveGreat[JudgeType.Great][2] },
		{ "1p_key3_bomb_great_exclusive_2", () => BombExclusiveGreat[JudgeType.Great][3] },
		{ "1p_key4_bomb_great_exclusive_2", () => BombExclusiveGreat[JudgeType.Great][4] },
		{ "1p_key5_bomb_great_exclusive_2", () => BombExclusiveGreat[JudgeType.Great][5] },
		{ "1p_key6_bomb_great_exclusive_2", () => BombExclusiveGreat[JudgeType.Great][6] },
		{ "1p_key7_bomb_great_exclusive_2", () => BombExclusiveGreat[JudgeType.Great][7] },
		{ "1p_key8_bomb_great_exclusive_2", () => BombExclusiveGreat[JudgeType.Great][8] },
		{ "1p_key9_bomb_great_exclusive_2", () => BombExclusiveGreat[JudgeType.Great][9] },
		{ "1p_scratch_bomb_great_exclusive_2", () => BombExclusiveGreat[JudgeType.Great][0] },
		{ "2p_key1_bomb_great_exclusive_2", () => BombExclusiveGreat[JudgeType.Great][11] },
		{ "2p_key2_bomb_great_exclusive_2", () => BombExclusiveGreat[JudgeType.Great][12] },
		{ "2p_key3_bomb_great_exclusive_2", () => BombExclusiveGreat[JudgeType.Great][13] },
		{ "2p_key4_bomb_great_exclusive_2", () => BombExclusiveGreat[JudgeType.Great][14] },
		{ "2p_key5_bomb_great_exclusive_2", () => BombExclusiveGreat[JudgeType.Great][15] },
		{ "2p_key6_bomb_great_exclusive_2", () => BombExclusiveGreat[JudgeType.Great][16] },
		{ "2p_key7_bomb_great_exclusive_2", () => BombExclusiveGreat[JudgeType.Great][17] },
		{ "2p_key8_bomb_great_exclusive_2", () => BombExclusiveGreat[JudgeType.Great][18] },
		{ "2p_key9_bomb_great_exclusive_2", () => BombExclusiveGreat[JudgeType.Great][19] },
		{ "2p_scratch_bomb_great_exclusive_2", () => BombExclusiveGreat[JudgeType.Great][10] }
	});

	public static double NowTime { get; private set; }

	public static double ElapsedTime => NowTime - prevTime;

	public static double SceneStart { get; set; }

	public static double LoadStart { get; set; }

	public static double LoadComplete { get; set; }

	public static double PlayStart { get; set; }

	public static double Player1ScratchDown => Math.Max(Math.Max(Math.Max(KeyDown[PlayingKeys.Player1ScratchLeft], KeyDown[PlayingKeys.Player1ScratchRight]), KeyDown[PlayingKeys.Player1ScratchToggle]), (!Flags.AutoPlay) ? PlayingKeyState.Player1PlayingScratchRotaryDownTimer : 0.0);

	public static double Player1ScratchDownAlt => Math.Max(Math.Max(Math.Max(KeyDownAlt[PlayingKeys.Player1ScratchLeft], KeyDownAlt[PlayingKeys.Player1ScratchRight]), KeyDownAlt[PlayingKeys.Player1ScratchToggle]), (!Flags.AutoPlay) ? PlayingKeyState.Player1PlayingScratchRotaryDownAltTimer : 0.0);

	public static double Player1ScratchUp
	{
		get
		{
			if (Flags.IsPlayer1ScratchDown)
			{
				return 0.0;
			}
			return Math.Max(Math.Max(Math.Max(KeyUp[PlayingKeys.Player1ScratchLeft], KeyUp[PlayingKeys.Player1ScratchRight]), KeyUp[PlayingKeys.Player1ScratchToggle]), (!Flags.AutoPlay) ? PlayingKeyState.Player1PlayingScratchRotaryUpTimer : 0.0);
		}
	}

	public static double Player2ScratchDown => Math.Max(Math.Max(Math.Max(KeyDown[PlayingKeys.Player2ScratchLeft], KeyDown[PlayingKeys.Player2ScratchRight]), KeyDown[PlayingKeys.Player2ScratchToggle]), (!Flags.AutoPlay) ? PlayingKeyState.Player2PlayingScratchRotaryDownTimer : 0.0);

	public static double Player2ScratchDownAlt => Math.Max(Math.Max(Math.Max(KeyDownAlt[PlayingKeys.Player2ScratchLeft], KeyDownAlt[PlayingKeys.Player2ScratchRight]), KeyDownAlt[PlayingKeys.Player2ScratchToggle]), (!Flags.AutoPlay) ? PlayingKeyState.Player2PlayingScratchRotaryDownAltTimer : 0.0);

	public static double Player2ScratchUp
	{
		get
		{
			if (Flags.IsPlayer2ScratchDown)
			{
				return 0.0;
			}
			return Math.Max(Math.Max(Math.Max(KeyUp[PlayingKeys.Player2ScratchLeft], KeyUp[PlayingKeys.Player2ScratchRight]), KeyUp[PlayingKeys.Player2ScratchToggle]), (!Flags.AutoPlay) ? PlayingKeyState.Player2PlayingScratchRotaryUpTimer : 0.0);
		}
	}

	public static ScratchTimer Player1ScratchRotate { get; private set; } = new ScratchTimer(1);

	public static ScratchTimer Player2ScratchRotate { get; private set; } = new ScratchTimer(2);

	public static Dictionary<PlayingKeys, double> KeyDown { get; set; } = InitKeyTimer();

	public static Dictionary<PlayingKeys, double> KeyDownAlt { get; set; } = InitKeyTimer();

	public static Dictionary<PlayingKeys, double> KeyUp { get; set; } = InitKeyTimer();

	public static double Player1LaneCoverChanged { get; set; }

	public static double Player2LaneCoverChanged { get; set; }

	public static double Player1LiftChanged { get; set; }

	public static double Player2LiftChanged { get; set; }

	public static double GrooveGaugeIncrease { get; set; }

	public static double GrooveGaugeMax { get; set; }

	public static double PoorBga { get; set; }

	public static double Player1Judge { get; private set; }

	public static double Player2Judge { get; private set; }

	public static double Player1LNJudge { get; private set; }

	public static double Player2LNJudge { get; private set; }

	public static double Player1JudgeLeft { get; private set; }

	public static double Player1JudgeCenter { get; private set; }

	public static double Player1JudgeRight { get; private set; }

	public static double Player2JudgeLeft { get; private set; }

	public static double Player2JudgeCenter { get; private set; }

	public static double Player2JudgeRight { get; private set; }

	public static double Player1LNJudgeLeft { get; private set; }

	public static double Player1LNJudgeCenter { get; private set; }

	public static double Player1LNJudgeRight { get; private set; }

	public static double Player2LNJudgeLeft { get; private set; }

	public static double Player2LNJudgeCenter { get; private set; }

	public static double Player2LNJudgeRight { get; private set; }

	public static List<double> LaneJudge { get; set; } = InitLaneJudgeTimer();

	public static List<double> LaneLNJudge { get; set; } = InitLaneJudgeTimer();

	public static List<double> LaneLNHolding { get; set; } = InitLaneJudgeTimer();

	public static Dictionary<JudgeType, List<double>> Bomb { get; set; } = new Dictionary<JudgeType, List<double>>
	{
		{
			JudgeType.PGreat,
			InitLaneJudgeTimer()
		},
		{
			JudgeType.Great,
			InitLaneJudgeTimer()
		},
		{
			JudgeType.Good,
			InitLaneJudgeTimer()
		}
	};

	public static Dictionary<JudgeType, List<double>> BombExclusive { get; set; } = new Dictionary<JudgeType, List<double>>
	{
		{
			JudgeType.PGreat,
			InitLaneJudgeTimer()
		},
		{
			JudgeType.Great,
			InitLaneJudgeTimer()
		},
		{
			JudgeType.Good,
			InitLaneJudgeTimer()
		}
	};

	public static Dictionary<JudgeType, List<double>> BombExclusiveGreat { get; set; } = new Dictionary<JudgeType, List<double>>
	{
		{
			JudgeType.PGreat,
			InitLaneJudgeTimer()
		},
		{
			JudgeType.Great,
			InitLaneJudgeTimer()
		}
	};

	public static double FirstNoteJudge { get; set; }

	public static double BpmTimer
	{
		get
		{
			if (bpmTimer < 0.0)
			{
				return 0.0;
			}
			return NowTime - bpmTimer;
		}
	}

	public static double MaxNotesPerSecondUpdate { get; set; } = 0.0;

	public static double GetValue(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return 0.0;
		}
		if (EventTimerDictionary.ContainsKey(name))
		{
			return EventTimerDictionary[name]();
		}
		return 0.0;
	}

	public static void Update()
	{
		prevTime = NowTime;
		NowTime = GetNowTime();
		Player1ScratchRotate.Update(SceneStart, NowTime);
		Player2ScratchRotate.Update(SceneStart, NowTime);
	}

	public static double GetNowTime()
	{
		return (double)Stopwatch.GetTimestamp() / ((double)Stopwatch.Frequency / 1000.0);
	}

	public static void UpdateNowTime()
	{
		NowTime = GetNowTime();
	}

	private static Dictionary<PlayingKeys, double> InitKeyTimer()
	{
		Dictionary<PlayingKeys, double> dictionary = new Dictionary<PlayingKeys, double>();
		foreach (PlayingKeys value in Enum.GetValues(typeof(PlayingKeys)))
		{
			dictionary.Add(value, 0.0);
		}
		return dictionary;
	}

	private static List<double> InitLaneJudgeTimer()
	{
		List<double> list = new List<double>();
		for (int i = 0; i < 20; i++)
		{
			list.Add(0.0);
		}
		return list;
	}

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

	public static void AddBpmTimer(double time, double bpm)
	{
		bpmTimer += time * (bpm / 60.0);
	}

	public static void StopBpmTimer()
	{
		bpmTimer = -1.0;
	}

	public static void ResetBpmTimer()
	{
		bpmTimer = 0.0;
	}
}
