using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TF2Net.Data
{
	[DebuggerDisplay("{Hostname}: {MapName}")]
	public class ServerInfo
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
	}
}
