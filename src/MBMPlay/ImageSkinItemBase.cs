using System;
using System.Drawing;

namespace MBMPlay;

public abstract class ImageSkinItemBase : SingleDestinationSkinItem
{
	private Rectangle _srcRect;

	protected int divX = 1;

	protected int divY = 1;

	protected string divTimer = "scene_start";

	public string TextureID { get; set; }

	public ref Rectangle SourceRectangle => ref _srcRect;

	public int X
	{
		get
		{
			return _srcRect.X;
		}
		set
		{
			_srcRect.X = value;
		}
	}

	public int Y
	{
		get
		{
			return _srcRect.Y;
		}
		set
		{
			_srcRect.Y = value;
		}
	}

	public int Width
	{
		get
		{
			return _srcRect.Width;
		}
		set
		{
			_srcRect.Width = value;
		}
	}

	public int Height
	{
		get
		{
			return _srcRect.Height;
		}
		set
		{
			_srcRect.Height = value;
		}
	}

	public int DivisionX
	{
		get
		{
			return divX;
		}
		set
		{
			divX = value;
		}
	}

	public int DivisionY
	{
		get
		{
			return divY;
		}
		set
		{
			divY = value;
		}
	}

	public int Cycle { get; set; }

	public string DivisionTimer
	{
		get
		{
			return divTimer;
		}
		set
		{
			divTimer = value;
		}
	}

	protected ImageSkinItemBase(Skin skin)
		: base(skin)
	{
	}

	public virtual double GetDestinationRotate(double time)
	{
		if (!CheckDisplayed(time))
		{
			return 0.0;
		}
		if (Destination.DestinationList.Count == 1)
		{
			return Destination.DestinationList[0].Rotate;
		}
		double dstTime = GetDstTime(time);
		double result = 0.0;
		for (int i = 0; i < Destination.DestinationList.Count; i++)
		{
			SkinDestination skinDestination = Destination.DestinationList[i];
			if ((double)skinDestination.Time >= dstTime)
			{
				if (i <= 0)
				{
					return skinDestination.Rotate;
				}
				SkinDestination skinDestination2 = Destination.DestinationList[i - 1];
				double num = (dstTime - (double)skinDestination2.Time) / (double)(skinDestination.Time - skinDestination2.Time);
				result = skinDestination2.Rotate + (skinDestination.Rotate - skinDestination2.Rotate) * num;
				break;
			}
		}
		return result;
	}

	public virtual double GetDestinationPivotX(double time)
	{
		if (!CheckDisplayed(time))
		{
			return 0.0;
		}
		if (Destination.DestinationList.Count == 1)
		{
			if (Destination.DestinationList[0].PivotXType != PivotType.Relative)
			{
				return Destination.DestinationList[0].PivotX;
			}
			return (double)Destination.DestinationList[0].Width * Destination.DestinationList[0].PivotX;
		}
		double dstTime = GetDstTime(time);
		double result = 0.0;
		for (int i = 0; i < Destination.DestinationList.Count; i++)
		{
			SkinDestination skinDestination = Destination.DestinationList[i];
			if ((double)skinDestination.Time >= dstTime)
			{
				double num = ((skinDestination.PivotXType == PivotType.Relative) ? ((double)skinDestination.Width * skinDestination.PivotX) : skinDestination.PivotX);
				if (i <= 0)
				{
					return num;
				}
				SkinDestination skinDestination2 = Destination.DestinationList[i - 1];
				double num2 = ((skinDestination2.PivotXType == PivotType.Relative) ? ((double)skinDestination2.Width * skinDestination2.PivotX) : skinDestination2.PivotX);
				double num3 = (dstTime - (double)skinDestination2.Time) / (double)(skinDestination.Time - skinDestination2.Time);
				result = num2 + (num - num2) * num3;
				break;
			}
		}
		return result;
	}

	public virtual double GetDestinationPivotY(double time)
	{
		if (!CheckDisplayed(time))
		{
			return 0.0;
		}
		if (Destination.DestinationList.Count == 1)
		{
			if (Destination.DestinationList[0].PivotYType != PivotType.Relative)
			{
				return Destination.DestinationList[0].PivotY;
			}
			return (double)Destination.DestinationList[0].Height * Destination.DestinationList[0].PivotY;
		}
		double dstTime = GetDstTime(time);
		double result = 0.0;
		for (int i = 0; i < Destination.DestinationList.Count; i++)
		{
			SkinDestination skinDestination = Destination.DestinationList[i];
			if ((double)skinDestination.Time >= dstTime)
			{
				double num = ((skinDestination.PivotYType == PivotType.Relative) ? ((double)skinDestination.Height * skinDestination.PivotY) : skinDestination.PivotY);
				if (i <= 0)
				{
					return skinDestination.PivotY;
				}
				SkinDestination skinDestination2 = Destination.DestinationList[i - 1];
				double num2 = ((skinDestination2.PivotYType == PivotType.Relative) ? ((double)skinDestination2.Height * skinDestination2.PivotY) : skinDestination2.PivotY);
				double num3 = (dstTime - (double)skinDestination2.Time) / (double)(skinDestination.Time - skinDestination2.Time);
				result = num2 + (num - num2) * num3;
				break;
			}
		}
		return result;
	}

	public virtual Rectangle GetSourceRectangle(double time)
	{
		if (DivisionX <= 1 && DivisionY <= 1)
		{
			return SourceRectangle;
		}
		Rectangle result = default;
		int num = 0;
		if (Cycle > 0)
		{
			num = (int)Math.Floor((time - GetTimerValue(DivisionTimer)) % (double)Cycle / (double)Cycle * (double)(DivisionX * DivisionY));
		}
		result.Width = Width / DivisionX;
		result.Height = Height / DivisionY;
		result.X = X + result.Width * (num % DivisionX);
		result.Y = Y + result.Height * (num / DivisionX);
		return result;
	}
}
