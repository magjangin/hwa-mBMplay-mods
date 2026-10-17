extern alias MistUtilsAlias;

using System;
using System.Drawing;
using MistGraphics;
using MistUtils;
using MistUtilsAlias::MistUtils;

namespace MBMPlay;

public abstract class SingleDestinationSkinItem : ISingleDestinationSkinItem, IDrawableSkinItem, ISkinItem
{
	protected WeakReference<Skin> skinRef;

	protected WeakReference<ISkinItemGroup> parentGroupRef;

	public string ID { get; set; }

	public virtual SkinItemType ItemType => SkinItemType.Drawable;

	public virtual Skin Skin
	{
		get
		{
			if (skinRef == null)
			{
				return null;
			}
			if (!skinRef.TryGetTarget(out var target))
			{
				return null;
			}
			return target;
		}
	}

	public virtual ISkinItemGroup ParentGroup
	{
		get
		{
			if (parentGroupRef == null)
			{
				return null;
			}
			if (!parentGroupRef.TryGetTarget(out var target))
			{
				return null;
			}
			return target;
		}
		internal set
		{
			if (value == null)
			{
				parentGroupRef = null;
			}
			else
			{
				parentGroupRef = new WeakReference<ISkinItemGroup>(value);
			}
		}
	}

	public SkinDestinationContainer Destination { get; set; } = new SkinDestinationContainer();

	protected SingleDestinationSkinItem(Skin skin)
	{
		skinRef = new WeakReference<Skin>(skin);
	}

	protected int Round(double value, RoundMethod method)
	{
		switch (method)
		{
		default:
			return (int)Math.Floor(value);
		case RoundMethod.Ceiling:
			return (int)Math.Ceiling(value);
		case RoundMethod.Truncate:
			return (int)Math.Truncate(value);
		case RoundMethod.Round:
			return (int)Math.Round(value);
		case RoundMethod.AwayFromZero:
			if (value < 0.0)
			{
				return (int)Math.Floor(value);
			}
			return (int)Math.Ceiling(value);
		}
	}

	public virtual Rectangle GetDestinationRectangle(double time, Point offset)
	{
		if (!CheckDisplayed(time))
		{
			return Rectangle.Empty;
		}
		if (Destination.DestinationList.Count == 1)
		{
			return Destination.DestinationList[0].DestinationRectangle.Move(offset);
		}
		double dstTime = GetDstTime(time);
		Rectangle result = default;
		for (int i = 0; i < Destination.DestinationList.Count; i++)
		{
			SkinDestination skinDestination = Destination.DestinationList[i];
			if ((double)skinDestination.Time >= dstTime)
			{
				if (i <= 0)
				{
					return skinDestination.DestinationRectangle.Move(offset);
				}
				SkinDestination skinDestination2 = Destination.DestinationList[i - 1];
				double num = (dstTime - (double)skinDestination2.Time) / (double)(skinDestination.Time - skinDestination2.Time);
				result.X = skinDestination2.X + Round((double)(skinDestination.X - skinDestination2.X) * num, skinDestination.RoundMethodX) + offset.X;
				result.Y = skinDestination2.Y + Round((double)(skinDestination.Y - skinDestination2.Y) * num, skinDestination.RoundMethodY) + offset.Y;
				result.Width = skinDestination2.Width + Round((double)(skinDestination.Width - skinDestination2.Width) * num, skinDestination.RoundMethodWidth);
				result.Height = skinDestination2.Height + Round((double)(skinDestination.Height - skinDestination2.Height) * num, skinDestination.RoundMethodHeight);
				break;
			}
		}
		return result;
	}

	public virtual Color GetDestinationColor(double time)
	{
		if (!CheckDisplayed(time))
		{
			return Color.White;
		}
		if (Destination.DestinationList.Count == 1)
		{
			return Destination.DestinationList[0].DrawColor;
		}
		double dstTime = GetDstTime(time);
		Color result = Color.White;
		for (int i = 0; i < Destination.DestinationList.Count; i++)
		{
			SkinDestination skinDestination = Destination.DestinationList[i];
			if ((double)skinDestination.Time >= dstTime)
			{
				if (i <= 0)
				{
					return skinDestination.DrawColor;
				}
				SkinDestination skinDestination2 = Destination.DestinationList[i - 1];
				double num = (dstTime - (double)skinDestination2.Time) / (double)(skinDestination.Time - skinDestination2.Time);
				int red = skinDestination2.R + (int)Math.Ceiling((double)(skinDestination.R - skinDestination2.R) * num);
				int green = skinDestination2.G + (int)Math.Ceiling((double)(skinDestination.G - skinDestination2.G) * num);
				int blue = skinDestination2.B + (int)Math.Ceiling((double)(skinDestination.B - skinDestination2.B) * num);
				result = Color.FromArgb(skinDestination2.A + (int)Math.Ceiling((double)(skinDestination.A - skinDestination2.A) * num), red, green, blue);
				break;
			}
		}
		return result;
	}

	public virtual ColorVector GetDestinationColorVector(double time)
	{
		if (!CheckDisplayed(time))
		{
			return ColorVector.White;
		}
		if (Destination.DestinationList.Count == 1)
		{
			return Destination.DestinationList[0].ColorVector;
		}
		double dstTime = GetDstTime(time);
		ColorVector result = ColorVector.White;
		for (int i = 0; i < Destination.DestinationList.Count; i++)
		{
			SkinDestination skinDestination = Destination.DestinationList[i];
			if ((double)skinDestination.Time >= dstTime)
			{
				if (i <= 0)
				{
					return skinDestination.ColorVector;
				}
				SkinDestination skinDestination2 = Destination.DestinationList[i - 1];
				float num = ((float)dstTime - (float)skinDestination2.Time) / (float)(skinDestination.Time - skinDestination2.Time);
				result = skinDestination2.ColorVector + (skinDestination.ColorVector - skinDestination2.ColorVector) * num;
				break;
			}
		}
		return result;
	}

	public virtual bool CheckDisplayed(double time)
	{
		if (Destination.DestinationList.Count <= 0)
		{
			return false;
		}
		Skin skin = Skin;
		if (skin == null || !skin.CheckConditions(Destination.Conditions))
		{
			return false;
		}
		double timerValue = GetTimerValue();
		if (timerValue <= 0.0)
		{
			return false;
		}
		if ((double)Destination.DestinationList[0].Time > time - timerValue)
		{
			return false;
		}
		if (Destination.DestinationLoop < 0 && (double)Destination.DestinationList[Destination.DestinationList.Count - 1].Time < time - timerValue)
		{
			return false;
		}
		return true;
	}

	protected virtual double GetTimerValue()
	{
		return GetTimerValue(Destination.DestinationTimer);
	}

	protected virtual double GetTimerValue(string name)
	{
		if (name.StartsWith("$_", StringComparison.Ordinal))
		{
			return Skin?.GetTimerValue(name) ?? 0.0;
		}
		return EventTimers.GetValue(name);
	}

	protected virtual double GetDstTime(double time)
	{
		double num = Destination.DestinationList[Destination.DestinationList.Count - 1].Time;
		double num2 = time - GetTimerValue();
		if (num2 <= num)
		{
			return num2;
		}
		if (num2 >= num && MistUtils.Extensions.EpsilonEquals(num, Destination.DestinationLoop))
		{
			return num;
		}
		return (double)Destination.DestinationLoop + (num2 - num) % (num - (double)Destination.DestinationLoop);
	}

	public virtual Point GetOffset(double time)
	{
		if (parentGroupRef == null)
		{
			return Point.Empty;
		}
		ISkinItemGroup parentGroup = ParentGroup;
		if (parentGroup != null)
		{
			int num = 0;
			int num2 = 0;
			do
			{
				if (parentGroup is ISingleDestinationSkinItem singleDestinationSkinItem)
				{
					Rectangle destinationRectangle = singleDestinationSkinItem.GetDestinationRectangle(time, Point.Empty);
					num += destinationRectangle.X;
					num2 += destinationRectangle.Y;
				}
				parentGroup = parentGroup.ParentGroup;
			}
			while (parentGroup != null);
			return new Point(num, num2);
		}
		return Point.Empty;
	}
}
