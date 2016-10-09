using System;
using System.Diagnostics;
using BitSet;
using TF2Net.Data;

namespace TF2Net.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	public class NetUsrMsgMessage : INetMessage
	{
		const int MAX_USER_MSG_TYPE_BITS = 8;

		public int MessageType { get; set; }

		public BitStream Data { get; set; }

		public string Description
		{
			get
			{
				return string.Format("svc_UserMessage: type {0}, bytes {1}", MessageType, BitInfo.BitsToBytes(Data.Length));
			}
		}

		public void ReadMsg(BitStream stream)
		{
			MessageType = stream.ReadInt(MAX_USER_MSG_TYPE_BITS);

			ulong bitCount = stream.ReadULong(SourceConstants.MAX_USER_MSG_LENGTH_BITS);
			Data = stream.Subsection(stream.Cursor, stream.Cursor + bitCount);
			stream.Seek(bitCount, System.IO.SeekOrigin.Current);
		}

		public void ApplyWorldState(WorldState ws)
		{
			Console.WriteLine("hi");
			//throw new NotImplementedException();
		}
	}
}
