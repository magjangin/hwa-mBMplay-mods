namespace MBMPlay.Common;

public class DirectX12Settings
{
	[ValueRange(2, 16)]
	public int BufferCount { get; set; } = 2;

	[ValueRange(1, int.MaxValue)]
	public int MaxObjects { get; set; } = 65536;

	[ValueRange(1, int.MaxValue)]
	public int MaxTextures { get; set; } = 256;

	[ValueRange(-1, int.MaxValue)]
	public int MaxDynamicTextureUpdateThreads { get; set; } = -1;

	public DirectX12Settings()
	{
	}

	public DirectX12Settings(DirectX12Settings settings)
	{
		BufferCount = settings.BufferCount;
		MaxObjects = settings.MaxObjects;
		MaxTextures = settings.MaxTextures;
		MaxDynamicTextureUpdateThreads = settings.MaxDynamicTextureUpdateThreads;
	}
}
