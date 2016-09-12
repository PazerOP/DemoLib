using System;
using System.Diagnostics;
using BitSet;

namespace TF2Net.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	public class NetTickMessage : INetMessage
	{
		const int TICK_BITS = 32;
		const int FLOAT_BITS = 16;
		const double NET_TICK_SCALEUP = 100000.0;

		public uint Tick { get; set; }
		public double HostFrameTime { get; set; }
		public double HostFrameTimeStdDev { get; set; }

		public string Description
		{
			get
			{
				return string.Format("net_Tick: tick {0}", Tick);
			}
		}

		public void ReadMsg(BitStream stream, IReadOnlyWorldState ws)
		{
			Tick = stream.ReadUInt(TICK_BITS);

			HostFrameTime = stream.ReadUInt(FLOAT_BITS) / NET_TICK_SCALEUP;
			HostFrameTimeStdDev = stream.ReadUInt(FLOAT_BITS) / NET_TICK_SCALEUP;
		}

		public void ApplyWorldState(WorldState ws)
		{
			throw new NotImplementedException();
		}
	}
}
