namespace MBMPlay;

public interface IControlSkinItemGroup : IControlSkinItem, IDrawableSkinItem, ISkinItem
{
	void AddControlReference(IControlSkinItem control);
}
