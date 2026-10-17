using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using MBMPlay.Common;
using MistGraphics;
using MistUtils;

namespace MBMPlay;

public partial class Skin
{
	private sealed partial class SkinBodyReader
	{
		// <note, ln_start, ln_end, ln_body, ln_start_holding, ln_end_holding, ln_body_holding, ln_start_released, ln_end_released, ln_body_released>
		private void ReadNote()
		{
			if (!(skinItem is LaneSkinItem))
			{
				return;
			}
			NoteSource noteSource = new NoteSource(skin);
			if (reader.MoveToFirstAttribute())
			{
				do
				{
					switch (reader.LocalName)
					{
					case "texture":
						noteSource.TextureID = reader.Value;
						break;
					case "x":
						noteSource.X = skin.GetIntValue(reader.Value);
						break;
					case "y":
						noteSource.Y = skin.GetIntValue(reader.Value);
						break;
					case "width":
						noteSource.Width = skin.GetIntValue(reader.Value);
						break;
					case "height":
						noteSource.Height = skin.GetIntValue(reader.Value);
						break;
					case "div_x":
						noteSource.DivisionX = skin.GetIntValue(reader.Value);
						break;
					case "div_y":
						noteSource.DivisionY = skin.GetIntValue(reader.Value);
						break;
					case "alpha":
						noteSource.A = skin.GetIntValue(reader.Value);
						break;
					case "r":
						noteSource.R = skin.GetIntValue(reader.Value);
						break;
					case "g":
						noteSource.G = skin.GetIntValue(reader.Value);
						break;
					case "b":
						noteSource.B = skin.GetIntValue(reader.Value);
						break;
					case "cycle":
						noteSource.Cycle = skin.GetIntValue(reader.Value);
						break;
					case "div_timer":
						noteSource.DivisionTimer = reader.Value;
						break;
					}
				}
				while (reader.MoveToNextAttribute());
				reader.MoveToElement();
			}
			((LaneSkinItem)skinItem).NoteSources.Add(reader.LocalName, noteSource);
		}

		// <gauge>
		private void ReadGauge()
		{
			GaugeSkinItem gaugeSkinItem = new GaugeSkinItem(skin);
			if (reader.MoveToFirstAttribute())
			{
				do
				{
					switch (reader.LocalName)
					{
					case "texture":
						gaugeSkinItem.TextureID = reader.Value;
						break;
					case "x":
						gaugeSkinItem.X = skin.GetIntValue(reader.Value);
						break;
					case "y":
						gaugeSkinItem.Y = skin.GetIntValue(reader.Value);
						break;
					case "width":
						gaugeSkinItem.Width = skin.GetIntValue(reader.Value);
						break;
					case "height":
						gaugeSkinItem.Height = skin.GetIntValue(reader.Value);
						break;
					case "div_x":
						gaugeSkinItem.DivisionX = skin.GetIntValue(reader.Value);
						break;
					case "div_y":
						gaugeSkinItem.DivisionY = skin.GetIntValue(reader.Value);
						break;
					case "cycle":
						gaugeSkinItem.Cycle = skin.GetIntValue(reader.Value);
						break;
					case "div_timer":
						gaugeSkinItem.DivisionTimer = reader.Value;
						break;
					case "div_gauge":
						gaugeSkinItem.GaugeDivision = skin.GetIntValue(reader.Value);
						break;
					case "border":
						gaugeSkinItem.Border = skin.GetIntValue(reader.Value);
						break;
					case "direction":
						switch (reader.Value)
						{
						case "right":
							gaugeSkinItem.Direction = BarGraphDirection.Right;
							break;
						case "left":
							gaugeSkinItem.Direction = BarGraphDirection.Left;
							break;
						case "up":
							gaugeSkinItem.Direction = BarGraphDirection.Up;
							break;
						case "down":
							gaugeSkinItem.Direction = BarGraphDirection.Down;
							break;
						}
						break;
					}
				}
				while (reader.MoveToNextAttribute());
				reader.MoveToElement();
			}
			skinItem = gaugeSkinItem;
		}

		// <gauge_dst, panel_dst>
		private void ReadGroupDestination()
		{
			string localName3 = reader.LocalName;
			SkinDestinationContainer skinDestinationContainer2 = new SkinDestinationContainer();
			key = 0;
			if (reader.MoveToFirstAttribute())
			{
				do
				{
					switch (reader.LocalName)
					{
					case "index":
						if (localName3 == "gauge_dst")
						{
							key = skin.GetIntValue(reader.Value);
						}
						break;
					case "state":
						if (localName3 == "panel_dst")
						{
							key = ((reader.Value == "open") ? 1 : 0);
						}
						break;
					case "dst_timer":
						skinDestinationContainer2.DestinationTimer = ConvertTimerString(reader.Value);
						break;
					case "loop":
						skinDestinationContainer2.DestinationLoop = skin.GetIntValue(reader.Value);
						break;
					case "blend":
						if (blendTypeDict.ContainsKey(reader.Value))
						{
							skinDestinationContainer2.Blend = blendTypeDict[reader.Value];
						}
						break;
					case "filter":
						localName = reader.Value;
						if (!(localName == "point"))
						{
							if (localName == "linear")
							{
								skinDestinationContainer2.Filter = SamplerType.Linear;
							}
						}
						else
						{
							skinDestinationContainer2.Filter = SamplerType.Point;
						}
						break;
					case "condition":
						AddConditions(skinDestinationContainer2, reader.Value);
						break;
					}
				}
				while (reader.MoveToNextAttribute());
				reader.MoveToElement();
			}
			skinDestinationContainer = skinDestinationContainer2;
		}

		// <dst>
		private void ReadDestination()
		{
			List<SkinDestination> list2 = null;
			if (trackBarSkinItem?.Thumb != null)
			{
				list2 = trackBarSkinItem.Destination.DestinationList;
			}
			else if (skinItem == null && groups.Count > 0)
			{
				if (groups[groups.Count - 1] is PanelSkinItem && skinDestinationContainer != null)
				{
					list2 = skinDestinationContainer.DestinationList;
				}
				else if (groups[groups.Count - 1] is SkinItemGroupBase skinItemGroupBase3)
				{
					list2 = skinItemGroupBase3.Destination.DestinationList;
				}
			}
			else if (skinItem is SingleDestinationSkinItem singleDestinationSkinItem)
			{
				list2 = singleDestinationSkinItem.Destination.DestinationList;
			}
			else if (skinItem is MultipleDestinationSkinItem && skinDestinationContainer != null)
			{
				list2 = skinDestinationContainer.DestinationList;
			}
			if (list2 == null)
			{
				return;
			}
			SkinDestination skinDestination = ((list2.Count > 0) ? new SkinDestination(list2[list2.Count - 1]) : new SkinDestination());
			string text4 = null;
			string text5 = null;
			if (reader.MoveToFirstAttribute())
			{
				do
				{
					switch (reader.LocalName)
					{
					case "time":
						skinDestination.Time = skin.GetIntValue(reader.Value);
						break;
					case "x":
						skinDestination.X = skin.GetIntValue(reader.Value);
						break;
					case "y":
						skinDestination.Y = skin.GetIntValue(reader.Value);
						break;
					case "width":
						skinDestination.Width = skin.GetIntValue(reader.Value);
						break;
					case "height":
						skinDestination.Height = skin.GetIntValue(reader.Value);
						break;
					case "alpha":
						skinDestination.A = skin.GetIntValue(reader.Value);
						break;
					case "r":
						skinDestination.R = skin.GetIntValue(reader.Value);
						break;
					case "g":
						skinDestination.G = skin.GetIntValue(reader.Value);
						break;
					case "b":
						skinDestination.B = skin.GetIntValue(reader.Value);
						break;
					case "rotate":
						skinDestination.Rotate = double.Parse(reader.Value);
						break;
					case "pivot_x":
						text4 = reader.Value;
						break;
					case "pivot_y":
						text5 = reader.Value;
						break;
					case "round":
					{
						if (TryGetRoundMethod(reader.Value, out var method5))
						{
							skinDestination.RoundMethodX = method5;
							skinDestination.RoundMethodY = method5;
							skinDestination.RoundMethodWidth = method5;
							skinDestination.RoundMethodHeight = method5;
						}
						break;
					}
					case "round_x":
					{
						if (TryGetRoundMethod(reader.Value, out var method4))
						{
							skinDestination.RoundMethodX = method4;
						}
						break;
					}
					case "round_y":
					{
						if (TryGetRoundMethod(reader.Value, out var method3))
						{
							skinDestination.RoundMethodY = method3;
						}
						break;
					}
					case "round_width":
					{
						if (TryGetRoundMethod(reader.Value, out var method2))
						{
							skinDestination.RoundMethodWidth = method2;
						}
						break;
					}
					case "round_height":
					{
						if (TryGetRoundMethod(reader.Value, out var method))
						{
							skinDestination.RoundMethodHeight = method;
						}
						break;
					}
					}
				}
				while (reader.MoveToNextAttribute());
				reader.MoveToElement();
			}
			switch (text4)
			{
			case "left":
				skinDestination.PivotXType = PivotType.Relative;
				skinDestination.PivotX = 0.0;
				break;
			case "center":
				skinDestination.PivotXType = PivotType.Relative;
				skinDestination.PivotX = 0.5;
				break;
			case "right":
				skinDestination.PivotXType = PivotType.Relative;
				skinDestination.PivotX = 1.0;
				break;
			default:
				if (text4.Length > 1 && text4.Substring(text4.Length - 1) == "%")
				{
					skinDestination.PivotXType = PivotType.Relative;
					skinDestination.PivotX = double.Parse(text4.Substring(0, text4.Length - 1)) / 100.0;
				}
				else
				{
					skinDestination.PivotXType = PivotType.Absolute;
					skinDestination.PivotX = double.Parse(text4);
				}
				break;
			case null:
				break;
			}
			switch (text5)
			{
			case "top":
				skinDestination.PivotYType = PivotType.Relative;
				skinDestination.PivotY = 0.0;
				break;
			case "center":
				skinDestination.PivotYType = PivotType.Relative;
				skinDestination.PivotY = 0.5;
				break;
			case "bottom":
				skinDestination.PivotYType = PivotType.Relative;
				skinDestination.PivotY = 1.0;
				break;
			default:
				if (text5.Length > 1 && text5.Substring(text5.Length - 1) == "%")
				{
					skinDestination.PivotYType = PivotType.Relative;
					skinDestination.PivotY = double.Parse(text5.Substring(0, text5.Length - 1)) / 100.0;
				}
				else
				{
					skinDestination.PivotYType = PivotType.Absolute;
					skinDestination.PivotY = double.Parse(text5);
				}
				break;
			case null:
				break;
			}
			list2.Add(skinDestination);
		}
	}
}
