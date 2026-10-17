extern alias MistAudioAlias;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using BeMusic;
using MBMPlay.Platform;
using MistAudioAlias::MistAudio;
using MistGraphics;
using MistInput;
using MistUtils;

namespace MBMPlay;

public sealed class PreloadScene : Scene
{
	private readonly ReadOnlyDictionary<string, Func<IList<string>, Action<double>>> ButtonCommandDictionary;

	private Skin skin;

	private Dictionary<string, TextureInfo> skinTextures = new Dictionary<string, TextureInfo>();

	private Dictionary<string, Font> fonts = new Dictionary<string, Font>();

	private bool isSkinReady;

	private bool _isContentLoaded;

	private List<string> dropFileList = new List<string>();

	private bool requestOpenFileDialog;

	private ReadOnlyDictionary<string, Func<IList<string>, Action<double>>> CreateButtonCommandDictionary()
	{
		return Extensions.AsReadOnly(new Dictionary<string, Func<IList<string>, Action<double>>> { 
		{
			"show_open_file_dialog",
			(IList<string> args) => (double _) =>
			{
				requestOpenFileDialog = true;
			}
		} });
	}

	public PreloadScene(GraphicsBase graphics, GraphicsWindow window, AudioInstance audio)
		: base(graphics, window, audio)
	{
		ButtonCommandDictionary = CreateButtonCommandDictionary();
	}

	public override Action<double> GetButtonCommandAction(string command)
	{
		if (ButtonCommands.ParseCommand(command, out var name, out var args) && ButtonCommandDictionary.ContainsKey(name))
		{
			return ButtonCommandDictionary[name]?.Invoke(args) ?? null;
		}
		return null;
	}

	public override void LoadContent()
	{
		string preload = Program.Settings.SkinSettings.Preload;
		skin = Skin.Load(Utils.GetPath(Program.StartupPath + Path.DirectorySeparatorChar + preload), this);
		Program.InvokeMainThreadAndWait(() =>
		{
			lock (Program.GraphicsLockObj)
			{
				skinTextures.Add("#black", Skin.GenerateSolidColorTexture(Graphics, Color.Black));
				skinTextures.Add("#white", Skin.GenerateSolidColorTexture(Graphics, Color.White));
				foreach (KeyValuePair<string, string> texture in skin.Textures)
				{
					skinTextures.Add(texture.Key, Graphics.LoadTextureFromFile(Utils.GetPath(texture.Value)));
				}
			}
			Window.Title = Program.WindowTitle + " " + Program.GraphicsAudioTypeWindowTitle;
		});
		foreach (KeyValuePair<string, string> font in skin.Fonts)
		{
			fonts.Add(font.Key, new Font(Utils.GetPath(font.Value), Graphics));
		}
		isSkinReady = true;
		Window.FileDrop += OnFileDrop;
		EventTimers.SceneStart = EventTimers.GetNowTime();
		IsActive = true;
		_isContentLoaded = true;
	}

	public override void UnloadContent()
	{
		if (!_isContentLoaded)
		{
			return;
		}
		isSkinReady = false;
		if (Window != null)
		{
			Window.FileDrop -= OnFileDrop;
		}
		Program.InvokeMainThreadAndWait(() =>
		{
			lock (Program.GraphicsLockObj)
			{
				while (skinTextures.Count > 0)
				{
					string key = skinTextures.First().Key;
					Utils.Free(skinTextures[key]);
					skinTextures.Remove(key);
				}
			}
		});
		_isContentLoaded = false;
	}

	public override void Update()
	{
		if (Program.RemoteProxy != null)
		{
			if (!string.IsNullOrEmpty(Program.RemoteProxy.BmsPath))
			{
				if (File.Exists(Program.RemoteProxy.BmsPath) && Bms.CheckBmsFile(Program.RemoteProxy.BmsPath))
				{
					Program.SkipDraw = true;
					SceneManager.Add(new PlayScene(Graphics, Window, Audio)
					{
						BmsPath = Program.RemoteProxy.BmsPath,
						StartBar = Program.RemoteProxy.StartBar
					});
					if (Program.RemoteProxy.Player1LaneSwapOption.HasValue)
					{
						Program.InitialPlayer1LaneSwapOption = Program.RemoteProxy.Player1LaneSwapOption;
					}
					if (Program.RemoteProxy.Player2LaneSwapOption.HasValue)
					{
						Program.InitialPlayer2LaneSwapOption = Program.RemoteProxy.Player2LaneSwapOption;
					}
					if (Program.RemoteProxy.FlipSide.HasValue)
					{
						Program.InitialFlipSide = Program.RemoteProxy.FlipSide;
					}
					Program.RemoteProxy.Reset();
					Program.InvokeMainThread(() =>
					{
						Window.SetActive();
					});
					SceneManager.Remove(this);
					return;
				}
				Program.RemoteProxy.Reset();
			}
			if (Program.RemoteProxy.IsStopRequested)
			{
				Program.RemoteProxy.Reset();
				Window.Exit();
				return;
			}
		}
		if (dropFileList.Count > 0)
		{
			try
			{
				string fullPath = Path.GetFullPath(dropFileList[0]);
				if (File.Exists(fullPath) && Bms.CheckBmsFile(fullPath))
				{
					Program.SkipDraw = true;
					SceneManager.Add(new PlayScene(Graphics, Window, Audio)
					{
						BmsPath = fullPath,
						StartBar = 0
					});
					dropFileList.Clear();
					Program.InvokeMainThread(() =>
					{
						Window.SetActive();
					});
					SceneManager.Remove(this);
					return;
				}
			}
			catch (Exception)
			{
			}
			dropFileList.ForEach((string s) =>
			{
			});
			dropFileList.Clear();
		}
		if (EnableInput)
		{
			if (ProcessCommonKeyInput())
			{
				return;
			}
			if (Flags.EnableOpenFileDialog && Program.KeyConfig.PreloadScene.OpenFileDialog.IsKeyDownOnce())
			{
				requestOpenFileDialog = true;
			}
			InputDevices.Mouse.IsKeyDownOnce(MouseInput.WheelUp);
			InputDevices.Mouse.IsKeyDownOnce(MouseInput.WheelDown);
			skin.UpdateControls(EventTimers.NowTime);
			if (requestOpenFileDialog)
			{
				requestOpenFileDialog = false;
				if (PlatformGeneral.ShowOpenFileDialog(out var fileName))
				{
					Program.SkipDraw = true;
					SceneManager.Add(new PlayScene(Graphics, Window, Audio)
					{
						BmsPath = fileName,
						StartBar = 0
					});
					dropFileList.Clear();
					Program.InvokeMainThread(() =>
					{
						Window.SetActive();
					});
					SceneManager.Remove(this);
					return;
				}
			}
		}
		if (Program.SkipDraw && isSkinReady)
		{
			Program.SkipDraw = false;
		}
	}

	public override void Draw()
	{
		if (!isSkinReady)
		{
			return;
		}
		double nowTime = EventTimers.NowTime;
		BlendType prevBlend = BlendType.Alpha;
		SamplerType prevSampler = SamplerType.Point;
		foreach (ISkinItem item in skin.Items)
		{
			DrawSkinItem(item, nowTime, ref prevBlend, ref prevSampler, Point.Empty, in ColorVector.White);
		}
	}

	protected override void DrawSkinItem(ISkinItem item, double now, ref BlendType prevBlend, ref SamplerType prevSampler, Point offset, in ColorVector color)
	{
		if (!item.ItemType.Contains(SkinItemType.Drawable))
		{
			return;
		}
		if (item is SingleDestinationSkinItem singleDestinationSkinItem)
		{
			if (singleDestinationSkinItem.CheckDisplayed(now))
			{
				SetBlendState(singleDestinationSkinItem.Destination.Blend, ref prevBlend);
				SetSamplerState(singleDestinationSkinItem.Destination.Filter, ref prevSampler);
				DrawCommonSkinItem(singleDestinationSkinItem, now, ref prevBlend, ref prevSampler, skinTextures, fonts, offset, in color);
			}
		}
		else if (item is MultipleDestinationSkinItem item2)
		{
			DrawCommonMultipleDestinationSkinItem(item2, now, ref prevBlend, ref prevSampler, skinTextures, fonts, offset, in color);
		}
	}

	public void OnFileDrop(object sender, FileDropEventArgs e)
	{
		dropFileList.Add(e.FileName);
	}

	public override void Dispose()
	{
		UnloadContent();
	}
}
