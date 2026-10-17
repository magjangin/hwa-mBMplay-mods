using System.Drawing;

namespace MBMPlay;

public interface ISingleDestinationSkinItem : IDrawableSkinItem, ISkinItem
{
	SkinDestinationContainer Destination { get; set; }

	Rectangle GetDestinationRectangle(double time, Point offset);

	Color GetDestinationColor(double time);

	bool CheckDisplayed(double time);
}
