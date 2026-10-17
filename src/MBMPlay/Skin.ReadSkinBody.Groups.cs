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
		// <texture, font>
		private void ReadTextureOrFont()
		{
			if (!reader.MoveToFirstAttribute())
			{
				return;
			}
			string key2 = "";
			string value = "";
			do
			{
				localName = reader.LocalName;
				if (!(localName == "id"))
				{
					if (localName == "src")
					{
						value = skin.GetFilePath(reader.Value);
					}
				}
				else
				{
					key2 = reader.Value;
				}
			}
			while (reader.MoveToNextAttribute());
			reader.MoveToElement();
			if (reader.LocalName == "texture")
			{
				skin.Textures.Add(key2, value);
			}
			else if (reader.LocalName == "font")
			{
				skin.Fonts.Add(key2, value);
			}
		}

		// <sc_side>
		private void ReadScratchSide()
		{
			if (!reader.MoveToFirstAttribute())
			{
				return;
			}
			do
			{
				localName = reader.LocalName;
				if (!(localName == "p1"))
				{
					if (localName == "p2")
					{
						if (reader.Value == "right")
						{
							skin.Player2ScratchSide = ScratchSide.Right;
						}
						else if (reader.Value == "left")
						{
							skin.Player2ScratchSide = ScratchSide.Left;
						}
					}
				}
				else if (reader.Value == "right")
				{
					skin.Player1ScratchSide = ScratchSide.Right;
				}
				else if (reader.Value == "left")
				{
					skin.Player1ScratchSide = ScratchSide.Left;
				}
			}
			while (reader.MoveToNextAttribute());
			reader.MoveToElement();
		}

		// <group, button, playarea>
		private void ReadGroup()
		{
			string localName2 = reader.LocalName;
			SkinItemGroupBase skinItemGroupBase;
			if (localName2 == "button")
			{
				skinItemGroupBase = new ButtonSkinItem(skin);
			}
			else
			{
				skinItemGroupBase = ((!(localName2 == "playarea")) ? ((SkinItemGroupBase)new SkinItemGroup(skin)) : ((SkinItemGroupBase)new PlayAreaSkinItem(skin)));
			}
			if (reader.MoveToFirstAttribute())
			{
				do
				{
					switch (reader.LocalName)
					{
					case "id":
						skinItemGroupBase.ID = reader.Value;
						continue;
					case "dst_timer":
						skinItemGroupBase.Destination.DestinationTimer = ConvertTimerString(reader.Value);
						continue;
					case "loop":
						skinItemGroupBase.Destination.DestinationLoop = skin.GetIntValue(reader.Value);
						continue;
					case "condition":
						AddConditions(skinItemGroupBase.Destination, reader.Value);
						continue;
					}
					SkinItemGroupBase skinItemGroupBase2 = skinItemGroupBase;
					if (skinItemGroupBase2 == null)
					{
						continue;
					}
					if (!(skinItemGroupBase2 is ButtonSkinItem buttonSkinItem))
					{
						if (skinItemGroupBase2 is PlayAreaSkinItem playAreaSkinItem)
						{
							PlayAreaSkinItem playAreaSkinItem2 = playAreaSkinItem;
							switch (reader.LocalName)
							{
							case "side":
								playAreaSkinItem2.Side = skin.GetIntValue(reader.Value);
								break;
							case "range_x":
								playAreaSkinItem2.RangeX = skin.GetIntValue(reader.Value);
								break;
							case "range_y":
								playAreaSkinItem2.RangeY = skin.GetIntValue(reader.Value);
								break;
							}
						}
						continue;
					}
					ButtonSkinItem buttonSkinItem2 = buttonSkinItem;
					switch (reader.LocalName)
					{
					case "command":
						buttonSkinItem2.Command = reader.Value;
						if (!string.IsNullOrEmpty(buttonSkinItem2.Command))
						{
							Action<double> action = scene?.GetButtonCommandAction(buttonSkinItem2.Command);
							if (action == null)
							{
								action = skin.GetButtonCommandAction(buttonSkinItem2.Command);
							}
							if (action == null)
							{
								action = ButtonCommands.GetAction(buttonSkinItem2.Command);
							}
							if (action != null)
							{
								buttonSkinItem2.CommandAction = action;
							}
						}
						break;
					case "keyrepeat":
					{
						if (bool.TryParse(reader.Value, out var result7))
						{
							buttonSkinItem2.IsKeyRepeatEnable = result7;
						}
						break;
					}
					case "fallthrough":
					{
						if (bool.TryParse(reader.Value, out var result6))
						{
							buttonSkinItem2.AllowFallThrough = result6;
						}
						break;
					}
					}
				}
				while (reader.MoveToNextAttribute());
				reader.MoveToElement();
			}
			groups.Add(skinItemGroupBase);
		}

		// <panel>
		private void ReadPanel()
		{
			_ = reader.LocalName;
			PanelSkinItem panelSkinItem = new PanelSkinItem(skin);
			if (reader.MoveToFirstAttribute())
			{
				do
				{
					switch (reader.LocalName)
					{
					case "id":
						panelSkinItem.ID = reader.Value;
						break;
					case "panel_group":
						panelSkinItem.PanelGroup = reader.Value;
						break;
					case "fallthrough":
					{
						if (bool.TryParse(reader.Value, out var result9))
						{
							panelSkinItem.AllowFallThrough = result9;
						}
						break;
					}
					case "keepopen":
					{
						if (bool.TryParse(reader.Value, out var result8))
						{
							panelSkinItem.KeepOpen = result8;
						}
						break;
					}
					}
				}
				while (reader.MoveToNextAttribute());
				reader.MoveToElement();
			}
			groups.Add(panelSkinItem);
			controlGroups.Add(panelSkinItem);
		}

		// <trackbar>
		private void ReadTrackBar()
		{
			if (trackBarSkinItem != null)
			{
				return;
			}
			trackBarSkinItem = new TrackBarSkinItem(skin);
			if (!reader.MoveToFirstAttribute())
			{
				return;
			}
			do
			{
				switch (reader.LocalName)
				{
				case "id":
					trackBarSkinItem.ID = reader.Value;
					break;
				case "dst_timer":
					trackBarSkinItem.Destination.DestinationTimer = ConvertTimerString(reader.Value);
					break;
				case "loop":
					trackBarSkinItem.Destination.DestinationLoop = skin.GetIntValue(reader.Value);
					break;
				case "condition":
					AddConditions(trackBarSkinItem.Destination, reader.Value);
					break;
				case "max":
				{
					if (double.TryParse(reader.Value, out var result3))
					{
						trackBarSkinItem.Max = result3;
					}
					break;
				}
				case "min":
				{
					if (double.TryParse(reader.Value, out var result2))
					{
						trackBarSkinItem.Min = result2;
					}
					break;
				}
				case "start":
				case "end":
				{
					CornerPosition? cornerPosition = null;
					switch (reader.Value)
					{
					case "top_left":
						cornerPosition = CornerPosition.TopLeft;
						break;
					case "top_right":
						cornerPosition = CornerPosition.TopRight;
						break;
					case "bottom_left":
						cornerPosition = CornerPosition.BottomLeft;
						break;
					case "bottom_right":
						cornerPosition = CornerPosition.BottomRight;
						break;
					}
					if (cornerPosition.HasValue)
					{
						if (reader.LocalName == "start")
						{
							trackBarSkinItem.StartPosition = cornerPosition.Value;
						}
						else if (reader.LocalName == "end")
						{
							trackBarSkinItem.EndPosition = cornerPosition.Value;
						}
					}
					break;
				}
				case "value":
				{
					trackBarSkinItem.NumberID = reader.Value;
					ValueCommand<double> valueCommand = scene?.GetTrackBarCommand(trackBarSkinItem.NumberID);
					if (valueCommand == null)
					{
						valueCommand = TrackBarCommands.GetCommand(trackBarSkinItem.NumberID);
					}
					if (valueCommand != null)
					{
						trackBarSkinItem.ValueCommand = valueCommand;
					}
					break;
				}
				case "apply":
					if (reader.Value == "drag")
					{
						trackBarSkinItem.ApplyTiming = TrackBarApplyTiming.Drag;
					}
					else if (reader.Value == "release")
					{
						trackBarSkinItem.ApplyTiming = TrackBarApplyTiming.Release;
					}
					break;
				case "readonly":
					if (reader.Value == "true")
					{
						trackBarSkinItem.IsReadOnly = true;
					}
					else
					{
						trackBarSkinItem.IsReadOnly = false;
					}
					break;
				case "step":
				{
					if (double.TryParse(reader.Value, out var result))
					{
						trackBarSkinItem.Step = result;
					}
					break;
				}
				}
			}
			while (reader.MoveToNextAttribute());
			reader.MoveToElement();
		}

		// <trackbar_thumb>
		private void ReadTrackBarThumb()
		{
			if (trackBarSkinItem == null)
			{
				return;
			}
			TrackBarThumbSkinItem trackBarThumbSkinItem = new TrackBarThumbSkinItem(skin, trackBarSkinItem);
			if (reader.MoveToFirstAttribute())
			{
				do
				{
					switch (reader.LocalName)
					{
					case "id":
						trackBarThumbSkinItem.ID = reader.Value;
						break;
					case "dst_timer":
						trackBarThumbSkinItem.Destination.DestinationTimer = ConvertTimerString(reader.Value);
						break;
					case "loop":
						trackBarThumbSkinItem.Destination.DestinationLoop = skin.GetIntValue(reader.Value);
						break;
					case "condition":
						AddConditions(trackBarThumbSkinItem.Destination, reader.Value);
						break;
					}
				}
				while (reader.MoveToNextAttribute());
				reader.MoveToElement();
			}
			groups.Add(trackBarThumbSkinItem);
		}
	}
}
