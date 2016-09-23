using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TF2Net.Data
{
	[DebuggerDisplay("{ToString(),nq}")]
	public class ServerClass
	{
		public string Classname { get; set; }
		public string DatatableName { get; set; }

		public override string ToString()
		{
			return string.Format("{0} ({1})", Classname, DatatableName);
		}
	}
}
