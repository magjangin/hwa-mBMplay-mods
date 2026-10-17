using System.Collections.Generic;
using System.IO;
using System.Linq;
using MistUtils;
using Randomize;

namespace MBMPlay.Common;

public class CustomFile
{
	private string customValue = "";

	private string basePath = "";

	private string filter = "";

	private CustomFileType customFileType;

	private bool allowRandom = true;

	private string defaultValue = "";

	private string filename;

	public string ID { get; set; }

	public string Name { get; set; }

	public string Value
	{
		get
		{
			return customValue;
		}
		set
		{
			customValue = value ?? "";
			filename = null;
		}
	}

	public string BasePath
	{
		get
		{
			return basePath;
		}
		set
		{
			basePath = value ?? "";
			filename = null;
		}
	}

	public string Filter
	{
		get
		{
			return filter;
		}
		set
		{
			filter = value ?? "";
			Filters = ((!string.IsNullOrEmpty(filter)) ? (from s in filter.Split(',')
				select s.Trim() into s
				where !string.IsNullOrEmpty(s)
				select s).ToList() : new List<string>());
			filename = null;
		}
	}

	public List<string> Filters { get; protected set; } = new List<string>();

	public CustomFileType CustomFileType
	{
		get
		{
			return customFileType;
		}
		set
		{
			customFileType = value;
			filename = null;
		}
	}

	public bool AllowRandom
	{
		get
		{
			return allowRandom;
		}
		set
		{
			allowRandom = value;
			filename = null;
		}
	}

	public string Default
	{
		get
		{
			return defaultValue;
		}
		set
		{
			defaultValue = value ?? "";
			filename = null;
		}
	}

	public List<string> GetFileList(string skinPath)
	{
		string path = Utils.GetPath(skinPath + Path.DirectorySeparatorChar + BasePath);
		List<string> list = new List<string>(Filters);
		if (list.Count <= 0)
		{
			list.Add("*");
		}
		List<string> list2 = new List<string>();
		foreach (string item in list)
		{
			list2 = ((CustomFileType != CustomFileType.Directory) ? list2.Union(from s in Directory.EnumerateFiles(path, item)
				select Path.GetFileName(s)).ToList() : list2.Union(from s in Directory.EnumerateDirectories(path, item)
				select Path.GetFileName(s)).ToList());
		}
		return list2;
	}

	public string GetTargetPath(string skinPath, long randomSeed)
	{
		if (filename == null)
		{
			List<string> fileList = GetFileList(skinPath);
			if (fileList.Count <= 0)
			{
				filename = "";
			}
			else if (fileList.Count == 1)
			{
				filename = fileList[0];
			}
			else if (Value == "*" && AllowRandom)
			{
				XorShift xorShift = new XorShift(randomSeed);
				filename = fileList[xorShift.Next(fileList.Count)];
			}
			else if (!string.IsNullOrEmpty(Value) && fileList.Contains(Value))
			{
				filename = Value;
			}
			else if (!string.IsNullOrEmpty(Default) && fileList.Contains(Default))
			{
				filename = Default;
			}
			else
			{
				filename = fileList[0];
			}
		}
		if (!string.IsNullOrEmpty(filename))
		{
			string text = BasePath + Path.DirectorySeparatorChar + filename;
			if (Path.DirectorySeparatorChar == '\\')
			{
				return text;
			}
			return text.Replace('\\', Path.DirectorySeparatorChar);
		}
		return "";
	}

	public string GetTargetPath(string skinPath)
	{
		return GetTargetPath(skinPath, XorShift.GenerateSeed());
	}

	public CustomFileValue GetCustomFileValue()
	{
		return new CustomFileValue
		{
			ID = ID,
			Value = Value
		};
	}
}
