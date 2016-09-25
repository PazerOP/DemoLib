using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TF2Net.Data
{
	[DebuggerDisplay("{FullName,nq}")]
	public class FlattenedProp
	{
		public string FullName { get; set; }

		public SendProp Property { get; set; }
	}
}
