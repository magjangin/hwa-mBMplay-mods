using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MBMPlay.Common;

public class InputCollection : Collection<InputDevice>, IInputDevice
{
	public InputCollection()
	{
	}

	public InputCollection(InputCollection inputs)
		: base((IList<InputDevice>)((inputs != null) ? (from input in inputs.Select((InputDevice input) =>
			{
				if (input != null)
				{
					if (input is KeyboardInput input2)
					{
						return new KeyboardInput(input2);
					}
					if (input is JoystickButtonInput input3)
					{
						return new JoystickButtonInput(input3);
					}
					if (input is MouseButtonInput input4)
					{
						return new MouseButtonInput(input4);
					}
					if (input is PlayingKeyInput input5)
					{
						return new PlayingKeyInput(input5);
					}
					if (input is MultiDevicesInput input6)
					{
						return new MultiDevicesInput(input6);
					}
				}
				return (InputDevice)null;
			})
			where input != null
			select input).ToList() : new List<InputDevice>()))
	{
	}

	public InputCollection(params InputCollection[] inputs)
	{
		if (inputs == null)
		{
			return;
		}
		for (int i = 0; i < inputs.Length; i++)
		{
			if (inputs[i] != null)
			{
				for (int j = 0; j < inputs[i].Count; j++)
				{
					Add(inputs[i][j]);
				}
			}
		}
	}

	public bool IsKeyDownOnce()
	{
		if (Count == 0)
		{
			return false;
		}
		for (int i = 0; i < Count; i++)
		{
			if (base[i].IsKeyDownOnce())
			{
				return true;
			}
		}
		return false;
	}

	public bool IsKeyDown()
	{
		if (Count == 0)
		{
			return false;
		}
		for (int i = 0; i < Count; i++)
		{
			if (base[i].IsKeyDown())
			{
				return true;
			}
		}
		return false;
	}

	public bool IsKeyUpOnce()
	{
		if (Count == 0)
		{
			return false;
		}
		if (!IsKeyUp())
		{
			return false;
		}
		for (int i = 0; i < Count; i++)
		{
			if (base[i].IsKeyUpOnce())
			{
				return true;
			}
		}
		return false;
	}

	public bool IsKeyUp()
	{
		if (Count == 0)
		{
			return false;
		}
		for (int i = 0; i < Count; i++)
		{
			if (!base[i].IsKeyUp())
			{
				return false;
			}
		}
		return true;
	}
}
