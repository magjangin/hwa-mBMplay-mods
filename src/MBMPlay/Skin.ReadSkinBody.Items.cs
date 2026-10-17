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
		// <image, number, bargraph, slider, lanecover, barline, judge, combo>
		private void ReadDrawableItem()
		{
			ImageSkinItemBase imageSkinItemBase = reader.LocalName switch
			{
				"number" => (ImageSkinItemBase)new NumberSkinItem(skin), 
				"bargraph" => new BarGraphSkinItem(skin), 
				"slider" => new SliderSkinItem(skin), 
				"lanecover" => new LaneCoverSkinItem(skin), 
				"barline" => new BarlineSkinItem(skin), 
				"judge" => new JudgeSkinItem(skin), 
				"combo" => new ComboSkinItem(skin), 
				_ => new ImageSkinItem(skin), 
			};
			if (reader.MoveToFirstAttribute())
			{
				do
				{
					switch (reader.LocalName)
					{
					case "id":
						imageSkinItemBase.ID = reader.Value;
						break;
					case "texture":
						imageSkinItemBase.TextureID = reader.Value;
						break;
					case "x":
						imageSkinItemBase.X = skin.GetIntValue(reader.Value);
						break;
					case "y":
						imageSkinItemBase.Y = skin.GetIntValue(reader.Value);
						break;
					case "width":
						imageSkinItemBase.Width = skin.GetIntValue(reader.Value);
						break;
					case "height":
						imageSkinItemBase.Height = skin.GetIntValue(reader.Value);
						break;
					case "div_x":
						imageSkinItemBase.DivisionX = skin.GetIntValue(reader.Value);
						break;
					case "div_y":
						imageSkinItemBase.DivisionY = skin.GetIntValue(reader.Value);
						break;
					case "cycle":
						imageSkinItemBase.Cycle = skin.GetIntValue(reader.Value);
						break;
					case "div_timer":
						imageSkinItemBase.DivisionTimer = ConvertTimerString(reader.Value);
						break;
					case "dst_timer":
						imageSkinItemBase.Destination.DestinationTimer = ConvertTimerString(reader.Value);
						break;
					case "loop":
						imageSkinItemBase.Destination.DestinationLoop = skin.GetIntValue(reader.Value);
						break;
					case "blend":
						if (blendTypeDict.ContainsKey(reader.Value))
						{
							imageSkinItemBase.Destination.Blend = blendTypeDict[reader.Value];
						}
						break;
					case "filter":
						localName = reader.Value;
						if (!(localName == "point"))
						{
							if (localName == "linear")
							{
								imageSkinItemBase.Destination.Filter = SamplerType.Linear;
							}
						}
						else
						{
							imageSkinItemBase.Destination.Filter = SamplerType.Point;
						}
						break;
					case "condition":
						AddConditions(imageSkinItemBase.Destination, reader.Value);
						break;
					case "value":
						if (imageSkinItemBase is NumberSkinItemBase numberSkinItemBase)
						{
							numberSkinItemBase.NumberID = reader.Value;
						}
						else if (imageSkinItemBase is BarGraphSkinItem barGraphSkinItem)
						{
							barGraphSkinItem.NumberID = reader.Value;
						}
						else if (imageSkinItemBase is SliderSkinItem sliderSkinItem)
						{
							sliderSkinItem.NumberID = reader.Value;
						}
						break;
					case "align":
						if (imageSkinItemBase is NumberSkinItemBase numberSkinItemBase5)
						{
							switch (reader.Value)
							{
							case "left":
								numberSkinItemBase5.Align = NumberAlign.Left;
								break;
							case "center":
								numberSkinItemBase5.Align = NumberAlign.Center;
								break;
							case "right":
								numberSkinItemBase5.Align = NumberAlign.Right;
								break;
							}
						}
						break;
					case "digit":
						if (imageSkinItemBase is NumberSkinItemBase numberSkinItemBase4)
						{
							numberSkinItemBase4.Digit = skin.GetIntValue(reader.Value);
						}
						break;
					case "space":
						if (imageSkinItemBase is NumberSkinItemBase numberSkinItemBase3)
						{
							numberSkinItemBase3.Space = skin.GetIntValue(reader.Value);
						}
						break;
					case "overflow":
						if (!(imageSkinItemBase is NumberSkinItemBase numberSkinItemBase2))
						{
							break;
						}
						localName = reader.Value;
						if (!(localName == "hidden"))
						{
							if (localName == "stop")
							{
								numberSkinItemBase2.Overflow = NumberOverflow.CounterStop;
							}
						}
						else
						{
							numberSkinItemBase2.Overflow = NumberOverflow.Hidden;
						}
						break;
					case "max":
						if (imageSkinItemBase is BarGraphSkinItem barGraphSkinItem3)
						{
							barGraphSkinItem3.Max = skin.GetIntValue(reader.Value);
						}
						else if (imageSkinItemBase is SliderSkinItem sliderSkinItem3)
						{
							sliderSkinItem3.Max = skin.GetIntValue(reader.Value);
						}
						break;
					case "min":
						if (imageSkinItemBase is BarGraphSkinItem barGraphSkinItem2)
						{
							barGraphSkinItem2.Min = skin.GetIntValue(reader.Value);
						}
						else if (imageSkinItemBase is SliderSkinItem sliderSkinItem2)
						{
							sliderSkinItem2.Min = skin.GetIntValue(reader.Value);
						}
						break;
					case "img_mode":
						if (!(imageSkinItemBase is BarGraphSkinItem barGraphSkinItem6))
						{
							break;
						}
						localName = reader.Value;
						if (!(localName == "stretch"))
						{
							if (localName == "cut")
							{
								barGraphSkinItem6.ImageMode = BarGraphImageMode.Cut;
							}
						}
						else
						{
							barGraphSkinItem6.ImageMode = BarGraphImageMode.Stretch;
						}
						break;
					case "direction":
						if (imageSkinItemBase is BarGraphSkinItem barGraphSkinItem5)
						{
							switch (reader.Value)
							{
							case "right":
								barGraphSkinItem5.Direction = BarGraphDirection.Right;
								break;
							case "left":
								barGraphSkinItem5.Direction = BarGraphDirection.Left;
								break;
							case "up":
								barGraphSkinItem5.Direction = BarGraphDirection.Up;
								break;
							case "down":
								barGraphSkinItem5.Direction = BarGraphDirection.Down;
								break;
							}
						}
						break;
					case "round":
					{
						if (imageSkinItemBase is BarGraphSkinItem barGraphSkinItem4 && TryGetRoundMethod(reader.Value, out var method6))
						{
							barGraphSkinItem4.RoundMethod = method6;
						}
						break;
					}
					case "start_x":
						if (imageSkinItemBase is SliderSkinItem)
						{
							((SliderSkinItem)imageSkinItemBase).StartX = skin.GetIntValue(reader.Value);
						}
						else if (imageSkinItemBase is LaneCoverSkinItem)
						{
							((LaneCoverSkinItem)imageSkinItemBase).StartX = skin.GetIntValue(reader.Value);
						}
						break;
					case "start_y":
						if (imageSkinItemBase is SliderSkinItem)
						{
							((SliderSkinItem)imageSkinItemBase).StartY = skin.GetIntValue(reader.Value);
						}
						else if (imageSkinItemBase is LaneCoverSkinItem)
						{
							((LaneCoverSkinItem)imageSkinItemBase).StartY = skin.GetIntValue(reader.Value);
						}
						break;
					case "end_x":
						if (imageSkinItemBase is SliderSkinItem)
						{
							((SliderSkinItem)imageSkinItemBase).EndX = skin.GetIntValue(reader.Value);
						}
						else if (imageSkinItemBase is LaneCoverSkinItem)
						{
							((LaneCoverSkinItem)imageSkinItemBase).EndX = skin.GetIntValue(reader.Value);
						}
						break;
					case "end_y":
						if (imageSkinItemBase is SliderSkinItem)
						{
							((SliderSkinItem)imageSkinItemBase).EndY = skin.GetIntValue(reader.Value);
						}
						else if (imageSkinItemBase is LaneCoverSkinItem)
						{
							((LaneCoverSkinItem)imageSkinItemBase).EndY = skin.GetIntValue(reader.Value);
						}
						break;
					case "side":
						if (imageSkinItemBase is BarlineSkinItem)
						{
							((BarlineSkinItem)imageSkinItemBase).Side = skin.GetIntValue(reader.Value);
						}
						else if (imageSkinItemBase is LaneCoverSkinItem)
						{
							((LaneCoverSkinItem)imageSkinItemBase).Side = skin.GetIntValue(reader.Value);
						}
						break;
					case "index":
						if (imageSkinItemBase is JudgeSkinItem)
						{
							((JudgeSkinItem)imageSkinItemBase).JudgeID = skin.GetIntValue(reader.Value);
						}
						else if (imageSkinItemBase is ComboSkinItem)
						{
							((ComboSkinItem)imageSkinItemBase).JudgeID = skin.GetIntValue(reader.Value);
						}
						break;
					}
				}
				while (reader.MoveToNextAttribute());
				reader.MoveToElement();
			}
			skinItem = imageSkinItemBase;
		}

		// <text>
		private void ReadText()
		{
			TextSkinItem textSkinItem = new TextSkinItem(skin);
			if (reader.MoveToFirstAttribute())
			{
				do
				{
					switch (reader.LocalName)
					{
					case "font":
						textSkinItem.FontID = reader.Value;
						break;
					case "value":
						textSkinItem.TextID = reader.Value;
						break;
					case "align":
						switch (reader.Value)
						{
						case "left":
							textSkinItem.Align = TextAlign.Left;
							break;
						case "center":
							textSkinItem.Align = TextAlign.Center;
							break;
						case "right":
							textSkinItem.Align = TextAlign.Right;
							break;
						}
						break;
					case "overflow":
						switch (reader.Value)
						{
						case "visible":
							textSkinItem.Overflow = TextOverflow.Visible;
							break;
						case "hidden":
							textSkinItem.Overflow = TextOverflow.Hidden;
							break;
						case "stretch":
							textSkinItem.Overflow = TextOverflow.Stretch;
							break;
						}
						break;
					case "dst_timer":
						textSkinItem.Destination.DestinationTimer = ConvertTimerString(reader.Value);
						break;
					case "loop":
						textSkinItem.Destination.DestinationLoop = skin.GetIntValue(reader.Value);
						break;
					case "blend":
						if (blendTypeDict.ContainsKey(reader.Value))
						{
							textSkinItem.Destination.Blend = blendTypeDict[reader.Value];
						}
						break;
					case "filter":
						localName = reader.Value;
						if (!(localName == "point"))
						{
							if (localName == "linear")
							{
								textSkinItem.Destination.Filter = SamplerType.Linear;
							}
						}
						else
						{
							textSkinItem.Destination.Filter = SamplerType.Point;
						}
						break;
					case "condition":
						AddConditions(textSkinItem.Destination, reader.Value);
						break;
					}
				}
				while (reader.MoveToNextAttribute());
				reader.MoveToElement();
			}
			skinItem = textSkinItem;
		}

		// <bga>
		private void ReadBga()
		{
			BgaSkinItem bgaSkinItem = new BgaSkinItem(skin);
			if (reader.MoveToFirstAttribute())
			{
				do
				{
					switch (reader.LocalName)
					{
					case "dst_timer":
						bgaSkinItem.Destination.DestinationTimer = ConvertTimerString(reader.Value);
						break;
					case "loop":
						bgaSkinItem.Destination.DestinationLoop = skin.GetIntValue(reader.Value);
						break;
					case "blend":
						if (blendTypeDict.ContainsKey(reader.Value))
						{
							bgaSkinItem.Destination.Blend = blendTypeDict[reader.Value];
						}
						break;
					case "filter":
						localName = reader.Value;
						if (!(localName == "point"))
						{
							if (localName == "linear")
							{
								bgaSkinItem.Destination.Filter = SamplerType.Linear;
							}
						}
						else
						{
							bgaSkinItem.Destination.Filter = SamplerType.Point;
						}
						break;
					case "condition":
						AddConditions(bgaSkinItem.Destination, reader.Value);
						break;
					}
				}
				while (reader.MoveToNextAttribute());
				reader.MoveToElement();
			}
			skinItem = bgaSkinItem;
		}

		// <lane>
		private void ReadLane()
		{
			LaneSkinItem laneSkinItem = new LaneSkinItem(skin);
			if (reader.MoveToFirstAttribute())
			{
				do
				{
					switch (reader.LocalName)
					{
					case "index":
						laneSkinItem.LaneIndex = skin.GetIntValue(reader.Value);
						break;
					case "dst_timer":
						laneSkinItem.Destination.DestinationTimer = ConvertTimerString(reader.Value);
						break;
					case "loop":
						laneSkinItem.Destination.DestinationLoop = skin.GetIntValue(reader.Value);
						break;
					case "blend":
						if (blendTypeDict.ContainsKey(reader.Value))
						{
							laneSkinItem.Destination.Blend = blendTypeDict[reader.Value];
						}
						break;
					case "filter":
						localName = reader.Value;
						if (!(localName == "point"))
						{
							if (localName == "linear")
							{
								laneSkinItem.Destination.Filter = SamplerType.Linear;
							}
						}
						else
						{
							laneSkinItem.Destination.Filter = SamplerType.Point;
						}
						break;
					case "condition":
						AddConditions(laneSkinItem.Destination, reader.Value);
						break;
					}
				}
				while (reader.MoveToNextAttribute());
				reader.MoveToElement();
			}
			skinItem = laneSkinItem;
		}
	}
}
