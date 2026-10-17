using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Randomize;

public class XorShift
{
	private static readonly ReadOnlyCollection<uint> Primes1 = new List<uint>
	{
		248999659u, 313975751u, 757732021u, 810895027u, 1118136319u, 1563241297u, 1794337481u, 1889888629u, 2389456523u, 2587205443u,
		2772385103u, 2988579337u, 3324410807u, 3717267487u, 3861777707u, 4192757381u
	}.AsReadOnly();

	private static readonly ReadOnlyCollection<uint> Primes2 = new List<uint>
	{
		235797431u, 504282979u, 753532033u, 935683361u, 1282677371u, 1363675871u, 1859948081u, 2087022943u, 2205435149u, 2523103421u,
		2937984811u, 3055864993u, 3245600857u, 3593117959u, 3865501543u, 4226534341u
	}.AsReadOnly();

	private static readonly ReadOnlyCollection<uint> Primes3 = new List<uint>
	{
		201810811u, 380214409u, 629663737u, 824391569u, 1162893871u, 1591570573u, 1852635949u, 2121369491u, 2172709853u, 2660730811u,
		2849713819u, 3022406197u, 3459902753u, 3609214229u, 4003803941u, 4191068099u
	}.AsReadOnly();

	private static readonly ReadOnlyCollection<uint> Primes4 = new List<uint>
	{
		256154231u, 474403613u, 718845761u, 864517573u, 1260056729u, 1465519397u, 1855844027u, 2097484577u, 2164492529u, 2442420823u,
		2810084561u, 3188669723u, 3277626361u, 3739748977u, 3902243953u, 4087864771u
	}.AsReadOnly();

	private uint x;

	private uint y;

	private uint z;

	private uint w;

	public static long GenerateSeed()
	{
		long timestamp = Stopwatch.GetTimestamp();
		return DateTime.Now.Ticks ^ (timestamp << 32);
	}

	private static long GenerateLongSeedFromUint(uint seed)
	{
		uint num = (seed << 24) | ((seed & 0xFF00) << 8) | ((seed & 0xFF0000) >> 8) | (seed >> 24);
		num *= Primes4[(int)(num & 0xF)];
		return (long)(seed | ((ulong)num << 32));
	}

	public XorShift()
		: this(GenerateSeed())
	{
	}

	public XorShift(long seed)
	{
		long num = ((((seed >> 32) & 0x7FFFFFFF) == 0L) ? (GenerateLongSeedFromUint((uint)seed) ^ (seed & long.MinValue)) : seed);
		x = (uint)((int)num | (1 << (int)num));
		y = (uint)((int)(~(num >> 32)) | (1 << (int)x));
		z = ((x ^ y) * Primes1[(int)(x & 0xF)]) | (uint)(1 << (int)y);
		w = (Primes2[(int)(y & 0xF)] * Primes3[(int)(z & 0xF)]) | (uint)(1 << (int)z);
		for (uint num2 = 0u; num2 < 32; num2++)
		{
			Next();
		}
		uint num3 = Next() & 0x1F;
		for (uint num4 = 0u; num4 < num3; num4++)
		{
			Next();
		}
	}

	public XorShift(ulong seed)
		: this((long)seed)
	{
	}

	public XorShift(uint seed)
		: this(GenerateLongSeedFromUint(seed))
	{
	}

	public XorShift(int seed)
		: this((uint)seed)
	{
	}

	public uint Next()
	{
		uint num = x ^ (x << 11);
		x = y;
		y = z;
		z = w;
		w = w ^ (w >> 19) ^ (num ^ (num >> 8));
		return w;
	}

	public int Next(int min, int lessThan)
	{
		if (min >= lessThan)
		{
			throw new ArgumentOutOfRangeException("lessThan");
		}
		uint num = (uint)(lessThan - min);
		if (num == 1)
		{
			return min;
		}
		uint num2 = uint.MaxValue - uint.MaxValue % num;
		uint num3;
		do
		{
			num3 = Next();
		}
		while (num3 > num2);
		return min + (int)(num3 % num);
	}

	public int Next(int lessThan)
	{
		return Next(0, lessThan);
	}

	public bool NextBool()
	{
		return (Next() & 1) == 1;
	}

	public double NextDouble()
	{
		return BitConverter.ToDouble(BitConverter.GetBytes(0x3FF0000000000000L | (NextLong() >> 12)), 0) - 1.0;
	}

	public long NextLong()
	{
		return (long)(((ulong)Next() << 32) | Next());
	}
}
