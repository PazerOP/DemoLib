using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoLib.Commands
{
	public class DemoCommand
	{
		public DemoCommandType Type { get; protected set; } = DemoCommandType.dem_invalid;
	}
}
