using System;
using System.Collections.Generic;
using System.Diagnostics;
using BitSet;
using TF2Net.Data;

namespace TF2Net.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	public class NetGameEventListMessage : INetMessage
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		ulong BitCount { get; set; }
		//public byte[] Data { get; set; }

		public IList<GameEventDeclaration> Events { get; set; }

		public string Description
		{
			get
			{
				return string.Format("svc_GameEventList: number {0}, bytes {1}",
					Events.Count, BitInfo.BitsToBytes(BitCount));
			}
		}
		
		public void ReadMsg(BitStream stream, IReadOnlyWorldState ws)
		{
			ushort eventsCount = stream.ReadUShort(SourceConstants.MAX_EVENT_BITS);

			BitCount = stream.ReadULong(20);

			Events = new List<GameEventDeclaration>();
			for (int i = 0; i < eventsCount; i++)
			{
				GameEventDeclaration e = new GameEventDeclaration();
				e.ID = stream.ReadInt(SourceConstants.MAX_EVENT_BITS);
				e.Name = stream.ReadCString();

				GameEventDataType type;

				e.Values = new Dictionary<string, GameEventDataType>();
				while ((type = (GameEventDataType)stream.ReadUShort(3)) != GameEventDataType.Local)
				{
					string name = stream.ReadCString();
					e.Values.Add(name, type);
				}

				Events.Add(e);
			}
		}

		public void ApplyWorldState(WorldState ws)
		{
			ws.EventDeclarations = Events;
		}
	}
}
