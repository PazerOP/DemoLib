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
	public class NetEntityMessage : INetMessage
	{
		const int DATA_LENGTH_BITS = 11;

		public uint EntityIndex { get; set; }
		public uint ClassID { get; set; }

		public BitStream Data { get; set; }

		public string Description
		{
			get
			{
				return string.Format("svc_EntityMessage: entity {0}, class {1}, bytes {2}",
					EntityIndex, ClassID, BitInfo.BitsToBytes(Data.Length));
			}
		}
		public void WriteMsg(byte[] buffer, ref ulong bitOffset)
		{
			throw new NotImplementedException();
		}

		public void ReadMsg(BitStream stream)
		{
			EntityIndex = stream.ReadUInt(SourceConstants.MAX_EDICT_BITS);
			ClassID = stream.ReadUInt(SourceConstants.MAX_SERVER_CLASS_BITS);

			ulong bitCount = stream.ReadULong(DATA_LENGTH_BITS);
			Data = stream.Subsection(stream.Cursor, stream.Cursor + bitCount);
			stream.Seek(bitCount, System.IO.SeekOrigin.Current);
		}

		public void ApplyWorldState(WorldState ws)
		{
			Entity target = ws.Entities[EntityIndex];
			Console.WriteLine("hi");
		}
	}
}
