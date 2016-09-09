using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoLib.DataExtraction
{
	[DebuggerDisplay("Game event: {Name}")]
	public class GameEvent
	{
		public int ID { get; set; }
		public string Name { get; set; }

		public enum Type
		{
			Local = 0, // not networked
			String,    // zero terminated ASCII string
			Float,     // float 32 bit
			Long,      // signed int 32 bit
			Short,     // signed int 16 bit
			Byte,      // unsigned int 8 bit
			Bool,      // unsigned int 1 bit
		};

		public IDictionary<string, Type> Values { get; set; }
	}
}
