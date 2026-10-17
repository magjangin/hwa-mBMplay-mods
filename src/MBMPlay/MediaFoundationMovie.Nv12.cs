using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using CSCore.MediaFoundation;
using CSCore.Win32;
using MBMPlay.Common;
using UnmanagedCollections;

namespace MBMPlay;

public sealed partial class MediaFoundationMovie
{
	// 1080p 한 장을 한 스레드로 변환하면 Debug 빌드에서 30ms 가까이 걸려 24fps도 못 따라가므로 행 묶음으로 나눠 병렬 변환한다
	private static void ConvertNv12ToBgra(IntPtr source, Nv12Layout layout, IntPtr destination, int destinationStride)
	{
		int bands = Math.Min(ConversionBands, layout.Height);
		if (bands <= 1)
		{
			ConvertNv12Rows(source, in layout, destination, destinationStride, 0, layout.Height);
			return;
		}
		Parallel.For(0, bands, band => ConvertNv12Rows(source, in layout, destination, destinationStride, layout.Height * band / bands, layout.Height * (band + 1) / bands));
	}

	private static unsafe void ConvertNv12Rows(IntPtr sourcePtr, in Nv12Layout layout, IntPtr destinationPtr, int destinationStride, int startRow, int endRow)
	{
		YuvToRgb k = layout.Coefficients;
		const int round = 1 << 15;
		byte* source = (byte*)sourcePtr;
		byte* destination = (byte*)destinationPtr;
		byte* uvPlane = source + layout.Stride * layout.PlaneHeight;
		for (int y = startRow; y < endRow; y++)
		{
			int sourceY = layout.CropY + y;
			byte* yRow = source + sourceY * layout.Stride;
			byte* uvRow = uvPlane + (sourceY >> 1) * layout.Stride;
			byte* pixel = destination + y * destinationStride;
			int rAdd = 0;
			int gAdd = 0;
			int bAdd = 0;
			for (int x = 0; x < layout.Width; x++)
			{
				int sourceX = layout.CropX + x;
				// 4:2:0이므로 색차는 가로 2픽셀마다 한 번만 계산한다
				if (x == 0 || (sourceX & 1) == 0)
				{
					byte* uv = uvRow + (sourceX & ~1);
					int u = uv[0] - 128;
					int v = uv[1] - 128;
					rAdd = k.Rv * v + round;
					gAdd = round - k.Gu * u - k.Gv * v;
					bAdd = k.Bu * u + round;
				}
				int luma = (yRow[sourceX] - k.YOffset) * k.Y;
				pixel[0] = Clamp((luma + bAdd) >> 16);
				pixel[1] = Clamp((luma + gAdd) >> 16);
				pixel[2] = Clamp((luma + rAdd) >> 16);
				pixel[3] = byte.MaxValue;
				pixel += 4;
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte Clamp(int value)
	{
		if ((uint)value <= 255u)
		{
			return (byte)value;
		}
		return (byte)((value < 0) ? 0 : 255);
	}

	/// <summary>
	/// 디코더 출력 버퍼의 NV12 배치. Y 평면은 디코더 정렬 높이(PlaneHeight)만큼 이어지고 그 뒤에 UV 평면이 온다.
	/// 표시 영역(Crop/Width/Height)은 그 안의 실제 화면 부분이다.
	/// </summary>
	private readonly struct Nv12Layout
	{
		public readonly int Stride;

		public readonly int PlaneHeight;

		public readonly int CropX;

		public readonly int CropY;

		public readonly int Width;

		public readonly int Height;

		public readonly YuvToRgb Coefficients;

		public int RequiredLength => Stride * PlaneHeight + Stride * ((CropY + Height + 1) / 2);

		private Nv12Layout(int stride, int planeHeight, int cropX, int cropY, int width, int height, YuvToRgb coefficients)
		{
			Stride = stride;
			PlaneHeight = planeHeight;
			CropX = cropX;
			CropY = cropY;
			Width = width;
			Height = height;
			Coefficients = coefficients;
		}

		public static Nv12Layout FromMediaType(MFMediaType type, int movieWidth, int movieHeight)
		{
			if (!TryGetSize(type, MF_MT_FRAME_SIZE, out int frameWidth, out int frameHeight))
			{
				frameWidth = movieWidth;
				frameHeight = movieHeight;
			}
			int stride = TryGetUInt32(type, MF_MT_DEFAULT_STRIDE, out int defaultStride) ? Math.Abs(defaultStride) : frameWidth;
			stride = Math.Max(stride, frameWidth);
			if (!TryGetArea(type, MF_MT_MINIMUM_DISPLAY_APERTURE, out int cropX, out int cropY, out int areaWidth, out int areaHeight) && !TryGetArea(type, MF_MT_GEOMETRIC_APERTURE, out cropX, out cropY, out areaWidth, out areaHeight))
			{
				cropX = 0;
				cropY = 0;
				areaWidth = frameWidth;
				areaHeight = frameHeight;
			}
			cropX = Clamp(cropX, 0, frameWidth);
			cropY = Clamp(cropY, 0, frameHeight);
			int width = Math.Min(Math.Min(areaWidth, frameWidth - cropX), movieWidth);
			int height = Math.Min(Math.Min(areaHeight, frameHeight - cropY), movieHeight);
			return new Nv12Layout(stride, frameHeight, cropX, cropY, Math.Max(width, 0), Math.Max(height, 0), YuvToRgb.FromMediaType(type, movieHeight));
		}

		// MFVideoArea: MFOffset OffsetX { WORD fract; short value; }, MFOffset OffsetY, SIZE Area
		private static bool TryGetArea(MFMediaType type, Guid key, out int x, out int y, out int width, out int height)
		{
			x = 0;
			y = 0;
			width = 0;
			height = 0;
			if (!type.Exists(key))
			{
				return false;
			}
			byte[] blob = type.GetBlob(key);
			if (blob == null || blob.Length < 16)
			{
				return false;
			}
			x = BitConverter.ToInt16(blob, 2);
			y = BitConverter.ToInt16(blob, 6);
			width = BitConverter.ToInt32(blob, 8);
			height = BitConverter.ToInt32(blob, 12);
			return width > 0 && height > 0;
		}

		private static int Clamp(int value, int min, int max)
		{
			return Math.Max(min, Math.Min(value, max));
		}
	}

	/// <summary>
	/// Y'CbCr → RGB 변환 계수(16비트 고정소수점). 영상의 색 행렬(BT.601/709/2020)과 레인지(TV/PC)를 따른다.
	/// </summary>
	private readonly struct YuvToRgb
	{
		private const int MFVideoTransferMatrix_BT709 = 1;
		private const int MFVideoTransferMatrix_BT601 = 2;
		private const int MFVideoTransferMatrix_SMPTE240M = 3;
		private const int MFVideoTransferMatrix_BT2020_10 = 4;
		private const int MFVideoTransferMatrix_BT2020_12 = 5;
		private const int MFNominalRange_0_255 = 1;

		public readonly int YOffset;

		public readonly int Y;

		public readonly int Rv;

		public readonly int Gu;

		public readonly int Gv;

		public readonly int Bu;

		private YuvToRgb(double kr, double kb, bool fullRange)
		{
			double kg = 1.0 - kr - kb;
			double yScale = fullRange ? 1.0 : (255.0 / 219.0);
			double cScale = fullRange ? 1.0 : (255.0 / 224.0);
			YOffset = fullRange ? 0 : 16;
			Y = ToFixed(yScale);
			Rv = ToFixed(2.0 * (1.0 - kr) * cScale);
			Bu = ToFixed(2.0 * (1.0 - kb) * cScale);
			Gu = ToFixed(2.0 * kb * (1.0 - kb) / kg * cScale);
			Gv = ToFixed(2.0 * kr * (1.0 - kr) / kg * cScale);
		}

		public static YuvToRgb FromMediaType(MFMediaType type, int movieHeight)
		{
			bool fullRange = TryGetUInt32(type, MF_MT_VIDEO_NOMINAL_RANGE, out int range) && range == MFNominalRange_0_255;
			if (!TryGetUInt32(type, MF_MT_YUV_MATRIX, out int matrix))
			{
				// 정보가 없으면 관례대로 HD는 BT.709, SD는 BT.601로 본다
				matrix = (movieHeight >= 720) ? MFVideoTransferMatrix_BT709 : MFVideoTransferMatrix_BT601;
			}
			switch (matrix)
			{
				case MFVideoTransferMatrix_BT601:
					return new YuvToRgb(0.299, 0.114, fullRange);
				case MFVideoTransferMatrix_SMPTE240M:
					return new YuvToRgb(0.212, 0.087, fullRange);
				case MFVideoTransferMatrix_BT2020_10:
				case MFVideoTransferMatrix_BT2020_12:
					return new YuvToRgb(0.2627, 0.0593, fullRange);
				default:
					return new YuvToRgb(0.2126, 0.0722, fullRange);
			}
		}

		private static int ToFixed(double value)
		{
			return (int)Math.Round(value * 65536.0);
		}
	}
}
