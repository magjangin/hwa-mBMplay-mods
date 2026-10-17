namespace MBMPlay;

internal class PlayingKeyStateTime
{
	public double KeyDownTime;

	public double KeyUpTime;

	public PlayingKeyStateTime(double keyDownTime, double keyUpTime)
	{
		KeyDownTime = keyDownTime;
		KeyUpTime = keyUpTime;
	}
}
