extern alias MistAudioAlias;

using System;
using System.Collections.Generic;
using System.Drawing;
using MBMPlay.Common;
using MistAudioAlias::MistAudio;
using MistGraphics;
using MistUtils;

namespace MBMPlay;

public abstract class Scene : IDisposable
{
	protected GraphicsBase Graphics { get; set; }

	protected GraphicsWindow Window { get; set; }

	protected AudioInstance Audio { get; set; }

	public bool EnableInput { get; set; }

	public bool IsActive { get; set; }

	protected Scene(GraphicsBase graphics, GraphicsWindow window, AudioInstance audio)
	{
		Graphics = graphics;
		Window = window;
		Audio = audio;
	}

	public abstract void LoadContent();

	public abstract void UnloadContent();

	public abstract void Update();

	public abstract void Draw();

	public virtual Action<double> GetButtonCommandAction(string command)
	{
		return null;
	}

	public virtual ValueCommand<double> GetTrackBarCommand(string command)
	{
		return null;
	}

	protected bool ProcessCommonKeyInput()
	{
		if (Program.KeyConfig.General.Exit.IsKeyDownOnce())
		{
			Window.Exit();
			return true;
		}
		if (Program.KeyConfig.General.ScreenShot.IsKeyDownOnce())
		{
			Graphics.RequestScreenshot();
		}
		if (Program.KeyConfig.General.DisplayFps.IsKeyDownOnce())
		{
			Flags.DisplayFps = !Flags.DisplayFps;
		}
		return false;
	}

	protected abstract void DrawSkinItem(ISkinItem item, double now, ref BlendType prevBlend, ref SamplerType prevSampler, Point offset, in ColorVector color);

	protected void SetBlendState(BlendType value, ref BlendType prevBlend)
	{
		if (value != prevBlend)
		{
			prevBlend = value;
			Graphics.BlendType = value;
		}
	}

	protected void SetSamplerState(SamplerType value, ref SamplerType prevSampler)
	{
		SamplerType samplerType = Program.FilterLevel switch
		{
			FilterLevel.Disabled => SamplerType.Point, 
			FilterLevel.ForceOnScaled => (!Extensions.EpsilonEquals(Program.WindowScale, 1.0)) ? SamplerType.Linear : value, 
			FilterLevel.ForceAlways => SamplerType.Linear, 
			_ => value, 
		};
		if (samplerType != prevSampler)
		{
			prevSampler = samplerType;
			Graphics.SamplerType = samplerType;
		}
	}

	protected void DrawCommonSkinItem(SingleDestinationSkinItem item, double now, ref BlendType prevBlend, ref SamplerType prevSampler, Dictionary<string, TextureInfo> textures, Dictionary<string, Font> fonts, Point offset, in ColorVector color)
	{
		if (item == null)
		{
			return;
		}
		if (!(item is ImageSkinItem imageSkinItem))
		{
			if (!(item is NumberSkinItem numberSkinItem))
			{
				if (item is SkinItemGroup skinItemGroup)
				{
					Rectangle destinationRectangle = skinItemGroup.GetDestinationRectangle(now, offset);
					ColorVector color2 = skinItemGroup.GetDestinationColorVector(now) * color;
					{
						foreach (ISkinItem item2 in skinItemGroup.Items)
						{
							DrawSkinItem(item2, now, ref prevBlend, ref prevSampler, destinationRectangle.Location, in color2);
						}
						return;
					}
				}
				if (item is TrackBarSkinItem trackBarSkinItem)
				{
					Rectangle destinationRectangle2 = trackBarSkinItem.GetDestinationRectangle(now, offset);
					Rectangle destinationRectangle3 = trackBarSkinItem.Thumb.GetDestinationRectangle(now, destinationRectangle2.Location);
					ColorVector color3 = trackBarSkinItem.Thumb.GetDestinationColorVector(now) * color;
					{
						foreach (ISkinItem item3 in trackBarSkinItem.Thumb.Items)
						{
							DrawSkinItem(item3, now, ref prevBlend, ref prevSampler, destinationRectangle3.Location, in color3);
						}
						return;
					}
				}
				if (item is ButtonSkinItem buttonSkinItem)
				{
					Rectangle destinationRectangle4 = buttonSkinItem.GetDestinationRectangle(now, offset);
					ColorVector color4 = buttonSkinItem.GetDestinationColorVector(now) * color;
					{
						foreach (ISkinItem item4 in buttonSkinItem.Items)
						{
							DrawSkinItem(item4, now, ref prevBlend, ref prevSampler, destinationRectangle4.Location, in color4);
						}
						return;
					}
				}
				if (!(item is TextSkinItem textSkinItem))
				{
					if (!(item is BarGraphSkinItem barGraphSkinItem))
					{
						if (item is SliderSkinItem sliderSkinItem)
						{
							SliderSkinItem sliderSkinItem2 = sliderSkinItem;
							sliderSkinItem2.Draw(Graphics, textures[sliderSkinItem2.TextureID], now, offset, in color);
						}
					}
					else
					{
						BarGraphSkinItem barGraphSkinItem2 = barGraphSkinItem;
						barGraphSkinItem2.Draw(Graphics, textures[barGraphSkinItem2.TextureID], now, offset, in color);
					}
				}
				else
				{
					TextSkinItem textSkinItem2 = textSkinItem;
					textSkinItem2.Draw(Graphics, fonts[textSkinItem2.FontID], now, offset, in color);
				}
			}
			else
			{
				NumberSkinItem numberSkinItem2 = numberSkinItem;
				numberSkinItem2.Draw(Graphics, textures[numberSkinItem2.TextureID], now, offset, in color);
			}
		}
		else
		{
			ImageSkinItem imageSkinItem2 = imageSkinItem;
			imageSkinItem2.Draw(Graphics, textures[imageSkinItem2.TextureID], now, offset, in color);
		}
	}

	protected void DrawCommonMultipleDestinationSkinItem(MultipleDestinationSkinItem item, double now, ref BlendType prevBlend, ref SamplerType prevSampler, Dictionary<string, TextureInfo> textures, Dictionary<string, Font> fonts, Point offset, in ColorVector color)
	{
		if (item == null || !(item is PanelSkinItem panelSkinItem))
		{
			return;
		}
		PanelSkinItem panelSkinItem2 = panelSkinItem;
		if (!panelSkinItem2.CheckDisplayed(now))
		{
			return;
		}
		SetBlendState(panelSkinItem2.Destination.Blend, ref prevBlend);
		SetSamplerState(panelSkinItem2.Destination.Filter, ref prevSampler);
		Rectangle destinationRectangle = panelSkinItem2.GetDestinationRectangle(now, offset);
		ColorVector color2 = panelSkinItem2.GetDestinationColorVector(now) * color;
		foreach (ISkinItem item2 in panelSkinItem2.Items)
		{
			DrawSkinItem(item2, now, ref prevBlend, ref prevSampler, destinationRectangle.Location, in color2);
		}
	}

	public abstract void Dispose();
}
