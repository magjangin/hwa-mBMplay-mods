using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using Mafu.MafuFont;
using MistGraphics;

namespace MBMPlay;

public class TextSkinItem : SingleDestinationSkinItem
{
	private string _prevValue;

	private List<string> _elements = new List<string>();

	public string TextID { get; set; }

	public string Value => Texts.GetValue(TextID);

	public string FontID { get; set; }

	public TextAlign Align { get; set; }

	public TextOverflow Overflow { get; set; }

	public TextSkinItem(Skin skin)
		: base(skin)
	{
	}

	private static bool TryGetCharCode(string s, out int code)
	{
		if (s.Length >= 1 && !char.IsControl(s, 0))
		{
			if (!char.IsHighSurrogate(s[0]))
			{
				code = s[0];
				return true;
			}
			if (s.Length >= 2 && char.IsLowSurrogate(s[1]))
			{
				code = char.ConvertToUtf32(s, 0);
				return true;
			}
		}
		code = -1;
		return false;
	}

	public void Draw(GraphicsBase graphics, Font font, double time, Point offset, in ColorVector color)
	{
		Rectangle destinationRectangle = GetDestinationRectangle(time, offset);
		ColorVector color2 = GetDestinationColorVector(time) * color;
		string value = Value;
		if (value != _prevValue)
		{
			_prevValue = value;
			_elements.Clear();
			TextElementEnumerator textElementEnumerator = StringInfo.GetTextElementEnumerator(value);
			while (textElementEnumerator.MoveNext())
			{
				string textElement = textElementEnumerator.GetTextElement();
				if (!string.IsNullOrEmpty(textElement) && !char.IsControl(textElement[0]))
				{
					_elements.Add(textElement);
				}
			}
		}
		double num = (double)destinationRectangle.Height / (double)font.FontData.Header.FontSize;
		int num2 = (int)Math.Truncate((double)font.FontData.Header.LetterSpacing * num);
		int num3 = 0;
		for (int i = 0; i < _elements.Count; i++)
		{
			if (font.Characters.ContainsKey(_elements[i]))
			{
				int num4 = (int)Math.Floor((double)font.Characters[_elements[i]].Width * num);
				num3 += ((i >= _elements.Count - 1) ? num4 : (num4 + num2));
			}
		}
		double num5 = 1.0;
		if (Overflow == TextOverflow.Stretch && destinationRectangle.Width < num3)
		{
			num5 = (double)destinationRectangle.Width / (double)num3;
			num2 = (int)Math.Floor((double)num2 * num5);
			num3 = 0;
			for (int j = 0; j < _elements.Count; j++)
			{
				if (font.Characters.ContainsKey(_elements[j]))
				{
					int num6 = (int)Math.Floor((double)font.Characters[_elements[j]].Width * num * num5);
					num3 += ((j >= _elements.Count - 1) ? num6 : (num6 + num2));
				}
			}
		}
		int num7 = 0;
		if (Align == TextAlign.Center)
		{
			num7 -= num3 / 2;
		}
		else if (Align == TextAlign.Right)
		{
			num7 -= num3;
		}
		int num8 = 0;
		for (int k = 0; k < _elements.Count; k++)
		{
			if (!font.Characters.ContainsKey(_elements[k]))
			{
				continue;
			}
			CharSrc charSrc = font.Characters[_elements[k]];
			int num9 = (int)Math.Floor((double)charSrc.Width * num);
			int num10 = num9;
			int num11 = 0;
			Rectangle srcRectangle = Rectangle.Empty;
			if (Overflow == TextOverflow.Hidden)
			{
				if (Align == TextAlign.Left)
				{
					if (num8 > destinationRectangle.Width)
					{
						break;
					}
					if (num8 + num9 > destinationRectangle.Width)
					{
						int num12 = num9 - (num8 + num9 - destinationRectangle.Width);
						double num13 = (double)num12 / (double)num9;
						srcRectangle = new Rectangle
						{
							X = charSrc.SourceRectangle.X,
							Y = charSrc.SourceRectangle.Y,
							Width = (int)Math.Ceiling((double)charSrc.SourceRectangle.Width * num13),
							Height = charSrc.SourceRectangle.Height
						};
						num9 = num12;
					}
					else
					{
						srcRectangle = charSrc.SourceRectangle;
					}
				}
				else if (Align == TextAlign.Center)
				{
					if (num3 > destinationRectangle.Width)
					{
						int num14 = (num3 - destinationRectangle.Width) / 2;
						int num15 = 0;
						int num16 = charSrc.SourceRectangle.Width;
						if (num8 + num9 < num14)
						{
							num8 += num9 + num2;
							continue;
						}
						if (num8 < num14)
						{
							int num17 = num8 + num9 - num14;
							num11 = num9 - num17;
							double num18 = (double)num17 / (double)num9;
							num16 = (int)Math.Ceiling((double)num16 * num18);
							num15 = charSrc.SourceRectangle.Width - num16;
							num9 = num17;
						}
						if (num8 > num14 + destinationRectangle.Width)
						{
							break;
						}
						if (num8 + num9 > num14 + destinationRectangle.Width)
						{
							int num19 = num9 - (num8 + num9 - (num14 + destinationRectangle.Width));
							double num20 = (double)num19 / (double)num9;
							num16 = (int)Math.Ceiling((double)num16 * num20);
							num9 = num19;
						}
						srcRectangle = new Rectangle
						{
							X = charSrc.SourceRectangle.X + num15,
							Y = charSrc.SourceRectangle.Y,
							Width = num16,
							Height = charSrc.SourceRectangle.Height
						};
					}
					else
					{
						srcRectangle = charSrc.SourceRectangle;
					}
				}
				else if (Align == TextAlign.Right)
				{
					if (num3 > destinationRectangle.Width)
					{
						int num21 = num3 - destinationRectangle.Width;
						if (num8 + num9 < num21)
						{
							num8 += num9 + num2;
							continue;
						}
						if (num8 < num21)
						{
							int num22 = num8 + num9 - num21;
							num11 = num9 - num22;
							double num23 = (double)num22 / (double)num9;
							int num24 = (int)Math.Ceiling((double)charSrc.SourceRectangle.Width * num23);
							int num25 = charSrc.SourceRectangle.Width - num24;
							srcRectangle = new Rectangle
							{
								X = charSrc.SourceRectangle.X + num25,
								Y = charSrc.SourceRectangle.Y,
								Width = num24,
								Height = charSrc.SourceRectangle.Height
							};
							num9 = num22;
						}
						else
						{
							srcRectangle = charSrc.SourceRectangle;
						}
					}
					else
					{
						srcRectangle = charSrc.SourceRectangle;
					}
				}
			}
			else
			{
				srcRectangle = charSrc.SourceRectangle;
			}
			Rectangle dstRectangle = new Rectangle
			{
				X = destinationRectangle.X + num7 + num8 + num11,
				Y = destinationRectangle.Y,
				Width = (int)Math.Floor((double)num9 * num5),
				Height = (int)Math.Floor((double)charSrc.Height * num)
			};
			TextureInfo texture = font.GetTexture(charSrc.TextureID);
			if ((object)texture != null)
			{
				graphics.Draw(texture, in srcRectangle, in dstRectangle, in color2);
			}
			num8 += (int)Math.Floor((double)num10 * num5) + num2;
		}
	}

	public void Draw(GraphicsBase graphics, Font font, double time)
	{
		Draw(graphics, font, time, Point.Empty, in ColorVector.White);
	}
}
