using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

		public event Action<StringTable> StringTableCreated;
		public void OnStringTableCreated(StringTable st) { StringTableCreated?.Invoke(st); }

		public event Action<WorldState, StringTable> StringTableUpdated;
		public void OnStringTableUpdated(WorldState ws, StringTable t) { StringTableUpdated?.Invoke(ws, t); }

		public event Action<Entity> EntityCreated;
		public void OnEntityCreate(Entity e) { EntityCreated?.Invoke(e); }

		event Action<Entity> m_EntityEnteredPVS;
		public event Action<Entity> EntityEnteredPVS
		{
			add
			{
				Debug.Assert(m_EntityEnteredPVS?.GetInvocationList().Contains(value) != true);
				m_EntityEnteredPVS += value;
			}
			remove { m_EntityEnteredPVS -= value; }
		}
		public void OnEntityEnteredPVS(Entity e) { m_EntityEnteredPVS?.Invoke(e); }

		event Action<Entity> m_EntityLeftPVS;
		public event Action<Entity> EntityLeftPVS
		{
			add
			{
				if (m_EntityLeftPVS?.GetInvocationList().Contains(value) == true)
					return;
				m_EntityLeftPVS += value;
			}
			remove { m_EntityLeftPVS -= value; }
		}
		public void OnEntityLeftPVS(Entity e) { m_EntityLeftPVS?.Invoke(e); }

		event Action<Entity> m_EntityDeleted;
		public event Action<Entity> EntityDeleted
		{
			add
			{
				if (m_EntityDeleted?.GetInvocationList().Contains(value) == true)
					return;
				m_EntityDeleted += value;
			}
			remove { m_EntityDeleted -= value; }
		}
		public void OnEntityDeleted(Entity e) { m_EntityDeleted?.Invoke(e); }

		public event Action<Player> PlayerAdded;
		public void OnPlayerAdded(Player p) { PlayerAdded?.Invoke(p); }
		
		public event Action<Player> PlayerRemoved;
		public void OnPlayerRemoved(Player p) { PlayerRemoved?.Invoke(p); }
	}
}
