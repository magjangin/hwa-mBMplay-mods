using System.Collections.Generic;
using System.Drawing;
using MistGraphics;

namespace MBMPlay;

public sealed class BarlineSkinItem : ImageSkinItemBase
{
	private int side = 1;

	public int Side
	{
		get
		{
			return side;
		}
		set
		{
			if (value >= 2)
			{
				side = 2;
			}
			else
			{
				side = 1;
			}
		}
	}

	public BarlineSkinItem(Skin skin)
		: base(skin)
	{
	}

	public void Draw(GraphicsBase graphics, TextureInfo texture, double time, IList<Point> positions, Point offset, in ColorVector color)
	{
		ColorVector color2 = GetDestinationColorVector(time) * color;
		for (int i = 0; i < positions.Count; i++)
		{
			Rectangle dstRectangle = GetDestinationRectangle(time, Point.Empty);
			dstRectangle.X = positions[i].X + offset.X;
			dstRectangle.Y = positions[i].Y + offset.Y;
			graphics.Draw(texture, GetSourceRectangle(time), in dstRectangle, in color2);
		}
	}

	public void Draw(GraphicsBase graphics, TextureInfo texture, double time, IList<Point> positions)
	{
		Draw(graphics, texture, time, positions, Point.Empty, in ColorVector.White);
	}
}
