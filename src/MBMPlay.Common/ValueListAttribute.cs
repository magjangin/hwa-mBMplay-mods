using System;

namespace MBMPlay.Common;

public class ValueListAttribute : Attribute
{
	public int[] Values { get; set; }

	public ValueListAttribute(params int[] values)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		Values = values;
	}
}
