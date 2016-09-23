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
	public class NetFileMessage : INetMessage
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
		public void ReadMsg(BitStream stream)
		{
			TransferID = stream.ReadUInt();
			Filename = stream.ReadCString();

			Status = (FileStatus)stream.ReadByte(1);
		}

		public void ApplyWorldState(WorldState ws)
		{
		}
	}
}
