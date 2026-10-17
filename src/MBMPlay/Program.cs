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
	internal static readonly string StartupPath = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);

	internal static readonly string ProductName = ((AssemblyProductAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyProductAttribute))).Product;

	internal static readonly Encoding DefaultEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

	internal static string WindowTitle = "mBMplay v" + Version;

	internal static readonly string LegacyLNModeWindowTitle = "(레거시 롱노트 모드)";

	internal static string GraphicsAudioTypeWindowTitle = "";

	private static IpcServer<RemoteProxy> _ipcServer = null;

	internal static readonly string SettingsDirectoryName = "settings";

	internal static string SettingsDirectoryPath = StartupPath + Path.DirectorySeparatorChar + SettingsDirectoryName;

	internal static readonly string SettingsFileName = "settings.xml";

	internal static string SettingsFilePath = SettingsDirectoryPath + Path.DirectorySeparatorChar + SettingsFileName;

	internal static string AlternativeSettingsFilePath = StartupPath + Path.DirectorySeparatorChar + SettingsFileName;

	internal static readonly string KeyConfigFileName = "keyconfig.xml";

	internal static string KeyConfigFilePath = SettingsDirectoryPath + Path.DirectorySeparatorChar + KeyConfigFileName;

	internal static readonly string ScreenShotDirectoryName = "ss";

	internal static string ScreenShotDirectoryPath = StartupPath + Path.DirectorySeparatorChar + ScreenShotDirectoryName;

	internal static readonly object GraphicsLockObj = new object();

	private static GraphicsType _graphicsType;

	private static AudioType _audioType;

	private static GraphicsBase _graphics;

	private static AudioInstance _audioInstance;

	private static string _bmsPath = "";

	private static int _startBar = 0;

	private static bool _callStop = false;

	private static bool _vsync = false;

	private static bool _updateAsync = false;

	private static int _targetUpdateRate = 0;

	private static ThreadPriority _updateThreadPriority = ThreadPriority.Normal;

	private static WindowStartPosition _position = WindowStartPosition.CenterScreen;

	private static Point _location = Point.Empty;

	private static int _display = -2;

	private static SemaphoreSlim _mainSemaphore = new SemaphoreSlim(1, 1);

	private static bool _isWaitingDraw = false;

	private static Action _invokeMainThreadAction = null;

	private static SemaphoreSlim _invokeSemaphore = new SemaphoreSlim(1, 1);

	private static readonly object _invokeActionLockObj = new object();

	private static readonly object _waitMainSemaphoreLockObj = new object();

	private static bool _waitUntilDrawComplete = true;

	private static bool _requireReleaseInvokeSemaphore = false;

	internal static bool SkipDraw { get; set; } = false;

	internal static RemoteProxy RemoteProxy => _ipcServer?.Data;

	internal static MBMPlay.Common.Settings Settings { get; set; }

	internal static KeyConfigRoot KeyConfig { get; set; }

	internal static bool InitialAutoplayState { get; set; } = true;

	internal static LaneSwapOption? InitialPlayer1LaneSwapOption { get; set; } = null;

	internal static LaneSwapOption? InitialPlayer2LaneSwapOption { get; set; } = null;

	internal static bool? InitialFlipSide { get; set; } = null;

	internal static double WindowScale { get; set; } = 1.0;

	internal static FilterLevel FilterLevel { get; set; } = FilterLevel.BySkin;

	internal static int WindowWidth { get; set; }

	internal static int WindowHeight { get; set; }

	public static string Version => Utils.GetVersionString(Assembly.GetExecutingAssembly().GetName().Version);

	public static XorShift Random { get; } = new XorShift();

	private static void WaitDraw()
	{
		if (_invokeMainThreadAction == null)
		{
			return;
		}
		lock (_invokeActionLockObj)
		{
			_invokeMainThreadAction();
			_invokeMainThreadAction = null;
			if (!_waitUntilDrawComplete && _mainSemaphore.CurrentCount == 0)
			{
				_mainSemaphore.Release();
			}
			if (_requireReleaseInvokeSemaphore)
			{
				_invokeSemaphore.Release();
				_requireReleaseInvokeSemaphore = false;
			}
		}
		if (!_waitUntilDrawComplete)
		{
			lock (_waitMainSemaphoreLockObj)
			{
			}
			_mainSemaphore.Wait();
			WaitDraw();
		}
	}

	private static void SaveScreenShot()
	{
		if (!_graphics.IsScreenshotGenerated)
		{
			return;
		}
		string text = DateTime.Now.ToString("yyyyMMdd-HHmmss-fff") + ".png";
		if (!Directory.Exists(ScreenShotDirectoryPath))
		{
			Directory.CreateDirectory(ScreenShotDirectoryPath);
		}
		string filename = ScreenShotDirectoryPath + Path.DirectorySeparatorChar + text;
		_graphics.SaveScreenshotAsSkiaBitmapAsync(filename).ContinueWith((Task t) =>
		{
			InvokeMainThread(() =>
			{
				if (t.Exception != null)
				{
					throw t.Exception;
				}
			});
		});
	}

	internal static void InvokeMainThread(Action action)
	{
		if (_updateAsync)
		{
			lock (_invokeActionLockObj)
			{
				_waitUntilDrawComplete = false;
				_invokeMainThreadAction = (Action)Delegate.Combine(_invokeMainThreadAction, action);
				return;
			}
		}
		action();
	}

	internal static void InvokeMainThreadAndWait(Action action, bool waitUntilDrawComplete = false)
	{
		if (_updateAsync)
		{
			_invokeSemaphore.Wait();
			lock (_invokeActionLockObj)
			{
				_waitUntilDrawComplete = waitUntilDrawComplete;
				_invokeMainThreadAction = (Action)Delegate.Combine(_invokeMainThreadAction, (Action)(() =>
				{
					_requireReleaseInvokeSemaphore = true;
					action();
				}));
			}
			lock (_waitMainSemaphoreLockObj)
			{
				_mainSemaphore.Release();
				_invokeSemaphore.Wait();
				_mainSemaphore.Wait();
				_invokeSemaphore.Release();
				return;
			}
		}
		action();
	}

	internal static void SetWindowTitle(GraphicsWindow window, string title)
	{
		InvokeMainThread(() =>
		{
			window.Title = title;
		});
	}

	internal static void UpdateWindowValues(GraphicsWindow window)
	{
		if (window != null)
		{
			WindowWidth = window.Width;
			WindowHeight = window.Height;
			InputDevices.UpdateMousePosition(window);
		}
	}
}
