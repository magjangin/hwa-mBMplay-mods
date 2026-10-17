extern alias MistAudioAlias;

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using BeMusic;
using MBMPlay.Common;
using MBMPlay.Platform;
using MBMPlay.Platform.Ipc;
using MBMPlay.Properties;
using MistAudioAlias::MistAudio;
using MistGraphics;
using MistGraphics.BitmapImage.Skia;
using MistUtils;
using Randomize;

namespace MBMPlay;

internal static partial class Program
{
	// 설정 파일을 읽기 전에 필요한 인자(-S, -T, -RANMIR, -FLIP, BMS 경로)를 먼저 읽는다.
	// 나머지 인자는 설정을 읽은 뒤 ApplyArgumentOverrides()가 설정값 위에 덮어쓴다.
	private static void ParseStartupArguments(string[] args)
	{
		for (int i = 0; i < args.Length; i++)
		{
			if (PlatformGeneral.IsArgument(args[i]))
			{
				bool flag = false;
				switch (args[i].Substring(1).ToUpper())
				{
				case "S":
					i++;
					if (i >= args.Length)
					{
						flag = true;
					}
					else
					{
						int.TryParse(args[i], out _startBar);
					}
					break;
				case "T":
					_callStop = true;
					break;
				case "RANMIR":
				{
					i++;
					if (i >= args.Length)
					{
						flag = true;
						break;
					}
					string[] array = args[i].Split(',');
					for (int j = 0; j < array.Length; j++)
					{
						LaneSwapOption value;
						switch (array[j].ToUpper())
						{
						case "OFF":
							value = LaneSwapOption.None;
							break;
						case "RANDOM":
							value = LaneSwapOption.Random;
							break;
						case "MIRROR":
							value = LaneSwapOption.Mirror;
							break;
						case "ROTATE":
							value = LaneSwapOption.Rotate;
							break;
						case "CROSS":
							value = LaneSwapOption.Cross;
							break;
						case "XROTATE":
							value = LaneSwapOption.CrossRotate;
							break;
						case "SPLIT":
							value = LaneSwapOption.Split;
							break;
						case "ODDEVEN":
							value = LaneSwapOption.OddEven;
							break;
						case "SPREAD":
							value = LaneSwapOption.Spread;
							break;
						case "SRANDOM":
							value = LaneSwapOption.SRandom;
							break;
						case "HRANDOM":
							value = LaneSwapOption.HRandom;
							break;
						case "SRANDOM_N":
							value = LaneSwapOption.NonCorrectedSRandom;
							break;
						case "SRANDOM_P":
							value = LaneSwapOption.PeriodicSRandom;
							break;
						case "ALLSCR":
							value = LaneSwapOption.AllScratch;
							break;
						default:
							continue;
						}
						switch (j)
						{
						case 0:
							InitialPlayer1LaneSwapOption = value;
							InitialPlayer2LaneSwapOption = value;
							break;
						case 1:
							InitialPlayer2LaneSwapOption = value;
							break;
						}
					}
					break;
				}
				case "FLIP":
					i++;
					if (i >= args.Length)
					{
						flag = true;
					}
					else
					{
						InitialFlipSide = args[i] == "1";
					}
					break;
				case "L":
				case "JTIME":
				case "NTIME":
				case "GR":
				case "VSYNC":
				case "UPS":
				case "AU":
				case "POSITION":
				case "DISPLAY":
				case "WSCALE":
				case "FILTER":
				case "PRIORITY":
					i++;
					if (i < args.Length)
					{
						continue;
					}
					flag = true;
					break;
				case "NOBGA":
				case "NOBASE62":
				case "TESTPLAY":
					continue;
				}
				if (flag)
				{
					break;
				}
				continue;
			}
			try
			{
				string fullPath = Path.GetFullPath(args[i]);
				if (File.Exists(fullPath))
				{
					_bmsPath = fullPath;
				}
			}
			catch (Exception)
			{
			}
		}
	}

	private static void ApplyArgumentOverrides(string[] args)
	{
		for (int k = 0; k < args.Length; k++)
		{
			if (!PlatformGeneral.IsArgument(args[k]))
			{
				continue;
			}
			bool flag2 = false;
			switch (args[k].Substring(1).ToUpper())
			{
			case "S":
			case "RANMIR":
			case "FLIP":
				k++;
				if (k < args.Length)
				{
					continue;
				}
				flag2 = true;
				break;
			case "L":
			{
				k++;
				if (k >= args.Length)
				{
					flag2 = true;
					break;
				}
				int result5 = 0;
				int.TryParse(args[k], out result5);
				Flags.LegacyLN = result5 == 1;
				break;
			}
			case "NOBGA":
				Flags.DisableBga = true;
				break;
			case "NOBASE62":
				Bms.DisableBase62 = true;
				break;
			case "TESTPLAY":
				InitialAutoplayState = false;
				break;
			case "JTIME":
			{
				k++;
				int result8;
				if (k >= args.Length)
				{
					flag2 = true;
				}
				else if (int.TryParse(args[k], out result8))
				{
					Numbers.JudgeTiming = result8;
				}
				break;
			}
			case "NTIME":
			{
				k++;
				int result6;
				if (k >= args.Length)
				{
					flag2 = true;
				}
				else if (int.TryParse(args[k], out result6))
				{
					Numbers.NoteTiming = result6;
				}
				break;
			}
			case "GR":
				k++;
				if (k >= args.Length)
				{
					flag2 = true;
					break;
				}
				switch (args[k].ToUpper())
				{
				case "DX9":
					_graphicsType = GraphicsType.DirectX9;
					break;
				case "DX11":
					_graphicsType = GraphicsType.DirectX11;
					break;
				case "DX12":
					_graphicsType = GraphicsType.DirectX12;
					break;
				case "GL":
					_graphicsType = GraphicsType.OpenGL;
					break;
				case "VK":
					_graphicsType = GraphicsType.Vulkan;
					break;
				}
				break;
			case "VSYNC":
				k++;
				if (k >= args.Length)
				{
					flag2 = true;
				}
				else if (args[k].ToUpper() == "ON")
				{
					_vsync = true;
				}
				else if (args[k].ToUpper() == "OFF")
				{
					_vsync = false;
				}
				break;
			case "UPS":
			{
				k++;
				if (k >= args.Length)
				{
					flag2 = true;
					break;
				}
				int.TryParse(args[k], out var result3);
				if (result3 >= 0)
				{
					_targetUpdateRate = result3;
				}
				break;
			}
			case "AU":
				k++;
				if (k >= args.Length)
				{
					flag2 = true;
					break;
				}
				switch (args[k].ToUpper())
				{
				case "DS":
					_audioType = AudioType.DirectSound;
					break;
				case "XA":
					_audioType = AudioType.XAudio2;
					break;
				case "AL":
					_audioType = AudioType.OpenAL;
					break;
				case "WASAPI":
					_audioType = AudioType.Wasapi;
					break;
				}
				break;
			case "POSITION":
				k++;
				if (k >= args.Length)
				{
					flag2 = true;
				}
				else if (args[k].ToUpper() == "MANUAL")
				{
					_position = WindowStartPosition.Manual;
				}
				else if (args[k].ToUpper() == "CENTER")
				{
					_position = WindowStartPosition.CenterScreen;
				}
				break;
			case "DISPLAY":
			{
				k++;
				int result2;
				if (k >= args.Length)
				{
					flag2 = true;
				}
				else if (int.TryParse(args[k], out result2))
				{
					_display = result2;
				}
				break;
			}
			case "WSCALE":
			{
				k++;
				double result7;
				if (k >= args.Length)
				{
					flag2 = true;
				}
				else if (double.TryParse(args[k], out result7))
				{
					WindowScale = result7;
				}
				break;
			}
			case "FILTER":
			{
				k++;
				int result4;
				if (k >= args.Length)
				{
					flag2 = true;
				}
				else if (int.TryParse(args[k], out result4) && result4 >= 0 && result4 <= 3)
				{
					FilterLevel = (FilterLevel)result4;
				}
				break;
			}
			case "PRIORITY":
			{
				k++;
				int result;
				if (k >= args.Length)
				{
					flag2 = true;
				}
				else if (int.TryParse(args[k], out result) && result >= 0 && result <= 4)
				{
					_updateThreadPriority = (ThreadPriority)result;
				}
				break;
			}
			case "T":
				continue;
			}
			if (flag2)
			{
				break;
			}
		}
	}
}
