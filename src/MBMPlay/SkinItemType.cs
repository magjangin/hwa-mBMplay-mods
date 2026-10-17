using System;

namespace MBMPlay;

[Flags]
public enum SkinItemType
{
	Command = 1,
	Drawable = 2,
	Group = 4,
	Control = 8
}
