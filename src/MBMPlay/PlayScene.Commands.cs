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
	private ReadOnlyDictionary<string, Func<IList<string>, Action<double>>> CreateButtonCommandDictionary()
	{
		return Extensions.AsReadOnly(new Dictionary<string, Func<IList<string>, Action<double>>>
		{
			{
				"music_speed_up",
				(IList<string> args) =>
				{
					List<int> list = ButtonCommands.ParseIntArgs(args);
					int i = ((list == null || list.Count < 1) ? 1 : list[0]);
					return (double _) =>
					{
						ChangeMusicSpeed(Numbers.MusicSpeedLevel + i);
					};
				}
			},
			{
				"music_speed_down",
				(IList<string> args) =>
				{
					List<int> list = ButtonCommands.ParseIntArgs(args);
					int i = ((list != null && list.Count >= 1) ? (list[0] * -1) : (-1));
					return (double _) =>
					{
						ChangeMusicSpeed(Numbers.MusicSpeedLevel + i);
					};
				}
			}
		});
	}

	private ReadOnlyDictionary<string, ValueCommand<double>> CreateTrackBarCommandDictionary()
	{
		return Extensions.AsReadOnly(new Dictionary<string, ValueCommand<double>>
		{
			{
				"music_time_position",
				new ValueCommand<double>(() => Numbers.MusicTimePosition, (double value) =>
				{
					double num = Numbers.TotalMusicTime.TotalMilliseconds * (value / 100.0);
					if (num < Numbers.ElapsedMusicTime.TotalMilliseconds)
					{
						if (num <= 0.0 || Numbers.CurrentBars < 1)
						{
							StartBar = 0;
						}
						else
						{
							StartBar = Numbers.CurrentBars;
							for (int num2 = Numbers.CurrentBars; num2 >= 0; num2--)
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
					}
					else
					{
						int num3 = ((_reverseBar >= 0) ? _reverseBar : Numbers.TotalBars);
						if (num >= Numbers.TotalMusicTime.TotalMilliseconds || Numbers.CurrentBars >= num3)
						{
							StartBar = num3;
						}
						else
						{
							StartBar = Numbers.CurrentBars;
							for (int i = Numbers.CurrentBars; i <= num3; i++)
							{
								if (_baseTimeTable.Rows[_baseTimeTable.BarIndex[i]].Time >= num)
								{
									StartBar = ((i >= 0) ? (i - 1) : 0);
									break;
								}
								if (i == num3)
								{
									StartBar = num3;
								}
							}
						}
					}
					Restart();
				})
			},
			{
				"music_speed_level",
				new ValueCommand<double>(() => Numbers.MusicSpeedLevel, (double value) =>
				{
					ChangeMusicSpeed((int)value);
				})
			}
		});
	}

	public override Action<double> GetButtonCommandAction(string command)
	{
		if (ButtonCommands.ParseCommand(command, out var name, out var args) && ButtonCommandDictionary.ContainsKey(name))
		{
			return ButtonCommandDictionary[name]?.Invoke(args) ?? null;
		}
		return null;
	}

	public override ValueCommand<double> GetTrackBarCommand(string command)
	{
		if (TrackBarCommandDictionary.ContainsKey(command))
		{
			return TrackBarCommandDictionary[command];
		}
		return null;
	}

	private bool ProcessRemoteProxy()
	{
		if (Program.RemoteProxy != null)
		{
			if (!string.IsNullOrEmpty(Program.RemoteProxy.BmsPath))
			{
				if (File.Exists(Program.RemoteProxy.BmsPath) && Bms.CheckBmsFile(Program.RemoteProxy.BmsPath))
				{
					ClearResources();
					BmsPath = Program.RemoteProxy.BmsPath;
					StartBar = Program.RemoteProxy.StartBar;
					SetLaneSwapOptionsFromRemoteProxy();
					Program.RemoteProxy.Reset();
					LoadBms();
					Program.InvokeMainThread(() =>
					{
						Window.SetActive();
					});
					return true;
				}
			}
			else if (Program.RemoteProxy.Player1LaneSwapOption.HasValue || Program.RemoteProxy.Player2LaneSwapOption.HasValue || Program.RemoteProxy.FlipSide.HasValue)
			{
				SetLaneSwapOptionsFromRemoteProxy();
				StartBar = Program.RemoteProxy.StartBar;
				Program.RemoteProxy.Reset();
				Restart(swapLanes: true);
				Program.InvokeMainThread(() =>
				{
					Window.SetActive();
				});
				return false;
			}
			if (Program.RemoteProxy.IsStopRequested)
			{
				Program.RemoteProxy.Reset();
				Window.Exit();
				return true;
			}
			Program.RemoteProxy.Reset();
		}
		return false;
	}

	private void SetLaneSwapOptionsFromRemoteProxy()
	{
		if (Program.RemoteProxy.Player1LaneSwapOption.HasValue)
		{
			Program.InitialPlayer1LaneSwapOption = Program.RemoteProxy.Player1LaneSwapOption;
		}
		if (Program.RemoteProxy.Player2LaneSwapOption.HasValue)
		{
			Program.InitialPlayer2LaneSwapOption = Program.RemoteProxy.Player2LaneSwapOption;
		}
		if (Program.RemoteProxy.FlipSide.HasValue)
		{
			Program.InitialFlipSide = Program.RemoteProxy.FlipSide;
		}
		Flags.Player1LaneSwapOption = Program.InitialPlayer1LaneSwapOption ?? LaneSwapOption.None;
		Flags.Player2LaneSwapOption = Program.InitialPlayer2LaneSwapOption ?? LaneSwapOption.None;
		Flags.FlipSide = Program.InitialFlipSide ?? false;
	}
}
