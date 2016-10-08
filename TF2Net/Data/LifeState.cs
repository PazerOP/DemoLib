using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TF2Net.Data
{
	public enum LifeState
	{
		Alive = 0,

		/// <summary>
		/// playing death animation or still falling off of a ledge waiting to hit ground
		/// </summary>
		Dying = 1,

		/// <summary>
		/// dead. lying still.
		/// </summary>
		Dead = 2,
		Respawnable = 3,
		DiscardBody = 4,
	}
}
