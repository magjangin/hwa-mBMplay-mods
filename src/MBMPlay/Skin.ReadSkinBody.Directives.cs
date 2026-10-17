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

public partial class Skin
{
	private sealed partial class SkinBodyReader
	{
		// <include>
		private void ReadInclude()
		{
			if (!reader.MoveToFirstAttribute())
			{
				return;
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
				return;
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
		}

		// <var>
		private void ReadVar()
		{
			if (!reader.MoveToFirstAttribute())
			{
				return;
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
		}

		// <set>
		private void ReadSet()
		{
			if (!reader.MoveToFirstAttribute())
			{
				return;
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
		}

		// <if>
		private void ReadIf()
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
				return;
			}
			reader.Skip();
			flag = true;
		}

		// <elseif, else>
		private void ReadElse()
		{
			if (flag2)
			{
				reader.Skip();
				flag = true;
				return;
			}
			if (!(reader.LocalName == "elseif"))
			{
				if (reader.LocalName == "else")
				{
					num++;
				}
				return;
			}
			ReadIf();
		}
	}
}
