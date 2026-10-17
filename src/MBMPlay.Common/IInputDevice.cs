namespace MBMPlay.Common;

public interface IInputDevice
{
	bool IsKeyDownOnce();

	bool IsKeyDown();

	bool IsKeyUpOnce();

	bool IsKeyUp();
}
