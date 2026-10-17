using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MBMPlay.Common;
using MistUtils;
using Tsukikage.DirectShow;
using Tsukikage.DirectShow.ComInterop;
using UnmanagedCollections;

namespace MBMPlay;

public class DirectShowMovie : IMovie, IDisposable
{
	private SampleGrabberGraph _graph;

	private List<UnmanagedArray<byte>> frameDataList = new List<UnmanagedArray<byte>>();

	private readonly object _lockObj = new object();

	private double _startTime;

	public int UpdateCount;

	public double CurrentPosition
	{
		get
		{
			return _graph.CurrentPosition;
		}
		set
		{
			_graph.CurrentPosition = value;
		}
	}

	public double TotalTime { get; private set; }

	public GraphState Status => _graph.Status;

	public double FPS
	{
		get
		{
			if (_graph == null)
			{
				return 0.0;
			}
			return _graph.VideoFPS;
		}
	}

	public bool NeedsCapture => frameDataList.Count > 0;

	public bool IsPlaying => _graph.IsPlaying;

	public bool IsStopped => _graph.IsStopped;

	public bool IsPaused => _graph.IsPaused;

	public int Width => _graph.VideoSize.Width;

	public int Height => _graph.VideoSize.Height;

	public static IMovie Load(string path)
	{
		DirectShowMovie directShowMovie = new DirectShowMovie();
		try
		{
			directShowMovie._graph = new SampleGrabberGraph(path);
			directShowMovie._graph.Load();
			if (directShowMovie._graph.HasVideo)
			{
				directShowMovie._graph.VideoFrame += directShowMovie.graph_VideoFrame;
				IMediaSeeking mediaSeeking = directShowMovie._graph.GraphBuilder as IMediaSeeking;
				try
				{
					mediaSeeking.GetDuration(out var pDuration);
					directShowMovie.TotalTime = (double)pDuration / 10000.0;
				}
				catch (COMException ex)
				{
					if (ex.HResult != -2147220895)
					{
						throw;
					}
					directShowMovie.TotalTime = 0.0;
				}
			}
		}
		catch (Exception)
		{
			directShowMovie._graph?.Dispose();
			directShowMovie._graph = null;
			return null;
		}
		return directShowMovie;
	}

	public UnmanagedArray<byte> GetFrameData()
	{
		UnmanagedArray<byte> result = null;
		lock (_lockObj)
		{
			if (frameDataList.Count > 0)
			{
				result = frameDataList[0];
				frameDataList.RemoveAt(0);
			}
		}
		return result;
	}

	public void Play()
	{
		if (_graph.IsPlaying)
		{
			_graph.Stop();
		}
		_graph.Play();
		_startTime = EventTimers.GetNowTime();
		UpdateCount = 0;
	}

	public void Stop()
	{
		_graph.Stop();
		UnmanagedArray<byte> unmanagedArray = new UnmanagedArray<byte>(Width * 4 * Height);
		unmanagedArray.Fill(0, 0, 0, 255);
		lock (_lockObj)
		{
			Utils.Free(frameDataList);
			frameDataList.Add(unmanagedArray);
		}
	}

	public void Pause()
	{
		_graph.Pause();
	}

	private void graph_VideoFrame(double sampleTime, IntPtr pFrame, int bufferSize)
	{
		Task.Factory.StartNew(() =>
		{
			UnmanagedArray<byte> unmanagedArray = new UnmanagedArray<byte>(Width * 4 * Height);
			_graph.RenderVideoFrameToTexture(pFrame, unmanagedArray.DataPtr, Width, Height);
			lock (_lockObj)
			{
				frameDataList.Add(unmanagedArray);
				UpdateCount++;
			}
		});
	}

	public void Dispose()
	{
		lock (_lockObj)
		{
			Utils.Free(frameDataList);
		}
		_graph?.Dispose();
		_graph = null;
	}
}
