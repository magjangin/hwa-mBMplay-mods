using System.Collections.Generic;

namespace MBMPlay;

public interface ISkinItemGroup : IDrawableSkinItem, ISkinItem
{
	List<ISkinItem> Items { get; set; }
}
