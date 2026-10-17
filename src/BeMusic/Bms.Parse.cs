using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BeMusic.Extentions;
using Randomize;

namespace BeMusic;

public partial class Bms
{
	public static Bms Parse(string path, int key, XorShift random)
	{
		string input = ReadBmsFile(path, null, random);
		MatchCollection matchCollection = BmsRegex.Data.Matches(input);
		HashSet<int> hashSet = new HashSet<int>();
		SortedSet<int> sortedSet = new SortedSet<int>();
		foreach (Match item in matchCollection)
		{
			hashSet.Add(int.Parse(item.Groups["channel"].Value));
			sortedSet.Add(int.Parse(item.Groups["bar"].Value));
		}
		MatchCollection matchCollection2 = BmsRegex.BarLength.Matches(input);
		foreach (Match item2 in matchCollection2)
		{
			sortedSet.Add(int.Parse(item2.Groups["bar"].Value));
		}
		int max = sortedSet.Max;
		sortedSet.Clear();
		int num = 5;
		if (KeyDetectionList.Contains(key))
		{
			num = key;
		}
		else
		{
			List<int> first = hashSet.ToList();
			if (key == 9)
			{
				num = ((!first.Intersect(ChannelListDP).Any()) ? (-9) : 9);
			}
			else if (first.Intersect(ChannelListDP).Any())
			{
				if (first.Intersect(ChannelList14Keys).Any())
				{
					num = 14;
				}
				else
				{
					num = ((!first.Intersect(ChannelList10Keys).Any()) ? 9 : 10);
				}
			}
			else if (first.Intersect(ChannelList7Keys).Any())
			{
				num = ((!first.Intersect(ChannelListPms).Any()) ? 7 : (-9));
			}
			else
			{
				switch (Path.GetExtension(path).ToLower())
				{
				case ".pms":
					num = -9;
					break;
				case ".bme":
				case ".bml":
					num = 7;
					break;
				default:
					num = 5;
					break;
				}
			}
		}
		double result = 130.0;
		Match match3 = BmsRegex.Bpm.Match(input);
		if (match3.Success)
		{
			double.TryParse(match3.Groups["data"].Value, out result);
		}
		string title = "";
		Match match4 = BmsRegex.Title.Match(input);
		if (match4.Success)
		{
			title = match4.Groups["data"].Value;
		}
		string subtitle = "";
		Match match5 = BmsRegex.Subtitle.Match(input);
		if (match5.Success)
		{
			subtitle = match5.Groups["data"].Value;
		}
		string artist = "";
		Match match6 = BmsRegex.Artist.Match(input);
		if (match6.Success)
		{
			artist = match6.Groups["data"].Value;
		}
		string subartist = "";
		Match match7 = BmsRegex.Subartist.Match(input);
		if (match7.Success)
		{
			subartist = match7.Groups["data"].Value;
		}
		string genre = "";
		Match match8 = BmsRegex.Genre.Match(input);
		if (match8.Success)
		{
			genre = match8.Groups["data"].Value;
		}
		int result2 = 0;
		Match match9 = BmsRegex.PlayLevel.Match(input);
		if (match9.Success)
		{
			int.TryParse(match9.Groups["data"].Value, out result2);
		}
		int? num2 = null;
		Match match10 = BmsRegex.JudgeRank.Match(input);
		if (match10.Success && int.TryParse(match10.Groups["data"].Value, out var result3))
		{
			num2 = result3;
		}
		double result4 = double.NaN;
		Match match11 = BmsRegex.Total.Match(input);
		if (match11.Success)
		{
			double.TryParse(match11.Groups["data"].Value, out result4);
		}
		string stageFile = "";
		Match match12 = BmsRegex.StageFile.Match(input);
		if (match12.Success)
		{
			stageFile = match12.Groups["data"].Value;
		}
		string backBmp = "";
		Match match13 = BmsRegex.BackBmp.Match(input);
		if (match13.Success)
		{
			backBmp = match13.Groups["data"].Value;
		}
		string banner = "";
		Match match14 = BmsRegex.Banner.Match(input);
		if (match14.Success)
		{
			banner = match14.Groups["data"].Value;
		}
		int? num3 = null;
		Match match15 = BmsRegex.Difficulty.Match(input);
		if (match15.Success && int.TryParse(match15.Groups["data"].Value, out var result5))
		{
			num3 = result5;
		}
		List<double> list = new List<double>();
		for (int i = 0; i <= max; i++)
		{
			list.Add(1.0);
		}
		foreach (Match item3 in matchCollection2)
		{
			list[int.Parse(item3.Groups["bar"].Value)] = double.Parse(item3.Groups["data"].Value);
		}
		bool isCaseSensitive = !DisableBase62 && BmsRegex.Base62.IsMatch(input);
		_ = isCaseSensitive;
		MatchCollection matchCollection3 = BmsRegex.WavFiles.Matches(input);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (Match item4 in matchCollection3)
		{
			dictionary[GetIndexName(item4.Groups["index"].Value)] = item4.Groups["data"].Value;
		}
		MatchCollection matchCollection4 = BmsRegex.BmpFiles.Matches(input);
		Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
		foreach (Match item5 in matchCollection4)
		{
			dictionary2[GetIndexName(item5.Groups["index"].Value)] = item5.Groups["data"].Value;
		}
		MatchCollection matchCollection5 = BmsRegex.BpmList.Matches(input);
		Dictionary<string, double> dictionary3 = new Dictionary<string, double>();
		foreach (Match item6 in matchCollection5)
		{
			dictionary3[GetIndexName(item6.Groups["index"].Value)] = double.Parse(item6.Groups["data"].Value);
		}
		MatchCollection matchCollection6 = BmsRegex.StopList.Matches(input);
		Dictionary<string, double> dictionary4 = new Dictionary<string, double>();
		foreach (Match item7 in matchCollection6)
		{
			dictionary4[GetIndexName(item7.Groups["index"].Value)] = double.Parse(item7.Groups["data"].Value);
		}
		Bms bms = new Bms(Math.Abs(num), result)
		{
			Title = title,
			Subtitle = subtitle,
			Artist = artist,
			Subartist = subartist,
			Genre = genre,
			PlayLevel = result2,
			StageFile = stageFile,
			BackBmp = backBmp,
			Banner = banner,
			WavFiles = dictionary,
			BmpFiles = dictionary2,
			IsBase62Defined = isCaseSensitive
		};
		if (!double.IsNaN(result4))
		{
			bms.Total = result4;
		}
		if (num2.HasValue)
		{
			bms.JudgeRank = num2.Value;
		}
		if (num3.HasValue)
		{
			bms.Difficulty = num3.Value;
		}
		for (int j = 0; j <= max; j++)
		{
			bms.Bars.Add(new BmsBar(bms.Keys, list[j]));
		}
		bool has3DigitKey = dictionary.Keys.Any(k => k.Length == 3) || dictionary2.Keys.Any(k => k.Length == 3);
		foreach (Match item8 in matchCollection)
		{
			string value = item8.Groups["data"].Value;
			string value2 = item8.Groups["channel"].Value;
			int index = int.Parse(item8.Groups["bar"].Value);
			List<string> list2 = new List<string>();
			int slotWidth = 2;
			if (has3DigitKey && value.Length % 3 == 0 && (value.Length % 2 != 0 || value2 == "01" || value2[0] == '1' || value2[0] == '2' || value2[0] == '5' || value2[0] == '6'))
			{
				slotWidth = 3;
			}
			for (int k = 0; k < value.Length / slotWidth; k++)
			{
				list2.Add(GetIndexName(value.Substring(k * slotWidth, slotWidth)));
			}
			double num4 = 192.0 * list[index] / (double)list2.Count;
			bool IsNotEmpty(string s) => s != "00" && s != "000";
			switch (value2)
			{
			case "01":
			{
				for (int num5 = 0; num5 < list2.Count; num5++)
				{
					if (IsNotEmpty(list2[num5]))
					{
						bms.Bars[index].AddBmsObject(BmsChannelType.Bgm, new BmsObject(num4 * (double)num5, list2[num5], BmsObjectType.Note));
					}
				}
				continue;
			}
			case "03":
			{
				for (int l = 0; l < list2.Count; l++)
				{
					if (IsNotEmpty(list2[l]))
					{
						bms.Bars[index].AddBmsObject(BmsChannelType.Bpm, new BmsObject(num4 * (double)l, Convert.ToInt32(list2[l], 16).ToString(), BmsObjectType.Bpm));
					}
				}
				continue;
			}
			case "04":
			{
				for (int num6 = 0; num6 < list2.Count; num6++)
				{
					if (IsNotEmpty(list2[num6]))
					{
						bms.Bars[index].AddBmsObject(BmsChannelType.Bga, new BmsObject(num4 * (double)num6, list2[num6], BmsObjectType.Bga));
					}
				}
				continue;
			}
			case "06":
			{
				for (int num7 = 0; num7 < list2.Count; num7++)
				{
					if (IsNotEmpty(list2[num7]))
					{
						bms.Bars[index].AddBmsObject(BmsChannelType.Poor, new BmsObject(num4 * (double)num7, list2[num7], BmsObjectType.Bga));
					}
				}
				continue;
			}
			case "07":
			{
				for (int n = 0; n < list2.Count; n++)
				{
					if (IsNotEmpty(list2[n]))
					{
						bms.Bars[index].AddBmsObject(BmsChannelType.Layer, new BmsObject(num4 * (double)n, list2[n], BmsObjectType.Bga));
					}
				}
				continue;
			}
			case "08":
			{
				for (int m = 0; m < list2.Count; m++)
				{
					if (IsNotEmpty(list2[m]) && dictionary3.ContainsKey(list2[m]))
					{
						bms.Bars[index].AddBmsObject(BmsChannelType.Bpm, new BmsObject(num4 * (double)m, dictionary3[list2[m]].ToString(), BmsObjectType.Bpm));
					}
				}
				continue;
			}
			case "09":
			{
				for (int num8 = 0; num8 < list2.Count; num8++)
				{
					if (IsNotEmpty(list2[num8]) && dictionary4.ContainsKey(list2[num8]))
					{
						bms.Bars[index].AddBmsObject(BmsChannelType.Stop, new BmsObject(num4 * (double)num8, dictionary4[list2[num8]].ToString(), BmsObjectType.Stop));
					}
				}
				continue;
			}
			}
			if (!BmsRegex.NotesChannel.IsMatch(value2))
			{
				continue;
			}
			if (ChannelAssign[num].ContainsKey(value2))
			{
				BmsObjectType type;
				switch (value2[0])
				{
				case '1':
				case '2':
					type = BmsObjectType.Note;
					break;
				case '3':
				case '4':
					type = BmsObjectType.InvisibleNote;
					break;
				case '5':
				case '6':
					type = BmsObjectType.LNStart;
					break;
				default:
					type = BmsObjectType.Note;
					break;
				}
				for (int num9 = 0; num9 < list2.Count; num9++)
				{
					if (IsNotEmpty(list2[num9]))
					{
						bms.Bars[index].AddBmsObject(ChannelAssign[num][value2], new BmsObject(num4 * (double)num9, list2[num9], type));
					}
				}
				continue;
			}
			for (int num10 = 0; num10 < list2.Count; num10++)
			{
				if (IsNotEmpty(list2[num10]))
				{
					bms.Bars[index].AddBmsObject(BmsChannelType.Bgm, new BmsObject(num4 * (double)num10, list2[num10], BmsObjectType.Note));
				}
			}
		}
		for (int num11 = 0; num11 < bms.Bars.Count; num11++)
		{
			for (int num12 = 0; num12 < bms.Bars[num11].Lanes.Count; num12++)
			{
				if (bms.Bars[num11].Lanes[num12].Count < 2)
				{
					continue;
				}
				bms.Bars[num11].Lanes[num12].Sort((BmsObject a, BmsObject b) =>
				{
					if (a.Position > b.Position)
					{
						return 1;
					}
					return (a.Position < b.Position) ? (-1) : 0;
				});
			}
		}
		string text = "";
		Match match22 = BmsRegex.LNObj.Match(input);
		if (match22.Success)
		{
			text = GetIndexName(match22.Groups["index"].Value);
		}
		for (int num13 = 0; num13 < bms.Bars.Count; num13++)
		{
			for (int num14 = 0; num14 < bms.Bars[num13].Lanes.Count; num14++)
			{
				for (int num15 = 0; num15 < bms.Bars[num13].Lanes[num14].Count; num15++)
				{
					if (bms.Bars[num13].Lanes[num14][num15].ObjectType == BmsObjectType.LNStart)
					{
						bool flag = false;
						for (int num16 = num13; num16 < bms.Bars.Count; num16++)
						{
							for (int num17 = 0; num17 < bms.Bars[num16].Lanes[num14].Count; num17++)
							{
								if ((num16 > num13 || num17 > num15) && bms.Bars[num16].Lanes[num14][num17].ObjectType == BmsObjectType.LNStart)
								{
									bms.Bars[num16].Lanes[num14][num17].ObjectType = BmsObjectType.LNEnd;
									flag = true;
									break;
								}
							}
							if (flag)
							{
								break;
							}
						}
						if (!flag)
						{
							bms.Bars[num13].Lanes[num14][num15].ObjectType = BmsObjectType.Note;
						}
					}
					if (string.IsNullOrEmpty(text) || bms.Bars[num13].Lanes[num14][num15].ObjectType != BmsObjectType.Note || !(bms.Bars[num13].Lanes[num14][num15].Value == text))
					{
						continue;
					}
					bool flag2 = false;
					for (int num18 = num13; num18 >= 0; num18--)
					{
						for (int num19 = bms.Bars[num18].Lanes[num14].Count - 1; num19 >= 0; num19--)
						{
							if (num18 < num13 || num19 < num15)
							{
								if (bms.Bars[num18].Lanes[num14][num19].ObjectType == BmsObjectType.Note)
								{
									bms.Bars[num18].Lanes[num14][num19].ObjectType = BmsObjectType.LNStart;
									bms.Bars[num13].Lanes[num14][num15].ObjectType = BmsObjectType.LNEnd;
								}
								flag2 = true;
								break;
							}
						}
						if (flag2)
						{
							break;
						}
					}
				}
			}
		}
		for (int num20 = 0; num20 < bms.Bars[0].Lanes.Count; num20++)
		{
			bool flag3 = false;
			if (bms.Bars[0].Lanes[num20].Count >= 1)
			{
				foreach (BmsObject item9 in bms.Bars[0].Lanes[num20])
				{
					if (item9.ObjectType != BmsObjectType.InvisibleNote)
					{
						bms.Bars.Insert(0, new BmsBar(bms.Keys, 1.0));
						flag3 = true;
						break;
					}
				}
			}
			if (flag3)
			{
				break;
			}
		}
		return bms;
		string GetIndexName(string rawName)
		{
			if (!isCaseSensitive)
			{
				return rawName.ToUpper();
			}
			return rawName;
		}
	}

	public static Bms Parse(string path, int key)
	{
		return Parse(path, key, new XorShift());
	}

	public static Bms Parse(string path)
	{
		return Parse(path, 0, new XorShift());
	}
}
