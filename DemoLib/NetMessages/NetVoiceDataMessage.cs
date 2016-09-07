using System;
using System.Diagnostics;
using BitSet;

namespace DemoLib.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	class NetVoiceDataMessage : INetMessage
	{
		public byte ClientIndex { get; set; }
		public bool Proximity { get; set; }
		
		public ulong BitCount { get; set; }
		public byte[] Data { get; set; }

		public string Description
		{
			get
			{
				return string.Format("svc_VoiceData: client {0}, bytes {1}",
					ClientIndex, BitInfo.BitsToBytes(BitCount));
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
			ClientIndex = (byte)BitReader.ReadUIntBits(buffer, ref bitOffset, 8);
			Proximity = BitReader.ReadUIntBits(buffer, ref bitOffset, 8) != 0;

			BitCount = BitReader.ReadUIntBits(buffer, ref bitOffset, 16);
			Data = new byte[BitInfo.BitsToBytes(BitCount)];
			BitReader.CopyBits(buffer, BitCount, ref bitOffset, Data);			
		}

		public void WriteMsg(byte[] buffer, ref ulong bitOffset)
		{
			throw new NotImplementedException();
		}
	}
}
