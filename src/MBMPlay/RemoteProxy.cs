using System;
using BeMusic;

namespace MBMPlay;

[Serializable]
public class RemoteProxy
{
	public string BmsPath { get; set; } = "";

	public int StartBar { get; set; }

	public bool IsStopRequested { get; set; }

	public LaneSwapOption? Player1LaneSwapOption { get; set; }

	public LaneSwapOption? Player2LaneSwapOption { get; set; }

	public bool? FlipSide { get; set; }

	public void Reset()
	{
		BmsPath = "";
		StartBar = 0;
		IsStopRequested = false;
		Player1LaneSwapOption = null;
		Player2LaneSwapOption = null;
		FlipSide = null;
	}
}
