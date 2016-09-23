using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitSet;
using TF2Net.Data;

namespace TF2Net.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	public class NetTempEntityMessage : INetMessage
	{
		public int EntryCount { get; set; }

		public BitStream Data { get; set; }

		public string Description
		{
			get
			{
				return string.Format("svc_TempEntities: number {0}, bytes {1}", EntryCount, BitInfo.BitsToBytes(Data.Length));
			}
		}

		public void ReadMsg(BitStream stream)
		{
			EntryCount = stream.ReadInt(SourceConstants.EVENT_INDEX_BITS);

			ulong bitCount = stream.ReadVarInt();

			Data = stream.Subsection(stream.Cursor, stream.Cursor + bitCount);
			stream.Seek(bitCount, System.IO.SeekOrigin.Current);
		}

		public void ApplyWorldState(WorldState ws)
		{
		}
	}
}
