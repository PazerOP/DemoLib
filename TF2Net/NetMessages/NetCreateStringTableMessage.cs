using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using BitSet;
using Snappy;
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
		
		//public BitStream Data { get; set; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		ulong BitCount { get; set; }

		public IList<StringTableEntry> StringEntries { get; set; } = new List<StringTableEntry>();

		public string Description
		{
			get
			{
				return string.Format("svc_CreateStringTable: table {0}, entries {1}, bytes {2} userdatasize {3} userdatabits {4}",
					TableName, Entries, BitInfo.BitsToBytes(BitCount), UserDataSize, UserDataSizeBits);
			}
		}

		public void ReadMsg(BitStream stream)
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

			BitCount = stream.ReadVarInt();

			UserDataFixedSize = stream.ReadBool();
			if (UserDataFixedSize)
			{
				UserDataSize = stream.ReadUShort(12);
				UserDataSizeBits = stream.ReadByte(4);
			}

			CompressedData = stream.ReadBool();

			BitStream data = stream.Subsection(stream.Cursor, stream.Cursor + BitCount);
			stream.Seek(BitCount, System.IO.SeekOrigin.Current);

			if (CompressedData)
			{
				uint decompressedNumBytes = data.ReadUInt();
				uint compressedNumBytes = data.ReadUInt();

				byte[] compressedData = data.ReadBytes(compressedNumBytes);

				char[] magic = Encoding.ASCII.GetChars(compressedData, 0, 4);
				if (
					magic[0] != 'S' ||
					magic[1] != 'N' ||
					magic[2] != 'A' ||
					magic[3] != 'P')
				{
					throw new FormatException("Unknown format for compressed stringtable");
				}

				int snappyDecompressedNumBytes = SnappyCodec.GetUncompressedLength(compressedData, 4, compressedData.Length - 4);
				if (snappyDecompressedNumBytes != decompressedNumBytes)
					throw new FormatException("Mismatching decompressed data lengths");

				byte[] decompressedData = new byte[snappyDecompressedNumBytes];
				if (SnappyCodec.Uncompress(compressedData, 4, compressedData.Length - 4, decompressedData, 0) != decompressedNumBytes)
					throw new FormatException("Snappy didn't decode all the bytes");

				data = new BitStream(decompressedData);
			}

			StringTableParser.ParseUpdate(data, StringEntries, Entries, MaxEntries, UserDataSize, UserDataSizeBits);
		}

		public void ApplyWorldState(WorldState ws)
		{
			StringTable foundTable = ws.StringTables.SingleOrDefault(t => t.TableName == TableName);

			if (foundTable != null)
				throw new InvalidOperationException("Attempted to create a stringtable that already exists!");
			
			foundTable = new StringTable();
			foundTable.MaxEntries = MaxEntries;
			foundTable.Entries = new List<StringTableEntry>(StringEntries);
			foundTable.TableName = TableName;
			foundTable.UserDataSize = UserDataSize;
			foundTable.UserDataSizeBits = UserDataSizeBits;

			ws.StringTables.Add(foundTable);
		}
	}
}
