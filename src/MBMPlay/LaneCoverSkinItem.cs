using System;
using System.Drawing;
using MistGraphics;

namespace MBMPlay;

public sealed class LaneCoverSkinItem : ImageSkinItemBase
{
	private int _side = 1;

	public static readonly int Max = 10000;

	public static readonly int Min = 0;

	public int Side
	{
		get
		{
			return _side;
		}
		set
		{
			if (value >= 2)
			{
				_side = 2;
			}
			else
			{
				_side = 1;
			}
		}
	}

	public string NumberID { get; private set; }

	public int Value
	{
		get
		{
			if (Side != 2)
			{
				return Numbers.Player1LaneCover;
			}
			return Numbers.Player2LaneCover;
		}
	}

	public int StartX { get; set; }

	public int StartY { get; set; }

	public int EndX { get; set; }

	public int EndY { get; set; }

	public int RangeX => Math.Abs(EndX - StartX);

	public int RangeY => Math.Abs(EndY - StartY);

	public LaneCoverSkinItem(Skin skin)
		: base(skin)
	{
	}

	public override Rectangle GetDestinationRectangle(double time, Point offset)
	{
		Rectangle destinationRectangle = base.GetDestinationRectangle(time, offset);
		int num = ((Side == 2) ? Numbers.Player2Lift : Numbers.Player1Lift);
		double num2 = (double)(((num + Value > Max) ? (Max - num) : Value) - Min) / (double)Max;
		if (num2 < 0.0)
		{
			num2 = 0.0;
		}
		else if (num2 > 1.0)
		{
			num2 = 1.0;
		}
		destinationRectangle.X += StartX + (int)Math.Floor((double)(EndX - StartX) * num2);
		destinationRectangle.Y += StartY + (int)Math.Floor((double)(EndY - StartY) * num2);
		return destinationRectangle;
	}

	public void Draw(GraphicsBase graphics, TextureInfo texture, double time, Point offset, in ColorVector color)
	{
		ColorVector color2 = GetDestinationColorVector(time) * color;
		graphics.Draw(texture, GetSourceRectangle(time), GetDestinationRectangle(time, offset), in color2, (float)GetDestinationRotate(time), (float)GetDestinationPivotX(time), (float)GetDestinationPivotY(time));
	}

	public override bool CheckDisplayed(double time)
	{
		if (Side == 1 && !Flags.Player1LaneCoverEnabled)
		{
			return false;
		}
		if (Side == 2 && !Flags.Player2LaneCoverEnabled)
		{
			return false;
		}
		return base.CheckDisplayed(time);
	}
}
