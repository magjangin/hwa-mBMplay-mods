using System.Collections.Generic;

namespace MBMPlay.Common;

public class CustomOption
{
	public string ID { get; set; }

	public string Name { get; set; }

	public string Value { get; set; }

	public Dictionary<string, CustomOptionItem> Items { get; set; } = new Dictionary<string, CustomOptionItem>();

	public bool CheckValue(string value)
	{
		if (!(value == Value))
		{
			return false;
		}
		return Items.ContainsKey(value);
	}

	public CustomOptionValue GetCustomOptionValue()
	{
		return new CustomOptionValue
		{
			ID = ID,
			Value = Value
		};
	}
}
