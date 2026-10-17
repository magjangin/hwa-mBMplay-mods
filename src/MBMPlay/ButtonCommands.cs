using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using MBMPlay.Common;
using MistUtils;

namespace MBMPlay;

public static class ButtonCommands
{
	private static readonly ReadOnlyDictionary<string, Func<IList<string>, Action<double>>> CommandDictionary = Extensions.AsReadOnly(new Dictionary<string, Func<IList<string>, Action<double>>>
	{
		{ "none", null },
		{
			"test",
			(IList<string> args) =>
			{
				List<double> list = ParseDoubleArgs(args);
				if (args != null && args.Count >= 1)
				{
					_ = list[0];
					return (double _) =>
					{
					};
				}
				return (double _) =>
				{
				};
			}
		},
		{
			"volume_master_up",
			(IList<string> args) =>
			{
				List<int> list = ParseIntArgs(args);
				int i = ((list == null || list.Count < 1) ? 1 : list[0]);
				return (double _) =>
				{
					Numbers.MasterVolume += i;
				};
			}
		},
		{
			"volume_master_down",
			(IList<string> args) =>
			{
				List<int> list = ParseIntArgs(args);
				int i = ((list == null || list.Count < 1) ? 1 : list[0]);
				return (double _) =>
				{
					Numbers.MasterVolume -= i;
				};
			}
		},
		{
			"volume_master_mute",
			(IList<string> args) => (double _) =>
			{
				Flags.IsMasterVolumeMute = !Flags.IsMasterVolumeMute;
			}
		},
		{
			"volume_key_up",
			(IList<string> args) =>
			{
				List<int> list = ParseIntArgs(args);
				int i = ((list == null || list.Count < 1) ? 1 : list[0]);
				return (double _) =>
				{
					Numbers.KeyVolume += i;
				};
			}
		},
		{
			"volume_key_down",
			(IList<string> args) =>
			{
				List<int> list = ParseIntArgs(args);
				int i = ((list == null || list.Count < 1) ? 1 : list[0]);
				return (double _) =>
				{
					Numbers.KeyVolume -= i;
				};
			}
		},
		{
			"volume_key_mute",
			(IList<string> args) => (double _) =>
			{
				Flags.IsKeyVolumeMute = !Flags.IsKeyVolumeMute;
			}
		},
		{
			"volume_bgm_up",
			(IList<string> args) =>
			{
				List<int> list = ParseIntArgs(args);
				int i = ((list == null || list.Count < 1) ? 1 : list[0]);
				return (double _) =>
				{
					Numbers.BgmVolume += i;
				};
			}
		},
		{
			"volume_bgm_down",
			(IList<string> args) =>
			{
				List<int> list = ParseIntArgs(args);
				int i = ((list == null || list.Count < 1) ? 1 : list[0]);
				return (double _) =>
				{
					Numbers.BgmVolume -= i;
				};
			}
		},
		{
			"volume_bgm_mute",
			(IList<string> args) => (double _) =>
			{
				Flags.IsBgmVolumeMute = !Flags.IsBgmVolumeMute;
			}
		},
		{
			"hs_up",
			(IList<string> args) =>
			{
				List<double> list = ParseDoubleArgs(args);
				double d = ((list != null && list.Count >= 1) ? list[0] : 0.25);
				if (d < 0.0)
				{
					d = 0.0;
				}
				return (double _) =>
				{
					HiSpeedUp(d);
				};
			}
		},
		{
			"hs_down",
			(IList<string> args) =>
			{
				List<double> list = ParseDoubleArgs(args);
				double d = ((list != null && list.Count >= 1) ? list[0] : 0.25);
				if (d < 0.0)
				{
					d = 0.0;
				}
				return (double _) =>
				{
					HiSpeedDown(d);
				};
			}
		},
		{
			"hsfix_change",
			(IList<string> args) => (double _) =>
			{
				ChangeHiSpeedFixType();
			}
		},
		{
			"hsfix_change_back",
			(IList<string> args) => (double _) =>
			{
				ChangeHiSpeedFixTypeBack();
			}
		},
		{
			"hsfix_set",
			(IList<string> args) =>
			{
				if (args.Count > 0)
				{
					switch (args[0])
					{
					case "normal":
					case "none":
						return (double _) =>
						{
							Flags.HiSpeedFixType = HiSpeedFixType.None;
						};
					case "maxfix":
					case "max":
						return (double _) =>
						{
							Flags.HiSpeedFixType = HiSpeedFixType.Max;
						};
					case "minfix":
					case "min":
						return (double _) =>
						{
							Flags.HiSpeedFixType = HiSpeedFixType.Min;
						};
					case "main_notes":
						return (double _) =>
						{
							Flags.HiSpeedFixType = HiSpeedFixType.MainNotes;
						};
					case "main_time":
						return (double _) =>
						{
							Flags.HiSpeedFixType = HiSpeedFixType.MainTime;
						};
					case "regular_speed":
					case "reg_speed":
						return (double _) =>
						{
							Flags.HiSpeedFixType = HiSpeedFixType.RegularSpeed;
						};
					default:
						return (Action<double>)null;
					}
				}
				return (Action<double>)null;
			}
		},
		{
			"judge_timing_up",
			(IList<string> args) =>
			{
				List<int> list = ParseIntArgs(args);
				int i = ((list == null || list.Count < 1) ? 1 : list[0]);
				return (double _) =>
				{
					Numbers.JudgeTiming = (int)Numbers.JudgeTiming + i;
				};
			}
		},
		{
			"judge_timing_down",
			(IList<string> args) =>
			{
				List<int> list = ParseIntArgs(args);
				int i = ((list != null && list.Count >= 1) ? (list[0] * -1) : (-1));
				return (double _) =>
				{
					Numbers.JudgeTiming = (int)Numbers.JudgeTiming + i;
				};
			}
		}
	});

	public static Action<double> GetAction(string command)
	{
		if (ParseCommand(command, out var name, out var args) && CommandDictionary.ContainsKey(name))
		{
			return CommandDictionary[name]?.Invoke(args) ?? null;
		}
		return null;
	}

	public static bool ParseCommand(string command, out string name, out List<string> args)
	{
		Match match = Regex.Match(command.Replace(" ", ""), "([\\da-z_]+)\\(?([\\dA-Za-z_\\-,\\.]*)\\)?");
		if (match.Success && match.Groups.Count >= 2)
		{
			name = match.Groups[1].Value;
			if (match.Groups.Count >= 3 && !string.IsNullOrEmpty(match.Groups[2].Value))
			{
				args = match.Groups[2].Value.Split(',').ToList();
			}
			else
			{
				args = new List<string>();
			}
			return true;
		}
		name = null;
		args = null;
		return false;
	}

	public static List<int> ParseIntArgs(IList<string> args)
	{
		if (args == null)
		{
			return null;
		}
		List<int> list = new List<int>(args.Count);
		foreach (string arg in args)
		{
			if (int.TryParse(arg, out var result))
			{
				list.Add(result);
			}
		}
		return list;
	}

	public static List<double> ParseDoubleArgs(IList<string> args)
	{
		if (args == null)
		{
			return null;
		}
		List<double> list = new List<double>(args.Count);
		foreach (string arg in args)
		{
			if (double.TryParse(arg, out var result))
			{
				list.Add(result);
			}
		}
		return list;
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

	private static void ChangeHiSpeedFixType()
	{
		Flags.HiSpeedFixType = (HiSpeedFixType)((int)(Flags.HiSpeedFixType + 1) % Enum.GetValues(typeof(HiSpeedFixType)).Length);
	}

	private static void ChangeHiSpeedFixTypeBack()
	{
		int length = Enum.GetValues(typeof(HiSpeedFixType)).Length;
		Flags.HiSpeedFixType = (HiSpeedFixType)((int)(Flags.HiSpeedFixType + length - 1) % length);
	}
}
