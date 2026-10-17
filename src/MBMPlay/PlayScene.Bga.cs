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
	private BitmapImage LoadBgaBmp(string path)
	{
		BitmapImage bitmapImage2;
		using (BitmapImage bitmapImage = PlatformGeneral.LoadBitmapImage(path))
		{
			bitmapImage2 = bitmapImage.ToRawBitmapImage();
		}
		for (int i = 0; i < bitmapImage2.Width; i++)
		{
			for (int j = 0; j < bitmapImage2.Height; j++)
			{
				Span<byte> pixelAsSpan = bitmapImage2.GetPixelAsSpan(i, j);
				if (pixelAsSpan[0] == 0 && pixelAsSpan[1] == 0 && pixelAsSpan[2] == 0 && pixelAsSpan[3] == byte.MaxValue)
				{
					pixelAsSpan[3] = 0;
				}
			}
		}
		return bitmapImage2;
	}

	// #BMP/BGA 채널 없이 동영상 파일만 있는 차트(식스타 게이트 커스텀 등)는 마디 0에 동영상 BGA를 넣는다
	private void AddFallbackBgaMovie()
	{
		if (Flags.DisableBga || _bmsData.Bars.Count == 0 || _bmsData.Bars.Any(b => b.Bga.Count > 0))
		{
			return;
		}
		string bgaKey = _bmsData.BmpFiles.FirstOrDefault(kvp => MovieExtension.Contains(Path.GetExtension(kvp.Value).ToLowerInvariant())).Key;
		if (bgaKey == null)
		{
			string movieFile = FindFallbackBgaMovie();
			if (movieFile == null)
			{
				return;
			}
			bgaKey = FallbackBgaKey;
			_bmsData.BmpFiles[bgaKey] = movieFile;
		}
		_bmsData.Bars[0].AddBmsObject(BmsChannelType.Bga, new BmsObject(0.0, bgaKey, BmsObjectType.Bga));
	}

	private string FindFallbackBgaMovie()
	{
		string dir = string.IsNullOrEmpty(BmsPath) ? null : Path.GetDirectoryName(BmsPath);
		if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
		{
			return null;
		}
		return FallbackBgaMovieNames.FirstOrDefault(name => File.Exists(Path.Combine(dir, name))) ?? Directory.GetFiles(dir, "*.mp4").Select(Path.GetFileName).FirstOrDefault();
	}

	private void PlayMovie(string index)
	{
		if (_movies.TryGetValue(index, out IMovie movie))
		{
			movie.Play();
		}
	}

	private void RestartMovie(string index, double position)
	{
		if (_movies.TryGetValue(index, out IMovie movie))
		{
			if (!movie.IsStopped)
			{
				movie.Stop();
			}
			movie.CurrentPosition = Math.Min(position, movie.TotalTime);
			movie.Play();
		}
	}

	private void UpdateBga(TextureInfo texture, string prevIndex, string nowIndex)
	{
		if (_movies.TryGetValue(nowIndex, out IMovie movie))
		{
			if (!movie.NeedsCapture)
			{
				return;
			}
			UnmanagedArray<byte> frameData = movie.GetFrameData();
			if (frameData != null)
			{
				try
				{
					UpdateBgaTexture(texture, frameData.DataPtr, movie.Width, movie.Height, movie.Width * 4);
				}
				finally
				{
					frameData.Dispose();
				}
			}
		}
		else if (prevIndex != nowIndex)
		{
			if (_bgaBmps.TryGetValue(nowIndex, out BitmapImage bitmapImage))
			{
				UpdateBgaTexture(texture, bitmapImage);
			}
			else
			{
				((IDynamicTextureInfo)texture).Clear();
			}
		}
	}

	private void UpdateBgaTexture(TextureInfo texture, BitmapImage rawTexture)
	{
		UpdateBgaTexture(texture, rawTexture.DataPtr, rawTexture.Width, rawTexture.Height, rawTexture.Stride);
	}

	private void UpdateBgaTexture(TextureInfo texture, IntPtr dataPtr, int width, int height, int stride)
	{
		int num = stride / width;
		if (_tempBgaData == null || _tempBgaData.Count != _maxBgaWidth * _maxBgaHeight * num)
		{
			_tempBgaData?.Dispose();
			_tempBgaData = new UnmanagedArray<byte>(_maxBgaWidth * _maxBgaHeight * num);
		}
		for (int i = 0; i < height; i++)
		{
			_tempBgaData.CopyFromPtr(dataPtr + stride * i, _maxBgaWidth * num * i, stride);
		}
		((IDynamicTextureInfo)texture).UpdateTextureData(_tempBgaData.DataPtr, _tempBgaData.Count);
	}

	private void DrawBga(TextureInfo texture, string index, in Rectangle baseDst, in ColorVector color, double widthRatio, double heightRatio)
	{
		if (texture == null || !TryGetBgaSize(index, out int width, out int height))
		{
			return;
		}
		Rectangle dstRectangle = Rectangle.Empty;
		dstRectangle.Width = (int)Math.Ceiling((double)width * widthRatio);
		dstRectangle.Height = (int)Math.Ceiling((double)height * heightRatio);
		dstRectangle.X = baseDst.X + (baseDst.Width - dstRectangle.Width) / 2;
		dstRectangle.Y = baseDst.Y;
		Graphics.Draw(texture, new Rectangle(0, 0, width, height), in dstRectangle, in color);
	}

	private bool TryGetBgaSize(string index, out int width, out int height)
	{
		if (_movies.TryGetValue(index, out IMovie movie) && movie.IsPlaying)
		{
			width = movie.Width;
			height = movie.Height;
			return true;
		}
		if (_bgaBmps.TryGetValue(index, out BitmapImage bitmapImage) && bitmapImage != null)
		{
			width = bitmapImage.Width;
			height = bitmapImage.Height;
			return true;
		}
		width = 0;
		height = 0;
		return false;
	}
}
