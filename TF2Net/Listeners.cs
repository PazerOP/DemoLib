using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TF2Net.Data;
using TF2Net.NetMessages;

namespace TF2Net
{
	public class EventListeners
	{
		/// <summary>
		/// Fired when the list of game events has been loaded.
		/// </summary>
		public event Action<NetGameEventListMessage> GameEventsListLoaded;
		internal void GameEventsListLoaded_Fire(NetGameEventListMessage msg) { GameEventsListLoaded(msg); }
		
		public event Action<IReadOnlyGameEvent> GameEventTriggered;
		internal void GameEventTriggered_Fire(IReadOnlyGameEvent e) { GameEventTriggered(e); }
	}
}
