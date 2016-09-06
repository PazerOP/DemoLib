using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using DemoLib.NetMessages;

namespace DemoLib.Commands
{
	[DebuggerDisplay("{Tick, nq} network packet [{Length, nq}]")]
	sealed class DemoPacketCommand : TimestampedDemoCommand
	{
		public DemoViewpoint Viewpoint { get; set; }

		public int SequenceIn { get; set; }
		public int SequenceOut { get; set; }

		public List<INetMessage> Messages { get; set; }

		public uint Length { get; set; }

		public DemoPacketCommand(Stream input) : base(input)
		{
			Type = DemoCommandType.dem_packet;

			using (BinaryReader reader = new BinaryReader(input, Encoding.ASCII, true))
			{
				Viewpoint = new DemoViewpoint();

				Viewpoint.ViewpointFlags = (DemoViewpoint.Flags)reader.ReadInt32();

				Viewpoint.ViewOrigin1 = new float[3];
				for (byte i = 0; i < 3; i++)
					Viewpoint.ViewOrigin1[i] = reader.ReadSingle();

				Viewpoint.ViewAngles1 = new float[3];
				for (byte i = 0; i < 3; i++)
					Viewpoint.ViewAngles1[i] = reader.ReadSingle();
				
				Viewpoint.LocalViewAngles1 = new float[3];
				for (byte i = 0; i < 3; i++)
					Viewpoint.LocalViewAngles1[i] = reader.ReadSingle();


				Viewpoint.ViewOrigin2 = new float[3];
				for (byte i = 0; i < 3; i++)
					Viewpoint.ViewOrigin2[i] = reader.ReadSingle();

				Viewpoint.ViewAngles2 = new float[3];
				for (byte i = 0; i < 3; i++)
					Viewpoint.ViewAngles2[i] = reader.ReadSingle();

				Viewpoint.LocalViewAngles2 = new float[3];
				for (byte i = 0; i < 3; i++)
					Viewpoint.LocalViewAngles2[i] = reader.ReadSingle();

				SequenceIn = reader.ReadInt32();
				SequenceOut = reader.ReadInt32();

				Length = reader.ReadUInt32();
				Messages = NetMessageCoder.Decode(reader.ReadBytes((int)Length));
			}
		}
	}
}
