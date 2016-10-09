using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TF2Net
{
	public class SingleEvent<T> where T : class
	{
		readonly List<WeakReference> m_Keys = new List<WeakReference>();
		readonly ConditionalWeakTable<object, List<Delegate>> m_Delegates = new ConditionalWeakTable<object, List<Delegate>>();

		public SingleEvent()
		{
			if (!typeof(T).IsSubclassOf(typeof(Delegate)))
				throw new InvalidOperationException(typeof(T).Name + " is not a delegate type");
		}

		public bool Add(T input)
		{
			CleanKeysList();
			Delegate forceCast = (Delegate)(object)input;

			object target = forceCast.Target;
			Debug.Assert(target != null);

			List<Delegate> delegates = m_Delegates.GetValue(target, key =>
			{
				lock (m_Keys)
				{
					if (!m_Keys.Contains(target))
						m_Keys.Add(new WeakReference(target));
				}

				return new List<Delegate>();
			});

			lock (delegates)
			{
				if (delegates.Contains(forceCast))
					return false;

				delegates.Add(forceCast);
			}

			return true;
		}
		public bool Remove(T input)
		{
			Delegate forceCast = (Delegate)(object)input;

			object target = forceCast.Target;
			Debug.Assert(target != null);

			List<Delegate> values;
			m_Delegates.TryGetValue(target, out values);

			lock (values)
				return values.Remove(forceCast);
		}

		void CleanKeysList()
		{
			lock (m_Keys)
			{
				for (int i = 0; i < m_Keys.Count; i++)
				{
					if (!m_Keys[i].IsAlive)
						m_Keys.RemoveAt(i--);
				}
			}
		}

		public void Invoke(params object[] args)
		{
			if (m_Keys.Count > 0)
			{
				IEnumerable<Delegate> all = Enumerable.Empty<Delegate>();

				lock (m_Keys)
				{
					IEnumerable<object> validObjects = m_Keys.Select(k => k.Target).Where(t => t != null);
					foreach (object o in validObjects)
					{
						List<Delegate> delegates;
						if (m_Delegates.TryGetValue(o, out delegates))
							all = all.Concat(delegates);
					}
				}

				foreach (Delegate d in all)
				{
					var test = d.Target;
					d.DynamicInvoke(args);
				}
			}
		}
	}
}
