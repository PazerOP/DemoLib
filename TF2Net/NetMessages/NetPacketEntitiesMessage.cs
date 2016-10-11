using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BitSet;
using TF2Net.Data;
using TF2Net.Entities;
using TF2Net.NetMessages.Shared;

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
		public BaselineIndex? Baseline { get; set; }
		public int? DeltaFrom { get; set; }

		public BitStream Data { get; set; }

		public string Description
		{
			get
			{
				return string.Format("svc_PacketEntities: delta {0}, max {1}, changed {2},{3} bytes {4}",
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

			Baseline = (BaselineIndex)stream.ReadByte(1);

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

		public void ApplyWorldState(WorldState ws)
		{
			if (ws.SignonState.State == ConnectionState.Spawn)
			{
				if (!IsDelta)
				{
					// We are done with signon sequence.
					ws.SignonState.State = ConnectionState.Full;
				}
				else
					throw new InvalidOperationException("eceived delta packet entities while spawing!");
			}

			//ClientFrame newFrame = new ClientFrame(ws.Tick);
			//ws.Frames.Add(newFrame);
			//ClientFrame oldFrame = null;
			if (IsDelta)
			{
				if (ws.Tick == (ulong)DeltaFrom.Value)
					throw new InvalidDataException("Update self-referencing");

				//oldFrame = ws.Frames.Single(f => f.ServerTick == (ulong)DeltaFrom.Value);
			}

			if (UpdateBaseline)
			{
				if (Baseline.Value == BaselineIndex.Baseline0)
				{
					ws.InstanceBaselines[(int)BaselineIndex.Baseline1] = ws.InstanceBaselines[(int)BaselineIndex.Baseline0];
					ws.InstanceBaselines[(int)BaselineIndex.Baseline0] = new IList<SendProp>[SourceConstants.MAX_EDICTS];
				}
				else if (Baseline.Value == BaselineIndex.Baseline1)
				{
					ws.InstanceBaselines[(int)BaselineIndex.Baseline0] = ws.InstanceBaselines[(int)BaselineIndex.Baseline1];
					ws.InstanceBaselines[(int)BaselineIndex.Baseline1] = new IList<SendProp>[SourceConstants.MAX_EDICTS];
				}
				else
					throw new ArgumentOutOfRangeException(nameof(Baseline));
			}

			Data.Seek(0, System.IO.SeekOrigin.Begin);
			
			int newEntity = -1;
			for (int i = 0; i < UpdatedEntries; i++)
			{
				newEntity += 1 + (int)EntityCoder.ReadUBitVar(Data);

				// Leave PVS flag
				if (!Data.ReadBool())
				{
					// Enter PVS flag
					if (Data.ReadBool())
					{
						Entity e = ReadEnterPVS(ws, Data, (uint)newEntity);

						EntityCoder.ApplyEntityUpdate(e, Data);

						if (ws.Entities[e.Index] != null && !ReferenceEquals(e, ws.Entities[e.Index]))
							ws.Entities[e.Index].Dispose();

						ws.Entities[e.Index] = e;

						if (UpdateBaseline)
							ws.InstanceBaselines[Baseline.Value == BaselineIndex.Baseline0 ? 1 : 0][e.Index] = new List<SendProp>(e.Properties.Select(sp => sp.Clone()));

						e.InPVS = true;
					}
					else
					{
						// Preserve/update
						Entity e = ws.Entities[(uint)newEntity];// ws.Entities.Single(ent => ent.Index == newEntity);
						EntityCoder.ApplyEntityUpdate(e, Data);
					}
				}
				else
				{
					bool shouldDelete = Data.ReadBool();

					Entity e = ws.Entities[newEntity];
					if (e != null)
						e.InPVS = false;
					
					ReadLeavePVS(ws, newEntity, shouldDelete);
				}
			}

			if (IsDelta)
			{
				// Read explicit deletions
				while (Data.ReadBool())
				{
					uint ent = Data.ReadUInt(SourceConstants.MAX_EDICT_BITS);

					//Debug.Assert(ws.Entities[ent] != null);
					if (ws.Entities[ent] != null)
						ws.Entities[ent].Dispose();

					ws.Entities[ent] = null;
				}
			}

			//Console.WriteLine("Parsed {0}", Description);
		}

		class SendPropDefinitionComparer : IEqualityComparer<SendProp>
		{
			public static SendPropDefinitionComparer Instance { get; } = new SendPropDefinitionComparer();
			private SendPropDefinitionComparer() { }

			public bool Equals(SendProp x, SendProp y)
			{
				return x.Definition.Equals(y.Definition);
			}

			public int GetHashCode(SendProp obj)
			{
				return obj.Definition.GetHashCode();
			}
		}

		Entity ReadEnterPVS(WorldState ws, BitStream stream, uint entityIndex)
		{
			ServerClass serverClass = ws.ServerClasses[(int)stream.ReadUInt(ws.ClassBits)];
			SendTable networkTable = ws.SendTables.Single(st => st.NetTableName == serverClass.DatatableName);
			uint serialNumber = stream.ReadUInt(SourceConstants.NUM_NETWORKED_EHANDLE_SERIAL_NUMBER_BITS);

			Entity e;
			{
				Entity existing = ws.Entities[entityIndex];
				e = (existing == null || existing.SerialNumber != serialNumber) ?
					new Entity(ws, serverClass, networkTable, entityIndex, serialNumber) :
					existing;
			}

			var decodedBaseline = ws.InstanceBaselines[(int)Baseline.Value][entityIndex];
			if (decodedBaseline != null)
			{
				var propertiesToAdd =
					decodedBaseline
					.Except(e.Properties, SendPropDefinitionComparer.Instance)
					.Select(sp => sp.Clone(e));

				foreach (var p2a in propertiesToAdd)
					e.AddProperty(p2a);
			}
			else
			{
				BitStream baseline = ws.StaticBaselines.SingleOrDefault(bl => bl.Key == e.Class).Value;
				if (baseline != null)
				{
					baseline.Cursor = 0;
					EntityCoder.ApplyEntityUpdate(e, baseline);
					Debug.Assert((baseline.Length - baseline.Cursor) < 8);
				}
			}

			return e;
		}

		void ReadLeavePVS(WorldState ws, int newEntity, bool delete)
		{
			if (delete)
			{
				//Debug.Assert(ws.Entities[newEntity] != null);

				if (ws.Entities[newEntity] != null)
					ws.Entities[newEntity].Dispose();

				ws.Entities[newEntity] = null;
			}
		}
	}
}
