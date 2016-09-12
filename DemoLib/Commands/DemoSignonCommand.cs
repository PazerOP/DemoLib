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
		public DemoSignonCommand(Stream input, ulong signonLength) : base(input)
		{
			Type = DemoCommandType.dem_signon;
		}
	}
}
