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
	class NetFileMessage : INetMessage
	{
		public uint TransferID { get; set; }
		public string Filename { get; set; }

		public enum FileStatus
		{
			Denied = 0,
			Requested = 1,
		}

		public FileStatus Status { get; set; }

		public string Description
		{
			get
			{
				return string.Format("net_File: {0} {1}", Filename, Status);
			}
		}

		public ulong Size
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public NetMessageType Type { get { return NetMessageType.NET_FILE; } }

		public void ReadMsg(byte[] buffer, ref ulong bitOffset)
		{
			TransferID = BitReader.ReadUInt32(buffer, ref bitOffset);
			Filename = BitReader.ReadCString(buffer, ref bitOffset);

			Status = (FileStatus)BitReader.ReadUInt1(buffer, ref bitOffset);
		}

		public void WriteMsg(byte[] buffer, ref ulong bitOffset)
		{
			throw new NotImplementedException();
		}
	}
}
