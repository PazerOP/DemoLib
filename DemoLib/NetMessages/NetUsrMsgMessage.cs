using System;
using System.Diagnostics;
using BitSet;

namespace DemoLib.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	class NetUsrMsgMessage : INetMessage
	{
		const int MAX_USER_MSG_TYPE_BITS = 8;

		public int MessageType { get; set; }

		public ulong BitCount { get; set; }
		public byte[] Data { get; set; }

		public string Description
		{
			get
			{
				return string.Format("svc_UserMessage: type {0}, bytes {1}", MessageType, BitInfo.BitsToBytes(BitCount));
			}
		}

		public ulong Size
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public NetMessageType Type { get { return NetMessageType.SVC_USERMESSAGE; } }

		public void ReadMsg(DemoReader reader, uint? serverTick, byte[] buffer, ref ulong bitOffset)
		{
			MessageType = (int)BitReader.ReadUIntBits(buffer, ref bitOffset, MAX_USER_MSG_TYPE_BITS);
			BitCount = BitReader.ReadUIntBits(buffer, ref bitOffset, SourceConstants.MAX_USER_MSG_LENGTH_BITS);

			Data = new byte[BitInfo.BitsToBytes(BitCount)];
			BitReader.CopyBits(buffer, BitCount, ref bitOffset, Data);
		}

		public void WriteMsg(byte[] buffer, ref ulong bitOffset)
		{
			throw new NotImplementedException();
		}
	}
}
