using System;
using UnmanagedCollections;

namespace MBMPlay.Common;

public interface IMovie : IDisposable
{
	double CurrentPosition { get; set; }

	double TotalTime { get; }

	double FPS { get; }

	bool NeedsCapture { get; }

	bool IsPlaying { get; }

	bool IsStopped { get; }

	bool IsPaused { get; }

	int Width { get; }

	int Height { get; }

	UnmanagedArray<byte> GetFrameData();

	void Play();

	void Stop();

	void Pause();
}
