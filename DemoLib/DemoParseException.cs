using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoLib
{

	[Serializable]
	public class DemoParseException : Exception
	{
		public DemoParseException(string message) : base(message) { }
		public DemoParseException(string message, Exception inner) : base(message, inner) { }
		protected DemoParseException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
