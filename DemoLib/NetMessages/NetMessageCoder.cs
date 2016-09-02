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
			ulong cursor = 0;
			while (cursor < ((ulong)buffer.LongLength * 8) - SourceConstants.NETMSG_TYPE_BITS)
			{
				NetMessageType type = (NetMessageType)BitReader.ReadUInt(buffer, ref cursor, SourceConstants.NETMSG_TYPE_BITS);

				switch (type)
				{
					default:	throw new NotImplementedException(string.Format("Unimplemented {0} \"{1}\"", typeof(NetMessageType).Name, type));
				}
			}

			throw new NotImplementedException();
		}
	}
}
