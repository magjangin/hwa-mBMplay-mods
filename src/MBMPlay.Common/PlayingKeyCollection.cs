using System.Collections.Generic;
using System.Linq;

namespace MBMPlay.Common;

public class PlayingKeyCollection : KeyCollection<PlayingKeys>
{
	public PlayingKeyCollection()
	{
	}

	public PlayingKeyCollection(PlayingKeyCollection keys)
		: base((IList<PlayingKeys>)keys)
	{
	}

	public PlayingKeyCollection(params PlayingKeys[] keys)
		: base((IList<PlayingKeys>)keys)
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
			return PlayingKeyState.IsKeyDown(base[0]);
		}
		return PlayingKeyState.IsKeyDownAll(this.ToArray());
	}

	public override bool IsKeyDownOnce()
	{
		if (Count == 0)
		{
			return false;
		}
		if (Count == 1)
		{
			return PlayingKeyState.IsKeyDownOnce(base[0]);
		}
		if (IsKeyDown())
		{
			return PlayingKeyState.IsKeyDownOnceAny(this.ToArray());
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
			return PlayingKeyState.IsKeyUp(base[0]);
		}
		return PlayingKeyState.IsKeyUpAny(this.ToArray());
	}

	public override bool IsKeyUpOnce()
	{
		if (Count == 0)
		{
			return false;
		}
		if (Count == 1)
		{
			return PlayingKeyState.IsKeyUpOnce(base[0]);
		}
		return PlayingKeyState.IsKeyUpOnceAny(this.ToArray());
	}
}
