using System;

namespace SloshyDoshMan
{
	public static class ObjectExtensions
	{
		internal static bool Equals<T>(this T objA, T objB, params Func<T, object>[] propertyAccessors)
		{
			foreach (var propertyAccessor in propertyAccessors)
			{
				if (!propertyAccessor(objA).Equals(propertyAccessor(objB)))
				{
					return false;
				}
			}

			return true;
		}

		internal static int GetHashCode(params object[] hashCodeProperties)
		{
			unchecked
			{
				int hash = (int)2166136261;

				foreach (var hashCodeProperty in hashCodeProperties)
				{
					hash = (hash * 16777619) ^ hashCodeProperty.GetHashCode();
				}

				return hash;
			}
		}
	}
}
