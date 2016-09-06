using System;
using System.Diagnostics;
using BitSet;

namespace DemoLib.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	class NetPacketEntitiesMessage : INetMessage
	{
		const int DELTA_INDEX_BITS = 32;
		const int DELTA_SIZE_BITS = 20;

		public uint MaxEntries { get; set; }
		public uint UpdatedEntries { get; set; }
		public bool IsDelta { get; set; }
		public bool UpdateBaseline { get; set; }
		public bool Baseline { get; set; }
		public int DeltaFrom { get; set; }

		public ulong BitCount { get; set; }
		public byte[] Data { get; set; }

		private bool Unknown { get; set; }

		public string Description
		{
			get
			{
				return string.Format("svc_PacketEntities: delta {0}, max {1}, changed {2}, {3} bytes {4}",
					DeltaFrom, MaxEntries, UpdatedEntries,
					UpdateBaseline ? " BL update," : "",
					BitInfo.BitsToBytes(BitCount));
			}
		}

		public ulong Size
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public NetMessageType Type { get { return NetMessageType.SVC_PACKETENTITIES; } }

		public void ReadMsg(byte[] buffer, ref ulong bitOffset)
		{
			MaxEntries = (uint)BitReader.ReadUInt(buffer, ref bitOffset, SourceConstants.MAX_EDICT_BITS);

			IsDelta = BitReader.ReadUInt1(buffer, ref bitOffset) != 0;
			if (IsDelta)
				DeltaFrom = (int)BitReader.ReadUInt(buffer, ref bitOffset, DELTA_INDEX_BITS);
			else
				DeltaFrom = -1;

			Baseline = BitReader.ReadUInt1(buffer, ref bitOffset) != 0;

			UpdatedEntries = (uint)BitReader.ReadUInt(buffer, ref bitOffset, SourceConstants.MAX_EDICT_BITS);

			BitCount = BitReader.ReadUInt(buffer, ref bitOffset, DELTA_SIZE_BITS);

			UpdateBaseline = BitReader.ReadUInt1(buffer, ref bitOffset) != 0;

			Data = new byte[BitInfo.BitsToBytes(BitCount)];
			BitReader.CopyBits(buffer, BitCount, ref bitOffset, Data);
		}

		public void WriteMsg(byte[] buffer, ref ulong bitOffset)
		{
			throw new NotImplementedException();
		}
	}
}
