using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitSet;

namespace TF2Net.Data
{
	[DebuggerDisplay("{ToString(),nq}")]
	public class Player
	{
		public WorldState World { get; }

		public uint EntityIndex { get; }
		public Entity Entity { get { return World.Entities[EntityIndex]; } }
		public bool InPVS { get { return Entity != null && Entity.InPVS; } }

		public UserInfo Info { get; set; }

		public Vector Position
		{
			get
			{
				var originXYProps = Entity.Properties
					.Where(p => p.Definition.Name == "m_vecOrigin")
					.OrderByDescending(p => p.LastChangedTick)
					.Select(p => p.Value)
					.Cast<Vector>();

				var originZProps = Entity.Properties
					.Where(p => p.Definition.Name == "m_vecOrigin[2]")
					.OrderByDescending(p => p.LastChangedTick)
					.Select(p => p.Value)
					.Cast<double>();

				var vec = originXYProps.FirstOrDefault()?.Clone();
				if (vec == null)
					return null;

				if (originZProps.Any())
					vec.Z = originZProps.First();

				return vec;
			}
		}

		public Team? Team
		{
			get
			{
				var prop = Entity.Properties.SingleOrDefault(p => p.Definition.Parent.NetTableName == "DT_BaseEntity" && p.Definition.Name == "m_iTeamNum");
				if (prop == null)
					return null;

				return (Team)(int)prop.Value;
			}
		}
		
		public Class? Class
		{
			get
			{
				var prop = Entity.Properties.SingleOrDefault(p => p.Definition.Parent.NetTableName == "DT_TFPlayerClassShared" && p.Definition.Name == "m_iClass");
				if (prop == null)
					return null;

				return (Class)(uint)prop.Value;
			}
		}

		public event Action<Player> EnteredPVS;
		public event Action<Player> PropertiesUpdated;
		public event Action<Player> LeftPVS;

		public Player(UserInfo info, WorldState ws, uint entityIndex)
		{
			EntityIndex = entityIndex;
			Info = info;
			World = ws;

			World.Listeners.EntityEnteredPVS -= Listeners_EntityEnteredPVS;
			World.Listeners.EntityEnteredPVS += Listeners_EntityEnteredPVS;

			World.Listeners.EntityLeftPVS -= Listeners_EntityLeftPVS;
			World.Listeners.EntityLeftPVS += Listeners_EntityLeftPVS;

			if (InPVS)
			{
				Entity.PropertiesUpdated -= Entity_PropertiesUpdated;
				Entity.PropertiesUpdated += Entity_PropertiesUpdated;

				Entity.PropertyAdded -= Entity_PropertyAdded;
				Entity.PropertyAdded += Entity_PropertyAdded;

				foreach (var prop in Entity.Properties)
					Entity_PropertyAdded(Entity, prop);

				Entity_PropertiesUpdated(Entity);
			}
		}

		private void Entity_PropertyAdded(Entity e, SendProp prop)
		{
			if (prop.Definition.Parent.NetTableName == "DT_TFPlayerClassShared" && prop.Definition.Name == "m_iClass")
			{
				prop.ValueChanged -= ClassPropertyChanged;
				prop.ValueChanged += ClassPropertyChanged;
			}
		}

		private void ClassPropertyChanged(SendProp prop)
		{
			//Class = (Class)(uint)prop.Value;
		}

		private void Listeners_EntityLeftPVS(Entity e)
		{
			Debug.Assert(e != null);
			if (e != Entity)
				return;

			LeftPVS?.Invoke(this);
			Entity.PropertiesUpdated -= Entity_PropertiesUpdated;
		}

		private void Listeners_EntityEnteredPVS(Entity e)
		{
			Debug.Assert(e != null);
			if (e != Entity)
				return;

			EnteredPVS?.Invoke(this);

			Entity.PropertiesUpdated -= Entity_PropertiesUpdated;
			Entity.PropertiesUpdated += Entity_PropertiesUpdated;

			Entity.PropertyAdded -= Entity_PropertyAdded;
			Entity.PropertyAdded += Entity_PropertyAdded;
		}

		private void Entity_PropertiesUpdated(Entity e)
		{
			Debug.Assert(e == Entity);

			PropertiesUpdated?.Invoke(this);
		}

		public override string ToString()
		{
			return string.Format("\"{0}\": {1}", Info.Name, Info.GUID);
		}
	}
}
