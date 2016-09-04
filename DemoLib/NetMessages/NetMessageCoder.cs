using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitSet;

namespace DemoLib.NetMessages
{
	static class NetMessageCoder
	{
		public static List<INetMessage> Decode(byte[] buffer)
		{
			List<INetMessage> messages = new List<INetMessage>();

			ulong cursor = 0;
			while (cursor < ((ulong)buffer.LongLength * 8) - SourceConstants.NETMSG_TYPE_BITS)
			{
				NetMessageType type = (NetMessageType)BitReader.ReadUInt(buffer, ref cursor, SourceConstants.NETMSG_TYPE_BITS);

				INetMessage newMsg = CreateNetMessage(type);
				newMsg.ReadMsg(buffer, ref cursor);
				messages.Add(newMsg);
			}

			throw new NotImplementedException();
		}

		static INetMessage CreateNetMessage(NetMessageType type)
		{
			switch (type)
			{
				case NetMessageType.NET_TICK:				return new NetTickMessage();

				case NetMessageType.SVC_UPDATESTRINGTABLE:	return new NetUpdateStringTableMessage();

				default:	throw new NotImplementedException(string.Format("Unimplemented {0} \"{1}\"", typeof(NetMessageType).Name, type));
			}
		}
	}
}
