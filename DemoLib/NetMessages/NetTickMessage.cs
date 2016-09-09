using System;
using System.Diagnostics;
using BitSet;

namespace DemoLib.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	class NetTickMessage : INetMessage
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

		public ulong Size
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public NetMessageType Type { get { return NetMessageType.NET_TICK; } }

		public void ReadMsg(DemoReader reader, byte[] buffer, ref ulong bitOffset)
		{
			Tick = (uint)BitReader.ReadUIntBits(buffer, ref bitOffset, TICK_BITS);

			HostFrameTime = BitReader.ReadUIntBits(buffer, ref bitOffset, FLOAT_BITS) / NET_TICK_SCALEUP;
			HostFrameTimeStdDev = BitReader.ReadUIntBits(buffer, ref bitOffset, FLOAT_BITS) / NET_TICK_SCALEUP;
		}

		public void WriteMsg(byte[] buffer, ref ulong bitOffset)
		{
			throw new NotImplementedException();
		}
	}
}
