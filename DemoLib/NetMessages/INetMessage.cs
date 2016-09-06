using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoLib.NetMessages
{
	interface INetMessage
	{
		void WriteMsg(byte[] buffer, ref ulong bitOffset);
		void ReadMsg(byte[] buffer, ref ulong bitOffset);

		ulong Size { get; }

		string Description { get; }
	}
}
