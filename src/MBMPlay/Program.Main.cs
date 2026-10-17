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
	[STAThread]
	private static void Main(string[] args)
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
		bool createdNew;
		using (new ExclusiveProcess(ProductName, out createdNew))
		{
			if (createdNew)
			{
				_ipcServer = new IpcServer<RemoteProxy>(ProductName, new RemoteProxy());
				if (!File.Exists(SettingsFilePath) && File.Exists(AlternativeSettingsFilePath))
				{
					Directory.CreateDirectory(SettingsDirectoryPath);
					File.Move(AlternativeSettingsFilePath, SettingsFilePath);
				}
				if (File.Exists(SettingsFilePath))
				{
					try
					{
						using StreamReader textReader = new StreamReader(SettingsFilePath, DefaultEncoding);
						Settings = new XmlSerializer(typeof(MBMPlay.Common.Settings)).Deserialize(textReader) as MBMPlay.Common.Settings;
					}
					catch (Exception)
					{
					}
				}
				if (Settings == null)
				{
					Settings = new MBMPlay.Common.Settings();
				}
				if (Settings.UpgradeRequired)
				{
					_ = Settings.SettingsFileVersion;
					Settings.Upgrade();
				}
				_graphicsType = Settings.GraphicsType;
				_audioType = Settings.AudioType;
				_vsync = Settings.VSync;
				_targetUpdateRate = Settings.TargetUpdateRate;
				_updateThreadPriority = Settings.UpdateThreadPriority;
				_position = ((Settings.WindowStartPosition == WindowPosition.Manual) ? WindowStartPosition.Manual : WindowStartPosition.CenterScreen);
				_display = Settings.TargetDisplay;
				WindowScale = Settings.WindowScale;
				FilterLevel = Settings.FilterLevel;
				Flags.DisplayFps = Settings.DisplayFps;
				Flags.DisplayNoteCount = Settings.DisplayNoteCount;
				Flags.PlaySide = Settings.PlaySide;
				Numbers.JudgeTiming = Settings.JudgeTiming;
				Numbers.NoteTiming = Settings.NoteTiming;
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
				if (File.Exists(KeyConfigFilePath))
				{
					try
					{
						using StreamReader textReader2 = new StreamReader(KeyConfigFilePath, DefaultEncoding);
						KeyConfig = new XmlSerializer(typeof(KeyConfigRoot)).Deserialize(textReader2) as KeyConfigRoot;
					}
					catch (Exception)
					{
					}
				}
				if (KeyConfig == null || KeyConfig.UpgradeRequired)
				{
					if (KeyConfig == null)
					{
						KeyConfig = KeyConfigRoot.Default;
					}
					else
					{
						_ = KeyConfig.KeyConfigFileVersion;
						KeyConfig.Upgrade();
					}
					using StreamWriter textWriter = new StreamWriter(KeyConfigFilePath, append: false, DefaultEncoding);
					new XmlSerializer(typeof(KeyConfigRoot)).Serialize(textWriter, KeyConfig);
				}
				bool debug = false;
				PlatformGeneral.CreateGraphics(_graphicsType, Settings, out _graphics, ref GraphicsAudioTypeWindowTitle, debug);
				PlatformGeneral.BeginPeriod(1u);
				try
				{
					if (WindowScale < 0.5)
					{
						WindowScale = 0.5;
					}
					int width = ((!Extensions.EpsilonEquals(WindowScale, 1.0)) ? ((int)Math.Floor(800.0 * WindowScale)) : 800);
					int height = ((!Extensions.EpsilonEquals(WindowScale, 1.0)) ? ((int)Math.Floor(480.0 * WindowScale)) : 480);
					GraphicsWindow window = _graphics.CreateWindow(WindowTitle, width, height, _vsync);
					try
					{
						byte[] mBMPlayIcon = Resources.MBMPlayIcon;
						if (mBMPlayIcon != null)
						{
							window.SetIcon(mBMPlayIcon);
						}
						window.AllowDrop = true;
						window.Resizable = false;
						window.Exiting += (object sender, EventArgs e) =>
						{
							Flags.Exiting = true;
						};
						window.StartPosition = _position;
						if (_position == WindowStartPosition.Manual)
						{
							window.Location = Settings.WindowLocation;
						}
						else if (_position == WindowStartPosition.CenterScreen)
						{
							window.TargetDisplay = _display;
						}
						if (!Extensions.EpsilonEquals(WindowScale, 1.0))
						{
							_graphics.TargetWidth = 800;
							_graphics.TargetHeight = 480;
						}
						_graphics.Initialize(window);
						PlatformGeneral.CreateAudioInstance(_audioType, Settings, window.Handle, out _audioInstance, ref GraphicsAudioTypeWindowTitle);
						Numbers.MasterVolumeChanged += (object sender, EventArgs e) =>
						{
							_audioInstance.MasterVolume = (Flags.IsMasterVolumeMute ? 0f : ((float)Numbers.MasterVolume / 100f));
						};
						InputDevices.Init(window);
						PlayingKeyState.Init();
						AutoPlayKeyState.Init();
						if (!string.IsNullOrEmpty(_bmsPath) && File.Exists(_bmsPath) && Bms.CheckBmsFile(_bmsPath))
						{
							SceneManager.Add(new PlayScene(_graphics, window, _audioInstance)
							{
								BmsPath = _bmsPath,
								StartBar = _startBar
							});
						}
						else
						{
							SceneManager.Add(new PreloadScene(_graphics, window, _audioInstance));
						}
						_updateAsync = _targetUpdateRate > 0;
						if (_updateAsync)
						{
							bool isExiting = false;
							double waitTime = 1000.0 / (double)_targetUpdateRate;
							int updateCount = 0;
							Thread thread = new Thread(() =>
							{
								while (!isExiting)
								{
									if (!_isWaitingDraw)
									{
										long timestamp = Stopwatch.GetTimestamp();
										_mainSemaphore.Wait();
										InputDevices.Update(window);
										EventTimers.Update();
										PlayingKeyState.Update(EventTimers.NowTime);
										AutoPlayKeyState.Update(EventTimers.NowTime);
										SceneManager.Update();
										Numbers.UpdateUps();
										Interlocked.Increment(ref updateCount);
										_mainSemaphore.Release();
										while (true)
										{
											double num = (double)(Stopwatch.GetTimestamp() - timestamp) / ((double)Stopwatch.Frequency / 1000.0);
											if (num >= waitTime)
											{
												break;
											}
											Thread.Sleep((int)(waitTime - num));
										}
									}
								}
							});
							thread.Priority = _updateThreadPriority;
							thread.Start();
							bool prepareCalled = false;
							window.Run(() =>
							{
								UpdateWindowValues(window);
								if (_invokeMainThreadAction != null)
								{
									lock (_invokeActionLockObj)
									{
										_invokeMainThreadAction();
										_invokeMainThreadAction = null;
										if (_requireReleaseInvokeSemaphore)
										{
											_invokeSemaphore.Release();
											_requireReleaseInvokeSemaphore = false;
										}
									}
								}
								if (!Flags.Exiting && updateCount > 0)
								{
									if (!SkipDraw && !prepareCalled)
									{
										lock (GraphicsLockObj)
										{
											_graphics.Prepare();
											_graphics.Clear();
											prepareCalled = true;
										}
									}
									_isWaitingDraw = true;
									_mainSemaphore.Wait();
									WaitDraw();
									_isWaitingDraw = false;
									if (!SkipDraw & prepareCalled)
									{
										lock (GraphicsLockObj)
										{
											SceneManager.Draw();
											_mainSemaphore.Release();
											_graphics.Present();
											prepareCalled = false;
											Numbers.UpdateFps();
											SaveScreenShot();
										}
									}
									else
									{
										_mainSemaphore.Release();
									}
									Interlocked.Exchange(ref updateCount, 0);
								}
								_waitUntilDrawComplete = true;
							});
							isExiting = true;
							thread.Join();
						}
						else
						{
							window.Run(() =>
							{
								UpdateWindowValues(window);
								InputDevices.Update(window);
								EventTimers.Update();
								PlayingKeyState.Update(EventTimers.NowTime);
								AutoPlayKeyState.Update(EventTimers.NowTime);
								SceneManager.Update();
								if (!Flags.Exiting)
								{
									if (!SkipDraw)
									{
										_graphics.Prepare();
										_graphics.Clear();
										SceneManager.Draw();
										_graphics.Present();
										Numbers.UpdateFps();
										SaveScreenShot();
									}
									Numbers.UpdateUps();
								}
							});
						}
						_location = window.Location;
						return;
					}
					finally
					{
						_updateAsync = false;
						SceneManager.Clear();
						Utils.Free(window);
					}
				}
				catch (Exception ex4)
				{
					PlatformGeneral.ShowErrorDialog(ex4.ToString());
					return;
				}
				finally
				{
					PlatformGeneral.EndPeriod(1u);
					Settings.GraphicsType = _graphicsType;
					Settings.AudioType = _audioType;
					Settings.VSync = _vsync;
					Settings.TargetUpdateRate = _targetUpdateRate;
					Settings.UpdateThreadPriority = _updateThreadPriority;
					Settings.PlaySide = Flags.PlaySide;
					Settings.JudgeTiming = (int)Numbers.JudgeTiming;
					Settings.NoteTiming = (int)Numbers.NoteTiming;
					Settings.WindowStartPosition = ((_position == WindowStartPosition.Manual) ? WindowPosition.Manual : WindowPosition.CenterScreen);
					Settings.TargetDisplay = _display;
					Settings.WindowScale = WindowScale;
					Settings.FilterLevel = FilterLevel;
					Settings.DisplayNoteCount = Flags.DisplayNoteCount;
					if (_position == WindowStartPosition.Manual)
					{
						Settings.WindowLocation = _location;
					}
					Settings.SettingsFileVersion = Utils.GetVersionString(MBMPlay.Common.Settings.SettingsVersion);
					using (StreamWriter textWriter2 = new StreamWriter(SettingsFilePath, append: false, DefaultEncoding))
					{
						new XmlSerializer(typeof(MBMPlay.Common.Settings)).Serialize(textWriter2, Settings);
					}
					Utils.Free(_ipcServer);
					InputDevices.Free();
					Utils.Free(_mainSemaphore);
					Utils.Free(_invokeSemaphore);
					Utils.Free(_audioInstance);
					Utils.Free(_graphics);
				}
			}
			new IpcCiient<RemoteProxy>(ProductName).SendData(new RemoteProxy
			{
				StartBar = _startBar,
				BmsPath = _bmsPath,
				IsStopRequested = _callStop,
				Player1LaneSwapOption = InitialPlayer1LaneSwapOption,
				Player2LaneSwapOption = InitialPlayer2LaneSwapOption,
				FlipSide = InitialFlipSide
			});
		}
	}
}
