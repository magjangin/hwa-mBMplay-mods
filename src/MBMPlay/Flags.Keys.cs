using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BeMusic;
using MBMPlay.Common;
using MBMPlay.Platform;
using MistUtils;

namespace MBMPlay;

public static partial class Flags
{
	public static bool IsKeyDown(PlayingKeys key)
	{
		if (!AutoPlay)
		{
			return PlayingKeyState.IsKeyDown(key);
		}
		return AutoPlayKeyState.IsKeyDown(key);
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
		if (!AutoPlay)
		{
			return PlayingKeyState.IsKeyDownOnce(key);
		}
		return AutoPlayKeyState.IsKeyDownOnce(key);
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
		if (!AutoPlay)
		{
			return PlayingKeyState.IsKeyUp(key);
		}
		return AutoPlayKeyState.IsKeyUp(key);
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
		if (!AutoPlay)
		{
			return PlayingKeyState.IsKeyUpOnce(key);
		}
		return AutoPlayKeyState.IsKeyUpOnce(key);
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
}
