using System;
using System.Collections.Generic;
using System.Linq;
using BeMusic.Extentions;
using Randomize;

namespace BeMusic;

public partial class BmsTimeTable
{
	private class NotesTimeInfo
	{
		internal double Time { get; set; }

		internal List<int> Notes { get; set; }

		internal NotesTimeInfo(double time, int laneCount)
		{
			Time = time;
			Notes = new List<int>(laneCount);
		}
	}

	public List<BmsTimeTableRow> Rows { get; set; }

	public int LaneCount { get; private set; }

	public List<int> BarIndex { get; set; }

	public double InitialBpm { get; set; }

	public double MaxBpm { get; set; }

	public double MinBpm { get; set; }

	public double MaxNotesBpm { get; set; }

	public double MinNotesBpm { get; set; }

	public double MainNotesBpm { get; set; }

	public double MainTimeBpm { get; set; }

	protected BmsTimeTable()
	{
	}

	public BmsTimeTable(int laneCount)
	{
		LaneCount = laneCount;
		Rows = new List<BmsTimeTableRow>();
		BarIndex = new List<int>();
	}

	public BmsTimeTable(BmsTimeTable timeTable, double musicSpeed = 1.0)
	{
		LaneCount = timeTable.LaneCount;
		Rows = new List<BmsTimeTableRow>();
		for (int i = 0; i < timeTable.Rows.Count; i++)
		{
			Rows.Add(new BmsTimeTableRow(timeTable.Rows[i], musicSpeed));
		}
		BarIndex = new List<int>(timeTable.BarIndex);
		InitialBpm = timeTable.InitialBpm * musicSpeed;
		MaxBpm = timeTable.MaxBpm * musicSpeed;
		MinBpm = timeTable.MinBpm * musicSpeed;
		MaxNotesBpm = timeTable.MaxNotesBpm * musicSpeed;
		MinNotesBpm = timeTable.MinNotesBpm * musicSpeed;
		MainNotesBpm = timeTable.MainNotesBpm * musicSpeed;
		MainTimeBpm = timeTable.MainTimeBpm * musicSpeed;
	}

	public static BmsTimeTable CreateFromBmsData(Bms bms)
	{
		if (bms == null)
		{
			throw new ArgumentNullException("bms");
		}
		if (bms.Bars.Count <= 0)
		{
			return null;
		}
		BmsTimeTable bmsTimeTable = new BmsTimeTable(bms.Bars[0].Lanes.Count);
		double num = bms.StartBpm;
		double num2 = 0.0;
		double num3 = 0.0;
		double num4 = 0.0;
		BmsTimeTableRow bmsTimeTableRow = null;
		SortedDictionary<double, int> sortedDictionary = new SortedDictionary<double, int>();
		SortedDictionary<double, double> sortedDictionary2 = new SortedDictionary<double, double> { { num, 0.0 } };
		for (int i = 0; i < bms.Bars.Count; i++)
		{
			_ = bms.Bars.Count - 1;
			if (i == 0)
			{
				bmsTimeTableRow = new BmsTimeTableRow(bmsTimeTable.LaneCount, num2);
				bmsTimeTable.BarIndex.Add(bmsTimeTable.Rows.Count);
			}
			else if (i > 0)
			{
				if (bmsTimeTableRow != null)
				{
					bmsTimeTable.Rows.Add(bmsTimeTableRow);
				}
				num2 += 60000.0 / (num * 48.0) * (192.0 * bms.Bars[i - 1].BarLength - num4);
				bmsTimeTableRow = new BmsTimeTableRow(bmsTimeTable.LaneCount, num2)
				{
					Barline = new BmsTime(num2, "", BmsObjectType.Barline)
				};
				bmsTimeTable.BarIndex.Add(bmsTimeTable.Rows.Count);
				num4 = 0.0;
			}
			for (int j = 0; bms.Bars[i][j] != null; j++)
			{
				BmsRow bmsRow = bms.Bars[i][j];
				num2 += 60000.0 / (num * 48.0) * (bmsRow.Position - num4);
				if (bmsTimeTableRow == null || !bmsTimeTableRow.Time.EpsilonEquals(num2))
				{
					if (bmsTimeTableRow != null)
					{
						bmsTimeTable.Rows.Add(bmsTimeTableRow);
					}
					bmsTimeTableRow = new BmsTimeTableRow(bmsTimeTable.LaneCount, num2);
				}
				if (bmsRow.Channels[BmsChannelType.Bpm].Count > 0)
				{
					foreach (BmsObject item in bmsRow.Channels[BmsChannelType.Bpm])
					{
						double num5 = double.Parse(item.Value);
						if (!num5.EpsilonEquals(0.0))
						{
							if (sortedDictionary2.ContainsKey(num))
							{
								sortedDictionary2[num] += num2 - num3;
							}
							else
							{
								sortedDictionary2.Add(num, num2 - num3);
							}
							num3 = num2;
							if (num5 < 0.0 && !sortedDictionary2.ContainsKey(num5))
							{
								sortedDictionary2.Add(num5, 0.0);
							}
							num = Math.Abs(num5);
							bmsTimeTableRow.Channels[BmsChannelType.Bpm].Add(new BmsTime(num2, item.Value, item.ObjectType));
						}
					}
				}
				if (bmsRow.LaneObjectCount > 0)
				{
					for (int k = 0; k < bmsRow.Lanes.Count; k++)
					{
						foreach (BmsObject item2 in bmsRow.Lanes[k])
						{
							if (sortedDictionary.ContainsKey(num))
							{
								sortedDictionary[num]++;
							}
							else
							{
								sortedDictionary.Add(num, 1);
							}
							bmsTimeTableRow.Lanes[k].Add(new BmsLaneTime(num2, item2.Value, item2.ObjectType));
						}
					}
				}
				if (bmsRow.Channels[BmsChannelType.Bgm].Count > 0)
				{
					foreach (BmsObject item3 in bmsRow.Channels[BmsChannelType.Bgm])
					{
						bmsTimeTableRow.Channels[BmsChannelType.Bgm].Add(new BmsTime(num2, item3.Value, item3.ObjectType));
					}
				}
				if (bmsRow.Channels[BmsChannelType.Bga].Count > 0)
				{
					foreach (BmsObject item4 in bmsRow.Channels[BmsChannelType.Bga])
					{
						bmsTimeTableRow.Channels[BmsChannelType.Bga].Add(new BmsTime(num2, item4.Value, item4.ObjectType));
					}
				}
				if (bmsRow.Channels[BmsChannelType.Layer].Count > 0)
				{
					foreach (BmsObject item5 in bmsRow.Channels[BmsChannelType.Layer])
					{
						bmsTimeTableRow.Channels[BmsChannelType.Layer].Add(new BmsTime(num2, item5.Value, item5.ObjectType));
					}
				}
				if (bmsRow.Channels[BmsChannelType.Poor].Count > 0)
				{
					foreach (BmsObject item6 in bmsRow.Channels[BmsChannelType.Poor])
					{
						bmsTimeTableRow.Channels[BmsChannelType.Poor].Add(new BmsTime(num2, item6.Value, item6.ObjectType));
					}
				}
				if (bmsRow.Channels[BmsChannelType.Stop].Count > 0)
				{
					double num6 = 0.0;
					foreach (BmsObject item7 in bmsRow.Channels[BmsChannelType.Stop])
					{
						bmsTimeTableRow.Channels[BmsChannelType.Stop].Add(new BmsTime(num2, item7.Value, item7.ObjectType));
						num6 += 60000.0 / (num * 48.0) * double.Parse(item7.Value);
					}
					num2 += num6;
				}
				num4 = bmsRow.Position;
				bmsTimeTable.Rows.Add(bmsTimeTableRow);
				bmsTimeTableRow = null;
			}
		}
		if (bmsTimeTableRow != null)
		{
			bmsTimeTable.Rows.Add(bmsTimeTableRow);
		}
		if (sortedDictionary2.ContainsKey(num))
		{
			sortedDictionary2[num] += num2 - num3;
		}
		else
		{
			sortedDictionary2.Add(num, num2 - num3);
		}
		bmsTimeTable.MinBpm = sortedDictionary2.First().Key;
		bmsTimeTable.MaxBpm = sortedDictionary2.Last().Key;
		bmsTimeTable.MinNotesBpm = ((sortedDictionary.Count > 0) ? sortedDictionary.First().Key : bmsTimeTable.MinBpm);
		bmsTimeTable.MaxNotesBpm = ((sortedDictionary.Count > 0) ? sortedDictionary.Last().Key : bmsTimeTable.MaxBpm);
		double num7 = 0.0;
		foreach (KeyValuePair<double, double> item8 in sortedDictionary2)
		{
			if (item8.Value >= num7)
			{
				bmsTimeTable.MainTimeBpm = item8.Key;
				num7 = item8.Value;
			}
		}
		if (sortedDictionary.Count <= 0)
		{
			bmsTimeTable.MainNotesBpm = bmsTimeTable.MainTimeBpm;
		}
		else
		{
			int num8 = 0;
			foreach (KeyValuePair<double, int> item9 in sortedDictionary)
			{
				if (item9.Value >= num8)
				{
					bmsTimeTable.MainNotesBpm = item9.Key;
					num8 = item9.Value;
				}
			}
		}
		return bmsTimeTable;
	}

	public List<int> GetTotalNotesPerLane(bool countLNEnd)
	{
		if (Rows.Count > 0)
		{
			List<int> list = new List<int>(Rows[0].Lanes.Count);
			for (int i = 0; i < Rows[0].Lanes.Count; i++)
			{
				list.Add(0);
			}
			for (int j = 0; j < Rows.Count; j++)
			{
				for (int k = 0; k < Rows[j].Lanes.Count; k++)
				{
					if (Rows[j].Lanes[k].Count >= 1 && Rows[j].Lanes[k].HasVisibleNotes && (countLNEnd || !Rows[j].Lanes[k].HasLNEnd))
					{
						list[k]++;
					}
				}
			}
			return list;
		}
		return null;
	}
}
