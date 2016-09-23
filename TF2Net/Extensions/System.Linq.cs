using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace System.Linq
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class TF2Net_Extensions
	{
		class ListWrapper<T> : IReadOnlyList<T>
		{
			readonly IList<T> m_Source;
			public ListWrapper(IList<T> src)
			{
				m_Source = src;
			}

			public T this[int index] { get { return m_Source[index]; } }
			public int Count { get { return m_Source.Count; } }
			public IEnumerator<T> GetEnumerator() { return m_Source.GetEnumerator(); }
			IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
		}

		public static IReadOnlyList<T> AsReadOnly<T>(this IList<T> input)
		{
			if (input == null)
				throw new ArgumentNullException(nameof(input));

			return new ListWrapper<T>(input);
		}

		class DictionaryWrapper<K, V> : IReadOnlyDictionary<K, V>
		{
			readonly IDictionary<K, V> m_Source;
			public DictionaryWrapper(IDictionary<K, V> src)
			{
				m_Source = src;
			}

			public V this[K key] { get { return m_Source[key]; } }
			public int Count { get { return m_Source.Count; } }

			public IEnumerable<K> Keys { get { return m_Source.Keys; } }
			public IEnumerable<V> Values { get { return m_Source.Values; } }

			public bool ContainsKey(K key)
			{
				return m_Source.ContainsKey(key);
			}

			public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
			{
				return m_Source.GetEnumerator();
			}

			public bool TryGetValue(K key, out V value)
			{
				return m_Source.TryGetValue(key, out value);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		public static IReadOnlyDictionary<K, V> AsReadOnly<K, V>(this IDictionary<K, V> input)
		{
			if (input == null)
				throw new ArgumentNullException(nameof(input));

			return new DictionaryWrapper<K, V>(input);
		}

		public static IEnumerable<T> Except<T>(this IEnumerable<T> input, T without)
		{
			if (input == null)
				throw new ArgumentNullException(nameof(input));

			foreach (var x in input)
			{
				if (x.Equals(without))
					continue;

				yield return x;
			}
		}
	}
}
