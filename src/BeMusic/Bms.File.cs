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
