using System.Collections.Generic;

namespace SloshyDoshMan.Service
{
	public static class DictionaryExtensions
	{
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
