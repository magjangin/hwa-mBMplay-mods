using System;
using System.Collections.Generic;
using MBMPlay.Common;

namespace MBMPlay;

public static class PlayingKeyState
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

	public static void GetState()
	{
		foreach (PlayingKeys value in Enum.GetValues(typeof(PlayingKeys)))
		{
			prevStates[value] = new PlayingKeyStateFlag(nowStates[value].IsKeyDown, nowStates[value].IsKeyUp);
			CheckState(value);
		}
		previousPlayingScratchRotaryStates = currentPlayingScratchRotaryStates;
		currentPlayingScratchRotaryStates = InitPlayingScratchRotaryStates();
		if (player1LastScratchRotaryUpdateTime != 0.0)
		{
			if (Player1ScratchRotaryValue == 0)
			{
				if (updateTime - player1LastScratchRotaryActiveStartTime > scratchRotaryActiveTime)
				{
					currentPlayingScratchRotaryStates[PlayingKeys.Player1ScratchLeftRotary] = false;
					currentPlayingScratchRotaryStates[PlayingKeys.Player1ScratchRightRotary] = false;
					Player1PlayingScratchLeftRotaryDownTimer = 0.0;
					Player1PlayingScratchRightRotaryDownTimer = 0.0;
					if (previousPlayingScratchRotaryStates[PlayingKeys.Player1ScratchLeftRotary])
					{
						Player1PlayingScratchLeftRotaryUpTimer = updateTime;
						Player1PlayingScratchRightRotaryUpTimer = 0.0;
					}
					else if (previousPlayingScratchRotaryStates[PlayingKeys.Player1ScratchRightRotary])
					{
						Player1PlayingScratchLeftRotaryUpTimer = 0.0;
						Player1PlayingScratchRightRotaryUpTimer = updateTime;
					}
				}
				else
				{
					currentPlayingScratchRotaryStates[PlayingKeys.Player1ScratchLeftRotary] = previousPlayingScratchRotaryStates[PlayingKeys.Player1ScratchLeftRotary];
					currentPlayingScratchRotaryStates[PlayingKeys.Player1ScratchRightRotary] = previousPlayingScratchRotaryStates[PlayingKeys.Player1ScratchRightRotary];
				}
			}
			else if (Player1ScratchRotaryValue < 0)
			{
				if (!previousPlayingScratchRotaryStates[PlayingKeys.Player1ScratchLeftRotary])
				{
					if (Player1ScratchRotaryValue * -1 > scratchRotaryThreshold)
					{
						currentPlayingScratchRotaryStates[PlayingKeys.Player1ScratchLeftRotary] = true;
						currentPlayingScratchRotaryStates[PlayingKeys.Player1ScratchRightRotary] = false;
						Player1PlayingScratchLeftRotaryDownTimer = updateTime;
						Player1PlayingScratchLeftRotaryDownAltTimer = updateTime;
						Player1PlayingScratchLeftRotaryUpTimer = 0.0;
						Player1PlayingScratchRightRotaryDownTimer = 0.0;
						Player1PlayingScratchRightRotaryDownAltTimer = 0.0;
						if (previousPlayingScratchRotaryStates[PlayingKeys.Player1ScratchRightRotary])
						{
							Player1PlayingScratchRightRotaryUpTimer = updateTime;
						}
						else
						{
							Player1PlayingScratchRightRotaryUpTimer = 0.0;
						}
					}
					else
					{
						currentPlayingScratchRotaryStates[PlayingKeys.Player1ScratchLeftRotary] = previousPlayingScratchRotaryStates[PlayingKeys.Player1ScratchLeftRotary];
						currentPlayingScratchRotaryStates[PlayingKeys.Player1ScratchRightRotary] = previousPlayingScratchRotaryStates[PlayingKeys.Player1ScratchRightRotary];
					}
				}
				else
				{
					currentPlayingScratchRotaryStates[PlayingKeys.Player1ScratchLeftRotary] = true;
					currentPlayingScratchRotaryStates[PlayingKeys.Player1ScratchRightRotary] = false;
				}
			}
			else if (Player1ScratchRotaryValue > 0)
			{
				if (!previousPlayingScratchRotaryStates[PlayingKeys.Player1ScratchRightRotary])
				{
					if (Player1ScratchRotaryValue > scratchRotaryThreshold)
					{
						currentPlayingScratchRotaryStates[PlayingKeys.Player1ScratchLeftRotary] = false;
						currentPlayingScratchRotaryStates[PlayingKeys.Player1ScratchRightRotary] = true;
						Player1PlayingScratchLeftRotaryDownTimer = 0.0;
						Player1PlayingScratchLeftRotaryDownAltTimer = 0.0;
						Player1PlayingScratchRightRotaryDownTimer = updateTime;
						Player1PlayingScratchRightRotaryDownAltTimer = updateTime;
						Player1PlayingScratchRightRotaryUpTimer = 0.0;
						if (previousPlayingScratchRotaryStates[PlayingKeys.Player1ScratchRightRotary])
						{
							Player1PlayingScratchLeftRotaryUpTimer = updateTime;
						}
						else
						{
							Player1PlayingScratchLeftRotaryUpTimer = 0.0;
						}
					}
					else
					{
						currentPlayingScratchRotaryStates[PlayingKeys.Player1ScratchLeftRotary] = previousPlayingScratchRotaryStates[PlayingKeys.Player1ScratchLeftRotary];
						currentPlayingScratchRotaryStates[PlayingKeys.Player1ScratchRightRotary] = previousPlayingScratchRotaryStates[PlayingKeys.Player1ScratchRightRotary];
					}
				}
				else
				{
					currentPlayingScratchRotaryStates[PlayingKeys.Player1ScratchLeftRotary] = false;
					currentPlayingScratchRotaryStates[PlayingKeys.Player1ScratchRightRotary] = true;
				}
			}
		}
		if (player2LastScratchRotaryUpdateTime == 0.0)
		{
			return;
		}
		if (Player2ScratchRotaryValue == 0)
		{
			if (updateTime - player2LastScratchRotaryActiveStartTime > scratchRotaryActiveTime)
			{
				currentPlayingScratchRotaryStates[PlayingKeys.Player2ScratchLeftRotary] = false;
				currentPlayingScratchRotaryStates[PlayingKeys.Player2ScratchRightRotary] = false;
				Player2PlayingScratchLeftRotaryDownTimer = 0.0;
				Player2PlayingScratchRightRotaryDownTimer = 0.0;
				if (previousPlayingScratchRotaryStates[PlayingKeys.Player2ScratchLeftRotary])
				{
					Player2PlayingScratchLeftRotaryUpTimer = updateTime;
					Player2PlayingScratchRightRotaryUpTimer = 0.0;
				}
				else if (previousPlayingScratchRotaryStates[PlayingKeys.Player2ScratchRightRotary])
				{
					Player2PlayingScratchLeftRotaryUpTimer = 0.0;
					Player2PlayingScratchRightRotaryUpTimer = updateTime;
				}
			}
			else
			{
				currentPlayingScratchRotaryStates[PlayingKeys.Player2ScratchLeftRotary] = previousPlayingScratchRotaryStates[PlayingKeys.Player2ScratchLeftRotary];
				currentPlayingScratchRotaryStates[PlayingKeys.Player2ScratchRightRotary] = previousPlayingScratchRotaryStates[PlayingKeys.Player2ScratchRightRotary];
			}
		}
		else if (Player2ScratchRotaryValue < 0)
		{
			if (!previousPlayingScratchRotaryStates[PlayingKeys.Player2ScratchLeftRotary])
			{
				if (Player2ScratchRotaryValue * -1 > scratchRotaryThreshold)
				{
					currentPlayingScratchRotaryStates[PlayingKeys.Player2ScratchLeftRotary] = true;
					currentPlayingScratchRotaryStates[PlayingKeys.Player2ScratchRightRotary] = false;
					Player2PlayingScratchLeftRotaryDownTimer = updateTime;
					Player2PlayingScratchLeftRotaryDownAltTimer = updateTime;
					Player2PlayingScratchLeftRotaryUpTimer = 0.0;
					Player2PlayingScratchRightRotaryDownTimer = 0.0;
					Player2PlayingScratchRightRotaryDownAltTimer = 0.0;
					if (previousPlayingScratchRotaryStates[PlayingKeys.Player2ScratchRightRotary])
					{
						Player2PlayingScratchRightRotaryUpTimer = updateTime;
					}
					else
					{
						Player2PlayingScratchRightRotaryUpTimer = 0.0;
					}
				}
				else
				{
					currentPlayingScratchRotaryStates[PlayingKeys.Player2ScratchLeftRotary] = previousPlayingScratchRotaryStates[PlayingKeys.Player2ScratchLeftRotary];
					currentPlayingScratchRotaryStates[PlayingKeys.Player2ScratchRightRotary] = previousPlayingScratchRotaryStates[PlayingKeys.Player2ScratchRightRotary];
				}
			}
			else
			{
				currentPlayingScratchRotaryStates[PlayingKeys.Player2ScratchLeftRotary] = true;
				currentPlayingScratchRotaryStates[PlayingKeys.Player2ScratchRightRotary] = false;
			}
		}
		else
		{
			if (Player2ScratchRotaryValue <= 0)
			{
				return;
			}
			if (!previousPlayingScratchRotaryStates[PlayingKeys.Player2ScratchRightRotary])
			{
				if (Player2ScratchRotaryValue > scratchRotaryThreshold)
				{
					currentPlayingScratchRotaryStates[PlayingKeys.Player2ScratchLeftRotary] = false;
					currentPlayingScratchRotaryStates[PlayingKeys.Player2ScratchRightRotary] = true;
					Player2PlayingScratchLeftRotaryDownTimer = 0.0;
					Player2PlayingScratchLeftRotaryDownAltTimer = 0.0;
					Player2PlayingScratchRightRotaryDownTimer = updateTime;
					Player2PlayingScratchRightRotaryDownAltTimer = updateTime;
					Player2PlayingScratchRightRotaryUpTimer = 0.0;
					if (previousPlayingScratchRotaryStates[PlayingKeys.Player2ScratchRightRotary])
					{
						Player2PlayingScratchLeftRotaryUpTimer = updateTime;
					}
					else
					{
						Player2PlayingScratchLeftRotaryUpTimer = 0.0;
					}
				}
				else
				{
					currentPlayingScratchRotaryStates[PlayingKeys.Player2ScratchLeftRotary] = previousPlayingScratchRotaryStates[PlayingKeys.Player2ScratchLeftRotary];
					currentPlayingScratchRotaryStates[PlayingKeys.Player2ScratchRightRotary] = previousPlayingScratchRotaryStates[PlayingKeys.Player2ScratchRightRotary];
				}
			}
			else
			{
				currentPlayingScratchRotaryStates[PlayingKeys.Player2ScratchLeftRotary] = false;
				currentPlayingScratchRotaryStates[PlayingKeys.Player2ScratchRightRotary] = true;
			}
		}
	}

	private static void CheckState(PlayingKeys key)
	{
		if (ScratchRotaryInput.Contains(key))
		{
			if (KeyConfig != null && KeyConfig[key] != null)
			{
				nowStates[key].IsKeyDown = KeyConfig[key].IsKeyDownOnce();
				nowStates[key].IsKeyUp = !KeyConfig[key].IsKeyDownOnce();
			}
			else
			{
				nowStates[key].IsKeyDown = false;
				nowStates[key].IsKeyUp = true;
			}
		}
		else
		{
			nowStates[key].IsKeyDown = stateTime[key].KeyDownTime > stateTime[key].KeyUpTime;
			nowStates[key].IsKeyUp = !nowStates[key].IsKeyDown;
		}
	}

	public static void Update(double time)
	{
		if (stateTime == null)
		{
			Init();
		}
		updateTime = time;
		foreach (PlayingKeys value in Enum.GetValues(typeof(PlayingKeys)))
		{
			if (KeyConfig != null && KeyConfig[value] != null)
			{
				if (ScratchRotaryInput.Contains(value))
				{
					if (player1LastScratchRotaryUpdateTime == 0.0)
					{
						player1LastScratchRotaryUpdateTime = time;
					}
					if (player2LastScratchRotaryUpdateTime == 0.0)
					{
						player2LastScratchRotaryUpdateTime = time;
					}
					if (!KeyConfig[value].IsKeyDownOnce())
					{
						continue;
					}
					stateTime[value].KeyDownTime = updateTime;
					stateTime[value].KeyUpTime = updateTime;
					switch (value)
					{
					case PlayingKeys.Player1ScratchLeftRotary:
						if (time - player1LastScratchRotaryUpdateTime >= scratchRotaryUpdateInterval && player1ScratchRotaryCount == 0 && Player1ScratchRotaryValue == 0)
						{
							player1LastScratchRotaryUpdateTime = time;
						}
						player1ScratchRotaryCount--;
						if (player1ScratchRotaryCount == 0)
						{
							player1ScratchRotaryCount--;
						}
						break;
					case PlayingKeys.Player2ScratchLeftRotary:
						if (time - player2LastScratchRotaryUpdateTime >= scratchRotaryUpdateInterval && player2ScratchRotaryCount == 0 && Player2ScratchRotaryValue == 0)
						{
							player2LastScratchRotaryUpdateTime = time;
						}
						player2ScratchRotaryCount--;
						if (player2ScratchRotaryCount == 0)
						{
							player2ScratchRotaryCount--;
						}
						break;
					case PlayingKeys.Player1ScratchRightRotary:
						if (time - player1LastScratchRotaryUpdateTime >= scratchRotaryUpdateInterval && player1ScratchRotaryCount == 0 && Player1ScratchRotaryValue == 0)
						{
							player1LastScratchRotaryUpdateTime = time;
						}
						player1ScratchRotaryCount++;
						if (player1ScratchRotaryCount == 0)
						{
							player1ScratchRotaryCount++;
						}
						break;
					case PlayingKeys.Player2ScratchRightRotary:
						if (time - player2LastScratchRotaryUpdateTime >= scratchRotaryUpdateInterval && player2ScratchRotaryCount == 0 && Player2ScratchRotaryValue == 0)
						{
							player2LastScratchRotaryUpdateTime = time;
						}
						player2ScratchRotaryCount++;
						if (player2ScratchRotaryCount == 0)
						{
							player2ScratchRotaryCount++;
						}
						break;
					}
				}
				else
				{
					if (KeyConfig[value].IsKeyDownOnce() && stateTime[value].KeyDownTime <= stateTime[value].KeyUpTime && updateTime - stateTime[value].KeyUpTime > MinKeyUpTime)
					{
						stateTime[value].KeyDownTime = updateTime;
					}
					if (!KeyConfig[value].IsKeyDown() && stateTime[value].KeyDownTime >= stateTime[value].KeyUpTime && updateTime - stateTime[value].KeyDownTime > MinKeyDownTime)
					{
						stateTime[value].KeyUpTime = updateTime;
					}
				}
			}
			else
			{
				stateTime[value].KeyDownTime = 0.0;
				stateTime[value].KeyUpTime = 0.0;
			}
		}
		if (player1LastScratchRotaryUpdateTime != 0.0 && (player1ScratchRotaryCount != 0 || Player1ScratchRotaryValue != 0) && time - player1LastScratchRotaryUpdateTime >= scratchRotaryUpdateInterval)
		{
			int num = player1ScratchRotaryCount * 2 / scratchRotaryThreshold;
			if (num == 0)
			{
				if (Player1ScratchRotaryValue != 0 && time - player1LastScratchRotaryActiveStartTime > scratchRotaryActiveTime)
				{
					Player1ScratchRotaryValue = num;
				}
			}
			else
			{
				player1LastScratchRotaryActiveStartTime = time;
				Player1ScratchRotaryValue = num;
				player1LastScratchRotaryUpdateTime = time;
			}
			player1ScratchRotaryCount = 0;
		}
		if (player2LastScratchRotaryUpdateTime != 0.0 && (player2ScratchRotaryCount != 0 || Player2ScratchRotaryValue != 0) && time - player2LastScratchRotaryUpdateTime >= scratchRotaryUpdateInterval)
		{
			int num2 = player2ScratchRotaryCount * 2 / scratchRotaryThreshold;
			if (num2 == 0)
			{
				if (Player2ScratchRotaryValue != 0 && time - player2LastScratchRotaryActiveStartTime > scratchRotaryActiveTime)
				{
					Player2ScratchRotaryValue = num2;
				}
			}
			else
			{
				player2LastScratchRotaryActiveStartTime = time;
				Player2ScratchRotaryValue = num2;
				player2LastScratchRotaryUpdateTime = time;
			}
			player2ScratchRotaryCount = 0;
		}
		GetState();
		if (IsKeyUpOnce(PlayingKeys.Player1ScratchToggle))
		{
			Player1ScratchToggleDirection = ((Player1ScratchToggleDirection != ScratchDirection.Right) ? ScratchDirection.Right : ScratchDirection.Left);
		}
		if (IsKeyUpOnce(PlayingKeys.Player2ScratchToggle))
		{
			Player2ScratchToggleDirection = ((Player2ScratchToggleDirection != ScratchDirection.Right) ? ScratchDirection.Right : ScratchDirection.Left);
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
