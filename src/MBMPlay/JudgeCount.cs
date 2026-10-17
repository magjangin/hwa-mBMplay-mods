namespace MBMPlay;

public class JudgeCount
{
	public int Fast { get; set; }

	public int Slow { get; set; }

	public int Total => Fast + Slow;
}
