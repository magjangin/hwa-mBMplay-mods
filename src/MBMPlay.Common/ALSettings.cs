using System.Threading;

namespace MBMPlay.Common;

public class ALSettings
{
	[ValueList(new int[] { 8000, 11025, 16000, 22050, 32000, 44100, 48000, 88200, 96000, 192000 })]
	public int DefaultFrequency { get; set; } = 44100;

	[ValueList(new int[] { 8, 16 })]
	public int DefaultBitsPerSample { get; set; } = 16;

	[ValueRange(1, 16)]
	public int DefaultChannels { get; set; } = 2;

	[ValueRange(1, 65536)]
	public int BufferSize { get; set; } = 1024;

	[ValueRange(1, 256)]
	public int BufferCount { get; set; } = 3;

	public ThreadPriority PlayingThreadPriority { get; set; } = ThreadPriority.Normal;

	public ALSettings()
	{
	}

	public ALSettings(ALSettings settings)
	{
		DefaultFrequency = settings.DefaultFrequency;
		DefaultBitsPerSample = settings.DefaultBitsPerSample;
		DefaultChannels = settings.DefaultChannels;
		BufferSize = settings.BufferSize;
		BufferCount = settings.BufferCount;
		PlayingThreadPriority = settings.PlayingThreadPriority;
	}
}
