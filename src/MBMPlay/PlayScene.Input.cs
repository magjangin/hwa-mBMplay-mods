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
	private bool ProcessKeyInput()
	{
		foreach (KeyValuePair<string, KeyRepeatManager> keyRepeatManager in _keyRepeatManagers)
		{
			keyRepeatManager.Value.Update(EventTimers.NowTime);
		}
		if (ProcessCommonKeyInput())
		{
			return true;
		}
		if (!Flags.Loading && Flags.LoadComplete)
		{
			if (!Program.KeyConfig.PlayScene.BackBar.IsKeyDown() || !Program.KeyConfig.PlayScene.ForwardBar.IsKeyDown())
			{
				if (_keyRepeatManagers["BackBarTime"].IsKeyDownOnce())
				{
					double num = Numbers.ElapsedMusicTime.TotalMilliseconds - 5000.0;
					if (num <= 0.0 || Numbers.CurrentBars < 1)
					{
						StartBar = 0;
					}
					else
					{
						StartBar = Numbers.CurrentBars - 1;
						for (int num2 = Numbers.CurrentBars - 1; num2 >= 0; num2--)
						{
							if (_baseTimeTable.Rows[_baseTimeTable.BarIndex[num2]].Time <= num)
							{
								StartBar = num2;
								break;
							}
							if (num2 == 0)
							{
								StartBar = 0;
							}
						}
					}
					Restart();
				}
				else if (_keyRepeatManagers["BackBar4"].IsKeyDownOnce())
				{
					StartBar = ((Numbers.CurrentBars - 4 > 0) ? (Numbers.CurrentBars - 4) : 0);
					Restart();
				}
				else if (_keyRepeatManagers["BackBar"].IsKeyDownOnce())
				{
					StartBar = ((Numbers.CurrentBars - 1 > 0) ? (Numbers.CurrentBars - 1) : 0);
					Restart();
				}
				else if (_keyRepeatManagers["ForwardBarTime"].IsKeyDownOnce())
				{
					int num3 = ((_reverseBar >= 0) ? _reverseBar : Numbers.TotalBars);
					double num4 = Numbers.ElapsedMusicTime.TotalMilliseconds + 5000.0;
					if (num4 >= Numbers.TotalMusicTime.TotalMilliseconds || Numbers.CurrentBars >= num3)
					{
						StartBar = num3;
					}
					else
					{
						StartBar = Numbers.CurrentBars + 1;
						for (int i = Numbers.CurrentBars + 1; i <= num3; i++)
						{
							if (_baseTimeTable.Rows[_baseTimeTable.BarIndex[i]].Time >= num4)
							{
								StartBar = i;
								break;
							}
							if (i == num3)
							{
								StartBar = num3;
							}
						}
					}
					Restart();
				}
				else if (_keyRepeatManagers["ForwardBar4"].IsKeyDownOnce())
				{
					int num5 = ((_reverseBar >= 0) ? _reverseBar : Numbers.TotalBars);
					StartBar = ((Numbers.CurrentBars + 4 >= num5) ? num5 : (Numbers.CurrentBars + 4));
					Restart();
				}
				else if (_keyRepeatManagers["ForwardBar"].IsKeyDownOnce())
				{
					int num6 = ((_reverseBar >= 0) ? _reverseBar : Numbers.TotalBars);
					StartBar = ((Numbers.CurrentBars + 1 >= num6) ? num6 : (Numbers.CurrentBars + 1));
					Restart();
				}
			}
			if (Program.KeyConfig.PlayScene.ResetMusicSpeed.IsKeyDown())
			{
				if (Program.KeyConfig.PlayScene.ResetMusicSpeed.IsKeyDownOnce())
				{
					ChangeMusicSpeed(0);
				}
			}
			else if (_keyRepeatManagers["IncreaseMusicSpeed"].IsKeyDownOnce())
			{
				ChangeMusicSpeed(Numbers.MusicSpeedLevel + 1);
			}
			else if (_keyRepeatManagers["DecreaseMusicSpeed"].IsKeyDownOnce())
			{
				ChangeMusicSpeed(Numbers.MusicSpeedLevel - 1);
			}
			if (Program.KeyConfig.PlayScene.ToggleAutoplay.IsKeyDownOnce())
			{
				Flags.AutoPlay = !Flags.AutoPlay;
				StartBar = 0;
				Restart();
			}
			if (Program.KeyConfig.PlayScene.ChangeLNMode.IsKeyDownOnce())
			{
				Flags.LegacyLN = !Flags.LegacyLN;
				Program.InvokeMainThread(() =>
				{
					Window.Title = PlatformGeneral.GetPlayingWindowTitle();
				});
				StartBar = 0;
				Restart();
			}
			else
			{
				if (Program.KeyConfig.PlayScene.Reload.IsKeyDownOnce())
				{
					ClearResources();
					LoadBms();
					return true;
				}
				if (Program.KeyConfig.PlayScene.Flip.IsKeyDownOnce())
				{
					if (Flags.IsPlayStyle14 || Flags.IsPlayStyle10)
					{
						Flags.FlipSide = !Flags.FlipSide;
						StartBar = 0;
						Restart(swapLanes: true);
					}
				}
				else if (Program.KeyConfig.PlayScene.Random.IsKeyDownOnce() || Program.KeyConfig.PlayScene.Mirror.IsKeyDownOnce() || Program.KeyConfig.PlayScene.SRandom.IsKeyDownOnce())
				{
					LaneSwapOption laneSwapOption;
					if (Program.KeyConfig.PlayScene.Mirror.IsKeyDownOnce())
					{
						laneSwapOption = LaneSwapOption.Mirror;
					}
					else if (Program.KeyConfig.PlayScene.SRandom.IsKeyDownOnce())
					{
						if (PlayingKeyState.IsKeyDownAny(PlayingKeys.Player1Key2, PlayingKeys.Player2Key2))
						{
							laneSwapOption = LaneSwapOption.HRandom;
						}
						else if (PlayingKeyState.IsKeyDownAny(PlayingKeys.Player1Key3, PlayingKeys.Player2Key3))
						{
							laneSwapOption = LaneSwapOption.NonCorrectedSRandom;
						}
						else if (PlayingKeyState.IsKeyDownAny(PlayingKeys.Player1Key4, PlayingKeys.Player2Key4))
						{
							laneSwapOption = LaneSwapOption.PeriodicSRandom;
						}
						else
						{
							laneSwapOption = ((!PlayingKeyState.IsKeyDownAny(PlayingKeys.Player1Key5, PlayingKeys.Player2Key5) || Flags.IsPlayStyle9) ? LaneSwapOption.SRandom : LaneSwapOption.AllScratch);
						}
					}
					else if (PlayingKeyState.IsKeyDownAny(PlayingKeys.Player1Key2, PlayingKeys.Player2Key2))
					{
						laneSwapOption = ((!PlayingKeyState.IsKeyDownAny(PlayingKeys.Player1Key3, PlayingKeys.Player2Key3)) ? LaneSwapOption.Rotate : LaneSwapOption.CrossRotate);
					}
					else if (PlayingKeyState.IsKeyDownAny(PlayingKeys.Player1Key3, PlayingKeys.Player2Key3))
					{
						laneSwapOption = LaneSwapOption.Cross;
					}
					else if (PlayingKeyState.IsKeyDownAny(PlayingKeys.Player1Key4, PlayingKeys.Player2Key4))
					{
						laneSwapOption = ((!PlayingKeyState.IsKeyDownAny(PlayingKeys.Player1Key5, PlayingKeys.Player2Key5)) ? LaneSwapOption.Split : LaneSwapOption.Spread);
					}
					else
					{
						laneSwapOption = ((!PlayingKeyState.IsKeyDownAny(PlayingKeys.Player1Key5, PlayingKeys.Player2Key5)) ? LaneSwapOption.Random : LaneSwapOption.OddEven);
					}
					if (Flags.IsPlayStyle14 || Flags.IsPlayStyle10)
					{
						if (PlayingKeyState.IsKeyDownAny(PlayingKeys.Player1Key1, PlayingKeys.Player2Key1))
						{
							if (PlayingKeyState.IsKeyDown(PlayingKeys.Player1Key1))
							{
								Flags.Player1LaneSwapOption = ((Flags.Player1LaneSwapOption != laneSwapOption) ? laneSwapOption : LaneSwapOption.None);
							}
							if (PlayingKeyState.IsKeyDown(PlayingKeys.Player2Key1))
							{
								Flags.Player2LaneSwapOption = ((Flags.Player2LaneSwapOption != laneSwapOption) ? laneSwapOption : LaneSwapOption.None);
							}
						}
						else
						{
							if (Flags.Player1LaneSwapOption == Flags.Player2LaneSwapOption && Flags.Player1LaneSwapOption == laneSwapOption)
							{
								Flags.Player1LaneSwapOption = LaneSwapOption.None;
							}
							else
							{
								Flags.Player1LaneSwapOption = laneSwapOption;
							}
							Flags.Player2LaneSwapOption = Flags.Player1LaneSwapOption;
						}
					}
					else
					{
						Flags.Player1LaneSwapOption = ((Flags.Player1LaneSwapOption != laneSwapOption) ? laneSwapOption : LaneSwapOption.None);
						Flags.Player2LaneSwapOption = LaneSwapOption.None;
					}
					StartBar = 0;
					Restart(swapLanes: true);
				}
				else if (Program.KeyConfig.PlayScene.ClearRandomMirror.IsKeyDownOnce())
				{
					bool flag = false;
					if (Flags.Player1LaneSwapOption != LaneSwapOption.None)
					{
						flag = true;
					}
					if ((Flags.IsPlayStyle14 || Flags.IsPlayStyle10) && (Flags.Player2LaneSwapOption != LaneSwapOption.None || Flags.FlipSide))
					{
						flag = true;
					}
					if (flag)
					{
						Flags.Player1LaneSwapOption = LaneSwapOption.None;
						if (Flags.IsPlayStyle14 || Flags.IsPlayStyle10)
						{
							Flags.Player2LaneSwapOption = LaneSwapOption.None;
							Flags.FlipSide = false;
						}
						StartBar = 0;
						Restart(swapLanes: true);
					}
				}
				else if (Program.KeyConfig.PlayScene.Rerandomize.IsKeyDownOnce())
				{
					StartBar = 0;
					Restart(Flags.Player1LaneSwapOption.IsRandomize() || Flags.Player2LaneSwapOption.IsRandomize());
				}
				else if (((Flags.AutoPlay || Flags.IsMusicFinished) && Program.KeyConfig.PlayScene.BackToStart.IsKeyDownOnce()) || Program.KeyConfig.PlayScene.BackToStartInTestPlay.IsKeyDownOnce())
				{
					StartBar = 0;
					Restart();
				}
			}
			if (!Flags.AutoPlay)
			{
				if (Program.KeyConfig.PlayScene.JudgeTimingReset.IsKeyDown())
				{
					if (Program.KeyConfig.PlayScene.JudgeTimingReset.IsKeyDownOnce())
					{
						Numbers.JudgeTiming = 0.0;
					}
				}
				else if (_keyRepeatManagers["JudgeTimingUp10"].IsKeyDownOnce())
				{
					Numbers.JudgeTiming = (int)Numbers.JudgeTiming + 10;
				}
				else if (_keyRepeatManagers["JudgeTimingUp"].IsKeyDownOnce())
				{
					Numbers.JudgeTiming = (int)Numbers.JudgeTiming + 1;
				}
				else if (_keyRepeatManagers["JudgeTimingDown10"].IsKeyDownOnce())
				{
					Numbers.JudgeTiming = (int)Numbers.JudgeTiming - 10;
				}
				else if (_keyRepeatManagers["JudgeTimingDown"].IsKeyDownOnce())
				{
					Numbers.JudgeTiming = (int)Numbers.JudgeTiming - 1;
				}
			}
		}
		if (Program.KeyConfig.PlayScene.MasterVolumeMute.IsKeyDown() || Program.KeyConfig.PlayScene.MasterVolumeMax.IsKeyDown())
		{
			if (Program.KeyConfig.PlayScene.MasterVolumeMute.IsKeyDownOnce())
			{
				Numbers.MasterVolume = 0;
			}
			else if (Program.KeyConfig.PlayScene.MasterVolumeMax.IsKeyDownOnce())
			{
				Numbers.MasterVolume = 100;
			}
		}
		else if (_keyRepeatManagers["MasterVolumeDown10"].IsKeyDownOnce())
		{
			Numbers.MasterVolume -= 10;
		}
		else if (_keyRepeatManagers["MasterVolumeDown1"].IsKeyDownOnce())
		{
			Numbers.MasterVolume--;
		}
		else if (_keyRepeatManagers["MasterVolumeDown"].IsKeyDownOnce())
		{
			Numbers.MasterVolume -= 5;
		}
		else if (_keyRepeatManagers["MasterVolumeUp10"].IsKeyDownOnce())
		{
			Numbers.MasterVolume += 10;
		}
		else if (_keyRepeatManagers["MasterVolumeUp1"].IsKeyDownOnce())
		{
			Numbers.MasterVolume++;
		}
		else if (_keyRepeatManagers["MasterVolumeUp"].IsKeyDownOnce())
		{
			Numbers.MasterVolume += 5;
		}
		if (Program.KeyConfig.PlayScene.KeyVolumeMute.IsKeyDown() || Program.KeyConfig.PlayScene.KeyVolumeMax.IsKeyDown())
		{
			if (Program.KeyConfig.PlayScene.KeyVolumeMute.IsKeyDownOnce())
			{
				Numbers.KeyVolume = 0;
			}
			else if (Program.KeyConfig.PlayScene.KeyVolumeMax.IsKeyDownOnce())
			{
				Numbers.KeyVolume = 100;
			}
		}
		else if (_keyRepeatManagers["KeyVolumeDown10"].IsKeyDownOnce())
		{
			Numbers.KeyVolume -= 10;
		}
		else if (_keyRepeatManagers["KeyVolumeDown1"].IsKeyDownOnce())
		{
			Numbers.KeyVolume--;
		}
		else if (_keyRepeatManagers["KeyVolumeDown"].IsKeyDownOnce())
		{
			Numbers.KeyVolume -= 5;
		}
		else if (_keyRepeatManagers["KeyVolumeUp10"].IsKeyDownOnce())
		{
			Numbers.KeyVolume += 10;
		}
		else if (_keyRepeatManagers["KeyVolumeUp1"].IsKeyDownOnce())
		{
			Numbers.KeyVolume++;
		}
		else if (_keyRepeatManagers["KeyVolumeUp"].IsKeyDownOnce())
		{
			Numbers.KeyVolume += 5;
		}
		if (Program.KeyConfig.PlayScene.BgmVolumeMute.IsKeyDown() || Program.KeyConfig.PlayScene.BgmVolumeMax.IsKeyDown())
		{
			if (Program.KeyConfig.PlayScene.BgmVolumeMute.IsKeyDownOnce())
			{
				Numbers.BgmVolume = 0;
			}
			else if (Program.KeyConfig.PlayScene.BgmVolumeMax.IsKeyDownOnce())
			{
				Numbers.BgmVolume = 100;
			}
		}
		else if (_keyRepeatManagers["BgmVolumeDown10"].IsKeyDownOnce())
		{
			Numbers.BgmVolume -= 10;
		}
		else if (_keyRepeatManagers["BgmVolumeDown1"].IsKeyDownOnce())
		{
			Numbers.BgmVolume--;
		}
		else if (_keyRepeatManagers["BgmVolumeDown"].IsKeyDownOnce())
		{
			Numbers.BgmVolume -= 5;
		}
		else if (_keyRepeatManagers["BgmVolumeUp10"].IsKeyDownOnce())
		{
			Numbers.BgmVolume += 10;
		}
		else if (_keyRepeatManagers["BgmVolumeUp1"].IsKeyDownOnce())
		{
			Numbers.BgmVolume++;
		}
		else if (_keyRepeatManagers["BgmVolumeUp"].IsKeyDownOnce())
		{
			Numbers.BgmVolume += 5;
		}
		if (Program.KeyConfig.PlayScene.HiSpeedReset.IsKeyDown())
		{
			if (Program.KeyConfig.PlayScene.HiSpeedReset.IsKeyDownOnce())
			{
				Numbers.Player1HiSpeed = 1.0;
				Numbers.Player2HiSpeed = 1.0;
			}
		}
		else if (_keyRepeatManagers["HiSpeedDown1"].IsKeyDownOnce())
		{
			HiSpeedDown(1.0);
		}
		else if (_keyRepeatManagers["HiSpeedDown005"].IsKeyDownOnce())
		{
			HiSpeedDown(0.05);
		}
		else if (_keyRepeatManagers["HiSpeedDown"].IsKeyDownOnce())
		{
			HiSpeedDown(0.25);
		}
		else if (_keyRepeatManagers["HiSpeedUp1"].IsKeyDownOnce())
		{
			HiSpeedUp(1.0);
		}
		else if (_keyRepeatManagers["HiSpeedUp005"].IsKeyDownOnce())
		{
			HiSpeedUp(0.05);
		}
		else if (_keyRepeatManagers["HiSpeedUp"].IsKeyDownOnce())
		{
			HiSpeedUp(0.25);
		}
		if (Program.KeyConfig.PlayScene.ChangeHSFixBack.IsKeyDownOnce())
		{
			int length = Enum.GetValues(typeof(HiSpeedFixType)).Length;
			Flags.HiSpeedFixType = (HiSpeedFixType)((int)(Flags.HiSpeedFixType + length - 1) % length);
		}
		else if (Program.KeyConfig.PlayScene.ChangeHSFix.IsKeyDownOnce())
		{
			Flags.HiSpeedFixType = (HiSpeedFixType)((int)(Flags.HiSpeedFixType + 1) % Enum.GetValues(typeof(HiSpeedFixType)).Length);
		}
		if (Program.KeyConfig.PlayScene.ChangePlaySide.IsKeyDownOnce())
		{
			Flags.PlaySide = (PlaySide)((int)(Flags.PlaySide + 1) % Enum.GetValues(typeof(PlaySide)).Length);
		}
		if (Program.KeyConfig.PlayScene.DisplayNoteCount.IsKeyDownOnce())
		{
			Flags.DisplayNoteCount = !Flags.DisplayNoteCount;
		}
		if (!Program.KeyConfig.PlayScene.LiftUp.IsKeyDown() || !Program.KeyConfig.PlayScene.LiftDown.IsKeyDown())
		{
			if (_keyRepeatManagers["LiftUp005"].IsKeyDownOnce())
			{
				SetLiftValue(5);
			}
			else if (_keyRepeatManagers["LiftUp10"].IsKeyDownOnce())
			{
				SetLiftValue(1000);
			}
			else if (_keyRepeatManagers["LiftUp01"].IsKeyDownOnce())
			{
				SetLiftValue(10);
			}
			else if (_keyRepeatManagers["LiftUp"].IsKeyDownOnce())
			{
				SetLiftValue(100);
			}
			else if (_keyRepeatManagers["LiftDown005"].IsKeyDownOnce())
			{
				SetLiftValue(-5);
			}
			else if (_keyRepeatManagers["LiftDown10"].IsKeyDownOnce())
			{
				SetLiftValue(-1000);
			}
			else if (_keyRepeatManagers["LiftDown01"].IsKeyDownOnce())
			{
				SetLiftValue(-10);
			}
			else if (_keyRepeatManagers["LiftDown"].IsKeyDownOnce())
			{
				SetLiftValue(-100);
			}
		}
		if (!Program.KeyConfig.PlayScene.LaneCoverUp.IsKeyDown() || !Program.KeyConfig.PlayScene.LaneCoverDown.IsKeyDown())
		{
			if (_keyRepeatManagers["LaneCoverUp005"].IsKeyDownOnce())
			{
				SetLaneCoverValue(-5);
			}
			else if (_keyRepeatManagers["LaneCoverUp10"].IsKeyDownOnce())
			{
				SetLaneCoverValue(-1000);
			}
			else if (_keyRepeatManagers["LaneCoverUp01"].IsKeyDownOnce())
			{
				SetLaneCoverValue(-10);
			}
			else if (_keyRepeatManagers["LaneCoverUp"].IsKeyDownOnce())
			{
				SetLaneCoverValue(-100);
			}
			else if (_keyRepeatManagers["LaneCoverDown005"].IsKeyDownOnce())
			{
				SetLaneCoverValue(5);
			}
			else if (_keyRepeatManagers["LaneCoverDown10"].IsKeyDownOnce())
			{
				SetLaneCoverValue(1000);
			}
			else if (_keyRepeatManagers["LaneCoverDown01"].IsKeyDownOnce())
			{
				SetLaneCoverValue(10);
			}
			else if (_keyRepeatManagers["LaneCoverDown"].IsKeyDownOnce())
			{
				SetLaneCoverValue(100);
			}
		}
		if (Program.KeyConfig.PlayScene.Player1LaneCoverToggle.IsKeyDownOnce())
		{
			Flags.Player1LaneCoverEnabled = !Flags.Player1LaneCoverEnabled;
			if (!Flags.IsPlayStyle7 && !Flags.IsPlayStyle5)
			{
				Flags.Player2LaneCoverEnabled = Flags.Player1LaneCoverEnabled;
			}
		}
		if (Program.KeyConfig.PlayScene.Player2LaneCoverToggle.IsKeyDownOnce())
		{
			Flags.Player2LaneCoverEnabled = !Flags.Player2LaneCoverEnabled;
			if (!Flags.IsPlayStyle7 && !Flags.IsPlayStyle5)
			{
				Flags.Player1LaneCoverEnabled = Flags.Player2LaneCoverEnabled;
			}
		}
		_skin.UpdateControls(EventTimers.NowTime);
		return false;
	}

	private void ChangeMusicSpeed(int value)
	{
		_prevMusicSpeedLevel = Numbers.MusicSpeedLevel;
		Numbers.MusicSpeedLevel = value;
		if (_prevMusicSpeedLevel != Numbers.MusicSpeedLevel)
		{
			StartBar = Numbers.CurrentBars;
			Restart();
		}
	}

	private static void SetLaneCoverValue(int value)
	{
		Numbers.Player1LaneCover += value;
		Numbers.Player2LaneCover = Numbers.Player1LaneCover;
		EventTimers.Player1LaneCoverChanged = EventTimers.NowTime;
		EventTimers.Player2LaneCoverChanged = EventTimers.NowTime;
	}

	private static void SetLiftValue(int value)
	{
		Numbers.Player1Lift += value;
		Numbers.Player2Lift = Numbers.Player1Lift;
		EventTimers.Player1LiftChanged = EventTimers.NowTime;
		EventTimers.Player2LiftChanged = EventTimers.NowTime;
	}

	private static void HiSpeedUp(double amount)
	{
		Numbers.Player1HiSpeed = (double)((int)(Numbers.Player1HiSpeed * 10000.0) + (int)(amount * 10000.0)) / 10000.0;
		Numbers.Player2HiSpeed = Numbers.Player1HiSpeed;
	}

	private static void HiSpeedDown(double amount)
	{
		Numbers.Player1HiSpeed = (double)((int)(Numbers.Player1HiSpeed * 10000.0) - (int)(amount * 10000.0)) / 10000.0;
		if (Numbers.Player1HiSpeed < 0.25)
		{
			Numbers.Player1HiSpeed = 0.25;
		}
		Numbers.Player2HiSpeed = Numbers.Player1HiSpeed;
	}
}
