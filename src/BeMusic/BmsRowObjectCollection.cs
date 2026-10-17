using System;
using System.Collections.ObjectModel;
using BeMusic.Extentions;

namespace BeMusic;

public class BmsRowObjectCollection : Collection<BmsObject>
{
	public double Position { get; private set; } = double.NaN;

	public BmsRowObjectCollection(double position)
	{
		if (double.PositiveInfinity.Equals(position) || double.NegativeInfinity.Equals(position) || double.NaN.Equals(position))
		{
			throw new ArgumentException("position");
		}
		Position = position;
	}

	public new void Add(BmsObject item)
	{
		if (item == null)
		{
			throw new ArgumentNullException("item");
		}
		double position = item.Position;
		if (double.PositiveInfinity.Equals(position) || double.NegativeInfinity.Equals(position) || double.NaN.Equals(position))
		{
			throw new ArgumentException("item");
		}
		if (double.IsNaN(Position))
		{
			Position = item.Position;
		}
		else if (!Position.EpsilonEquals(item.Position))
		{
			return;
		}
		base.Add(item);
	}
}
