using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MistUtils;

namespace MBMPlay;

public static partial class Numbers
{
	public static int MasterVolume
	{
		get
		{
			return masterVolume;
		}
		set
		{
			if (value < 0)
			{
				masterVolume = 0;
			}
			else if (value > 100)
			{
				masterVolume = 100;
			}
			else
			{
				masterVolume = value;
			}
			OnMasterVolumeChanged();
		}
	}

	public static int KeyVolume
	{
		get
		{
			return keyVolume;
		}
		set
		{
			if (value < 0)
			{
				keyVolume = 0;
			}
			else if (value > 100)
			{
				keyVolume = 100;
			}
			else
			{
				keyVolume = value;
			}
		}
	}

	public static int BgmVolume
	{
		get
		{
			return bgmVolume;
		}
		set
		{
			if (value < 0)
			{
				bgmVolume = 0;
			}
			else if (value > 100)
			{
				bgmVolume = 100;
			}
			else
			{
				bgmVolume = value;
			}
		}
	}

	public static int PlayStyle
	{
		get
		{
			return playStyle;
		}
		set
		{
			if (playStyleList.Contains(value))
			{
				playStyle = value;
			}
		}
	}

	public static double MusicSpeed
	{
		get
		{
			if (MusicSpeedLevel == 0)
			{
				return 1.0;
			}
			return Math.Pow(2.0, (double)MusicSpeedLevel / 12.0);
		}
	}

	public static int MusicSpeedLevel
	{
		get
		{
			return musicSpeedLevel;
		}
		set
		{
			if (value < -12)
			{
				musicSpeedLevel = -12;
			}
			else if (value > 12)
			{
				musicSpeedLevel = 12;
			}
			else
			{
				musicSpeedLevel = value;
			}
		}
	}

	public static int Player1LaneCover
	{
		get
		{
			return _player1LaneCover;
		}
		set
		{
			if (value > LaneCoverMax)
			{
				_player1LaneCover = LaneCoverMax;
			}
			else if (value < LaneCoverMin)
			{
				_player1LaneCover = LaneCoverMin;
			}
			else
			{
				_player1LaneCover = value;
			}
		}
	}

	public static int Player2LaneCover
	{
		get
		{
			return _player2LaneCover;
		}
		set
		{
			if (value > LaneCoverMax)
			{
				_player2LaneCover = LaneCoverMax;
			}
			else if (value < LaneCoverMin)
			{
				_player2LaneCover = LaneCoverMin;
			}
			else
			{
				_player2LaneCover = value;
			}
		}
	}

	public static int Player1Lift
	{
		get
		{
			return _player1Lift;
		}
		set
		{
			if (value > LiftMax)
			{
				_player1Lift = LiftMax;
			}
			else if (value < LiftMin)
			{
				_player1Lift = LiftMin;
			}
			else
			{
				_player1Lift = value;
			}
		}
	}

	public static int Player2Lift
	{
		get
		{
			return _player2Lift;
		}
		set
		{
			if (value > LiftMax)
			{
				_player2Lift = LiftMax;
			}
			else if (value < LiftMin)
			{
				_player2Lift = LiftMin;
			}
			else
			{
				_player2Lift = value;
			}
		}
	}

	internal static void OnMasterVolumeChanged()
	{
		MasterVolumeChanged?.Invoke(null, EventArgs.Empty);
	}
}
