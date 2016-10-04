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

		List<Action<Player>> ValueChangedDelegates { get; } = new List<Action<Player>>();

		public IPropertyMonitor<Vector> Position { get; }
		public IPropertyMonitor<Team?> Team { get; }
		public IPropertyMonitor<Class?> Class { get; }

		event Action<Player> m_EnteredPVS;
		public event Action<Player> EnteredPVS
		{
			add
			{
				if (m_EnteredPVS?.GetInvocationList().Contains(value) == true)
					return;
				m_EnteredPVS += value;

				if (InPVS)
					value?.Invoke(this);
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

		event Action<Player> m_PropertiesUpdated;
		public event Action<Player> PropertiesUpdated
		{
			add
			{
				if (m_PropertiesUpdated?.GetInvocationList().Contains(value) == true)
					return;
				m_PropertiesUpdated += value;
			}
			remove { m_PropertiesUpdated -= value; }
		}

		public Player(UserInfo info, WorldState ws, uint entityIndex)
		{
			EntityIndex = entityIndex;
			Info = info;
			World = ws;

			Position = new PositionPropertyMonitor(this);
			Class = new PropertyMonitor<Class?>("DT_TFPlayerClassShared.m_iClass", this, o => (Class)(uint)o);
			Team = new PropertyMonitor<Team?>("DT_BaseEntity.m_iTeamNum", this, o => (Team)(int)o);

			World.Listeners.EntityEnteredPVS += Listeners_EntityEnteredPVS;
			World.Listeners.EntityLeftPVS += Listeners_EntityLeftPVS;

			if (InPVS)
				Listeners_EntityEnteredPVS(Entity);
		}

		private void Listeners_EntityLeftPVS(Entity e)
		{
			Debug.Assert(e != null);
			if (e != Entity)
				return;

			e.PropertiesUpdated -= Entity_PropertiesUpdated;

			m_LeftPVS?.Invoke(this);
		}

		private void Listeners_EntityEnteredPVS(Entity e)
		{
			Debug.Assert(e != null);
			if (e != Entity)
				return;

			e.PropertiesUpdated += Entity_PropertiesUpdated;

			m_EnteredPVS?.Invoke(this);
		}

		private void Entity_PropertiesUpdated(Entity e)
		{
			Debug.Assert(e == Entity);
			m_PropertiesUpdated?.Invoke(this);

			foreach (var action in ValueChangedDelegates)
				action(this);

			ValueChangedDelegates.Clear();
		}

		public override string ToString()
		{
			return string.Format("\"{0}\": {1}", Info.Name, Info.GUID);
		}

		public interface IPropertyMonitor<T>
		{
			T Value { get; }
			event Action<Player> ValueChanged;

			string PropertyName { get; }
			Player Player { get; }
		}
		[DebuggerDisplay("{PropertyName,nq}: {Value}")]
		class PropertyMonitor<T> : IPropertyMonitor<T>
		{
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			T m_Value;

			public T Value
			{
				get
				{
					Debug.Assert(DebugValue?.Equals(m_Value) != false);
					return m_Value;
				}
			}
			public string PropertyName { get; }
			public Player Player { get; }

			object DebugValue
			{
				get
				{
					SendProp prop = Player.Entity.Properties.SingleOrDefault(p => p.Definition.FullName == PropertyName);

					if (prop != null)
						return Decoder(prop.Value);
					else
						return null;
				}
			}

			Func<object, T> Decoder { get; }

			bool m_ValueChanged = false;
			event Action<Player> m_ValueChangedEvent;
			public event Action<Player> ValueChanged
			{
				add
				{
					if (m_ValueChangedEvent?.GetInvocationList().Contains(value) == true)
						return;
					m_ValueChangedEvent += value;
				}
				remove { m_ValueChangedEvent -= value; }
			}

			public PropertyMonitor(string propertyName, Player player, Func<object, T> decoder)
			{
				Player = player;
				PropertyName = propertyName;
				Decoder = decoder;

				player.EnteredPVS += Player_EnteredPVS;
				player.LeftPVS += Player_LeftPVS;
				player.PropertiesUpdated += Player_PropertiesUpdated;
			}

			private void Player_PropertiesUpdated(Player obj)
			{
				if (m_ValueChanged)
				{
					m_ValueChangedEvent?.Invoke(Player);
					m_ValueChanged = false;
				}
			}

			private void Player_EnteredPVS(Player p)
			{
				Entity e = p.Entity;
				e.PropertyAdded += Entity_PropertyAdded;

				foreach (SendProp prop in e.Properties)
					Entity_PropertyAdded(prop);
			}

			private void Entity_PropertyAdded(SendProp prop)
			{
				if (prop.Definition.FullName == PropertyName)
					prop.ValueChanged += Prop_ValueChanged;
			}

			private void Prop_ValueChanged(SendProp prop)
			{
				m_Value = Decoder(prop.Value);
				m_ValueChanged = true;
			}

			private void Player_LeftPVS(Player p)
			{
				p.Entity.PropertyAdded -= Entity_PropertyAdded;
			}
		}
		
		class PositionPropertyMonitor : IPropertyMonitor<Vector>
		{
			Vector m_Value = new Vector();
			public Vector Value { get { return m_Value.Clone(); } }

			event Action<Player> m_ValueChanged;
			public event Action<Player> ValueChanged
			{
				add
				{
					if (m_ValueChanged?.GetInvocationList().Contains(value) == true)
						return;
					m_ValueChanged += value;
				}
				remove { m_ValueChanged -= value; }
			}

			public Player Player { get; }
			public string PropertyName { get { return null; } }

			IPropertyMonitor<Vector> LocalOriginXY { get; }
			IPropertyMonitor<double> LocalOriginZ { get; }
			IPropertyMonitor<Vector> NonLocalOriginXY { get; }
			IPropertyMonitor<double> NonLocalOriginZ { get; }

			bool m_PositionChanged = false;

			public PositionPropertyMonitor(Player player)
			{
				Player = player;

				LocalOriginXY = new PropertyMonitor<Vector>("DT_TFLocalPlayerExclusive.m_vecOrigin", Player, o => (Vector)o);
				LocalOriginZ = new PropertyMonitor<double>("DT_TFLocalPlayerExclusive.m_vecOrigin[2]", Player, o => (double)o);
				NonLocalOriginXY = new PropertyMonitor<Vector>("DT_TFNonLocalPlayerExclusive.m_vecOrigin", Player, o => (Vector)o);
				NonLocalOriginZ = new PropertyMonitor<double>("DT_TFNonLocalPlayerExclusive.m_vecOrigin[2]", Player, o => (double)o);

				LocalOriginXY.ValueChanged += LocalOriginXY_ValueChanged;
				LocalOriginZ.ValueChanged += LocalOriginZ_ValueChanged;
				NonLocalOriginXY.ValueChanged += NonLocalOriginXY_ValueChanged;
				NonLocalOriginZ.ValueChanged += NonLocalOriginZ_ValueChanged;

				Player.PropertiesUpdated += Player_PropertiesUpdated;
			}

			private void NonLocalOriginZ_ValueChanged(Player p)
			{
				Debug.Assert(Player == p);
				m_Value.Z = NonLocalOriginZ.Value;
				m_PositionChanged = true;
			}

			private void NonLocalOriginXY_ValueChanged(Player p)
			{
				Debug.Assert(Player == p);
				m_Value.X = NonLocalOriginXY.Value.X;
				m_Value.Y = NonLocalOriginXY.Value.Y;
				m_PositionChanged = true;
			}

			private void LocalOriginZ_ValueChanged(Player p)
			{
				Debug.Assert(Player == p);
				m_Value.Z = LocalOriginZ.Value;
				m_PositionChanged = true;
			}

			private void LocalOriginXY_ValueChanged(Player p)
			{
				Debug.Assert(Player == p);
				m_Value.X = LocalOriginXY.Value.X;
				m_Value.Y = LocalOriginXY.Value.Y;
				m_PositionChanged = true;
			}

			private void Player_PropertiesUpdated(Player p)
			{
				Debug.Assert(Player == p);

				if (m_PositionChanged)
				{
					m_PositionChanged = false;
					m_ValueChanged?.Invoke(Player);
				}
			}
		}
	}
}
