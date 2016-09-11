using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoLib.DataExtraction
{
	[DebuggerDisplay("{Classname,nq} ({DatatableName,nq})")]
	public class ServerClass
	{
		public string Classname { get; set; }
		public string DatatableName { get; set; }
	}
}
