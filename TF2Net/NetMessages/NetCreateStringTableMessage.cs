using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using BitSet;
using TF2Net.Data;
using TF2Net.NetMessages.Shared;

namespace TF2Net.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	public class NetCreateStringTableMessage : INetMessage
	{
		public string TableName { get; set; }
		public ushort MaxEntries { get; set; }
		public ushort Entries { get; set; }
		public bool UserDataFixedSize { get; set; }
		public ushort? UserDataSize { get; set; }
		public byte? UserDataSizeBits { get; set; }
		public bool IsFilenames { get; set; }
		public bool CompressedData { get; set; }
		
		public BitStream Data { get; set; }

		public IList<StringTableEntry> StringEntries { get; set; } = new List<StringTableEntry>();

		public string Description
		{
			get
			{
				return string.Format("svc_CreateStringTable: table {0}, entries {1}, bytes {2} userdatasize {3} userdatabits {4}",
					TableName, Entries, BitInfo.BitsToBytes(Data.Length), UserDataSize, UserDataSizeBits);
			}
		}

		public void ReadMsg(BitStream stream, IReadOnlyWorldState ws)
		{
			if (stream.ReadChar() == ':')
			{
				IsFilenames = true;
			}
			else
			{
				stream.Seek(-8, System.IO.SeekOrigin.Current);
				IsFilenames = false;
			}

			TableName = stream.ReadCString();

			MaxEntries = stream.ReadUShort();
			int encodeBits = ExtMath.Log2(MaxEntries);
			Entries = stream.ReadUShort((byte)(encodeBits + 1));

			ulong bitCount = stream.ReadVarInt();

			UserDataFixedSize = stream.ReadBool();
			if (UserDataFixedSize)
			{
				UserDataSize = stream.ReadUShort(12);
				UserDataSizeBits = stream.ReadByte(4);
			}

			CompressedData = stream.ReadBool();

			//StringTableParser.ParseUpdate(buffer, ref bitOffset, StringEntries, Entries, MaxEntries);

			Data = stream.Subsection(stream.Cursor, stream.Cursor + bitCount);
			stream.Seek(bitCount, System.IO.SeekOrigin.Current);
		}

		public void ApplyWorldState(WorldState ws)
		{
			throw new NotImplementedException();
		}
	}
}
