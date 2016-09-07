using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoLib.Commands
{
	class DemoDataTablesCommand : TimestampedDemoCommand
	{
		public byte[] Data { get; set; }

		public DemoDataTablesCommand(Stream input) : base(input)
		{
			Type = DemoCommandType.dem_datatables;

			using (BinaryReader reader = new BinaryReader(input, Encoding.ASCII, true))
			{
				int length = reader.ReadInt32();
				Data = reader.ReadBytes(length);
			}
		}
	}
}
