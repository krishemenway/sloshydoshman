using System;
using System.Collections.Generic;

namespace SloshyDoshMan.Service
{
	public static class DictionaryExtensions
	{
		/// <summary>
		/// Sets default value for a set of keys when a value does not exist.
		/// </summary>
		/// <typeparam name="TKey">The type of the key of the dictionary.</typeparam>
		/// <typeparam name="TValue">The type of the value of the dictionary.</typeparam>
		/// <param name="valuesDictionary">Dictionary that will be altered with values for each of the keys received.</param>
		/// <param name="keys">The keys to set default values for.</param>
		/// <param name="getDefaultValueForKey">Function to get default value for a key.</param>
		/// <returns></returns>
		public static IDictionary<TKey, TValue> SetDefaultValuesForKeys<TKey, TValue>(this IDictionary<TKey, TValue> valuesDictionary, IEnumerable<TKey> keys, Func<TKey, TValue> getDefaultValueForKey)
		{
			foreach(var key in keys)
			{
				if (!valuesDictionary.ContainsKey(key))
				{
					valuesDictionary.Add(key, getDefaultValueForKey(key));
				}
			}

			return valuesDictionary;
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
