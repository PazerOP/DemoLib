using System;
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

		public GameEvent Event { get; set; }

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

		public void ReadMsg(DemoReader reader, uint? serverTick, byte[] buffer, ref ulong bitOffset)
		{
			m_BitCount = BitReader.ReadUIntBits(buffer, ref bitOffset, EVENT_LENGTH_BITS);
			var startBit = bitOffset;

			Event = new GameEvent();

			int eventID = (int)BitReader.ReadUIntBits(buffer, ref bitOffset, SourceConstants.MAX_EVENT_BITS);

			Event.Declaration = reader.GameEventDeclarations.Single(g => g.ID == eventID);

			foreach (var value in Event.Declaration.Values)
			{
				switch (value.Value)
				{
					case GameEventDataType.Local:		break;
					case GameEventDataType.Bool:		Event.Values.Add(value.Key, BitReader.ReadBool(buffer, ref bitOffset)); break;
					case GameEventDataType.Byte:		Event.Values.Add(value.Key, BitReader.ReadByte(buffer, ref bitOffset)); break;
					case GameEventDataType.Float:		Event.Values.Add(value.Key, BitReader.ReadSingle(buffer, ref bitOffset)); break;
					case GameEventDataType.Long:		Event.Values.Add(value.Key, BitReader.ReadInt(buffer, ref bitOffset)); break;
					case GameEventDataType.Short:		Event.Values.Add(value.Key, BitReader.ReadShort(buffer, ref bitOffset)); break;
					case GameEventDataType.String:		Event.Values.Add(value.Key, BitReader.ReadCString(buffer, ref bitOffset)); break;

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
