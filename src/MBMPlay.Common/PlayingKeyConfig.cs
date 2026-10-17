using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;

namespace MBMPlay.Common;

public class PlayingKeyConfig
{
	private readonly Dictionary<PlayingKeys, Func<InputCollection>> GetPlayingKeyConfigDictionary;

	private readonly Dictionary<PlayingKeys, Action<InputCollection>> SetPlayingKeyConfigDictionary;

	public InputCollection this[PlayingKeys key]
	{
		get
		{
			return GetPlayingKeyConfigDictionary[key]();
		}
		set
		{
			SetPlayingKeyConfigDictionary[key](value);
		}
	}

	[XmlArrayItem(Type = typeof(KeyboardInput), IsNullable = false)]
	[XmlArrayItem(Type = typeof(JoystickButtonInput), IsNullable = false)]
	public InputCollection Player1Key1 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput), IsNullable = false)]
	[XmlArrayItem(Type = typeof(JoystickButtonInput), IsNullable = false)]
	public InputCollection Player1Key2 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput), IsNullable = false)]
	[XmlArrayItem(Type = typeof(JoystickButtonInput), IsNullable = false)]
	public InputCollection Player1Key3 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput), IsNullable = false)]
	[XmlArrayItem(Type = typeof(JoystickButtonInput), IsNullable = false)]
	public InputCollection Player1Key4 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput), IsNullable = false)]
	[XmlArrayItem(Type = typeof(JoystickButtonInput), IsNullable = false)]
	public InputCollection Player1Key5 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput), IsNullable = false)]
	[XmlArrayItem(Type = typeof(JoystickButtonInput), IsNullable = false)]
	public InputCollection Player1Key6 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput), IsNullable = false)]
	[XmlArrayItem(Type = typeof(JoystickButtonInput), IsNullable = false)]
	public InputCollection Player1Key7 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput), IsNullable = false)]
	[XmlArrayItem(Type = typeof(JoystickButtonInput), IsNullable = false)]
	public InputCollection Player1Key8 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput), IsNullable = false)]
	[XmlArrayItem(Type = typeof(JoystickButtonInput), IsNullable = false)]
	public InputCollection Player1Key9 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput), IsNullable = false)]
	[XmlArrayItem(Type = typeof(JoystickButtonInput), IsNullable = false)]
	public InputCollection Player1ScratchLeft { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput), IsNullable = false)]
	[XmlArrayItem(Type = typeof(JoystickButtonInput), IsNullable = false)]
	public InputCollection Player1ScratchRight { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput), IsNullable = false)]
	[XmlArrayItem(Type = typeof(JoystickButtonInput), IsNullable = false)]
	public InputCollection Player1ScratchToggle { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput), IsNullable = false)]
	[XmlArrayItem(Type = typeof(JoystickButtonInput), IsNullable = false)]
	public InputCollection Player1Start { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput), IsNullable = false)]
	[XmlArrayItem(Type = typeof(JoystickButtonInput), IsNullable = false)]
	public InputCollection Player1Select { get; set; }

	[XmlArrayItem(Type = typeof(JoystickButtonInput), IsNullable = false)]
	public InputCollection Player1ScratchLeftRotary { get; set; }

	[XmlArrayItem(Type = typeof(JoystickButtonInput), IsNullable = false)]
	public InputCollection Player1ScratchRightRotary { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput), IsNullable = false)]
	[XmlArrayItem(Type = typeof(JoystickButtonInput), IsNullable = false)]
	public InputCollection Player2Key1 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput), IsNullable = false)]
	[XmlArrayItem(Type = typeof(JoystickButtonInput), IsNullable = false)]
	public InputCollection Player2Key2 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput), IsNullable = false)]
	[XmlArrayItem(Type = typeof(JoystickButtonInput), IsNullable = false)]
	public InputCollection Player2Key3 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput), IsNullable = false)]
	[XmlArrayItem(Type = typeof(JoystickButtonInput), IsNullable = false)]
	public InputCollection Player2Key4 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput), IsNullable = false)]
	[XmlArrayItem(Type = typeof(JoystickButtonInput), IsNullable = false)]
	public InputCollection Player2Key5 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput), IsNullable = false)]
	[XmlArrayItem(Type = typeof(JoystickButtonInput), IsNullable = false)]
	public InputCollection Player2Key6 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput), IsNullable = false)]
	[XmlArrayItem(Type = typeof(JoystickButtonInput), IsNullable = false)]
	public InputCollection Player2Key7 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput), IsNullable = false)]
	[XmlArrayItem(Type = typeof(JoystickButtonInput), IsNullable = false)]
	public InputCollection Player2Key8 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput), IsNullable = false)]
	[XmlArrayItem(Type = typeof(JoystickButtonInput), IsNullable = false)]
	public InputCollection Player2Key9 { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput), IsNullable = false)]
	[XmlArrayItem(Type = typeof(JoystickButtonInput), IsNullable = false)]
	public InputCollection Player2ScratchLeft { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput), IsNullable = false)]
	[XmlArrayItem(Type = typeof(JoystickButtonInput), IsNullable = false)]
	public InputCollection Player2ScratchRight { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput), IsNullable = false)]
	[XmlArrayItem(Type = typeof(JoystickButtonInput), IsNullable = false)]
	public InputCollection Player2ScratchToggle { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput), IsNullable = false)]
	[XmlArrayItem(Type = typeof(JoystickButtonInput), IsNullable = false)]
	public InputCollection Player2Start { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput), IsNullable = false)]
	[XmlArrayItem(Type = typeof(JoystickButtonInput), IsNullable = false)]
	public InputCollection Player2Select { get; set; }

	[XmlArrayItem(Type = typeof(JoystickButtonInput), IsNullable = false)]
	public InputCollection Player2ScratchLeftRotary { get; set; }

	[XmlArrayItem(Type = typeof(JoystickButtonInput), IsNullable = false)]
	public InputCollection Player2ScratchRightRotary { get; set; }

	public PlayingKeyConfig()
	{
		GetPlayingKeyConfigDictionary = InitGetPlayingKeyConfigDictionary();
		SetPlayingKeyConfigDictionary = InitSetPlayingKeyConfigDictionary();
	}

	public PlayingKeyConfig(PlayingKeyConfig keyConfig)
	{
		if (keyConfig != null)
		{
			PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (propertyInfo.CanWrite && propertyInfo.PropertyType == typeof(InputCollection) && propertyInfo.GetIndexParameters().Length == 0)
				{
					object value = propertyInfo.GetValue(keyConfig);
					if (value != null)
					{
						propertyInfo.SetValue(this, new InputCollection((InputCollection)value));
					}
				}
			}
		}
		GetPlayingKeyConfigDictionary = InitGetPlayingKeyConfigDictionary();
		SetPlayingKeyConfigDictionary = InitSetPlayingKeyConfigDictionary();
	}

	private Dictionary<PlayingKeys, Func<InputCollection>> InitGetPlayingKeyConfigDictionary()
	{
		return new Dictionary<PlayingKeys, Func<InputCollection>>
		{
			{
				PlayingKeys.Player1Key1,
				() => Player1Key1
			},
			{
				PlayingKeys.Player1Key2,
				() => Player1Key2
			},
			{
				PlayingKeys.Player1Key3,
				() => Player1Key3
			},
			{
				PlayingKeys.Player1Key4,
				() => Player1Key4
			},
			{
				PlayingKeys.Player1Key5,
				() => Player1Key5
			},
			{
				PlayingKeys.Player1Key6,
				() => Player1Key6
			},
			{
				PlayingKeys.Player1Key7,
				() => Player1Key7
			},
			{
				PlayingKeys.Player1Key8,
				() => Player1Key8
			},
			{
				PlayingKeys.Player1Key9,
				() => Player1Key9
			},
			{
				PlayingKeys.Player1ScratchLeft,
				() => Player1ScratchLeft
			},
			{
				PlayingKeys.Player1ScratchRight,
				() => Player1ScratchRight
			},
			{
				PlayingKeys.Player1ScratchLeftRotary,
				() => Player1ScratchLeftRotary
			},
			{
				PlayingKeys.Player1ScratchRightRotary,
				() => Player1ScratchRightRotary
			},
			{
				PlayingKeys.Player1ScratchToggle,
				() => Player1ScratchToggle
			},
			{
				PlayingKeys.Player1Start,
				() => Player1Start
			},
			{
				PlayingKeys.Player1Select,
				() => Player1Select
			},
			{
				PlayingKeys.Player2Key1,
				() => Player2Key1
			},
			{
				PlayingKeys.Player2Key2,
				() => Player2Key2
			},
			{
				PlayingKeys.Player2Key3,
				() => Player2Key3
			},
			{
				PlayingKeys.Player2Key4,
				() => Player2Key4
			},
			{
				PlayingKeys.Player2Key5,
				() => Player2Key5
			},
			{
				PlayingKeys.Player2Key6,
				() => Player2Key6
			},
			{
				PlayingKeys.Player2Key7,
				() => Player2Key7
			},
			{
				PlayingKeys.Player2Key8,
				() => Player2Key8
			},
			{
				PlayingKeys.Player2Key9,
				() => Player2Key9
			},
			{
				PlayingKeys.Player2ScratchLeft,
				() => Player2ScratchLeft
			},
			{
				PlayingKeys.Player2ScratchRight,
				() => Player2ScratchRight
			},
			{
				PlayingKeys.Player2ScratchLeftRotary,
				() => Player2ScratchLeftRotary
			},
			{
				PlayingKeys.Player2ScratchRightRotary,
				() => Player2ScratchRightRotary
			},
			{
				PlayingKeys.Player2ScratchToggle,
				() => Player2ScratchToggle
			},
			{
				PlayingKeys.Player2Start,
				() => Player2Start
			},
			{
				PlayingKeys.Player2Select,
				() => Player2Select
			}
		};
	}

	private Dictionary<PlayingKeys, Action<InputCollection>> InitSetPlayingKeyConfigDictionary()
	{
		return new Dictionary<PlayingKeys, Action<InputCollection>>
		{
			{
				PlayingKeys.Player1Key1,
				(InputCollection value) =>
				{
					Player1Key1 = value;
				}
			},
			{
				PlayingKeys.Player1Key2,
				(InputCollection value) =>
				{
					Player1Key2 = value;
				}
			},
			{
				PlayingKeys.Player1Key3,
				(InputCollection value) =>
				{
					Player1Key3 = value;
				}
			},
			{
				PlayingKeys.Player1Key4,
				(InputCollection value) =>
				{
					Player1Key4 = value;
				}
			},
			{
				PlayingKeys.Player1Key5,
				(InputCollection value) =>
				{
					Player1Key5 = value;
				}
			},
			{
				PlayingKeys.Player1Key6,
				(InputCollection value) =>
				{
					Player1Key6 = value;
				}
			},
			{
				PlayingKeys.Player1Key7,
				(InputCollection value) =>
				{
					Player1Key7 = value;
				}
			},
			{
				PlayingKeys.Player1Key8,
				(InputCollection value) =>
				{
					Player1Key8 = value;
				}
			},
			{
				PlayingKeys.Player1Key9,
				(InputCollection value) =>
				{
					Player1Key9 = value;
				}
			},
			{
				PlayingKeys.Player1ScratchLeft,
				(InputCollection value) =>
				{
					Player1ScratchLeft = value;
				}
			},
			{
				PlayingKeys.Player1ScratchRight,
				(InputCollection value) =>
				{
					Player1ScratchRight = value;
				}
			},
			{
				PlayingKeys.Player1ScratchLeftRotary,
				(InputCollection value) =>
				{
					Player1ScratchLeftRotary = value;
				}
			},
			{
				PlayingKeys.Player1ScratchRightRotary,
				(InputCollection value) =>
				{
					Player1ScratchRightRotary = value;
				}
			},
			{
				PlayingKeys.Player1ScratchToggle,
				(InputCollection value) =>
				{
					Player1ScratchToggle = value;
				}
			},
			{
				PlayingKeys.Player1Start,
				(InputCollection value) =>
				{
					Player1Start = value;
				}
			},
			{
				PlayingKeys.Player1Select,
				(InputCollection value) =>
				{
					Player1Select = value;
				}
			},
			{
				PlayingKeys.Player2Key1,
				(InputCollection value) =>
				{
					Player2Key1 = value;
				}
			},
			{
				PlayingKeys.Player2Key2,
				(InputCollection value) =>
				{
					Player2Key2 = value;
				}
			},
			{
				PlayingKeys.Player2Key3,
				(InputCollection value) =>
				{
					Player2Key3 = value;
				}
			},
			{
				PlayingKeys.Player2Key4,
				(InputCollection value) =>
				{
					Player2Key4 = value;
				}
			},
			{
				PlayingKeys.Player2Key5,
				(InputCollection value) =>
				{
					Player2Key5 = value;
				}
			},
			{
				PlayingKeys.Player2Key6,
				(InputCollection value) =>
				{
					Player2Key6 = value;
				}
			},
			{
				PlayingKeys.Player2Key7,
				(InputCollection value) =>
				{
					Player2Key7 = value;
				}
			},
			{
				PlayingKeys.Player2Key8,
				(InputCollection value) =>
				{
					Player2Key8 = value;
				}
			},
			{
				PlayingKeys.Player2Key9,
				(InputCollection value) =>
				{
					Player2Key9 = value;
				}
			},
			{
				PlayingKeys.Player2ScratchLeft,
				(InputCollection value) =>
				{
					Player2ScratchLeft = value;
				}
			},
			{
				PlayingKeys.Player2ScratchRight,
				(InputCollection value) =>
				{
					Player2ScratchRight = value;
				}
			},
			{
				PlayingKeys.Player2ScratchLeftRotary,
				(InputCollection value) =>
				{
					Player2ScratchLeftRotary = value;
				}
			},
			{
				PlayingKeys.Player2ScratchRightRotary,
				(InputCollection value) =>
				{
					Player2ScratchRightRotary = value;
				}
			},
			{
				PlayingKeys.Player2ScratchToggle,
				(InputCollection value) =>
				{
					Player2ScratchToggle = value;
				}
			},
			{
				PlayingKeys.Player2Start,
				(InputCollection value) =>
				{
					Player2Start = value;
				}
			},
			{
				PlayingKeys.Player2Select,
				(InputCollection value) =>
				{
					Player2Select = value;
				}
			}
		};
	}

	public PlayingKeyConfig ConvertToSinglePlay()
	{
		PlayingKeyConfig playingKeyConfig = new PlayingKeyConfig();
		playingKeyConfig.Player1Key1 = new InputCollection(Player1Key1, Player2Key1);
		playingKeyConfig.Player1Key2 = new InputCollection(Player1Key2, Player2Key2);
		playingKeyConfig.Player1Key3 = new InputCollection(Player1Key3, Player2Key3);
		playingKeyConfig.Player1Key4 = new InputCollection(Player1Key4, Player2Key4);
		playingKeyConfig.Player1Key5 = new InputCollection(Player1Key5, Player2Key5);
		playingKeyConfig.Player1Key6 = new InputCollection(Player1Key6, Player2Key6);
		playingKeyConfig.Player1Key7 = new InputCollection(Player1Key7, Player2Key7);
		playingKeyConfig.Player1Key8 = new InputCollection(Player1Key8, Player2Key8);
		playingKeyConfig.Player1Key9 = new InputCollection(Player1Key9, Player2Key9);
		playingKeyConfig.Player1ScratchLeft = new InputCollection(Player1ScratchLeft, Player2ScratchLeft);
		playingKeyConfig.Player1ScratchRight = new InputCollection(Player1ScratchRight, Player2ScratchRight);
		playingKeyConfig.Player1ScratchLeftRotary = new InputCollection(Player1ScratchLeftRotary, Player2ScratchLeftRotary);
		playingKeyConfig.Player1ScratchRightRotary = new InputCollection(Player1ScratchRightRotary, Player2ScratchRightRotary);
		playingKeyConfig.Player1ScratchToggle = new InputCollection(Player1ScratchToggle, Player2ScratchToggle);
		playingKeyConfig.Player1Start = new InputCollection(Player1Start, Player2Start);
		playingKeyConfig.Player1Select = new InputCollection(Player1Select, Player2Select);
		return playingKeyConfig;
	}
}
