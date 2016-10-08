using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TF2Net
{
	[DebuggerDisplay("{DebugCount} Delegates")]
	public class SingleEvent<T>
	{
		readonly Lazy<ConcurrentDictionary<Delegate, object>> m_Delegates = new Lazy<ConcurrentDictionary<Delegate, object>>();

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		uint DebugCount { get { return m_Delegates.IsValueCreated ? (uint)m_Delegates.Value.Count : 0; } }

		public SingleEvent()
		{
			if (!typeof(T).IsSubclassOf(typeof(Delegate)))
				throw new InvalidOperationException(typeof(T).Name + " is not a delegate type");
		}

		public bool Add(T input)
		{
			Delegate forceCast = (Delegate)(object)input;

			return m_Delegates.Value.TryAdd(forceCast, null);
		}
		public bool Remove(T input)
		{
			Delegate forceCast = (Delegate)(object)input;

			object dummy;
			return m_Delegates.Value.TryRemove(forceCast, out dummy);
		}

		public void Invoke(params object[] args)
		{
			foreach (Delegate d in m_Delegates.Value.Keys)
			{
				var test = d.Target;
				d.DynamicInvoke(args);
			}
		}
	}
}
