using System;
using System.Threading;
using MistUtils;

namespace MBMPlay.Platform;

public class ExclusiveProcess : IDisposable
{
	private Mutex _mutex;

	public ExclusiveProcess(string name, out bool createdNew)
	{
		_mutex = new Mutex(initiallyOwned: true, name, out createdNew);
	}

	public void Dispose()
	{
		Utils.Free(_mutex);
	}
}
