using BeMusic;

namespace MBMPlay;

internal class NotePosition
{
	public BmsObjectType ObjectType { get; set; }

	public int X { get; set; }

	public int Y { get; set; }

	public NoteStatus Status { get; set; }

	public NotePosition(BmsObjectType type, int x, int y, NoteStatus status)
	{
		ObjectType = type;
		X = x;
		Y = y;
		Status = status;
	}
}
