using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TF2Net.Data
{
	[DebuggerDisplay("Stringtable: {TableName} ({Entries.Count,nq}/{MaxEntries,nq})")]
	public class StringTable
	{
		public string TableName { get; set; }

		public ushort MaxEntries { get; set; }
		public IList<StringTableEntry> Entries { get; set; }
		
		public ushort? UserDataSize { get; set; }
		public byte? UserDataSizeBits { get; set; }
	}
}
