using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoLib.NetMessages;

namespace DemoLib.DataExtraction
{
	public class EntityDataExtractor : IEnumerable<EntityData>
	{
		readonly IEnumerable<INetMessage> m_NetMessages;
		public EntityDataExtractor(IEnumerable<INetMessage> netMsgs)
		{
			m_NetMessages = netMsgs;
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

		static void Process(NetPacketEntitiesMessage pe, uint lastTick, IList<ClientFrame> oldFrames)
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

		static void ReadPacketEntities(EntityReadInfo u)
		{
			u.NextOldEntity();

			while (u.UpdateType < DeltaUpdateType.Finished)
			{
				u.HeaderCount--;

				u.IsEntity = (u.HeaderCount >= 0);
				if (u.IsEntity.Value)
				{

				}
			}
		}

		static void ParseDeltaHeader(EntityReadInfo u)
		{
			u.UpdateFlags
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
			public uint? StartEntity { get; set; }

			/// <summary>
			/// current entity index in <see cref="EndFrame"/>
			/// </summary>
			public uint? EndEntity { get; set; }

			public uint? HeaderBase { get; set; }
			public uint? HeaderCount { get; set; }

			public void NextOldEntity()
			{
				if (StartFrame != null)
				{
					StartEntity = StartFrame.TransmitEntity.FindNextSetBit((StartEntity + 1).Value);

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
				EndEntity = EndFrame.TransmitEntity.FindNextSetBit((EndEntity + 1).Value);

				if (!EndEntity.HasValue)
				{
					// Sentinel/end of list....
					EndEntity = ENTITY_SENTINEL;
				}
			}
		}
	}
}
