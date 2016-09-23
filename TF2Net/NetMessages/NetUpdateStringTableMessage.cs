using System;
using System.Diagnostics;
using BitSet;
using TF2Net.Data;
using TF2Net.NetMessages.Shared;

namespace TF2Net.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	public class NetUpdateStringTableMessage : INetMessage
	{
		const int MAX_TABLE_BITS = 5;
		const int MAX_TABLES = (1 << MAX_TABLE_BITS);
		const int DATA_LENGTH_BITS = 20;
		const int CHANGED_ENTRIES_BITS = 16;

		public int TableID { get; set; }
		public int ChangedEntries { get; set; }

		public BitStream Data { get; set; }

		public string Description
		{
			get
			{
				return string.Format("svc_UpdateStringTable: table {0}, changed {1}, bytes {2}",
					TableID, ChangedEntries, BitInfo.BitsToBytes(Data.Length));
			}
		}

		public void ReadMsg(BitStream stream)
		{
			TableID = stream.ReadInt(MAX_TABLE_BITS);

			bool multipleChanged = stream.ReadBool();
			if (!multipleChanged)
				ChangedEntries = 1;
			else
				ChangedEntries = stream.ReadInt(CHANGED_ENTRIES_BITS);

			ulong bitCount = stream.ReadULong(DATA_LENGTH_BITS);
			Data = stream.Subsection(stream.Cursor, stream.Cursor + bitCount);
			stream.Seek(bitCount, System.IO.SeekOrigin.Current);
		}

		public void ApplyWorldState(WorldState ws)
		{
			StringTable found = ws.StringTables[TableID];
			StringTableParser.ParseUpdate(Data, found, (ushort)ChangedEntries);

			ws.Listeners.OnStringTableUpdated(ws, found);
		}
	}
}
