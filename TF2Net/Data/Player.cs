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

		#region Player Position
		Vector m_Position;
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
		void PositionXYPropertyChanged(SendProp prop)
		{
			if (prop.)
		}
		void PositionZPropertyChanged(SendProp prop)
		{

		}
		event Action<Player> m_PositionChanged;
		#endregion
		#region Player Team
		public Team? Team
		{
			get { return (Team?)(int?)Entity.Properties.SingleOrDefault(p => p.Definition.FullName == "DT_BaseEntity.m_iTeamNum")?.Value; }
		}
		#endregion
		#region Player Class
		Class? m_Class;
		public Class? Class
		{
			get
			{
				var prop = Entity.Properties.SingleOrDefault(p => p.Definition.FullName == "DT_TFPlayerClassShared.m_iClass");
				if (prop == null)
					return null;

				Class retVal = (Class)(uint)prop.Value;
				Debug.Assert(retVal == m_Class);
				return retVal;
			}
		}
		void ClassPropertyChanged(SendProp prop)
		{
			m_Class = (Class)(uint)prop.Value;
			m_ClassChanged?.Invoke(this);
		}
		event Action<Player> m_ClassChanged;
		public event Action<Player> ClassChanged
		{
			add
			{
				if (m_ClassChanged?.GetInvocationList().Contains(value) == true)
					return;
				m_ClassChanged += value;
			}
			remove { m_ClassChanged -= value; }
		}
		#endregion

		event Action<Player> m_EnteredPVS;
		public event Action<Player> EnteredPVS
		{
			add
			{
				if (m_EnteredPVS?.GetInvocationList().Contains(value) == true)
					return;
				m_EnteredPVS += value;
			}
			remove { m_EnteredPVS -= value; }
		}

		event Action<Player> m_LeftPVS;
		public event Action<Player> LeftPVS
		{
			add
			{
				if (m_LeftPVS?.GetInvocationList().Contains(value) == true)
					return;
				m_LeftPVS += value;
			}
			remove { m_LeftPVS -= value; }
		}

		public Player(UserInfo info, WorldState ws, uint entityIndex)
		{
			EntityIndex = entityIndex;
			Info = info;
			World = ws;
			
			World.Listeners.EntityEnteredPVS += Listeners_EntityEnteredPVS;
			World.Listeners.EntityLeftPVS += Listeners_EntityLeftPVS;

			if (InPVS)
				Listeners_EntityEnteredPVS(Entity);
		}

		private void Entity_PropertyAdded(SendProp prop)
		{
			if (prop.Definition.FullName == "DT_TFPlayerClassShared.m_iClass")
				prop.ValueChanged += ClassPropertyChanged;
			else if (prop.Definition.FullName == "DT_TFLocalPlayerExclusive.m_vecOrigin")
				prop.ValueChanged += PositionXYPropertyChanged;
		}

		private void Listeners_EntityLeftPVS(Entity e)
		{
			Debug.Assert(e != null);
			if (e != Entity)
				return;

			m_LeftPVS?.Invoke(this);
			Entity.PropertyAdded -= Entity_PropertyAdded;
		}

		private void Listeners_EntityEnteredPVS(Entity e)
		{
			Debug.Assert(e != null);
			if (e != Entity)
				return;

			//if (Info.Name == "Laggy")
			//	Debugger.Break();
		
			Entity.PropertyAdded += Entity_PropertyAdded;

			foreach (SendProp prop in Entity.Properties)
				Entity_PropertyAdded(prop);

			m_EnteredPVS?.Invoke(this);
		}

		public override string ToString()
		{
			return string.Format("\"{0}\": {1}", Info.Name, Info.GUID);
		}
	}
}
