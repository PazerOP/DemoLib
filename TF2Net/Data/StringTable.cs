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
	public class StringTable
	{
		public string TableName { get; set; }

		public ushort MaxEntries { get; set; }
		public IList<StringTableEntry> Entries { get; }
		
		public ushort? UserDataSize { get; set; }
		public byte? UserDataSizeBits { get; set; }

		public IList<StringTableEntry> History { get; } = new List<StringTableEntry>();

		public StringTable()
		{
			Entries = new SortedAutoList<StringTableEntry>(
				Comparer<StringTableEntry>.Create(
					(lhs, rhs) =>
					{
						Debug.Assert(lhs.ID != rhs.ID);

						return Comparer<int>.Default.Compare(lhs.ID, rhs.ID);
					}));
		}

		private class SortedAutoList<T> : SortedSet<T>, IList<T>
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
	}
}
