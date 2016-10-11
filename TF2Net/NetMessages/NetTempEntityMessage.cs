using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitSet;
using TF2Net.Data;
using TF2Net.Entities;

namespace TF2Net.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	public class NetTempEntityMessage : INetMessage
	{
		public int EntryCount { get; set; }

		public BitStream Data { get; set; }

		public string Description
		{
			get
			{
				return string.Format("svc_TempEntities: number {0}, bytes {1}", EntryCount, BitInfo.BitsToBytes(Data.Length));
			}
		}

		public void ReadMsg(BitStream stream)
		{
			EntryCount = stream.ReadInt(SourceConstants.EVENT_INDEX_BITS);

			ulong bitCount = stream.ReadVarUInt();

			Data = stream.Subsection(stream.Cursor, stream.Cursor + bitCount);
			stream.Seek(bitCount, System.IO.SeekOrigin.Current);
		}

		public void ApplyWorldState(WorldState ws)
		{
			List<IBaseEntity> tempents = new List<IBaseEntity>();
			{
				BitStream local = Data.Clone();
				local.Cursor = 0;

				TempEntity e = null;
				for (int i = 0; i < EntryCount; i++)
				{
					double delay = 0;
					if (local.ReadBool())
						delay = local.ReadInt(8) / 100.0;

					if (local.ReadBool())
					{
						uint classID = local.ReadUInt(ws.ClassBits);

						ServerClass serverClass = ws.ServerClasses[(int)classID - 1];
						SendTable sendTable = ws.SendTables.Single(st => st.NetTableName == serverClass.DatatableName);
						var flattened = sendTable.FlattenedProps;

						e = new TempEntity(ws, serverClass, sendTable);
						EntityCoder.ApplyEntityUpdate(e, local);
						tempents.Add(e);
					}
					else
					{
						Debug.Assert(e != null);
						EntityCoder.ApplyEntityUpdate(e, local);
					}
				}
			}

			foreach (IBaseEntity te in tempents)
			{
				ws.Listeners.TempEntityCreated.Invoke(te);
			}
		}

		[DebuggerDisplay("{Class,nq}")]
		class TempEntity : IEntity
		{
			readonly List<SendProp> m_Properties = new List<SendProp>();
			public IReadOnlyList<SendProp> Properties { get { return m_Properties; } }

			public SingleEvent<Action<IPropertySet>> PropertiesUpdated { get; } = new SingleEvent<Action<IPropertySet>>();
			public SingleEvent<Action<SendProp>> PropertyAdded { get; } = new SingleEvent<Action<SendProp>>();

			public WorldState World { get; }
			public ServerClass Class { get; }
			public SendTable NetworkTable { get; }

			public TempEntity(WorldState ws, ServerClass sClass, SendTable table)
			{
				World = ws;
				Class = sClass;
				NetworkTable = table;
			}

			public void AddProperty(SendProp newProp)
			{
				Debug.Assert(!m_Properties.Any(p => ReferenceEquals(p.Definition, newProp.Definition)));
				Debug.Assert(ReferenceEquals(newProp.Entity, this));

				m_Properties.Add(newProp);
				PropertyAdded.Invoke(newProp);
			}
		}
	}
}
