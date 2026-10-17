using System.Xml.Serialization;
using MistInput;

namespace MBMPlay.Common;

public class MouseButtonInput : InputDevice
{
	public override InputDeviceType InputDeviceType => InputDeviceType.Mouse;

	[XmlElement(ElementName = "Key", Type = typeof(MouseInput))]
	public MouseKeyCollection KeyList { get; set; }

	public MouseButtonInput()
	{
		KeyList = new MouseKeyCollection();
	}

	public MouseButtonInput(MouseButtonInput input)
	{
		KeyList = ((input != null) ? new MouseKeyCollection(input.KeyList) : new MouseKeyCollection());
	}

	public MouseButtonInput(params MouseInput[] keys)
	{
		KeyList = new MouseKeyCollection();
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
		if (!InputDevices.IsMouseCursorInWindow)
		{
			return false;
		}
		return KeyList.IsKeyDown();
	}

	public override bool IsKeyDownOnce()
	{
		if (!InputDevices.IsMouseCursorInWindow)
		{
			return false;
		}
		return KeyList.IsKeyDownOnce();
	}

	public override bool IsKeyUp()
	{
		if (!InputDevices.IsMouseCursorInWindow)
		{
			return true;
		}
		return KeyList.IsKeyUp();
	}

	public override bool IsKeyUpOnce()
	{
		if (!InputDevices.IsMouseCursorInWindow)
		{
			return false;
		}
		return KeyList.IsKeyUpOnce();
	}
}
