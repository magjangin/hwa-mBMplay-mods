using System.Xml.Serialization;

namespace MBMPlay.Common;

public class PlayingKeyInput : InputDevice
{
	public override InputDeviceType InputDeviceType => InputDeviceType.PlayingKey;

	[XmlElement(ElementName = "Key", Type = typeof(PlayingKeys))]
	public PlayingKeyCollection KeyList { get; set; }

	public PlayingKeyInput()
	{
		KeyList = new PlayingKeyCollection();
	}

	public PlayingKeyInput(PlayingKeyInput input)
	{
		KeyList = ((input != null) ? new PlayingKeyCollection(input.KeyList) : new PlayingKeyCollection());
	}

	public PlayingKeyInput(params PlayingKeys[] keys)
	{
		KeyList = new PlayingKeyCollection();
		if (keys != null)
		{
			for (int i = 0; i < keys.Length; i++)
			{
				KeyList.Add(keys[i]);
			}
		}
	}

	public override bool IsKeyDown()
	{
		return KeyList.IsKeyDown();
	}

	public override bool IsKeyDownOnce()
	{
		return KeyList.IsKeyDownOnce();
	}

	public override bool IsKeyUp()
	{
		return KeyList.IsKeyUp();
	}

	public override bool IsKeyUpOnce()
	{
		return KeyList.IsKeyUpOnce();
	}
}
