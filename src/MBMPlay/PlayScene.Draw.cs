extern alias MistAudioAlias;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BeMusic;
using MBMPlay.Common;
using MBMPlay.Platform;
using MistAudioAlias::MistAudio;
using MistGraphics;
using MistGraphics.BitmapImage;
using MistUtils;
using Randomize;
using UnmanagedCollections;

namespace MBMPlay;

public sealed partial class PlayScene
{
	public override void Draw()
	{
		if (!_isSkinReady)
		{
			return;
		}
		double nowTime = EventTimers.NowTime;
		BlendType prevBlend = BlendType.Alpha;
		SamplerType prevSampler = SamplerType.Point;
		if (_isBmsReady && !Flags.Loading && Flags.LoadComplete)
		{
			if (!Flags.DisableBga)
			{
				if (_bgaTexture != null)
				{
					UpdateBga(_bgaTexture, _prevBga, _nowBga);
				}
				if (_layerTexture != null)
				{
					UpdateBga(_layerTexture, _prevLayer, _nowLayer);
				}
				if (_poorTexture != null)
				{
					UpdateBga(_poorTexture, _prevPoor, _nowPoor);
				}
			}
			_prevBga = _nowBga;
			_prevLayer = _nowLayer;
			_prevPoor = _nowPoor;
		}
		foreach (ISkinItem item in _skin.Items)
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
				SingleDestinationSkinItem singleDestinationSkinItem2 = singleDestinationSkinItem;
				if (singleDestinationSkinItem2 != null)
				{
					if (singleDestinationSkinItem2 is PlayAreaSkinItem playAreaSkinItem)
					{
						Rectangle destinationRectangle = playAreaSkinItem.GetDestinationRectangle(now, offset);
						ColorVector color2 = playAreaSkinItem.GetDestinationColorVector(now) * color;
						{
							foreach (ISkinItem item2 in playAreaSkinItem.Items)
							{
								DrawSkinItem(item2, now, ref prevBlend, ref prevSampler, destinationRectangle.Location, in color2);
							}
							return;
						}
					}
					if (singleDestinationSkinItem2 is BarlineSkinItem barlineSkinItem)
					{
						BarlineSkinItem barlineSkinItem2 = barlineSkinItem;
						BarlinePositionCollection barlinePositionCollection = null;
						if (barlineSkinItem2.Side == 2)
						{
							if (_barlinePos2.Count > 0)
							{
								barlinePositionCollection = _barlinePos2[0];
								_barlinePos2.RemoveAt(0);
							}
						}
						else if (_barlinePos1.Count > 0)
						{
							barlinePositionCollection = _barlinePos1[0];
							_barlinePos1.RemoveAt(0);
						}
						if (barlinePositionCollection != null)
						{
							barlineSkinItem2.Draw(Graphics, _skinTextures[barlineSkinItem2.TextureID], now, barlinePositionCollection, offset, in color);
						}
						return;
					}
					if (singleDestinationSkinItem2 is LaneSkinItem laneSkinItem)
					{
						LaneSkinItem laneSkinItem2 = laneSkinItem;
						if (_notePos.Count <= laneSkinItem2.LaneIndex || _notePos[laneSkinItem2.LaneIndex].Count <= 0)
						{
							return;
						}
						for (int i = 0; i < _notePos[laneSkinItem2.LaneIndex][0].Count; i++)
						{
							ColorVector colorVector = laneSkinItem2.GetDestinationColorVector(now) * color;
							Rectangle destinationRectangle2 = laneSkinItem2.GetDestinationRectangle(now, offset);
							switch (_notePos[laneSkinItem2.LaneIndex][0][i].ObjectType)
							{
							case BmsObjectType.Note:
							{
								ColorVector color6 = colorVector * laneSkinItem2.NoteSources["note"].ColorVector;
								Rectangle dstRectangle4 = destinationRectangle2;
								dstRectangle4.X = _notePos[laneSkinItem2.LaneIndex][0][i].X + offset.X;
								dstRectangle4.Y = _notePos[laneSkinItem2.LaneIndex][0][i].Y + offset.Y;
								Graphics.Draw(_skinTextures[laneSkinItem2.NoteSources["note"].TextureID], laneSkinItem2.NoteSources["note"].GetSourceRectangle(now), in dstRectangle4, in color6);
								break;
							}
							case BmsObjectType.LNStart:
							{
								string text = "";
								if (_notePos[laneSkinItem2.LaneIndex][0][i].Status == NoteStatus.LNHolding)
								{
									text = "_holding";
								}
								else if (_notePos[laneSkinItem2.LaneIndex][0][i].Status == NoteStatus.LNReleased)
								{
									text = "_released";
								}
								bool flag = false;
								Rectangle dstRectangle = default;
								Rectangle dstRectangle2 = default;
								ColorVector color3 = colorVector * laneSkinItem2.NoteSources["ln_body" + text].ColorVector;
								ColorVector color4 = ColorVector.Zero;
								for (int j = i; j < _notePos[laneSkinItem2.LaneIndex][0].Count; j++)
								{
									if (_notePos[laneSkinItem2.LaneIndex][0][j].ObjectType == BmsObjectType.LNEnd)
									{
										flag = true;
										color4 = colorVector * laneSkinItem2.NoteSources["ln_end" + text].ColorVector;
										dstRectangle = destinationRectangle2;
										dstRectangle.X = _notePos[laneSkinItem2.LaneIndex][0][i].X + offset.X;
										dstRectangle.Y = _notePos[laneSkinItem2.LaneIndex][0][j].Y + laneSkinItem2.NoteSources["ln_end" + text].Height + offset.Y;
										dstRectangle.Height = _notePos[laneSkinItem2.LaneIndex][0][i].Y - _notePos[laneSkinItem2.LaneIndex][0][j].Y - laneSkinItem2.NoteSources["ln_end" + text].Height;
										dstRectangle2 = destinationRectangle2;
										dstRectangle2.X = _notePos[laneSkinItem2.LaneIndex][0][j].X + offset.X;
										dstRectangle2.Y = _notePos[laneSkinItem2.LaneIndex][0][j].Y + offset.Y;
										break;
									}
								}
								if (!flag)
								{
									dstRectangle = destinationRectangle2;
									dstRectangle.X = _notePos[laneSkinItem2.LaneIndex][0][i].X + offset.X;
									dstRectangle.Y = offset.Y;
									dstRectangle.Height = _notePos[laneSkinItem2.LaneIndex][0][i].Y;
								}
								Graphics.Draw(_skinTextures[laneSkinItem2.NoteSources["ln_body" + text].TextureID], laneSkinItem2.NoteSources["ln_body" + text].GetSourceRectangle(now), in dstRectangle, in color3);
								ColorVector color5 = colorVector * laneSkinItem2.NoteSources["ln_start" + text].ColorVector;
								Rectangle dstRectangle3 = destinationRectangle2;
								dstRectangle3.X = _notePos[laneSkinItem2.LaneIndex][0][i].X + offset.X;
								dstRectangle3.Y = _notePos[laneSkinItem2.LaneIndex][0][i].Y + offset.Y;
								Graphics.Draw(_skinTextures[laneSkinItem2.NoteSources["ln_start" + text].TextureID], laneSkinItem2.NoteSources["ln_start" + text].GetSourceRectangle(now), in dstRectangle3, in color5);
								if (flag)
								{
									Graphics.Draw(_skinTextures[laneSkinItem2.NoteSources["ln_end" + text].TextureID], laneSkinItem2.NoteSources["ln_end" + text].GetSourceRectangle(now), in dstRectangle2, in color4);
								}
								break;
							}
							}
						}
						_notePos[laneSkinItem2.LaneIndex].RemoveAt(0);
						return;
					}
					if (singleDestinationSkinItem2 is BgaSkinItem bgaSkinItem)
					{
						Rectangle dstRectangle5 = bgaSkinItem.GetDestinationRectangle(now, offset);
						ColorVector color7 = bgaSkinItem.GetDestinationColorVector(now) * color;
						Graphics.Draw(_skinTextures["#black"], new Rectangle(0, 0, 1, 1), in dstRectangle5, in color7);
						double num = 1.0;
						double num2 = 1.0;
						num = ((_maxBgaWidth <= 256) ? ((double)dstRectangle5.Width / 256.0) : ((double)dstRectangle5.Width / 256.0 * (256.0 / (double)_maxBgaWidth)));
						num2 = ((_maxBgaHeight <= 256) ? ((double)dstRectangle5.Height / 256.0) : ((double)dstRectangle5.Height / 256.0 * (256.0 / (double)_maxBgaHeight)));
						if (_hasPoorBga && Flags.PoorBga)
						{
							DrawBga(_poorTexture, _nowPoor, in dstRectangle5, in color7, num, num2);
							return;
						}
						DrawBga(_bgaTexture, _nowBga, in dstRectangle5, in color7, num, num2);
						DrawBga(_layerTexture, _nowLayer, in dstRectangle5, in color7, num, num2);
						return;
					}
					if (singleDestinationSkinItem2 is JudgeSkinItem judgeSkinItem)
					{
						JudgeSkinItem judgeSkinItem2 = judgeSkinItem;
						Rectangle dstRectangle6 = judgeSkinItem2.GetDestinationRectangle(now, offset);
						ColorVector color8 = judgeSkinItem2.GetDestinationColorVector(now) * color;
						ComboSkinItem comboItem = judgeSkinItem2.ComboItem;
						if (comboItem != null)
						{
							int num3 = (int)Math.Ceiling((double)comboItem.GetDestinationRectangle(now, offset).Width / 2.0);
							dstRectangle6.X -= num3;
						}
						Graphics.Draw(_skinTextures[judgeSkinItem2.TextureID], judgeSkinItem2.GetSourceRectangle(now), in dstRectangle6, in color8);
						return;
					}
					if (singleDestinationSkinItem2 is ComboSkinItem comboSkinItem)
					{
						ComboSkinItem comboSkinItem2 = comboSkinItem;
						Point empty = Point.Empty;
						JudgeSkinItem judgeItem = comboSkinItem2.JudgeItem;
						if (judgeItem != null)
						{
							int num4 = (int)Math.Ceiling((double)comboSkinItem2.GetDestinationRectangle(now, offset).Width / 2.0);
							Rectangle destinationRectangle3 = judgeItem.GetDestinationRectangle(now, offset);
							empty.X = destinationRectangle3.X - num4;
							empty.Y = destinationRectangle3.Y;
						}
						ColorVector color9 = comboSkinItem2.GetDestinationColorVector(now) * color;
						int value = comboSkinItem2.Value;
						int numberDigit = comboSkinItem2.GetNumberDigit();
						int num5;
						if (comboSkinItem2.NumberType == 11 && comboSkinItem2.Digit != 0)
						{
							num5 = Math.Abs(comboSkinItem2.Digit);
						}
						else if (comboSkinItem2.NumberType == 24 && comboSkinItem2.Digit != 0)
						{
							num5 = Math.Abs(comboSkinItem2.Digit) + 1;
						}
						else
						{
							num5 = ((comboSkinItem2.NumberType != 24 || comboSkinItem2.Digit != 0 || value >= 0) ? numberDigit : (numberDigit + 1));
						}
						int num6 = Math.Abs(value);
						for (int k = 0; k < num5; k++)
						{
							int num7;
							if (comboSkinItem2.NumberType == 24 && (comboSkinItem2.Digit != 0 || (comboSkinItem2.Digit == 0 && value < 0)) && k == num5 - 1)
							{
								num7 = 11;
							}
							else if (k >= numberDigit)
							{
								num7 = 10;
							}
							else
							{
								num7 = Math.Abs(num6) % 10;
								num6 = (num6 - num7) / 10;
							}
							if (comboSkinItem2.NumberType == 24 && value < 0)
							{
								num7 += 12;
							}
							Rectangle dstRectangle7 = comboSkinItem2.GetDestinationRectangle(now, k, empty);
							Graphics.Draw(_skinTextures[comboSkinItem2.TextureID], comboSkinItem2.GetSourceRectangle(now, num7), in dstRectangle7, in color9);
						}
						return;
					}
					if (singleDestinationSkinItem2 is LaneCoverSkinItem laneCoverSkinItem)
					{
						LaneCoverSkinItem laneCoverSkinItem2 = laneCoverSkinItem;
						laneCoverSkinItem2.Draw(Graphics, _skinTextures[laneCoverSkinItem2.TextureID], now, offset, in color);
						return;
					}
				}
				DrawCommonSkinItem(singleDestinationSkinItem, now, ref prevBlend, ref prevSampler, _skinTextures, _fonts, offset, in color);
			}
			else if (singleDestinationSkinItem is BarlineSkinItem barlineSkinItem3)
			{
				if (barlineSkinItem3.Side == 2)
				{
					if (_barlinePos2.Count > 0)
					{
						_barlinePos2.RemoveAt(0);
					}
				}
				else if (_barlinePos1.Count > 0)
				{
					_barlinePos1.RemoveAt(0);
				}
			}
			else if (singleDestinationSkinItem is LaneSkinItem laneSkinItem3 && _notePos.Count > laneSkinItem3.LaneIndex && _notePos[laneSkinItem3.LaneIndex].Count > 0)
			{
				_notePos[laneSkinItem3.LaneIndex].RemoveAt(0);
			}
		}
		else
		{
			if (!(item is MultipleDestinationSkinItem multipleDestinationSkinItem))
			{
				return;
			}
			if (multipleDestinationSkinItem is GaugeSkinItem gaugeSkinItem)
			{
				int totalIndex = gaugeSkinItem.GetTotalIndex();
				for (int l = 0; l < totalIndex; l++)
				{
					int containerIndex = gaugeSkinItem.GetContainerIndex(l, totalIndex);
					if (gaugeSkinItem.Destinations.ContainsKey(containerIndex))
					{
						SetBlendState(gaugeSkinItem.Destinations[containerIndex].Blend, ref prevBlend);
						SetSamplerState(gaugeSkinItem.Destinations[containerIndex].Filter, ref prevSampler);
						ColorVector color10 = gaugeSkinItem.GetDestinationColorVector(containerIndex, now) * color;
						Graphics.Draw(_skinTextures[gaugeSkinItem.TextureID], gaugeSkinItem.GetSourceRectangle(l, now), gaugeSkinItem.GetDestinationRectangle(l, totalIndex, now, offset), in color10);
					}
				}
			}
			else
			{
				DrawCommonMultipleDestinationSkinItem(multipleDestinationSkinItem, now, ref prevBlend, ref prevSampler, _skinTextures, _fonts, offset, in color);
			}
		}
	}
}
