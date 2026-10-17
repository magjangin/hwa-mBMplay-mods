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

public partial class Skin : SkinInfo
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
