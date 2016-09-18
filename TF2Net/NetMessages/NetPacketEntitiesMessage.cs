using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BitSet;
using TF2Net.Data;

namespace TF2Net.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	public class NetPacketEntitiesMessage : INetMessage
	{
		const int DELTA_INDEX_BITS = 32;
		const int DELTA_SIZE_BITS = 20;

		public uint MaxEntries { get; set; }
		public uint UpdatedEntries { get; set; }
		public bool IsDelta { get; set; }
		public bool UpdateBaseline { get; set; }
		public bool Baseline { get; set; }
		public int? DeltaFrom { get; set; }

		public BitStream Data { get; set; }

		public string Description
		{
			get
			{
				return string.Format("svc_PacketEntities: delta {0}, max {1}, changed {2}, {3} bytes {4}",
					DeltaFrom, MaxEntries, UpdatedEntries,
					UpdateBaseline ? " BL update," : "",
					BitInfo.BitsToBytes(Data.Length));
			}
		}

		public void ReadMsg(BitStream stream)
		{
			MaxEntries = stream.ReadUInt(SourceConstants.MAX_EDICT_BITS);

			IsDelta = stream.ReadBool();
			if (IsDelta)
				DeltaFrom = stream.ReadInt(DELTA_INDEX_BITS);

			Baseline = stream.ReadBool();

			UpdatedEntries = stream.ReadUInt(SourceConstants.MAX_EDICT_BITS);

			ulong bitCount = stream.ReadULong(DELTA_SIZE_BITS);

			UpdateBaseline = stream.ReadBool();

			Data = stream.Subsection(stream.Cursor, stream.Cursor + bitCount);
			stream.Seek(bitCount, System.IO.SeekOrigin.Current);
		}

		static uint ReadUBitInt(BitStream stream)
		{
			uint ret = stream.ReadUInt(6);
			switch (ret & (16 | 32))
			{
				case 16:
				ret = (ret & 15) | (stream.ReadUInt(4) << 4);
				break;
				case 32:
				ret = (ret & 15) | (stream.ReadUInt(8) << 4);
				break;
				case 48:
				ret = (ret & 15) | (stream.ReadUInt(32 - 4) << 4);
				break;
			}
			return ret;
		}

		static uint ReadUBitVar(BitStream stream)
		{
			switch (stream.ReadByte(2))
			{
				case 0:		return stream.ReadUInt(4);
				case 1:		return stream.ReadUInt(8);
				case 2:		return stream.ReadUInt(12);
				case 3:		return stream.ReadUInt(32);
			}

			throw new Exception("Should never get here...");
		}

		public void ApplyWorldState(WorldState ws)
		{
			Data.Seek(0, System.IO.SeekOrigin.Begin);

			int currentEntity = -1;
			for (int i = 0; i < UpdatedEntries; i++)
			{
				currentEntity += 1 + (int)ReadUBitInt(Data);

				// Leave PVS flag
				if (!Data.ReadBool())
				{
					// Enter PVS flag
					if (Data.ReadBool())
					{
						Entity e = ReadEnterPVS(ws, Data, (uint)currentEntity);

						Debug.Assert(!ws.Entities.Any(x => x.Index == e.Index));
						ws.Entities.Add(e);
					}
				}
			}
			
		}

		static Entity ReadEnterPVS(WorldState ws, BitStream stream, uint entityIndex)
		{
			uint serverClassID = stream.ReadUInt(ws.ClassBits);
			uint serialNumber = stream.ReadUInt(SourceConstants.NUM_NETWORKED_EHANDLE_SERIAL_NUMBER_BITS);

			Entity e = new Entity(entityIndex, serialNumber);
			e.Class = ws.ServerClasses[(int)serverClassID];
			e.NetworkTable = ws.SendTables.Single(st => st.NetTableName == e.Class.DatatableName);

			ulong start = stream.Cursor;
			ulong test = stream.Cursor;
			while (stream.ReadULong(ws.ClassBits) != 306)
				stream.Cursor = ++test;
			stream.Cursor = start;
			stream = stream.Subsection(stream.Cursor, test);

			StringTable instanceBaselines = ws.StringTables.Single(st => st.TableName == "instancebaseline");
			StringTableEntry baseline = instanceBaselines.Entries.SingleOrDefault(ib => uint.Parse(ib.Value) == serverClassID);
			if (baseline != null)
				ApplyEntityUpdate(e, baseline.UserData);

			ApplyEntityUpdate(e, stream);

			return e;
		}

		static void ApplyEntityUpdate(Entity e, BitStream stream)
		{
			var props = e.NetworkTable.AllProperties.ToArray();
			//SendProp[] props = e.NetworkTable.Properties.ToArray();

			int index = -1;
			while ((index = ReadFieldIndex(stream, index, false)) != -1)
			{
				Debug.Assert(index < SourceConstants.MAX_DATATABLE_PROPS);

				var prop = props[index];

				var decoded = prop.Decode(stream);

				e.Properties.Add(prop, decoded);
				props[index] = null;
			}

			throw new NotImplementedException();
		}

		static int ReadFieldIndex(BitStream stream, int lastIndex, bool bNewWay)
		{
			if (!stream.ReadBool())
				return -1;
			
			var diff = ReadUBitVar(stream);
			return (int)(lastIndex + diff + 1);

			if (bNewWay)
			{
				if (stream.ReadBool())
					return lastIndex + 1;
			}

			int ret = 0;
			if (bNewWay && stream.ReadBool())
			{
				ret = stream.ReadInt(3);  // read 3 bits
			}
			else
			{
				ret = stream.ReadInt(7); // read 7 bits
				switch (ret & (32 | 64))
				{
					case 32:
					ret = (ret & ~96) | stream.ReadInt(2) << 5;
					break;
					case 64:
					ret = (ret & ~96) | stream.ReadInt(4) << 5;
					break;
					case 96:
					ret = (ret & ~96) | stream.ReadInt(7) << 5;
					break;
				}
			}

			if (ret == 0xFFF)
			{ // end marker is 4095 for cs:go
				return -1;
			}

			return lastIndex + 1 + ret;
		}
	}
}
