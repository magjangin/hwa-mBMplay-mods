using System;
using MistInput;

namespace MBMPlay;

public class ButtonSkinItem : SkinItemGroupBase, IControlSkinItem, IDrawableSkinItem, ISkinItem
{
	private double nextKeyRepeat;

	private Action<double> commandAction;

	public override SkinItemType ItemType => SkinItemType.Drawable | SkinItemType.Group | SkinItemType.Control;

	public ControlSkinItemFlags Flags { get; set; } = new ControlSkinItemFlags();

	public ControlSkinItemTimers Timers { get; set; } = new ControlSkinItemTimers();

	public bool IsKeyRepeatEnable { get; set; }

	public double KeyRepeatStartTime { get; set; } = 500.0;

	public double KeyRepeatCycle { get; set; } = 100.0;

	public bool AllowFallThrough { get; set; }

	public bool IsActive { get; private set; }

	public string Command { get; set; }

	internal Action<double> CommandAction
	{
		set
		{
			commandAction = value;
		}
	}

	public ButtonSkinItem(Skin skin)
		: base(skin)
	{
	}

	public void ExecuteCommand(double time)
	{
		commandAction?.Invoke(time);
	}

	public bool Update(double time, bool isMouseProcessed)
	{
		IsActive = false;
		if (!isMouseProcessed)
		{
			if (GetDestinationRectangle(time, GetOffset(time)).Contains(InputDevices.ScaledMousePosition))
			{
				if (!Flags.IsMouseHover)
				{
					Timers.MouseEnter = time;
					Timers.MouseOut = 0.0;
				}
				Flags.IsMouseHover = true;
				Flags.IsMouseOut = false;
				if (!Flags.IsButtonDown)
				{
					if (InputDevices.Mouse.IsKeyDownOnce(MouseInput.LeftClick))
					{
						Flags.IsButtonDown = true;
						Timers.MouseDown = time;
						Timers.MouseUp = 0.0;
						if (IsKeyRepeatEnable)
						{
							nextKeyRepeat = Timers.MouseDown + KeyRepeatStartTime;
							ExecuteCommand(time);
						}
						IsActive = true;
					}
				}
				else if (InputDevices.Mouse.IsKeyUpOnce(MouseInput.LeftClick))
				{
					Flags.IsButtonDown = false;
					Timers.MouseDown = 0.0;
					Timers.MouseUp = time;
					if (!IsKeyRepeatEnable)
					{
						ExecuteCommand(time);
					}
					else
					{
						nextKeyRepeat = 0.0;
					}
				}
				else if (IsKeyRepeatEnable && time >= nextKeyRepeat)
				{
					nextKeyRepeat += KeyRepeatCycle;
					ExecuteCommand(time);
				}
				return !AllowFallThrough;
			}
			if (Flags.IsMouseHover)
			{
				Timers.MouseEnter = 0.0;
				Timers.MouseOut = time;
				Flags.IsMouseHover = false;
				Flags.IsMouseOut = true;
			}
			Flags.IsButtonDown = false;
			Timers.MouseDown = 0.0;
			Timers.MouseUp = 0.0;
		}
		else
		{
			if (Flags.IsButtonDown)
			{
				Timers.MouseDown = 0.0;
				Timers.MouseUp = time;
				Flags.IsButtonDown = false;
			}
			if (Flags.IsMouseHover)
			{
				Timers.MouseEnter = 0.0;
				Timers.MouseOut = time;
				Flags.IsMouseHover = false;
				Flags.IsMouseOut = true;
			}
			if (IsKeyRepeatEnable)
			{
				nextKeyRepeat = 0.0;
			}
		}
		return false;
	}
}
