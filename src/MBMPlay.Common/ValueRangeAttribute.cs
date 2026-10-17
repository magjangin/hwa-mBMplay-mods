using System;

namespace MBMPlay.Common;

public class ValueRangeAttribute : Attribute
{
	public int Max { get; set; } = int.MaxValue;

	public int Min { get; set; } = int.MinValue;

	public ValueRangeAttribute(int min, int max)
	{
		Max = max;
		Min = min;
	}
}
