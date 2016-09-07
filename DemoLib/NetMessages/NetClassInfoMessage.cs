using System;
using System.Collections.Generic;
using System.Diagnostics;
using BitSet;

namespace DemoLib.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	class NetClassInfoMessage : INetMessage
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

		public ulong Size
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public void ReadMsg(byte[] buffer, ref ulong bitOffset)
		{
			ServerClassCount = BitReader.ReadShort(buffer, ref bitOffset);

			CreateOnClient = BitReader.ReadBool(buffer, ref bitOffset);
			if (CreateOnClient)
				return;

			byte serverClassBits = (byte)(ExtMath.Log2(ServerClassCount) + 1);
			ServerClasses = new List<ServerClass>();
			for (int i = 0; i < ServerClassCount; i++)
			{
				ServerClass sc = new ServerClass();

				sc.ClassID = (ushort)BitReader.ReadUIntBits(buffer, ref bitOffset, serverClassBits);
				sc.ClassName = BitReader.ReadCString(buffer, ref bitOffset);
				sc.DataTableName = BitReader.ReadCString(buffer, ref bitOffset);

				ServerClasses.Add(sc);
			}
		}

		public void WriteMsg(byte[] buffer, ref ulong bitOffset)
		{
			throw new NotImplementedException();
		}
	}
}
