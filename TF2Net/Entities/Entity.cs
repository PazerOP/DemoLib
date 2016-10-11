using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TF2Net.Data;
using TF2Net.Monitors;

namespace TF2Net.Entities
{
	[DebuggerDisplay("{ToString(),nq}")]
	public class Entity : IEntity, IDisposable, IEquatable<Entity>
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly WorldState m_World;
		public WorldState World { get { CheckDisposed(); return m_World; } }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly ServerClass m_Class;
		public ServerClass Class { get { CheckDisposed(); return m_Class; } }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly SendTable m_NetworkTable;
		public SendTable NetworkTable { get { CheckDisposed(); return m_NetworkTable; } }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly List<SendProp> m_Properties = new List<SendProp>();
		public IReadOnlyList<SendProp> Properties { get { CheckDisposed(); return m_Properties; } }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly uint m_Index;
		public uint Index { get { CheckDisposed(); return m_Index; } }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly uint m_SerialNumber;
		public uint SerialNumber { get { CheckDisposed(); return m_SerialNumber; } }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool m_InPVS;
		public bool InPVS
		{
			get { CheckDisposed(); return m_InPVS; }
			set
			{
				CheckDisposed();
				var oldValue = m_InPVS;
				m_InPVS = value;

				if (value && !oldValue)
				{
					EnteredPVS.Invoke(this);
					World.Listeners.EntityEnteredPVS.Invoke(this);
				}
				else if (!value && oldValue)
				{
					LeftPVS.Invoke(this);
					World.Listeners.EntityLeftPVS.Invoke(this);
				}
			}
		}

		public SingleEvent<Action<Entity>> EnteredPVS { get; } = new SingleEvent<Action<Entity>>();
		public SingleEvent<Action<Entity>> LeftPVS { get; } = new SingleEvent<Action<Entity>>();

		public SingleEvent<Action<SendProp>> PropertyAdded { get; } = new SingleEvent<Action<SendProp>>();
		public SingleEvent<Action<IPropertySet>> PropertiesUpdated { get; } = new SingleEvent<Action<IPropertySet>>();

		public IEntityPropertyMonitor<EHandle> Owner { get; }
		public IEntityPropertyMonitor<Team?> Team { get; }

		public Entity(WorldState ws, ServerClass sClass, SendTable table, uint index, uint serialNumber)
		{
			m_World = ws;
			m_Class = sClass;
			m_NetworkTable = table;
			m_Index = index;
			m_SerialNumber = serialNumber;

			Team = new EntityPropertyMonitor<Team?>("DT_BaseEntity.m_iTeamNum", this, o => (Team)(int)o);
			Owner = new EntityPropertyMonitor<EHandle>("DT_BaseEntity.m_hOwnerEntity", this, o => new EHandle(ws, (uint)o));
		}

		public void AddProperty(SendProp newProp)
		{
			CheckDisposed();
			Debug.Assert(!m_Properties.Any(p => ReferenceEquals(p.Definition, newProp.Definition)));
			Debug.Assert(ReferenceEquals(newProp.Entity, this));

			m_Properties.Add(newProp);
			PropertyAdded.Invoke(newProp);
		}

		public override string ToString()
		{
			CheckDisposed();
			return string.Format("{0}({1}): {2}", Index, SerialNumber, Class.Classname);
		}

		public bool Equals(Entity other)
		{
			CheckDisposed();
			return (
				other?.Index == Index &&
				other.SerialNumber == SerialNumber);
		}
		public override bool Equals(object obj)
		{
			CheckDisposed();
			if (GetHashCode() != obj.GetHashCode())
				return false;

			return Equals(obj as Entity);
		}
		public override int GetHashCode()
		{
			CheckDisposed();
			return (int)(Index + (SerialNumber << SourceConstants.MAX_EDICT_BITS));
		}

		protected void CheckDisposed()
		{
			if (m_Disposed)
				throw new ObjectDisposedException(nameof(Entity));
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool m_Disposed = false;
		public void Dispose()
		{
			CheckDisposed();
			m_Disposed = true;

			foreach (SendProp prop in m_Properties)
				prop.Dispose();
		}
	}
}
