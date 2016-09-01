using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoLib.Commands
{
	class TimestampedDemoCommand : DemoCommand
	{
		public int Tick { get; set; }

		public TimestampedDemoCommand(Stream input)
		{
			using (BinaryReader reader = new BinaryReader(input, Encoding.Default, true))
			{
				Tick = reader.ReadInt32();
			}
		}
	}
}
