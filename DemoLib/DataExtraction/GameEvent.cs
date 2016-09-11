using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoLib.DataExtraction
{
	public interface IReadOnlyGameEvent
	{
		GameEventDeclaration Declaration { get; }
		IReadOnlyDictionary<string, object> Values { get; }
	}

	[DebuggerDisplay("Game event: {Declaration.Name}")]
	public class GameEvent : IReadOnlyGameEvent
	{
		public GameEventDeclaration Declaration { get; set; }
		public IDictionary<string, object> Values { get; set; } = new Dictionary<string, object>();
		IReadOnlyDictionary<string, object> IReadOnlyGameEvent.Values { get { return Values.AsReadOnly(); } }
	}
}
