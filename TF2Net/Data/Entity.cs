using System;
using System.Collections.Generic;
using System.Diagnostics;

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

		public IList<SendProp> Properties { get; set; } = new List<SendProp>();

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
					EnteredPVS?.Invoke(World, this);
					World.Listeners.OnEntityEnteredPVS(World, this);
				}
				else if (!value && oldValue)
				{
					LeftPVS?.Invoke(World, this);
					World.Listeners.OnEntityLeftPVS(World, this);
				}
			}
		}
		
		public event Action<WorldState, Entity> EnteredPVS;
		public event Action<WorldState, Entity> LeftPVS;

		public event Action<WorldState, Entity> PropertiesUpdated;
		public void OnPropertiesUpdated() { PropertiesUpdated?.Invoke(World, this); }

		public Entity(WorldState ws, uint index, uint serialNumber)
		{
			if (ws == null)
				throw new ArgumentNullException(nameof(ws));

			World = ws;
			Index = index;
			SerialNumber = serialNumber;
		}

		public override string ToString()
		{
			return string.Format("{0}: {1}", Index, Class.Classname);
		}
	}
}
