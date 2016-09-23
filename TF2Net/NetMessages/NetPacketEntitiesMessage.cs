using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
				currentEntity += 1 + (int)ReadUBitVar(Data);

				// Leave PVS flag
				if (!Data.ReadBool())
				{
					// Enter PVS flag
					if (Data.ReadBool())
					{
						Entity e = ReadEnterPVS(ws, Data, (uint)currentEntity);

						ApplyEntityUpdate(e, Data);

						Debug.Assert(!ws.Entities.Any(x => x.Index == e.Index));
						ws.Entities.Add(e);
					}
					else
					{
						// Preserve/update
						Entity e = ws.Entities.Single(ent => ent.Index == currentEntity);
						ApplyEntityUpdate(e, Data);
					}
				}
				else
				{
					if (Data.ReadBool())
						throw new NotImplementedException("Force delete");
					else
						throw new NotImplementedException("Leave PVS");

					var removed = ws.Entities.RemoveWhere(e => e.Index == currentEntity);
					Debug.Assert(removed == 1);

					Data.Cursor++;
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

			//ulong start = stream.Cursor;
			//ulong test = stream.Cursor;
			//while (stream.ReadULong(ws.ClassBits) != 306)
			//	stream.Cursor = ++test;
			//stream.Cursor = start;
			//stream = stream.Subsection(stream.Cursor, test);

			StringTable instanceBaselines = ws.StringTables.Single(st => st.TableName == "instancebaseline");

			/*int index = 0;
			foreach (var bl in instanceBaselines.Entries.OrderBy(ib => ib.UserData.Length))
			{
				Entity testEnt = new Entity(uint.MaxValue, uint.MaxValue);
				testEnt.Class = ws.ServerClasses[int.Parse(bl.Value)];
				testEnt.NetworkTable = ws.SendTables.Single(st => st.NetTableName == testEnt.Class.DatatableName);
				ApplyEntityUpdate(testEnt, bl.UserData);
				index++;
			}*/

			StringTableEntry baseline = instanceBaselines.Entries.SingleOrDefault(ib => uint.Parse(ib.Value) == serverClassID);
			if (baseline != null)
			{
				baseline.UserData.Cursor = 0;
				ApplyEntityUpdate(e, baseline.UserData);
				Debug.Assert((baseline.UserData.Length - baseline.UserData.Cursor) < 8);
			}

			return e;
		}

		static void ApplyEntityUpdate(Entity e, BitStream stream)
		{
			//bool newWay = stream.ReadBool();

			var guessProps = e.NetworkTable.SortedProperties.ToArray();

			//var nUnknownLength = guessProps.Where(p => p.BitCount.HasValue && p.BitCount != 0);
			//var nStrings = guessProps.Where(p => p.Type != SendPropType.String);
			//var props = nUnknownLength.Concat(nStrings.Concat(guessProps)).Distinct().ToArray();
			//SendProp[] props = e.NetworkTable.Properties.ToArray();

			//var test = BruteForce(stream, guessProps);

			//Debug.Fail("BruteForce finished");

			int index = -1;
			while ((index = ReadFieldIndex(stream, index)) != -1)
			{
				Debug.Assert(index < guessProps.Length);
				Debug.Assert(index < SourceConstants.MAX_DATATABLE_PROPS);

				var prop = guessProps[index];

				var decoded = prop.Decode(stream);

				e.Properties[prop] = decoded;
				guessProps[index] = null;
			}
		}

		[DebuggerDisplay("[{HighestDepth,nq}] {Property,nq} = {Value}")]
		class Node
		{
			/// <summary>
			/// If we have multiple paths with the same depth.
			/// </summary>
			public List<Node> ChildNodes { get; set; }

			public int FieldIndex { get; set; }
			
			public SendProp Property { get; set; }

			public ulong BitsRead { get; set; }
			public object Value { get; set; }

			public BitStream Stream { get; set; }

			public IEnumerable<SendProp> RemainingProps { get; set; }

			public int HighestDepth
			{
				get
				{
					int highest = 0;

					foreach (var node in ChildNodes)
						highest = Math.Max(highest, node.HighestDepth + 1);

					return highest;
				}
			}
		}

		static ulong cursor = 0;

		static List<Node> BruteForce(BitStream stream, IEnumerable<SendProp> remainingProps)
		{
			List<Node> retVal = new List<Node>();

			var outerStartPos = stream.Cursor;

			int fieldIndex = -1;
			while ((fieldIndex = ReadFieldIndex(stream, fieldIndex)) != -1)
			{
				var startPos = stream.Cursor;
				foreach (SendProp prop in remainingProps)
				{
					try
					{
						bool passedTest = false;
						if (prop.BitCount.HasValue && prop.BitCount != 0 &&
							(prop.Type == SendPropType.Float ||
							prop.Type == SendPropType.Int))
						{
							stream.Cursor += prop.BitCount.Value;

							if (!stream.ReadBool())
								continue;

							passedTest = true;
							stream.Cursor = startPos;
						}

						ulong startReadBit = stream.Cursor;
						if (prop.Type == SendPropType.String)
						{
							var length = stream.ReadULong(9);
							if ((length * 8) > (stream.Length - stream.Cursor))
								continue;
							else
								stream.Cursor = startReadBit;
						}

						//var streamCopy = stream.Clone();

						object decodeTest = prop.Decode(stream);

						if (!passedTest)
						{
							if (!stream.PeekBool())
								continue;
						}
						
						Node newNode = new Node();
						newNode.Property = prop;
						newNode.Value = decodeTest;
						newNode.FieldIndex = fieldIndex;
						newNode.Stream = stream.Clone();
						newNode.BitsRead = stream.Cursor - startReadBit;
						newNode.RemainingProps = remainingProps;
						
						newNode.ChildNodes = BruteForce(stream, remainingProps.Except(prop));

						retVal.Add(newNode);
					}
					catch (OverflowException) { }
					catch (FormatException) { }
					finally
					{
						stream.Cursor = startPos;
					}
				}
			}

			stream.Cursor = outerStartPos;
			return retVal;
		}

		static int ReadFieldIndex(BitStream stream, int lastIndex)
		{
#if true
			if (!stream.ReadBool())
				return -1;
			
			var diff = ReadUBitVar(stream);
			return (int)(lastIndex + diff + 1);
#else
			if (stream.ReadBool())
				return lastIndex + 1;

			int ret = 0;
			if (stream.ReadBool())
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
#endif
		}
	}
}
