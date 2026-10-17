using System;
using System.Drawing;
using MistInput;
using MistUtils;

namespace MBMPlay;

public class TrackBarSkinItem : SingleDestinationSkinItem, IControlSkinItem, IDrawableSkinItem, ISkinItem
{
	private double trackBarValue;

	public override SkinItemType ItemType => SkinItemType.Drawable | SkinItemType.Group | SkinItemType.Control;

	public TrackBarThumbSkinItem Thumb { get; internal set; }

	public ControlSkinItemFlags Flags { get; set; } = new ControlSkinItemFlags();

	public ControlSkinItemTimers Timers { get; set; } = new ControlSkinItemTimers();

	public double Max { get; set; } = 100.0;

	public double Min { get; set; }

	public double Step { get; set; }

	public CornerPosition StartPosition { get; set; }

	public CornerPosition EndPosition { get; set; } = CornerPosition.TopRight;

	public double Value
	{
		get
		{
			return trackBarValue;
		}
		set
		{
			double num = value;
			if (num >= Max)
			{
				num = Max;
			}
			else if (num <= Min)
			{
				num = Min;
			}
			trackBarValue = num;
			if (Thumb != null)
			{
				Thumb.Value = trackBarValue;
			}
		}
	}

	public string NumberID { get; set; }

	internal ValueCommand<double> ValueCommand { get; set; }

	public TrackBarApplyTiming ApplyTiming { get; set; } = TrackBarApplyTiming.Release;

	public bool IsReadOnly { get; set; }

	public bool IsActive { get; private set; }

	public TrackBarSkinItem(Skin skin)
		: base(skin)
	{
	}

	public bool Update(double time, bool isMouseProcessed)
	{
		IsActive = false;
		if (!IsReadOnly && !isMouseProcessed && Thumb != null)
		{
			bool result = false;
			Rectangle destinationRectangle = GetDestinationRectangle(time, GetOffset(time));
			Rectangle destinationRectangle2 = Thumb.GetDestinationRectangle(time, destinationRectangle.Location);
			bool flag = InputDevices.Mouse.IsKeyDownOnce(MouseInput.LeftClick);
			bool flag2 = InputDevices.Mouse.IsKeyUpOnce(MouseInput.LeftClick);
			bool flag3 = false;
			if (destinationRectangle2.Contains(InputDevices.ScaledMousePosition))
			{
				if (!Thumb.Flags.IsMouseHover)
				{
					Thumb.Timers.MouseEnter = time;
					Thumb.Timers.MouseOut = 0.0;
				}
				Thumb.Flags.IsMouseHover = true;
				Thumb.Flags.IsMouseOut = false;
				if (!Thumb.Flags.IsButtonDown && flag)
				{
					Thumb.Flags.IsButtonDown = true;
					Thumb.Timers.MouseDown = time;
					Thumb.Timers.MouseUp = 0.0;
				}
				result = true;
			}
			if (Thumb.Flags.IsButtonDown & flag2)
			{
				Thumb.Flags.IsButtonDown = false;
				Thumb.Timers.MouseDown = 0.0;
				Thumb.Timers.MouseUp = time;
				flag3 = true;
			}
			bool flag4 = destinationRectangle.Contains(InputDevices.ScaledMousePosition);
			if (flag4)
			{
				if (!Flags.IsMouseHover)
				{
					Timers.MouseEnter = time;
					Timers.MouseOut = 0.0;
				}
				Flags.IsMouseHover = true;
				Flags.IsMouseOut = false;
				if (!Flags.IsButtonDown && flag)
				{
					Flags.IsButtonDown = true;
					Timers.MouseDown = time;
					Timers.MouseUp = 0.0;
				}
				result = true;
			}
			if (Flags.IsButtonDown & flag2)
			{
				Flags.IsButtonDown = false;
				Timers.MouseDown = 0.0;
				Timers.MouseUp = time;
				flag3 = true;
			}
			if (flag3)
			{
				result = true;
			}
			if (Flags.IsButtonDown && !Thumb.Flags.IsButtonDown)
			{
				Thumb.Flags.IsButtonDown = true;
				Thumb.Timers.MouseDown = time;
				Thumb.Timers.MouseUp = 0.0;
			}
			else if (!Flags.IsButtonDown && Thumb.Flags.IsButtonDown)
			{
				Flags.IsButtonDown = true;
				Timers.MouseDown = time;
				Timers.MouseUp = 0.0;
			}
			if (Flags.IsButtonDown || Thumb.Flags.IsButtonDown)
			{
				Point cornerPoint = GetCornerPoint(destinationRectangle, StartPosition);
				Point cornerPoint2 = GetCornerPoint(destinationRectangle, EndPosition);
				int num = cornerPoint2.X - cornerPoint.X;
				int num2 = cornerPoint2.Y - cornerPoint.Y;
				double num3 = 0.0;
				int num4 = 0;
				if (Math.Abs(num) < Math.Abs(num2))
				{
					int num5 = destinationRectangle.Height - destinationRectangle2.Height;
					if (num5 != 0)
					{
						num4 = InputDevices.ScaledMousePosition.Y - destinationRectangle.Y - destinationRectangle2.Height / 2;
						if (num4 < 0)
						{
							num4 = 0;
						}
						else if (num4 > num5)
						{
							num4 = num5;
						}
						num3 = ((num2 >= 0) ? ((double)num4 / (double)num5 * (Max - Min) + Min) : ((1.0 - (double)num4 / (double)num5) * (Max - Min) + Min));
					}
				}
				else
				{
					int num5 = destinationRectangle.Width - destinationRectangle2.Width;
					if (num5 != 0)
					{
						num4 = InputDevices.ScaledMousePosition.X - destinationRectangle.X - destinationRectangle2.Width / 2;
						if (num4 < 0)
						{
							num4 = 0;
						}
						else if (num4 > num5)
						{
							num4 = num5;
						}
						num3 = ((num >= 0) ? ((double)num4 / (double)num5 * (Max - Min) + Min) : ((1.0 - (double)num4 / (double)num5) * (Max - Min) + Min));
					}
				}
				double value = Value;
				if (Step > 0.0)
				{
					num3 = Math.Round(num3 / Step) * Step;
				}
				Value = num3;
				Extensions.EpsilonEquals(Value, value);
				if (ApplyTiming == TrackBarApplyTiming.Drag)
				{
					ValueCommand?.SetValue?.Invoke(Value);
				}
				destinationRectangle2 = Thumb.GetDestinationRectangle(time, destinationRectangle.Location);
				result = true;
				IsActive = true;
			}
			else if ((ApplyTiming == TrackBarApplyTiming.Release) & flag3)
			{
				ValueCommand?.SetValue?.Invoke(Value);
			}
			else if (ValueCommand?.GetValue != null)
			{
				Value = ValueCommand.GetValue();
			}
			if (!flag4 && !Flags.IsButtonDown && Flags.IsMouseHover)
			{
				Timers.MouseEnter = 0.0;
				Timers.MouseOut = time;
				Flags.IsMouseHover = false;
				Flags.IsMouseOut = true;
			}
			if (!destinationRectangle2.Contains(InputDevices.ScaledMousePosition) && !Thumb.Flags.IsButtonDown && Thumb.Flags.IsMouseHover)
			{
				Thumb.Timers.MouseEnter = 0.0;
				Thumb.Timers.MouseOut = time;
				Thumb.Flags.IsMouseHover = false;
				Thumb.Flags.IsMouseOut = true;
			}
			return result;
		}
		if (!IsReadOnly)
		{
			if (Thumb != null)
			{
				if (Thumb.Flags.IsButtonDown)
				{
					Thumb.Timers.MouseDown = 0.0;
					Thumb.Timers.MouseUp = time;
					Thumb.Flags.IsButtonDown = false;
				}
				if (Thumb.Flags.IsMouseHover)
				{
					Thumb.Timers.MouseEnter = 0.0;
					Thumb.Timers.MouseOut = time;
					Thumb.Flags.IsMouseHover = false;
					Thumb.Flags.IsMouseOut = true;
				}
			}
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
		}
		if (ValueCommand?.GetValue != null)
		{
			Value = ValueCommand.GetValue();
		}
		return false;
	}

	public Point GetCornerPoint(Rectangle rectangle, CornerPosition position)
	{
		return position switch
		{
			CornerPosition.TopLeft => new Point(rectangle.Left, rectangle.Top), 
			CornerPosition.TopRight => new Point(rectangle.Right, rectangle.Top), 
			CornerPosition.BottomLeft => new Point(rectangle.Left, rectangle.Bottom), 
			CornerPosition.BottomRight => new Point(rectangle.Right, rectangle.Bottom), 
			_ => Point.Empty, 
		};
	}
}
