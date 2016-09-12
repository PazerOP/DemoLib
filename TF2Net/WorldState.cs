using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TF2Net.Data;
using TF2Net.NetMessages;

namespace TF2Net
{
	public class WorldState : IReadOnlyWorldState
	{
		public Dictionary<string, string> ConVars { get; } = new Dictionary<string, string>();
		
		public NetClassInfoMessage ServerClassInfo { get; set; }

		public IList<GameEventDeclaration> EventDeclarations { get; set; }
		IReadOnlyList<IReadOnlyGameEventDeclaration> IReadOnlyWorldState.EventDeclarations { get { return EventDeclarations.AsReadOnly(); } }
	}
}
