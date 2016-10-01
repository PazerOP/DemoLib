using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitSet;

namespace TF2Net.Data
{
	public class UserInfo
	{
		public string Name { get; set; }
		public int? UserID { get; set; }

		public string GUID { get; set; }

		public uint? FriendsID { get; set; }
		public string FriendsName { get; set; }

		public bool? IsFakePlayer { get; set; }
		public bool? IsHLTV { get; set; }

		public uint?[] CustomFiles { get; } = new uint?[4];

		public uint? FilesDownloaded { get; set; }

		public UserInfo(BitStream stream)
		{
			Name = Encoding.ASCII.GetString(stream.ReadBytes(32)).TrimEnd('\0');

			UserID = stream.ReadInt();

			GUID = Encoding.ASCII.GetString(stream.ReadBytes(33)).TrimEnd('\0');

			FriendsID = stream.ReadUInt();

			FriendsName = Encoding.ASCII.GetString(stream.ReadBytes(32)).TrimEnd('\0');

			IsFakePlayer = stream.ReadByte() > 0 ? true : false;
			IsHLTV = stream.ReadByte() > 0 ? true : false;

			for (byte i = 0; i < 4; i++)
				CustomFiles[i] = stream.ReadUInt();

			FilesDownloaded = stream.ReadByte();
		}
	}
}
