using System;
using System.Collections.Generic;
using System.Linq;

namespace SloshyDoshMan.Service
{
	public static class DictionaryExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="valuesDictionary"></param>
		/// <param name="keys"></param>
		/// <param name="getDefaultValueForKey"></param>
		/// <returns></returns>
		public static IDictionary<TKey, TValue> SetDefaultValuesForKeys<TKey, TValue>(this IDictionary<TKey, TValue> valuesDictionary, IEnumerable<TKey> keys, Func<TKey, TValue> getDefaultValueForKey)
		{
			var defaultsDictionary = keys.ToDictionary(x => x, x => getDefaultValueForKey(x));
			return defaultsDictionary.Merge(valuesDictionary);
		}

		/// <summary>
		/// Set all keys in "to" parameter into the "from" parameter
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns>Dictionary with merged options</returns>
		public static IDictionary<TKey, TValue> Merge<TKey, TValue>(this IDictionary<TKey, TValue> from, IDictionary<TKey, TValue> to)
		{
			foreach (var pair in to)
			{
				from[pair.Key] = pair.Value;
			}

			return from;
		}
	}
}
