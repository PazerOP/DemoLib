using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitSet;

namespace DemoLib.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	class NetServerInfoMessage : INetMessage
	{
		/// <summary>
		/// protocol version
		/// </summary>
		public short Protocol { get; set; }

		/// <summary>
		/// number of changelevels since server start
		/// </summary>
		public int ServerCount { get; set; }

		/// <summary>
		/// dedicated server?
		/// </summary>
		public bool IsDedicated { get; set; }

		/// <summary>
		/// HLTV server?
		/// </summary>
		public bool IsHLTV { get; set; }

		public enum OperatingSystem
		{
			Unknown,

			Linux,
			Windows,
		}
		public OperatingSystem OS { get; set; }

		/// <summary>
		/// server map CRC
		/// </summary>
		//public uint MapCRC { get; set; }

		/// <summary>
		/// client.dll CRC server is using
		/// </summary>
		public uint ClientCRC { get; set; }

		/// <summary>
		/// max number of clients on server
		/// </summary>
		public byte MaxClients { get; set; }

		/// <summary>
		/// max number of server classes
		/// </summary>
		public ushort MaxClasses { get; set; }

		/// <summary>
		/// our client slot number
		/// </summary>
		public int PlayerSlot { get; set; }

		/// <summary>
		/// server tick interval
		/// </summary>
		public double TickInterval { get; set; }

		/// <summary>
		/// game directory eg "tf2"
		/// </summary>
		public string GameDirectory { get; set; }
		
		public string MapName { get; set; }

		/// <summary>
		/// Current skybox name
		/// </summary>
		public string SkyName { get; set; }

		/// <summary>
		/// Server name
		/// </summary>
		public string Hostname { get; set; }

		public string Description
		{
			get
			{
				return string.Format("svc_ServerInfo: game \"{0}\", map \"{1}\", max {2}",
					GameDirectory, MapName, MaxClients);
			}
		}

		public ulong Size
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public void ReadMsg(byte[] buffer, ref ulong bitOffset)
		{
			Protocol = BitReader.ReadShort(buffer, ref bitOffset);
			ServerCount = BitReader.ReadInt(buffer, ref bitOffset);
			IsHLTV = BitReader.ReadBool(buffer, ref bitOffset);
			IsDedicated = BitReader.ReadBool(buffer, ref bitOffset);
			ClientCRC = BitReader.ReadUInt(buffer, ref bitOffset);
			MaxClasses = BitReader.ReadUShort(buffer, ref bitOffset);

			// Unknown
			bitOffset += 16 << 3;

			PlayerSlot = BitReader.ReadByte(buffer, ref bitOffset);
			MaxClients = BitReader.ReadByte(buffer, ref bitOffset);
			TickInterval = BitReader.ReadSingle(buffer, ref bitOffset);

			switch (BitReader.ReadChar(buffer, ref bitOffset))
			{
				case 'l':
				case 'L':
				OS = OperatingSystem.Linux;
				break;

				case 'w':
				case 'W':
				OS = OperatingSystem.Windows;
				break;

				default:
				OS = OperatingSystem.Unknown;
				break;
			}

			GameDirectory = BitReader.ReadCString(buffer, ref bitOffset);
			MapName = BitReader.ReadCString(buffer, ref bitOffset);
			SkyName = BitReader.ReadCString(buffer, ref bitOffset);
			Hostname = BitReader.ReadCString(buffer, ref bitOffset);

			// Unknown
			bitOffset += 1;
		}

		public void WriteMsg(byte[] buffer, ref ulong bitOffset)
		{
			throw new NotImplementedException();
		}
	}
}
