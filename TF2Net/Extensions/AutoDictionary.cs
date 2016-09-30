using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TF2Net.Extensions
{
	class AutoDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
	{
		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		readonly List<TValue> m_Contents = new List<TValue>();

		public TValue this[TKey key]
		{
			get
			{
				TValue outValue;
				if (!TryGetValue(key, out outValue))
					throw new KeyNotFoundException();

				return outValue;
			}
			set
			{
				for (uint i = 0; i < m_Contents.Count; i++)
				{
					TKey currentKey = m_KeySelector(m_Contents[(int)i]);
					if (m_KeyComparer.Equals(currentKey, key))
					{
						m_Contents[(int)i] = value;
						return;
					}
				}

				Add(key, value);
			}
		}

		public int Count { get { return m_Contents.Count; } }
		public bool IsReadOnly { get { return false; } }
		public ICollection<TKey> Keys { get { return new KeyCollection(m_Contents, m_KeySelector, m_KeyComparer); } }
		public ICollection<TValue> Values { get { return m_Contents; } }

		IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys { get { return m_Contents.Select(v => m_KeySelector(v)); } }
		IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values { get { return Values; } }

		readonly Func<TValue, TKey> m_KeySelector;
		readonly IEqualityComparer<TKey> m_KeyComparer;

		public AutoDictionary(Func<TValue, TKey> keySelector) : this(keySelector, EqualityComparer<TKey>.Default) { }
		public AutoDictionary(Func<TValue, TKey> keySelector, IEqualityComparer<TKey> keyComparer)
		{
			if (keySelector == null)
				throw new ArgumentNullException(nameof(keySelector));
			if (keyComparer == null)
				throw new ArgumentNullException(nameof(keyComparer));

			m_KeySelector = keySelector;
			m_KeyComparer = keyComparer;
		}

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			Add(item.Key, item.Value);
		}

		public void Add(TKey key, TValue value)
		{
			Debug.Assert(m_KeyComparer.Equals(key, m_KeySelector(value)));
			Add(value);
		}

		public void Add(TValue value)
		{
			m_Contents.Add(value);
		}

		public void Clear()
		{
			m_Contents.Clear();
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			throw new NotImplementedException();
		}

		public bool ContainsKey(TKey key)
		{
			return m_Contents.SingleOrDefault(v => m_KeyComparer.Equals(key, m_KeySelector(v))) != null;
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			foreach (var kvPair in this)
				array[arrayIndex++] = kvPair;
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return m_Contents
				.Select(v => new KeyValuePair<TKey, TValue>(m_KeySelector(v), v))
				.GetEnumerator();
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			throw new NotImplementedException();
		}

		public bool Remove(TKey key)
		{
			for (uint i = 0; i < m_Contents.Count; i++)
			{
				TKey currentKey = m_KeySelector(m_Contents[(int)i]);

				if (m_KeyComparer.Equals(currentKey, key))
				{
					m_Contents.RemoveAt((int)i);
					return true;
				}
			}

			return false;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			foreach (var v in m_Contents)
			{
				TKey currentKey = m_KeySelector(v);
				if (m_KeyComparer.Equals(currentKey, key))
				{
					value = v;
					return true;
				}
			}

			value = default(TValue);
			return false;
		}

		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

		class KeyCollection : ICollection<TKey>
		{
			readonly List<TValue> m_Values;
			readonly Func<TValue, TKey> m_KeySelector;
			readonly IEqualityComparer<TKey> m_KeyComparer;

			public KeyCollection(List<TValue> values, Func<TValue, TKey> keySelector, IEqualityComparer<TKey> keyComparer)
			{
				m_Values = values;
				m_KeySelector = keySelector;
				m_KeyComparer = keyComparer;
			}

			public int Count { get { return m_Values.Count; } }
			public bool IsReadOnly { get { return false; } }

			public void Add(TKey item) { throw new NotSupportedException(); }

			public void Clear() { m_Values.Clear(); }

			public bool Contains(TKey item)
			{
				return this.Where(k => m_KeyComparer.Equals(item, k)).Any();
			}

			public void CopyTo(TKey[] array, int arrayIndex)
			{
				throw new NotImplementedException();
			}

			IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
			public IEnumerator<TKey> GetEnumerator()
			{
				return m_Values.Select(v => m_KeySelector(v)).GetEnumerator();
			}

			public bool Remove(TKey item)
			{
				for (uint i = 0; i < m_Values.Count; i++)
				{
					TKey currentKey = m_KeySelector(m_Values[(int)i]);
					if (m_KeyComparer.Equals(currentKey, item))
					{
						m_Values.RemoveAt((int)i);
						return true;
					}
				}

				return false;
			}
		}
	}
}
