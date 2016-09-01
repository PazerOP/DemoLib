using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		public float[] ViewOrigin1 { get; set; }
		public float[] ViewAngles1 { get; set; }
		public float[] LocalViewAngles1 { get; set; }

		public float[] ViewOrigin2 { get; set; }
		public float[] ViewAngles2 { get; set; }
		public float[] LocalViewAngles2 { get; set; }
	}
}
