namespace MBMPlay.Common;

public class CustomFileValue : SkinOptionValue
{
	public override SkinOptionType SkinOptionType => SkinOptionType.CustomFile;

	public CustomFileValue()
	{
	}

	public CustomFileValue(CustomFileValue customFileValue)
	{
		ID = customFileValue.ID;
		Value = customFileValue.Value;
	}
}
