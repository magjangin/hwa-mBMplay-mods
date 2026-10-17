namespace BeMusic;

public static class LaneSwapOptionExtension
{
	public static bool IsRandomize(this LaneSwapOption option)
	{
		if ((uint)(option - 1) <= 11u)
		{
			return true;
		}
		return false;
	}

	public static bool IsRandomizePerLane(this LaneSwapOption option)
	{
		if ((uint)(option - 1) <= 6u)
		{
			return true;
		}
		return false;
	}

	public static bool IsRandomizePerNote(this LaneSwapOption option)
	{
		if ((uint)(option - 8) <= 4u)
		{
			return true;
		}
		return false;
	}
}
