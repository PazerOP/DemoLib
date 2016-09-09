using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoLib.Commands
{
	sealed class DemoSignonCommand : DemoPacketCommand
	{
		public DemoSignonCommand(DemoReader reader, Stream input, ulong signonLength) : base(reader, input)
		{
			Type = DemoCommandType.dem_signon;
		}
	}
}
