using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitSet;

namespace TF2Net.Data
{
	[DebuggerDisplay("{ID,nq}: {Value}")]
	public class StringTableEntry
	{
		public StringTable Table { get; }

		public SingleEvent<Action<StringTableEntry>> EntryChanged { get; } = new SingleEvent<Action<StringTableEntry>>();

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		ushort m_ID;
		public ushort ID
		{
			get { return m_ID; }
			set
			{
				if (m_ID != value)
				{
					m_ID = value;
					EntryChanged?.Invoke(this);
				}
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		string m_Value;
		public string Value
		{
			get { return m_Value; }
			set
			{
				if (m_Value != value)
				{
					m_Value = value;
					EntryChanged?.Invoke(this);
				}
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		BitStream m_UserData;
		public BitStream UserData
		{
			get { return m_UserData?.Clone(); }
			set
			{
				if (m_UserData != value)
				{
					m_UserData = value;
					EntryChanged?.Invoke(this);
				}
			}
		}

		public StringTableEntry(StringTable table)
		{
			Table = table;
		}
	}
}
