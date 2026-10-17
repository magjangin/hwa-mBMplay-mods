using MistInput;

namespace MBMPlay.Common;

public class KeyConfigPlayStyles
{
	public PlayingKeyConfig FiveKeys { get; set; }

	public PlayingKeyConfig SevenKeys { get; set; }

	public PlayingKeyConfig TenKeys { get; set; }

	public PlayingKeyConfig FourteenKeys { get; set; }

	public PlayingKeyConfig NineButtons { get; set; }

	public PlayingKeyConfig this[int keys]
	{
		get
		{
			return keys switch
			{
				7 => SevenKeys, 
				14 => FourteenKeys, 
				5 => FiveKeys, 
				10 => TenKeys, 
				9 => NineButtons, 
				_ => null, 
			};
		}
		set
		{
			switch (keys)
			{
			case 7:
				SevenKeys = value;
				break;
			case 14:
				FourteenKeys = value;
				break;
			case 5:
				FiveKeys = value;
				break;
			case 10:
				TenKeys = value;
				break;
			case 9:
				NineButtons = value;
				break;
			}
		}
	}

	public static KeyConfigPlayStyles Default
	{
		get
		{
			KeyConfigPlayStyles keyConfigPlayStyles = new KeyConfigPlayStyles();
			keyConfigPlayStyles.FourteenKeys = new PlayingKeyConfig
			{
				Player1ScratchToggle = new InputCollection
				{
					new KeyboardInput(Key.LeftShift)
				},
				Player1Key1 = new InputCollection
				{
					new KeyboardInput(Key.Z)
				},
				Player1Key2 = new InputCollection
				{
					new KeyboardInput(Key.S)
				},
				Player1Key3 = new InputCollection
				{
					new KeyboardInput(Key.X)
				},
				Player1Key4 = new InputCollection
				{
					new KeyboardInput(Key.D)
				},
				Player1Key5 = new InputCollection
				{
					new KeyboardInput(Key.C)
				},
				Player1Key6 = new InputCollection
				{
					new KeyboardInput(Key.F)
				},
				Player1Key7 = new InputCollection
				{
					new KeyboardInput(Key.V)
				},
				Player2Key1 = new InputCollection
				{
					new KeyboardInput(Key.Comma)
				},
				Player2Key2 = new InputCollection
				{
					new KeyboardInput(Key.L)
				},
				Player2Key3 = new InputCollection
				{
					new KeyboardInput(Key.Period)
				},
				Player2Key4 = new InputCollection
				{
					new KeyboardInput(Key.Semicolon)
				},
				Player2Key5 = new InputCollection
				{
					new KeyboardInput(Key.Slash)
				},
				Player2Key6 = new InputCollection
				{
					new KeyboardInput(Key.Colon)
				},
				Player2Key7 = new InputCollection
				{
					new KeyboardInput(Key.BackSlash)
				},
				Player2ScratchToggle = new InputCollection
				{
					new KeyboardInput(Key.RightShift)
				}
			};
			keyConfigPlayStyles.NineButtons = new PlayingKeyConfig
			{
				Player1Key1 = new InputCollection
				{
					new KeyboardInput(Key.Z)
				},
				Player1Key2 = new InputCollection
				{
					new KeyboardInput(Key.S)
				},
				Player1Key3 = new InputCollection
				{
					new KeyboardInput(Key.X)
				},
				Player1Key4 = new InputCollection
				{
					new KeyboardInput(Key.D)
				},
				Player1Key5 = new InputCollection
				{
					new KeyboardInput(Key.C)
				},
				Player1Key6 = new InputCollection
				{
					new KeyboardInput(Key.F)
				},
				Player1Key7 = new InputCollection
				{
					new KeyboardInput(Key.V)
				},
				Player1Key8 = new InputCollection
				{
					new KeyboardInput(Key.G)
				},
				Player1Key9 = new InputCollection
				{
					new KeyboardInput(Key.B)
				}
			};
			return keyConfigPlayStyles;
		}
	}

	public KeyConfigPlayStyles()
	{
	}

	public KeyConfigPlayStyles(KeyConfigPlayStyles keyConfig)
	{
		if (keyConfig != null)
		{
			if (keyConfig.FiveKeys != null)
			{
				FiveKeys = new PlayingKeyConfig(keyConfig.FiveKeys);
			}
			if (keyConfig.SevenKeys != null)
			{
				SevenKeys = new PlayingKeyConfig(keyConfig.SevenKeys);
			}
			if (keyConfig.TenKeys != null)
			{
				TenKeys = new PlayingKeyConfig(keyConfig.TenKeys);
			}
			if (keyConfig.FourteenKeys != null)
			{
				FourteenKeys = new PlayingKeyConfig(keyConfig.FourteenKeys);
			}
			if (keyConfig.NineButtons != null)
			{
				NineButtons = new PlayingKeyConfig(keyConfig.NineButtons);
			}
		}
	}
}
