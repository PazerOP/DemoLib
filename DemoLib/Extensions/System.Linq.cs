using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class DemoLib_Extensions
	{
		class ListWrapper<T> : IReadOnlyList<T>
		{
			readonly IList<T> m_Source;
			public ListWrapper(IList<T> src)
			{
				m_Source = src;
			}

			public T this[int index] { get { return m_Source[index]; } }
			public int Count { get { return m_Source.Count; } }
			public IEnumerator<T> GetEnumerator() { return m_Source.GetEnumerator(); }
			IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
		}

		public static IReadOnlyList<T> AsReadOnlyList<T>(this IList<T> input)
		{
			if (input == null)
				throw new ArgumentNullException(nameof(input));

			return new ListWrapper<T>(input);
		}
	}
}
