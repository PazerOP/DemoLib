using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoLib.Commands;

namespace DemoLib
{
	public class DemoReader
	{
		public DemoHeader Header { get; private set; }

		public IReadOnlyList<DemoCommand> Commands { get; private set; }

		private DemoReader() { }

		public static DemoReader FromStream(Stream input)
		{
			DemoReader reader = new DemoReader();

			reader.Header = new DemoHeader(input);

			List<DemoCommand> commands = new List<DemoCommand>();

			DemoCommand cmd = null;
			while ((cmd = ParseCommand(input, reader.Header)) != null)
			{
				commands.Add(cmd);
			}

			reader.Commands = commands;

			return reader;
		}

		static DemoCommand ParseCommand(Stream input, DemoHeader header)
		{
			DemoCommandType cmdType = (DemoCommandType)input.ReadByte();

			switch (cmdType)
			{
				case DemoCommandType.dem_signon:		return new DemoSignonCommand(input, (ulong)header.m_SignonLength);
				case DemoCommandType.dem_stringtables:	return new DemoStringTablesCommand(input);
				case DemoCommandType.dem_consolecmd:	return new DemoConsoleCommand(input);
				case DemoCommandType.dem_packet:		return new DemoPacketCommand(input);
				case DemoCommandType.dem_usercmd:		return new DemoUserCommand(input);
				case DemoCommandType.dem_stop:			return null;

				default:
					throw new NotImplementedException(string.Format("Unknown command type {0}", cmdType));
			}
		}
	}
}
