using System;

namespace BeMusic.Extentions;

internal static class FloatExtensions
{
	public static bool EpsilonEquals(this float f1, float f2)
	{
		return Math.Abs(f1 - f2) < float.Epsilon;
	}

	public static bool EpsilonEquals(this double d1, double d2)
	{
		return Math.Abs(d1 - d2) < double.Epsilon;
	}
}
