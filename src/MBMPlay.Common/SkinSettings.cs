using System;
using System.Collections.Generic;

namespace MBMPlay.Common;

public class SkinSettings
{
	private readonly Dictionary<SkinType, Func<string>> GetSkinSettingsDictionary;

	private readonly Dictionary<SkinType, Action<string>> SetSkinSettingsDictionary;

	public string Preload { get; set; } = "skin\\default\\default_preload.xml";

	public string Play7 { get; set; } = "skin\\default\\default_7.xml";

	public string Play14 { get; set; } = "skin\\default\\default_14.xml";

	public string Play5 { get; set; } = "";

	public string Play10 { get; set; } = "";

	public string Play9 { get; set; } = "skin\\default\\default_9.xml";

	public string this[SkinType skinType]
	{
		get
		{
			if (!GetSkinSettingsDictionary.ContainsKey(skinType))
			{
				return "";
			}
			return GetSkinSettingsDictionary[skinType]();
		}
		set
		{
			if (SetSkinSettingsDictionary.ContainsKey(skinType))
			{
				SetSkinSettingsDictionary[skinType](value);
			}
		}
	}

	public SkinSettings()
	{
		GetSkinSettingsDictionary = InitGetSkinSettingsDictionary();
		SetSkinSettingsDictionary = InitSetSkinSettingsDictionary();
	}

	public SkinSettings(SkinSettings settings)
	{
		Preload = settings.Preload;
		Play7 = settings.Play7;
		Play14 = settings.Play14;
		Play5 = settings.Play5;
		Play10 = settings.Play10;
		Play9 = settings.Play9;
		GetSkinSettingsDictionary = InitGetSkinSettingsDictionary();
		SetSkinSettingsDictionary = InitSetSkinSettingsDictionary();
	}

	private Dictionary<SkinType, Func<string>> InitGetSkinSettingsDictionary()
	{
		return new Dictionary<SkinType, Func<string>>
		{
			{
				SkinType.Preload,
				() => Preload
			},
			{
				SkinType.Play7,
				() => Play7
			},
			{
				SkinType.Play14,
				() => Play14
			},
			{
				SkinType.Play5,
				() => Play5
			},
			{
				SkinType.Play10,
				() => Play10
			},
			{
				SkinType.Play9,
				() => Play9
			}
		};
	}

	private Dictionary<SkinType, Action<string>> InitSetSkinSettingsDictionary()
	{
		return new Dictionary<SkinType, Action<string>>
		{
			{
				SkinType.Preload,
				(string value) =>
				{
					Preload = value;
				}
			},
			{
				SkinType.Play7,
				(string value) =>
				{
					Play7 = value;
				}
			},
			{
				SkinType.Play14,
				(string value) =>
				{
					Play14 = value;
				}
			},
			{
				SkinType.Play5,
				(string value) =>
				{
					Play5 = value;
				}
			},
			{
				SkinType.Play10,
				(string value) =>
				{
					Play10 = value;
				}
			},
			{
				SkinType.Play9,
				(string value) =>
				{
					Play9 = value;
				}
			}
		};
	}
}
