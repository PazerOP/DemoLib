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

		public bool Equals(ServerClass other)
		{
			return (
				Classname == other.Classname &&
				DatatableName == other.DatatableName);
		}
		public override bool Equals(object obj)
		{
			ServerClass cast = obj as ServerClass;
			return (cast != null ? Equals(cast) : false);
		}
		public override int GetHashCode()
		{
			return unchecked(Classname.GetHashCode() + DatatableName.GetHashCode());
		}
	}
}
