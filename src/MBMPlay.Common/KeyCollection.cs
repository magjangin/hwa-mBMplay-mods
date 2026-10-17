using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MBMPlay.Common;

public abstract class KeyCollection<T> : Collection<T>, IInputDevice
{
	public KeyCollection()
	{
	}

	public KeyCollection(IList<T> list)
		: base((IList<T>)((list != null) ? new List<T>(list) : new List<T>()))
	{
	}

	public abstract bool IsKeyDownOnce();

	public abstract bool IsKeyDown();

	public abstract bool IsKeyUpOnce();

	public abstract bool IsKeyUp();
}
