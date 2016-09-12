using System;
using System.Diagnostics;
using BitSet;

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

		public void ReadMsg(BitStream stream, IReadOnlyWorldState ws)
		{
			TableID = stream.ReadInt(MAX_TABLE_BITS);

			bool multipleChanged = stream.ReadBool();
			if (!multipleChanged)
				ChangedEntries = 1;
			else
				ChangedEntries = stream.ReadInt(CHANGED_ENTRIES_BITS);

			ulong bitCount = stream.ReadULong(DATA_LENGTH_BITS);
			Data = stream.Subsection(stream.Cursor, stream.Cursor + bitCount);
		}

		public void ApplyWorldState(WorldState ws)
		{
			throw new NotImplementedException();
		}
	}
}
