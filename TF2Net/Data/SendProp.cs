using System;
using System.Diagnostics;
using System.Linq;
using TF2Net.Entities;

namespace TF2Net.Data
{
	[DebuggerDisplay("{Definition,nq} :: {Value,nq}")]
	public class SendProp : ICloneable, IDisposable
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly IEntity m_Entity;
		public IEntity Entity
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

		public SingleEvent<Action<SendProp>> ValueChanged { get; } = new SingleEvent<Action<SendProp>>();

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
					Debug.Assert(value?.Equals(m_Value) != true);
					m_Value = value;
					m_LastChangedTick = Entity.World.Tick;
					ValueChanged.Invoke(this);
				}
			}
		}

		public SendProp(IEntity e, SendPropDefinition definition)
		{
			m_Entity = e;
			m_Definition = definition;
		}

		public SendProp Clone(IEntity forEnt)
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
