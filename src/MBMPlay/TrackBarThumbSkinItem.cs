using System;
using System.Drawing;

namespace MBMPlay;

public class TrackBarThumbSkinItem : SkinItemGroupBase
{
	protected readonly WeakReference<TrackBarSkinItem> trackBarRef;

	public override SkinItemType ItemType => SkinItemType.Drawable | SkinItemType.Group | SkinItemType.Control;

	public virtual TrackBarSkinItem TrackBar
	{
		get
		{
			if (trackBarRef == null)
			{
				return null;
			}
			if (!trackBarRef.TryGetTarget(out var target))
			{
				return null;
			}
			return target;
		}
	}

	public override ISkinItemGroup ParentGroup
	{
		get
		{
			return null;
		}
		internal set
		{
		}
	}

	public double Value { get; set; }

	public ControlSkinItemFlags Flags { get; set; } = new ControlSkinItemFlags();

	public ControlSkinItemTimers Timers { get; set; } = new ControlSkinItemTimers();

	public TrackBarThumbSkinItem(Skin skin, TrackBarSkinItem trackBar)
		: base(skin)
	{
		if (trackBar != null)
		{
			trackBarRef = new WeakReference<TrackBarSkinItem>(trackBar);
		}
	}

	public override Rectangle GetDestinationRectangle(double time, Point offset)
	{
		Rectangle destinationRectangle = base.GetDestinationRectangle(time, offset);
		TrackBarSkinItem trackBar = TrackBar;
		if (trackBar != null)
		{
			Rectangle destinationRectangle2 = trackBar.GetDestinationRectangle(time, Point.Empty);
			Point cornerPoint = trackBar.GetCornerPoint(destinationRectangle2, trackBar.StartPosition);
			Point cornerPoint2 = trackBar.GetCornerPoint(destinationRectangle2, trackBar.EndPosition);
			int num = cornerPoint2.X - cornerPoint.X;
			int num2 = cornerPoint2.Y - cornerPoint.Y;
			double num3 = (Value - trackBar.Min) / (trackBar.Max - trackBar.Min);
			if (num > 0)
			{
				destinationRectangle.X += (int)Math.Floor((double)(num - destinationRectangle.Width) * num3);
			}
			else if (num < 0)
			{
				destinationRectangle.X += (int)Math.Floor((double)(num * -1 - destinationRectangle.Width) * (1.0 - num3));
			}
			if (num2 > 0)
			{
				destinationRectangle.Y += (int)Math.Floor((double)(num2 - destinationRectangle.Height) * num3);
			}
			else if (num2 < 0)
			{
				destinationRectangle.Y += (int)Math.Floor((double)(num2 * -1 - destinationRectangle.Height) * (1.0 - num3));
			}
		}
		return destinationRectangle;
	}

	public override Point GetOffset(double time)
	{
		if (trackBarRef == null)
		{
			return Point.Empty;
		}
		TrackBarSkinItem trackBar = TrackBar;
		if (trackBar != null)
		{
			Rectangle destinationRectangle = trackBar.GetDestinationRectangle(time, Point.Empty);
			int num = destinationRectangle.X;
			int num2 = destinationRectangle.Y;
			ISkinItemGroup parentGroup = trackBar.ParentGroup;
			if (parentGroup != null)
			{
				do
				{
					if (parentGroup is ISingleDestinationSkinItem singleDestinationSkinItem)
					{
						Rectangle destinationRectangle2 = singleDestinationSkinItem.GetDestinationRectangle(time, Point.Empty);
						num += destinationRectangle2.X;
						num2 += destinationRectangle2.Y;
					}
					parentGroup = parentGroup.ParentGroup;
				}
				while (parentGroup != null);
			}
			return new Point(num, num2);
		}
		return Point.Empty;
	}
}
