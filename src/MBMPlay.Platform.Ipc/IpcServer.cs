using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting.Lifetime;

namespace MBMPlay.Platform.Ipc;

public class IpcServer<T> : IDisposable
{
	private IpcChannel _ipc;

	private RemoteContainer<T> _container;

	public T Data
	{
		get
		{
			if (_container != null)
			{
				return _container.Data;
			}
			return default;
		}
	}

	public string Name { get; private set; }

	public IpcServer(string name, T data)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		Name = name;
		_container = new RemoteContainer<T>(data);
		LifetimeServices.LeaseTime = TimeSpan.Zero;
		LifetimeServices.RenewOnCallTime = TimeSpan.Zero;
		_ipc = new IpcChannel(name);
		ChannelServices.RegisterChannel(_ipc, ensureSecurity: false);
		RemotingServices.Marshal(_container, "RemoteProxy");
	}

	public void Dispose()
	{
		ChannelServices.UnregisterChannel(_ipc);
	}
}
