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
	public class NetSignonStateMessage : INetMessage
	{
		public SignonState State { get; set; }

		public string Description
		{
			get
			{
				return string.Format("net_SignonState: state {0}, count {1}", State.State, State.SpawnCount);
			}
		}

		public void ReadMsg(BitStream stream)
		{
			State = new SignonState();

			State.State = (ConnectionState)stream.ReadByte();
			State.SpawnCount = stream.ReadInt();
		}

		public void ApplyWorldState(WorldState ws)
		{
			ws.SignonState = State;
		}
	}
}
