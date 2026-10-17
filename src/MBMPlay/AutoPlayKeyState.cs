using System;
using System.Collections.Generic;
using MBMPlay.Common;

namespace MBMPlay;

public static class AutoPlayKeyState
{
	private static Dictionary<PlayingKeys, PlayingKeyStateFlag> nowStates;

	private static Dictionary<PlayingKeys, PlayingKeyStateFlag> prevStates;

	private static Dictionary<PlayingKeys, PlayingKeyStateTime> stateTime;

	private static HashSet<PlayingKeys> KeyDownRequest = new HashSet<PlayingKeys>();

	private static HashSet<PlayingKeys> KeyUpRequest = new HashSet<PlayingKeys>();

	private static double updateTime = 0.0;

	public static double MinKeyDownTime { get; set; }

	public static double MinKeyUpTime { get; set; }

	public static ScratchDirection Player1ScratchToggleDirection { get; private set; } = ScratchDirection.Right;

	public static ScratchDirection Player2ScratchToggleDirection { get; private set; } = ScratchDirection.Right;

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
	}

	public static void GetState()
	{
		foreach (PlayingKeys value in Enum.GetValues(typeof(PlayingKeys)))
		{
			prevStates[value] = new PlayingKeyStateFlag(nowStates[value].IsKeyDown, nowStates[value].IsKeyUp);
			CheckState(value);
		}
	}

	private static void CheckState(PlayingKeys key)
	{
		nowStates[key].IsKeyDown = stateTime[key].KeyDownTime > stateTime[key].KeyUpTime;
		nowStates[key].IsKeyUp = !nowStates[key].IsKeyDown;
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
			if (KeyUpRequest.Contains(value) && updateTime - stateTime[value].KeyDownTime > MinKeyDownTime)
			{
				stateTime[value].KeyUpTime = updateTime;
				KeyUpRequest.Remove(value);
			}
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

	public static void ResetKeyState()
	{
		foreach (PlayingKeys value in Enum.GetValues(typeof(PlayingKeys)))
		{
			stateTime[value].KeyDownTime = 0.0;
			stateTime[value].KeyUpTime = 0.0;
			nowStates[value].IsKeyDown = false;
			nowStates[value].IsKeyUp = true;
			prevStates[value].IsKeyDown = false;
			prevStates[value].IsKeyUp = true;
		}
		KeyDownRequest.Clear();
		KeyUpRequest.Clear();
	}
}
