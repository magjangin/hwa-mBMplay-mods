using System.Drawing;
using MistGraphics;

namespace MBMPlay;

public class SkinDestination
{
	private Rectangle _dstRect;

	private int _a = 255;

	private int _r = 255;

	private int _g = 255;

	private int _b = 255;

	private ColorVector? _color;

	public int Time { get; set; }

	public ref Rectangle DestinationRectangle => ref _dstRect;

	public int X
	{
		get
		{
			return _dstRect.X;
		}
		set
		{
			_dstRect.X = value;
		}
	}

	public int Y
	{
		get
		{
			return _dstRect.Y;
		}
		set
		{
			_dstRect.Y = value;
		}
	}

	public int Width
	{
		get
		{
			return _dstRect.Width;
		}
		set
		{
			_dstRect.Width = value;
		}
	}

	public int Height
	{
		get
		{
			return _dstRect.Height;
		}
		set
		{
			_dstRect.Height = value;
		}
	}

	public Color DrawColor => Color.FromArgb(A, R, G, B);

	public int A
	{
		get
		{
			return _a;
		}
		set
		{
			_a = value;
			_color = null;
		}
	}

	public int R
	{
		get
		{
			return _r;
		}
		set
		{
			_r = value;
			_color = null;
		}
	}

	public int G
	{
		get
		{
			return _g;
		}
		set
		{
			_g = value;
			_color = null;
		}
	}

	public int B
	{
		get
		{
			return _b;
		}
		set
		{
			_b = value;
			_color = null;
		}
	}

	internal ColorVector ColorVector
	{
		get
		{
			if (!_color.HasValue)
			{
				_color = ColorVector.FromColor(R, G, B, A);
			}
			return _color.Value;
		}
	}

	public double Rotate { get; set; }

	public PivotType PivotXType { get; set; } = PivotType.Absolute;

	public double PivotX { get; set; }

	public PivotType PivotYType { get; set; } = PivotType.Absolute;

	public double PivotY { get; set; }

	public RoundMethod RoundMethodX { get; set; } = RoundMethod.Ceiling;

	public RoundMethod RoundMethodY { get; set; } = RoundMethod.Ceiling;

	public RoundMethod RoundMethodWidth { get; set; } = RoundMethod.Ceiling;

	public RoundMethod RoundMethodHeight { get; set; } = RoundMethod.Ceiling;

	public SkinDestination()
	{
	}

	public SkinDestination(SkinDestination destination)
	{
		Time = destination.Time;
		_dstRect = destination._dstRect;
		_a = destination._a;
		_r = destination._r;
		_g = destination._g;
		_b = destination._b;
		Rotate = destination.Rotate;
		PivotXType = destination.PivotXType;
		PivotX = destination.PivotX;
		PivotYType = destination.PivotYType;
		PivotY = destination.PivotY;
		RoundMethodX = destination.RoundMethodX;
		RoundMethodY = destination.RoundMethodY;
		RoundMethodWidth = destination.RoundMethodWidth;
		RoundMethodHeight = destination.RoundMethodHeight;
	}
}
