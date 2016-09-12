using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitSet;
using TF2Net.Data;

namespace TF2Net.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	public class NetSetViewMessage : INetMessage
	{
		public ushort EntityIndex { get; set; }

		public string Description
		{
			get
			{
				return string.Format("svc_SetView: view entity {0}", EntityIndex);
			}
		}
		
		public void ReadMsg(BitStream stream, IReadOnlyWorldState ws)
		{
			EntityIndex = stream.ReadUShort(SourceConstants.MAX_EDICT_BITS);
		}

		public void ApplyWorldState(WorldState ws)
		{
			throw new NotImplementedException();
		}
	}
}
