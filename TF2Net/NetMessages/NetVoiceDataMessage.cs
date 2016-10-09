using System;
using System.Diagnostics;
using BitSet;
using TF2Net.Data;

namespace TF2Net.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	public class NetVoiceDataMessage : INetMessage
	{
		public byte ClientIndex { get; set; }
		public bool Proximity { get; set; }
		
		public BitStream Data { get; set; }

		public string Description
		{
			get
			{
				return string.Format("svc_VoiceData: client {0}, bytes {1}",
					ClientIndex, BitInfo.BitsToBytes(Data.Length));
			}
		}

		public void ReadMsg(BitStream stream)
		{
			ClientIndex = stream.ReadByte();
			Proximity = stream.ReadByte() != 0;

			ulong bitCount = stream.ReadULong(16);
			Data = stream.Subsection(stream.Cursor, stream.Cursor + bitCount);
			stream.Seek(bitCount, System.IO.SeekOrigin.Current);
		}

		public void ApplyWorldState(WorldState ws)
		{
		}
	}
}
