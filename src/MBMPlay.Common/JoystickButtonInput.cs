using System.Xml.Serialization;
using MistInput;

namespace MBMPlay.Common;

public class JoystickButtonInput : InputDevice
{
	public override InputDeviceType InputDeviceType => InputDeviceType.Joystick;

	[XmlAttribute("id")]
	public int ID { get; set; }

	[XmlElement(ElementName = "Key", Type = typeof(JoystickInput))]
	public JoystickKeyCollection KeyList { get; set; }

	public JoystickButtonInput()
	{
		KeyList = new JoystickKeyCollection();
	}

	public JoystickButtonInput(JoystickButtonInput input)
	{
		if (input != null)
		{
			KeyList = new JoystickKeyCollection(input.KeyList);
			ID = input.ID;
		}
		else
		{
			KeyList = new JoystickKeyCollection();
		}
	}

	public JoystickButtonInput(int id, params JoystickInput[] keys)
	{
		KeyList = new JoystickKeyCollection();
		ID = id;
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
		return KeyList.IsKeyDown(ID);
	}

	public override bool IsKeyDownOnce()
	{
		return KeyList.IsKeyDownOnce(ID);
	}

	public override bool IsKeyUp()
	{
		return KeyList.IsKeyUp(ID);
	}

	public override bool IsKeyUpOnce()
	{
		return KeyList.IsKeyUpOnce(ID);
	}
}
