using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TF2Net.NetMessages
{
	static class ExtMath
	{
		public static int Log2(int x)
		{
			int answer = 0;
			while ((x >>= 1) > 0)
				answer++;
			return answer;
		}
	}
}
