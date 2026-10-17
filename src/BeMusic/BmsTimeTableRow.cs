using System;
using System.Collections.Generic;

namespace BeMusic;

public class BmsTimeTableRow
{
	public BmsTime Barline { get; set; }

	public List<BmsLaneTimeRowCollection> Lanes { get; set; }

	public Dictionary<BmsChannelType, BmsTimeRowCollection> Channels { get; set; }

	public int LaneObjectCount
	{
		get
		{
			int num = 0;
			foreach (BmsLaneTimeRowCollection lane in Lanes)
			{
				num += lane.Count;
			}
			return num;
		}
	}

	public int Count
	{
		get
		{
			int num = LaneObjectCount + ((Barline != null) ? 1 : 0);
			foreach (KeyValuePair<BmsChannelType, BmsTimeRowCollection> channel in Channels)
			{
				num += channel.Value.Count;
			}
			return num;
		}
	}

	public double Time { get; private set; }

	public BmsTimeTableRow(int laneCount, double time)
	{
		Time = time;
		Lanes = new List<BmsLaneTimeRowCollection>(laneCount);
		for (int i = 0; i < Lanes.Capacity; i++)
		{
			Lanes.Add(new BmsLaneTimeRowCollection());
		}
		Channels = new Dictionary<BmsChannelType, BmsTimeRowCollection>();
		foreach (BmsChannelType value in Enum.GetValues(typeof(BmsChannelType)))
		{
			Channels.Add(value, new BmsTimeRowCollection());
		}
	}

	public BmsTimeTableRow(BmsTimeTableRow row, double musicSpeed = 1.0)
	{
		Time = row.Time * (1.0 / musicSpeed);
		Lanes = new List<BmsLaneTimeRowCollection>(row.Lanes.Count);
		for (int i = 0; i < Lanes.Capacity; i++)
		{
			Lanes.Add(new BmsLaneTimeRowCollection());
			foreach (BmsLaneTime item in row.Lanes[i])
			{
				Lanes[i].Add(new BmsLaneTime(item, musicSpeed));
			}
		}
		Channels = new Dictionary<BmsChannelType, BmsTimeRowCollection>();
		foreach (BmsChannelType value in Enum.GetValues(typeof(BmsChannelType)))
		{
			Channels.Add(value, new BmsTimeRowCollection());
			foreach (BmsTime item2 in row.Channels[value])
			{
				Channels[value].Add(new BmsTime(item2, musicSpeed));
			}
		}
		if (row.Barline != null)
		{
			Barline = new BmsTime(row.Barline, musicSpeed);
		}
	}

	public BmsTimeTableRow(BmsTimeTableRow row, IDictionary<int, int> swapDictionary)
	{
		Time = row.Time;
		Lanes = new List<BmsLaneTimeRowCollection>(row.Lanes.Count);
		for (int i = 0; i < Lanes.Capacity; i++)
		{
			Lanes.Add(new BmsLaneTimeRowCollection());
		}
		for (int j = 0; j < Lanes.Capacity; j++)
		{
			foreach (BmsLaneTime item in row.Lanes[j])
			{
				if (swapDictionary.ContainsKey(j))
				{
					Lanes[swapDictionary[j]].Add(new BmsLaneTime(item));
				}
				else
				{
					Lanes[j].Add(new BmsLaneTime(item));
				}
			}
		}
		Channels = new Dictionary<BmsChannelType, BmsTimeRowCollection>();
		foreach (BmsChannelType value in Enum.GetValues(typeof(BmsChannelType)))
		{
			Channels.Add(value, new BmsTimeRowCollection());
			foreach (BmsTime item2 in row.Channels[value])
			{
				Channels[value].Add(new BmsTime(item2));
			}
		}
		if (row.Barline != null)
		{
			Barline = new BmsTime(row.Barline);
		}
	}
}
