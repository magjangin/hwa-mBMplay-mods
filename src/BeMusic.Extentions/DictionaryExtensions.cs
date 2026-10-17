using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BeMusic.Extentions;

internal static class DictionaryExtensions
{
	public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
	{
		return new ReadOnlyDictionary<TKey, TValue>(dictionary);
	}
}
