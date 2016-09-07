using System;
using System.Diagnostics;
using BitSet;

namespace DemoLib.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	class NetUpdateStringTableMessage : INetMessage
	{
		const int MAX_TABLE_BITS = 5;
		const int MAX_TABLES = (1 << MAX_TABLE_BITS);
		const int DATA_LENGTH_BITS = 20;
		const int CHANGED_ENTRIES_BITS = 16;

		public int TableID { get; set; }
		public int ChangedEntries { get; set; }

		public ulong BitCount { get; set; }
		public byte[] Data { get; set; }

		public string Description
		{
			get
			{
				return string.Format("svc_UpdateStringTable: table {0}, changed {1}, bytes {2}",
					TableID, ChangedEntries, BitInfo.BitsToBytes(BitCount));
			}
		}

		public ulong Size
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public NetMessageType Type { get { return NetMessageType.SVC_UPDATESTRINGTABLE; } }

		public void ReadMsg(byte[] buffer, ref ulong bitOffset)
		{
			TableID = (int)BitReader.ReadUIntBits(buffer, ref bitOffset, MAX_TABLE_BITS);

			bool multipleChanged = false;
			multipleChanged = BitReader.ReadUInt1(buffer, ref bitOffset) != 0;

			if (!multipleChanged)
				ChangedEntries = 1;
			else
				ChangedEntries = (int)BitReader.ReadUIntBits(buffer, ref bitOffset, CHANGED_ENTRIES_BITS);

			BitCount = BitReader.ReadUIntBits(buffer, ref bitOffset, DATA_LENGTH_BITS);
			Data = new byte[BitInfo.BitsToBytes(BitCount)];
			BitReader.CopyBits(buffer, BitCount, ref bitOffset, Data);
		}

		public void WriteMsg(byte[] buffer, ref ulong bitOffset)
		{
			throw new NotImplementedException();
		}
	}
}
