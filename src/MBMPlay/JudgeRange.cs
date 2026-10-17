using System;

namespace MBMPlay;

internal class JudgeRange
{
	public double PGreat { get; set; }

	public double Great { get; set; }

	public double Good { get; set; }

	public double Bad { get; set; }

	public double FastPoor { get; set; }

	public double SlowPoor { get; set; }

	public double ScratchRangeScale { get; set; }

	public double LNEndRangeScale { get; set; }

	public double Timeout => Math.Min(Bad * -1.0, SlowPoor);
}
