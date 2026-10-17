using System;

namespace MBMPlay.Common;

public class AudioSettings
{
	public DirectSoundSettings DirectSoundSettings { get; set; } = new DirectSoundSettings();

	public XAudio2Settings XAudio2Settings { get; set; } = new XAudio2Settings();

	public ALSettings ALSettings { get; set; } = new ALSettings();

	public WasapiSettings WasapiSettings { get; set; } = new WasapiSettings();

	[Obsolete]
	public OpenALSettings OpenALSettings { get; set; } = new OpenALSettings();

	public AudioSettings()
	{
	}

	public AudioSettings(AudioSettings settings)
	{
		DirectSoundSettings = new DirectSoundSettings(settings.DirectSoundSettings);
		XAudio2Settings = new XAudio2Settings(settings.XAudio2Settings);
		ALSettings = new ALSettings(settings.ALSettings);
		WasapiSettings = new WasapiSettings(settings.WasapiSettings);
		OpenALSettings = new OpenALSettings(settings.OpenALSettings);
	}
}
