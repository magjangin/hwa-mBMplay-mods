using System;
using System.Collections.Generic;

namespace BeMusic;

public class BmsRow
{
	public List<BmsRowObjectCollection> Lanes { get; private set; }

	public Dictionary<BmsChannelType, BmsRowObjectCollection> Channels { get; private set; }

	public int LaneObjectCount
	{
		get
		{
			int num = 0;
			foreach (BmsRowObjectCollection lane in Lanes)
			{
				num += lane.Count;
			}
			return num;
		}
	}

	public double Position { get; private set; }

	public BmsRow(int laneCount, double position)
	{
		Position = position;
		Lanes = new List<BmsRowObjectCollection>(laneCount);
		for (int i = 0; i < Lanes.Capacity; i++)
		{
			Lanes.Add(new BmsRowObjectCollection(position));
		}
		Channels = new Dictionary<BmsChannelType, BmsRowObjectCollection>();
		foreach (BmsChannelType value in Enum.GetValues(typeof(BmsChannelType)))
		{
			Channels.Add(value, new BmsRowObjectCollection(position));
		}
	}
}
