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
	class NetGameEventListMessage : INetMessage
	{
		public ushort EventsCount { get; set; }
		
		public ulong BitCount { get; set; }
		public byte[] Data { get; set; }

		public string Description
		{
			get
			{
				return string.Format("svc_GameEventList: number {0}, bytes {1}",
					EventsCount, BitInfo.BitsToBytes(BitCount));
			}
		}

		public ulong Size
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public void ReadMsg(byte[] buffer, ref ulong bitOffset)
		{
			EventsCount = (ushort)BitReader.ReadUIntBits(buffer, ref bitOffset, SourceConstants.MAX_EVENT_BITS);

			BitCount = BitReader.ReadUIntBits(buffer, ref bitOffset, 20);
			Data = new byte[BitInfo.BitsToBytes(BitCount)];
			BitReader.CopyBits(buffer, BitCount, ref bitOffset, Data);
		}

		public void WriteMsg(byte[] buffer, ref ulong bitOffset)
		{
			throw new NotImplementedException();
		}
	}
}
