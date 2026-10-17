using System;

namespace MBMPlay.Common;

public class JoystickSettings
{
	public int LocalID { get; set; } = -1;

	public Guid Guid { get; set; } = Guid.Empty;

	public string Name { get; set; } = "";

	public int DeadZone { get; set; } = 10;

	public int AnalogOnThreshold { get; set; } = 60;

	public int AnalogOffThreshold { get; set; } = 40;

	public JoystickSettings()
	{
	}

	public JoystickSettings(JoystickSettings settings)
	{
		LocalID = settings.LocalID;
		Guid = settings.Guid;
		Name = settings.Name;
		DeadZone = settings.DeadZone;
		AnalogOnThreshold = settings.AnalogOnThreshold;
		AnalogOffThreshold = settings.AnalogOffThreshold;
	}
}
