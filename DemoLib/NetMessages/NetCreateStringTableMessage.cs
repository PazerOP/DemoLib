using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using BitSet;
using DemoLib.NetMessages.Shared;

namespace DemoLib.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	class NetCreateStringTableMessage : INetMessage
	{
		public string TableName { get; set; }
		public ushort MaxEntries { get; set; }
		public ushort Entries { get; set; }
		public bool UserDataFixedSize { get; set; }
		public ushort UserDataSize { get; set; }
		public byte UserDataSizeBits { get; set; }
		public bool IsFilenames { get; set; }
		public bool CompressedData { get; set; }
		
		public ulong BitCount { get; set; }
		public byte[] Data { get; set; }

		public IList<StringTableParser.StringEntry> StringEntries { get; set; } = new List<StringTableParser.StringEntry>();

		public string Description
		{
			get
			{
				return string.Format("svc_CreateStringTable: table {0}, entries {1}, bytes {2} userdatasize {3} userdatabits {4}",
					TableName, Entries, BitInfo.BitsToBytes(BitCount), UserDataSize, UserDataSizeBits);
			}
		}

		public ulong Size
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public void ReadMsg(DemoReader reader, uint? serverTick, byte[] buffer, ref ulong bitOffset)
		{
			if (BitReader.ReadChar(buffer, ref bitOffset) == ':')
			{
				IsFilenames = true;
			}
			else
			{
				bitOffset -= (sizeof(byte) << 3);
				IsFilenames = false;
			}

			TableName = BitReader.ReadCString(buffer, ref bitOffset);

			MaxEntries = BitReader.ReadUShort(buffer, ref bitOffset);
			int encodeBits = ExtMath.Log2(MaxEntries);
			Entries = (ushort)BitReader.ReadUIntBits(buffer, ref bitOffset, (byte)(encodeBits + 1));

			BitCount = BitReader.ReadVarInt(buffer, ref bitOffset);

			UserDataFixedSize = BitReader.ReadBool(buffer, ref bitOffset);
			if (UserDataFixedSize)
			{
				UserDataSize = (ushort)BitReader.ReadUIntBits(buffer, ref bitOffset, 12);
				UserDataSizeBits = (byte)BitReader.ReadUIntBits(buffer, ref bitOffset, 4);
			}
			else
			{
				UserDataSize = 0;
				UserDataSizeBits = 0;
			}

			CompressedData = BitReader.ReadBool(buffer, ref bitOffset);

			ulong startBit = bitOffset;

			//StringTableParser.ParseUpdate(buffer, ref bitOffset, StringEntries, Entries, MaxEntries);

			Data = new byte[BitInfo.BitsToBytes(BitCount)];
			BitReader.CopyBits(buffer, BitCount, ref bitOffset, Data);

			Debug.Assert((startBit + BitCount) == bitOffset);
		}

		public void WriteMsg(byte[] buffer, ref ulong bitOffset)
		{
			throw new NotImplementedException();
		}
	}
}
