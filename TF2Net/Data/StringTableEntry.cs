using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TF2Net.Data
{
	[DebuggerDisplay("{ID,nq}: {Value}")]
	public class StringTableEntry
	{
		public ushort ID { get; set; }
		public string Value { get; set; }
		public byte[] UserData { get; set; }
	}
}
