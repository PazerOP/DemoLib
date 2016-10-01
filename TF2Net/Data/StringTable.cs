using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TF2Net.Data
{
	[DebuggerDisplay("Stringtable: {TableName} ({Entries.Count,nq}/{MaxEntries,nq})")]
	public class StringTable : IEnumerable<StringTableEntry>
	{
		public WorldState World { get; }

		public string TableName { get; }

		public ushort MaxEntries { get; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly SortedAutoList<StringTableEntry> m_Entries;
		public IReadOnlyList<StringTableEntry> Entries { get { return m_Entries; } }
		
		public ushort? UserDataSize { get; }
		public byte? UserDataSizeBits { get; }

		public event Action<StringTable> StringTableUpdated;

		public StringTable(WorldState ws, string tableName, ushort maxEntries, ushort? userDataSize, byte? userDataSizeBits)
		{
			World = ws;
			TableName = tableName;
			MaxEntries = maxEntries;
			UserDataSize = userDataSize;
			UserDataSizeBits = userDataSizeBits;

			m_Entries = new SortedAutoList<StringTableEntry>(
				Comparer<StringTableEntry>.Create(
					(lhs, rhs) =>
					{
						Debug.Assert(lhs.ID != rhs.ID);

						return Comparer<int>.Default.Compare(lhs.ID, rhs.ID);
					}));
		}

		public void Add(StringTableEntry entry)
		{
			Debug.Assert(entry.Table == this);
			m_Entries.Add(entry);
			entry.EntryChanged += Entry_EntryChanged;
		}

		private void Entry_EntryChanged(StringTableEntry entry)
		{
			StringTableUpdated?.Invoke(this);
		}

		private class SortedAutoList<T> : SortedSet<T>, IList<T>, IReadOnlyList<T>
		{
			public T this[int index]
			{
				get
				{
					return this.ElementAt(index);
				}
				set
				{
					Remove(this.ElementAt(index));
					Add(value);
				}
			}

			public int IndexOf(T item)
			{
				throw new NotSupportedException();
			}

			public void Insert(int index, T item)
			{
				throw new NotSupportedException();
			}

			public void RemoveAt(int index)
			{
				Remove(this.ElementAt(index));
			}

			public SortedAutoList(IComparer<T> comparer) : base(comparer) { }
		}

		public IEnumerator<StringTableEntry> GetEnumerator()
		{
			return Entries.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
	}
}
