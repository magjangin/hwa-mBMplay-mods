using System.Xml.Serialization;

namespace MBMPlay.Common;

public class MultiDevicesInput : InputDevice
{
	public override InputDeviceType InputDeviceType => InputDeviceType.MultiDevices;

	[XmlElement(ElementName = "KeyboardInput", Type = typeof(KeyboardInput))]
	[XmlElement(ElementName = "JoystickButtonInput", Type = typeof(JoystickButtonInput))]
	[XmlElement(ElementName = "MouseButtonInput", Type = typeof(MouseButtonInput))]
	[XmlElement(ElementName = "PlayingKeyInput", Type = typeof(PlayingKeyInput))]
	public InputCollection InputDevices { get; set; }

	public MultiDevicesInput()
	{
		InputDevices = new InputCollection();
	}

	public MultiDevicesInput(MultiDevicesInput input)
	{
		InputDevices = ((input != null) ? new InputCollection(input.InputDevices) : new InputCollection());
	}

	public MultiDevicesInput(params InputDevice[] devices)
	{
		InputDevices = new InputCollection();
		if (devices != null)
		{
			for (int i = 0; i < devices.Length; i++)
			{
				InputDevices.Add(devices[i]);
			}
		}
	}

	public override bool IsKeyDown()
	{
		if (InputDevices == null || InputDevices.Count == 0)
		{
			return false;
		}
		for (int i = 0; i < InputDevices.Count; i++)
		{
			if (!InputDevices[i].IsKeyDown())
			{
				return false;
			}
		}
		return true;
	}

	public override bool IsKeyDownOnce()
	{
		if (IsKeyDown())
		{
			return IsKeyDownOnceAny();
		}
		return false;
	}

	private bool IsKeyDownOnceAny()
	{
		if (InputDevices == null || InputDevices.Count == 0)
		{
			return false;
		}
		for (int i = 0; i < InputDevices.Count; i++)
		{
			if (InputDevices[i].IsKeyDownOnce())
			{
				return true;
			}
		}
		return false;
	}

	public override bool IsKeyUp()
	{
		if (InputDevices == null || InputDevices.Count == 0)
		{
			return false;
		}
		for (int i = 0; i < InputDevices.Count; i++)
		{
			if (!InputDevices[i].IsKeyUp())
			{
				return false;
			}
		}
		return true;
	}

	public override bool IsKeyUpOnce()
	{
		if (IsKeyUp())
		{
			return IsKeyUpOnceAny();
		}
		return false;
	}

	private bool IsKeyUpOnceAny()
	{
		if (InputDevices == null || InputDevices.Count == 0)
		{
			return false;
		}
		for (int i = 0; i < InputDevices.Count; i++)
		{
			if (InputDevices[i].IsKeyUpOnce())
			{
				return true;
			}
		}
		return false;
	}
}
