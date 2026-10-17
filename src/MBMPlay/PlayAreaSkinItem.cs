using System;
using System.Drawing;

namespace MBMPlay;

public sealed class PlayAreaSkinItem : SkinItemGroupBase
{
	private int _side = 1;

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

	public int Value
	{
		get
		{
			if (Side != 2)
			{
				return Numbers.Player1Lift;
			}
			return Numbers.Player2Lift;
		}
	}

	public int RangeX { get; set; }

	public int RangeY { get; set; }

	public PlayAreaSkinItem(Skin skin)
		: base(skin)
	{
	}

	public override Rectangle GetDestinationRectangle(double time, Point offset)
	{
		Rectangle destinationRectangle = base.GetDestinationRectangle(time, offset);
		double num = (double)Value / 10000.0;
		if (num < 0.0)
		{
			num = 0.0;
		}
		else if (num > 1.0)
		{
			num = 1.0;
		}
		destinationRectangle.X += (int)Math.Floor((double)RangeX * num);
		destinationRectangle.Y += (int)Math.Floor((double)RangeY * num);
		return destinationRectangle;
	}
}
