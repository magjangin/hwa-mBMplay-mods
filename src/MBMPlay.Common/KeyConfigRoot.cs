using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using MistUtils;

namespace MBMPlay.Common;

[XmlRoot("KeyConfig")]
public class KeyConfigRoot
{
	public static readonly Version KeyConfigVersion = new Version(3, 24, 824, 0);

	public string KeyConfigFileVersion { get; set; }

	public bool UpgradeRequired
	{
		get
		{
			if (!Version.TryParse(KeyConfigFileVersion, out var result))
			{
				return true;
			}
			return KeyConfigVersion > result;
		}
	}

	public KeyConfigPlayStyles PlayingKeys { get; set; }

	public GeneralKeyConfig General { get; set; }

	public PreloadSceneKeyConfig PreloadScene { get; set; }

	public PlaySceneKeyConfig PlayScene { get; set; }

	public static KeyConfigRoot Default => new KeyConfigRoot
	{
		KeyConfigFileVersion = Utils.GetVersionString(KeyConfigVersion),
		PlayingKeys = KeyConfigPlayStyles.Default,
		General = GeneralKeyConfig.Default,
		PreloadScene = PreloadSceneKeyConfig.Default,
		PlayScene = PlaySceneKeyConfig.Default
	};

	public KeyConfigRoot()
	{
	}

	public KeyConfigRoot(KeyConfigRoot keyConfig)
	{
		KeyConfigFileVersion = keyConfig.KeyConfigFileVersion;
		PlayingKeys = new KeyConfigPlayStyles(keyConfig.PlayingKeys);
		General = new GeneralKeyConfig(keyConfig.General);
		PreloadScene = new PreloadSceneKeyConfig(keyConfig.PreloadScene);
		PlayScene = new PlaySceneKeyConfig(keyConfig.PlayScene);
	}

	public void Upgrade()
	{
		if (!Version.TryParse(KeyConfigFileVersion, out var result))
		{
			return;
		}
		KeyConfigRoot _default = Default;
		if (result < new Version(2, 21, 1024, 0))
		{
			List<Func<InputCollection>[]> list = new List<Func<InputCollection>[]>();
			list.Add(new Func<InputCollection>[2]
			{
				() => PlayScene.BackBar,
				() => _default.PlayScene.BackBar
			});
			list.Add(new Func<InputCollection>[2]
			{
				() => PlayScene.BackBar4,
				() => _default.PlayScene.BackBar4
			});
			list.Add(new Func<InputCollection>[2]
			{
				() => PlayScene.BackBarTime,
				() => _default.PlayScene.BackBarTime
			});
			list.Add(new Func<InputCollection>[2]
			{
				() => PlayScene.ForwardBar,
				() => _default.PlayScene.ForwardBar
			});
			list.Add(new Func<InputCollection>[2]
			{
				() => PlayScene.ForwardBar4,
				() => _default.PlayScene.ForwardBar4
			});
			list.Add(new Func<InputCollection>[2]
			{
				() => PlayScene.ForwardBarTime,
				() => _default.PlayScene.ForwardBarTime
			});
			list.Add(new Func<InputCollection>[2]
			{
				() => PlayScene.BackToStart,
				() => _default.PlayScene.BackToStart
			});
			list.Add(new Func<InputCollection>[2]
			{
				() => PlayScene.BackToStartInTestPlay,
				() => _default.PlayScene.BackToStartInTestPlay
			});
			List<Func<InputCollection>[]> list2 = list;
			for (int num = 0; num < list2.Count; num++)
			{
				InputCollection inputCollection = list2[num][0]();
				InputCollection inputCollection2 = list2[num][1]();
				bool flag = false;
				for (int num2 = 0; num2 < inputCollection.Count; num2++)
				{
					if (inputCollection[num2] is MouseButtonInput || inputCollection[num2] is MultiDevicesInput)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					continue;
				}
				int num3 = -1;
				for (int num4 = 1; num4 < inputCollection2.Count; num4++)
				{
					if (inputCollection2[num4] is MouseButtonInput || inputCollection2[num4] is MultiDevicesInput)
					{
						num3 = num4;
						break;
					}
				}
				if (num3 >= 0)
				{
					inputCollection.Add(inputCollection2[num3]);
				}
			}
		}
		if (result < new Version(3, 23, 821, 0))
		{
			General.ScreenShot = _default.General.ScreenShot;
			PlayScene.JudgeTimingUp = _default.PlayScene.JudgeTimingUp;
			PlayScene.JudgeTimingUp10 = _default.PlayScene.JudgeTimingUp10;
			PlayScene.JudgeTimingDown = _default.PlayScene.JudgeTimingDown;
			PlayScene.JudgeTimingDown10 = _default.PlayScene.JudgeTimingDown10;
			PlayScene.JudgeTimingReset = _default.PlayScene.JudgeTimingReset;
		}
		if (result < KeyConfigVersion)
		{
			PlayScene.Random = _default.PlayScene.Random;
			PlayScene.SRandom = _default.PlayScene.SRandom;
			PlayScene.Mirror = _default.PlayScene.Mirror;
			PlayScene.Flip = _default.PlayScene.Flip;
			PlayScene.ClearRandomMirror = _default.PlayScene.ClearRandomMirror;
			PlayScene.Rerandomize = _default.PlayScene.Rerandomize;
			PlayScene.LiftUp = _default.PlayScene.LiftUp;
			PlayScene.LiftUp10 = _default.PlayScene.LiftUp10;
			PlayScene.LiftUp01 = _default.PlayScene.LiftUp01;
			PlayScene.LiftUp005 = _default.PlayScene.LiftUp005;
			PlayScene.LiftDown = _default.PlayScene.LiftDown;
			PlayScene.LiftDown10 = _default.PlayScene.LiftDown10;
			PlayScene.LiftDown01 = _default.PlayScene.LiftDown01;
			PlayScene.LiftDown005 = _default.PlayScene.LiftDown005;
		}
		KeyConfigFileVersion = Utils.GetVersionString(KeyConfigVersion);
	}
}
