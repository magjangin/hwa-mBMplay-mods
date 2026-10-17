using System.Collections.Generic;

namespace MBMPlay;

public sealed class LaneSkinItem : SingleDestinationSkinItem
{
	public int LaneIndex { get; set; }

	public Dictionary<string, NoteSource> NoteSources { get; set; } = new Dictionary<string, NoteSource>();

	public LaneSkinItem(Skin skin)
		: base(skin)
	{
	}
}
