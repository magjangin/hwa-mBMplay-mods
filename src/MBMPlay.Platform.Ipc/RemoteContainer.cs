using System;

namespace MBMPlay.Platform.Ipc;

public class RemoteContainer<T> : MarshalByRefObject
{
	public T Data { get; set; }

	public RemoteContainer(T data)
	{
		Data = data;
	}
}
