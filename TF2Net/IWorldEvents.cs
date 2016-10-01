using System;
using System.Collections.Generic;
using TF2Net.Data;

namespace TF2Net
{
	public interface IWorldEvents
	{
		event Action<WorldState> GameEventsListLoaded;
		event Action<WorldState, IReadOnlyGameEvent> GameEvent;

		event Action<WorldState> ServerClassesLoaded;
		event Action<WorldState> SendTablesLoaded;

		/// <summary>
		/// A "Print Message" command from the server.
		/// </summary>
		event Action<WorldState, string> ServerTextMessage;

		event Action<WorldState> ServerInfoLoaded;

		event Action<WorldState> NewTick;

		event Action<WorldState, KeyValuePair<string, string>> ServerSetConVar;
		event Action<WorldState, string> ServerConCommand;

		event Action<WorldState> ViewEntityUpdated;

		event Action<StringTable> StringTableCreated;
		event Action<WorldState, StringTable> StringTableUpdated;

		event Action<Entity> EntityCreated;
		event Action<Entity> EntityEnteredPVS;
		event Action<Entity> EntityLeftPVS;
		event Action<Entity> EntityDeleted;

		event Action<Player> PlayerAdded;
		event Action<Player> PlayerRemoved;
	}
}
