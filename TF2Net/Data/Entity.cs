using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TF2Net.Data
{
	[DebuggerDisplay("{ToString(),nq}")]
	public class Entity : IDisposable
	{
		public WorldState World { get; }

		public uint Index { get; }
		public uint SerialNumber { get; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		ServerClass m_Class;
		public ServerClass Class
		{
			get
			{
				if (m_Disposed)
					throw new ObjectDisposedException(nameof(Entity));

				return m_Class;
			}
			set
			{
				if (m_Disposed)
					throw new ObjectDisposedException(nameof(Entity));

				m_Class = value;
			}
		}

		SendTable m_NetworkTable;
		public SendTable NetworkTable
		{
			get
			{
				if (m_Disposed)
					throw new ObjectDisposedException(nameof(Entity));
				
				return m_NetworkTable;
			}
			set
			{
				if (m_Disposed)
					throw new ObjectDisposedException(nameof(Entity));

				m_NetworkTable = value;
			}
		}

		readonly List<SendProp> m_Properties = new List<SendProp>();
		public IReadOnlyList<SendProp> Properties
		{
			get
			{
				if (m_Disposed)
					throw new ObjectDisposedException(nameof(Entity));

				return m_Properties;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool m_InPVS;

		public bool InPVS
		{
			get
			{
				if (m_Disposed)
					throw new ObjectDisposedException(nameof(Entity));

				return m_InPVS;
			}
			set
			{
				if (m_Disposed)
					throw new ObjectDisposedException(nameof(Entity));

				var oldValue = m_InPVS;
				m_InPVS = value;

				if (value && !oldValue)
				{
					m_EnteredPVS?.Invoke(this);
					World.Listeners.OnEntityEnteredPVS(this);
				}
				else if (!value && oldValue)
				{
					LeftPVS?.Invoke(this);
					World.Listeners.OnEntityLeftPVS(this);
				}
			}
		}

		event Action<Entity> m_EnteredPVS;
		public event Action<Entity> EnteredPVS
		{
			add
			{
				if (m_Disposed)
					throw new ObjectDisposedException(nameof(Entity));
				if (m_EnteredPVS != null)
					Debug.Assert(!m_EnteredPVS.GetInvocationList().Contains(value));

				m_EnteredPVS += value;
			}
			remove { m_EnteredPVS -= value; }
		}
		public event Action<Entity> LeftPVS;

		event Action<SendProp> m_PropertyAdded;
		public event Action<SendProp> PropertyAdded
		{
			add
			{
				if (m_Disposed)
					throw new ObjectDisposedException(nameof(Entity));
				if (m_PropertyAdded != null)
					Debug.Assert(!m_PropertyAdded.GetInvocationList().Contains(value));

				m_PropertyAdded += value;
			}
			remove { m_PropertyAdded -= value; }
		}

		public Entity(WorldState ws, uint index, uint serialNumber)
		{
			if (ws == null)
				throw new ArgumentNullException(nameof(ws));

			World = ws;
			Index = index;
			SerialNumber = serialNumber;
		}

		public void AddProperty(SendProp newProp)
		{
			if (m_Disposed)
				throw new ObjectDisposedException(nameof(Entity));

			Debug.Assert(!m_Properties.Any(p => p.Definition == newProp.Definition));
			Debug.Assert(newProp.Entity == this);
			
			m_Properties.Add(newProp);

			m_PropertyAdded?.Invoke(newProp);
		}

		public SendProp GetProperty(SendPropDefinition def)
		{
			if (m_Disposed)
				throw new ObjectDisposedException(nameof(Entity));

			return Properties.FirstOrDefault(x => x.Definition == def);
		}

		public override string ToString()
		{
			return string.Format("{0}({1}): {2}", Index, SerialNumber, Class.Classname);
		}

		public bool Equals(Entity other)
		{
			if (m_Disposed)
				throw new ObjectDisposedException(nameof(Entity));

			return (
				other?.Index == Index &&
				other.SerialNumber == SerialNumber);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as Entity);
		}
		public override int GetHashCode()
		{
			if (m_Disposed)
				throw new ObjectDisposedException(nameof(Entity));

			return (int)(Index + (SerialNumber << SourceConstants.MAX_EDICT_BITS));
		}

		bool m_Disposed = false;
		public void Dispose()
		{
			m_Disposed = true;

			m_Class = null;
			m_NetworkTable = null;
		}
	}
}
