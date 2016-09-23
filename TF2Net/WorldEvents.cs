using System;
using System.Collections.Generic;
using TF2Net.Data;

namespace TF2Net
{
	public class WorldEvents : IWorldEvents
	{
		public event Action<WorldState> GameEventsListLoaded;
		public void OnGameEventsListLoaded(WorldState ws) { GameEventsListLoaded?.Invoke(ws); }

		public event Action<WorldState, IReadOnlyGameEvent> GameEvent;
		public void OnGameEvent(WorldState ws, IReadOnlyGameEvent e) { GameEvent?.Invoke(ws, e); }

		public event Action<WorldState> SendTablesLoaded;
		public void OnSendTablesLoaded(WorldState ws) { SendTablesLoaded?.Invoke(ws); }

		public event Action<WorldState> ServerClassesLoaded;
		public void OnServerClassesLoaded(WorldState ws) { ServerClassesLoaded?.Invoke(ws); }

		public event Action<WorldState, string> ServerTextMessage;
		public void OnServerTextMessage(WorldState ws, string msg) { ServerTextMessage?.Invoke(ws, msg); }

		public event Action<WorldState> ServerInfoLoaded;
		public void OnServerInfoLoaded(WorldState ws) { ServerInfoLoaded?.Invoke(ws); }

		public event Action<WorldState> NewTick;
		public void OnNewTick(WorldState ws) { NewTick?.Invoke(ws); }

		public event Action<WorldState, KeyValuePair<string, string>> ServerSetConVar;
		public void OnServerSetConVar(WorldState ws, KeyValuePair<string, string> cvar) { ServerSetConVar?.Invoke(ws, cvar); }

		public event Action<WorldState, string> ServerConCommand;
		public void OnServerConCommand(WorldState ws, string cmd) { ServerConCommand?.Invoke(ws, cmd); }

		public event Action<WorldState> ViewEntityUpdated;
		public void OnViewEntityUpdated(WorldState ws) { ViewEntityUpdated?.Invoke(ws); }

		public event Action<WorldState, StringTable> StringTableUpdated;
		public void OnStringTableUpdated(WorldState ws, StringTable t) { StringTableUpdated?.Invoke(ws, t); }
	}
}
