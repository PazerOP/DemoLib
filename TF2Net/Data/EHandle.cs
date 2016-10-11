using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TF2Net.Entities;

namespace TF2Net.Data
{
	[DebuggerDisplay("{Entity,nq}")]
	public class EHandle
	{
		public WorldState World { get; }
		public uint EntityIndex { get; }
		public uint SerialNumber { get; }

		public Entity Entity
		{
			get
			{
				Entity potential = World.Entities[EntityIndex];
				if (potential?.SerialNumber == SerialNumber)
					return potential;

				return null;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Entity DebugEntity { get { return Entity; } }

		public EHandle(WorldState ws, uint handle)
		{
			World = ws;

			EntityIndex = handle & ((1 << SourceConstants.MAX_EDICT_BITS) - 1);
			SerialNumber = handle >> SourceConstants.MAX_EDICT_BITS;
		}

		public override string ToString()
		{
			return Entity?.ToString() ?? string.Format("null {0}", nameof(EHandle));
		}
	}
}
