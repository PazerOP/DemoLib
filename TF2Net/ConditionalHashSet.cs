using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace TF2Net
{
	sealed class ConditionalHashSet<T> where T : class
	{
		private readonly object locker = new object();
		private readonly List<WeakReference> weakList = new List<WeakReference>();
		private readonly ConditionalWeakTable<T, WeakReference> weakDictionary =
			new ConditionalWeakTable<T, WeakReference>();

		public void Add(T item)
		{
			lock (locker)
			{
				var reference = new WeakReference(item);
				weakDictionary.Add(item, reference);
				weakList.Add(reference);
				Shrink();
			}
		}

		public void Remove(T item)
		{
			lock (locker)
			{
				WeakReference reference;

				if (weakDictionary.TryGetValue(item, out reference))
				{
					reference.Target = null;
					weakDictionary.Remove(item);
				}
			}
		}

		public T[] ToArray()
		{
			lock (locker)
			{
				return (
					from weakReference in weakList
					let item = (T)weakReference.Target
					where item != null
					select item)
					.ToArray();
			}
		}

		private void Shrink()
		{
			// This method prevents the List<T> from growing indefinitely, but 
			// might also cause  a performance problem in some cases.
			if (weakList.Capacity == weakList.Count)
			{
				weakList.RemoveAll(weak => !weak.IsAlive);
			}
		}
	}
}
