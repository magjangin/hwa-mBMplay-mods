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
	private static readonly ReadOnlyDictionary<int, ReadOnlyDictionary<string, int>> ChannelAssign = new Dictionary<int, ReadOnlyDictionary<string, int>>
	{
		{
			5,
			new Dictionary<string, int>
			{
				{ "11", 1 },
				{ "12", 2 },
				{ "13", 3 },
				{ "14", 4 },
				{ "15", 5 },
				{ "16", 0 },
				{ "31", 1 },
				{ "32", 2 },
				{ "33", 3 },
				{ "34", 4 },
				{ "35", 5 },
				{ "36", 0 },
				{ "51", 1 },
				{ "52", 2 },
				{ "53", 3 },
				{ "54", 4 },
				{ "55", 5 },
				{ "56", 0 }
			}.AsReadOnly()
		},
		{
			10,
			new Dictionary<string, int>
			{
				{ "11", 1 },
				{ "12", 2 },
				{ "13", 3 },
				{ "14", 4 },
				{ "15", 5 },
				{ "16", 0 },
				{ "21", 11 },
				{ "22", 12 },
				{ "23", 13 },
				{ "24", 14 },
				{ "25", 15 },
				{ "26", 10 },
				{ "31", 1 },
				{ "32", 2 },
				{ "33", 3 },
				{ "34", 4 },
				{ "35", 5 },
				{ "36", 0 },
				{ "41", 11 },
				{ "42", 12 },
				{ "43", 13 },
				{ "44", 14 },
				{ "45", 15 },
				{ "46", 10 },
				{ "51", 1 },
				{ "52", 2 },
				{ "53", 3 },
				{ "54", 4 },
				{ "55", 5 },
				{ "56", 0 },
				{ "61", 11 },
				{ "62", 12 },
				{ "63", 13 },
				{ "64", 14 },
				{ "65", 15 },
				{ "66", 10 }
			}.AsReadOnly()
		},
		{
			7,
			new Dictionary<string, int>
			{
				{ "11", 1 },
				{ "12", 2 },
				{ "13", 3 },
				{ "14", 4 },
				{ "15", 5 },
				{ "16", 0 },
				{ "18", 6 },
				{ "19", 7 },
				{ "31", 1 },
				{ "32", 2 },
				{ "33", 3 },
				{ "34", 4 },
				{ "35", 5 },
				{ "36", 0 },
				{ "38", 6 },
				{ "39", 7 },
				{ "51", 1 },
				{ "52", 2 },
				{ "53", 3 },
				{ "54", 4 },
				{ "55", 5 },
				{ "56", 0 },
				{ "58", 6 },
				{ "59", 7 }
			}.AsReadOnly()
		},
		{
			14,
			new Dictionary<string, int>
			{
				{ "11", 1 },
				{ "12", 2 },
				{ "13", 3 },
				{ "14", 4 },
				{ "15", 5 },
				{ "16", 0 },
				{ "18", 6 },
				{ "19", 7 },
				{ "21", 11 },
				{ "22", 12 },
				{ "23", 13 },
				{ "24", 14 },
				{ "25", 15 },
				{ "26", 10 },
				{ "28", 16 },
				{ "29", 17 },
				{ "31", 1 },
				{ "32", 2 },
				{ "33", 3 },
				{ "34", 4 },
				{ "35", 5 },
				{ "36", 0 },
				{ "38", 6 },
				{ "39", 7 },
				{ "41", 11 },
				{ "42", 12 },
				{ "43", 13 },
				{ "44", 14 },
				{ "45", 15 },
				{ "46", 10 },
				{ "48", 16 },
				{ "49", 17 },
				{ "51", 1 },
				{ "52", 2 },
				{ "53", 3 },
				{ "54", 4 },
				{ "55", 5 },
				{ "56", 0 },
				{ "58", 6 },
				{ "59", 7 },
				{ "61", 11 },
				{ "62", 12 },
				{ "63", 13 },
				{ "64", 14 },
				{ "65", 15 },
				{ "66", 10 },
				{ "68", 16 },
				{ "69", 17 }
			}.AsReadOnly()
		},
		{
			9,
			new Dictionary<string, int>
			{
				{ "11", 1 },
				{ "12", 2 },
				{ "13", 3 },
				{ "14", 4 },
				{ "15", 5 },
				{ "22", 6 },
				{ "23", 7 },
				{ "24", 8 },
				{ "25", 9 },
				{ "31", 1 },
				{ "32", 2 },
				{ "33", 3 },
				{ "34", 4 },
				{ "35", 5 },
				{ "42", 6 },
				{ "43", 7 },
				{ "44", 8 },
				{ "45", 9 },
				{ "51", 1 },
				{ "52", 2 },
				{ "53", 3 },
				{ "54", 4 },
				{ "55", 5 },
				{ "62", 6 },
				{ "63", 7 },
				{ "64", 8 },
				{ "65", 9 }
			}.AsReadOnly()
		},
		{
			-9,
			new Dictionary<string, int>
			{
				{ "11", 1 },
				{ "12", 2 },
				{ "13", 3 },
				{ "14", 4 },
				{ "15", 5 },
				{ "18", 6 },
				{ "19", 7 },
				{ "16", 8 },
				{ "17", 9 },
				{ "31", 1 },
				{ "32", 2 },
				{ "33", 3 },
				{ "34", 4 },
				{ "35", 5 },
				{ "38", 6 },
				{ "39", 7 },
				{ "36", 8 },
				{ "37", 9 },
				{ "51", 1 },
				{ "52", 2 },
				{ "53", 3 },
				{ "54", 4 },
				{ "55", 5 },
				{ "58", 6 },
				{ "59", 7 },
				{ "56", 8 },
				{ "57", 9 }
			}.AsReadOnly()
		}
	}.AsReadOnly();

	private static readonly ReadOnlyCollection<int> KeyDetectionList = new List<int> { 5, 7, 10, 14 }.AsReadOnly();

	private static readonly ReadOnlyCollection<int> ChannelListDP = new List<int>
	{
		21, 22, 23, 24, 25, 26, 27, 28, 29, 41,
		42, 43, 44, 45, 46, 47, 48, 49, 61, 62,
		63, 64, 65, 66, 67, 68, 69
	}.AsReadOnly();

	private static readonly ReadOnlyCollection<int> ChannelList14Keys = new List<int>
	{
		18, 19, 28, 29, 38, 39, 48, 49, 58, 59,
		68, 69
	}.AsReadOnly();

	private static readonly ReadOnlyCollection<int> ChannelList7Keys = new List<int> { 18, 19, 38, 39, 58, 59 }.AsReadOnly();

	private static readonly ReadOnlyCollection<int> ChannelList10Keys = new List<int> { 16, 21, 26, 36, 41, 46, 56, 61, 66 }.AsReadOnly();

	private static readonly ReadOnlyCollection<int> ChannelListPms = new List<int> { 17, 37, 57 }.AsReadOnly();

	public Dictionary<string, string> WavFiles;

	public Dictionary<string, string> BmpFiles;

	private int _judgeRank;

	private double _total;

	private int _difficulty;

	public static int DefaultJudgeRank { get; set; } = 2;

	public static double DefaultTotal { get; set; } = 300.0;

	public static int DefaultDifficulty { get; set; } = 2;

	public static bool DisableBase62 { get; set; } = false;

	public List<BmsBar> Bars { get; private set; }

	public int Keys { get; set; }

	public string Title { get; set; }

	public string Subtitle { get; set; }

	public string Artist { get; set; }

	public string Subartist { get; set; }

	public string Genre { get; set; }

	public double StartBpm { get; set; }

	public int PlayLevel { get; set; }

	public int JudgeRank
	{
		get
		{
			return _judgeRank;
		}
		set
		{
			_judgeRank = value;
			IsJudgeRankDefined = true;
		}
	}

	public bool IsJudgeRankDefined { get; private set; }

	public double Total
	{
		get
		{
			return _total;
		}
		set
		{
			_total = value;
			IsTotalDefined = true;
		}
	}

	public bool IsTotalDefined { get; private set; }

	public string StageFile { get; set; }

	public string BackBmp { get; set; }

	public string Banner { get; set; }

	public int Difficulty
	{
		get
		{
			return _difficulty;
		}
		set
		{
			_difficulty = value;
			IsDifficultyDefined = true;
		}
	}

	public bool IsDifficultyDefined { get; private set; }

	public bool IsBase62Defined { get; private set; }

	public int LastNotesBar
	{
		get
		{
			for (int num = Bars.Count - 1; num >= 0; num--)
			{
				for (int i = 0; i < Bars[num].Lanes.Count; i++)
				{
					if (Bars[num].Lanes[i].Count < 1)
					{
						continue;
					}
					foreach (BmsObject item in Bars[num].Lanes[i])
					{
						if (item.ObjectType != BmsObjectType.InvisibleNote)
						{
							return num;
						}
					}
				}
			}
			return -1;
		}
	}

	public int FirstNotesBar
	{
		get
		{
			for (int i = 0; i < Bars.Count; i++)
			{
				for (int j = 0; j < Bars[i].Lanes.Count; j++)
				{
					if (Bars[i].Lanes[j].Count < 1)
					{
						continue;
					}
					foreach (BmsObject item in Bars[i].Lanes[j])
					{
						if (item.ObjectType != BmsObjectType.InvisibleNote)
						{
							return i;
						}
					}
				}
			}
			return -1;
		}
	}

	public int TotalNotes
	{
		get
		{
			int num = 0;
			for (int i = 0; i < Bars.Count; i++)
			{
				for (int j = 0; j < Bars[i].Lanes.Count; j++)
				{
					if (Bars[i].Lanes[j].Count < 1)
					{
						continue;
					}
					foreach (BmsObject item in Bars[i].Lanes[j])
					{
						if (item.ObjectType != BmsObjectType.InvisibleNote)
						{
							num++;
						}
					}
				}
			}
			return num;
		}
	}

	public int TotalNotesWithoutLNEnd
	{
		get
		{
			int num = 0;
			for (int i = 0; i < Bars.Count; i++)
			{
				for (int j = 0; j < Bars[i].Lanes.Count; j++)
				{
					if (Bars[i].Lanes[j].Count < 1)
					{
						continue;
					}
					foreach (BmsObject item in Bars[i].Lanes[j])
					{
						if (item.ObjectType != BmsObjectType.InvisibleNote && item.ObjectType != BmsObjectType.LNEnd)
						{
							num++;
						}
					}
				}
			}
			return num;
		}
	}

	public List<int> GetTotalNotesPerLane(bool countLNEnd)
	{
		if (Bars.Count > 0)
		{
			List<int> list = new List<int>(Bars[0].Lanes.Count);
			for (int i = 0; i < Bars[0].Lanes.Count; i++)
			{
				list.Add(0);
			}
			for (int j = 0; j < Bars.Count; j++)
			{
				for (int k = 0; k < Bars[j].Lanes.Count; k++)
				{
					if (Bars[j].Lanes[k].Count < 1)
					{
						continue;
					}
					foreach (BmsObject item in Bars[j].Lanes[k])
					{
						if (item.ObjectType != BmsObjectType.InvisibleNote && (countLNEnd || item.ObjectType != BmsObjectType.LNEnd))
						{
							list[k]++;
						}
					}
				}
			}
			return list;
		}
		return null;
	}

	internal Bms(int keys, double bpm)
	{
		Keys = keys;
		StartBpm = bpm;
		Title = "";
		Subtitle = "";
		Artist = "";
		Subartist = "";
		Genre = "";
		PlayLevel = 0;
		_judgeRank = DefaultJudgeRank;
		_total = DefaultTotal;
		StageFile = "";
		BackBmp = "";
		Banner = "";
		_difficulty = DefaultDifficulty;
		Bars = new List<BmsBar>();
		WavFiles = new Dictionary<string, string>();
		BmpFiles = new Dictionary<string, string>();
	}
}
