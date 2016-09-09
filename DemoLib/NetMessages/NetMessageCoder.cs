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
		public static List<INetMessage> Decode(DemoReader reader, byte[] buffer)
		{
			List<INetMessage> messages = new List<INetMessage>();

			ulong cursor = 0;
			while (cursor < ((ulong)buffer.LongLength * 8) - SourceConstants.NETMSG_TYPE_BITS)
			{
				NetMessageType type = (NetMessageType)BitReader.ReadUIntBits(buffer, ref cursor, SourceConstants.NETMSG_TYPE_BITS);

				if (type == NetMessageType.NET_NOOP)
					continue;

				INetMessage newMsg = CreateNetMessage(type);
				newMsg.ReadMsg(reader, buffer, ref cursor);
				messages.Add(newMsg);
			}

			return messages;
		}

		static INetMessage CreateNetMessage(NetMessageType type)
		{
			switch (type)
			{
				case NetMessageType.NET_FILE:				return new NetFileMessage();
				case NetMessageType.NET_TICK:				return new NetTickMessage();
				case NetMessageType.NET_STRINGCMD:			return new NetStringCmdMessage();
				case NetMessageType.NET_SETCONVAR:			return new NetSetConvarMessage();
				case NetMessageType.NET_SIGNONSTATE:		return new NetSignonStateMessage();
				case NetMessageType.SVC_PRINT:				return new NetPrintMessage();
				case NetMessageType.SVC_SERVERINFO:			return new NetServerInfoMessage();

				case NetMessageType.SVC_CLASSINFO:			return new NetClassInfoMessage();
				case NetMessageType.SVC_SETPAUSE:			return new NetSetPausedMessage();
				case NetMessageType.SVC_CREATESTRINGTABLE:	return new NetCreateStringTableMessage();
				case NetMessageType.SVC_UPDATESTRINGTABLE:	return new NetUpdateStringTableMessage();
				case NetMessageType.SVC_VOICEINIT:			return new NetVoiceInitMessage();
				case NetMessageType.SVC_VOICEDATA:			return new NetVoiceDataMessage();

				case NetMessageType.SVC_SOUND:				return new NetSoundMessage();
				case NetMessageType.SVC_SETVIEW:			return new NetSetViewMessage();
				case NetMessageType.SVC_FIXANGLE:			return new NetFixAngleMessage();

				case NetMessageType.SVC_USERMESSAGE:		return new NetUsrMsgMessage();

				case NetMessageType.SVC_ENTITYMESSAGE:		return new NetEntityMessage();
				case NetMessageType.SVC_GAMEEVENT:			return new NetGameEventMessage();
				case NetMessageType.SVC_PACKETENTITIES:		return new NetPacketEntitiesMessage();
				case NetMessageType.SVC_TEMPENTITIES:		return new NetTempEntityMessage();
				case NetMessageType.SVC_PREFETCH:			return new NetPrefetchMessage();

				case NetMessageType.SVC_GAMEEVENTLIST:		return new NetGameEventListMessage();

				default:	throw new NotImplementedException(string.Format("Unimplemented {0} \"{1}\"", typeof(NetMessageType).Name, type));
			}
		}
	}
}
