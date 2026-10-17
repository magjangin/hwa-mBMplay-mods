namespace MBMPlay.Common;

public class CustomOptionValue : SkinOptionValue
{
	public override SkinOptionType SkinOptionType => SkinOptionType.CustomOption;

	public CustomOptionValue()
	{
	}

	public CustomOptionValue(CustomOptionValue customOptionValue)
	{
		ID = customOptionValue.ID;
		Value = customOptionValue.Value;
	}
}
