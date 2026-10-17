using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using MBMPlay.Platform;
using Mafu.MafuFont;
using MistGraphics;
using MistUtils;

namespace MBMPlay;

public class Font : IDisposable
{
	public GraphicsBase Graphics { get; protected set; }

	public MafuFont FontData { get; protected set; }

	public Dictionary<string, TextureInfo> Textures { get; protected set; } = new Dictionary<string, TextureInfo>();

	public Dictionary<string, CharSrc> Characters { get; protected set; } = new Dictionary<string, CharSrc>();

	public string FontDirectory { get; set; }

	public Font(string path, GraphicsBase graphics)
	{
		FontDirectory = Path.GetDirectoryName(path);
		Graphics = graphics;
		if (File.Exists(path))
		{
			try
			{
				using StreamReader textReader = new StreamReader(path, Program.DefaultEncoding);
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(MafuFont));
				FontData = xmlSerializer.Deserialize(textReader) as MafuFont;
			}
			catch (Exception)
			{
			}
		}
		if (FontData != null)
		{
			Init();
		}
	}

	public Font(MafuFont fontData, GraphicsBase graphics)
	{
		FontData = fontData;
		Graphics = graphics;
		Init();
	}

	protected void Init()
	{
		foreach (CharSrc character in FontData.Characters)
		{
			Characters.Add(character.GetStringElement(), character);
		}
	}

	public TextureInfo GetTexture(string name)
	{
		if (Textures.ContainsKey(name))
		{
			return Textures[name];
		}
		foreach (Texture texture in FontData.Textures)
		{
			if (texture.ID == name)
			{
				Textures.Add(name, Graphics.LoadTextureFromFile(Utils.GetPath(FontDirectory + Path.DirectorySeparatorChar + texture.FileName)));
				return Textures[name];
			}
		}
		return null;
	}

	public void LoadAllTextures()
	{
		foreach (Texture texture in FontData.Textures)
		{
			Textures.Add(texture.ID, Graphics.LoadTextureFromFile(Utils.GetPath(FontDirectory + Path.DirectorySeparatorChar + texture.FileName)));
		}
	}

	public void Dispose()
	{
		while (Textures.Count > 0)
		{
			Utils.Free(Textures.First().Value);
			Textures.Remove(Textures.First().Key);
		}
	}
}
