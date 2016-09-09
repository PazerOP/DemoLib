using System;
using System.Collections;
using System.Collections.Generic;

namespace DemoLib
{
	class WTFMicrosoft<T> : IReadOnlyList<T>
	{
		IList<T> Source { get; }
		public WTFMicrosoft(IList<T> source)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));

			Source = source;
		}

		public T this[int index] { get { return Source[index]; } }
		public int Count { get { return Source.Count; } }
		public IEnumerator<T> GetEnumerator() { return Source.GetEnumerator(); }
		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
	}
}
