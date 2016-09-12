using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitSet;

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

		public void ReadMsg(BitStream stream, IReadOnlyWorldState ws)
		{
			Message = stream.ReadCString();
		}

		public void ApplyWorldState(WorldState ws)
		{
			throw new NotImplementedException();
		}
	}
}
