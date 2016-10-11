using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TF2Net.Data
{
	public interface IReadOnlyVector : ICloneable
	{
		double X { get; }
		double Y { get; }
		double Z { get; }
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class IReadOnlyVectorExtensions
	{
		public static Vector Clone(this IReadOnlyVector v)
		{
			return new Vector(v.X, v.Y, v.Z);
		}
	}
}
