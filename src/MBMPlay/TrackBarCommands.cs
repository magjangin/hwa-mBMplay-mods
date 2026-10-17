using System.Collections.Generic;
using System.Collections.ObjectModel;
using MistUtils;

namespace MBMPlay;

public static class TrackBarCommands
{
	private static readonly ReadOnlyDictionary<string, ValueCommand<double>> CommandDictionary = Extensions.AsReadOnly(new Dictionary<string, ValueCommand<double>>
	{
		{
			"test",
			new ValueCommand<double>(() => Numbers.TestValue, (double value) =>
			{
				Numbers.TestValue = value;
			})
		},
		{
			"1p_hs",
			new ValueCommand<double>(() => Numbers.Player1HiSpeed, (double value) =>
			{
				Numbers.Player1HiSpeed = value;
				if (Numbers.Player1HiSpeed < 0.25)
				{
					Numbers.Player1HiSpeed = 0.25;
				}
				Numbers.Player2HiSpeed = Numbers.Player1HiSpeed;
			})
		},
		{
			"2p_hs",
			new ValueCommand<double>(() => Numbers.Player2HiSpeed, (double value) =>
			{
				Numbers.Player2HiSpeed = value;
				if (Numbers.Player2HiSpeed < 0.25)
				{
					Numbers.Player2HiSpeed = 0.25;
				}
				Numbers.Player1HiSpeed = Numbers.Player2HiSpeed;
			})
		},
		{
			"volume_master",
			new ValueCommand<double>(() => Numbers.MasterVolume, (double value) =>
			{
				Numbers.MasterVolume = (int)value;
			})
		},
		{
			"volume_key",
			new ValueCommand<double>(() => Numbers.KeyVolume, (double value) =>
			{
				Numbers.KeyVolume = (int)value;
			})
		},
		{
			"volume_bgm",
			new ValueCommand<double>(() => Numbers.BgmVolume, (double value) =>
			{
				Numbers.BgmVolume = (int)value;
			})
		}
	});

	public static ValueCommand<double> GetCommand(string name)
	{
		if (CommandDictionary.ContainsKey(name))
		{
			return CommandDictionary[name];
		}
		if (Numbers.TryGetCommandDouble(name, out var func))
		{
			return new ValueCommand<double>(func, (double _) =>
			{
			});
		}
		return null;
	}
}
