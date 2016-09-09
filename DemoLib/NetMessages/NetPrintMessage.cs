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
	class NetPrintMessage : INetMessage
	{
		public string Message { get; set; }

		public string Description
		{
			get
			{
				return string.Format("svc_Print: \"{0}\"", Message);
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
			Message = BitReader.ReadCString(buffer, ref bitOffset);
		}

		public void WriteMsg(byte[] buffer, ref ulong bitOffset)
		{
			throw new NotImplementedException();
		}
	}
}
