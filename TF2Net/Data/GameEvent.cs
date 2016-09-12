using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TF2Net.Data
{
	[DebuggerDisplay("Game event: {Declaration.Name}")]
	public class GameEvent : IReadOnlyGameEvent
	{
		public IReadOnlyGameEventDeclaration Declaration { get; set; }
		public IDictionary<string, object> Values { get; set; }

		IReadOnlyDictionary<string, object> IReadOnlyGameEvent.Values { get { return Values.AsReadOnly(); } }
	}

	public interface IReadOnlyGameEvent
	{
		IReadOnlyGameEventDeclaration Declaration { get; }
		IReadOnlyDictionary<string, object> Values { get; }
	}
}
