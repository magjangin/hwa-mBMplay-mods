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
	private void ResetBmsState()
	{
		EventTimers.PlayStart = 0.0;
		lock (_loadWavLockObj)
		{
			foreach (KeyValuePair<string, AudioSource> keySound in _keySounds)
			{
				keySound.Value.Stop();
			}
		}
		lock (_loadBgaLockObj)
		{
			foreach (KeyValuePair<string, IMovie> movie in _movies)
			{
				if (movie.Value.IsPlaying)
				{
					movie.Value.Stop();
				}
			}
		}
		EventTimers.ResetPlayingTimers();
		EventTimers.StopBpmTimer();
		Flags.ResetPlayingFlags();
		AutoPlayKeyState.ResetKeyState();
		Numbers.ElapsedMusicTime = TimeSpan.Zero;
		Numbers.ResetNotesPerSecond();
		Numbers.ClearJudgeCount();
		_nowBga = "";
		_nowLayer = "";
		_nowPoor = "";
		_prevBga = "";
		_prevLayer = "";
		_prevPoor = "";
		_reverseTime = 0.0;
		_lastCount = 0;
		_startLNHolding.Clear();
		_startLNHoldingProcessed = false;
		_laneIndex.Clear();
		_lnJudge.Clear();
		_lnJudgeTime.Clear();
		for (int i = 0; i < _lastKeySounds.Count; i++)
		{
			_lastKeySounds[i] = "";
			_laneIndex.Add(i);
			_lnJudge.Add(JudgeType.None);
			_lnJudgeTime.Add(0.0);
		}
		_isBarlineCountAdded = false;
		_isBarlineReached = false;
		Program.InvokeMainThreadAndWait(() =>
		{
			lock (Program.GraphicsLockObj)
			{
				if (_bgaTexture != null)
				{
					((IDynamicTextureInfo)_bgaTexture).Clear();
				}
				if (_layerTexture != null)
				{
					((IDynamicTextureInfo)_layerTexture).Clear();
				}
				if (_poorTexture != null)
				{
					((IDynamicTextureInfo)_poorTexture).Clear();
				}
			}
		});
	}

	private void Restart(int restartBar, bool swapLanes)
	{
		if (restartBar < 0 || restartBar > Numbers.TotalBars)
		{
			return;
		}
		ResetBmsState();
		if (swapLanes)
		{
			if (Flags.Player1LaneSwapOption != LaneSwapOption.None || Flags.Player2LaneSwapOption != LaneSwapOption.None || Flags.FlipSide)
			{
				_baseTimeTable = _originalTimeTable.SwapLanes(_bmsData.Keys, Flags.Player1LaneSwapOption, Flags.Player2LaneSwapOption, Flags.FlipSide, Program.Random.NextLong());
				CheckScratchSideForAllScratch();
			}
			else
			{
				_baseTimeTable = _originalTimeTable;
			}
			InitBarInfo();
			GC.Collect();
		}
		_timeTable = new BmsTimeTable(_baseTimeTable, Numbers.MusicSpeed);
		Numbers.Bpm = _barInfo[restartBar].Bpm * Numbers.MusicSpeed;
		Numbers.GrooveGauge = ((restartBar <= 0) ? 20.0 : 2.0);
		Numbers.GaugeTotalIncrement = 0.0;
		Numbers.CurrentBars = ((restartBar > 0) ? (restartBar - 1) : 0);
		Numbers.MaxBpm = _timeTable.MaxBpm;
		Numbers.MinBpm = _timeTable.MinBpm;
		Numbers.MainNotesBpm = _timeTable.MainNotesBpm;
		Numbers.MainTimeBpm = _timeTable.MainTimeBpm;
		Numbers.TotalNotes = (Flags.LegacyLN ? _bmsData.TotalNotesWithoutLNEnd : _bmsData.TotalNotes);
		Numbers.TotalNotesPerLane = _baseTimeTable.GetTotalNotesPerLane(!Flags.LegacyLN);
		_gaugeIncrements[JudgeType.PGreat] = Numbers.GaugeTotal / (double)Numbers.TotalNotes;
		_gaugeIncrements[JudgeType.Great] = _gaugeIncrements[JudgeType.PGreat];
		_gaugeIncrements[JudgeType.Good] = _gaugeIncrements[JudgeType.PGreat] / 2.0;
		_nowBga = _barInfo[restartBar].Bga;
		_nowLayer = _barInfo[restartBar].Layer;
		_nowPoor = _barInfo[restartBar].Poor;
		if (_hasPoorBga && _nowPoor == "")
		{
			_nowPoor = "00";
		}
		_prevBga = "*";
		_prevLayer = "*";
		_prevPoor = "*";
		_lastIndex = _lastNoteIndex;
		_startLNHolding.AddRange(_barInfo[restartBar].LNHolding);
		EventTimers.UpdateNowTime();
		EventTimers.PlayStart = ((_baseTimeTable.Rows.Count > _baseTimeTable.BarIndex[restartBar]) ? (EventTimers.NowTime - _baseTimeTable.Rows[_baseTimeTable.BarIndex[restartBar]].Time * (1.0 / Numbers.MusicSpeed)) : EventTimers.NowTime);
		EventTimers.ResetBpmTimer();
		if (Numbers.MusicSpeedLevel != _prevMusicSpeedLevel)
		{
			foreach (KeyValuePair<string, AudioSource> keySound in _keySounds)
			{
				keySound.Value.Speed = (float)Numbers.MusicSpeed;
			}
		}
		foreach (KeyValuePair<string, double> item in _barInfo[restartBar].Bgm)
		{
			if (_keySounds.ContainsKey(item.Key))
			{
				_keySounds[item.Key].Volume = (Flags.IsBgmVolumeMute ? 0f : ((float)Numbers.BgmVolume / 100f));
				if (item.Value / 1000.0 < _keySounds[item.Key].TotalTime && (!Flags.IsBgmVolumeMute || Numbers.BgmVolume > 0))
				{
					_keySounds[item.Key].Play(_keySounds[item.Key].GetPositionFromSeconds(item.Value / 1000.0));
				}
			}
		}
		if (Flags.AutoPlay)
		{
			foreach (KeyValuePair<string, double> key in _barInfo[restartBar].Keys)
			{
				if (_keySounds.ContainsKey(key.Key))
				{
					_keySounds[key.Key].Volume = (Flags.IsKeyVolumeMute ? 0f : ((float)Numbers.KeyVolume / 100f));
					if (key.Value / 1000.0 < _keySounds[key.Key].TotalTime && (!Flags.IsKeyVolumeMute || Numbers.KeyVolume > 0))
					{
						_keySounds[key.Key].Play(_keySounds[key.Key].GetPositionFromSeconds(key.Value / 1000.0));
					}
				}
			}
		}
		RestartMovie(_nowBga, _barInfo[restartBar].BgaPosition);
		RestartMovie(_nowLayer, _barInfo[restartBar].LayerPosition);
		RestartMovie(_nowPoor, _barInfo[restartBar].PoorPosition);
		double num = (EventTimers.NowTime - EventTimers.PlayStart) * Numbers.MusicSpeed;
		Numbers.ElapsedMusicTime = ((num < _totalMusicTime) ? TimeSpan.FromMilliseconds(num) : Numbers.TotalMusicTime);
	}

	private void Restart(int restartBar)
	{
		Restart(restartBar, swapLanes: false);
	}

	private void Restart(bool swapLanes)
	{
		Restart(StartBar, swapLanes);
	}

	private void Restart()
	{
		Restart(StartBar, swapLanes: false);
	}
}
