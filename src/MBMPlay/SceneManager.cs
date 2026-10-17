using System.Collections.Generic;
using MistUtils;

namespace MBMPlay;

public static class SceneManager
{
	private static List<Scene> sceneList = new List<Scene>();

	public static void Add(Scene scene)
	{
		sceneList.Add(scene);
		scene.LoadContent();
	}

	public static void Remove(Scene scene)
	{
		if (sceneList.Contains(scene))
		{
			sceneList.Remove(scene);
			scene.Dispose();
		}
	}

	public static void Clear()
	{
		Utils.Free(sceneList);
	}

	public static void Update()
	{
		bool flag = false;
		for (int num = sceneList.Count - 1; num >= 0; num--)
		{
			if (sceneList[num].IsActive && !flag)
			{
				sceneList[num].EnableInput = true;
				flag = true;
			}
			else
			{
				sceneList[num].EnableInput = false;
			}
		}
		foreach (Scene item in new List<Scene>(sceneList))
		{
			item.Update();
		}
	}

	public static void Draw()
	{
		foreach (Scene item in new List<Scene>(sceneList))
		{
			item.Draw();
		}
	}
}
