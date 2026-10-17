using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using MistUtils;

namespace MBMPlay.Common;

public class SkinInfo
{
	protected static readonly HashSet<string> keyList = new HashSet<string> { "5", "7", "9", "10", "14" };

	protected static readonly Dictionary<string, CustomFileType> CustomFileTypeDictionary = new Dictionary<string, CustomFileType>
	{
		{
			"file",
			CustomFileType.File
		},
		{
			"directory",
			CustomFileType.Directory
		},
		{
			"folder",
			CustomFileType.Directory
		}
	};

	public static readonly string SkinOptionsDirectory = "skin_options";

	public SkinType Type { get; protected set; }

	public int Keys { get; protected set; }

	public string SkinPath { get; protected set; }

	public string SkinDirectory { get; protected set; }

	public string Title { get; protected set; }

	public string Version { get; protected set; }

	public string Author { get; protected set; }

	public string Hash { get; protected set; }

	public Dictionary<string, CustomOption> CustomOptions { get; protected set; } = new Dictionary<string, CustomOption>();

	public Dictionary<string, CustomFile> CustomFiles { get; protected set; } = new Dictionary<string, CustomFile>();

	public List<SkinOptionOrderItem> SkinOptionOrder { get; protected set; } = new List<SkinOptionOrderItem>();

	protected SkinInfo()
	{
	}

	public static bool TryGetSkinInfo(string filepath, string basePath, string settingsDirectoryPath, Encoding encoding, out SkinInfo skinInfo, bool loadSkinOptionValues = true, bool updateSkinOptionValues = true, bool throwOnException = false)
	{
		skinInfo = null;
		if (filepath == null)
		{
			throw new ArgumentNullException("filepath");
		}
		string path = Utils.GetPath(filepath);
		Path.GetDirectoryName(path);
		if (!File.Exists(path))
		{
			return false;
		}
		SkinInfo skinInfo2 = new SkinInfo
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
			skinInfo2.Hash = Utils.EncodeSHA256(fileStream);
			fileStream.Position = 0L;
			using XmlReader reader = XmlReader.Create(fileStream, settings);
			if (!GetSkinInfo(reader, skinInfo2, throwOnException))
			{
				return false;
			}
			if (loadSkinOptionValues)
			{
				LoadSkinOptions(skinInfo2, basePath, settingsDirectoryPath, updateSkinOptionValues, encoding);
			}
		}
		skinInfo = skinInfo2;
		return true;
	}

	public static SkinInfo GetSkinInfo(string filepath, string basePath, string settingsDirectoryPath, Encoding encoding, bool loadSkinOptionValues = true, bool updateSkinOptionValues = true)
	{
		if (!TryGetSkinInfo(filepath, basePath, settingsDirectoryPath, encoding, out var skinInfo, loadSkinOptionValues, updateSkinOptionValues, throwOnException: true))
		{
			return null;
		}
		return skinInfo;
	}

	protected static bool GetSkinInfo(XmlReader reader, SkinInfo skin, bool throwOnException = true)
	{
		try
		{
			if (!reader.ReadToFollowing("skin"))
			{
				return false;
			}
			if (reader.MoveToFirstAttribute())
			{
				do
				{
					string localName = reader.LocalName;
					if (!(localName == "type"))
					{
						if (localName == "keys" && keyList.Contains(reader.Value))
						{
							skin.Keys = int.Parse(reader.Value);
						}
						continue;
					}
					switch (reader.Value)
					{
					case "play":
						skin.Type = SkinType.Play;
						break;
					case "result":
						skin.Type = SkinType.Result;
						break;
					case "select":
						skin.Type = SkinType.Select;
						break;
					case "preload":
						skin.Type = SkinType.Preload;
						break;
					}
				}
				while (reader.MoveToNextAttribute());
				if (skin.Type == SkinType.Unknown)
				{
					return false;
				}
				if (skin.Type == SkinType.Play)
				{
					switch (skin.Keys)
					{
					case 7:
						skin.Type = SkinType.Play7;
						break;
					case 14:
						skin.Type = SkinType.Play14;
						break;
					case 5:
						skin.Type = SkinType.Play5;
						break;
					case 10:
						skin.Type = SkinType.Play10;
						break;
					case 9:
						skin.Type = SkinType.Play9;
						break;
					}
				}
				reader.MoveToElement();
			}
			if (!reader.ReadToFollowing("head"))
			{
				return false;
			}
			while (reader.Read() && (reader.NodeType != XmlNodeType.EndElement || !(reader.LocalName == "head")))
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					switch (reader.LocalName)
					{
					case "title":
						skin.Title = reader.ReadString();
						break;
					case "version":
						skin.Version = reader.ReadString();
						break;
					case "author":
						skin.Author = reader.ReadString();
						break;
					}
				}
			}
			while (reader.Read() && (reader.NodeType != XmlNodeType.Element || (!(reader.LocalName == "options") && !(reader.LocalName == "body"))))
			{
			}
			if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "options")
			{
				CustomOption customOption = null;
				while (reader.Read() && (reader.NodeType != XmlNodeType.EndElement || !(reader.LocalName == "options")))
				{
					if (reader.NodeType == XmlNodeType.Element)
					{
						switch (reader.LocalName)
						{
						case "customoption":
							if (!reader.MoveToFirstAttribute())
							{
								break;
							}
							customOption = new CustomOption();
							do
							{
								string localName = reader.LocalName;
								if (!(localName == "id"))
								{
									if (localName == "name")
									{
										customOption.Name = reader.Value;
									}
								}
								else
								{
									customOption.ID = reader.Value;
								}
							}
							while (reader.MoveToNextAttribute());
							reader.MoveToElement();
							break;
						case "customfile":
						{
							if (!reader.MoveToFirstAttribute())
							{
								break;
							}
							CustomFile customFile = new CustomFile();
							do
							{
								switch (reader.LocalName)
								{
								case "id":
									customFile.ID = reader.Value;
									break;
								case "name":
									customFile.Name = reader.Value;
									break;
								case "path":
									customFile.BasePath = reader.Value;
									break;
								case "filter":
									customFile.Filter = reader.Value;
									break;
								case "type":
								{
									string key = reader.Value.ToLower();
									if (CustomFileTypeDictionary.ContainsKey(key))
									{
										customFile.CustomFileType = CustomFileTypeDictionary[key];
									}
									break;
								}
								case "random":
								{
									if (bool.TryParse(reader.Value, out var result2))
									{
										customFile.AllowRandom = result2;
									}
									break;
								}
								case "default":
									customFile.Default = reader.Value;
									customFile.Value = customFile.Default;
									break;
								}
							}
							while (reader.MoveToNextAttribute());
							reader.MoveToElement();
							if (!string.IsNullOrEmpty(customFile.ID))
							{
								if (string.IsNullOrEmpty(customFile.Name))
								{
									customFile.Name = customFile.ID;
								}
								skin.CustomFiles.Add(customFile.ID, customFile);
								skin.SkinOptionOrder.Add(new SkinOptionOrderItem
								{
									ID = customFile.ID,
									SkinOptionType = SkinOptionType.CustomFile
								});
							}
							break;
						}
						case "option":
						{
							CustomOptionItem customOptionItem = new CustomOptionItem();
							if (reader.MoveToFirstAttribute())
							{
								do
								{
									string localName = reader.LocalName;
									if (!(localName == "id"))
									{
										if (localName == "default" && bool.TryParse(reader.Value, out var result))
										{
											customOptionItem.IsDefault = result;
										}
									}
									else
									{
										customOptionItem.ID = reader.Value;
									}
								}
								while (reader.MoveToNextAttribute());
								reader.MoveToElement();
								customOptionItem.Name = reader.ReadString();
							}
							if (customOption != null && !string.IsNullOrEmpty(customOptionItem.ID))
							{
								if (string.IsNullOrEmpty(customOptionItem.Name))
								{
									customOptionItem.Name = customOptionItem.ID;
								}
								customOption.Items.Add(customOptionItem.ID, customOptionItem);
								if (customOptionItem.IsDefault && string.IsNullOrEmpty(customOption.Value))
								{
									customOption.Value = customOptionItem.ID;
								}
							}
							break;
						}
						}
					}
					else
					{
						if (reader.NodeType != XmlNodeType.EndElement)
						{
							continue;
						}
						string localName = reader.LocalName;
						if (!(localName == "customoption") || customOption == null)
						{
							continue;
						}
						if (!string.IsNullOrEmpty(customOption.ID))
						{
							if (string.IsNullOrEmpty(customOption.Name))
							{
								customOption.Name = customOption.ID;
							}
							if (string.IsNullOrEmpty(customOption.Value) && customOption.Items.Count > 0)
							{
								customOption.Value = customOption.Items.First().Value.ID;
							}
							skin.CustomOptions.Add(customOption.ID, customOption);
							skin.SkinOptionOrder.Add(new SkinOptionOrderItem
							{
								ID = customOption.ID,
								SkinOptionType = SkinOptionType.CustomOption
							});
						}
						customOption = null;
					}
				}
			}
			return true;
		}
		catch (Exception)
		{
			if (throwOnException)
			{
				throw;
			}
			return false;
		}
	}

	public string GetOptionFileName(string basePath)
	{
		return GetOptionFileName(SkinPath, basePath);
	}

	public static string GetOptionFileName(string skinPath, string basePath)
	{
		string str = new Uri(basePath + Path.DirectorySeparatorChar).MakeRelativeUri(new Uri(skinPath)).ToString();
		return Path.GetFileNameWithoutExtension(skinPath) + "_" + Utils.EncodeMD5(str) + ".xml";
	}

	protected static void LoadSkinOptions(SkinInfo skin, string basePath, string settingsDirectoryPath, bool updateSkinOptionValues, Encoding encoding)
	{
		string optionFileName = GetOptionFileName(skin.SkinPath, basePath);
		string path = Utils.GetPath(settingsDirectoryPath + Path.DirectorySeparatorChar + SkinOptionsDirectory + Path.DirectorySeparatorChar + optionFileName);
		SkinOptions skinOptions = null;
		if (File.Exists(path))
		{
			try
			{
				using StreamReader textReader = new StreamReader(path, encoding);
				skinOptions = new XmlSerializer(typeof(SkinOptions)).Deserialize(textReader) as SkinOptions;
			}
			catch (Exception)
			{
			}
		}
		if (skinOptions != null)
		{
			skin.SetSkinOptions(skinOptions);
		}
		if (updateSkinOptionValues && (skinOptions == null || skinOptions.Hash != skin.Hash))
		{
			skinOptions = skin.GetSkinOptions();
			string directoryName = Path.GetDirectoryName(path);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			using StreamWriter textWriter = new StreamWriter(path, append: false, encoding);
			new XmlSerializer(typeof(SkinOptions)).Serialize(textWriter, skinOptions);
		}
	}

	public SkinOptions GetSkinOptions()
	{
		SkinOptions skinOptions = new SkinOptions
		{
			OptionValues = new List<SkinOptionValue>(),
			Hash = Hash
		};
		foreach (KeyValuePair<string, CustomOption> customOption in CustomOptions)
		{
			skinOptions.OptionValues.Add(customOption.Value.GetCustomOptionValue());
		}
		foreach (KeyValuePair<string, CustomFile> customFile in CustomFiles)
		{
			skinOptions.OptionValues.Add(customFile.Value.GetCustomFileValue());
		}
		return skinOptions;
	}

	public void SetSkinOptions(SkinOptions skinOptions)
	{
		if (skinOptions == null || skinOptions.OptionValues == null)
		{
			return;
		}
		foreach (SkinOptionValue optionValue in skinOptions.OptionValues)
		{
			switch (optionValue.SkinOptionType)
			{
			case SkinOptionType.CustomOption:
				if (CustomOptions.ContainsKey(optionValue.ID) && CustomOptions[optionValue.ID].Items.ContainsKey(optionValue.Value))
				{
					CustomOptions[optionValue.ID].Value = optionValue.Value;
				}
				break;
			case SkinOptionType.CustomFile:
				if (CustomFiles.ContainsKey(optionValue.ID))
				{
					CustomFiles[optionValue.ID].Value = optionValue.Value;
				}
				break;
			}
		}
	}
}
