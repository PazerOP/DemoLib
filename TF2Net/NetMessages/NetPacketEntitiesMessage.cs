using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using BitSet;
using TF2Net.Data;

namespace TF2Net.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	public class NetPacketEntitiesMessage : INetMessage
	{
		const int DELTA_INDEX_BITS = 32;
		const int DELTA_SIZE_BITS = 20;

		public uint MaxEntries { get; set; }
		public uint UpdatedEntries { get; set; }
		public bool IsDelta { get; set; }
		public bool UpdateBaseline { get; set; }
		public bool Baseline { get; set; }
		public int? DeltaFrom { get; set; }

		public BitStream Data { get; set; }

		public string Description
		{
			get
			{
				return string.Format("svc_PacketEntities: delta {0}, max {1}, changed {2}, {3} bytes {4}",
					DeltaFrom, MaxEntries, UpdatedEntries,
					UpdateBaseline ? " BL update," : "",
					BitInfo.BitsToBytes(Data.Length));
			}
		}

		public void ReadMsg(BitStream stream)
		{
			MaxEntries = stream.ReadUInt(SourceConstants.MAX_EDICT_BITS);

			IsDelta = stream.ReadBool();
			if (IsDelta)
				DeltaFrom = stream.ReadInt(DELTA_INDEX_BITS);

			Baseline = stream.ReadBool();

			UpdatedEntries = stream.ReadUInt(SourceConstants.MAX_EDICT_BITS);

			ulong bitCount = stream.ReadULong(DELTA_SIZE_BITS);

			UpdateBaseline = stream.ReadBool();

			Data = stream.Subsection(stream.Cursor, stream.Cursor + bitCount);
			stream.Seek(bitCount, System.IO.SeekOrigin.Current);
		}

		uint ReadUBitInt(BitStream stream)
		{
			uint ret = stream.ReadUInt(6);
			switch (ret & (16 | 32))
			{
				case 16:
				ret = (ret & 15) | (stream.ReadUInt(4) << 4);
				break;
				case 32:
				ret = (ret & 15) | (stream.ReadUInt(8) << 4);
				break;
				case 48:
				ret = (ret & 15) | (stream.ReadUInt(32 - 4) << 4);
				break;
			}
			return ret;
		}

		public void ApplyWorldState(WorldState ws)
		{
			Data.Seek(0, System.IO.SeekOrigin.Begin);

			int currentEntity = -1;
			for (int i = 0; i < UpdatedEntries; i++)
			{
				currentEntity += 1 + (int)ReadUBitInt(Data);

				// Leave PVS flag
				if (!Data.ReadBool())
				{
					// Enter PVS flag
					if (Data.ReadBool())
					{
						ws.Entities.
					}
				}
			}
			
		}

		static Entity ReadEnterPVS(BitStream stream)
		{
			throw new NotImplementedException();
		}
	}
}
