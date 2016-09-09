using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitSet;

namespace DemoLib.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	class NetSetPausedMessage : INetMessage
	{
		public bool Paused { get; set; }

		public string Description
		{
			get
			{
				return string.Format("svc_SetPause: {0}", Paused ? "Paused" : "Unpaused");
			}
		}

		public ulong Size
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public void ReadMsg(DemoReader reader, byte[] buffer, ref ulong bitOffset)
		{
			Paused = BitReader.ReadUInt1(buffer, ref bitOffset) != 0;
		}

		public void WriteMsg(byte[] buffer, ref ulong bitOffset)
		{
			throw new NotImplementedException();
		}
	}
}
