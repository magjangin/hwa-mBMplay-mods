using System.Xml.Serialization;
using MistInput;

namespace MBMPlay.Common;

public class PreloadSceneKeyConfig
{
	[XmlArrayItem(Type = typeof(KeyboardInput))]
	[XmlArrayItem(Type = typeof(PlayingKeyInput))]
	[XmlArrayItem(Type = typeof(JoystickButtonInput))]
	[XmlArrayItem(Type = typeof(MouseButtonInput))]
	[XmlArrayItem(Type = typeof(MultiDevicesInput))]
	public InputCollection OpenFileDialog { get; set; }

	public static PreloadSceneKeyConfig Default
	{
		get
		{
			PreloadSceneKeyConfig preloadSceneKeyConfig = new PreloadSceneKeyConfig();
			preloadSceneKeyConfig.OpenFileDialog = new InputCollection
			{
				new KeyboardInput(Key.Control, Key.O)
			};
			return preloadSceneKeyConfig;
		}
	}

	public PreloadSceneKeyConfig()
	{
	}

	public PreloadSceneKeyConfig(PreloadSceneKeyConfig keyConfig)
	{
		if (keyConfig != null && keyConfig.OpenFileDialog != null)
		{
			OpenFileDialog = new InputCollection(keyConfig.OpenFileDialog);
		}
	}
}
