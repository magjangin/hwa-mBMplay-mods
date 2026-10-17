using System.Collections.Generic;
using MBMPlay.Common;
using MistUtils;

namespace MBMPlay;

public class ScratchTimer
{
	private static readonly Dictionary<byte, List<PlayingKeys>> ScratchList = new Dictionary<byte, List<PlayingKeys>>
	{
		{
			1,
			new List<PlayingKeys>
			{
				PlayingKeys.Player1ScratchLeft,
				PlayingKeys.Player1ScratchRight,
				PlayingKeys.Player1ScratchToggle
			}
		},
		{
			2,
			new List<PlayingKeys>
			{
				PlayingKeys.Player2ScratchLeft,
				PlayingKeys.Player2ScratchRight,
				PlayingKeys.Player2ScratchToggle
			}
		}
	};

	private byte player = 1;

	private double baseTimer;

	private double nowTime;

	private double prevTime;

	private double rotate = -100000.0;

	public byte Player
	{
		get
		{
			return player;
		}
		set
		{
			if (value == 1 || value == 2)
			{
				player = value;
			}
		}
	}

	public double Speed { get; set; } = 1000.0;

	public double Value => baseTimer + rotate;

	public ScratchTimer(byte player)
	{
		Player = player;
	}

	public void Update(double timer, double now)
	{
		baseTimer = timer;
		if (!(baseTimer > 0.0) || !(nowTime < now))
		{
			return;
		}
		prevTime = (Extensions.EpsilonEquals(prevTime, 0.0) ? now : nowTime);
		nowTime = now;
		double num = (nowTime - prevTime) * (Speed / 1000.0);
		int num2 = ((Player == 2) ? PlayingKeyState.Player2ScratchRotaryValue : PlayingKeyState.Player1ScratchRotaryValue);
		if (!Flags.AutoPlay && num2 != 0)
		{
			rotate += num / 2.0 * (double)(num2 + ((num2 < 0) ? (-2) : 0)) * -1.0;
		}
		else if (Flags.IsKeyDown(ScratchList[Player][0]))
		{
			rotate += num * 3.0;
		}
		else if (Flags.IsKeyDown(ScratchList[Player][1]))
		{
			rotate -= num;
		}
		else if (Flags.IsKeyDown(ScratchList[Player][2]))
		{
			if (((Player == 2) ? Flags.Player2ScratchToggleDirection : Flags.Player1ScratchToggleDirection) == ScratchDirection.Right)
			{
				rotate -= num;
			}
			else
			{
				rotate += num * 3.0;
			}
		}
		if (rotate >= 0.0)
		{
			rotate -= 100000.0;
		}
		else if (rotate <= -1000000.0)
		{
			rotate += 100000.0;
		}
	}

	public void Reset()
	{
		baseTimer = 0.0;
		nowTime = 0.0;
		prevTime = 0.0;
		rotate = -100000.0;
	}
}
