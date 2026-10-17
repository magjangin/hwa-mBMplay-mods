namespace MBMPlay;

public class ControlSkinItemTimers
{
	public double MouseEnter { get; set; }

	public double MouseOut { get; set; }

	public double MouseDown { get; set; }

	public double MouseUp { get; set; }

	public double GetValue(string name)
	{
		return name switch
		{
			"mouse_enter" => MouseEnter, 
			"mouse_out" => MouseOut, 
			"mouse_down" => MouseDown, 
			"mouse_up" => MouseUp, 
			_ => 0.0, 
		};
	}
}
