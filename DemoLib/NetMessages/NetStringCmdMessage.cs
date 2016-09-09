using System;
using System.Diagnostics;
using BitSet;

namespace DemoLib.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	class NetStringCmdMessage : INetMessage
	{
		public string Command { get; set; }

		public string Description
		{
			get
			{
				return string.Format("net_StringCmd: \"{0}\"", Command);
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
			Command = BitReader.ReadCString(buffer, ref bitOffset);
		}

		public void WriteMsg(byte[] buffer, ref ulong bitOffset)
		{
			throw new NotImplementedException();
		}
	}
}
