namespace MBMPlay;

public interface IControlSkinItem : IDrawableSkinItem, ISkinItem
{
	bool IsActive { get; }

	bool Update(double time, bool isMouseProcessed);
}
