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

public class Skin : SkinInfo
{
	private static readonly ReadOnlyDictionary<string, BlendType> blendTypeDict = Extensions.AsReadOnly(new Dictionary<string, BlendType>
	{
		{
			"alpha",
			BlendType.Alpha
		},
		{
			"additive",
			BlendType.AlphaAdditive
		},
		{
			"subtract",
			BlendType.Subtract
		},
		{
			"multiply",
			BlendType.Multiply
		},
		{
			"screen",
			BlendType.Screen
		}
	});

	private readonly ReadOnlyDictionary<string, Func<IList<string>, Action<double>>> ButtonCommandDictionary;

	private Dictionary<string, int> intVars = new Dictionary<string, int>();

	private Dictionary<string, bool> boolVars = new Dictionary<string, bool>();

	private Dictionary<int, LaneItemReferenceCollection> noteRefs = new Dictionary<int, LaneItemReferenceCollection>();

	private Dictionary<int, BarlineItemReferenceCollection> barlineRefs = new Dictionary<int, BarlineItemReferenceCollection>();

	private List<WeakReference<IControlSkinItem>> controlRefs = new List<WeakReference<IControlSkinItem>>();

	private List<WeakReference<ButtonSkinItem>> buttonRefs = new List<WeakReference<ButtonSkinItem>>();

	private List<WeakReference<TrackBarSkinItem>> trackBarRefs = new List<WeakReference<TrackBarSkinItem>>();

	private int activeControlIndex = -1;

	private Dictionary<string, WeakReference<ISkinItem>> idDictionary = new Dictionary<string, WeakReference<ISkinItem>>();

	private Dictionary<int, JudgeSkinItem> judgeRefs = new Dictionary<int, JudgeSkinItem>();

	private Dictionary<int, ComboSkinItem> comboRefs = new Dictionary<int, ComboSkinItem>();

	public List<ISkinItem> Items { get; private set; } = new List<ISkinItem>();

	public Dictionary<string, string> Textures { get; private set; } = new Dictionary<string, string>();

	public Dictionary<string, string> Fonts { get; private set; } = new Dictionary<string, string>();

	public PanelManager PanelManager { get; private set; } = new PanelManager();

	public ScratchSide Player1ScratchSide { get; private set; }

	public ScratchSide Player2ScratchSide { get; private set; }

	private ReadOnlyDictionary<string, Func<IList<string>, Action<double>>> CreateButtonCommandDictionary()
	{
		return Extensions.AsReadOnly(new Dictionary<string, Func<IList<string>, Action<double>>>
		{
			{
				"panel_action",
				(IList<string> args) =>
				{
					string s = ((args != null && args.Count > 0) ? args[0] : "");
					return (double time) =>
					{
						PanelManager.OpenPanel(s, time, closeIfOpened: true);
					};
				}
			},
			{
				"panel_close",
				(IList<string> args) =>
				{
					string s = ((args != null && args.Count > 0) ? args[0] : "");
					return (double time) =>
					{
						PanelManager.ClosePanel(s, time);
					};
				}
			}
		});
	}

	private Skin()
	{
		ButtonCommandDictionary = CreateButtonCommandDictionary();
	}

	public static Skin Load(string filepath, Scene scene)
	{
		if (filepath == null)
		{
			throw new ArgumentNullException("filepath");
		}
		string path = Utils.GetPath(filepath);
		Path.GetDirectoryName(path);
		if (!File.Exists(path))
		{
			return null;
		}
		Skin skin = new Skin
		{
			SkinPath = path,
			SkinDirectory = Path.GetDirectoryName(path)
		};
		XmlReaderSettings settings = new XmlReaderSettings
		{
			IgnoreComments = true,
			IgnoreWhitespace = true
		};
		using (FileStream fileStream = new FileStream(path, FileMode.Open))
		{
			skin.Hash = Utils.EncodeSHA256(fileStream);
			fileStream.Position = 0L;
			using XmlReader reader = XmlReader.Create(fileStream, settings);
			if (!SkinInfo.GetSkinInfo(reader, skin))
			{
				return null;
			}
			SkinInfo.LoadSkinOptions(skin, Program.StartupPath, Program.SettingsDirectoryPath, updateSkinOptionValues: true, Program.DefaultEncoding);
			ReadSkinBody(reader, skin, scene);
		}
		foreach (KeyValuePair<int, JudgeSkinItem> judgeRef in skin.judgeRefs)
		{
			if (skin.comboRefs.ContainsKey(judgeRef.Value.JudgeID))
			{
				judgeRef.Value.ComboItem = skin.comboRefs[judgeRef.Value.JudgeID];
				skin.comboRefs[judgeRef.Value.JudgeID].JudgeItem = judgeRef.Value;
			}
		}
		skin.judgeRefs.Clear();
		skin.judgeRefs = null;
		skin.comboRefs.Clear();
		skin.comboRefs = null;
		return skin;
	}

	public static Skin Load(string filepath)
	{
		return Load(filepath, null);
	}

	private static void ReadSkinBody(XmlReader reader, Skin skin, Scene scene, ISkinItemGroup parentGroup = null)
	{
		ISkinItem skinItem = null;
		SkinDestinationContainer skinDestinationContainer = null;
		int key = 0;
		bool flag = false;
		int num = 0;
		bool flag2 = false;
		List<ISkinItemGroup> groups = new List<ISkinItemGroup>();
		TrackBarSkinItem trackBarSkinItem = null;
		List<IControlSkinItemGroup> controlGroups = new List<IControlSkinItemGroup>();
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
				string localName = reader.LocalName;
				if (!(localName == "elseif") && !(localName == "else"))
				{
					flag2 = false;
				}
				switch (reader.LocalName)
				{
				case "texture":
				case "font":
				{
					if (!reader.MoveToFirstAttribute())
					{
						break;
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
					break;
				}
				case "sc_side":
					if (!reader.MoveToFirstAttribute())
					{
						break;
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
					break;
				case "group":
				case "button":
				case "playarea":
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
					break;
				}
				case "panel":
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
					break;
				}
				case "trackbar":
					if (trackBarSkinItem != null)
					{
						break;
					}
					trackBarSkinItem = new TrackBarSkinItem(skin);
					if (!reader.MoveToFirstAttribute())
					{
						break;
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
					break;
				case "trackbar_thumb":
				{
					if (trackBarSkinItem == null)
					{
						break;
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
					break;
				}
				case "image":
				case "number":
				case "bargraph":
				case "slider":
				case "lanecover":
				case "barline":
				case "judge":
				case "combo":
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
					break;
				}
				case "text":
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
					break;
				}
				case "bga":
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
					break;
				}
				case "lane":
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
					break;
				}
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
				{
					if (!(skinItem is LaneSkinItem))
					{
						break;
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
					break;
				}
				case "gauge":
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
					break;
				}
				case "gauge_dst":
				case "panel_dst":
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
					break;
				}
				case "dst":
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
						break;
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
					break;
				}
				case "include":
				{
					if (!reader.MoveToFirstAttribute())
					{
						break;
					}
					string path = "";
					do
					{
						if (reader.LocalName == "src")
						{
							path = skin.GetFilePath(reader.Value);
						}
					}
					while (reader.MoveToNextAttribute());
					reader.MoveToElement();
					if (!File.Exists(path))
					{
						break;
					}
					XmlReaderSettings settings = new XmlReaderSettings
					{
						IgnoreComments = true,
						IgnoreWhitespace = true
					};
					using (FileStream input = new FileStream(path, FileMode.Open))
					{
						using XmlReader xmlReader = XmlReader.Create(input, settings);
						bool flag3 = false;
						xmlReader.ReadToFollowing("skin");
						if (xmlReader.MoveToFirstAttribute())
						{
							do
							{
								if (xmlReader.LocalName == "type" && xmlReader.Value == "include")
								{
									flag3 = true;
								}
							}
							while (xmlReader.MoveToNextAttribute());
							xmlReader.MoveToElement();
						}
						if (flag3)
						{
							ISkinItemGroup parentGroup2 = null;
							if (groups.Count > 0)
							{
								parentGroup2 = groups[groups.Count - 1];
							}
							else if (parentGroup != null)
							{
								parentGroup2 = parentGroup;
							}
							ReadSkinBody(xmlReader, skin, scene, parentGroup2);
						}
					}
					break;
				}
				case "var":
				{
					if (!reader.MoveToFirstAttribute())
					{
						break;
					}
					string text = "";
					string text2 = "";
					string text3 = "";
					do
					{
						switch (reader.LocalName)
						{
						case "id":
							text = reader.Value;
							break;
						case "type":
							text2 = reader.Value;
							break;
						case "value":
							text3 = reader.Value;
							break;
						}
					}
					while (reader.MoveToNextAttribute());
					reader.MoveToElement();
					if (!string.IsNullOrEmpty(text) && !skin.intVars.ContainsKey(text) && !skin.boolVars.ContainsKey(text))
					{
						if (text2 == "bool")
						{
							skin.boolVars.Add(text, bool.TryParse(text3, out var result4) && result4);
						}
						else if (text2 == "int")
						{
							skin.intVars.Add(text, int.TryParse(text3, out var result5) ? result5 : 0);
						}
					}
					break;
				}
				case "set":
				{
					if (!reader.MoveToFirstAttribute())
					{
						break;
					}
					string text6 = "";
					string text7 = "";
					do
					{
						localName = reader.LocalName;
						if (!(localName == "id"))
						{
							if (localName == "value")
							{
								text7 = reader.Value;
							}
						}
						else
						{
							text6 = reader.Value;
						}
					}
					while (reader.MoveToNextAttribute());
					reader.MoveToElement();
					if (!string.IsNullOrEmpty(text6))
					{
						bool result11;
						if (skin.intVars.ContainsKey(text6) && int.TryParse(text7, out var result10))
						{
							skin.intVars[text6] = result10;
						}
						else if (skin.boolVars.ContainsKey(text6) && bool.TryParse(text7, out result11))
						{
							skin.boolVars[text6] = result11;
						}
					}
					break;
				}
				case "if":
				{
					List<string> list = new List<string>();
					if (reader.MoveToFirstAttribute())
					{
						do
						{
							localName = reader.LocalName;
							if (localName == "condition")
							{
								list.AddRange(reader.Value.Replace(" ", "").Split(','));
							}
						}
						while (reader.MoveToNextAttribute());
						reader.MoveToElement();
					}
					if (skin.CheckConditions(list))
					{
						num++;
						break;
					}
					reader.Skip();
					flag = true;
					break;
				}
				case "elseif":
				case "else":
					if (flag2)
					{
						reader.Skip();
						flag = true;
						break;
					}
					if (!(reader.LocalName == "elseif"))
					{
						if (reader.LocalName == "else")
						{
							num++;
						}
						break;
					}
					goto case "if";
				}
			}
			else
			{
				if (reader.NodeType != XmlNodeType.EndElement)
				{
					continue;
				}
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
		}
		void AddConditions(SkinDestinationContainer destinationContainer, string condition)
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
		void AddControlReference(IControlSkinItem controlSkinItem)
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
		void AddSkinItem(ISkinItem skinItem2)
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
		string ConvertTimerString(string timer)
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

	private static bool TryGetRoundMethod(string value, out RoundMethod method)
	{
		RoundMethod? roundMethod = null;
		switch (value)
		{
		case "floor":
			roundMethod = RoundMethod.Floor;
			break;
		case "ceiling":
			roundMethod = RoundMethod.Ceiling;
			break;
		case "truncate":
			roundMethod = RoundMethod.Truncate;
			break;
		case "round":
			roundMethod = RoundMethod.Round;
			break;
		case "away_from_zero":
			roundMethod = RoundMethod.AwayFromZero;
			break;
		}
		if (roundMethod.HasValue)
		{
			method = roundMethod.Value;
			return true;
		}
		method = RoundMethod.Round;
		return false;
	}

	public void UpdateControls(double time)
	{
		bool isMouseProcessed = false;
		int num = activeControlIndex;
		activeControlIndex = -1;
		if (num >= 0 && num < controlRefs.Count)
		{
			UpdateControl(num);
		}
		for (int num2 = controlRefs.Count - 1; num2 >= 0; num2--)
		{
			if (num2 != num)
			{
				UpdateControl(num2);
			}
		}
		PanelManager.CloseRequested(time);
		void UpdateControl(int index)
		{
			if (controlRefs[index].TryGetTarget(out var target))
			{
				bool flag = target.Update(time, isMouseProcessed);
				if (flag)
				{
					isMouseProcessed = flag;
				}
				if (target.IsActive && activeControlIndex < 0)
				{
					activeControlIndex = index;
				}
			}
		}
	}

	internal string GetFilePath(string basePath)
	{
		if (string.IsNullOrEmpty(basePath))
		{
			return "";
		}
		string text = basePath;
		string pattern = "%([^%]+)%";
		Match match = Regex.Match(text, pattern);
		if (match.Success)
		{
			string value = match.Groups[1].Value;
			if (CustomFiles.ContainsKey(value))
			{
				string targetPath = CustomFiles[value].GetTargetPath(SkinDirectory, Program.Random.NextLong());
				text = (string.IsNullOrEmpty(targetPath) ? "" : Regex.Replace(text, pattern, targetPath));
			}
			else
			{
				text = "";
			}
		}
		if (string.IsNullOrEmpty(text))
		{
			return "";
		}
		return Utils.GetPath(SkinDirectory + Path.DirectorySeparatorChar + text);
	}

	public List<Rectangle> GetLaneDestinations(int lane, double time)
	{
		List<Rectangle> list = new List<Rectangle>();
		if (noteRefs.ContainsKey(lane) && noteRefs[lane].Count > 0)
		{
			for (int i = 0; i < noteRefs[lane].Count; i++)
			{
				if (noteRefs[lane][i].TryGetTarget(out var target) && target != null)
				{
					list.Add(target.GetDestinationRectangle(time, Point.Empty));
				}
			}
		}
		return list;
	}

	public List<Rectangle> GetBarlineDestinations(int side, double time)
	{
		List<Rectangle> list = new List<Rectangle>();
		if (barlineRefs.ContainsKey(side) && barlineRefs[side].Count > 0)
		{
			for (int i = 0; i < barlineRefs[side].Count; i++)
			{
				if (barlineRefs[side][i].TryGetTarget(out var target) && target != null)
				{
					list.Add(target.GetDestinationRectangle(time, Point.Empty));
				}
			}
		}
		return list;
	}

	public bool CheckConditions(IList<string> conditions)
	{
		if (conditions.Count <= 0)
		{
			return true;
		}
		foreach (string condition in conditions)
		{
			if (string.IsNullOrEmpty(condition))
			{
				continue;
			}
			if (condition[0] == '!' && condition.Length >= 2)
			{
				if (GetFlagValue(condition.Substring(1)))
				{
					return false;
				}
			}
			else if (!GetFlagValue(condition))
			{
				return false;
			}
		}
		return true;
	}

	public ISkinItem GetSkinItemByID(string id)
	{
		if (idDictionary.TryGetValue(id, out var value) && value.TryGetTarget(out var target))
		{
			return target;
		}
		return null;
	}

	public Action<double> GetButtonCommandAction(string command)
	{
		if (ButtonCommands.ParseCommand(command, out var name, out var args) && ButtonCommandDictionary.ContainsKey(name))
		{
			return ButtonCommandDictionary[name]?.Invoke(args) ?? null;
		}
		return null;
	}

	private bool GetFlagValue(string name)
	{
		if (name[0] == '$')
		{
			if (name.StartsWith("$_option", StringComparison.Ordinal))
			{
				string[] array = name.Replace("]", "").Split('[');
				if (!CustomOptions.ContainsKey(array[1]))
				{
					return false;
				}
				return CustomOptions[array[1]].CheckValue(array[2]);
			}
			if (name.StartsWith("$_button", StringComparison.Ordinal))
			{
				string[] array2 = name.Replace("]", "").Split('[');
				if (array2.Length >= 3 && int.TryParse(array2[1], out var result) && result < buttonRefs.Count && buttonRefs[result].TryGetTarget(out var target))
				{
					return target.Flags.GetValue(array2[2]);
				}
			}
			else if (name.StartsWith("$_trackbar", StringComparison.Ordinal))
			{
				string[] array3 = name.Replace("]", "").Split('[');
				if (array3.Length >= 3 && int.TryParse(array3[1], out var result2) && result2 < trackBarRefs.Count && trackBarRefs[result2].TryGetTarget(out var target2))
				{
					if (array3[2].StartsWith("thumb_", StringComparison.Ordinal))
					{
						string name2 = array3[2].Replace("thumb_", "");
						return target2.Thumb.Flags.GetValue(name2);
					}
					return target2.Flags.GetValue(array3[2]);
				}
			}
			else
			{
				if (!name.StartsWith("$_panel", StringComparison.Ordinal))
				{
					string key = name.Substring(1);
					if (!boolVars.ContainsKey(key))
					{
						return false;
					}
					return boolVars[key];
				}
				string[] array4 = name.Replace("]", "").Split('[');
				if (array4.Length >= 3 && PanelManager.TryGetPanel(array4[1], out var panel))
				{
					return panel.Flags.GetValue(array4[2]);
				}
			}
			return false;
		}
		return Flags.GetValue(name);
	}

	private int GetIntValue(string value)
	{
		if (value[0] == '$')
		{
			string key = value.Substring(1);
			if (!intVars.ContainsKey(key))
			{
				return 0;
			}
			return intVars[key];
		}
		if (!int.TryParse(value, out var result))
		{
			return 0;
		}
		return result;
	}

	internal double GetTimerValue(string name)
	{
		if (name[0] == '$')
		{
			if (name.StartsWith("$_button", StringComparison.Ordinal))
			{
				string[] array = name.Replace("]", "").Split('[');
				if (array.Length >= 3 && int.TryParse(array[1], out var result) && result < buttonRefs.Count && buttonRefs[result].TryGetTarget(out var target))
				{
					return target.Timers.GetValue(array[2]);
				}
			}
			else if (name.StartsWith("$_trackbar", StringComparison.Ordinal))
			{
				string[] array2 = name.Replace("]", "").Split('[');
				if (array2.Length >= 3 && int.TryParse(array2[1], out var result2) && result2 < trackBarRefs.Count && trackBarRefs[result2].TryGetTarget(out var target2))
				{
					if (array2[2].StartsWith("thumb_", StringComparison.Ordinal))
					{
						string name2 = array2[2].Replace("thumb_", "");
						return target2.Thumb.Timers.GetValue(name2);
					}
					return target2.Timers.GetValue(array2[2]);
				}
			}
			else if (name.StartsWith("$_panel", StringComparison.Ordinal))
			{
				string[] array3 = name.Replace("]", "").Split('[');
				if (array3.Length >= 3 && PanelManager.TryGetPanel(array3[1], out var panel))
				{
					return panel.Timers.GetValue(array3[2]);
				}
			}
			return 0.0;
		}
		return EventTimers.GetValue(name);
	}

	public static TextureInfo GenerateSolidColorTexture(GraphicsBase graphics, Color color)
	{
		byte[] data = new byte[4] { color.B, color.G, color.R, color.A };
		return graphics.CreateTexture(data, 1, 1, 4);
	}
}
