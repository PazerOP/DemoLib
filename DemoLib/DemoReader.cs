using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoLib.Commands;
using DemoLib.DataExtraction;
using DemoLib.NetMessages;

namespace DemoLib
{
	public class DemoReader
	{
		public DemoHeader Header { get; private set; }

		public IReadOnlyList<GameEvent> GameEventDeclarations { get; private set; }

		public IReadOnlyList<DemoCommand> Commands { get; private set; }

		private DemoReader(Stream input)
		{
			Header = new DemoHeader(input);

			List<DemoCommand> commands = new List<DemoCommand>();
			Commands = commands;

			DemoCommand cmd = null;
			while ((cmd = ParseCommand(input)) != null)
				commands.Add(cmd);
		}

		public static DemoReader FromStream(Stream input)
		{
			return new DemoReader(input);
		}

		static IReadOnlyList<GameEvent> GetGameEventsList(DemoPacketCommand cmd)
		{
			if (cmd == null)
				throw new ArgumentNullException(nameof(cmd));

			var events = cmd.Messages
					.Where(m => m is NetGameEventListMessage).Cast<NetGameEventListMessage>()
					.SingleOrDefault()?.Events;

			return events != null ? new WTFMicrosoft<GameEvent>(events) : null;
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

				if (cmd.Messages.Any(m => m is NetGameEventListMessage))
				{
					Debug.Assert(GameEventDeclarations == null);
					GameEventDeclarations = ((NetGameEventListMessage)cmd.Messages.Single(m => m is NetGameEventListMessage)).Events.AsReadOnlyList();
				}

				return cmd;
			}
			else
			{
				switch (cmdType)
				{
					case DemoCommandType.dem_synctick: return new DemoSyncTickCommand(input);
					case DemoCommandType.dem_consolecmd: return new DemoConsoleCommand(input);
					case DemoCommandType.dem_usercmd: return new DemoUserCommand(input);
					case DemoCommandType.dem_datatables: return new DemoDataTablesCommand(input);
					case DemoCommandType.dem_stop: return null;
					case DemoCommandType.dem_stringtables: return new DemoStringTablesCommand(input);
					
					default:
						throw new NotImplementedException(string.Format("Unknown command type {0}", cmdType));
				}
			}
		}
	}
}
