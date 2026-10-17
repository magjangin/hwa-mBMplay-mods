namespace MBMPlay;

public class ControlSkinItemFlags
{
	public bool IsMouseHover { get; set; }

	public bool IsMouseOut { get; set; }

	public bool IsButtonDown { get; set; }

	public bool GetValue(string name)
	{
		switch (name)
		{
		case "mouse_hover":
		case "mouse_enter":
			return IsMouseHover;
		case "mouse_out":
			return IsMouseOut;
		case "pressed":
		case "mouse_down":
			return IsButtonDown;
		case "normal":
			return !IsMouseHover;
		case "initial":
			if (!IsMouseHover)
			{
				return !IsMouseOut;
			}
			return false;
		default:
			return false;
		}
	}
}
