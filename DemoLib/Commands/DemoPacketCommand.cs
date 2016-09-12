using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using BitSet;
using TF2Net.NetMessages;

namespace DemoLib.Commands
{
	[DebuggerDisplay("{Tick, nq} network packet [{Length, nq}]")]
	class DemoPacketCommand : TimestampedDemoCommand
	{
		public DemoViewpoint Viewpoint { get; set; }

		public int SequenceIn { get; set; }
		public int SequenceOut { get; set; }

		//public IList<INetMessage> Messages { get; set; }

		public BitStream Data { get; set; }

		public DemoPacketCommand(Stream input) : base(input)
		{
			Type = DemoCommandType.dem_packet;

			using (BinaryReader r = new BinaryReader(input, Encoding.ASCII, true))
			{
				Viewpoint = new DemoViewpoint();

				Viewpoint.ViewpointFlags = (DemoViewpoint.Flags)r.ReadInt32();

				Viewpoint.ViewOrigin1 = new float[3];
				for (byte i = 0; i < 3; i++)
					Viewpoint.ViewOrigin1[i] = r.ReadSingle();

				Viewpoint.ViewAngles1 = new float[3];
				for (byte i = 0; i < 3; i++)
					Viewpoint.ViewAngles1[i] = r.ReadSingle();
				
				Viewpoint.LocalViewAngles1 = new float[3];
				for (byte i = 0; i < 3; i++)
					Viewpoint.LocalViewAngles1[i] = r.ReadSingle();


				Viewpoint.ViewOrigin2 = new float[3];
				for (byte i = 0; i < 3; i++)
					Viewpoint.ViewOrigin2[i] = r.ReadSingle();

				Viewpoint.ViewAngles2 = new float[3];
				for (byte i = 0; i < 3; i++)
					Viewpoint.ViewAngles2[i] = r.ReadSingle();

				Viewpoint.LocalViewAngles2 = new float[3];
				for (byte i = 0; i < 3; i++)
					Viewpoint.LocalViewAngles2[i] = r.ReadSingle();

				SequenceIn = r.ReadInt32();
				SequenceOut = r.ReadInt32();

				ulong length = r.ReadUInt32();
				Data = new BitStream(r.ReadBytes((int)length));
			}
		}
	}
}
