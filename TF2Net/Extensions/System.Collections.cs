using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class TF2Net_Extensions
	{
		public static uint? FindNextSetBit(this BitArray source, uint startIndex = 0)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));

			for (uint i = startIndex; i < source.Length; i++)
			{
				if (source[(int)i])
					return i;
			}

			return null;
		}

		public static IDictionary<TKey, TValue> Clone<TKey, TValue>(this IDictionary<TKey, TValue> src)
		{
			return new Dictionary<TKey, TValue>(src);
		}

		public static void AddRange<T>(this IList<T> input, IEnumerable<T> range)
		{
			if (input == null)
				throw new ArgumentNullException(nameof(input));
			if (range == null)
				throw new ArgumentNullException(nameof(range));

			foreach (var x in range)
				input.Add(x);
		}
	}
}
