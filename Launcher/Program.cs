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
				Console.WriteLine("Parsing test demo file...");

				DateTime start = DateTime.Now;
				DemoReader reader = DemoReader.FromStream(stream);
				DateTime end = DateTime.Now;

				Console.WriteLine("Finished parsing test demo file in {0:N1}ms.", (end - start).TotalMilliseconds);

				Console.ReadLine();
			}
		}
	}
}
