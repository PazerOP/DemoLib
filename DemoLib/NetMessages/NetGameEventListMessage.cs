using System;
using System.Collections.Generic;
using System.Diagnostics;
using BitSet;
using DemoLib.DataExtraction;

namespace DemoLib.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	class NetGameEventListMessage : INetMessage
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		ulong BitCount { get; set; }
		//public byte[] Data { get; set; }

		public IList<GameEvent> Events { get; set; }

		public string Description
		{
			get
			{
				return string.Format("svc_GameEventList: number {0}, bytes {1}",
					Events.Count, BitInfo.BitsToBytes(BitCount));
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
			ushort eventsCount = (ushort)BitReader.ReadUIntBits(buffer, ref bitOffset, SourceConstants.MAX_EVENT_BITS);

			BitCount = BitReader.ReadUIntBits(buffer, ref bitOffset, 20);

			Events = new List<GameEvent>();
			for (int i = 0; i < eventsCount; i++)
			{
				GameEvent e = new GameEvent();
				e.ID = (int)BitReader.ReadUIntBits(buffer, ref bitOffset, SourceConstants.MAX_EVENT_BITS);
				e.Name = BitReader.ReadCString(buffer, ref bitOffset);

				GameEvent.Type type;

				e.Values = new Dictionary<string, GameEvent.Type>();
				while ((type = (GameEvent.Type)BitReader.ReadUIntBits(buffer, ref bitOffset, 3)) != GameEvent.Type.Local)
				{
					string name = BitReader.ReadCString(buffer, ref bitOffset);
					e.Values.Add(name, type);
				}

				Events.Add(e);
			}
		}

		public void WriteMsg(byte[] buffer, ref ulong bitOffset)
		{
			throw new NotImplementedException();
		}
	}
}
