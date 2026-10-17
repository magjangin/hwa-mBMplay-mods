namespace MBMPlay.Common;

public abstract class InputDevice : IInputDevice
{
	public abstract InputDeviceType InputDeviceType { get; }

	public abstract bool IsKeyDownOnce();

	public abstract bool IsKeyDown();

	public abstract bool IsKeyUpOnce();

	public abstract bool IsKeyUp();
}
