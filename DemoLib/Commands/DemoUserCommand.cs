using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoLib.Commands
{
	sealed class DemoUserCommand : TimestampedDemoCommand
	{
		public int OutgoingSequence { get; set; }

		public byte[] Data { get; set; }

		public DemoUserCommand(Stream input) : base(input)
		{
			Type = DemoCommandType.dem_usercmd;

			using (BinaryReader reader = new BinaryReader(input, Encoding.ASCII, true))
			{
				OutgoingSequence = reader.ReadInt32();

				Data = reader.ReadBytes(reader.ReadInt32());
			}
		}
	}
}
