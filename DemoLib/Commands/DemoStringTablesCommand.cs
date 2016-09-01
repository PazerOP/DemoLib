using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoLib.Commands
{
	sealed class DemoStringTablesCommand : TimestampedDemoCommand
	{
		public DemoStringTablesCommand(Stream input) : base(input)
		{
			Type = DemoCommandType.dem_stringtables;

			using (BinaryReader reader = new BinaryReader(input, Encoding.ASCII, true))
			{
				int dataLength = reader.ReadInt32();
				Data = reader.ReadBytes(dataLength);
			}
		}

		public byte[] Data { get; set; }
	}
}
