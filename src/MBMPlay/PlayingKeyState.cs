using System;
using System.Collections.Generic;
using MBMPlay.Common;

namespace MBMPlay;

public static partial class PlayingKeyState
{
	private static readonly HashSet<PlayingKeys> ScratchRotaryInput = new HashSet<PlayingKeys>
	{
		PlayingKeys.Player1ScratchLeftRotary,
		PlayingKeys.Player1ScratchRightRotary,
		PlayingKeys.Player2ScratchLeftRotary,
		PlayingKeys.Player2ScratchRightRotary
	};

	private static Dictionary<PlayingKeys, bool> currentPlayingScratchRotaryStates = InitPlayingScratchRotaryStates();

	private static Dictionary<PlayingKeys, bool> previousPlayingScratchRotaryStates = InitPlayingScratchRotaryStates();

	private static Dictionary<PlayingKeys, PlayingKeyStateFlag> nowStates;

	private static Dictionary<PlayingKeys, PlayingKeyStateFlag> prevStates;

	private static Dictionary<PlayingKeys, PlayingKeyStateTime> stateTime;

	private static HashSet<PlayingKeys> KeyDownRequest = new HashSet<PlayingKeys>();

	private static HashSet<PlayingKeys> KeyUpRequest = new HashSet<PlayingKeys>();

	private static double updateTime = 0.0;

	private static double scratchRotaryUpdateInterval = 0.0;

	private static double scratchRotaryActiveTime = 0.0;

	private static int scratchRotaryThreshold = 1;

	private static double player1LastScratchRotaryUpdateTime = 0.0;

	private static double player2LastScratchRotaryUpdateTime = 0.0;

	private static double player1LastScratchRotaryActiveStartTime = 0.0;

	private static double player2LastScratchRotaryActiveStartTime = 0.0;

	private static int player1ScratchRotaryCount = 0;

	private static int player2ScratchRotaryCount = 0;

	public static PlayingKeyConfig KeyConfig { get; set; }

	public static double MinKeyDownTime { get; set; }

	public static double MinKeyUpTime { get; set; }

	public static int Player1ScratchRotaryValue { get; private set; } = 0;

	public static int Player2ScratchRotaryValue { get; private set; } = 0;

	public static double Player1PlayingScratchLeftRotaryDownTimer { get; private set; } = 0.0;

	public static double Player1PlayingScratchLeftRotaryDownAltTimer { get; private set; } = 0.0;

	public static double Player1PlayingScratchLeftRotaryUpTimer { get; private set; } = 0.0;

	public static double Player2PlayingScratchLeftRotaryDownTimer { get; private set; } = 0.0;

	public static double Player2PlayingScratchLeftRotaryDownAltTimer { get; private set; } = 0.0;

	public static double Player2PlayingScratchLeftRotaryUpTimer { get; private set; } = 0.0;

	public static double Player1PlayingScratchRightRotaryDownTimer { get; private set; } = 0.0;

	public static double Player1PlayingScratchRightRotaryDownAltTimer { get; private set; } = 0.0;

	public static double Player1PlayingScratchRightRotaryUpTimer { get; private set; } = 0.0;

	public static double Player2PlayingScratchRightRotaryDownTimer { get; private set; } = 0.0;

	public static double Player2PlayingScratchRightRotaryDownAltTimer { get; private set; } = 0.0;

	public static double Player2PlayingScratchRightRotaryUpTimer { get; private set; } = 0.0;

	public static double Player1PlayingScratchRotaryDownTimer => Math.Max(Player1PlayingScratchLeftRotaryDownTimer, Player1PlayingScratchRightRotaryDownTimer);

	public static double Player1PlayingScratchRotaryDownAltTimer => Math.Max(Player1PlayingScratchLeftRotaryDownAltTimer, Player1PlayingScratchRightRotaryDownAltTimer);

	public static double Player1PlayingScratchRotaryUpTimer
	{
		get
		{
			if (IsPlayer1PlayingScratchRotaryDown)
			{
				return 0.0;
			}
			return Math.Max(Player1PlayingScratchLeftRotaryUpTimer, Player1PlayingScratchRightRotaryUpTimer);
		}
	}

	public static double Player2PlayingScratchRotaryDownTimer => Math.Max(Player2PlayingScratchLeftRotaryDownTimer, Player2PlayingScratchRightRotaryDownTimer);

	public static double Player2PlayingScratchRotaryDownAltTimer => Math.Max(Player2PlayingScratchLeftRotaryDownAltTimer, Player2PlayingScratchRightRotaryDownAltTimer);

	public static double Player2PlayingScratchRotaryUpTimer
	{
		get
		{
			if (IsPlayer2PlayingScratchRotaryDown)
			{
				return 0.0;
			}
			return Math.Max(Player2PlayingScratchLeftRotaryUpTimer, Player2PlayingScratchRightRotaryUpTimer);
		}
	}

	public static bool IsPlayer1PlayingScratchLeftRotaryDown => currentPlayingScratchRotaryStates[PlayingKeys.Player1ScratchLeftRotary];

	public static bool IsPlayer1PlayingScratchLeftRotaryDownOnce
	{
		get
		{
			if (currentPlayingScratchRotaryStates[PlayingKeys.Player1ScratchLeftRotary])
			{
				return !previousPlayingScratchRotaryStates[PlayingKeys.Player1ScratchLeftRotary];
			}
			return false;
		}
	}

	public static bool IsPlayer1PlayingScratchLeftRotaryUp => !currentPlayingScratchRotaryStates[PlayingKeys.Player1ScratchLeftRotary];

	public static bool IsPlayer1PlayingScratchLeftRotaryUpOnce
	{
		get
		{
			if (!currentPlayingScratchRotaryStates[PlayingKeys.Player1ScratchLeftRotary])
			{
				return previousPlayingScratchRotaryStates[PlayingKeys.Player1ScratchLeftRotary];
			}
			return false;
		}
	}

	public static bool IsPlayer1PlayingScratchRightRotaryDown => currentPlayingScratchRotaryStates[PlayingKeys.Player1ScratchRightRotary];

	public static bool IsPlayer1PlayingScratchRightRotaryDownOnce
	{
		get
		{
			if (currentPlayingScratchRotaryStates[PlayingKeys.Player1ScratchRightRotary])
			{
				return !previousPlayingScratchRotaryStates[PlayingKeys.Player1ScratchRightRotary];
			}
			return false;
		}
	}

	public static bool IsPlayer1PlayingScratchRightRotaryUp => !currentPlayingScratchRotaryStates[PlayingKeys.Player1ScratchRightRotary];

	public static bool IsPlayer1PlayingScratchRightRotaryUpOnce
	{
		get
		{
			if (!currentPlayingScratchRotaryStates[PlayingKeys.Player1ScratchRightRotary])
			{
				return previousPlayingScratchRotaryStates[PlayingKeys.Player1ScratchRightRotary];
			}
			return false;
		}
	}

	public static bool IsPlayer2PlayingScratchLeftRotaryDown => currentPlayingScratchRotaryStates[PlayingKeys.Player2ScratchLeftRotary];

	public static bool IsPlayer2PlayingScratchLeftRotaryDownOnce
	{
		get
		{
			if (currentPlayingScratchRotaryStates[PlayingKeys.Player2ScratchLeftRotary])
			{
				return !previousPlayingScratchRotaryStates[PlayingKeys.Player2ScratchLeftRotary];
			}
			return false;
		}
	}

	public static bool IsPlayer2PlayingScratchLeftRotaryUp => !currentPlayingScratchRotaryStates[PlayingKeys.Player2ScratchLeftRotary];

	public static bool IsPlayer2PlayingScratchLeftRotaryUpOnce
	{
		get
		{
			if (!currentPlayingScratchRotaryStates[PlayingKeys.Player2ScratchLeftRotary])
			{
				return previousPlayingScratchRotaryStates[PlayingKeys.Player2ScratchLeftRotary];
			}
			return false;
		}
	}

	public static bool IsPlayer2PlayingScratchRightRotaryDown => currentPlayingScratchRotaryStates[PlayingKeys.Player2ScratchRightRotary];

	public static bool IsPlayer2PlayingScratchRightRotaryDownOnce
	{
		get
		{
			if (currentPlayingScratchRotaryStates[PlayingKeys.Player2ScratchRightRotary])
			{
				return !previousPlayingScratchRotaryStates[PlayingKeys.Player2ScratchRightRotary];
			}
			return false;
		}
	}

	public static bool IsPlayer2PlayingScratchRightRotaryUp => !currentPlayingScratchRotaryStates[PlayingKeys.Player2ScratchRightRotary];

	public static bool IsPlayer2PlayingScratchRightRotaryUpOnce
	{
		get
		{
			if (!currentPlayingScratchRotaryStates[PlayingKeys.Player2ScratchRightRotary])
			{
				return previousPlayingScratchRotaryStates[PlayingKeys.Player2ScratchRightRotary];
			}
			return false;
		}
	}

	public static bool IsPlayer1PlayingScratchRotaryDown
	{
		get
		{
			if (!IsPlayer1PlayingScratchLeftRotaryDown)
			{
				return IsPlayer1PlayingScratchRightRotaryDown;
			}
			return true;
		}
	}

	public static bool IsPlayer1PlayingScratchRotaryDownOnce
	{
		get
		{
			if (!IsPlayer1PlayingScratchLeftRotaryDownOnce)
			{
				return IsPlayer1PlayingScratchRightRotaryDownOnce;
			}
			return true;
		}
	}

	public static bool IsPlayer1PlayingScratchRotaryUp
	{
		get
		{
			if (!IsPlayer1PlayingScratchLeftRotaryUp)
			{
				return IsPlayer1PlayingScratchRightRotaryUp;
			}
			return true;
		}
	}

	public static bool IsPlayer1PlayingScratchRotaryUpOnce
	{
		get
		{
			if (!IsPlayer1PlayingScratchLeftRotaryUpOnce)
			{
				return IsPlayer1PlayingScratchRightRotaryUpOnce;
			}
			return true;
		}
	}

	public static bool IsPlayer2PlayingScratchRotaryDown
	{
		get
		{
			if (!IsPlayer2PlayingScratchLeftRotaryDown)
			{
				return IsPlayer2PlayingScratchRightRotaryDown;
			}
			return true;
		}
	}

	public static bool IsPlayer2PlayingScratchRotaryDownOnce
	{
		get
		{
			if (!IsPlayer2PlayingScratchLeftRotaryDownOnce)
			{
				return IsPlayer2PlayingScratchRightRotaryDownOnce;
			}
			return true;
		}
	}

	public static bool IsPlayer2PlayingScratchRotaryUp
	{
		get
		{
			if (!IsPlayer2PlayingScratchLeftRotaryUp)
			{
				return IsPlayer2PlayingScratchRightRotaryUp;
			}
			return true;
		}
	}

	public static bool IsPlayer2PlayingScratchRotaryUpOnce
	{
		get
		{
			if (!IsPlayer2PlayingScratchLeftRotaryUpOnce)
			{
				return IsPlayer2PlayingScratchRightRotaryUpOnce;
			}
			return true;
		}
	}

	public static ScratchDirection Player1ScratchToggleDirection { get; private set; } = ScratchDirection.Right;

	public static ScratchDirection Player2ScratchToggleDirection { get; private set; } = ScratchDirection.Right;

	private static Dictionary<PlayingKeys, bool> InitPlayingScratchRotaryStates()
	{
		return new Dictionary<PlayingKeys, bool>
		{
			{
				PlayingKeys.Player1ScratchLeftRotary,
				false
			},
			{
				PlayingKeys.Player1ScratchRightRotary,
				false
			},
			{
				PlayingKeys.Player2ScratchLeftRotary,
				false
			},
			{
				PlayingKeys.Player2ScratchRightRotary,
				false
			}
		};
	}

	public static void Init()
	{
		stateTime = new Dictionary<PlayingKeys, PlayingKeyStateTime>();
		nowStates = new Dictionary<PlayingKeys, PlayingKeyStateFlag>();
		prevStates = new Dictionary<PlayingKeys, PlayingKeyStateFlag>();
		foreach (PlayingKeys value in Enum.GetValues(typeof(PlayingKeys)))
		{
			stateTime.Add(value, new PlayingKeyStateTime(0.0, 0.0));
			nowStates.Add(value, new PlayingKeyStateFlag(keyDown: false, keyUp: true));
			prevStates.Add(value, new PlayingKeyStateFlag(keyDown: false, keyUp: true));
		}
		MinKeyDownTime = Program.Settings.MinKeyDownTime;
		MinKeyUpTime = Program.Settings.MinKeyUpTime;
		scratchRotaryUpdateInterval = Program.Settings.ScratchRotaryUpdateInterval;
		scratchRotaryActiveTime = Program.Settings.ScratchRotaryActiveTime;
		scratchRotaryThreshold = Program.Settings.ScratchRotaryThreshold;
		if (scratchRotaryThreshold < 1)
		{
			scratchRotaryThreshold = 1;
		}
	}

	public static void ClearKeyboardAssign()
	{
		KeyConfig = null;
	}

	public static void SetKeyDown(PlayingKeys key)
	{
		if (IsKeyDown(key))
		{
			prevStates[key].IsKeyDown = false;
			prevStates[key].IsKeyUp = true;
		}
		stateTime[key].KeyDownTime = updateTime;
		CheckState(key);
	}

	public static void SetKeyDownOnce(PlayingKeys key)
	{
		if (IsKeyDown(key))
		{
			prevStates[key].IsKeyDown = false;
			prevStates[key].IsKeyUp = true;
		}
		stateTime[key].KeyDownTime = updateTime;
		KeyUpRequest.Add(key);
		CheckState(key);
	}

	public static void SetKeyUp(PlayingKeys key)
	{
		if (IsKeyDown(key))
		{
			stateTime[key].KeyUpTime = updateTime;
			CheckState(key);
		}
	}

	public static bool IsKeyDown(PlayingKeys key)
	{
		return nowStates[key].IsKeyDown;
	}

	public static bool IsKeyDownAll(params PlayingKeys[] keys)
	{
		if (keys == null)
		{
			return false;
		}
		for (int i = 0; i < keys.Length; i++)
		{
			if (!IsKeyDown(keys[i]))
			{
				return false;
			}
		}
		return true;
	}

	public static bool IsKeyDownAny(params PlayingKeys[] keys)
	{
		if (keys == null)
		{
			return false;
		}
		for (int i = 0; i < keys.Length; i++)
		{
			if (IsKeyDown(keys[i]))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsKeyDownOnce(PlayingKeys key)
	{
		if (nowStates[key].IsKeyDown)
		{
			return prevStates[key].IsKeyUp;
		}
		return false;
	}

	public static bool IsKeyDownOnceAny(params PlayingKeys[] keys)
	{
		if (keys == null)
		{
			return false;
		}
		for (int i = 0; i < keys.Length; i++)
		{
			if (IsKeyDownOnce(keys[i]))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsKeyUp(PlayingKeys key)
	{
		return nowStates[key].IsKeyUp;
	}

	public static bool IsKeyUpAll(params PlayingKeys[] keys)
	{
		if (keys == null)
		{
			return false;
		}
		for (int i = 0; i < keys.Length; i++)
		{
			if (!IsKeyUp(keys[i]))
			{
				return false;
			}
		}
		return true;
	}

	public static bool IsKeyUpAny(params PlayingKeys[] keys)
	{
		if (keys == null)
		{
			return false;
		}
		for (int i = 0; i < keys.Length; i++)
		{
			if (IsKeyUp(keys[i]))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsKeyUpOnce(PlayingKeys key)
	{
		if (nowStates[key].IsKeyUp)
		{
			return prevStates[key].IsKeyDown;
		}
		return false;
	}

	public static bool IsKeyUpOnceAny(params PlayingKeys[] keys)
	{
		if (keys == null)
		{
			return false;
		}
		for (int i = 0; i < keys.Length; i++)
		{
			if (IsKeyUpOnce(keys[i]))
			{
				return true;
			}
		}
		return false;
	}

	public static void ResetScratchTimers()
	{
		Player1PlayingScratchLeftRotaryDownTimer = 0.0;
		Player1PlayingScratchLeftRotaryDownAltTimer = 0.0;
		Player1PlayingScratchLeftRotaryUpTimer = 0.0;
		Player2PlayingScratchLeftRotaryDownTimer = 0.0;
		Player2PlayingScratchLeftRotaryDownAltTimer = 0.0;
		Player2PlayingScratchLeftRotaryUpTimer = 0.0;
		Player1PlayingScratchRightRotaryDownTimer = 0.0;
		Player1PlayingScratchRightRotaryDownAltTimer = 0.0;
		Player1PlayingScratchRightRotaryUpTimer = 0.0;
		Player2PlayingScratchRightRotaryDownTimer = 0.0;
		Player2PlayingScratchRightRotaryDownAltTimer = 0.0;
		Player2PlayingScratchRightRotaryUpTimer = 0.0;
	}
}
