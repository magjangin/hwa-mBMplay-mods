using System;
using System.Runtime.Remoting;

namespace MBMPlay.Platform.Ipc;

public class IpcCiient<T> : IDisposable
{
	private RemoteContainer<T> _container;

	public IpcCiient(string name)
	{
		_container = RemotingServices.Connect(typeof(RemoteContainer<T>), "ipc://" + name + "/RemoteProxy") as RemoteContainer<T>;
	}

	public void SendData(T data)
	{
		if (_container != null)
		{
			_container.Data = data;
		}
	}

	public void Dispose()
	{
		RemotingServices.Disconnect(_container);
	}
}
