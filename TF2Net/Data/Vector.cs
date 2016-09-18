using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TF2Net.Data
{
	[DebuggerDisplay("{ToString(),nq}")]
	public class Vector
	{
		public double X { get; set; }
		public double Y { get; set; }
		public double Z { get; set; }

		public Vector() { }
		public Vector(double x, double y, double z = 0)
		{
			X = x;
			Y = y;
			Z = z;
		}
		public Vector(double[] xyz)
		{
			if (xyz == null)
				throw new ArgumentNullException(nameof(xyz));
			if (xyz.Length != 3)
				throw new ArgumentException("Array is not of length 3", nameof(xyz));

			X = xyz[0];
			Y = xyz[1];
			Z = xyz[2];
		}

		public override string ToString()
		{
			return string.Format("{0}: ({1} {2} {3})", nameof(Vector), X, Y, Z);
		}
	}
}
