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
	public class NetSignonStateMessage : INetMessage
	{
		public enum SignonState : byte
		{
			/// <summary>
			/// no state yet, about to connect
			/// </summary>
			None = 0,

			/// <summary>
			/// client challenging server, all OOB packets
			/// </summary>
			Challenge = 1,

			/// <summary>
			/// client is connected to server, netchans ready
			/// </summary>
			Connected = 2,

			/// <summary>
			/// just got serverinfo and string tables
			/// </summary>
			New = 3,

			/// <summary>
			/// received signon buffers
			/// </summary>
			Prespawn = 4,

			/// <summary>
			/// ready to receive entity packets
			/// </summary>
			Spawn = 5,

			/// <summary>
			/// we are fully connected, first non-delta packet received
			/// </summary>
			Full = 6,

			/// <summary>
			/// server is changing level, please wait
			/// </summary>
			Changelevel = 7,
		}

		public SignonState State { get; set; }
		public int SpawnCount { get; set; }

		public string Description
		{
			get
			{
				return string.Format("net_SignonState: state {0}, count {1}", State, SpawnCount);
			}
		}

		public void ReadMsg(BitStream stream, IReadOnlyWorldState ws)
		{
			State = (SignonState)stream.ReadByte();

			SpawnCount = stream.ReadInt();
		}

		public void ApplyWorldState(WorldState ws)
		{
			throw new NotImplementedException();
		}
	}
}
