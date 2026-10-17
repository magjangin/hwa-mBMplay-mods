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
}
