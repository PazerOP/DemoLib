using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitSet;

namespace DemoLib.NetMessages
{
	class NetUpdateStringTableMessage : INetMessage
	{
		const int MAX_TABLE_BITS = 5;
		const int MAX_TABLES = (1 << MAX_TABLE_BITS);
		const int DATA_LENGTH_BITS = 20;
		const int CHANGED_ENTRIES_BITS = 16;

		public int TableID { get; set; }
		public int ChangedEntries { get; set; }
		public byte[] Data { get; set; }

		public string Description
		{
			get
			{
				throw new NotImplementedException();
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
			TableID = (int)BitReader.ReadUInt(buffer, ref bitOffset, MAX_TABLE_BITS);

			bool multipleChanged = false;
			multipleChanged = BitReader.ReadUInt1(buffer, ref bitOffset) != 0;

			if (!multipleChanged)
				ChangedEntries = 1;
			else
				ChangedEntries = (int)BitReader.ReadUInt(buffer, ref bitOffset, CHANGED_ENTRIES_BITS);

			Data = new byte[BitInfo.BitsToBytes(BitReader.ReadUInt(buffer, ref bitOffset, DATA_LENGTH_BITS))];
			throw new NotImplementedException();
		}

		public void WriteMsg(byte[] buffer, ref ulong bitOffset)
		{
			throw new NotImplementedException();
		}
	}
}
