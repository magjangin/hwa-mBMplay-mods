namespace MBMPlay.Common;

public class VulkanSettings
{
	[ValueRange(1, 16)]
	public int BufferCount { get; set; } = 2;

	[ValueRange(1, int.MaxValue)]
	public int MaxObjects { get; set; } = 65536;

	[ValueRange(1, int.MaxValue)]
	public int MaxTextures { get; set; } = 256;

	public VulkanSettings()
	{
	}

	public VulkanSettings(VulkanSettings settings)
	{
		BufferCount = settings.BufferCount;
		MaxObjects = settings.MaxObjects;
		MaxTextures = settings.MaxTextures;
	}
}
