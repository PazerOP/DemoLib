using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoLib.NetMessages
{
	class NetBSPDecalMessage : INetMessage
	{
		public string Description
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public ulong Size
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public NetMessageType Type { get { return NetMessageType.SVC_BSPDECAL; } }
		
		public void ReadMsg(byte[] buffer, ref ulong bitOffset)
		{
			throw new NotImplementedException();
		}

		public void WriteMsg(byte[] buffer, ref ulong bitOffset)
		{
			throw new NotImplementedException();
		}
	}
}
