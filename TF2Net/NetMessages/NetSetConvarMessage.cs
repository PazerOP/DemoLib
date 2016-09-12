using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitSet;

namespace TF2Net.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	public class NetSetConvarMessage : INetMessage
	{
		public IList<KeyValuePair<string, string>> Cvars { get; set; }

		public string Description
		{
			get
			{
				return string.Format("net_SetConVar: {0} cvars, \"{1}\"=\"{2}\"",
					Cvars.Count, Cvars[0].Key, Cvars[0].Value);
			}
		}

		public ulong Size
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public void WriteMsg(byte[] buffer, ref ulong bitOffset)
		{
			throw new NotImplementedException();
		}

		public void ReadMsg(BitStream stream, IReadOnlyWorldState ws)
		{
			byte count = stream.ReadByte();

			Cvars = new List<KeyValuePair<string, string>>(count);
			for (int i = 0; i < count; i++)
			{
				string name = stream.ReadCString();
				string value = stream.ReadCString();
				Cvars.Add(new KeyValuePair<string, string>(name, value));
			}
		}

		public void ApplyWorldState(WorldState ws)
		{
			throw new NotImplementedException();
		}
	}
}
