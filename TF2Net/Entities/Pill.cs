using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TF2Net.Data;

namespace TF2Net.Entities
{
	public class Pill : EntityWrapper
	{
		public const string CLASSNAME = "CTFGrenadePipebombProjectile";

		public Pill(Entity e) : base(e, CLASSNAME)
		{
		}
	}
}
