using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoLib;

namespace DemoLib.Launcher
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var stream = File.Open(@"D:\Steam\steamapps\common\Team Fortress 2\tf\demos\2016-07-14_18-13-36.dem", FileMode.Open, FileAccess.Read))
			{
				DemoReader reader = DemoReader.FromStream(stream);
			}
		}
	}
}
