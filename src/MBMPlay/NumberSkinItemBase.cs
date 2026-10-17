extern alias MistUtilsAlias;

using System;
using System.Drawing;
using MistUtils;
using MistUtilsAlias::MistUtils;

namespace MBMPlay;

public abstract class NumberSkinItemBase : ImageSkinItemBase
{
	private int _digit;

	private NumberOverflow _overflow;

	private int _maxValue = int.MaxValue;

	private int _minValue = int.MinValue;

	private ISkinItem _valueItem;

	public string NumberID { get; set; }

	public int Value
	{
		get
		{
			if (_overflow == NumberOverflow.CounterStop && _digit != 0)
			{
				return MistUtils.Utils.Clamp(GetValue(), _minValue, _maxValue);
			}
			return GetValue();
		}
	}

	public NumberAlign Align { get; set; }

	public int Digit
	{
		get
		{
			return _digit;
		}
		set
		{
			_digit = value;
			SetValueRange();
		}
	}

	public int NumberType { get; set; } = 10;

	public int Space { get; set; }

	public NumberOverflow Overflow
	{
		get
		{
			return _overflow;
		}
		set
		{
			_overflow = value;
			SetValueRange();
		}
	}

	protected int GetValue()
	{
		if (NumberID.StartsWith("$_trackbar", StringComparison.Ordinal))
		{
			if (_valueItem == null)
			{
				int num = NumberID.IndexOf('[');
				if (num >= 0 && NumberID.Length > num + 1)
				{
					string[] array = NumberID.Substring(num + 1).Replace("]", "").Split('[');
					if (array.Length >= 1)
					{
						_valueItem = Skin.GetSkinItemByID(array[0]);
					}
				}
			}
			if (_valueItem is TrackBarSkinItem trackBarSkinItem)
			{
				if (NumberID.StartsWith("$_trackbar_point[", StringComparison.Ordinal))
				{
					return Numbers.GetAfterPointValue(trackBarSkinItem.Value, Digit);
				}
				if (NumberID.StartsWith("$_trackbar[", StringComparison.Ordinal))
				{
					return Numbers.GetBeforePointValue(trackBarSkinItem.Value, Digit);
				}
			}
			return 0;
		}
		return Numbers.GetValue(NumberID, Digit);
	}

	protected void SetValueRange()
	{
		if (_overflow == NumberOverflow.CounterStop && _digit != 0)
		{
			_maxValue = (int)Math.Pow(10.0, Math.Abs(_digit)) - 1;
			_minValue = _maxValue * -1;
		}
		else
		{
			_maxValue = int.MaxValue;
			_minValue = int.MinValue;
		}
	}

	protected NumberSkinItemBase(Skin skin)
		: base(skin)
	{
		DivisionX = 10;
		DivisionY = 1;
	}

	public override Rectangle GetDestinationRectangle(double time, Point offset)
	{
		if (!CheckDisplayed(time))
		{
			return Rectangle.Empty;
		}
		int numberDigit = GetNumberDigit();
		int num = 0;
		int num2 = Space;
		Rectangle result;
		if (Destination.DestinationList.Count == 1)
		{
			result = Destination.DestinationList[0].DestinationRectangle.Move(offset);
		}
		else
		{
			double dstTime = GetDstTime(time);
			result = default;
			for (int i = 0; i < Destination.DestinationList.Count; i++)
			{
				SkinDestination skinDestination = Destination.DestinationList[i];
				if ((double)skinDestination.Time >= dstTime)
				{
					if (i <= 0)
					{
						result = skinDestination.DestinationRectangle.Move(offset);
						break;
					}
					SkinDestination skinDestination2 = Destination.DestinationList[i - 1];
					double num3 = (dstTime - (double)skinDestination2.Time) / (double)(skinDestination.Time - skinDestination2.Time);
					result.X = skinDestination2.X + Round((double)(skinDestination.X - skinDestination2.X) * num3, skinDestination.RoundMethodX) + offset.X;
					result.Width = skinDestination2.Width + Round((double)(skinDestination.Width - skinDestination2.Width) * num3, skinDestination.RoundMethodWidth);
					num2 = (int)Math.Truncate((double)Space * num3);
					result.Y = skinDestination2.Y + Round((double)(skinDestination.Y - skinDestination2.Y) * num3, skinDestination.RoundMethodY) + offset.Y;
					result.Height = skinDestination2.Height + Round((double)(skinDestination.Height - skinDestination2.Height) * num3, skinDestination.RoundMethodHeight);
					break;
				}
			}
		}
		if (Digit == 0)
		{
			if (Align == NumberAlign.Right)
			{
				num = (result.Width + num2) * (numberDigit - 1) * -1;
			}
			else if (Align == NumberAlign.Center)
			{
				num = (int)Math.Floor((double)((result.Width + num2) * (numberDigit - 1)) / 2.0) * -1;
			}
		}
		else if (NumberType == 10)
		{
			if (Align == NumberAlign.Right)
			{
				num = (result.Width + num2) * (Math.Abs(Digit) - numberDigit);
			}
			else if (Align == NumberAlign.Center)
			{
				num = (int)Math.Floor((double)((result.Width + num2) * (Math.Abs(Digit) - numberDigit)) / 2.0);
			}
		}
		result.X += num;
		result.Width = result.Width * numberDigit + ((numberDigit > 1) ? (num2 * (numberDigit - 1)) : 0);
		return result;
	}

	public Rectangle GetDestinationRectangle(double time, int digit, Point offset)
	{
		if (!CheckDisplayed(time))
		{
			return Rectangle.Empty;
		}
		int num;
		if (NumberType == 24 && Digit != 0)
		{
			num = Math.Abs(Digit) + 1;
		}
		else if (NumberType == 24 && Digit == 0 && Value < 0)
		{
			num = GetNumberDigit() + 1;
		}
		else if (NumberType == 11 && Digit != 0)
		{
			num = Math.Abs(Digit);
		}
		else
		{
			num = ((NumberType != 10 || Digit == 0) ? GetNumberDigit() : Math.Min(Math.Abs(Digit), GetNumberDigit()));
		}
		int num2 = 0;
		int num3 = Space;
		Rectangle rectangle;
		if (Destination.DestinationList.Count == 1)
		{
			rectangle = Destination.DestinationList[0].DestinationRectangle;
			rectangle.X += (rectangle.Width + num3) * (num - digit - 1);
		}
		else
		{
			double dstTime = GetDstTime(time);
			rectangle = default;
			for (int i = 0; i < Destination.DestinationList.Count; i++)
			{
				SkinDestination skinDestination = Destination.DestinationList[i];
				if ((double)skinDestination.Time >= dstTime)
				{
					if (i <= 0)
					{
						rectangle = Destination.DestinationList[0].DestinationRectangle;
						rectangle.X += (rectangle.Width + num3) * (num - digit - 1);
						break;
					}
					SkinDestination skinDestination2 = Destination.DestinationList[i - 1];
					double num4 = (dstTime - (double)skinDestination2.Time) / (double)(skinDestination.Time - skinDestination2.Time);
					num3 = (int)Math.Truncate((double)Space * num4);
					rectangle.Width = skinDestination2.Width + (int)Math.Ceiling((double)(skinDestination.Width - skinDestination2.Width) * num4);
					rectangle.X = skinDestination2.X + (rectangle.Width + num3) * (num - digit - 1) + (int)Math.Ceiling((double)(skinDestination.X - skinDestination2.X) * num4);
					rectangle.Y = skinDestination2.Y + (int)Math.Ceiling((double)(skinDestination.Y - skinDestination2.Y) * num4);
					rectangle.Height = skinDestination2.Height + (int)Math.Ceiling((double)(skinDestination.Height - skinDestination2.Height) * num4);
					break;
				}
			}
		}
		if (Digit == 0)
		{
			if (Align == NumberAlign.Right)
			{
				num2 = (rectangle.Width + num3) * (num - 1) * -1;
			}
			else if (Align == NumberAlign.Center)
			{
				num2 = (int)Math.Floor((double)((rectangle.Width + num3) * (num - 1)) / 2.0) * -1;
			}
		}
		else if (NumberType == 10)
		{
			if (Align == NumberAlign.Right)
			{
				num2 = (rectangle.Width + num3) * (Math.Abs(Digit) - num);
			}
			else if (Align == NumberAlign.Center)
			{
				num2 = (int)Math.Floor((double)((rectangle.Width + num3) * (Math.Abs(Digit) - num)) / 2.0);
			}
		}
		rectangle = rectangle.Move(offset);
		rectangle.X += num2;
		return rectangle;
	}

	public double GetDestinationPivotX(double time, int totalDigit, int digitIndex)
	{
		if (!CheckDisplayed(time))
		{
			return 0.0;
		}
		if (Destination.DestinationList.Count == 1)
		{
			if (Destination.DestinationList[0].PivotXType != PivotType.Relative)
			{
				return Destination.DestinationList[0].PivotX - (double)(Destination.DestinationList[0].Width * (totalDigit - 1 - digitIndex));
			}
			return (double)(Destination.DestinationList[0].Width * totalDigit) * Destination.DestinationList[0].PivotX - (double)(Destination.DestinationList[0].Width * (totalDigit - 1 - digitIndex));
		}
		double dstTime = GetDstTime(time);
		double result = 0.0;
		for (int i = 0; i < Destination.DestinationList.Count; i++)
		{
			SkinDestination skinDestination = Destination.DestinationList[i];
			if ((double)skinDestination.Time >= dstTime)
			{
				double num = ((skinDestination.PivotXType == PivotType.Relative) ? ((double)(skinDestination.Width * totalDigit) * skinDestination.PivotX - (double)(skinDestination.Width * (totalDigit - 1 - digitIndex))) : (skinDestination.PivotX - (double)(skinDestination.Width * (totalDigit - 1 - digitIndex))));
				if (i <= 0)
				{
					return num;
				}
				SkinDestination skinDestination2 = Destination.DestinationList[i - 1];
				double num2 = ((skinDestination2.PivotXType == PivotType.Relative) ? ((double)(skinDestination2.Width * totalDigit) * skinDestination2.PivotX - (double)(skinDestination2.Width * (totalDigit - 1 - digitIndex))) : (skinDestination2.PivotX - (double)(skinDestination2.Width * (totalDigit - 1 - digitIndex))));
				double num3 = (dstTime - (double)skinDestination2.Time) / (double)(skinDestination.Time - skinDestination2.Time);
				result = num2 + (num - num2) * num3;
				break;
			}
		}
		return result;
	}

	public Rectangle GetSourceRectangle(double time, int digitIndex)
	{
		Rectangle result = new Rectangle
		{
			Width = Width / DivisionX,
			Height = Height / DivisionY
		};
		int num = 0;
		if (Cycle > 0)
		{
			num = (int)Math.Floor((time - GetTimerValue(DivisionTimer)) % (double)Cycle / (double)Cycle * (double)(DivisionX * DivisionY / NumberType));
		}
		int num2 = digitIndex + num * NumberType;
		result.X = X + result.Width * (num2 % DivisionX);
		result.Y = Y + result.Height * (num2 / DivisionX);
		return result;
	}

	public int GetNumberDigit()
	{
		double valueDouble = Numbers.GetValueDouble(NumberID);
		if (valueDouble > 2147483647.0 || valueDouble < -2147483648.0)
		{
			return 9;
		}
		int value = Value;
		if (value == 0)
		{
			return 1;
		}
		return (int)Math.Floor(Math.Log10(Math.Abs(value))) + 1;
	}
}
