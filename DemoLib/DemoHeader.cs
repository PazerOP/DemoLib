using System;
using System.IO;
using System.Text;
using TF2Net.Data;

namespace DemoLib
{
	public class DemoHeader
	{
		public readonly string m_MagicToken;
		const string EXPECTED_MAGIC_TOKEN = "HL2DEMO";

		public readonly int? m_DemoProtocol;
		public readonly int? m_NetworkProtocol;
		public readonly string m_ServerName;
		public readonly string m_ClientName;
		public readonly string m_MapName;
		public readonly string m_GameDirectory;

		public readonly float? m_PlaybackTime;
		public readonly int? m_PlaybackTicks;
		public readonly int? m_PlaybackFrames;
		public readonly int? m_SignonLength;

		public DemoHeader(Stream inputStream)
		{
			using (BinaryReader reader = new BinaryReader(inputStream, Encoding.ASCII, true))
			{
				m_MagicToken = Encoding.ASCII.GetString(reader.ReadBytes(8)).TrimEnd('\0');
				if (m_MagicToken != EXPECTED_MAGIC_TOKEN)
					throw new FormatException(string.Format("Expected magic token: \"{0}\" Actual magic token: \"{1}\"", EXPECTED_MAGIC_TOKEN, m_MagicToken));

				m_DemoProtocol = reader.ReadInt32();
				m_NetworkProtocol = reader.ReadInt32();

				m_ServerName = Encoding.ASCII.GetString(reader.ReadBytes(SourceConstants.MAX_OSPATH)).TrimEnd('\0');
				m_ClientName = Encoding.ASCII.GetString(reader.ReadBytes(SourceConstants.MAX_OSPATH)).TrimEnd('\0');
				m_MapName = Encoding.ASCII.GetString(reader.ReadBytes(SourceConstants.MAX_OSPATH)).TrimEnd('\0');
				m_GameDirectory = Encoding.ASCII.GetString(reader.ReadBytes(SourceConstants.MAX_OSPATH)).TrimEnd('\0');

				m_PlaybackTime = reader.ReadSingle();
				m_PlaybackTicks = reader.ReadInt32();
				m_PlaybackFrames = reader.ReadInt32();
				m_SignonLength = reader.ReadInt32();
			}
		}

		public DemoHeader(string magicToken = null, int? demoProtocol = null, int? networkProtocol = null,
			string serverName = null, string clientName = null, string mapName = null, string gameDirectory = null,
			float? playbackTime = null, int? playbackTicks = null, int? playbackFrames = null, int? signonLength = null)
		{
			m_MagicToken = magicToken;
			m_DemoProtocol = demoProtocol;
			m_NetworkProtocol = networkProtocol;
			m_ServerName = serverName;
			m_ClientName = clientName;
			m_MapName = mapName;
			m_GameDirectory = gameDirectory;
			m_PlaybackTime = playbackTime;
			m_PlaybackTicks = playbackTicks;
			m_PlaybackFrames = playbackFrames;
			m_SignonLength = signonLength;
		}
	}
}
