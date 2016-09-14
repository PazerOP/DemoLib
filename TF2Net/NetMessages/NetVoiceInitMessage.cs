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
	public class NetVoiceInitMessage : INetMessage
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

		public void ReadMsg(BitStream stream)
		{
			VoiceCodec = stream.ReadCString();
			Quality = stream.ReadByte();
		}

		public void ApplyWorldState(WorldState ws)
		{
			//throw new NotImplementedException();
		}
	}
}
