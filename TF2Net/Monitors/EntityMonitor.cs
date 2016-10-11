using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TF2Net.Data;
using TF2Net.Entities;

namespace TF2Net.Monitors
{
	public class EntityMonitor
	{
		public SingleEvent<Action<EntityMonitor, Entity>> EnteredPVS { get; } = new SingleEvent<Action<EntityMonitor, Entity>>();
		public SingleEvent<Action<EntityMonitor, Entity>> LeftPVS { get; } = new SingleEvent<Action<EntityMonitor, Entity>>();

		public WorldState World { get; }
		public string ClassName { get; }

		public EntityMonitor(WorldState ws, string classname)
		{
			World = ws;
			ClassName = classname;

			World.Listeners.EntityEnteredPVS.Add(Entity_EnteredPVS);
			World.Listeners.EntityLeftPVS.Add(Entity_LeftPVS);
		}

		void Entity_EnteredPVS(Entity e)
		{
			if (e.Class.Classname == ClassName)
				EnteredPVS.Invoke(this, e);
		}
		void Entity_LeftPVS(Entity e)
		{
			if (e.Class.Classname == ClassName)
				LeftPVS.Invoke(this, e);
		}
	}
}
