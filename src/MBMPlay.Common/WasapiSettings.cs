using System.Threading;

namespace MBMPlay.Common;

public class WasapiSettings
{
	[ValueList(new int[] { 8000, 11025, 16000, 22050, 32000, 44100, 48000, 88200, 96000, 192000 })]
	public int DefaultFrequency { get; set; } = 48000;

	[ValueList(new int[] { 8, 16, 24, 32 })]
	public int DefaultBitsPerSample { get; set; } = 16;

	[ValueRange(1, 16)]
	public int DefaultChannels { get; set; } = 2;

	public bool IsExclusive { get; set; }

	[ValueRange(-1, 1000)]
	public int Latency { get; set; } = -1;

	public ThreadPriority PlayingThreadPriority { get; set; } = ThreadPriority.Normal;

	public WasapiSettings()
	{
	}

	public WasapiSettings(WasapiSettings settings)
	{
		DefaultFrequency = settings.DefaultFrequency;
		DefaultBitsPerSample = settings.DefaultBitsPerSample;
		DefaultChannels = settings.DefaultChannels;
		IsExclusive = settings.IsExclusive;
		Latency = settings.Latency;
		PlayingThreadPriority = settings.PlayingThreadPriority;
	}
}
