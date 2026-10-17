namespace MBMPlay.Common;

public class DirectX9Settings
{
	[ValueRange(1, 4)]
	public int BufferCount { get; set; } = 2;

	public DirectX9Settings()
	{
	}

	public DirectX9Settings(DirectX9Settings settings)
	{
		BufferCount = settings.BufferCount;
	}
}
