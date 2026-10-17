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

public class Bms
{
	private static class BmsRegex
	{
		private static class Patterns
		{
			internal static readonly string DataPattern = "^[\\s\\t]*#(?<bar>[\\d]{3})(?<channel>0[1346-9]|[1-6][1-9]):(?<data>[\\dA-Za-z]{2,})";

			internal static readonly string BpmPattern = "^[\\s\\t]*#BPM (?<data>\\d+(?:\\.\\d+)?(?:e[\\+-]?\\d+)?)";

			internal static readonly string TitlePattern = "^[\\s\\t]*#TITLE (?<data>[^\\r\\n]+)";

			internal static readonly string SubtitlePattern = "^[\\s\\t]*#SUBTITLE (?<data>[^\\r\\n]+)";

			internal static readonly string ArtistPattern = "^[\\s\\t]*#ARTIST (?<data>[^\\r\\n]+)";

			internal static readonly string SubartistPattern = "^[\\s\\t]*#SUBARTIST (?<data>[^\\r\\n]+)";

			internal static readonly string GenrePattern = "^[\\s\\t]*#GENRE (?<data>[^\\r\\n]+)";

			internal static readonly string PlayLevelPattern = "^[\\s\\t]*#PLAYLEVEL (?<data>[\\d]+)";

			internal static readonly string JudgeRankPattern = "^[\\s\\t]*#RANK (?<data>[\\d]+)";

			internal static readonly string TotalPattern = "^[\\s\\t]*#TOTAL (?<data>\\d+(?:\\.\\d+)?(?:e[\\+-]?\\d+)?)";

			internal static readonly string StageFilePattern = "^[\\s\\t]*#STAGEFILE (?<data>[^\\r\\n]+)";

			internal static readonly string BackBmpPattern = "^[\\s\\t]*#BACKBMP (?<data>[^\\r\\n]+)";

			internal static readonly string BannerPattern = "^[\\s\\t]*#BANNER (?<data>[^\\r\\n]+)";

			internal static readonly string DifficultyPattern = "^[\\s\\t]*#DIFFICULTY (?<data>[\\d]+)";

			internal static readonly string LNObjPattern = "^[\\s\\t]*#LNOBJ (?<index>[\\dA-Za-z]{2,3})";

			internal static readonly string BarLengthPattern = "^[\\s\\t]*#(?<bar>[\\d]{3})02:(?<data>\\d+(?:\\.\\d+)?(?:e[\\+-]?\\d+)?)";

			internal static readonly string BpmListPattern = "^[\\s\\t]*#BPM(?<index>[\\dA-Za-z]{2}) (?<data>-?\\d+(?:\\.\\d+)?(?:e[\\+-]?\\d+)?)";

			internal static readonly string StopListPattern = "^[\\s\\t]*#STOP(?<index>[\\dA-Za-z]{2}) (?<data>-?\\d+(?:\\.\\d+)?(?:e[\\+-]?\\d+)?)";

			internal static readonly string WavFilesPattern = "^[\\s\\t]*#WAV(?<index>[\\dA-Za-z]{2,3}) (?<data>[^\\r\\n]+)";

			internal static readonly string BmpFilesPattern = "^[\\s\\t]*#BMP(?<index>[\\dA-Za-z]{2,3}) (?<data>[^\\r\\n]+)";

			internal static readonly string Base62Pattern = "^[\\s\\t]*#BASE 62";

			internal static readonly string NotesChannelPattern = "[1-6][\\d]";

			internal static readonly string RandomPattern = "^[\\s\\t]*#RANDOM (?<data>[\\d]+)";

			internal static readonly string IfPattern = "^[\\s\\t]*#IF (?<data>[\\d]+)";

			internal static readonly string ElseIfPattern = "^[\\s\\t]*#ELSEIF (?<data>[\\d]+)";

			internal static readonly string ElsePattern = "^[\\s\\t]*#ELSE";

			internal static readonly string EndIfPattern = "^[\\s\\t]*#ENDIF";

			internal static readonly string TitleCheckPattern = "^[\\s\\t]*#TITLE";

			internal static readonly string GenreCheckPattern = "^[\\s\\t]*#GENRE";

			internal static readonly string ArtistCheckPattern = "^[\\s\\t]*#ARTIST";

			internal static readonly string BmsCheckPattern = "^[\\s\\t]*#(?:TITLE|GENRE|ARTIST|BPM|PLAYER|RANK|WAV|STAGEFILE|LNOBJ|DIFFICULTY|TOTAL|(?<bar>\\d{3})(?<channel>0[1346-9]|[1-6][1-9]):)";
		}

		internal static readonly RegexOptions DefaultOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant;

		internal static readonly TimeSpan DefaultTimeout = new TimeSpan(0, 0, 30);

		internal static readonly Regex Data = new Regex(Patterns.DataPattern, DefaultOptions | RegexOptions.Multiline, DefaultTimeout);

		internal static readonly Regex BarLength = new Regex(Patterns.BarLengthPattern, DefaultOptions | RegexOptions.Multiline, DefaultTimeout);

		internal static readonly Regex Bpm = new Regex(Patterns.BpmPattern, DefaultOptions | RegexOptions.Multiline | RegexOptions.IgnoreCase, DefaultTimeout);

		internal static readonly Regex Title = new Regex(Patterns.TitlePattern, DefaultOptions | RegexOptions.Multiline | RegexOptions.IgnoreCase, DefaultTimeout);

		internal static readonly Regex Subtitle = new Regex(Patterns.SubtitlePattern, DefaultOptions | RegexOptions.Multiline | RegexOptions.IgnoreCase, DefaultTimeout);

		internal static readonly Regex Artist = new Regex(Patterns.ArtistPattern, DefaultOptions | RegexOptions.Multiline | RegexOptions.IgnoreCase, DefaultTimeout);

		internal static readonly Regex Subartist = new Regex(Patterns.SubartistPattern, DefaultOptions | RegexOptions.Multiline | RegexOptions.IgnoreCase, DefaultTimeout);

		internal static readonly Regex Genre = new Regex(Patterns.GenrePattern, DefaultOptions | RegexOptions.Multiline | RegexOptions.IgnoreCase, DefaultTimeout);

		internal static readonly Regex PlayLevel = new Regex(Patterns.PlayLevelPattern, DefaultOptions | RegexOptions.Multiline | RegexOptions.IgnoreCase, DefaultTimeout);

		internal static readonly Regex JudgeRank = new Regex(Patterns.JudgeRankPattern, DefaultOptions | RegexOptions.Multiline | RegexOptions.IgnoreCase, DefaultTimeout);

		internal static readonly Regex Total = new Regex(Patterns.TotalPattern, DefaultOptions | RegexOptions.Multiline | RegexOptions.IgnoreCase, DefaultTimeout);

		internal static readonly Regex StageFile = new Regex(Patterns.StageFilePattern, DefaultOptions | RegexOptions.Multiline | RegexOptions.IgnoreCase, DefaultTimeout);

		internal static readonly Regex BackBmp = new Regex(Patterns.BackBmpPattern, DefaultOptions | RegexOptions.Multiline | RegexOptions.IgnoreCase, DefaultTimeout);

		internal static readonly Regex Banner = new Regex(Patterns.BannerPattern, DefaultOptions | RegexOptions.Multiline | RegexOptions.IgnoreCase, DefaultTimeout);

		internal static readonly Regex Difficulty = new Regex(Patterns.DifficultyPattern, DefaultOptions | RegexOptions.Multiline | RegexOptions.IgnoreCase, DefaultTimeout);

		internal static readonly Regex LNObj = new Regex(Patterns.LNObjPattern, DefaultOptions | RegexOptions.Multiline | RegexOptions.IgnoreCase, DefaultTimeout);

		internal static readonly Regex BpmList = new Regex(Patterns.BpmListPattern, DefaultOptions | RegexOptions.Multiline | RegexOptions.IgnoreCase, DefaultTimeout);

		internal static readonly Regex StopList = new Regex(Patterns.StopListPattern, DefaultOptions | RegexOptions.Multiline | RegexOptions.IgnoreCase, DefaultTimeout);

		internal static readonly Regex WavFiles = new Regex(Patterns.WavFilesPattern, DefaultOptions | RegexOptions.Multiline | RegexOptions.IgnoreCase, DefaultTimeout);

		internal static readonly Regex BmpFiles = new Regex(Patterns.BmpFilesPattern, DefaultOptions | RegexOptions.Multiline | RegexOptions.IgnoreCase, DefaultTimeout);

		internal static readonly Regex Base62 = new Regex(Patterns.Base62Pattern, DefaultOptions | RegexOptions.Multiline | RegexOptions.IgnoreCase, DefaultTimeout);

		internal static readonly Regex NotesChannel = new Regex(Patterns.NotesChannelPattern, DefaultOptions, DefaultTimeout);

		internal static readonly Regex Random = new Regex(Patterns.RandomPattern, DefaultOptions | RegexOptions.IgnoreCase, DefaultTimeout);

		internal static readonly Regex If = new Regex(Patterns.IfPattern, DefaultOptions | RegexOptions.IgnoreCase, DefaultTimeout);

		internal static readonly Regex ElseIf = new Regex(Patterns.ElseIfPattern, DefaultOptions | RegexOptions.IgnoreCase, DefaultTimeout);

		internal static readonly Regex Else = new Regex(Patterns.ElsePattern, DefaultOptions | RegexOptions.IgnoreCase, DefaultTimeout);

		internal static readonly Regex EndIf = new Regex(Patterns.EndIfPattern, DefaultOptions | RegexOptions.IgnoreCase, DefaultTimeout);

		internal static readonly Regex TitleCheck = new Regex(Patterns.TitleCheckPattern, DefaultOptions | RegexOptions.Multiline | RegexOptions.IgnoreCase, DefaultTimeout);

		internal static readonly Regex GenreCheck = new Regex(Patterns.GenreCheckPattern, DefaultOptions | RegexOptions.Multiline | RegexOptions.IgnoreCase, DefaultTimeout);

		internal static readonly Regex ArtistCheck = new Regex(Patterns.ArtistCheckPattern, DefaultOptions | RegexOptions.Multiline | RegexOptions.IgnoreCase, DefaultTimeout);

		internal static readonly Regex BmsCheck = new Regex(Patterns.BmsCheckPattern, DefaultOptions | RegexOptions.Multiline | RegexOptions.IgnoreCase, DefaultTimeout);
	}

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

	public static bool CheckBmsFile(string path, Encoding encoding = null, int length = 4096)
	{
		try
		{
			string input = ReadText(path, encoding, length);
			if (BmsRegex.BmsCheck.IsMatch(input) || BmsRegex.TitleCheck.IsMatch(input) || BmsRegex.GenreCheck.IsMatch(input) || BmsRegex.ArtistCheck.IsMatch(input))
			{
				return true;
			}
			return false;
		}
		catch (Exception)
		{
			return false;
		}
	}

	private static string ReadText(string path, Encoding encoding = null, int length = 0)
	{
		if (string.IsNullOrEmpty(path))
		{
			throw new ArgumentNullException("path");
		}
		using FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		Encoding encoding2 = GetEncoding(fileStream, out var bomLength, encoding);
		fileStream.Position = bomLength;
		if (length > 0)
		{
			int actualLength = (int)Math.Min((long)length, fileStream.Length - bomLength);
			if (actualLength <= 0)
			{
				return "";
			}
			byte[] array = new byte[actualLength];
			fileStream.Read(array, 0, actualLength);
			return encoding2.GetString(array, 0, array.Length);
		}
		using StreamReader streamReader = new StreamReader(fileStream, encoding2, detectEncodingFromByteOrderMarks: false);
		return streamReader.ReadToEnd();
	}

	private static List<string> ReadTextLines(string path, Encoding encoding = null)
	{
		if (string.IsNullOrEmpty(path))
		{
			throw new ArgumentNullException("path");
		}
		using FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		Encoding encoding2 = GetEncoding(fileStream, out var bomLength, encoding);
		fileStream.Position = bomLength;
		using StreamReader streamReader = new StreamReader(fileStream, encoding2, detectEncodingFromByteOrderMarks: false);
		List<string> list = new List<string>();
		while (streamReader.Peek() > -1)
		{
			string text = streamReader.ReadLine();
			if (text != null)
			{
				list.Add(text);
			}
		}
		return list;
	}

	private static Encoding GetEncoding(FileStream stream, out int bomLength, Encoding defaultEncoding = null)
	{
		if (stream == null)
		{
			throw new ArgumentNullException("stream");
		}
		long originalPosition = stream.Position;
		stream.Position = 0L;
		byte[] array = new byte[4];
		int bytesRead = stream.Read(array, 0, 4);
		if (bytesRead >= 3 && array[0] == 239 && array[1] == 187 && array[2] == 191)
		{
			bomLength = 3;
			return Encoding.UTF8;
		}
		if (bytesRead >= 2 && array[0] == byte.MaxValue && array[1] == 254)
		{
			if (bytesRead >= 4 && array[2] == 0 && array[3] == 0)
			{
				bomLength = 4;
				return Encoding.UTF32;
			}
			bomLength = 2;
			return Encoding.Unicode;
		}
		if (bytesRead >= 2 && array[0] == 254 && array[1] == byte.MaxValue)
		{
			bomLength = 2;
			return Encoding.BigEndianUnicode;
		}
		if (bytesRead >= 4 && array[0] == 0 && array[1] == 0 && array[2] == 254 && array[3] == byte.MaxValue)
		{
			bomLength = 4;
			return new UTF32Encoding(bigEndian: true, byteOrderMark: true);
		}
		bomLength = 0;

		// BOM 없는 UTF-8 감지 (bms editer 호환: Strict UTF-8 디코딩 시도)
		try
		{
			stream.Position = 0L;
			int sampleSize = (int)Math.Min(stream.Length, 65536L);
			byte[] sample = new byte[sampleSize];
			stream.Read(sample, 0, sampleSize);
			UTF8Encoding strictUtf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
			strictUtf8.GetString(sample);
			return Encoding.UTF8;
		}
		catch (DecoderFallbackException)
		{
		}
		finally
		{
			stream.Position = originalPosition;
		}

		return defaultEncoding ?? Encoding.GetEncoding("shift-jis");
	}

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

	private static string ReadBmsFile(string path, Encoding encoding, XorShift random)
	{
		if (string.IsNullOrEmpty(path))
		{
			throw new ArgumentNullException("path");
		}
		List<string> list = ReadTextLines(path, encoding);
		StringBuilder stringBuilder = new StringBuilder();
		List<int> list2 = new List<int>();
		List<bool> list3 = new List<bool>();
		List<bool> list4 = new List<bool>();
		int num = 0;
		bool flag = true;
		for (int i = 0; i < list.Count; i++)
		{
			if (BmsRegex.Random.IsMatch(list[i]))
			{
				int num2 = int.Parse(BmsRegex.Random.Match(list[i]).Groups["data"].Value);
				while (list2.Count > num)
				{
					list2.RemoveAt(list2.Count - 1);
					list3.RemoveAt(list3.Count - 1);
					list4.RemoveAt(list4.Count - 1);
				}
				if (num == 0 || list3[list3.Count - 1])
				{
					int item = random.Next(1, num2 + 1);
					list2.Add(item);
					list3.Add(item: false);
					list4.Add(item: false);
				}
				flag = true;
			}
			else if (list2.Count > 0)
			{
				if (BmsRegex.If.IsMatch(list[i]))
				{
					if (flag)
					{
						flag = false;
						num++;
					}
					else
					{
						while (list2.Count > num)
						{
							list2.RemoveAt(list2.Count - 1);
							list3.RemoveAt(list3.Count - 1);
							list4.RemoveAt(list4.Count - 1);
						}
					}
					if (num == list2.Count)
					{
						if (int.Parse(BmsRegex.If.Match(list[i]).Groups["data"].Value) == list2[num - 1])
						{
							list3[num - 1] = true;
							list4[num - 1] = true;
						}
						else
						{
							list3[num - 1] = false;
						}
					}
				}
				else if (BmsRegex.ElseIf.IsMatch(list[i]))
				{
					if (num != list2.Count)
					{
						continue;
					}
					if (!list4[num - 1])
					{
						if (int.Parse(BmsRegex.ElseIf.Match(list[i]).Groups["data"].Value) == list2[num - 1])
						{
							list3[num - 1] = true;
							list4[num - 1] = true;
						}
						else
						{
							list3[num - 1] = false;
						}
					}
					else
					{
						list3[num - 1] = false;
					}
				}
				else if (BmsRegex.Else.IsMatch(list[i]))
				{
					if (num == list2.Count)
					{
						if (!list4[num - 1])
						{
							list3[num - 1] = true;
							list4[num - 1] = true;
						}
						else
						{
							list3[num - 1] = false;
						}
					}
				}
				else if (BmsRegex.EndIf.IsMatch(list[i]))
				{
					if (num > 0)
					{
						num--;
					}
					while (list2.Count > num + 1)
					{
						list2.RemoveAt(list2.Count - 1);
						list3.RemoveAt(list3.Count - 1);
						list4.RemoveAt(list4.Count - 1);
					}
					flag = true;
				}
				else if (num == 0 || list3[list3.Count - 1])
				{
					stringBuilder.AppendLine(list[i]);
				}
			}
			else
			{
				stringBuilder.AppendLine(list[i]);
			}
		}
		return stringBuilder.ToString();
	}

	private static string ReadBmsFile(string path, Encoding encoding)
	{
		return ReadBmsFile(path, encoding, new XorShift());
	}
}
