using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TF2Net.Data;
using TF2Net.Entities;

namespace TF2Net
{
	public class WorldEvents : IWorldEvents
	{
		public SingleEvent<Action<Entity>> EntityCreated { get; } = new SingleEvent<Action<Entity>>();
		public SingleEvent<Action<Entity>> EntityDeleted { get; } = new SingleEvent<Action<Entity>>();

		public SingleEvent<Action<Entity>> EntityEnteredPVS { get; } = new SingleEvent<Action<Entity>>();
		public SingleEvent<Action<Entity>> EntityLeftPVS { get; } = new SingleEvent<Action<Entity>>();

		public SingleEvent<Action<WorldState>> GameEventsListLoaded { get; } = new SingleEvent<Action<WorldState>>();
		public SingleEvent<Action<WorldState, GameEvent>> GameEvent { get; } = new SingleEvent<Action<WorldState, GameEvent>>();

		public SingleEvent<Action<WorldState>> NewTick { get; } = new SingleEvent<Action<WorldState>>();

		public SingleEvent<Action<Player>> PlayerAdded { get; } = new SingleEvent<Action<Player>>();
		public SingleEvent<Action<Player>> PlayerRemoved { get; } = new SingleEvent<Action<Player>>();

		public SingleEvent<Action<WorldState>> SendTablesLoaded { get; } = new SingleEvent<Action<WorldState>>();

		public SingleEvent<Action<WorldState>> ServerClassesLoaded { get; } = new SingleEvent<Action<WorldState>>();

		public SingleEvent<Action<WorldState>> ServerInfoLoaded { get; } = new SingleEvent<Action<WorldState>>();

		public SingleEvent<Action<WorldState, KeyValuePair<string, string>>> ServerSetConVar { get; } = new SingleEvent<Action<WorldState, KeyValuePair<string, string>>>();

		public SingleEvent<Action<WorldState, string>> ServerConCommand { get; } = new SingleEvent<Action<WorldState, string>>();
		public SingleEvent<Action<WorldState, string>> ServerTextMessage { get; } = new SingleEvent<Action<WorldState, string>>();

		public SingleEvent<Action<StringTable>> StringTableCreated { get; } = new SingleEvent<Action<StringTable>>();
		public SingleEvent<Action<WorldState, StringTable>> StringTableUpdated { get; } = new SingleEvent<Action<WorldState, StringTable>>();

		public SingleEvent<Action<WorldState>> ViewEntityUpdated { get; } = new SingleEvent<Action<WorldState>>();

		public SingleEvent<Action<IBaseEntity>> TempEntityCreated { get; } = new SingleEvent<Action<IBaseEntity>>();
	}
}
