using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitSet;

namespace DemoLib.NetMessages
{
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
				throw new NotImplementedException();
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

		public void ReadMsg(byte[] buffer, ref ulong bitOffset)
		{
			Tick = (uint)BitReader.ReadUInt(buffer, ref bitOffset, TICK_BITS);

			HostFrameTime = BitReader.ReadUInt(buffer, ref bitOffset, FLOAT_BITS) / NET_TICK_SCALEUP;
			HostFrameTimeStdDev = BitReader.ReadUInt(buffer, ref bitOffset, FLOAT_BITS) / NET_TICK_SCALEUP;
		}

		public void WriteMsg(byte[] buffer, ref ulong bitOffset)
		{
			throw new NotImplementedException();
		}
	}
}
