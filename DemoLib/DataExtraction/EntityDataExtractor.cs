using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitSet;
using DemoLib.NetMessages;

namespace DemoLib.DataExtraction
{
	public class EntityDataExtractor : IEnumerable<EntityData>
	{
		readonly IEnumerable<INetMessage> m_NetMessages;
		readonly IReadOnlyList<SendTable> m_SendTables;
		readonly IReadOnlyList<ServerClass> m_ServerClasses;

		readonly byte m_ServerClassBits;

		public EntityDataExtractor(
			IEnumerable<INetMessage> netMsgs,
			IEnumerable<SendTable> sendTables,
			IEnumerable<ServerClass> serverClasses)
		{
			m_NetMessages = netMsgs;
			m_SendTables = sendTables.ToList().AsReadOnly();
			m_ServerClasses = serverClasses.ToList().AsReadOnly();

			m_ServerClassBits = (byte)Math.Ceiling(Math.Log(m_ServerClasses.Count, 2));
		}

		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
		public IEnumerator<EntityData> GetEnumerator()
		{
			List<ClientFrame> oldFrames = new List<ClientFrame>();

			uint lastTick = 0;
			foreach (INetMessage netMsg in m_NetMessages)
			{
				if (netMsg is NetTickMessage)
					lastTick = ((NetTickMessage)netMsg).Tick;

				if (netMsg is NetPacketEntitiesMessage)
				{
					NetPacketEntitiesMessage pe = (NetPacketEntitiesMessage)netMsg;
					Process(pe, lastTick, oldFrames);
				}
			}

			throw new NotImplementedException();
		}

		void Process(NetPacketEntitiesMessage pe, uint lastTick, IList<ClientFrame> oldFrames)
		{
			ClientFrame newFrame = new ClientFrame();
			newFrame.TickCount = lastTick;

			ClientFrame oldFrame = null;

			if (pe.IsDelta)
			{
				if (newFrame.TickCount == pe.DeltaFrom)
					throw new InvalidOperationException("Update self-referencing");

				throw new NotImplementedException();
			}
			else
			{
				// Clear out the client's entity states..
				//throw new NotImplementedException();
			}

			if (pe.UpdateBaseline)
			{
				// Copy entity baseline(s)
				//throw new NotImplementedException();
			}

			EntityReadInfo u = new EntityReadInfo();
			u.Data = pe.Data;
			u.StartFrame = oldFrame;
			u.EndFrame = newFrame;
			u.AsDelta = pe.IsDelta;
			u.HeaderCount = pe.UpdatedEntries;
			u.Baseline = pe.Baseline ? BaselineIndex.Baseline1 : BaselineIndex.Baseline0;
			u.UpdateBaseline = pe.UpdateBaseline;

			ReadPacketEntities(u);

			throw new NotImplementedException();
		}

		void ReadPacketEntities(EntityReadInfo u)
		{
			u.NextOldEntity();

			while (u.UpdateType < DeltaUpdateType.Finished)
			{
				u.HeaderCount--;

				u.IsEntity = (u.HeaderCount >= 0);
				if (u.IsEntity.Value)
					ParseDeltaHeader(u);

				u.UpdateType = DeltaUpdateType.PreserveEnt;

				while (u.UpdateType == DeltaUpdateType.PreserveEnt)
				{
					// Figure out what kind of an update this is.
					if (DetermineUpdateType(u))
					{
						switch (u.UpdateType)
						{
							case DeltaUpdateType.EnterPVS: ReadEnterPVS(u); break;
							case DeltaUpdateType.LeavePVS: ReadLeavePVS(u); break;
							case DeltaUpdateType.DeltaEnt: ReadDeltaEnt(u); break;
							case DeltaUpdateType.PreserveEnt: ReadPreserveEnt(u); break;

							default:
							throw new InvalidOperationException("ReadPacketEntities: unknown updatetype");
						}
					}
				}
			}
		}

		static uint ReadUBitIntTest(byte[] buffer, ref ulong bitOffset)
		{
			uint ret = (uint)BitReader.ReadUIntBits(buffer, ref bitOffset, 6);
			switch (ret & (16 | 32))
			{
				case 16:
				ret = (ret & 15) | ((uint)BitReader.ReadUIntBits(buffer, ref bitOffset, 4) << 4);
				break;
				case 32:
				ret = (ret & 15) | ((uint)BitReader.ReadUIntBits(buffer, ref bitOffset, 8) << 4);
				break;
				case 48:
				ret = (ret & 15) | ((uint)BitReader.ReadUIntBits(buffer, ref bitOffset, 32 - 4) << 4);
				break;
			}
			return ret;
		}

		void ParseDeltaHeader(EntityReadInfo u)
		{
			u.UpdateFlags = EntityUpdateFlags.Zero;

			// https://github.com/StatsHelix/demoinfo/blob/master/DemoInfo/DP/Handler/PacketEntitesHandler.cs#L27
			u.EndEntity = (int)(u.HeaderBase + 1 + ReadUBitIntTest(u.Data, ref u.Cursor));
			Debug.Assert(u.EndEntity.Value < SourceConstants.MAX_EDICTS);

			u.HeaderBase = (int)(u.EndEntity);

			// leave pvs flag
			if (!BitReader.ReadBool(u.Data, ref u.Cursor))
			{
				// enter pvs flag
				if (BitReader.ReadBool(u.Data, ref u.Cursor))
					u.UpdateFlags |= EntityUpdateFlags.EnterPVS;
			}
			else
			{
				u.UpdateFlags |= EntityUpdateFlags.LeavePVS;

				// Force delete flag
				if (!BitReader.ReadBool(u.Data, ref u.Cursor))
					u.UpdateFlags |= EntityUpdateFlags.Delete;
			}
		}

		bool DetermineUpdateType(EntityReadInfo u)
		{
			if (!u.IsEntity.Value || (u.EndEntity > u.StartEntity))
			{
				// If we're at the last entity, preserve whatever entities followed it in the old packet.
				// If newnum > oldnum, then the server skipped sending entities that it wants to leave the state alone for.
				if (u.StartFrame == null || (u.StartEntity > u.StartFrame.LastEntity))
				{
					Debug.Assert(!u.IsEntity.Value);
					u.UpdateType = DeltaUpdateType.Finished;
					return false;
				}

				// Preserve entities until we reach newnum (ie: the server didn't send certain entities because
				// they haven't changed).
				u.UpdateType = DeltaUpdateType.PreserveEnt;
			}
			else
			{
				if (u.UpdateFlags.Value.HasFlag(EntityUpdateFlags.EnterPVS))
				{
					u.UpdateType = DeltaUpdateType.EnterPVS;
				}
				else if (u.UpdateFlags.Value.HasFlag(EntityUpdateFlags.LeavePVS))
				{
					u.UpdateType = DeltaUpdateType.LeavePVS;
				}
				else
				{
					u.UpdateType = DeltaUpdateType.DeltaEnt;
				}
			}

			return true;
		}

		void ReadEnterPVS(EntityReadInfo u)
		{
			uint serverClassIndex = (uint)BitReader.ReadUIntBits(u.Data, ref u.Cursor, m_ServerClassBits);
			Debug.Assert(serverClassIndex < m_ServerClasses.Count);

			var test = m_ServerClasses[(int)serverClassIndex];

			var entitySerial = BitReader.ReadUIntBits(u.Data, ref u.Cursor, SourceConstants.NUM_NETWORKED_EHANDLE_SERIAL_NUMBER_BITS);

			throw new NotImplementedException();
		}

		static void ReadLeavePVS(EntityReadInfo u)
		{
			// Sanity check.
			if (!u.AsDelta.Value)
			{
				Debug.Assert(false, "WARNING: LeavePVS on full update");
				u.UpdateType = DeltaUpdateType.Failed;
				return;
			}

			Debug.Assert(!u.EndFrame.TransmitEntity.Get(u.StartEntity.Value));

			if (u.UpdateFlags.Value.HasFlag(EntityUpdateFlags.Delete))
			{
				// Delete entity
			}

			u.NextOldEntity();
		}

		static void ReadDeltaEnt(EntityReadInfo u)
		{
			throw new NotImplementedException();
		}

		static void ReadPreserveEnt(EntityReadInfo u)
		{
			throw new NotImplementedException();
		}

		class ClientFrame
		{
			/// <summary>
			/// highest entity index
			/// </summary>
			public int? LastEntity { get; set; }

			/// <summary>
			/// server tick of this snapshot
			/// </summary>
			public uint? TickCount { get; set; }

			/// <summary>
			/// if bit n is set, entity n will be send to client
			/// </summary>
			public BitArray TransmitEntity { get; set; }

			/// <summary>
			/// if bit n is set, this entity was send as update from baseline
			/// </summary>
			public BitArray FromBaseline { get; set; }

			/// <summary>
			/// if bit is set, don't do PVS checks before sending (HLTV only)
			/// </summary>
			public BitArray TransmitAlways { get; set; }

			public ClientFrame NextFrame { get; set; }
		}
		enum DeltaUpdateType
		{
			/// <summary>
			/// Entity came back into pvs, create new entity if one doesn't exist
			/// </summary>
			EnterPVS = 0,

			/// <summary>
			/// Entity left pvs
			/// </summary>
			LeavePVS,

			/// <summary>
			/// There is a delta for this entity.
			/// </summary>
			DeltaEnt,
			/// <summary>
			/// Entity stays alive but no delta ( could be LOD, or just unchanged )
			/// </summary>
			PreserveEnt,

			/// <summary>
			/// finished parsing entities successfully
			/// </summary>
			Finished,
			/// <summary>
			/// parsing error occured while reading entities
			/// </summary>
			Failed,
		}

		enum BaselineIndex
		{
			Baseline0,
			Baseline1,
		}

		[Flags]
		enum EntityUpdateFlags
		{
			Zero = 0,
			LeavePVS = (1 << 1),
			Delete = (1 << 2),
			EnterPVS = (1 << 3),
		}

		class EntityReadInfo
		{
			const int ENTITY_SENTINEL = 9999;

			public byte[] Data { get; set; }
			public ulong Cursor;

			public EntityUpdateFlags? UpdateFlags { get; set; }

			public bool? IsEntity { get; set; }

			/// <summary>
			/// what baseline index do we use (0/1)
			/// </summary>
			public BaselineIndex? Baseline { get; set; }

			/// <summary>
			/// update baseline while parsing snaphsot
			/// </summary>
			public bool UpdateBaseline { get; set; }

			public bool? AsDelta { get; set; }

			public ClientFrame StartFrame { get; set; }
			public ClientFrame EndFrame { get; set; }

			public DeltaUpdateType UpdateType { get; set; } = DeltaUpdateType.PreserveEnt;

			/// <summary>
			/// current entity index in <see cref="StartFrame"/>
			/// </summary>
			public int? StartEntity { get; set; }

			/// <summary>
			/// current entity index in <see cref="EndFrame"/>
			/// </summary>
			public int? EndEntity { get; set; }

			public int HeaderBase { get; set; } = -1;
			public uint? HeaderCount { get; set; }

			public void NextOldEntity()
			{
				if (StartFrame != null)
				{
					StartEntity = (int)StartFrame.TransmitEntity.FindNextSetBit((uint)(StartEntity + 1).Value);

					if (!StartEntity.HasValue)
					{
						// Sentinel/end of list....
						StartEntity = ENTITY_SENTINEL;
					}
				}
				else
				{
					StartEntity = ENTITY_SENTINEL;
				}
			}

			public void NextNewEntity()
			{
				EndEntity = (int)EndFrame.TransmitEntity.FindNextSetBit((uint)(EndEntity + 1).Value);

				if (!EndEntity.HasValue)
				{
					// Sentinel/end of list....
					EndEntity = ENTITY_SENTINEL;
				}
			}
		}
	}
}
