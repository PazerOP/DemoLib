using System;
using System.Collections.Generic;
using TF2Net.Data;
using TF2Net.Entities;

namespace TF2Net
{
	public interface IWorldEvents
	{
		SingleEvent<Action<WorldState>> GameEventsListLoaded { get; }
		SingleEvent<Action<WorldState, IReadOnlyGameEvent>> GameEvent { get; }

		SingleEvent<Action<WorldState>> ServerClassesLoaded { get; }
		SingleEvent<Action<WorldState>> SendTablesLoaded { get; }

		/// <summary>
		/// A "Print Message" command from the server.
		/// </summary>
		SingleEvent<Action<WorldState, string>> ServerTextMessage { get; }

		SingleEvent<Action<WorldState>> ServerInfoLoaded { get; }

		SingleEvent<Action<WorldState>> NewTick { get; }

		SingleEvent<Action<WorldState, KeyValuePair<string, string>>> ServerSetConVar { get; }
		SingleEvent<Action<WorldState, string>> ServerConCommand { get; }

		SingleEvent<Action<WorldState>> ViewEntityUpdated { get; }

		SingleEvent<Action<StringTable>> StringTableCreated { get; }
		SingleEvent<Action<WorldState, StringTable>> StringTableUpdated { get; }

		SingleEvent<Action<Entity>> EntityCreated { get; }
		SingleEvent<Action<Entity>> EntityEnteredPVS { get; }
		SingleEvent<Action<Entity>> EntityLeftPVS { get; }
		SingleEvent<Action<Entity>> EntityDeleted { get; }

		SingleEvent<Action<Player>> PlayerAdded { get; }
		SingleEvent<Action<Player>> PlayerRemoved { get; }

		SingleEvent<Action<IBaseEntity>> TempEntityCreated { get; }
	}
}
