using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MistUtils;

namespace MBMPlay;

public static class Texts
{
	private static readonly ReadOnlyDictionary<string, Func<string>> TextDictionary = Extensions.AsReadOnly(new Dictionary<string, Func<string>>
	{
		{
			"version",
			() => Version
		},
		{
			"title",
			() => Title
		},
		{
			"subtitle",
			() => Subtitle
		},
		{
			"title_with_subtitle",
			() => TitleWithSubtitle
		},
		{
			"genre",
			() => Genre
		},
		{
			"artist",
			() => Artist
		},
		{
			"subartist",
			() => Subartist
		},
		{
			"artist_with_subartist",
			() => ArtistWithSubartist
		}
	});

	public static string Version => "v" + Program.Version;

	public static string Title { get; set; }

	public static string Subtitle { get; set; }

	public static string TitleWithSubtitle
	{
		get
		{
			if (!string.IsNullOrEmpty(Subtitle))
			{
				return Title + " " + Subtitle;
			}
			return Title;
		}
	}

	public static string Genre { get; set; }

	public static string Artist { get; set; }

	public static string Subartist { get; set; }

	public static string ArtistWithSubartist
	{
		get
		{
			if (!string.IsNullOrEmpty(Subartist))
			{
				return Artist + " (" + Subartist + ")";
			}
			return Artist;
		}
	}

	public static string GetValue(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return "";
		}
		if (TextDictionary.ContainsKey(name))
		{
			return TextDictionary[name]();
		}
		return "";
	}
}
