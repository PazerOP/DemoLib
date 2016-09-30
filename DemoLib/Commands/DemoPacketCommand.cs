using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using BitSet;
using TF2Net;
using TF2Net.Data;
using TF2Net.NetMessages;

namespace DemoLib.Commands
{
	[DebuggerDisplay("{Tick, nq} network packet [{Data.Length, nq}]")]
	class DemoPacketCommand : TimestampedDemoCommand
	{
		public DemoViewpoint Viewpoint { get; set; }

		public int SequenceIn { get; set; }
		public int SequenceOut { get; set; }

		public IList<INetMessage> Messages { get; set; }

		public DemoPacketCommand(Stream input) : base(input)
		{
			Type = DemoCommandType.dem_packet;

			using (BinaryReader r = new BinaryReader(input, Encoding.ASCII, true))
			{
				Viewpoint = new DemoViewpoint();

				Viewpoint.ViewpointFlags = (DemoViewpoint.Flags)r.ReadInt32();

				Viewpoint.ViewOrigin1 = new Vector(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
				Viewpoint.ViewAngles1 = new Vector(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());				
				Viewpoint.LocalViewAngles1 = new Vector(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());

				Viewpoint.ViewOrigin2 = new Vector(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
				Viewpoint.ViewAngles2 = new Vector(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
				Viewpoint.LocalViewAngles2 = new Vector(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());

				SequenceIn = r.ReadInt32();
				SequenceOut = r.ReadInt32();
				
				BitStream data = new BitStream(r.ReadBytes((int)r.ReadUInt32()));				
				Messages = NetMessageCoder.Decode(data).ToArray();
			}
		}
	}
}
