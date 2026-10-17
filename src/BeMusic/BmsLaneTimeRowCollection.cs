using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BeMusic;

public class BmsLaneTimeRowCollection : Collection<BmsLaneTime>
{
	public bool HasVisibleNotes
	{
		get
		{
			using (IEnumerator<BmsLaneTime> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					BmsObjectType objectType = enumerator.Current.ObjectType;
					if ((uint)objectType <= 2u)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	public bool HasLNStart
	{
		get
		{
			using (IEnumerator<BmsLaneTime> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.ObjectType == BmsObjectType.LNStart)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	public bool HasLNEnd
	{
		get
		{
			using (IEnumerator<BmsLaneTime> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.ObjectType == BmsObjectType.LNEnd)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
