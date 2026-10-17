using System;
using System.Collections.Generic;
using System.Linq;
using BeMusic.Extentions;
using Randomize;

namespace BeMusic;

public partial class BmsTimeTable
{
	public BmsTimeTable SwapLanes(int keys, LaneSwapOption player1SwapOption, LaneSwapOption player2SwapOption, bool flip, long randomSeed)
	{
		XorShift random = new XorShift(randomSeed);
		List<int> p1Lanes = null;
		List<int> p2Lanes = null;

		switch (keys)
		{
			case 5:
				p1Lanes = new List<int> { 1, 2, 3, 4, 5 };
				break;
			case 7:
				p1Lanes = new List<int> { 1, 2, 3, 4, 5, 6, 7 };
				break;
			case 9:
				p1Lanes = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
				break;
			case 10:
				p1Lanes = new List<int> { 1, 2, 3, 4, 5 };
				p2Lanes = new List<int> { 11, 12, 13, 14, 15 };
				break;
			case 14:
				p1Lanes = new List<int> { 1, 2, 3, 4, 5, 6, 7 };
				p2Lanes = new List<int> { 11, 12, 13, 14, 15, 16, 17 };
				break;
			default:
				throw new NotSupportedException("Unsupported playstyle.");
		}

		Dictionary<int, int> swapDict;
		if (p2Lanes == null)
		{
			swapDict = CreateLaneSwapDictionary(p1Lanes, player1SwapOption, 1, random);
		}
		else
		{
			Dictionary<int, int> p1Dict = CreateLaneSwapDictionary(flip ? p2Lanes : p1Lanes, player1SwapOption, 1, random);
			Dictionary<int, int> p2Dict = CreateLaneSwapDictionary(flip ? p1Lanes : p2Lanes, player2SwapOption, 11, random);
			swapDict = new Dictionary<int, int>(p1Lanes.Count + p2Lanes.Count + 2);
			foreach (var kvp in p1Dict) swapDict.Add(kvp.Key, kvp.Value);
			foreach (var kvp in p2Dict) swapDict.Add(kvp.Key, kvp.Value);
			if (flip)
			{
				swapDict[0] = 10;
				swapDict[10] = 0;
			}
		}

		BmsTimeTable result = new BmsTimeTable
		{
			LaneCount = LaneCount,
			Rows = new List<BmsTimeTableRow>(Rows.Count),
			BarIndex = new List<int>(BarIndex),
			InitialBpm = InitialBpm,
			MaxBpm = MaxBpm,
			MinBpm = MinBpm,
			MaxNotesBpm = MaxNotesBpm,
			MinNotesBpm = MinNotesBpm,
			MainNotesBpm = MainNotesBpm,
			MainTimeBpm = MainTimeBpm
		};

		for (int i = 0; i < Rows.Count; i++)
		{
			result.Rows.Add(new BmsTimeTableRow(Rows[i], swapDict));
		}

		return result;
	}

	private static double GetMinNoteInterval(LaneSwapOption option)
	{
		return option switch
		{
			LaneSwapOption.SRandom => 50.0, 
			LaneSwapOption.HRandom => 200.0, 
			LaneSwapOption.AllScratch => 100.0, 
			_ => 0.0, 
		};
	}

	private static List<List<int>> GeneratePeriodicRandomList(IReadOnlyList<int> laneList, int startIndex, XorShift random)
	{
		List<List<int>> list = new List<List<int>>(laneList.Count);
		List<int> laneList2 = Randomize(laneList, random);
		List<int> list2 = Randomize(laneList, random);
		int num = ((!random.NextBool()) ? 1 : (-1));
		for (int i = 0; i < laneList.Count; i++)
		{
			List<int> list3 = Rotate(laneList2, i * num);
			List<int> list4 = new List<int>(list3.Count);
			for (int j = 0; j < list3.Count; j++)
			{
				list4.Add(list3[list2[j] - startIndex]);
			}
			list.Add(list4);
		}
		return list;
	}

	public BmsTimeTable SwapLanes(int keys, LaneSwapOption player1SwapOption, LaneSwapOption player2SwapOption, bool flip)
	{
		return SwapLanes(keys, player1SwapOption, player2SwapOption, flip, XorShift.GenerateSeed());
	}

	public BmsTimeTable SwapLanes(int keys, LaneSwapOption swapOption, long randomSeed)
	{
		return SwapLanes(keys, swapOption, swapOption, (keys == 14 || keys == 10) && swapOption == LaneSwapOption.Mirror, randomSeed);
	}

	public BmsTimeTable SwapLanes(int keys, LaneSwapOption swapOption)
	{
		return SwapLanes(keys, swapOption, swapOption, (keys == 14 || keys == 10) && swapOption == LaneSwapOption.Mirror, XorShift.GenerateSeed());
	}

	private static Dictionary<int, int> CreateLaneSwapDictionary(IReadOnlyList<int> laneList, LaneSwapOption swapOption, int startIndex, XorShift randomizer)
	{
		IReadOnlyList<int> readOnlyList;
		switch (swapOption)
		{
		case LaneSwapOption.Random:
			readOnlyList = Randomize(laneList, randomizer);
			break;
		case LaneSwapOption.Mirror:
			readOnlyList = Reverse(laneList);
			break;
		case LaneSwapOption.Rotate:
			readOnlyList = Rotate(laneList, randomizer);
			if (randomizer.NextBool())
			{
				readOnlyList = Reverse(readOnlyList);
			}
			break;
		case LaneSwapOption.Cross:
			readOnlyList = Cross(laneList, randomizer);
			if (randomizer.NextBool())
			{
				readOnlyList = Reverse(readOnlyList);
			}
			break;
		case LaneSwapOption.CrossRotate:
			readOnlyList = Rotate(Cross(laneList, randomizer), randomizer);
			if (randomizer.NextBool())
			{
				readOnlyList = Reverse(readOnlyList);
			}
			break;
		case LaneSwapOption.Split:
			readOnlyList = Split(laneList, randomizer);
			if (randomizer.NextBool())
			{
				readOnlyList = Reverse(readOnlyList);
			}
			break;
		case LaneSwapOption.OddEven:
			readOnlyList = OddEven(laneList, randomizer);
			break;
		case LaneSwapOption.Spread:
			readOnlyList = Spread(laneList, randomizer);
			break;
		default:
			readOnlyList = laneList;
			break;
		}
		Dictionary<int, int> dictionary = new Dictionary<int, int>(readOnlyList.Count);
		for (int i = 0; i < laneList.Count; i++)
		{
			dictionary[readOnlyList[i]] = i + startIndex;
		}
		return dictionary;
	}

	private static List<T> Randomize<T>(IReadOnlyList<T> laneList, XorShift randomizer)
	{
		List<T> list = new List<T>(laneList);
		List<T> list2 = new List<T>(list.Count);
		while (list.Count > 0)
		{
			int index = ((list.Count != 1) ? randomizer.Next(list.Count) : 0);
			list2.Add(list[index]);
			list.RemoveAt(index);
		}
		return list2;
	}

	private static List<T> Reverse<T>(IReadOnlyList<T> laneList)
	{
		List<T> list = new List<T>(laneList.Count);
		for (int num = laneList.Count - 1; num >= 0; num--)
		{
			list.Add(laneList[num]);
		}
		return list;
	}

	private static List<T> Rotate<T>(IReadOnlyList<T> laneList, int offset)
	{
		if (laneList == null)
		{
			throw new ArgumentNullException("laneList");
		}
		if (offset == 0)
		{
			return new List<T>(laneList);
		}
		int num = offset % laneList.Count;
		if (num < 0)
		{
			num = laneList.Count - num;
		}
		List<T> list = new List<T>(laneList.Count);
		int num2 = num;
		while (list.Count < laneList.Count)
		{
			list.Add(laneList[num2 % laneList.Count]);
			num2++;
		}
		return list;
	}

	private static List<T> Rotate<T>(IReadOnlyList<T> laneList, XorShift randomizer)
	{
		return Rotate(laneList, randomizer.Next(1, laneList.Count));
	}

	private static List<T> Cross<T>(IReadOnlyList<T> laneList, XorShift randomizer)
	{
		List<T> list = new List<T>(laneList);
		int num = randomizer.Next(laneList.Count);
		for (int i = 0; i < (laneList.Count - 1) / 2; i++)
		{
			int num2 = (num + 1 + i * 2) % laneList.Count;
			int index = (num2 + 1) % laneList.Count;
			T value = list[num2];
			list[num2] = list[index];
			list[index] = value;
		}
		return list;
	}

	private static List<T> Split<T>(IReadOnlyList<T> laneList, XorShift randomizer)
	{
		List<List<T>> list = new List<List<T>>
		{
			new List<T>(laneList.Count / 2 + laneList.Count % 2),
			new List<T>(laneList.Count / 2)
		};
		for (int i = 0; i < laneList.Count; i++)
		{
			list[i % 2].Add(laneList[i]);
		}
		List<T> list2 = new List<T>(laneList.Count);
		while (list.Count > 0)
		{
			while (list[0].Count > 0)
			{
				int index = ((list[0].Count != 1) ? randomizer.Next(list[0].Count) : 0);
				list2.Add(list[0][index]);
				list[0].RemoveAt(index);
			}
			list.RemoveAt(0);
		}
		return list2;
	}

	private static List<T> OddEven<T>(IReadOnlyList<T> laneList, XorShift randomizer)
	{
		List<List<T>> list = new List<List<T>>
		{
			new List<T>(laneList.Count / 2 + laneList.Count % 2),
			new List<T>(laneList.Count / 2)
		};
		for (int i = 0; i < laneList.Count; i++)
		{
			list[i % 2].Add(laneList[i]);
		}
		List<T> list2 = new List<T>(laneList.Count);
		while (list.Any((List<T> t) => t.Count > 0))
		{
			for (int num = 0; num < list.Count; num++)
			{
				if (list[num].Count > 0)
				{
					int index = ((list[num].Count != 1) ? randomizer.Next(list[num].Count) : 0);
					list2.Add(list[num][index]);
					list[num].RemoveAt(index);
				}
			}
		}
		if (list2.SequenceEqual(laneList) || list2.SequenceEqual(Reverse(laneList)))
		{
			return OddEven(laneList, randomizer);
		}
		return list2;
	}

	private static List<T> Spread<T>(IReadOnlyList<T> laneList, XorShift randomizer)
	{
		List<List<T>> list = new List<List<T>>
		{
			new List<T>(laneList.Count / 2 + laneList.Count % 2),
			new List<T>(laneList.Count / 2)
		};
		int num = randomizer.Next(laneList.Count);
		for (int i = 0; i < laneList.Count / 2 + laneList.Count % 2; i++)
		{
			list[0].Add(laneList[num % laneList.Count]);
			num++;
		}
		for (int j = 0; j < laneList.Count / 2; j++)
		{
			list[1].Add(laneList[num % laneList.Count]);
			num++;
		}
		List<T> list2 = new List<T>(laneList.Count);
		while (list.Any((List<T> t) => t.Count > 0))
		{
			for (int num2 = 0; num2 < list.Count; num2++)
			{
				if (list[num2].Count > 0)
				{
					int index = ((list[num2].Count != 1) ? randomizer.Next(list[num2].Count) : 0);
					list2.Add(list[num2][index]);
					list[num2].RemoveAt(index);
				}
			}
		}
		return list2;
	}
}
