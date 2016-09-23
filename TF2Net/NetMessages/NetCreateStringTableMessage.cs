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
		//public BitStream Data { get; set; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		ulong BitCount { get; set; }

		public StringTable Table { get; set; }

		public string Description
		{
			get
			{
				return string.Format("svc_CreateStringTable: table {0}, entries {1}, bytes {2} userdatasize {3} userdatabits {4}",
					Table.TableName, Table.Entries.Count, BitInfo.BitsToBytes(BitCount), Table.UserDataSize, Table.UserDataSizeBits);
			}
		}

		public void ReadMsg(BitStream stream)
		{
			bool isFilenames;
			if (stream.ReadChar() == ':')
			{
				isFilenames = true;
			}
			else
			{
				stream.Seek(-8, System.IO.SeekOrigin.Current);
				isFilenames = false;
			}

			Table = new StringTable();

			Table.TableName = stream.ReadCString();

			Table.MaxEntries = stream.ReadUShort();
			int encodeBits = ExtMath.Log2(Table.MaxEntries);
			ushort entries = stream.ReadUShort((byte)(encodeBits + 1));

			BitCount = stream.ReadVarInt();
			
			// userdatafixedsize
			if (stream.ReadBool())
			{
				Table.UserDataSize = stream.ReadUShort(12);
				Table.UserDataSizeBits = stream.ReadByte(4);
			}

			bool isCompressedData = stream.ReadBool();

			BitStream data = stream.Subsection(stream.Cursor, stream.Cursor + BitCount);
			stream.Seek(BitCount, System.IO.SeekOrigin.Current);

			if (isCompressedData)
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

			StringTableParser.ParseUpdate(data, Table, entries);
		}

		public void ApplyWorldState(WorldState ws)
		{
			StringTable foundTable = ws.StringTables.SingleOrDefault(t => t.TableName == Table.TableName);

			if (foundTable != null)
				throw new InvalidOperationException("Attempted to create a stringtable that already exists!");
			
			ws.StringTables.Add(Table);
		}
	}
}
