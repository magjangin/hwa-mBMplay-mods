using System.Collections.Generic;
using System.Linq;
using MistInput;

namespace MBMPlay.Common;

public class KeyboardKeyCollection : KeyCollection<Key>
{
	public KeyboardKeyCollection()
	{
	}

	public KeyboardKeyCollection(KeyboardKeyCollection keys)
		: base((IList<Key>)keys)
	{
	}

	public KeyboardKeyCollection(params Key[] keys)
		: base((IList<Key>)keys)
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
			return InputDevices.Keyboard.IsKeyDown(base[0]);
		}
		return InputDevices.Keyboard.IsKeyDownAll(this.ToArray());
	}

	public override bool IsKeyDownOnce()
	{
		if (Count == 0)
		{
			return false;
		}
		if (Count == 1)
		{
			return InputDevices.Keyboard.IsKeyDownOnce(base[0]);
		}
		if (IsKeyDown())
		{
			return InputDevices.Keyboard.IsKeyDownOnceAny(this.ToArray());
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
			return InputDevices.Keyboard.IsKeyUp(base[0]);
		}
		return InputDevices.Keyboard.IsKeyUpAny(this.ToArray());
	}

	public override bool IsKeyUpOnce()
	{
		if (Count == 0)
		{
			return false;
		}
		if (Count == 1)
		{
			return InputDevices.Keyboard.IsKeyUpOnce(base[0]);
		}
		return InputDevices.Keyboard.IsKeyUpOnceAny(this.ToArray());
	}
}
