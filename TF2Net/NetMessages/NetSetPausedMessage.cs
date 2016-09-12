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
	public class NetSetPausedMessage : INetMessage
	{
		public bool Paused { get; set; }

		public string Description
		{
			get
			{
				return string.Format("svc_SetPause: {0}", Paused ? "Paused" : "Unpaused");
			}
		}

		public void ReadMsg(BitStream stream, IReadOnlyWorldState ws)
		{
			Paused = stream.ReadBool();
		}

		public void ApplyWorldState(WorldState ws)
		{
			throw new NotImplementedException();
		}
	}
}
