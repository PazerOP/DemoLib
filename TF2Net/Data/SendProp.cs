using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TF2Net.Data
{
	[DebuggerDisplay("{Definition,nq} :: {Value,nq}")]
	public class SendProp : ICloneable
	{
		public WorldState World { get; }

		public SendPropDefinition Definition { get; }

		public ulong LastChangedTick { get; private set; }

		object m_Value;
		public object Value
		{
			get { return m_Value; }
			set
			{
				if (value != m_Value)
				{
					m_Value = value;
					LastChangedTick = World.Tick;
				}
			}
		}

		public SendProp(WorldState ws, SendPropDefinition definition)
		{
			World = ws;
			Definition = definition;
		}

		public SendProp Clone()
		{
			return (SendProp)MemberwiseClone();
		}
		object ICloneable.Clone() { return Clone(); }
	}
}
