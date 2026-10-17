namespace MBMPlay.Common;

public class GraphicsSettings
{
	public DirectX9Settings DirectX9Settings { get; set; } = new DirectX9Settings();

	public DirectX11Settings DirectX11Settings { get; set; } = new DirectX11Settings();

	public DirectX12Settings DirectX12Settings { get; set; } = new DirectX12Settings();

	public VulkanSettings VulkanSettings { get; set; } = new VulkanSettings();

	public GraphicsSettings()
	{
	}

	public GraphicsSettings(GraphicsSettings settings)
	{
		DirectX9Settings = new DirectX9Settings(settings.DirectX9Settings);
		DirectX11Settings = new DirectX11Settings(settings.DirectX11Settings);
		DirectX12Settings = new DirectX12Settings(settings.DirectX12Settings);
		VulkanSettings = new VulkanSettings(settings.VulkanSettings);
	}
}
