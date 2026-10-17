namespace BeMusic;

public class BmsObject
{
	public double Position { get; set; }

	public string Value { get; set; }

	public BmsObjectType ObjectType { get; set; }

	public BmsObject(double position, string value, BmsObjectType type)
	{
		Position = position;
		Value = value;
		ObjectType = type;
	}
}
