using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitSet;
using TF2Net.Data;

namespace TF2Net.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	public class NetServerInfoMessage : INetMessage
	{
		public ServerInfo Info { get; set; }

		public string Description
		{
			get
			{
				return string.Format("svc_ServerInfo: game \"{0}\", map \"{1}\", max {2}",
					Info.GameDirectory, Info.MapName, Info.MaxClients);
			}
		}

		public void ReadMsg(BitStream stream)
		{
			Info = new ServerInfo();

			Info.Protocol = stream.ReadShort();
			Info.ServerCount = stream.ReadInt();
			Info.IsHLTV = stream.ReadBool();
			Info.IsDedicated = stream.ReadBool();
			Info.ClientCRC = stream.ReadUInt();
			Info.MaxClasses = stream.ReadUShort();

			// Unknown
			stream.Seek(16 * 8, System.IO.SeekOrigin.Current);

			Info.PlayerSlot = stream.ReadByte();
			Info.MaxClients = stream.ReadByte();
			Info.TickInterval = stream.ReadSingle();

			switch (stream.ReadChar())
			{
				case 'l':
				case 'L':
				Info.OS = ServerInfo.OperatingSystem.Linux;
				break;

				case 'w':
				case 'W':
				Info.OS = ServerInfo.OperatingSystem.Windows;
				break;

				default:
				Info.OS = ServerInfo.OperatingSystem.Unknown;
				break;
			}

			Info.GameDirectory = stream.ReadCString();
			Info.MapName = stream.ReadCString();
			Info.SkyName = stream.ReadCString();
			Info.Hostname = stream.ReadCString();

			// Unknown
			stream.Seek(1, System.IO.SeekOrigin.Current);
		}

		public void ApplyWorldState(WorldState ws)
		{
			ws.ServerInfo = Info;
			ws.Listeners.ServerInfoLoaded.Invoke(ws);
		}
	}
}
