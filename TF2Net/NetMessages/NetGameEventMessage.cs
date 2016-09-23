using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BitSet;
using TF2Net.Data;

namespace TF2Net.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	public class NetGameEventMessage : INetMessage
	{
		const int EVENT_LENGTH_BITS = 11;
		
		public BitStream Data { get; set; }

		public string Description
		{
			get
			{
				return string.Format("svc_GameEvent: bytes {0}", BitInfo.BitsToBytes(Data.Length));
			}
		}

		public void ReadMsg(BitStream stream)
		{
			ulong bitCount = stream.ReadULong(EVENT_LENGTH_BITS);
			Data = stream.Subsection(stream.Cursor, stream.Cursor + bitCount);
			stream.Seek(bitCount, SeekOrigin.Current);
		}

		public void ApplyWorldState(WorldState ws)
		{
			GameEvent retVal = new GameEvent();

			int eventID = Data.ReadInt(SourceConstants.MAX_EVENT_BITS);

			retVal.Declaration = ws.EventDeclarations.Single(g => g.ID == eventID);

			retVal.Values = new Dictionary<string, object>();
			foreach (var value in retVal.Declaration.Values)
			{
				switch (value.Value)
				{
					case GameEventDataType.Local: break;
					case GameEventDataType.Bool: retVal.Values.Add(value.Key, Data.ReadBool()); break;
					case GameEventDataType.Byte: retVal.Values.Add(value.Key, Data.ReadByte()); break;
					case GameEventDataType.Float: retVal.Values.Add(value.Key, Data.ReadSingle()); break;
					case GameEventDataType.Long: retVal.Values.Add(value.Key, Data.ReadInt()); break;
					case GameEventDataType.Short: retVal.Values.Add(value.Key, Data.ReadShort()); break;
					case GameEventDataType.String: retVal.Values.Add(value.Key, Data.ReadCString()); break;

					default:
					throw new FormatException("Invalid GameEvent type");
				}
			}

			ws.Listeners.OnGameEvent(ws, retVal);
		}
	}
}
