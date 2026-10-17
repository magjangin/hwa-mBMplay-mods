using System.Drawing;
using MistGraphics;

namespace MBMPlay;

public class BarGraphSkinItem : ImageSkinItemBase
{
	public string NumberID { get; set; }

	public double Value => Numbers.GetValueDouble(NumberID);

	public int Max { get; set; } = 100;

	public int Min { get; set; }

	public BarGraphDirection Direction { get; set; } = BarGraphDirection.Right;

	public BarGraphImageMode ImageMode { get; set; }

	public RoundMethod RoundMethod { get; set; } = RoundMethod.Floor;

	public BarGraphSkinItem(Skin skin)
		: base(skin)
	{
	}

	public override Rectangle GetDestinationRectangle(double time, Point offset)
	{
		return GetDestinationRectangle(time, Value, offset);
	}

	public Rectangle GetDestinationRectangle(double time, double value, Point offset)
	{
		Rectangle rect = base.GetDestinationRectangle(time, offset);
		Resize(ref rect, value);
		return rect;
	}

	public override Rectangle GetSourceRectangle(double time)
	{
		return GetSourceRectangle(time, Value);
	}

	public Rectangle GetSourceRectangle(double time, double value)
	{
		Rectangle rect = base.GetSourceRectangle(time);
		if (ImageMode == BarGraphImageMode.Cut)
		{
			Resize(ref rect, value);
		}
		return rect;
	}

	private void Resize(ref Rectangle rect, double value)
	{
		double num = (value - (double)Min) / (double)(Max - Min);
		if (num < 0.0)
		{
			num = 0.0;
		}
		else if (num > 1.0)
		{
			num = 1.0;
		}
		if (Direction == BarGraphDirection.Right || Direction == BarGraphDirection.Left)
		{
			int width = rect.Width;
			rect.Width = Round((double)width * num, RoundMethod);
			if (Direction == BarGraphDirection.Left)
			{
				rect.X += width - rect.Width;
			}
		}
		else if (Direction == BarGraphDirection.Up || Direction == BarGraphDirection.Down)
		{
			int height = rect.Height;
			rect.Height = Round((double)height * num, RoundMethod);
			if (Direction == BarGraphDirection.Up)
			{
				rect.Y += height - rect.Height;
			}
		}
	}

	public void Draw(GraphicsBase graphics, TextureInfo texture, double time, Point offset, in ColorVector color)
	{
		ColorVector color2 = GetDestinationColorVector(time) * color;
		double value = Value;
		graphics.Draw(texture, GetSourceRectangle(time, value), GetDestinationRectangle(time, value, offset), in color2, (float)GetDestinationRotate(time), (float)GetDestinationPivotX(time), (float)GetDestinationPivotY(time));
	}

	public void Draw(GraphicsBase graphics, TextureInfo texture, double time)
	{
		Draw(graphics, texture, time, Point.Empty, in ColorVector.White);
	}
}
