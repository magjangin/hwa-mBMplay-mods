using System;
using System.Collections.Generic;
using MBMPlay.Common;

namespace MBMPlay;

public static partial class PlayingKeyState
{
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
}
