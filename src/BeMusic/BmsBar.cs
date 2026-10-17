using System;
using System.Collections.Generic;
using System.Linq;
using BeMusic.Extentions;

namespace BeMusic;

public class BmsBar
{
	private Dictionary<BmsChannelType, BmsObjectCollection> _channels;

	public double BarLength { get; set; }

	public BmsObjectCollection Bpm => _channels[BmsChannelType.Bpm];

	public BmsObjectCollection Stop => _channels[BmsChannelType.Stop];

	public List<BmsObjectCollection> Lanes { get; private set; }

	public BmsObjectCollection Bgm => _channels[BmsChannelType.Bgm];

	public BmsObjectCollection Bga => _channels[BmsChannelType.Bga];

	public BmsObjectCollection Layer => _channels[BmsChannelType.Layer];

	public BmsObjectCollection Poor => _channels[BmsChannelType.Poor];

	public BmsRow this[int index] => GetBmsRow(index);

	public BmsRow this[double position] => GetBmsRow(position);

	public SortedSet<double> PositionIndex { get; private set; }

	public BmsBar(int keys, double barlength)
	{
		BarLength = barlength;
		PositionIndex = new SortedSet<double>();
		if (keys >= 10)
		{
			Lanes = new List<BmsObjectCollection>(keys / 2 + 11);
		}
		else
		{
			Lanes = new List<BmsObjectCollection>(keys + 1);
		}
		for (int i = 0; i < Lanes.Capacity; i++)
		{
			Lanes.Add(new BmsObjectCollection());
		}
		_channels = new Dictionary<BmsChannelType, BmsObjectCollection>();
		foreach (BmsChannelType value in Enum.GetValues(typeof(BmsChannelType)))
		{
			_channels.Add(value, new BmsObjectCollection());
		}
	}

	public void AddBmsObject(int lane, BmsObject obj)
	{
		if (obj.ObjectType != BmsObjectType.InvisibleNote && Lanes[lane].Count > 0)
		{
			for (int i = 0; i < Lanes[lane].Count; i++)
			{
				if (!Lanes[lane][i].Position.EpsilonEquals(obj.Position))
				{
					continue;
				}
				if (obj.ObjectType == BmsObjectType.LNStart)
				{
					if (Lanes[lane][i].ObjectType != BmsObjectType.InvisibleNote)
					{
						Lanes[lane].RemoveAt(i);
						i--;
					}
				}
				else if (obj.ObjectType == BmsObjectType.Note)
				{
					if (Lanes[lane][i].ObjectType == BmsObjectType.LNStart)
					{
						return;
					}
					if (Lanes[lane][i].ObjectType == BmsObjectType.Note)
					{
						Lanes[lane].RemoveAt(i);
						i--;
					}
				}
			}
		}
		Lanes[lane].Add(obj);
		PositionIndex.Add(obj.Position);
	}

	public void AddBmsObject(BmsChannelType type, BmsObject obj)
	{
		_channels[type].Add(obj);
		PositionIndex.Add(obj.Position);
	}

	public BmsRow GetBmsRow(int index)
	{
		if (index >= 0 && index < PositionIndex.Count())
		{
			return GetBmsRow(PositionIndex.ElementAt(index));
		}
		return null;
	}

	public BmsRow GetBmsRow(double position)
	{
		bool flag = false;
		BmsRow bmsRow = new BmsRow(Lanes.Count, position);
		for (int i = 0; i < Lanes.Count; i++)
		{
			foreach (BmsObject item in Lanes[i])
			{
				if (item.Position.EpsilonEquals(position))
				{
					bmsRow.Lanes[i].Add(item);
					flag = true;
				}
			}
		}
		foreach (BmsChannelType value in Enum.GetValues(typeof(BmsChannelType)))
		{
			foreach (BmsObject item2 in _channels[value])
			{
				if (item2.Position.EpsilonEquals(position))
				{
					bmsRow.Channels[value].Add(item2);
					flag = true;
				}
			}
		}
		if (flag)
		{
			return bmsRow;
		}
		return null;
	}
}
