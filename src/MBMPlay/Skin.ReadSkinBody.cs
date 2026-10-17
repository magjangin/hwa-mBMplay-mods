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
	private static void ReadSkinBody(XmlReader reader, Skin skin, Scene scene, ISkinItemGroup parentGroup = null)
	{
		new SkinBodyReader(reader, skin, scene, parentGroup).Read();
	}

	// 스킨 XML의 <body>를 읽는다. 원래 1800줄짜리 ReadSkinBody 한 메서드였던 것을 지역 변수는 필드로,
	// 시작 태그별 처리는 Read*() 메서드로 옮겼다(Skin.ReadSkinBody.*.cs).
	private sealed partial class SkinBodyReader
	{
		private readonly XmlReader reader;

		private readonly Skin skin;

		private readonly Scene scene;

		private readonly ISkinItemGroup parentGroup;

		private ISkinItem skinItem = null;

		private SkinDestinationContainer skinDestinationContainer = null;

		private int key = 0;

		private bool flag = false;

		private int num = 0;

		private bool flag2 = false;

		private List<ISkinItemGroup> groups = new List<ISkinItemGroup>();

		private TrackBarSkinItem trackBarSkinItem = null;

		private List<IControlSkinItemGroup> controlGroups = new List<IControlSkinItemGroup>();

		private string localName;

		public SkinBodyReader(XmlReader reader, Skin skin, Scene scene, ISkinItemGroup parentGroup)
		{
			this.reader = reader;
			this.skin = skin;
			this.scene = scene;
			this.parentGroup = parentGroup;
		}

		public void Read()
		{
			if (reader.NodeType != XmlNodeType.Element || !(reader.LocalName == "body"))
			{
				reader.ReadToFollowing("body");
			}
			while (flag || reader.Read())
			{
				if (flag)
				{
					flag = false;
				}
				if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "body")
				{
					break;
				}
				if (reader.NodeType == XmlNodeType.Element)
				{
					localName = reader.LocalName;
					if (!(localName == "elseif") && !(localName == "else"))
					{
						flag2 = false;
					}
					switch (reader.LocalName)
					{
					case "texture":
					case "font":
						ReadTextureOrFont();
						break;
					case "sc_side":
						ReadScratchSide();
						break;
					case "group":
					case "button":
					case "playarea":
						ReadGroup();
						break;
					case "panel":
						ReadPanel();
						break;
					case "trackbar":
						ReadTrackBar();
						break;
					case "trackbar_thumb":
						ReadTrackBarThumb();
						break;
					case "image":
					case "number":
					case "bargraph":
					case "slider":
					case "lanecover":
					case "barline":
					case "judge":
					case "combo":
						ReadDrawableItem();
						break;
					case "text":
						ReadText();
						break;
					case "bga":
						ReadBga();
						break;
					case "lane":
						ReadLane();
						break;
					case "note":
					case "ln_start":
					case "ln_end":
					case "ln_body":
					case "ln_start_holding":
					case "ln_end_holding":
					case "ln_body_holding":
					case "ln_start_released":
					case "ln_end_released":
					case "ln_body_released":
						ReadNote();
						break;
					case "gauge":
						ReadGauge();
						break;
					case "gauge_dst":
					case "panel_dst":
						ReadGroupDestination();
						break;
					case "dst":
						ReadDestination();
						break;
					case "include":
						ReadInclude();
						break;
					case "var":
						ReadVar();
						break;
					case "set":
						ReadSet();
						break;
					case "if":
						ReadIf();
						break;
					case "elseif":
					case "else":
						ReadElse();
						break;
					}
				}
				else
				{
					if (reader.NodeType != XmlNodeType.EndElement)
					{
						continue;
					}
					ReadEndElement();
				}
			}
		}

		private void ReadEndElement()
		{
			switch (reader.LocalName)
			{
			case "panel":
				if (groups.Count <= 0)
				{
					break;
				}
				if (groups[groups.Count - 1] is PanelSkinItem panelSkinItem3)
				{
					if (groups.Count >= 2)
					{
						panelSkinItem3.ParentGroup = groups[groups.Count - 2];
						groups[groups.Count - 2].Items.Add(panelSkinItem3);
					}
					else
					{
						skin.Items.Add(panelSkinItem3);
					}
					skin.PanelManager.Add(panelSkinItem3);
				}
				groups.RemoveAt(groups.Count - 1);
				if (controlGroups[controlGroups.Count - 1] is PanelSkinItem control)
				{
					controlGroups.RemoveAt(controlGroups.Count - 1);
					AddControlReference(control);
				}
				break;
			case "button":
				if (groups.Count <= 0)
				{
					break;
				}
				if (groups[groups.Count - 1] is ButtonSkinItem buttonSkinItem3)
				{
					AddControlReference(buttonSkinItem3);
					if (groups.Count >= 2)
					{
						buttonSkinItem3.ParentGroup = groups[groups.Count - 2];
						groups[groups.Count - 2].Items.Add(buttonSkinItem3);
					}
					else
					{
						skin.Items.Add(buttonSkinItem3);
					}
				}
				groups.RemoveAt(groups.Count - 1);
				break;
			case "group":
				if (groups.Count <= 0)
				{
					break;
				}
				if (groups[groups.Count - 1] is SkinItemGroup skinItemGroup)
				{
					if (groups.Count >= 2)
					{
						skinItemGroup.ParentGroup = groups[groups.Count - 2];
						groups[groups.Count - 2].Items.Add(skinItemGroup);
					}
					else
					{
						skin.Items.Add(skinItemGroup);
					}
				}
				groups.RemoveAt(groups.Count - 1);
				break;
			case "playarea":
				if (groups.Count <= 0)
				{
					break;
				}
				if (groups[groups.Count - 1] is PlayAreaSkinItem playAreaSkinItem3)
				{
					if (groups.Count >= 2)
					{
						playAreaSkinItem3.ParentGroup = groups[groups.Count - 2];
						groups[groups.Count - 2].Items.Add(playAreaSkinItem3);
					}
					else
					{
						skin.Items.Add(playAreaSkinItem3);
					}
				}
				groups.RemoveAt(groups.Count - 1);
				break;
			case "trackbar":
				if (trackBarSkinItem != null)
				{
					AddSkinItem(trackBarSkinItem);
					AddControlReference(trackBarSkinItem);
				}
				trackBarSkinItem = null;
				break;
			case "trackbar_thumb":
				if (groups.Count > 0)
				{
					if (trackBarSkinItem != null && groups[groups.Count - 1] is TrackBarThumbSkinItem thumb)
					{
						trackBarSkinItem.Thumb = thumb;
					}
					groups.RemoveAt(groups.Count - 1);
				}
				break;
			case "image":
			case "bargraph":
			case "slider":
			case "lanecover":
			case "bga":
			case "gauge":
			case "text":
				if (skinItem != null)
				{
					AddSkinItem(skinItem);
				}
				skinItem = null;
				break;
			case "number":
				if (skinItem is NumberSkinItem numberSkinItem)
				{
					if (numberSkinItem.DivisionX * numberSkinItem.DivisionY % 10 == 0)
					{
						numberSkinItem.NumberType = 10;
					}
					else if (numberSkinItem.DivisionX * numberSkinItem.DivisionY % 11 == 0)
					{
						numberSkinItem.NumberType = 11;
					}
					else if (numberSkinItem.DivisionX * numberSkinItem.DivisionY % 24 == 0)
					{
						numberSkinItem.NumberType = 24;
					}
					AddSkinItem(numberSkinItem);
				}
				skinItem = null;
				break;
			case "barline":
				if (skinItem is BarlineSkinItem { Side: var side } barlineSkinItem)
				{
					if (!skin.barlineRefs.ContainsKey(side))
					{
						skin.barlineRefs.Add(side, new BarlineItemReferenceCollection());
					}
					skin.barlineRefs[side].Add(new WeakReference<BarlineSkinItem>(barlineSkinItem));
					AddSkinItem(barlineSkinItem);
				}
				skinItem = null;
				break;
			case "lane":
				if (skinItem is LaneSkinItem laneSkinItem2 && laneSkinItem2.NoteSources.ContainsKey("note"))
				{
					if (!laneSkinItem2.NoteSources.ContainsKey("ln_start"))
					{
						laneSkinItem2.NoteSources.Add("ln_start", laneSkinItem2.NoteSources["note"]);
					}
					if (!laneSkinItem2.NoteSources.ContainsKey("ln_end"))
					{
						laneSkinItem2.NoteSources.Add("ln_end", laneSkinItem2.NoteSources["note"]);
					}
					if (!laneSkinItem2.NoteSources.ContainsKey("ln_body"))
					{
						laneSkinItem2.NoteSources.Add("ln_body", laneSkinItem2.NoteSources["note"]);
					}
					if (!laneSkinItem2.NoteSources.ContainsKey("ln_start_holding"))
					{
						laneSkinItem2.NoteSources.Add("ln_start_holding", laneSkinItem2.NoteSources["ln_start"]);
					}
					if (!laneSkinItem2.NoteSources.ContainsKey("ln_end_holding"))
					{
						laneSkinItem2.NoteSources.Add("ln_end_holding", laneSkinItem2.NoteSources["ln_end"]);
					}
					if (!laneSkinItem2.NoteSources.ContainsKey("ln_body_holding"))
					{
						laneSkinItem2.NoteSources.Add("ln_body_holding", laneSkinItem2.NoteSources["ln_body"]);
					}
					if (!laneSkinItem2.NoteSources.ContainsKey("ln_start_released"))
					{
						laneSkinItem2.NoteSources.Add("ln_start_released", laneSkinItem2.NoteSources["ln_start"]);
					}
					if (!laneSkinItem2.NoteSources.ContainsKey("ln_end_released"))
					{
						laneSkinItem2.NoteSources.Add("ln_end_released", laneSkinItem2.NoteSources["ln_end"]);
					}
					if (!laneSkinItem2.NoteSources.ContainsKey("ln_body_released"))
					{
						laneSkinItem2.NoteSources.Add("ln_body_released", laneSkinItem2.NoteSources["ln_body"]);
					}
					if (!skin.noteRefs.ContainsKey(laneSkinItem2.LaneIndex))
					{
						skin.noteRefs.Add(laneSkinItem2.LaneIndex, new LaneItemReferenceCollection());
					}
					skin.noteRefs[laneSkinItem2.LaneIndex].Add(new WeakReference<LaneSkinItem>(laneSkinItem2));
					AddSkinItem(laneSkinItem2);
				}
				skinItem = null;
				break;
			case "judge":
				if (skinItem is JudgeSkinItem judgeSkinItem)
				{
					AddSkinItem(judgeSkinItem);
					if (!skin.judgeRefs.ContainsKey(judgeSkinItem.JudgeID))
					{
						skin.judgeRefs.Add(judgeSkinItem.JudgeID, judgeSkinItem);
					}
				}
				skinItem = null;
				break;
			case "combo":
				if (skinItem is ComboSkinItem comboSkinItem)
				{
					if (comboSkinItem.DivisionX * comboSkinItem.DivisionY % 10 == 0)
					{
						comboSkinItem.NumberType = 10;
					}
					else if (comboSkinItem.DivisionX * comboSkinItem.DivisionY % 11 == 0)
					{
						comboSkinItem.NumberType = 11;
					}
					else if (comboSkinItem.DivisionX * comboSkinItem.DivisionY % 24 == 0)
					{
						comboSkinItem.NumberType = 24;
					}
					AddSkinItem(comboSkinItem);
					if (!skin.comboRefs.ContainsKey(comboSkinItem.JudgeID))
					{
						skin.comboRefs.Add(comboSkinItem.JudgeID, comboSkinItem);
					}
				}
				skinItem = null;
				break;
			case "gauge_dst":
				if (skinDestinationContainer != null)
				{
					((MultipleDestinationSkinItem)skinItem).Destinations.Add(key, skinDestinationContainer);
				}
				skinDestinationContainer = null;
				key = 0;
				break;
			case "panel_dst":
				if (skinDestinationContainer != null && groups[groups.Count - 1] is PanelSkinItem panelSkinItem2)
				{
					panelSkinItem2.Destinations[key] = skinDestinationContainer;
				}
				skinDestinationContainer = null;
				key = 0;
				break;
			case "if":
			case "elseif":
			case "else":
				num--;
				flag2 = true;
				break;
			}
		}

		private void AddConditions(SkinDestinationContainer destinationContainer, string condition)
		{
			string[] array = condition.Replace(" ", "").Split(',');
			foreach (string text8 in array)
			{
				int num2 = ((text8[0] == '!') ? 1 : 0);
				if (text8.Substring(num2).StartsWith("button_", StringComparison.Ordinal))
				{
					Match match = Regex.Match(text8.Substring(num2), "^button_([a-z_]+)");
					if (match.Success && match.Groups.Count >= 2)
					{
						destinationContainer.Conditions.Add(string.Format("{0}$_button[{1}][{2}]", (num2 == 1) ? "!" : "", skin.buttonRefs.Count, match.Groups[1]));
					}
				}
				else if (text8.Substring(num2).StartsWith("trackbar_", StringComparison.Ordinal))
				{
					Match match2 = Regex.Match(text8.Substring(num2), "^trackbar_([a-z_]+)");
					if (match2.Success && match2.Groups.Count >= 2)
					{
						destinationContainer.Conditions.Add(string.Format("{0}$_trackbar[{1}][{2}]", (num2 == 1) ? "!" : "", skin.trackBarRefs.Count, match2.Groups[1]));
					}
				}
				else if (text8.Substring(num2).StartsWith("panel_", StringComparison.Ordinal))
				{
					Match match3 = Regex.Match(text8.Substring(num2), "^panel_([a-z_]+)");
					if (match3.Success && match3.Groups.Count >= 2)
					{
						ISkinItemGroup skinItemGroup2 = null;
						if (groups.Count > 0)
						{
							skinItemGroup2 = groups[groups.Count - 1];
						}
						else if (parentGroup != null)
						{
							skinItemGroup2 = parentGroup;
						}
						if (skinItemGroup2 is PanelSkinItem panelSkinItem4)
						{
							destinationContainer.Conditions.Add(string.Format("{0}$_panel[{1}][{2}]", (num2 == 1) ? "!" : "", panelSkinItem4.ID, match3.Groups[1]));
						}
					}
				}
				else
				{
					destinationContainer.Conditions.Add(text8);
				}
			}
		}

		private void AddControlReference(IControlSkinItem controlSkinItem)
		{
			if (controlGroups.Count > 0)
			{
				controlGroups[controlGroups.Count - 1].AddControlReference(controlSkinItem);
			}
			else
			{
				skin.controlRefs.Add(new WeakReference<IControlSkinItem>(controlSkinItem));
			}
			if (controlSkinItem != null)
			{
				if (!(controlSkinItem is ButtonSkinItem buttonSkinItem4))
				{
					if (controlSkinItem is TrackBarSkinItem trackBarSkinItem2)
					{
						TrackBarSkinItem target = trackBarSkinItem2;
						skin.trackBarRefs.Add(new WeakReference<TrackBarSkinItem>(target));
					}
				}
				else
				{
					ButtonSkinItem target2 = buttonSkinItem4;
					skin.buttonRefs.Add(new WeakReference<ButtonSkinItem>(target2));
				}
			}
		}

		private void AddSkinItem(ISkinItem skinItem2)
		{
			ISkinItemGroup skinItemGroup2 = null;
			if (groups.Count > 0)
			{
				skinItemGroup2 = groups[groups.Count - 1];
			}
			else if (parentGroup != null)
			{
				skinItemGroup2 = parentGroup;
			}
			if (skinItemGroup2 != null)
			{
				if (skinItem2 is SingleDestinationSkinItem singleDestinationSkinItem2)
				{
					singleDestinationSkinItem2.ParentGroup = skinItemGroup2;
				}
				else if (skinItem2 is MultipleDestinationSkinItem multipleDestinationSkinItem)
				{
					multipleDestinationSkinItem.ParentGroup = skinItemGroup2;
				}
				skinItemGroup2.Items.Add(skinItem2);
			}
			else
			{
				skin.Items.Add(skinItem2);
			}
			if (skinItem2 is IDrawableSkinItem drawableSkinItem && !string.IsNullOrEmpty(drawableSkinItem.ID) && !skin.idDictionary.ContainsKey(drawableSkinItem.ID))
			{
				skin.idDictionary.Add(drawableSkinItem.ID, new WeakReference<ISkinItem>(skinItem2));
			}
		}

		private string ConvertTimerString(string timer)
		{
			if (timer.StartsWith("button_", StringComparison.Ordinal))
			{
				Match match = Regex.Match(timer, "^button_([a-z_]+)");
				if (match.Success && match.Groups.Count >= 2)
				{
					return $"$_button[{skin.buttonRefs.Count}][{match.Groups[1]}]";
				}
			}
			else if (timer.StartsWith("trackbar_", StringComparison.Ordinal))
			{
				Match match2 = Regex.Match(timer, "^trackbar_([a-z_]+)");
				if (match2.Success && match2.Groups.Count >= 2)
				{
					return $"$_trackbar[{skin.trackBarRefs.Count}][{match2.Groups[1]}]";
				}
			}
			else if (timer.StartsWith("panel_", StringComparison.Ordinal))
			{
				Match match3 = Regex.Match(timer, "^panel_([a-z_]+)");
				if (match3.Success && match3.Groups.Count >= 2)
				{
					ISkinItemGroup skinItemGroup2 = null;
					if (groups.Count > 0)
					{
						skinItemGroup2 = groups[groups.Count - 1];
					}
					else if (parentGroup != null)
					{
						skinItemGroup2 = parentGroup;
					}
					if (skinItemGroup2 is PanelSkinItem panelSkinItem4)
					{
						return $"$_panel[{panelSkinItem4.ID}][{match3.Groups[1]}]";
					}
				}
			}
			return timer;
		}
	}
}
