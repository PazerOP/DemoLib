using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitSet;

namespace TF2Net.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	public class NetServerInfoMessage : INetMessage
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

		public void ReadMsg(BitStream stream, IReadOnlyWorldState ws)
		{
			Protocol = stream.ReadShort();
			ServerCount = stream.ReadInt();
			IsHLTV = stream.ReadBool();
			IsDedicated = stream.ReadBool();
			ClientCRC = stream.ReadUInt();
			MaxClasses = stream.ReadUShort();

			// Unknown
			stream.Seek(16 << 3, System.IO.SeekOrigin.Current);

			PlayerSlot = stream.ReadByte();
			MaxClients = stream.ReadByte();
			TickInterval = stream.ReadSingle();

			switch (stream.ReadChar())
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

			GameDirectory = stream.ReadCString();
			MapName = stream.ReadCString();
			SkyName = stream.ReadCString();
			Hostname = stream.ReadCString();

			// Unknown
			stream.Seek(1, System.IO.SeekOrigin.Current);
		}

		public void ApplyWorldState(WorldState ws)
		{
			throw new NotImplementedException();
		}
	}
}
