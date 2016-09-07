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
	class NetGameEventListMessage : INetMessage
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		ulong BitCount { get; set; }
		//public byte[] Data { get; set; }

		[DebuggerDisplay("Game event: {Name}")]
		public class GameEvent
		{
			public int ID { get; set; }
			public string Name { get; set; }

			public enum Type
			{
				Local = 0, // not networked
				String,    // zero terminated ASCII string
				Float,     // float 32 bit
				Long,      // signed int 32 bit
				Short,     // signed int 16 bit
				Byte,      // unsigned int 8 bit
				Bool,      // unsigned int 1 bit
			};

			public IDictionary<string, Type> Values { get; set; }
		}
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

		public void ReadMsg(byte[] buffer, ref ulong bitOffset)
		{
			ushort eventsCount = (ushort)BitReader.ReadUIntBits(buffer, ref bitOffset, SourceConstants.MAX_EVENT_BITS);

			BitCount = BitReader.ReadUIntBits(buffer, ref bitOffset, 20);
			//Data = new byte[BitInfo.BitsToBytes(BitCount)];
			//BitReader.CopyBits(buffer, BitCount, ref bitOffset, Data);

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
