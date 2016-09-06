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
	class NetTempEntityMessage : INetMessage
	{
		public int EntryCount { get; set; }

		public ulong BitCount { get; set; }
		public byte[] Data { get; set; }

		public string Description
		{
			get
			{
				return string.Format("svc_TempEntities: number {0}, bytes {1}", EntryCount, BitInfo.BitsToBytes(BitCount));
			}
		}

		public ulong Size
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public NetMessageType Type { get { return NetMessageType.SVC_TEMPENTITIES; } }

		public void ReadMsg(byte[] buffer, ref ulong bitOffset)
		{
			EntryCount = (int)BitReader.ReadUInt(buffer, ref bitOffset, SourceConstants.EVENT_INDEX_BITS);

			BitCount = BitReader.ReadVarInt(buffer, ref bitOffset);

			Data = new byte[BitInfo.BitsToBytes(BitCount)];
			BitReader.CopyBits(buffer, BitCount, ref bitOffset, Data);
		}

		public void WriteMsg(byte[] buffer, ref ulong bitOffset)
		{
			throw new NotImplementedException();
		}
	}
}
