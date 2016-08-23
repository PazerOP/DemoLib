using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DemoLib
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	struct demoheader_t
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		char[] demofilestamp;                      // Should be HL2DEMO

		int demoprotocol;                           // Should be DEMO_PROTOCOL
		int networkprotocol;                        // Should be PROTOCOL_VERSION
		char servername[SourceConstants.MAX_OSPATH];				// Name of server
		char clientname[MAX_OSPATH];				// Name of client who recorded the game
		char mapname[MAX_OSPATH];                   // Name of map
		
		char[] gamedirectory;             // Name of game directory (com_gamedir)

		float playback_time;						// Time of track
		int playback_ticks;                         // # of ticks in track
		int playback_frames;                        // # of frames in track
		int signonlength;                           // length of sigondata in bytes
	};

	class DemoHeader
	{
		string m_MagicToken;
		int m_DemoProtocol;
		int m_NetworkProtocol;
		string m_ServerName;
		string m_ClientName;
		string m_MapName;
		string m_GameDirectory;

		float m_PlaybackTime;
		int m_PlaybackTicks;
		int m_PlaybackFrames;
		int m_SignonLength;

		public DemoHeader(Stream inputStream)
		{
			using (BinaryReader reader = new BinaryReader(inputStream, Encoding.ASCII, true))
			{
				m_MagicToken = Encoding.ASCII.GetString(reader.ReadBytes(8));

				m_DemoProtocol = reader.ReadInt32();
				m_NetworkProtocol = reader.ReadInt32();
				m_ServerName = Encoding.ASCII.GetString(reader.ReadBytes(SourceConstants.MAX_OSPATH));
				m_ClientName = Encoding.ASCII.GetString(reader.ReadBytes(SourceConstants.MAX_OSPATH));
				m_MapName = Encoding.ASCII.GetString(reader.ReadBytes(SourceConstants.MAX_OSPATH));
			}
		}
	}
}
