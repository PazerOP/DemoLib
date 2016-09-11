using System;
using System.Diagnostics;
using BitSet;

namespace DemoLib.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	class NetSoundMessage : INetMessage
	{
		const int SOUND_COUNT_BITS = 8;
		const int RELIABLE_SIZE_BITS = 8;
		const int UNRELIABLE_SIZE_BITS = 16;

		public bool Reliable { get; set; }
		public int SoundCount { get; set; }


		public ulong BitCount { get; set; }
		public byte[] Data { get; set; }

		public string Description
		{
			get
			{
				return string.Format("svc_Sounds: number {0},{1} bytes {2}",
					SoundCount, Reliable ? " reliable," : "", BitInfo.BitsToBytes(BitCount));
			}
		}

		public ulong Size
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public NetMessageType Type { get { return NetMessageType.SVC_SOUND; } }

		public void ReadMsg(DemoReader reader, uint? serverTick, byte[] buffer, ref ulong bitOffset)
		{
			Reliable = BitReader.ReadUInt1(buffer, ref bitOffset) != 0;

			if (Reliable)
			{
				SoundCount = 1;
				BitCount = BitReader.ReadUIntBits(buffer, ref bitOffset, RELIABLE_SIZE_BITS);
			}
			else
			{
				SoundCount = (int)BitReader.ReadUIntBits(buffer, ref bitOffset, SOUND_COUNT_BITS);
				BitCount = BitReader.ReadUIntBits(buffer, ref bitOffset, UNRELIABLE_SIZE_BITS);
			}

			Data = new byte[BitInfo.BitsToBytes(BitCount)];
			BitReader.CopyBits(buffer, BitCount, ref bitOffset, Data);
		}

		public void WriteMsg(byte[] buffer, ref ulong bitOffset)
		{
			throw new NotImplementedException();
		}
	}
}
