using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BitSet;
using DemoLib.DataExtraction;

namespace DemoLib.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	class NetGameEventMessage : INetMessage
	{
		const int EVENT_LENGTH_BITS = 11;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		ulong m_BitCount;
		
		public GameEvent EventDeclaration { get; set; }

		public IDictionary<string, object> Values { get; set; } = new Dictionary<string, object>();

		public string Description
		{
			get
			{
				return string.Format("svc_GameEvent: bytes {0}", BitInfo.BitsToBytes(m_BitCount));
			}
		}

		public ulong Size
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public NetMessageType Type { get { return NetMessageType.SVC_GAMEEVENT; } }

		public void ReadMsg(DemoReader reader, byte[] buffer, ref ulong bitOffset)
		{
			m_BitCount = BitReader.ReadUIntBits(buffer, ref bitOffset, EVENT_LENGTH_BITS);
			var startBit = bitOffset;

			int eventID = (int)BitReader.ReadUIntBits(buffer, ref bitOffset, SourceConstants.MAX_EVENT_BITS);

			EventDeclaration = reader.GameEventDeclarations.Single(g => g.ID == eventID);

			foreach (var value in EventDeclaration.Values)
			{
				switch (value.Value)
				{
					case GameEvent.Type.Local:		break;
					case GameEvent.Type.Bool:		Values.Add(value.Key, BitReader.ReadBool(buffer, ref bitOffset)); break;
					case GameEvent.Type.Byte:		Values.Add(value.Key, BitReader.ReadByte(buffer, ref bitOffset)); break;
					case GameEvent.Type.Float:		Values.Add(value.Key, BitReader.ReadSingle(buffer, ref bitOffset)); break;
					case GameEvent.Type.Long:		Values.Add(value.Key, BitReader.ReadInt(buffer, ref bitOffset)); break;
					case GameEvent.Type.Short:		Values.Add(value.Key, BitReader.ReadShort(buffer, ref bitOffset)); break;
					case GameEvent.Type.String:		Values.Add(value.Key, BitReader.ReadCString(buffer, ref bitOffset)); break;

					default:
					throw new DemoParseException("Invalid GameEvent type");
				}
			}
		}

		public void WriteMsg(byte[] buffer, ref ulong bitOffset)
		{
			throw new NotImplementedException();
		}
	}
}
