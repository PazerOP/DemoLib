using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoLib.Commands
{
	sealed class DemoSignonCommand : TimestampedDemoCommand
	{
		public DemoSignonCommand(Stream input, ulong signonLength) : base(input)
		{
			Type = DemoCommandType.dem_signon;

			byte[] data = new byte[signonLength];
			var read = input.Read(data, 0, (int)signonLength);
			if (read != (int)signonLength)
				throw new ArgumentOutOfRangeException(nameof(signonLength));
			Data = data;
		}

		public byte[] Data { get; set; }
	}
}
