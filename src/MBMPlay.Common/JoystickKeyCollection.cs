using System;
using System.Collections.Generic;
using System.Linq;
using MistInput;

namespace MBMPlay.Common;

public class JoystickKeyCollection : KeyCollection<JoystickInput>
{
	public JoystickKeyCollection()
	{
	}

	public JoystickKeyCollection(JoystickKeyCollection keys)
		: base((IList<JoystickInput>)keys)
	{
	}

	public JoystickKeyCollection(params JoystickInput[] keys)
		: base((IList<JoystickInput>)keys)
	{
	}

	public bool IsKeyDown(int id)
	{
		if (Count == 0)
		{
			return false;
		}
		if (!InputDevices.Joysticks.ContainsKey(id))
		{
			return false;
		}
		if (Count == 1)
		{
			return InputDevices.Joysticks[id].IsKeyDown(base[0]);
		}
		return InputDevices.Joysticks[id].IsKeyDownAll(this.ToArray());
	}

	public bool IsKeyDownOnce(int id)
	{
		if (Count == 0)
		{
			return false;
		}
		if (!InputDevices.Joysticks.ContainsKey(id))
		{
			return false;
		}
		if (Count == 1)
		{
			return InputDevices.Joysticks[id].IsKeyDownOnce(base[0]);
		}
		if (IsKeyDown())
		{
			return InputDevices.Joysticks[id].IsKeyDownOnceAny(this.ToArray());
		}
		return false;
	}

	public bool IsKeyUp(int id)
	{
		if (Count == 0)
		{
			return false;
		}
		if (!InputDevices.Joysticks.ContainsKey(id))
		{
			return false;
		}
		if (Count == 1)
		{
			return InputDevices.Joysticks[id].IsKeyUp(base[0]);
		}
		return InputDevices.Joysticks[id].IsKeyUpAny(this.ToArray());
	}

	public bool IsKeyUpOnce(int id)
	{
		if (Count == 0)
		{
			return false;
		}
		if (!InputDevices.Joysticks.ContainsKey(id))
		{
			return false;
		}
		if (Count == 1)
		{
			return InputDevices.Joysticks[id].IsKeyUpOnce(base[0]);
		}
		return InputDevices.Joysticks[id].IsKeyUpOnceAny(this.ToArray());
	}

	public override bool IsKeyDown()
	{
		throw new NotSupportedException();
	}

	public override bool IsKeyDownOnce()
	{
		throw new NotSupportedException();
	}

	public override bool IsKeyUp()
	{
		throw new NotSupportedException();
	}

	public override bool IsKeyUpOnce()
	{
		throw new NotSupportedException();
	}
}
