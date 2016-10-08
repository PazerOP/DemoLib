using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using DemoLib.Commands;
using DemoLib.DataExtraction;
using TF2Net;
using TF2Net.Data;
using TF2Net.NetMessages;

namespace DemoLib
{
	public class DemoReader
	{
		public DemoHeader Header { get; private set; }
		
		public IReadOnlyList<DemoCommand> Commands { get; private set; }

		readonly WorldEvents m_Events = new WorldEvents();
		public IWorldEvents Events { get { return m_Events; } }

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

		public void SimulateDemo()
		{
			m_Events.NewTick.Add(TickTimer);

			WorldState ws = new WorldState();
			ws.Listeners = m_Events;

			foreach (DemoCommand cmd in Commands)
			{
				if (cmd.Type == DemoCommandType.dem_datatables)
				{
					DemoDataTablesCommand dt = (DemoDataTablesCommand)cmd;

					ws.ServerClasses = dt.ServerClasses;
					m_Events.ServerClassesLoaded.Invoke(ws);

					ws.SendTables = dt.SendTables;
					m_Events.SendTablesLoaded.Invoke(ws);
				}
				else if (cmd.Type == DemoCommandType.dem_signon ||
					cmd.Type == DemoCommandType.dem_packet)
				{
					DemoPacketCommand p = (DemoPacketCommand)cmd;

					foreach (INetMessage netMsg in p.Messages)
					{
						netMsg.ApplyWorldState(ws);
					}
				}
			}
		}

		ulong m_FirstTick = ulong.MinValue;
		DateTime m_FirstTickTime;
		private void TickTimer(WorldState ws)
		{
			if (m_FirstTick == ulong.MinValue)
			{
				if (ws.SignonState?.State == ConnectionState.Full)
				{
					m_FirstTick = ws.Tick;
					m_FirstTickTime = DateTime.Now;
				}
				return;
			}

			TimeSpan elapsed = TimeSpan.FromSeconds((ws.Tick - m_FirstTick) * (1.0 / 66.0));
			DateTime expected = m_FirstTickTime + elapsed;
			TimeSpan deltaFromExpected = DateTime.Now - expected;

			if (deltaFromExpected.TotalSeconds < 0)
				Thread.Sleep(-deltaFromExpected);
		}
	}
}
