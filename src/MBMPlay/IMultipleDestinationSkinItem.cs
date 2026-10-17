using System.Collections.Generic;
using System.Drawing;

namespace MBMPlay;

public interface IMultipleDestinationSkinItem : IDrawableSkinItem, ISkinItem
{
	Dictionary<int, SkinDestinationContainer> Destinations { get; set; }

	Rectangle GetDestinationRectangle(int index, double time, Point offset);

	Color GetDestinationColor(int index, double time);

	bool CheckDisplayed(int index, double time);
}
