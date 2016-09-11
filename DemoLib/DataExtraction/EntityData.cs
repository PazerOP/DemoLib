using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoLib.DataExtraction
{
	public class EntityData
	{
		public uint Tick { get; }

		public EntityData(uint tick)
		{
			Tick = tick;
		}
	}
}
