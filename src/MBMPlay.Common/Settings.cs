using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using MistUtils;

namespace MBMPlay.Common;

public class Settings
{
	public static readonly Version SettingsVersion = new Version(3, 24, 824, 0);

	public string SettingsFileVersion { get; set; }

	public bool UpgradeRequired
	{
		get
		{
			if (!Version.TryParse(SettingsFileVersion, out var result))
			{
				return true;
			}
			return SettingsVersion != result;
		}
	}

	public GraphicsType GraphicsType { get; set; }

	public AudioType AudioType { get; set; } = AudioType.XAudio2;

	public bool VSync { get; set; }

	public int TargetUpdateRate { get; set; }

	public ThreadPriority UpdateThreadPriority { get; set; } = ThreadPriority.Normal;

	public WindowPosition WindowStartPosition { get; set; }

	public Point WindowLocation { get; set; } = Point.Empty;

	public int TargetDisplay { get; set; } = -2;

	public double WindowScale { get; set; } = 1.0;

	public FilterLevel FilterLevel { get; set; } = FilterLevel.BySkin;

	public double HiSpeed { get; set; } = 1.0;

	public HiSpeedFixType HiSpeedFixType { get; set; }

	public int MasterVolume { get; set; } = 50;

	public int KeyVolume { get; set; } = 100;

	public int BgmVolume { get; set; } = 100;

	public bool LegacyLNMode { get; set; } = true;

	public PlaySide PlaySide { get; set; }

	public bool Player1LaneCoverEnabled { get; set; }

	public bool Player2LaneCoverEnabled { get; set; }

	public decimal LaneCoverRange { get; set; } = 20m;

	public decimal JudgeLineLift { get; set; }

	public int JudgeTiming { get; set; }

	public int NoteTiming { get; set; }

	public int MinKeyDownTime { get; set; } = 48;

	public int MinKeyUpTime { get; set; } = 16;

	public int ScratchRotaryUpdateInterval { get; set; } = 20;

	public int ScratchRotaryActiveTime { get; set; } = 200;

	public int ScratchRotaryThreshold { get; set; } = 2;

	public bool DisplayFps { get; set; }

	public bool DisplayNoteCount { get; set; }

	public GraphicsSettings GraphicsSettings { get; set; } = new GraphicsSettings();

	public AudioSettings AudioSettings { get; set; } = new AudioSettings();

	public SkinSettings SkinSettings { get; set; } = new SkinSettings();

	public List<JoystickSettings> Joysticks { get; set; } = new List<JoystickSettings>();

	public Settings()
	{
		SettingsFileVersion = Utils.GetVersionString(SettingsVersion);
	}

	public Settings(Settings settings)
	{
		PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
		foreach (PropertyInfo propertyInfo in properties)
		{
			if (propertyInfo.CanWrite && (propertyInfo.PropertyType.IsValueType || propertyInfo.PropertyType == typeof(string)))
			{
				propertyInfo.SetValue(this, propertyInfo.GetValue(settings));
			}
		}
		GraphicsSettings = new GraphicsSettings(settings.GraphicsSettings);
		AudioSettings = new AudioSettings(settings.AudioSettings);
		SkinSettings = new SkinSettings(settings.SkinSettings);
		Joysticks = settings.Joysticks.Select((JoystickSettings s) => new JoystickSettings(s)).ToList();
	}

	public void Upgrade()
	{
		SettingsFileVersion = Utils.GetVersionString(SettingsVersion);
	}
}
