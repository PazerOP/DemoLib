using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitSet;

namespace DemoLib.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	class NetEntityMessage : INetMessage
	{
		const int DATA_LENGTH_BITS = 11;

		public uint EntityIndex { get; set; }
		public uint ClassID { get; set; }

		public ulong BitCount { get; set; }
		public byte[] Data { get; set; }

		public string Description
		{
			get
			{
				return string.Format("svc_EntityMessage: entity {0}, class {1}, bytes {2}",
					EntityIndex, ClassID, BitInfo.BitsToBytes(BitCount));
			}
		}

		public ulong Size
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public void ReadMsg(DemoReader reader, byte[] buffer, ref ulong bitOffset)
		{
			EntityIndex = (uint)BitReader.ReadUIntBits(buffer, ref bitOffset, SourceConstants.MAX_EDICT_BITS);
			ClassID = (uint)BitReader.ReadUIntBits(buffer, ref bitOffset, SourceConstants.MAX_SERVER_CLASS_BITS);

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
