using System;
using System.Drawing;
using MistGraphics;

namespace MBMPlay;

public sealed class NumberSkinItem : NumberSkinItemBase
{
	public NumberSkinItem(Skin skin)
		: base(skin)
	{
	}

	public void Draw(GraphicsBase graphics, TextureInfo texture, double time, Point offset, in ColorVector color)
	{
		ColorVector color2 = GetDestinationColorVector(time) * color;
		int value = Value;
		int numberDigit = GetNumberDigit();
		int num;
		if (NumberType == 10 && Digit != 0)
		{
			num = Math.Min(Math.Abs(Digit), numberDigit);
		}
		else if (NumberType == 11 && Digit != 0)
		{
			num = Math.Abs(Digit);
		}
		else if (NumberType == 24 && Digit != 0)
		{
			num = Math.Abs(Digit) + 1;
		}
		else
		{
			num = ((NumberType != 24 || Digit != 0 || value >= 0) ? numberDigit : (numberDigit + 1));
		}
		int num2 = Math.Abs(value);
		for (int i = 0; i < num; i++)
		{
			int num3;
			if (NumberType == 24 && (Digit != 0 || (Digit == 0 && value < 0)) && i == num - 1)
			{
				num3 = 11;
			}
			else if (i >= numberDigit)
			{
				num3 = 10;
			}
			else
			{
				num3 = Math.Abs(num2) % 10;
				num2 = (num2 - num3) / 10;
			}
			if (NumberType == 24 && value < 0)
			{
				num3 += 12;
			}
			Rectangle dstRectangle = GetDestinationRectangle(time, i, offset);
			graphics.Draw(texture, GetSourceRectangle(time, num3), in dstRectangle, in color2, (float)GetDestinationRotate(time), (float)GetDestinationPivotX(time, num, i), (float)GetDestinationPivotY(time));
		}
	}

	public void Draw(GraphicsBase graphics, TextureInfo texture, double time)
	{
		Draw(graphics, texture, time, Point.Empty, in ColorVector.White);
	}
}
