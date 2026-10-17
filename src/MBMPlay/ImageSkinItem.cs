using System.Drawing;
using MistGraphics;

namespace MBMPlay;

public sealed class ImageSkinItem : ImageSkinItemBase
{
	public ImageSkinItem(Skin skin)
		: base(skin)
	{
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
