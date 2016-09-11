using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoLib.NetMessages;

namespace DemoLib.DataExtraction
{
	public class EventDeclarationsExtractor
	{
		public IReadOnlyList<GameEventDeclaration> Events { get; }

		public EventDeclarationsExtractor(IEnumerable<INetMessage> reader)
		{
			Events = reader.OfType<NetGameEventListMessage>().Single().Events.AsReadOnly();
		}
	}
}
