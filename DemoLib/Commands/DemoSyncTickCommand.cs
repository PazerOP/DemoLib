using System.Diagnostics;
using System.IO;

namespace DemoLib.Commands
{
	[DebuggerDisplay("{Tick, nq} dem_synctick")]
	sealed class DemoSyncTickCommand : TimestampedDemoCommand
	{
		public DemoSyncTickCommand(Stream input) : base(input)
		{
			Type = DemoCommandType.dem_synctick;
		}
	}
}
