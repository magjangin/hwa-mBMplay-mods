using System;

namespace MBMPlay.Common;

[Obsolete]
public class OpenALSettings
{
	public bool ForceResampling { get; set; }

	public int DefaultFrequency { get; set; } = 44100;

	public int DefaultBitsPerSample { get; set; } = 16;

	public int DefaultChannels { get; set; } = 2;

	public bool HaveSourcePerBuffer { get; set; }

	public int MaxSourceCount { get; set; } = 256;

	public int Latency { get; set; } = 8;

	public OpenALSettings()
	{
	}

	public OpenALSettings(OpenALSettings settings)
	{
		ForceResampling = settings.ForceResampling;
		DefaultFrequency = settings.DefaultFrequency;
		DefaultBitsPerSample = settings.DefaultBitsPerSample;
		DefaultChannels = settings.DefaultChannels;
		HaveSourcePerBuffer = settings.HaveSourcePerBuffer;
		MaxSourceCount = settings.MaxSourceCount;
		Latency = settings.Latency;
	}
}
