using System.Xml.Serialization;
using MistInput;

namespace MBMPlay.Common;

public class GeneralKeyConfig
{
	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection Exit { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection DisplayFps { get; set; }

	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection ScreenShot { get; set; }

	public static GeneralKeyConfig Default
	{
		get
		{
			GeneralKeyConfig generalKeyConfig = new GeneralKeyConfig();
			generalKeyConfig.Exit = new InputCollection
			{
				new KeyboardInput(Key.Escape)
			};
			generalKeyConfig.DisplayFps = new InputCollection
			{
				new KeyboardInput(Key.F7)
			};
			generalKeyConfig.ScreenShot = new InputCollection
			{
				new KeyboardInput(Key.F6)
			};
			return generalKeyConfig;
		}
	}

	public GeneralKeyConfig()
	{
	}

	public GeneralKeyConfig(GeneralKeyConfig keyConfig)
	{
		if (keyConfig != null)
		{
			if (keyConfig.Exit != null)
			{
				Exit = new InputCollection(keyConfig.Exit);
			}
			if (keyConfig.DisplayFps != null)
			{
				DisplayFps = new InputCollection(keyConfig.DisplayFps);
			}
			if (keyConfig.ScreenShot != null)
			{
				ScreenShot = new InputCollection(keyConfig.ScreenShot);
			}
		}
	}
}
