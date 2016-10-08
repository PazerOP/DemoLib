using System;
using System.Diagnostics;
using System.Linq;

namespace TF2Net.Data
{
	[DebuggerDisplay("{Definition,nq} :: {Value,nq}")]
	public class SendProp : ICloneable, IDisposable
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Entity m_Entity;
		public Entity Entity
		{
			get
			{
				CheckDisposed();
				return m_Entity;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly SendPropDefinition m_Definition;
		public SendPropDefinition Definition
		{
			get
			{
				CheckDisposed();
				return m_Definition;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		ulong m_LastChangedTick;
		public ulong LastChangedTick
		{
			get
			{
				CheckDisposed();
				return m_LastChangedTick;
			}
		}

		SingleEvent<Action<SendProp>> m_ValueChanged { get; } = new SingleEvent<Action<SendProp>>();
		public event Action<SendProp> ValueChanged
		{
			add
			{
				CheckDisposed();

				if (Value != null)
					value.Invoke(this);

				m_ValueChanged.Add(value);
			}
			remove { m_ValueChanged.Remove(value); }
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		object m_Value;
		public object Value
		{
			get
			{
				CheckDisposed();
				return m_Value;
			}
			set
			{
				CheckDisposed();
				if (value?.GetHashCode() != m_Value?.GetHashCode() || !value.Equals(m_Value))
				{
					m_Value = value;
					m_LastChangedTick = Entity.World.Tick;
					m_ValueChanged?.Invoke(this);
				}
			}
		}

		public SendProp(Entity e, SendPropDefinition definition)
		{
			m_Entity = e;
			m_Definition = definition;
		}

		public SendProp Clone(Entity forEnt)
		{
			CheckDisposed();
			SendProp cloned = new SendProp(forEnt, Definition);
			cloned.m_Value = Value;
			cloned.m_LastChangedTick = LastChangedTick;
			return cloned;
		}
		public SendProp Clone()
		{
			CheckDisposed();
			return (SendProp)MemberwiseClone();
		}
		object ICloneable.Clone() { return Clone(); }

		void CheckDisposed()
		{
			if (m_Disposed)
				throw new ObjectDisposedException(nameof(SendProp));
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool m_Disposed = false;
		public void Dispose()
		{
			CheckDisposed();
			m_Disposed = true;
		}
	}
}
