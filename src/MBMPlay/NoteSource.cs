using System;
using System.Drawing;
using MistGraphics;

namespace MBMPlay;

public class NoteSource
{
	private Rectangle _srcRect;

	private int _a = 255;

	private int _r = 255;

	private int _g = 255;

	private int _b = 255;

	private ColorVector? _color;

	private WeakReference<Skin> skinRef;

	public string TextureID { get; set; }

	public ref Rectangle SourceRectangle => ref _srcRect;

	public int X
	{
		get
		{
			return _srcRect.X;
		}
		set
		{
			_srcRect.X = value;
		}
	}

	public int Y
	{
		get
		{
			return _srcRect.Y;
		}
		set
		{
			_srcRect.Y = value;
		}
	}

	public int Width
	{
		get
		{
			return _srcRect.Width;
		}
		set
		{
			_srcRect.Width = value;
		}
	}

	public int Height
	{
		get
		{
			return _srcRect.Height;
		}
		set
		{
			_srcRect.Height = value;
		}
	}

	public int DivisionX { get; set; } = 1;

	public int DivisionY { get; set; } = 1;

	public Color BaseColor => Color.FromArgb(A, R, G, B);

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

	public int Cycle { get; set; }

	public string DivisionTimer { get; set; } = "scene_start";

	public Skin Skin
	{
		get
		{
			if (skinRef == null)
			{
				return null;
			}
			if (!skinRef.TryGetTarget(out var target))
			{
				return null;
			}
			return target;
		}
	}

	public NoteSource(Skin skin)
	{
		skinRef = new WeakReference<Skin>(skin);
	}

	public Rectangle GetSourceRectangle(double time)
	{
		if (DivisionX <= 1 && DivisionY <= 1)
		{
			return SourceRectangle;
		}
		Rectangle result = default;
		int num = 0;
		if (Cycle > 0)
		{
			num = (int)Math.Floor((time - GetTimerValue()) % (double)Cycle / (double)Cycle * (double)(DivisionX * DivisionY));
		}
		result.Width = Width / DivisionX;
		result.Height = Height / DivisionY;
		result.X = X + result.Width * (num % DivisionX);
		result.Y = Y + result.Height * (num / DivisionX);
		return result;
	}

	private double GetTimerValue()
	{
		if (DivisionTimer.StartsWith("$_", StringComparison.Ordinal))
		{
			return Skin?.GetTimerValue(DivisionTimer) ?? 0.0;
		}
		return EventTimers.GetValue(DivisionTimer);
	}
}
