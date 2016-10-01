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
		public Entity Entity { get; }
		public SendPropDefinition Definition { get; }

		public ulong LastChangedTick { get; private set; }

		event Action<SendProp> m_ValueChanged;
		public event Action<SendProp> ValueChanged
		{
			add
			{
				if (m_ValueChanged?.GetInvocationList().Contains(value) == true)
					return;

				m_ValueChanged += value;

				if (Value != null)
					value.Invoke(this);
			}
			remove { m_ValueChanged -= value; }
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		object m_Value;

		public object Value
		{
			get { return m_Value; }
			set
			{
				if (value?.GetHashCode() != m_Value?.GetHashCode() || !value.Equals(m_Value))
				{
					m_Value = value;
					LastChangedTick = Entity.World.Tick;
					m_ValueChanged?.Invoke(this);
				}
			}
		}

		public SendProp(Entity e, SendPropDefinition definition)
		{
			Entity = e;
			Definition = definition;
		}

		public SendProp Clone(Entity forEnt)
		{
			SendProp cloned = new SendProp(forEnt, Definition);
			cloned.m_Value = Value;
			cloned.LastChangedTick = LastChangedTick;
			return cloned;
		}
		public SendProp Clone()
		{
			return (SendProp)MemberwiseClone();
		}
		object ICloneable.Clone() { return Clone(); }
	}
}
