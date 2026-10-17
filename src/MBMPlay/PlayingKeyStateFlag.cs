namespace MBMPlay;

internal class PlayingKeyStateFlag
{
	public bool IsKeyDown;

	public bool IsKeyUp;

	public PlayingKeyStateFlag(bool keyDown, bool keyUp)
	{
		IsKeyDown = keyDown;
		IsKeyUp = keyUp;
	}
}
