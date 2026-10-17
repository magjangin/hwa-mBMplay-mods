using System.Collections.Generic;

namespace MBMPlay;

public abstract class SkinItemGroupBase : SingleDestinationSkinItem, ISkinItemGroup, IDrawableSkinItem, ISkinItem
{
	public override SkinItemType ItemType => SkinItemType.Drawable | SkinItemType.Group;

	public virtual List<ISkinItem> Items { get; set; } = new List<ISkinItem>();

	protected SkinItemGroupBase(Skin skin)
		: base(skin)
	{
	}
}
