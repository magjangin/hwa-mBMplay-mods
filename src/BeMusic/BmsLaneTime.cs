namespace BeMusic;

public class BmsLaneTime : BmsTime
{
	public bool IsJudged { get; set; }

	public bool FastPoor { get; set; }

	public BmsLaneTime(double time, string value, BmsObjectType type)
		: base(time, value, type)
	{
		IsJudged = false;
		FastPoor = false;
	}

	public BmsLaneTime(BmsLaneTime laneTime, double musicSpeed = 1.0)
		: base(laneTime, musicSpeed)
	{
		IsJudged = laneTime.IsJudged;
		FastPoor = laneTime.FastPoor;
	}
}
