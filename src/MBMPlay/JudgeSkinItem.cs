using System;

namespace MBMPlay;

public sealed class JudgeSkinItem : ImageSkinItemBase
{
	private WeakReference<ComboSkinItem> comboRef;

	public int JudgeID { get; set; }

	public ComboSkinItem ComboItem
	{
		get
		{
			if (comboRef == null)
			{
				return null;
			}
			if (!comboRef.TryGetTarget(out var target))
			{
				return null;
			}
			return target;
		}
		internal set
		{
			if (value == null)
			{
				comboRef = null;
			}
			else
			{
				comboRef = new WeakReference<ComboSkinItem>(value);
			}
		}
	}

	public JudgeSkinItem(Skin skin)
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
		if (item is ComboSkinItem comboSkinItem && comboSkinItem.JudgeID == JudgeID)
		{
			ComboItem = comboSkinItem;
			if (comboSkinItem.JudgeItem == null)
			{
				comboSkinItem.JudgeItem = this;
			}
			return true;
		}
		return false;
	}
}
