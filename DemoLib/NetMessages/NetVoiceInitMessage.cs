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
	class NetVoiceInitMessage : INetMessage
	{
		public string VoiceCodec { get; set; }
		public byte Quality { get; set; }

		public string Description
		{
			get
			{
				return string.Format("svc_VoiceInit: codec \"{0}\", qualitty {1}",
					VoiceCodec, Quality);
			}
		}

		public ulong Size
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public void ReadMsg(DemoReader reader, uint? serverTick, byte[] buffer, ref ulong bitOffset)
		{
			VoiceCodec = BitReader.ReadCString(buffer, ref bitOffset);
			Quality = BitReader.ReadByte(buffer, ref bitOffset);
		}

		public void WriteMsg(byte[] buffer, ref ulong bitOffset)
		{
			throw new NotImplementedException();
		}
	}
}
