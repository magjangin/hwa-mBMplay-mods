namespace MBMPlay.Common;

public class DirectX11Settings
{
	[ValueRange(1, 16)]
	public int BufferCount { get; set; } = 2;

	public DirectX11Settings()
	{
	}

	public DirectX11Settings(DirectX11Settings settings)
	{
		BufferCount = settings.BufferCount;
	}
}
