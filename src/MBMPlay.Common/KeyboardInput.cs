using System.Xml.Serialization;
using MistInput;

namespace MBMPlay.Common;

public class KeyboardInput : InputDevice
{
	public override InputDeviceType InputDeviceType => InputDeviceType.Keyboard;

	[XmlElement(ElementName = "Key", Type = typeof(Key))]
	public KeyboardKeyCollection KeyList { get; set; }

	public KeyboardInput()
	{
		KeyList = new KeyboardKeyCollection();
	}

	public KeyboardInput(KeyboardInput input)
	{
		KeyList = ((input != null) ? new KeyboardKeyCollection(input.KeyList) : new KeyboardKeyCollection());
	}

	public KeyboardInput(params Key[] keys)
	{
		KeyList = new KeyboardKeyCollection();
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
