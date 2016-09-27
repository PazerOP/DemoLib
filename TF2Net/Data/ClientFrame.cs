using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TF2Net.Data
{
	[DebuggerDisplay("ClientFrame: tick {ServerTick,nq}")]
	public class ClientFrame
	{
		public ClientFrame(ulong tick)
		{
			ServerTick = tick;
		}

		public int LastEntityIndex { get; set; }
		public ulong ServerTick { get; }

		public BitArray TransmitEntity { get; } = new BitArray(SourceConstants.MAX_EDICTS);
		public BitArray FromBaseline { get; } = new BitArray(SourceConstants.MAX_EDICTS);
		public BitArray TransmitAlways { get; } = new BitArray(SourceConstants.MAX_EDICTS);
	}
}
