using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DemoLib.Commands;
using DemoLib.DataExtraction;
using DemoLib.NetMessages;

namespace DemoLib
{
	public class DemoReader
	{
		public DemoHeader Header { get; private set; }

		public IReadOnlyList<GameEventDeclaration> GameEventDeclarations { get; private set; }
		public ILookup<int, IReadOnlyGameEvent> GameEvents { get; private set; }

		public IReadOnlyList<DemoCommand> Commands { get; private set; }

		public IEnumerable<INetMessage> NetMessages { get { return Commands.OfType<DemoPacketCommand>().SelectMany(c => c.Messages); } }

		public IReadOnlyList<SendTable> SendTables { get; private set; }

		private DemoReader(Stream input)
		{
			Header = new DemoHeader(input);

			List<DemoCommand> commands = new List<DemoCommand>();
			Commands = commands;

			DemoCommand cmd = null;
			while ((cmd = ParseCommand(input)) != null)
				commands.Add(cmd);

			GameEvents = Commands.OfType<DemoPacketCommand>()
				.SelectMany(c => c.Messages.OfType<NetGameEventMessage>(), (c, col) => Tuple.Create(c.Tick, (IReadOnlyGameEvent)col.Event))
				.ToLookup(k => k.Item1, v => v.Item2);
		}

		public static DemoReader FromStream(Stream input)
		{
			return new DemoReader(input);
		}

		static IReadOnlyList<GameEventDeclaration> GetGameEventsList(DemoPacketCommand cmd)
		{
			if (cmd == null)
				throw new ArgumentNullException(nameof(cmd));

			var events = cmd.Messages
					.Where(m => m is NetGameEventListMessage).Cast<NetGameEventListMessage>()
					.SingleOrDefault()?.Events;

			return events != null ? events.AsReadOnly() : null;
		}

		DemoCommand ParseCommand(Stream input)
		{
			DemoCommandType cmdType = (DemoCommandType)input.ReadByte();

			if (cmdType == DemoCommandType.dem_packet || cmdType == DemoCommandType.dem_signon)
			{
				DemoPacketCommand cmd = null;
				if (cmdType == DemoCommandType.dem_packet)
					cmd = new DemoPacketCommand(this, input);
				else if (cmdType == DemoCommandType.dem_signon)
					cmd = new DemoSignonCommand(this, input, (ulong)Header.m_SignonLength);

				{
					NetGameEventListMessage found = cmd.Messages.OfType<NetGameEventListMessage>().SingleOrDefault();
					if (found != null)
					{
						Debug.Assert(GameEventDeclarations == null);
						GameEventDeclarations = ((NetGameEventListMessage)cmd.Messages.Single(m => m is NetGameEventListMessage)).Events.AsReadOnly();
					}
				}

				return cmd;
			}
			else if (cmdType == DemoCommandType.dem_datatables)
			{
				DemoDataTablesCommand dt = new DemoDataTablesCommand(input);
				Debug.Assert(SendTables == null);
				SendTables = dt.SendTables.AsReadOnly();
				return dt;
			}
			else
			{
				switch (cmdType)
				{
					case DemoCommandType.dem_synctick: return new DemoSyncTickCommand(input);
					case DemoCommandType.dem_consolecmd: return new DemoConsoleCommand(input);
					case DemoCommandType.dem_usercmd: return new DemoUserCommand(input);
					case DemoCommandType.dem_stop: return null;
					case DemoCommandType.dem_stringtables: return new DemoStringTablesCommand(input);
					
					default:
						throw new NotImplementedException(string.Format("Unknown command type {0}", cmdType));
				}
			}
		}
	}
}
