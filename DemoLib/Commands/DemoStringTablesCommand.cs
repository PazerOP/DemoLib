using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitSet;

namespace DemoLib.Commands
{
	sealed class DemoStringTablesCommand : TimestampedDemoCommand
	{
		public BitStream Data { get; set; }

		public DemoStringTablesCommand(Stream input) : base(input)
		{
			Type = DemoCommandType.dem_stringtables;

			using (BinaryReader reader = new BinaryReader(input, Encoding.ASCII, true))
			{
				int dataLength = reader.ReadInt32();
				Data = new BitStream(reader.ReadBytes(dataLength));

				/*while (true)
				{
					ulong startCursor = Data.Cursor;

					string test = Data.ReadCString();
					Console.WriteLine(test);

					Data.Cursor = startCursor + 1;
				}*/
			}
		}
	}
}
