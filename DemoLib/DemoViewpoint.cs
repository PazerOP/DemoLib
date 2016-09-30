using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TF2Net.Data;

namespace DemoLib
{
	class DemoViewpoint
	{
		[Flags]
		public enum Flags
		{
			None = 0,
			UseOrigin2 = (1 << 0),
			UseAngles2 = (1 << 1),
			NoInterp = (1 << 2),
		}

		public Flags ViewpointFlags { get; set; }

		public Vector ViewOrigin1 { get; set; }
		public Vector ViewAngles1 { get; set; }
		public Vector LocalViewAngles1 { get; set; }

		public Vector ViewOrigin2 { get; set; }
		public Vector ViewAngles2 { get; set; }
		public Vector LocalViewAngles2 { get; set; }
	}
}
