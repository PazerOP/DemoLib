using System.Diagnostics;
using System.IO;
using System.Text;

namespace DemoLib.Commands
{
	[DebuggerDisplay("{Tick, nq} dem_usercmd")]
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
