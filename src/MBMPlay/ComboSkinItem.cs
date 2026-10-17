using System;

namespace MBMPlay;

public sealed class ComboSkinItem : NumberSkinItemBase
{
	private WeakReference<JudgeSkinItem> judgeRef;

	public int JudgeID { get; set; }

	public JudgeSkinItem JudgeItem
	{
		get
		{
			if (judgeRef == null)
			{
				return null;
			}
			if (!judgeRef.TryGetTarget(out var target))
			{
				return null;
			}
			return target;
		}
		internal set
		{
			if (value == null)
			{
				judgeRef = null;
			}
			else
			{
				judgeRef = new WeakReference<JudgeSkinItem>(value);
			}
		}
	}

	public ComboSkinItem(Skin skin)
		: base(skin)
	{
	}

	public void FindPair(Skin skin)
	{
		for (int i = 0; i < skin.Items.Count && !CheckPair(skin.Items[i]); i++)
		{
		}
	}

	private bool CheckPair(ISkinItem item)
	{
		if (item is ISkinItemGroup skinItemGroup)
		{
			for (int i = 0; i < skinItemGroup.Items.Count; i++)
			{
				if (CheckPair(skinItemGroup.Items[i]))
				{
					return true;
				}
			}
			return false;
		}
		if (item is JudgeSkinItem judgeSkinItem && judgeSkinItem.JudgeID == JudgeID)
		{
			JudgeItem = judgeSkinItem;
			if (judgeSkinItem.ComboItem == null)
			{
				judgeSkinItem.ComboItem = this;
			}
			return true;
		}
		return false;
	}
}
