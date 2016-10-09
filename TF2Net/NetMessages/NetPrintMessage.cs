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
	public class NetPrintMessage : INetMessage
	{
		public string Message { get; set; }

		public string Description
		{
			get
			{
				return string.Format("svc_Print: \"{0}\"", Message);
			}
		}

		public void ReadMsg(BitStream stream)
		{
			Message = stream.ReadCString();
		}

		public void ApplyWorldState(WorldState ws)
		{
			ws.Listeners.ServerTextMessage.Invoke(ws, Message);
		}
	}
}
