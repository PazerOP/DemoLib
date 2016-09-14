using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TF2Net.Data
{
	public class SignonState
	{
		public ConnectionState State { get; set; }
		public int SpawnCount { get; set; }
	}	
}
