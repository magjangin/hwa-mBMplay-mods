using System;
using System.Collections.Generic;
using System.Drawing;
using MistGraphics;
using MistInput;

namespace MBMPlay;

public class PanelSkinItem : MultipleDestinationSkinItem, ISingleDestinationSkinItem, IDrawableSkinItem, ISkinItem, ISkinItemGroup, IControlSkinItemGroup, IControlSkinItem
{
	public class PanelTimers
	{
		public double PanelOpen { get; set; }

		public double PanelClose { get; set; }

		public double GetValue(string name)
		{
			if (!(name == "open"))
			{
				if (name == "close")
				{
					return PanelClose;
				}
				return 0.0;
			}
			return PanelOpen;
		}
	}

	public class PanelFlags
	{
		public bool PanelOpen { get; set; }

		public bool GetValue(string name)
		{
			if (!(name == "open"))
			{
				if (name == "close")
				{
					return !PanelOpen;
				}
				return false;
			}
			return PanelOpen;
		}
	}

	public static readonly string DefaultPanelGroupName = "#default";

	private int activeControlIndex = -1;

	private List<WeakReference<IControlSkinItem>> controlRefs = new List<WeakReference<IControlSkinItem>>();

	public string PanelGroup { get; set; } = DefaultPanelGroupName;

	public PanelTimers Timers { get; set; } = new PanelTimers();

	public PanelFlags Flags { get; set; } = new PanelFlags();

	public bool AllowFallThrough { get; set; }

	public bool KeepOpen { get; set; }

	public bool IsActive { get; private set; }

	public SkinDestinationContainer Destination
	{
		get
		{
			return Destinations[Flags.PanelOpen ? 1 : 0];
		}
		set
		{
			Destinations[Flags.PanelOpen ? 1 : 0] = value;
		}
	}

	public virtual List<ISkinItem> Items { get; set; } = new List<ISkinItem>();

	public PanelSkinItem(Skin skin)
		: base(skin)
	{
		Destinations.Add(0, new SkinDestinationContainer());
		Destinations.Add(1, new SkinDestinationContainer());
	}

	public void Open(double time)
	{
		if (!Flags.PanelOpen)
		{
			Timers.PanelOpen = time;
			Timers.PanelClose = 0.0;
			Flags.PanelOpen = true;
		}
	}

	public void Close(double time)
	{
		if (Flags.PanelOpen)
		{
			Timers.PanelClose = time;
			Timers.PanelOpen = 0.0;
			Flags.PanelOpen = false;
		}
	}

	public Rectangle GetDestinationRectangle(double time, Point offset)
	{
		return GetDestinationRectangle(Flags.PanelOpen ? 1 : 0, time, offset);
	}

	public Color GetDestinationColor(double time)
	{
		return GetDestinationColor(Flags.PanelOpen ? 1 : 0, time);
	}

	public ColorVector GetDestinationColorVector(double time)
	{
		return GetDestinationColorVector(Flags.PanelOpen ? 1 : 0, time);
	}

	public bool CheckDisplayed(double time)
	{
		return CheckDisplayed(Flags.PanelOpen ? 1 : 0, time);
	}

	public void AddControlReference(IControlSkinItem control)
	{
		if (control != null)
		{
			controlRefs.Add(new WeakReference<IControlSkinItem>(control));
		}
	}

	public bool Update(double time, bool isMouseProcessed)
	{
		IsActive = false;
		bool subControlProcessed;
		if (Flags.PanelOpen)
		{
			int num = activeControlIndex;
			activeControlIndex = -1;
			subControlProcessed = isMouseProcessed;
			if (num >= 0 && num < controlRefs.Count)
			{
				UpdateControl(num);
			}
			for (int num2 = controlRefs.Count - 1; num2 >= 0; num2--)
			{
				if (num2 != num)
				{
					UpdateControl(num2);
				}
			}
			if (subControlProcessed)
			{
				return !AllowFallThrough;
			}
			if (GetDestinationRectangle(time, GetOffset(time)).Contains(InputDevices.ScaledMousePosition))
			{
				return !AllowFallThrough;
			}
			if (InputDevices.Mouse.IsKeyUpOnce(MouseInput.LeftClick))
			{
				Skin.PanelManager.RequestClose(ID);
			}
		}
		return false;
		void UpdateControl(int index)
		{
			if (controlRefs[index].TryGetTarget(out var target))
			{
				bool flag = target.Update(time, subControlProcessed);
				if (flag)
				{
					subControlProcessed = flag;
				}
				if (target.IsActive)
				{
					if (activeControlIndex < 0)
					{
						activeControlIndex = index;
					}
					IsActive = true;
				}
			}
		}
	}
}
