using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TF2Net.Data
{
	[DebuggerDisplay("{ToString(),nq}")]
	public class FlattenedProp
	{
		public string FullName { get; set; }

		public SendProp Property { get; set; }

		public override string ToString()
		{
			string bitCount = (Property.BitCount.HasValue && Property.BitCount.Value > 0) ? string.Format("[{0}]", Property.BitCount.Value) : string.Empty;

			return string.Format("{0}{1} \"{2}\" ({3})", Property.Type, bitCount, FullName, Property.Flags);
		}
	}
}
