using System.Drawing;

namespace MBMPlay;

public interface IDrawableSkinItem : ISkinItem
{
	string ID { get; set; }

	Skin Skin { get; }

	ISkinItemGroup ParentGroup { get; }

	Point GetOffset(double time);
}
