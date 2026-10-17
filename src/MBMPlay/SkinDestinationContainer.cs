using System.Collections.Generic;
using MistGraphics;

namespace MBMPlay;

public class SkinDestinationContainer
{
	public List<SkinDestination> DestinationList { get; set; } = new List<SkinDestination>();

	public string DestinationTimer { get; set; } = "scene_start";

	public int DestinationLoop { get; set; }

	public BlendType Blend { get; set; }

	public SamplerType Filter { get; set; }

	public List<string> Conditions { get; set; } = new List<string>();
}
