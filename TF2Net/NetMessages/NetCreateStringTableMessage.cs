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
		public BitStream Data { get; set; }

		public string TableName { get; set; }

		public ushort Entries { get; set; }
		public ushort MaxEntries { get; set; }

		public ushort? UserDataSize { get; set; }
		public byte? UserDataSizeBits { get; set; }

		public string Description
		{
			get
			{
				return string.Format("svc_CreateStringTable: table {0}, entries {1}, bytes {2} userdatasize {3} userdatabits {4}",
					TableName, MaxEntries, BitInfo.BitsToBytes(Data.Length), UserDataSize, UserDataSizeBits);
			}
		}

		public void ReadMsg(BitStream stream)
		{
			//bool isFilenames;
			if (stream.ReadChar() == ':')
			{
				//isFilenames = true;
			}
			else
			{
				stream.Seek(-8, System.IO.SeekOrigin.Current);
				//isFilenames = false;
			}

			TableName = stream.ReadCString();

			MaxEntries = stream.ReadUShort();
			int encodeBits = ExtMath.Log2(MaxEntries);
			Entries = stream.ReadUShort((byte)(encodeBits + 1));

			ulong bitCount = stream.ReadVarUInt();
			
			// userdatafixedsize
			if (stream.ReadBool())
			{
				UserDataSize = stream.ReadUShort(12);
				UserDataSizeBits = stream.ReadByte(4);
			}

			bool isCompressedData = stream.ReadBool();

			Data = stream.Subsection(stream.Cursor, stream.Cursor + bitCount);
			stream.Seek(bitCount, System.IO.SeekOrigin.Current);

			if (isCompressedData)
			{
				uint decompressedNumBytes = Data.ReadUInt();
				uint compressedNumBytes = Data.ReadUInt();

				byte[] compressedData = Data.ReadBytes(compressedNumBytes);

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

				Data = new BitStream(decompressedData);
			}
		}

		public void ApplyWorldState(WorldState ws)
		{
			Data.Cursor = 0;
			StringTable table = new StringTable(ws, TableName, MaxEntries, UserDataSize, UserDataSizeBits);
			StringTableParser.ParseUpdate(Data, table, Entries);

			StringTable foundTable = ws.StringTables.SingleOrDefault(t => t.TableName == table.TableName);

			if (foundTable != null)
				throw new InvalidOperationException("Attempted to create a stringtable that already exists!");
			
			ws.StringTables.Add(table);
			ws.Listeners.OnStringTableCreated(table);
		}
	}
}
