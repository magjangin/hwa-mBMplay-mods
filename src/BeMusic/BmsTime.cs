namespace BeMusic;

public class BmsTime
{
	public double Time { get; private set; }

	public string Value { get; private set; }

	public BmsObjectType ObjectType { get; private set; }

	public BmsTime(double time, string value, BmsObjectType type)
	{
		Time = time;
		Value = value;
		ObjectType = type;
	}

	public BmsTime(BmsTime bmsTime, double musicSpeed = 1.0)
	{
		Time = bmsTime.Time * (1.0 / musicSpeed);
		string text = bmsTime.Value;
		if (bmsTime.ObjectType == BmsObjectType.Bpm && double.TryParse(text, out var result))
		{
			text = (result * musicSpeed).ToString();
		}
		Value = text;
		ObjectType = bmsTime.ObjectType;
	}
}
