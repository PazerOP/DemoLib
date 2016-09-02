using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoLib.NetMessages
{
	enum NetMessageType : byte
	{
		NET_NOOP = 0,
		// 1
		// 2
		NET_TICK = 3,
		NET_STRINGCMD = 4,
		// 5
		// 6
		// 7
		// 8
		// 9
		// 10
		// 11	
		// 12
		SVC_UPDATESTRINGTABLE = 13,
		// 14
		// 15
		// 16
		SVC_SOUND = 17,
		// 18
		SVC_FIXANGLE = 19,
		// 20
		SVC_BSPDECAL = 21,
		// 22
		SVC_USERMESSAGE = 23,
		SVC_ENTITYMESSAGE = 24,
		SVC_GAMEEVENT = 25,
		SVC_PACKETENTITIES = 26,
		SVC_TEMPENTITIES = 27,
		SVC_PREFETCH = 28,
	}
}
