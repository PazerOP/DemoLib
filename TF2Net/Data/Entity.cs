using System.Collections.Generic;
using System.Diagnostics;

namespace TF2Net.Data
{
	[DebuggerDisplay("{Index,nq}: {Class}")]
	public class Entity
	{
		public uint Index { get; }
		public uint SerialNumber { get; }

		public ServerClass Class { get; set; }
		public SendTable NetworkTable { get; set; }

		public IDictionary<SendProp, object> Properties { get; set; } = new Dictionary<SendProp, object>();

		public Entity(uint index, uint serialNumber)
		{
			Index = index;
			SerialNumber = serialNumber;
		}
	}
}
