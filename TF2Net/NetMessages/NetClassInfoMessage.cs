using System;
using System.Collections.Generic;
using System.Diagnostics;
using BitSet;

namespace TF2Net.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	public class NetClassInfoMessage : INetMessage
	{
		public short ServerClassCount { get; set; }
		public bool CreateOnClient { get; set; }

		public class ServerClass
		{
			public ushort ClassID { get; set; }
			public string DataTableName { get; set; }
			public string ClassName { get; set; }
		}
		public IList<ServerClass> ServerClasses { get; set; }

		public string Description
		{
			get
			{
				return string.Format("svc_ClassInfo: num {0}, {1}", ServerClassCount,
					CreateOnClient ? "use client classes" : "full update");
			}
		}

		public void ReadMsg(BitStream stream, IReadOnlyWorldState ws)
		{
			ServerClassCount = stream.ReadShort();

			CreateOnClient = stream.ReadBool();
			if (CreateOnClient)
				return;

			byte serverClassBits = (byte)(ExtMath.Log2(ServerClassCount) + 1);
			ServerClasses = new List<ServerClass>();
			for (int i = 0; i < ServerClassCount; i++)
			{
				ServerClass sc = new ServerClass();

				sc.ClassID = stream.ReadUShort(serverClassBits);
				sc.ClassName = stream.ReadCString();
				sc.DataTableName = stream.ReadCString();

				ServerClasses.Add(sc);
			}
		}

		public void ApplyWorldState(WorldState ws)
		{
			throw new NotImplementedException();
		}
	}
}
