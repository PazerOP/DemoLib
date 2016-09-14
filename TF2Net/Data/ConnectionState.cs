using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TF2Net.Data
{
	public enum ConnectionState : byte
	{
		/// <summary>
		/// no state yet, about to connect
		/// </summary>
		None = 0,

		/// <summary>
		/// client challenging server, all OOB packets
		/// </summary>
		Challenge = 1,

		/// <summary>
		/// client is connected to server, netchans ready
		/// </summary>
		Connected = 2,

		/// <summary>
		/// just got serverinfo and string tables
		/// </summary>
		New = 3,

		/// <summary>
		/// received signon buffers
		/// </summary>
		Prespawn = 4,

		/// <summary>
		/// ready to receive entity packets
		/// </summary>
		Spawn = 5,

		/// <summary>
		/// we are fully connected, first non-delta packet received
		/// </summary>
		Full = 6,

		/// <summary>
		/// server is changing level, please wait
		/// </summary>
		Changelevel = 7,
	}
}
