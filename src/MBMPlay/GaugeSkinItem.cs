using System;
using System.Drawing;

namespace MBMPlay;

public sealed class GaugeSkinItem : MultipleDestinationSkinItem
{
	private int divX = 1;

	private int divY = 1;

	public string TextureID { get; set; }

	public Rectangle SourceRectangle
	{
		get
		{
			return new Rectangle(X, Y, Width, Height);
		}
		set
		{
			X = value.X;
			Y = value.Y;
			Width = value.Width;
			Height = value.Height;
		}
	}

	public int X { get; set; }

	public int Y { get; set; }

	public int Width { get; set; }

	public int Height { get; set; }

	public int DivisionX
	{
		get
		{
			return divX;
		}
		set
		{
			divX = value;
		}
	}

	public int DivisionY
	{
		get
		{
			return divY;
		}
		set
		{
			divY = value;
		}
	}

	public int Cycle { get; set; }

	public string DivisionTimer { get; set; } = "scene_start";

	public int GaugeDivision { get; set; } = 50;

	public int Border { get; set; } = 40;

	public BarGraphDirection Direction { get; set; } = BarGraphDirection.Right;

	public GaugeSkinItem(Skin skin)
		: base(skin)
	{
	}

	public Rectangle GetDestinationRectangle(int gaugeIndex, int totalIndex, double time, Point offset)
	{
		if (gaugeIndex < 0 || gaugeIndex > totalIndex)
		{
			return Rectangle.Empty;
		}
		Rectangle destinationRectangle = GetDestinationRectangle(GetContainerIndex(gaugeIndex, totalIndex), time, offset);
		switch (Direction)
		{
		case BarGraphDirection.Right:
			destinationRectangle.X += destinationRectangle.Width * gaugeIndex;
			break;
		case BarGraphDirection.Left:
			destinationRectangle.X += destinationRectangle.Width * GaugeDivision - destinationRectangle.Width * (gaugeIndex + 1);
			break;
		case BarGraphDirection.Down:
			destinationRectangle.Y += destinationRectangle.Height * gaugeIndex;
			break;
		case BarGraphDirection.Up:
			destinationRectangle.Y += destinationRectangle.Height * GaugeDivision - destinationRectangle.Height * (gaugeIndex + 1);
			break;
		}
		return destinationRectangle;
	}

	public int GetContainerIndex(int gaugeIndex, int totalIndex)
	{
		if (gaugeIndex < 0 || gaugeIndex > totalIndex)
		{
			return 0;
		}
		if (Destinations.ContainsKey(totalIndex - gaugeIndex))
		{
			return totalIndex - gaugeIndex;
		}
		if (Destinations.ContainsKey((gaugeIndex + 1) * -1))
		{
			return (gaugeIndex + 1) * -1;
		}
		return 0;
	}

	public int GetTotalIndex()
	{
		double num = 0.0;
		num = ((!(Numbers.GrooveGauge >= Numbers.GaugeBorder)) ? (Numbers.GrooveGauge * ((double)Border / Numbers.GaugeBorder)) : ((double)Border + (Numbers.GrooveGauge - Numbers.GaugeBorder) * ((double)(GaugeDivision - Border) / (100.0 - Numbers.GaugeBorder))));
		return (int)Math.Floor(num);
	}

	public Rectangle GetSourceRectangle(int gaugeIndex, double time)
	{
		Rectangle result = default;
		int num = 0;
		if (Cycle > 0)
		{
			num = (int)Math.Floor((time - GetTimerValue(DivisionTimer)) % (double)Cycle / (double)Cycle * (double)(DivisionX * DivisionY));
		}
		if (Direction == BarGraphDirection.Right || Direction == BarGraphDirection.Left)
		{
			result.Width = Width / DivisionX / GaugeDivision;
			if (Direction == BarGraphDirection.Right)
			{
				result.X = X + Width / DivisionX * (num % DivisionX) + result.Width * gaugeIndex;
			}
			else
			{
				result.X = X + Width / DivisionX * (num % DivisionX) + (Width - result.Width * (gaugeIndex + 1));
			}
		}
		else
		{
			result.Width = Width / DivisionX;
			result.X = X + result.Width * (num % DivisionX);
		}
		if (Direction == BarGraphDirection.Up || Direction == BarGraphDirection.Down)
		{
			result.Height = Height / DivisionY / GaugeDivision;
			if (Direction == BarGraphDirection.Down)
			{
				result.Y = Y + Height / DivisionY * (num / DivisionX) + result.Height * gaugeIndex;
			}
			else
			{
				result.Y = Y + Height / DivisionY * (num / DivisionX) + (Height - result.Height * (gaugeIndex + 1));
			}
		}
		else
		{
			result.Height = Height / DivisionY;
			result.Y = Y + result.Height * (num / DivisionX);
		}
		return result;
	}
}
