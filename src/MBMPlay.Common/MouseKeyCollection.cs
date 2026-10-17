using System.Collections.Generic;
using System.Linq;
using MistInput;

namespace MBMPlay.Common;

public class MouseKeyCollection : KeyCollection<MouseInput>
{
	public MouseKeyCollection()
	{
	}

	public MouseKeyCollection(MouseKeyCollection keys)
		: base((IList<MouseInput>)keys)
	{
	}

	public MouseKeyCollection(params MouseInput[] keys)
		: base((IList<MouseInput>)keys)
	{
	}

	public override bool IsKeyDown()
	{
		if (Count == 0)
		{
			return false;
		}
		if (Count == 1)
		{
			return InputDevices.Mouse.IsKeyDown(base[0]);
		}
		return InputDevices.Mouse.IsKeyDownAll(this.ToArray());
	}

	public override bool IsKeyDownOnce()
	{
		if (Count == 0)
		{
			return false;
		}
		if (Count == 1)
		{
			return InputDevices.Mouse.IsKeyDownOnce(base[0]);
		}
		if (IsKeyDown())
		{
			return InputDevices.Mouse.IsKeyDownOnceAny(this.ToArray());
		}
		return false;
	}

	public override bool IsKeyUp()
	{
		if (Count == 0)
		{
			return false;
		}
		if (Count == 1)
		{
			return InputDevices.Mouse.IsKeyUp(base[0]);
		}
		return InputDevices.Mouse.IsKeyUpAny(this.ToArray());
	}

	public override bool IsKeyUpOnce()
	{
		if (Count == 0)
		{
			return false;
		}
		if (Count == 1)
		{
			return InputDevices.Mouse.IsKeyUpOnce(base[0]);
		}
		return InputDevices.Mouse.IsKeyUpOnceAny(this.ToArray());
	}
}
