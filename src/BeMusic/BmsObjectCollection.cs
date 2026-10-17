using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BeMusic;

public class BmsObjectCollection : Collection<BmsObject>
{
	public void Sort(Comparison<BmsObject> comparison)
	{
		if (Items is List<BmsObject> list)
		{
			list.Sort(comparison);
			return;
		}
		List<BmsObject> list2 = Items.OrderBy((BmsObject item) => item, Comparer<BmsObject>.Create(comparison)).ToList();
		Items.Clear();
		foreach (BmsObject item in list2)
		{
			Items.Add(item);
		}
	}
}
