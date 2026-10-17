using System.Collections.Generic;

namespace MBMPlay;

internal class BarStartInfo
{
	public double Bpm { get; set; }

	public double Time { get; set; }

	public Dictionary<string, double> Keys { get; set; } = new Dictionary<string, double>();

	public Dictionary<string, double> Bgm { get; set; } = new Dictionary<string, double>();

	public List<int> LNHolding { get; set; } = new List<int>();

	public string Bga { get; set; } = "";

	public double BgaPosition { get; set; }

	public string Layer { get; set; } = "";

	public double LayerPosition { get; set; }

	public string Poor { get; set; } = "";

	public double PoorPosition { get; set; }
}
