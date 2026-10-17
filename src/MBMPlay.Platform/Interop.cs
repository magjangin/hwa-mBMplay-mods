using System.Runtime.InteropServices;

namespace MBMPlay.Platform;

internal static class Interop
{
	[DllImport("winmm.dll")]
	internal static extern uint timeBeginPeriod(uint uPeriod);

	[DllImport("winmm.dll")]
	internal static extern uint timeEndPeriod(uint uPeriod);
}
