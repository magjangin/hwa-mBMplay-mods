namespace MBMPlay;

public static class SkinItemTypeExtension
{
	public static bool Contains(this SkinItemType value, SkinItemType skinItemType)
	{
		return (value & skinItemType) == skinItemType;
	}
}
