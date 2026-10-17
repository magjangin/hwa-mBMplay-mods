using System;
using MBMPlay.Common;

namespace MBMPlay;

public class KeyRepeatManager : IInputDevice
{
	private IInputDevice input;

	private bool previousKeyDownState;

	private bool currentKeyDownState;

	private bool previousKeyRepeatState;

	private bool currentKeyRepeatState;

	private double keyDownTime;

	private double nextKeyRepeat;

	public double KeyRepeatStartTime { get; set; } = 500.0;

	public double KeyRepeatCycle { get; set; } = 100.0;

	public KeyRepeatManager(IInputDevice inputDevice)
	{
		input = inputDevice ?? throw new ArgumentNullException();
	}

	public KeyRepeatManager(IInputDevice inputDevice, double startTime, double cycle)
	{
		input = inputDevice ?? throw new ArgumentNullException();
		KeyRepeatStartTime = startTime;
		KeyRepeatCycle = cycle;
	}

	public void Update(double time)
	{
		previousKeyDownState = currentKeyDownState;
		currentKeyDownState = IsKeyDown();
		previousKeyRepeatState = currentKeyRepeatState;
		if (input.IsKeyDownOnce())
		{
			currentKeyRepeatState = true;
			keyDownTime = time;
			nextKeyRepeat = keyDownTime + KeyRepeatStartTime;
		}
		else if (input.IsKeyDown())
		{
			if (time >= nextKeyRepeat)
			{
				currentKeyRepeatState = true;
				nextKeyRepeat += KeyRepeatCycle;
			}
			else
			{
				currentKeyRepeatState = false;
			}
		}
		else
		{
			currentKeyRepeatState = false;
			keyDownTime = 0.0;
			nextKeyRepeat = 0.0;
		}
	}

	public bool IsKeyDown()
	{
		return input.IsKeyDown();
	}

	public bool IsKeyDownOnce()
	{
		return currentKeyRepeatState;
	}

	public bool IsKeyUp()
	{
		return input.IsKeyUp();
	}

	public bool IsKeyUpOnce()
	{
		if (!currentKeyRepeatState != previousKeyRepeatState)
		{
			return input.IsKeyUpOnce();
		}
		return true;
	}
}
