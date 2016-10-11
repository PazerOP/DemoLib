using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TF2Net.Data;
using TF2Net.Monitors;

namespace TF2Net.Entities
{
	public class TFRocket : IEquatable<TFRocket>, IEquatable<Entity>
	{
		public Entity Entity { get; }

		public IEntityPropertyMonitor<Vector> Position { get; }
		public IEntityPropertyMonitor<Vector> Angle { get; }
		public IEntityPropertyMonitor<Team?> Team { get { return Entity.Team; } }

		public TFRocket(Entity e)
		{
			if (e.Class.Classname != "CTFProjectile_Rocket")
				throw new ArgumentException("Entity is not a rocket!");

			Entity = e;

			Position = new EntityPropertyMonitor<Vector>("DT_TFBaseRocket.m_vecOrigin", Entity, o => (Vector)o);
			Angle = new EntityPropertyMonitor<Vector>("DT_TFBaseRocket.m_angRotation", Entity, o => (Vector)o);
		}

		public bool Equals(TFRocket other)
		{
			return Entity.Equals(other?.Entity);
		}
		public override bool Equals(object obj)
		{
			// Entity
			{
				Entity e = obj as Entity;
				if (e != null)
					return Equals(e);
			}

			// TFRocket
			{
				TFRocket r = obj as TFRocket;
				if (r != null)
					return Equals(r);
			}

			return false;
		}
		public override int GetHashCode()
		{
			return Entity.GetHashCode();
		}

		public bool Equals(Entity other)
		{
			return ((IEquatable<Entity>)Entity).Equals(other);
		}
	}
}
