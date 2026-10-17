using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MBMPlay.Common;
using MBMPlay.Platform;
using MistGraphics;
using MistInput;
using MistUtils;

namespace MBMPlay;

internal static class InputDevices
{
	private static Point _mousePosition = Point.Empty;

	internal static InputManager InputManager;

	internal static Keyboard Keyboard;

	internal static Dictionary<int, Joystick> Joysticks = new Dictionary<int, Joystick>();

	internal static Mouse Mouse;

	internal static Point MousePosition
	{
		get
		{
			return _mousePosition;
		}
		set
		{
			_mousePosition = value;
			int x = (int)((double)_mousePosition.X * (1.0 / Program.WindowScale));
			int y = (int)((double)_mousePosition.Y * (1.0 / Program.WindowScale));
			ScaledMousePosition = new Point(x, y);
		}
	}

	internal static Point ScaledMousePosition { get; private set; } = Point.Empty;

	internal static bool IsMouseCursorInWindow { get; private set; } = false;

	internal static void Init(GraphicsWindow window)
	{
		InputManager = PlatformGeneral.CreateInputManager(window);
		Keyboard = InputManager.CreateKeyboardDevice();
		List<JoystickInfo> joyStickInfo = InputManager.GetJoyStickInfo();
		for (int i = 0; i < joyStickInfo.Count; i++)
		{
			int num = -1;
			int index = -1;
			bool flag = false;
			for (int j = 0; j < Program.Settings.Joysticks.Count; j++)
			{
				if (Program.Settings.Joysticks[j].Guid == joyStickInfo[i].Guid)
				{
					Program.Settings.Joysticks[j].Name = joyStickInfo[i].Name;
					num = Program.Settings.Joysticks[j].LocalID;
					index = j;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				int k;
				for (k = 0; Program.Settings.Joysticks.Any((JoystickSettings s) => s.LocalID == k); k++)
				{
				}
				Program.Settings.Joysticks.Add(new JoystickSettings
				{
					LocalID = k,
					Guid = joyStickInfo[i].Guid,
					Name = joyStickInfo[i].Name
				});
				num = k;
				index = Program.Settings.Joysticks.Count - 1;
			}
			if (num >= 0)
			{
				Joysticks.Add(num, InputManager.CreateJoyStickDevice(joyStickInfo[i]));
				Joysticks[num].DeadZone = Program.Settings.Joysticks[index].DeadZone;
				Joysticks[num].AnalogOnThreshold = Program.Settings.Joysticks[index].AnalogOnThreshold;
				Joysticks[num].AnalogOffThreshold = Program.Settings.Joysticks[index].AnalogOffThreshold;
			}
		}
		Mouse = InputManager.CreateMouseDevice();
		Mouse.AllowBackgroundUpdate = false;
	}

	internal static void Update(GraphicsWindow window)
	{
		Keyboard.Update(window.Focused);
		foreach (KeyValuePair<int, Joystick> joystick in Joysticks)
		{
			joystick.Value.Update(window.Focused);
		}
		Mouse.Update(window.Focused);
	}

	internal static void UpdateMousePosition(GraphicsWindow window)
	{
		if (window != null)
		{
			MousePosition = window.MousePosition;
			IsMouseCursorInWindow = window.IsMouseCursorInWindow;
		}
	}

	internal static void Free()
	{
		Utils.Free(Keyboard);
		Utils.Free(Joysticks);
		Utils.Free(Mouse);
		Utils.Free(InputManager);
	}
}
