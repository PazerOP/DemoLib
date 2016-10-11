using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TF2Net.Data;

namespace TF2Net.Entities
{
	public class Pill
	{
		public Entity Entity { get; }

		public Pill(Entity e)
		{
			if (e.Class.Classname != "CTFGrenadePipebombProjectile")
				throw new ArgumentException("Entity is not a pill!");
		}
	}
}
