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

/// <summary>
/// Media Foundation Source Reader 기반 동영상 BGA.
/// 백그라운드 스레드에서 NV12로 디코딩한 뒤 BGRA32로 변환하고, 프레임 타임스탬프에 맞춰 내보낸다.
/// 재생 상태 의미는 DirectShowMovie와 같다: Stop()은 처음으로 되감고, 재생 중 Play()는 처음부터 다시 시작한다.
/// </summary>
public sealed partial class MediaFoundationMovie : IMovie
{
	[DllImport("mfplat.dll", ExactSpelling = true)]
	private static extern int MFStartup(uint version, uint dwFlags = 0);

	[DllImport("mfreadwrite.dll", ExactSpelling = true)]
	private static extern int MFCreateSourceReaderFromURL([MarshalAs(UnmanagedType.LPWStr)] string pwszURL, IntPtr pAttributes, out IntPtr ppSourceReader);

	private const uint MF_VERSION = 0x00020070;

	// MF_SOURCE_READER_* 가상 스트림 인덱스
	private const int MediaSourceStream = -1;
	private const int AllStreams = -2;
	private const int FirstVideoStream = -4;

	private const int MaxQueuedFrames = 3;
	private const int StopTimeoutMs = 1000;
	// 디코딩이 밀려 늦은 프레임은 변환을 건너뛰되, 화면이 멈추지 않도록 이 간격마다 한 장은 내보낸다
	private const double MaxSkipIntervalMs = 100.0;

	private static readonly int ConversionBands = Math.Max(1, Math.Min(Environment.ProcessorCount / 2, 8));

	private static readonly Guid MFMediaType_Video = new Guid("73646976-0000-0010-8000-00AA00389B71");
	private static readonly Guid MFVideoFormat_NV12 = new Guid("3231564E-0000-0010-8000-00AA00389B71");
	private static readonly Guid MF_MT_FRAME_SIZE = new Guid("1652c33d-d6b2-4012-b834-72030849a37d");
	private static readonly Guid MF_MT_FRAME_RATE = new Guid("c459a2e8-3d2c-4e44-b132-fee5156c7bb0");
	private static readonly Guid MF_MT_DEFAULT_STRIDE = new Guid("644b4e48-1e02-4516-b0eb-c01ca9d49ac6");
	private static readonly Guid MF_MT_MINIMUM_DISPLAY_APERTURE = new Guid("d7388766-18fe-48c6-a177-ee894867c8c4");
	private static readonly Guid MF_MT_GEOMETRIC_APERTURE = new Guid("66758743-7e5f-400d-980a-aa8596c85696");
	private static readonly Guid MF_MT_YUV_MATRIX = new Guid("3e23d450-2c75-4d25-a00e-b91670d12327");
	private static readonly Guid MF_MT_VIDEO_NOMINAL_RANGE = new Guid("c21b8ee5-b956-4071-8daf-325edf5cab11");
	private static readonly Guid MF_PD_DURATION = new Guid("6c990d33-bb8e-477a-8598-0d5d96fcd88a");

	private static readonly object _startupLock = new object();
	private static bool _isStartedUp;

	private readonly object _queueLock = new object();
	private readonly object _readerLock = new object();
	private readonly List<VideoFrame> _frameQueue = new List<VideoFrame>();

	private MFSourceReader _reader;
	private Nv12Layout _layout;
	private double _frameDurationMs;

	private Thread _decodeThread;
	private volatile int _decodeGeneration;
	private volatile bool _isPlaying;
	private volatile bool _isPaused;
	private bool _isDisposed;

	// 정지/일시정지 중의 재생 위치(ms)
	private double _positionMs;
	// 재생 중 미디어 시간 0에 해당하는 EventTimers.GetNowTime() 값
	private double _clockOriginMs;

	public int Width { get; private set; }

	public int Height { get; private set; }

	public double FPS { get; private set; }

	public double TotalTime { get; private set; }

	public bool IsPlaying => _isPlaying && !_isPaused;

	public bool IsStopped => !_isPlaying;

	public bool IsPaused => _isPaused;

	public bool NeedsCapture
	{
		get
		{
			double position = CurrentPosition;
			lock (_queueLock)
			{
				return _frameQueue.Count > 0 && _frameQueue[0].TimeMs <= position;
			}
		}
	}

	public double CurrentPosition
	{
		get
		{
			if (IsPlaying)
			{
				return Math.Max(0.0, EventTimers.GetNowTime() - _clockOriginMs);
			}
			return _positionMs;
		}
		set
		{
			bool resume = IsPlaying;
			StopDecoding();
			Seek(value);
			_positionMs = value;
			if (resume)
			{
				StartDecoding();
			}
		}
	}

	private MediaFoundationMovie(MFSourceReader reader)
	{
		_reader = reader;
	}

	public static IMovie Load(string path)
	{
		if (string.IsNullOrEmpty(path) || !File.Exists(path) || !EnsureStartup())
		{
			return null;
		}
		MediaFoundationMovie movie = null;
		try
		{
			if (MFCreateSourceReaderFromURL(path, IntPtr.Zero, out IntPtr reader) != 0 || reader == IntPtr.Zero)
			{
				return null;
			}
			movie = new MediaFoundationMovie(new MFSourceReader(reader));
			if (movie.Open())
			{
				return movie;
			}
		}
		catch (Exception)
		{
		}
		movie?.Dispose();
		return null;
	}

	private static bool EnsureStartup()
	{
		lock (_startupLock)
		{
			if (!_isStartedUp)
			{
				_isStartedUp = MFStartup(MF_VERSION) >= 0;
			}
			return _isStartedUp;
		}
	}

	private bool Open()
	{
		using (MFMediaType nativeType = _reader.GetNativeMediaType(FirstVideoStream, 0))
		{
			if (nativeType.MajorType != MFMediaType_Video || !TryGetSize(nativeType, MF_MT_FRAME_SIZE, out int width, out int height))
			{
				return false;
			}
			Width = width;
			Height = height;
			FPS = (TryGetSize(nativeType, MF_MT_FRAME_RATE, out int numerator, out int denominator) && numerator > 0 && denominator > 0) ? ((double)numerator / denominator) : 30.0;
			_frameDurationMs = 1000.0 / FPS;
		}
		_reader.SetStreamSelection(AllStreams, false);
		_reader.SetStreamSelection(FirstVideoStream, true);
		using (MFMediaType outputType = MFMediaType.CreateEmpty())
		{
			outputType.MajorType = MFMediaType_Video;
			outputType.SubType = MFVideoFormat_NV12;
			_reader.SetCurrentMediaType(FirstVideoStream, outputType);
		}
		RefreshLayout();
		TotalTime = ReadDuration();
		return true;
	}

	private double ReadDuration()
	{
		try
		{
			using (PropertyVariant duration = _reader.GetPresentationAttribute(MediaSourceStream, MF_PD_DURATION))
			{
				return Convert.ToInt64(duration.GetValue()) / 10000.0;
			}
		}
		catch (Exception)
		{
			return 0.0;
		}
	}

	// 디코더는 첫 샘플에서 출력 타입을 바꿀 수 있다(예: 1080 → 1088 정렬 높이). 그때마다 다시 읽어야 한다.
	private void RefreshLayout()
	{
		using (MFMediaType currentType = _reader.GetCurrentMediaType(FirstVideoStream))
		{
			_layout = Nv12Layout.FromMediaType(currentType, Width, Height);
		}
	}

	public void Play()
	{
		if (IsPlaying)
		{
			Stop();
		}
		_isPlaying = true;
		_isPaused = false;
		StartDecoding();
	}

	public void Pause()
	{
		if (IsPlaying)
		{
			_positionMs = CurrentPosition;
			_isPaused = true;
			StopDecoding();
		}
	}

	public void Stop()
	{
		_isPlaying = false;
		_isPaused = false;
		StopDecoding();
		Seek(0.0);
		_positionMs = 0.0;
		UnmanagedArray<byte> black = new UnmanagedArray<byte>(Width * Height * 4);
		black.Fill(0, 0, 0, 255);
		lock (_queueLock)
		{
			_frameQueue.Add(new VideoFrame(double.NegativeInfinity, black));
		}
	}

	public UnmanagedArray<byte> GetFrameData()
	{
		double position = CurrentPosition;
		lock (_queueLock)
		{
			int dueCount = 0;
			while (dueCount < _frameQueue.Count && _frameQueue[dueCount].TimeMs <= position)
			{
				dueCount++;
			}
			if (dueCount == 0)
			{
				return null;
			}
			// 이미 지나간 프레임은 버리고 가장 최근 프레임만 내보낸다
			for (int i = 0; i < dueCount - 1; i++)
			{
				_frameQueue[i].Data.Dispose();
			}
			UnmanagedArray<byte> frame = _frameQueue[dueCount - 1].Data;
			_frameQueue.RemoveRange(0, dueCount);
			return frame;
		}
	}

	private void StartDecoding()
	{
		_clockOriginMs = EventTimers.GetNowTime() - _positionMs;
		int generation = _decodeGeneration;
		_decodeThread = new Thread(() => DecodeLoop(generation))
		{
			IsBackground = true,
			Name = "MFMovieDecodeThread"
		};
		_decodeThread.Start();
	}

	private void StopDecoding()
	{
		_decodeGeneration++;
		_decodeThread?.Join(StopTimeoutMs);
		_decodeThread = null;
	}

	private void Seek(double positionMs)
	{
		lock (_readerLock)
		{
			if (_reader != null)
			{
				try
				{
					PropertyVariant position = default(PropertyVariant);
					position.DataType = VarEnum.VT_I8;
					position.HValue = (long)(positionMs * 10000.0);
					_reader.SetCurrentPosition(Guid.Empty, position);
				}
				catch (Exception)
				{
				}
			}
		}
		lock (_queueLock)
		{
			ClearQueue();
		}
	}

	private void DecodeLoop(int generation)
	{
		double lastQueuedAt = double.NegativeInfinity;
		try
		{
			while (generation == _decodeGeneration)
			{
				if (QueuedFrameCount() >= MaxQueuedFrames)
				{
					Thread.Sleep(5);
					continue;
				}
				MFSample sample;
				MFSourceReaderFlags flags;
				long timestamp;
				lock (_readerLock)
				{
					// Seek/Stop이 끼어들었다면 리더 위치가 바뀌었으므로 이 세대는 끝낸다
					if (generation != _decodeGeneration || _reader == null)
					{
						return;
					}
					sample = _reader.ReadSample(FirstVideoStream, 0, out _, out flags, out timestamp);
					if ((flags & MFSourceReaderFlags.CurrentMediaTypeChanged) != 0)
					{
						RefreshLayout();
					}
				}
				using (sample)
				{
					if ((flags & (MFSourceReaderFlags.EndOfStream | MFSourceReaderFlags.Error)) != 0)
					{
						return;
					}
					if (sample == null)
					{
						continue;
					}
					double timeMs = timestamp / 10000.0;
					double now = EventTimers.GetNowTime();
					bool isLate = timeMs + _frameDurationMs <= CurrentPosition;
					if (isLate && now - lastQueuedAt < MaxSkipIntervalMs)
					{
						continue;
					}
					UnmanagedArray<byte> frame = ConvertSample(sample);
					if (frame == null)
					{
						continue;
					}
					lock (_queueLock)
					{
						if (generation != _decodeGeneration)
						{
							frame.Dispose();
							return;
						}
						_frameQueue.Add(new VideoFrame(timeMs, frame));
					}
					lastQueuedAt = now;
				}
			}
		}
		catch (Exception)
		{
		}
	}

	private int QueuedFrameCount()
	{
		lock (_queueLock)
		{
			return _frameQueue.Count;
		}
	}

	private UnmanagedArray<byte> ConvertSample(MFSample sample)
	{
		Nv12Layout layout = _layout;
		using (MFMediaBuffer buffer = sample.ConvertToContiguousBuffer())
		{
			IntPtr source = buffer.Lock(out _, out int length);
			try
			{
				// 레이아웃과 버퍼 크기가 맞지 않으면 범위 밖을 읽지 않도록 프레임을 버린다
				if (length < layout.RequiredLength)
				{
					return null;
				}
				UnmanagedArray<byte> frame = new UnmanagedArray<byte>(Width * Height * 4);
				if (layout.Width < Width || layout.Height < Height)
				{
					frame.Fill(0, 0, 0, 255);
				}
				ConvertNv12ToBgra(source, layout, frame.DataPtr, Width * 4);
				return frame;
			}
			finally
			{
				buffer.Unlock();
			}
		}
	}

	private static bool TryGetSize(MFAttributes attributes, Guid key, out int high, out int low)
	{
		if (attributes.GetUINT64Native(key, out long packed) < 0)
		{
			high = 0;
			low = 0;
			return false;
		}
		high = (int)(packed >> 32);
		low = (int)(packed & 0xFFFFFFFFL);
		return true;
	}

	private static bool TryGetUInt32(MFAttributes attributes, Guid key, out int value)
	{
		return attributes.GetUINT32Native(key, out value) >= 0;
	}

	private void ClearQueue()
	{
		foreach (VideoFrame frame in _frameQueue)
		{
			frame.Data.Dispose();
		}
		_frameQueue.Clear();
	}

	public void Dispose()
	{
		if (_isDisposed)
		{
			return;
		}
		_isDisposed = true;
		_isPlaying = false;
		_isPaused = false;
		StopDecoding();
		lock (_queueLock)
		{
			ClearQueue();
		}
		lock (_readerLock)
		{
			try
			{
				_reader?.Dispose();
			}
			catch (Exception)
			{
			}
			_reader = null;
		}
	}

	private readonly struct VideoFrame
	{
		public readonly double TimeMs;

		public readonly UnmanagedArray<byte> Data;

		public VideoFrame(double timeMs, UnmanagedArray<byte> data)
		{
			TimeMs = timeMs;
			Data = data;
		}
	}
}
