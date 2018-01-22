using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitSet;
using TF2Net.Data;
using TF2Net.NetMessages;

namespace TF2Net
{
	public static class NetMessageCoder
	{
		public static List<INetMessage> Decode(BitStream stream)
		{
			List<INetMessage> messages = new List<INetMessage>();
			
			while (stream.Cursor < (stream.Length - SourceConstants.NETMSG_TYPE_BITS))
			{
				NetMessageType type = (NetMessageType)stream.ReadByte(SourceConstants.NETMSG_TYPE_BITS);

				if (type == NetMessageType.NET_NOOP)
					continue;

				INetMessage newMsg = CreateNetMessage(type);
				newMsg.ReadMsg(stream);
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
			    case NetMessageType.SVC_BSPDECAL:           return new NetBspDecalMessage();

                case NetMessageType.SVC_USERMESSAGE:		return new NetUsrMsgMessage();

				case NetMessageType.SVC_ENTITYMESSAGE:		return new NetEntityMessage();
				case NetMessageType.SVC_GAMEEVENT:			return new NetGameEventMessage();
				case NetMessageType.SVC_PACKETENTITIES:		return new NetPacketEntitiesMessage();
				case NetMessageType.SVC_TEMPENTITIES:		return new NetTempEntityMessage();
				case NetMessageType.SVC_PREFETCH:			return new NetPrefetchMessage();

				case NetMessageType.SVC_GAMEEVENTLIST:		return new NetGameEventListMessage();

				case NetMessageType.SVC_UNKNKOWN_34:		return new SomeBitMessage(10);

				default:	throw new NotImplementedException(string.Format("Unimplemented {0} \"{1}\"", typeof(NetMessageType).Name, type));
			}
		}
	}
}
