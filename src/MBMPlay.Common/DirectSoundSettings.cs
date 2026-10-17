namespace MBMPlay.Common;

public class DirectSoundSettings
{
	public bool ForceResampling { get; set; }

	[ValueList(new int[] { 8000, 11025, 16000, 22050, 32000, 44100, 48000, 88200, 96000, 192000 })]
	public int DefaultFrequency { get; set; } = 44100;

	[ValueList(new int[] { 8, 16, 24, 32 })]
	public int DefaultBitsPerSample { get; set; } = 16;

	[ValueRange(1, 16)]
	public int DefaultChannels { get; set; } = 2;

	public DirectSoundSettings()
	{
	}

	public DirectSoundSettings(DirectSoundSettings settings)
	{
		ForceResampling = settings.ForceResampling;
		DefaultFrequency = settings.DefaultFrequency;
		DefaultBitsPerSample = settings.DefaultBitsPerSample;
		DefaultChannels = settings.DefaultChannels;
	}
}
