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
	public class NetBSPDecalMessage : INetMessage
	{
		public string Description
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public void ApplyWorldState(WorldState ws)
		{
			throw new NotImplementedException();
		}

		public void ReadMsg(BitStream stream)
		{
			throw new NotImplementedException();
		}
	}
}
