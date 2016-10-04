using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BitSet;

namespace TF2Net.Data
{
	[DebuggerDisplay("{ToString(),nq}")]
	public class Entity
	{
		public WorldState World { get; }

		public uint Index { get; }
		public uint SerialNumber { get; }
		
		public ServerClass Class { get; set; }		
		public SendTable NetworkTable { get; set; }

		readonly List<SendProp> m_Properties = new List<SendProp>();
		public IReadOnlyList<SendProp> Properties { get { return m_Properties; } }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool m_InPVS;

		public bool InPVS
		{
			get { return m_InPVS; }
			set
			{
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
				if (m_PropertyAdded?.GetInvocationList().Contains(value) == true)
					return;
				m_PropertyAdded += value;
			}
			remove { m_PropertyAdded -= value; }
		}

		event Action<Entity> m_PropertiesUpdated;
		public event Action<Entity> PropertiesUpdated
		{
			add
			{
				if (m_PropertiesUpdated?.GetInvocationList().Contains(value) == true)
					return;
				m_PropertiesUpdated += value;
			}
			remove { m_PropertiesUpdated -= value; }
		}
		public void OnPropertiesUpdated() { m_PropertiesUpdated?.Invoke(this); }

		public Entity(WorldState ws, uint index, uint serialNumber)
		{
			World = ws;
			Index = index;
			SerialNumber = serialNumber;
		}

		public void AddProperty(SendProp newProp)
		{
			Debug.Assert(!m_Properties.Any(p => p.Definition == newProp.Definition));
			Debug.Assert(newProp.Entity == this);
			
			m_Properties.Add(newProp);

			m_PropertyAdded?.Invoke(newProp);
		}

		public SendProp GetProperty(SendPropDefinition def)
		{
			return Properties.FirstOrDefault(x => x.Definition == def);
		}

		public override string ToString()
		{
			return string.Format("{0}({1}): {2}", Index, SerialNumber, Class.Classname);
		}

		public bool Equals(Entity other)
		{
			return (
				other?.Index == Index &&
				other.SerialNumber == SerialNumber);
		}
		public override bool Equals(object obj)
		{
			if (GetHashCode() != obj.GetHashCode())
				return false;

			return Equals(obj as Entity);
		}
		public override int GetHashCode()
		{
			return (int)(Index + (SerialNumber << SourceConstants.MAX_EDICT_BITS));
		}
	}
}
