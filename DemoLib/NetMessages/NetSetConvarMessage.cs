using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitSet;

namespace DemoLib.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	class NetSetConvarMessage : INetMessage
	{
		public class Cvar
		{
			public string Name { get; set; }
			public string Value { get; set; }
		}
		public IList<Cvar> Cvars { get; set; }

		public string Description
		{
			get
			{
				return string.Format("net_SetConVar: {0} cvars, \"{1}\"=\"{2}\"",
					Cvars.Count, Cvars[0].Name, Cvars[0].Value);
			}
		}

		public ulong Size
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public void ReadMsg(DemoReader reader, byte[] buffer, ref ulong bitOffset)
		{
			byte count = BitReader.ReadByte(buffer, ref bitOffset);

			Cvars = new List<Cvar>(count);
			for (int i = 0; i < count; i++)
			{
				Cvar c = new Cvar();
				c.Name = BitReader.ReadCString(buffer, ref bitOffset);
				c.Value = BitReader.ReadCString(buffer, ref bitOffset);
				Cvars.Add(c);
			}
		}

		public void WriteMsg(byte[] buffer, ref ulong bitOffset)
		{
			throw new NotImplementedException();
		}
	}
}
