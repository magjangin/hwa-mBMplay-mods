using System;
using System.Drawing;
using MistGraphics;

namespace MBMPlay;

public class SliderSkinItem : ImageSkinItemBase
{
	public string NumberID { get; set; }

	public double Value => Numbers.GetValueDouble(NumberID);

	public int Max { get; set; } = 100;

	public int Min { get; set; }

	public int StartX { get; set; }

	public int StartY { get; set; }

	public int EndX { get; set; }

	public int EndY { get; set; }

	public SliderSkinItem(Skin skin)
		: base(skin)
	{
	}

	public override Rectangle GetDestinationRectangle(double time, Point offset)
	{
		Rectangle destinationRectangle = base.GetDestinationRectangle(time, offset);
		double num = (Value - (double)Min) / (double)(Max - Min);
		if (num < 0.0)
		{
			num = 0.0;
		}
		else if (num > 1.0)
		{
			num = 1.0;
		}
		destinationRectangle.X += StartX + (int)Math.Floor((double)(EndX - StartX) * num);
		destinationRectangle.Y += StartY + (int)Math.Floor((double)(EndY - StartY) * num);
		return destinationRectangle;
	}

	public void Draw(GraphicsBase graphics, TextureInfo texture, double time, Point offset, in ColorVector color)
	{
		ColorVector color2 = GetDestinationColorVector(time) * color;
		graphics.Draw(texture, GetSourceRectangle(time), GetDestinationRectangle(time, offset), in color2, (float)GetDestinationRotate(time), (float)GetDestinationPivotX(time), (float)GetDestinationPivotY(time));
	}

	public void Draw(GraphicsBase graphics, TextureInfo texture, double time)
	{
		Draw(graphics, texture, time, Point.Empty, in ColorVector.White);
	}
}
