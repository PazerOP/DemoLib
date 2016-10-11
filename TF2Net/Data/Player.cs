using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitSet;
using TF2Net.Monitors;

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

		public IPlayerPropertyMonitor<Vector> Position { get; }
		public IPlayerPropertyMonitor<Team?> Team { get; }
		public IPlayerPropertyMonitor<Class?> Class { get; }
		public IPlayerPropertyMonitor<bool?> IsDead { get; }
		public IPlayerPropertyMonitor<int?> Health { get; }
		public IPlayerPropertyMonitor<uint?> MaxHealth { get; }
		public IPlayerPropertyMonitor<uint?> MaxBuffedHealth { get; }
		public IPlayerPropertyMonitor<int?> Ping { get; }
		public IPlayerPropertyMonitor<int?> Score { get; }
		public IPlayerPropertyMonitor<int?> Deaths { get; }
		public IPlayerPropertyMonitor<bool?> Connected { get; }
		public IPlayerPropertyMonitor<PlayerState?> PlayerState { get; }

		SingleEvent<Action<Player>> m_EnteredPVS { get; } = new SingleEvent<Action<Player>>();
		public event Action<Player> EnteredPVS
		{
			add
			{
				if (!m_EnteredPVS.Add(value))
					return;

				if (InPVS)
					value?.Invoke(this);
			}
			remove { m_EnteredPVS.Remove(value); }
		}

		public SingleEvent<Action<Player>> LeftPVS { get; } = new SingleEvent<Action<Player>>();
		public SingleEvent<Action<Player>> PropertiesUpdated { get; } = new SingleEvent<Action<Player>>();

		public Player(UserInfo info, WorldState ws, uint entityIndex)
		{
			EntityIndex = entityIndex;
			Info = info;
			World = ws;

			#region Property Monitors
			Position = new PlayerPositionPropertyMonitor(this);

			Team = new MultiPlayerPropertyMonitor<Team?>(this,
				new IPropertyMonitor<Team?>[] {
					new PlayerResourcePropertyMonitor<Team?>("m_iTeam", this, o => (Team)Convert.ToInt32(o)),
					new PlayerPropertyMonitor<Team?>("DT_BaseEntity.m_iTeamNum", this, o => (Team)Convert.ToInt32(o))
				});

			IsDead = new MultiPlayerPropertyMonitor<bool?>(this,
				new IPropertyMonitor<bool?>[] {
					new PlayerResourcePropertyMonitor<bool?>("m_bAlive", this, o => Convert.ToInt32(o) == 0),
					new PlayerPropertyMonitor<bool?>("DT_BasePlayer.m_lifeState", this, o => (LifeState)Convert.ToInt32(o) != LifeState.Alive)
				});

			Health = new MultiPlayerPropertyMonitor<int?>(this,
				new IPropertyMonitor<int?>[] {
					new PlayerResourcePropertyMonitor<int?>("m_iHealth", this, o => Convert.ToInt32(o)),
					new PlayerPropertyMonitor<int?>("DT_BasePlayer.m_iHealth", this, o => Convert.ToInt32(o)),
				});

			Class = new MultiPlayerPropertyMonitor<Class?>(this,
				new IPropertyMonitor<Class?>[] {
					new PlayerResourcePropertyMonitor<Class?>("m_iPlayerClass", this, o => (Class)Convert.ToInt32(o)),
					new PlayerPropertyMonitor<Class?>("DT_TFPlayerClassShared.m_iClass", this, o => (Class)Convert.ToInt32(o))
				});

			PlayerState = new PlayerPropertyMonitor<PlayerState?>("DT_TFPlayerShared.m_nPlayerState", this, o => (PlayerState)(uint)(o));			
			MaxHealth = new PlayerResourcePropertyMonitor<uint?>("m_iMaxHealth", this, o => Convert.ToUInt32(o));
			MaxBuffedHealth = new PlayerResourcePropertyMonitor<uint?>("m_iMaxBuffedHealth", this, o => Convert.ToUInt32(o));
			Ping = new PlayerResourcePropertyMonitor<int?>("m_iPing", this, o => Convert.ToInt32(o));
			Score = new PlayerResourcePropertyMonitor<int?>("m_iScore", this, o => Convert.ToInt32(o));
			Deaths = new PlayerResourcePropertyMonitor<int?>("m_iDeaths", this, o => Convert.ToInt32(o));
			Connected = new PlayerResourcePropertyMonitor<bool?>("m_bConnected", this, o => Convert.ToInt32(o) != 0);
			#endregion

			World.Listeners.EntityEnteredPVS.Add(Listeners_EntityEnteredPVS);
			World.Listeners.EntityLeftPVS.Add(Listeners_EntityLeftPVS);

			if (InPVS)
				Listeners_EntityEnteredPVS(Entity);
		}

		void InitPropertyMonitors()
		{

		}

		private void Listeners_EntityLeftPVS(Entity e)
		{
			Debug.Assert(e != null);
			if (e != Entity)
				return;

			e.PropertiesUpdated.Remove(Entity_PropertiesUpdated);

			LeftPVS.Invoke(this);
		}

		private void Listeners_EntityEnteredPVS(Entity e)
		{
			Debug.Assert(e != null);
			if (e != Entity)
				return;

			e.PropertiesUpdated.Add(Entity_PropertiesUpdated);

			m_EnteredPVS?.Invoke(this);
		}

		private void Entity_PropertiesUpdated(Entity e)
		{
			Debug.Assert(e == Entity);
			PropertiesUpdated.Invoke(this);
		}

		public override string ToString()
		{
			return string.Format("\"{0}\": {1}", Info.Name, Info.GUID);
		}

		[DebuggerDisplay("{Value}")]
		class PlayerPropertyMonitor<T> : IPlayerPropertyMonitor<T>
		{
			bool m_ValueChanged = false;
			public T Value { get; private set; }
			object IPropertyMonitor.Value { get { return Value; } }
			public SendProp Property { get; private set; }

			public string PropertyName { get; }
			public Player Player { get; }
			public Entity Entity { get { return Player.Entity; } }

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

			SingleEvent<Action<IPropertyMonitor>> IPropertyMonitor.ValueChanged { get; } = new SingleEvent<Action<IPropertyMonitor>>();
			SingleEvent<Action<IPropertyMonitor<T>>> IPropertyMonitor<T>.ValueChanged { get; } = new SingleEvent<Action<IPropertyMonitor<T>>>();
			SingleEvent<Action<IEntityPropertyMonitor>> IEntityPropertyMonitor.ValueChanged { get; } = new SingleEvent<Action<IEntityPropertyMonitor>>();
			SingleEvent<Action<IEntityPropertyMonitor<T>>> IEntityPropertyMonitor<T>.ValueChanged { get; } = new SingleEvent<Action<IEntityPropertyMonitor<T>>>();
			SingleEvent<Action<IPlayerPropertyMonitor>> IPlayerPropertyMonitor.ValueChanged { get; } = new SingleEvent<Action<IPlayerPropertyMonitor>>();
			public SingleEvent<Action<IPlayerPropertyMonitor<T>>> ValueChanged { get; } = new SingleEvent<Action<IPlayerPropertyMonitor<T>>>();

			public PlayerPropertyMonitor(string propertyName, Player player, Func<object, T> decoder)
			{
				ValueChanged.Add((self) => ((IPropertyMonitor)self).ValueChanged.Invoke(self));
				ValueChanged.Add((self) => ((IPropertyMonitor<T>)self).ValueChanged.Invoke(self));
				ValueChanged.Add((self) => ((IEntityPropertyMonitor)self).ValueChanged.Invoke(self));
				ValueChanged.Add((self) => ((IEntityPropertyMonitor<T>)self).ValueChanged.Invoke(self));
				ValueChanged.Add((self) => ((IPlayerPropertyMonitor)self).ValueChanged.Invoke(self));

				Player = player;
				PropertyName = propertyName;
				Decoder = decoder;

				player.EnteredPVS += Player_EnteredPVS;
				player.LeftPVS.Add(Player_LeftPVS);
				player.PropertiesUpdated.Add(Player_PropertiesUpdated);
			}

			private void Player_PropertiesUpdated(Player p)
			{
				if (m_ValueChanged)
				{
					ValueChanged.Invoke(this);
					m_ValueChanged = false;
				}
			}

			private void Player_EnteredPVS(Player p)
			{
				Entity e = p.Entity;
				e.PropertyAdded.Add(Entity_PropertyAdded);

				foreach (SendProp prop in e.Properties)
					Entity_PropertyAdded(prop);
			}

			private void Entity_PropertyAdded(SendProp prop)
			{
				if (prop.Definition.FullName == PropertyName)
				{
					Property = prop;

					if (prop.ValueChanged.Add(Prop_ValueChanged))
					{
						// First add only
						if (prop.Value != null)
							Prop_ValueChanged(prop);
					}
				}
			}

			private void Prop_ValueChanged(SendProp prop)
			{
				Debug.Assert((!prop.Entity.InPVS && Property == null) || prop == Property);
				var newValue = Decoder(prop.Value);
				//Debug.Assert(Value?.Equals(newValue) != true);
				Value = newValue;
				m_ValueChanged = true;
			}

			private void Player_LeftPVS(Player p)
			{
				p.Entity.PropertyAdded.Remove(Entity_PropertyAdded);
				Property = null;
			}
		}

		[DebuggerDisplay("{Value}")]
		class PlayerPositionPropertyMonitor : IPlayerPropertyMonitor<Vector>
		{
			readonly Vector m_Value = new Vector();
			public Vector Value { get { return m_Value.Clone(); } }
			object IPropertyMonitor.Value { get { return Value; } }
			public SendProp Property { get { return null; } }

			public Player Player { get; }
			public Entity Entity { get { return Player.Entity; } }
			public string PropertyName { get { return null; } }

			IPlayerPropertyMonitor<Vector> LocalOriginXY { get; }
			IPlayerPropertyMonitor<double> LocalOriginZ { get; }
			IPlayerPropertyMonitor<Vector> NonLocalOriginXY { get; }
			IPlayerPropertyMonitor<double> NonLocalOriginZ { get; }

			SingleEvent<Action<IPropertyMonitor>> IPropertyMonitor.ValueChanged { get; } = new SingleEvent<Action<IPropertyMonitor>>();
			SingleEvent<Action<IPropertyMonitor<Vector>>> IPropertyMonitor<Vector>.ValueChanged { get; } = new SingleEvent<Action<IPropertyMonitor<Vector>>>();
			SingleEvent<Action<IEntityPropertyMonitor>> IEntityPropertyMonitor.ValueChanged { get; } = new SingleEvent<Action<IEntityPropertyMonitor>>();
			SingleEvent<Action<IEntityPropertyMonitor<Vector>>> IEntityPropertyMonitor<Vector>.ValueChanged { get; } = new SingleEvent<Action<IEntityPropertyMonitor<Vector>>>();
			SingleEvent<Action<IPlayerPropertyMonitor>> IPlayerPropertyMonitor.ValueChanged { get; } = new SingleEvent<Action<IPlayerPropertyMonitor>>();
			public SingleEvent<Action<IPlayerPropertyMonitor<Vector>>> ValueChanged { get; } = new SingleEvent<Action<IPlayerPropertyMonitor<Vector>>>();

			bool m_PositionChanged = false;

			public PlayerPositionPropertyMonitor(Player player)
			{
				ValueChanged.Add(self => ((IPropertyMonitor)self).ValueChanged.Invoke(self));
				ValueChanged.Add(self => ((IPropertyMonitor<Vector>)self).ValueChanged.Invoke(self));
				ValueChanged.Add(self => ((IEntityPropertyMonitor)self).ValueChanged.Invoke(self));
				ValueChanged.Add(self => ((IEntityPropertyMonitor<Vector>)self).ValueChanged.Invoke(self));
				ValueChanged.Add(self => ((IPlayerPropertyMonitor)self).ValueChanged.Invoke(self));

				Player = player;

				LocalOriginXY = new PlayerPropertyMonitor<Vector>("DT_TFLocalPlayerExclusive.m_vecOrigin", Player, o => (Vector)o);
				LocalOriginZ = new PlayerPropertyMonitor<double>("DT_TFLocalPlayerExclusive.m_vecOrigin[2]", Player, o => (double)o);
				NonLocalOriginXY = new PlayerPropertyMonitor<Vector>("DT_TFNonLocalPlayerExclusive.m_vecOrigin", Player, o => (Vector)o);
				NonLocalOriginZ = new PlayerPropertyMonitor<double>("DT_TFNonLocalPlayerExclusive.m_vecOrigin[2]", Player, o => (double)o);

				LocalOriginXY.ValueChanged.Add(OriginXY_ValueChanged);
				LocalOriginZ.ValueChanged.Add(OriginZ_ValueChanged);
				NonLocalOriginXY.ValueChanged.Add(OriginXY_ValueChanged);
				NonLocalOriginZ.ValueChanged.Add(OriginZ_ValueChanged);

				Player.PropertiesUpdated.Add(Player_PropertiesUpdated);
			}
			
			private void OriginZ_ValueChanged(IPlayerPropertyMonitor<double> z)
			{
				m_Value.Z = z.Value;
				m_PositionChanged = true;
			}
			private void OriginXY_ValueChanged(IPlayerPropertyMonitor<Vector> xy)
			{
				var value = xy.Value;
				m_Value.X = value.X;
				m_Value.Y = value.Y;
				m_PositionChanged = true;
			}

			private void Player_PropertiesUpdated(Player p)
			{
				Debug.Assert(Player == p);
				if (m_PositionChanged)
				{
					ValueChanged.Invoke(this);
					m_PositionChanged = false;
				}
			}
		}

		[DebuggerDisplay("{Value}")]
		class PlayerResourcePropertyMonitor<T> : IPlayerPropertyMonitor<T>
		{
			public Player Player { get; }
			public Entity Entity { get { return Player.Entity; } }

			public string PropertyName { get; }
			public SendProp Property { get { return InternalPropertyMonitor.Property; } }
			Func<object, T> Decoder { get; }

			public T Value { get { return InternalPropertyMonitor.Value; } }
			object IPropertyMonitor.Value { get { return Value; } }

			Entity PlayerResourceEntity { get; }

			EntityPropertyMonitor<T> InternalPropertyMonitor { get; }

			SingleEvent<Action<IPropertyMonitor>> IPropertyMonitor.ValueChanged { get; } = new SingleEvent<Action<IPropertyMonitor>>();
			SingleEvent<Action<IPropertyMonitor<T>>> IPropertyMonitor<T>.ValueChanged { get; } = new SingleEvent<Action<IPropertyMonitor<T>>>();
			SingleEvent<Action<IEntityPropertyMonitor>> IEntityPropertyMonitor.ValueChanged { get; } = new SingleEvent<Action<IEntityPropertyMonitor>>();
			SingleEvent<Action<IEntityPropertyMonitor<T>>> IEntityPropertyMonitor<T>.ValueChanged { get; } = new SingleEvent<Action<IEntityPropertyMonitor<T>>>();
			SingleEvent<Action<IPlayerPropertyMonitor>> IPlayerPropertyMonitor.ValueChanged { get; } = new SingleEvent<Action<IPlayerPropertyMonitor>>();
			public SingleEvent<Action<IPlayerPropertyMonitor<T>>> ValueChanged { get; } = new SingleEvent<Action<IPlayerPropertyMonitor<T>>>();

			public PlayerResourcePropertyMonitor(string propertyName, Player player, Func<object, T> decoder)
			{
				ValueChanged.Add(self => ((IPropertyMonitor)self).ValueChanged.Invoke(self));
				ValueChanged.Add(self => ((IPropertyMonitor<T>)self).ValueChanged.Invoke(self));
				ValueChanged.Add(self => ((IEntityPropertyMonitor)self).ValueChanged.Invoke(self));
				ValueChanged.Add(self => ((IEntityPropertyMonitor<T>)self).ValueChanged.Invoke(self));
				ValueChanged.Add(self => ((IPlayerPropertyMonitor)self).ValueChanged.Invoke(self));

				PropertyName = propertyName;
				Player = player;
				Decoder = decoder;

				PlayerResourceEntity = player.World.Entities.Single(e => e?.Class.Classname == "CTFPlayerResource");

				string specificProperty = string.Format("{0}.{1:D3}", PropertyName, Player.EntityIndex);

				var props = PlayerResourceEntity.Properties.Select(prop => prop.Definition.FullName.Remove(prop.Definition.FullName.Length - 4))
					.Except("m_iHealth")
					.Except("m_iPing")
					.Except("m_iScore")
					.Except("m_iDeaths")
					.Except("m_bConnected")
					.Except("m_iTeam")
					.Except("m_bAlive")
					.Distinct();

				InternalPropertyMonitor = new EntityPropertyMonitor<T>(specificProperty, PlayerResourceEntity, Decoder);
				InternalPropertyMonitor.ValueChanged.Add(InternalValueChanged);
			}

			private void InternalValueChanged(IPropertyMonitor p)
			{
				Debug.Assert(InternalPropertyMonitor == p);
				ValueChanged.Invoke(this);
			}
		}

		[DebuggerDisplay("{Value}")]
		class MultiPlayerPropertyMonitor<T> : MultiPropertyMonitor<T>, IPlayerPropertyMonitor<T>
		{
			public Player Player { get; }
			public Entity Entity { get { return Player.Entity; } }

			SingleEvent<Action<IEntityPropertyMonitor>> IEntityPropertyMonitor.ValueChanged { get; } = new SingleEvent<Action<IEntityPropertyMonitor>>();
			SingleEvent<Action<IEntityPropertyMonitor<T>>> IEntityPropertyMonitor<T>.ValueChanged { get; } = new SingleEvent<Action<IEntityPropertyMonitor<T>>>();
			SingleEvent<Action<IPlayerPropertyMonitor>> IPlayerPropertyMonitor.ValueChanged { get; } = new SingleEvent<Action<IPlayerPropertyMonitor>>();
			public new SingleEvent<Action<IPlayerPropertyMonitor<T>>> ValueChanged { get; } = new SingleEvent<Action<IPlayerPropertyMonitor<T>>>();

			public MultiPlayerPropertyMonitor(Player p, IEnumerable<IPropertyMonitor<T>> propertyMonitors) : base(propertyMonitors)
			{
				ValueChanged.Add(self => ((IPropertyMonitor)self).ValueChanged.Invoke(self));
				ValueChanged.Add(self => ((IPropertyMonitor<T>)self).ValueChanged.Invoke(self));
				ValueChanged.Add(self => ((IEntityPropertyMonitor)self).ValueChanged.Invoke(self));
				ValueChanged.Add(self => ((IEntityPropertyMonitor<T>)self).ValueChanged.Invoke(self));
				ValueChanged.Add(self => ((IPlayerPropertyMonitor)self).ValueChanged.Invoke(self));

				Player = p;
			}
		}
	}
}
