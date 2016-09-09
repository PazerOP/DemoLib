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
	class NetSetViewMessage : INetMessage
	{
		public ushort EntityIndex { get; set; }

		public string Description
		{
			get
			{
				return string.Format("svc_SetView: view entity {0}", EntityIndex);
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
			EntityIndex = (ushort)BitReader.ReadUIntBits(buffer, ref bitOffset, SourceConstants.MAX_EDICT_BITS);
		}

		public void WriteMsg(byte[] buffer, ref ulong bitOffset)
		{
			throw new NotImplementedException();
		}
	}
}
