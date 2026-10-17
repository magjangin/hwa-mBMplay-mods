using System.Collections.Generic;
using System.Xml.Serialization;

namespace MBMPlay.Common;

[XmlRoot("SkinOptions")]
public class SkinOptions
{
	[XmlElement("Hash")]
	public string Hash { get; set; }

	[XmlElement(Type = typeof(CustomOptionValue))]
	[XmlElement(Type = typeof(CustomFileValue))]
	public List<SkinOptionValue> OptionValues { get; set; }

	public SkinOptions()
	{
	}

	public SkinOptions(SkinOptions skinOptions)
	{
		Hash = skinOptions.Hash;
		if (skinOptions.OptionValues == null)
		{
			return;
		}
		OptionValues = new List<SkinOptionValue>();
		foreach (SkinOptionValue optionValue in skinOptions.OptionValues)
		{
			if (optionValue == null)
			{
				continue;
			}
			if (!(optionValue is CustomOptionValue customOptionValue))
			{
				if (optionValue is CustomFileValue customFileValue)
				{
					CustomFileValue customFileValue2 = customFileValue;
					OptionValues.Add(new CustomFileValue(customFileValue2));
				}
			}
			else
			{
				CustomOptionValue customOptionValue2 = customOptionValue;
				OptionValues.Add(new CustomOptionValue(customOptionValue2));
			}
		}
	}
}
