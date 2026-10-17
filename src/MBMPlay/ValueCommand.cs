using System;

namespace MBMPlay;

public class ValueCommand<T> where T : struct
{
	public Func<T> GetValue { get; private set; }

	public Action<T> SetValue { get; private set; }

	public ValueCommand(Func<T> getter, Action<T> setter)
	{
		GetValue = getter;
		SetValue = setter;
	}
}
