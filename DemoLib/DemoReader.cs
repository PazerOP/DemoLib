using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DemoLib.Commands;
using DemoLib.DataExtraction;
using TF2Net;
using TF2Net.Data;
using TF2Net.NetMessages;

namespace DemoLib
{
	public class DemoReader : IEnumerable<WorldState>
	{
		public DemoHeader Header { get; private set; }
		
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

		DemoCommand ParseCommand(Stream input)
		{
			DemoCommandType cmdType = (DemoCommandType)input.ReadByte();

			switch (cmdType)
			{
				case DemoCommandType.dem_signon: return new DemoSignonCommand(input, (ulong)Header.m_SignonLength);
				case DemoCommandType.dem_packet: return new DemoPacketCommand(input);
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

		public IEnumerator<WorldState> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
	}
}
