extern alias MistAudioAlias;

using System;
using System.IO;
using System.Windows.Forms;
using BeMusic;
using MBMPlay.Common;
using MistAudioAlias::MistAudio;
using MistAudio.DirectSound;
using MistAudio.OpenAL;
using MistAudio.Wasapi;
using MistAudio.XAudio2;
using MistGraphics;
using MistGraphics.BitmapImage;
using MistGraphics.BitmapImage.Skia;
using MistGraphics.DirectX;
using MistGraphics.DirectX11;
using MistGraphics.DirectX12;
using MistGraphics.DirectX9;
using MistGraphics.OpenGL.Windows;
using MistGraphics.Vulkan.Windows;
using MistGraphics.Windows;
using MistInput;
using MistInput.DirectInput;

namespace MBMPlay.Platform;

public static class PlatformGeneral
{
	public static readonly bool EnableOpenFileDialog = true;

	public static bool IsArgument(string arg)
	{
		if (arg.Length > 0)
		{
			if (arg[0] != '-')
			{
				return arg[0] == '/';
			}
			return true;
		}
		return false;
	}

	public static void CreateGraphics(GraphicsType graphicsType, Settings settings, out GraphicsBase graphics, ref string windowTitle, bool debug = false)
	{
		switch (graphicsType)
		{
		case GraphicsType.Vulkan:
		{
			VulkanSettings vulkanSettings = settings.GraphicsSettings.VulkanSettings;
			graphics = new VulkanGraphics(vulkanSettings.BufferCount, vulkanSettings.MaxObjects, vulkanSettings.MaxTextures)
			{
				EnableValidationLayer = debug
			};
			windowTitle = "[Vulkan";
			break;
		}
		case GraphicsType.OpenGL:
			graphics = (GraphicsBase)(object)new GLGraphics();
			windowTitle = "[OpenGL";
			break;
		case GraphicsType.DirectX12:
		{
			DirectX12Settings directX12Settings = settings.GraphicsSettings.DirectX12Settings;
			graphics = new DX12Graphics(directX12Settings.BufferCount, directX12Settings.MaxObjects, directX12Settings.MaxTextures)
			{
				EnableDebugLayer = debug,
				MaxUpdateDynamicTextureThreads = directX12Settings.MaxDynamicTextureUpdateThreads
			};
			windowTitle = "[DX12";
			break;
		}
		case GraphicsType.DirectX11:
		{
			DirectX11Settings directX11Settings = settings.GraphicsSettings.DirectX11Settings;
			graphics = new DX11Graphics(directX11Settings.BufferCount);
			windowTitle = "[DX11";
			break;
		}
		default:
			graphics = new DX9Graphics(settings.GraphicsSettings.DirectX9Settings.BufferCount);
			windowTitle = "[DX9";
			break;
		}
	}

	public static TextureInfo LoadTextureFromFile(this GraphicsBase graphicsBase, string filename)
	{
		return graphicsBase.LoadTextureFromSkiaBitmap(filename);
	}

	public static BitmapImage LoadBitmapImage(string filename)
	{
		return new SkiaBitmapImage(filename);
	}

	public static void CreateAudioInstance(AudioType audioType, Settings settings, IntPtr windowHandle, out AudioInstance audioInstance, ref string windowTitle)
	{
		switch (audioType)
		{
		default:
		{
			XAudio2Settings xAudio2Settings = settings.AudioSettings.XAudio2Settings;
			audioInstance = new XAudio2Instance(xAudio2Settings.DefaultFrequency, xAudio2Settings.DefaultBitsPerSample, xAudio2Settings.DefaultChannels)
			{
				ForceResampling = xAudio2Settings.ForceResampling
			};
			windowTitle += " / XAudio2]";
			break;
		}
		case AudioType.OpenAL:
		{
			ALSettings aLSettings = settings.AudioSettings.ALSettings;
			audioInstance = new OpenALInstance(aLSettings.DefaultFrequency, aLSettings.DefaultBitsPerSample, aLSettings.DefaultChannels, aLSettings.BufferSize, aLSettings.BufferCount, aLSettings.PlayingThreadPriority);
			windowTitle += " / OpenAL]";
			break;
		}
		case AudioType.DirectSound:
		{
			DirectSoundSettings directSoundSettings = settings.AudioSettings.DirectSoundSettings;
			audioInstance = new DirectSoundInstance(windowHandle, directSoundSettings.DefaultFrequency, directSoundSettings.DefaultBitsPerSample, directSoundSettings.DefaultChannels)
			{
				ForceResampling = directSoundSettings.ForceResampling
			};
			windowTitle += " / DSound]";
			break;
		}
		case AudioType.Wasapi:
		{
			WasapiSettings wasapiSettings = settings.AudioSettings.WasapiSettings;
			audioInstance = new WasapiInstance(wasapiSettings.DefaultFrequency, wasapiSettings.DefaultBitsPerSample, wasapiSettings.DefaultChannels, wasapiSettings.Latency, wasapiSettings.PlayingThreadPriority, wasapiSettings.IsExclusive);
			windowTitle = windowTitle + " / Wasapi (" + (wasapiSettings.IsExclusive ? "Exclusive" : "Shared") + ")]";
			break;
		}
		}
	}

	public static InputManager CreateInputManager(GraphicsWindow window)
	{
		if (window is DXWindow || window is FormGraphicsWindow)
		{
			return new DirectInputManager(window.Handle);
		}
		throw new ArgumentException();
	}

	public static void ShowErrorDialog(string dialogText)
	{
		MessageBox.Show(dialogText, "mBMplay", MessageBoxButtons.OK, MessageBoxIcon.Hand);
	}

	public static bool ShowOpenFileDialog(out string fileName)
	{
		OpenFileDialog ofd = new OpenFileDialog();
		try
		{
			ofd.Filter = "BMS/PMS 차트 파일 (*.bms;*.bme;*.bml;*.pms)|*.bms;*.bme;*.bml;*.pms|모든 파일 (*.*)|*.*";
			ofd.FilterIndex = 0;
			ofd.Title = "BMS 파일 열기";
			ofd.RestoreDirectory = true;
			DialogResult result = DialogResult.None;
			Program.InvokeMainThreadAndWait(() =>
			{
				result = ofd.ShowDialog();
			});
			if (result == DialogResult.OK && File.Exists(ofd.FileName) && Bms.CheckBmsFile(ofd.FileName))
			{
				fileName = ofd.FileName;
				return true;
			}
		}
		finally
		{
			if (ofd != null)
			{
				((IDisposable)ofd).Dispose();
			}
		}
		fileName = "";
		return false;
	}

	public static string GetPlayingWindowTitle()
	{
		return "[" + Texts.Genre + "] " + Texts.TitleWithSubtitle + " / " + Texts.ArtistWithSubartist + " - " + Program.WindowTitle + " " + Program.GraphicsAudioTypeWindowTitle + (Flags.LegacyLN ? (" " + Program.LegacyLNModeWindowTitle) : "");
	}

	public static IMovie LoadMovie(string path)
	{
		IMovie movie = MediaFoundationMovie.Load(path);
		if (movie != null)
		{
			return movie;
		}
		return DirectShowMovie.Load(path);
	}

	internal static void BeginPeriod(uint milliseconds)
	{
		Interop.timeBeginPeriod(milliseconds);
	}

	internal static void EndPeriod(uint milliseconds)
	{
		Interop.timeEndPeriod(milliseconds);
	}
}
