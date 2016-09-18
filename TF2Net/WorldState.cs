using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TF2Net.Data;
using TF2Net.NetMessages;

namespace TF2Net
{
	public class WorldState
	{
		public WorldEvents Listeners { get; set; }

		public ulong Tick { get; set; }

		public double LastFrameTime { get; set; }
		public double LastFrameTimeStdDev { get; set; }

		public SignonState SignonState { get; set; }

		public ServerInfo ServerInfo { get; set; }

		public SortedSet<Entity> Entities { get; }

		public IList<StringTable> StringTables { get; set; } = new List<StringTable>();

		public IDictionary<string, string> ConVars { get; }

		public byte ClassBits { get { return (byte)Math.Ceiling(Math.Log(ServerClasses.Count, 2)); } }
		
		public IList<ServerClass> ServerClasses { get; set; }
		public IList<SendTable> SendTables { get; set; }

		public IList<GameEventDeclaration> EventDeclarations { get; set; }

		public ushort? ViewEntity { get; set; }

		public WorldState()
		{
			Entities = new SortedSet<Entity>(
				Comparer<Entity>.Create((x, y) => Comparer<uint>.Default.Compare(x.Index, y.Index)));

			ConVars = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		}
	}
}
