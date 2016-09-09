using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitSet;

namespace DemoLib.NetMessages.Shared
{
	static class StringTableParser
	{
		const byte SUBSTRING_BITS = 5;
		const byte MAX_USERDATA_BITS = 14;

		[DebuggerDisplay("{ID,nq}: {Value}")]
		public class StringEntry
		{
			public ushort ID { get; set; }
			public string Value { get; set; }
			public byte[] UserData { get; set; }
		}

		public static void ParseUpdate(byte[] buffer, ref ulong bitOffset,
			IList<StringEntry> stringEntries, ushort entries, ushort maxEntries,
			ushort? userDataSize, byte? userDataSizeBits)
		{
			byte entryBits = (byte)ExtMath.Log2(maxEntries);
			int lastEntry = -1;
			for (int i = 0; i < entries; i++)
			{
				int entryIndex = lastEntry + 1;

				if (!BitReader.ReadBool(buffer, ref bitOffset))
					entryIndex = (int)BitReader.ReadUIntBits(buffer, ref bitOffset, entryBits);

				lastEntry = entryIndex;

				if (entryIndex < 0 || entryIndex > maxEntries)
					throw new DemoParseException("Server sent bogus string index for stringtable");

				string value = null;
				if (BitReader.ReadBool(buffer, ref bitOffset))
				{
					bool substringcheck = BitReader.ReadBool(buffer, ref bitOffset);

					if (substringcheck)
					{
						int index = (int)BitReader.ReadUIntBits(buffer, ref bitOffset, 5);
						int bytesToCopy = (int)BitReader.ReadUIntBits(buffer, ref bitOffset, SUBSTRING_BITS);
						value = stringEntries.Single(s => s.ID == index).Value.Substring(0, bytesToCopy) + BitReader.ReadCString(buffer, ref bitOffset);
					}
					else
					{
						value = BitReader.ReadCString(buffer, ref bitOffset);
					}
				}

				int nBytes = 0;
				byte[] userData = null;
				if (BitReader.ReadBool(buffer, ref bitOffset))
				{
					if (userDataSize.HasValue)
					{
						nBytes = userDataSize.Value;
						Debug.Assert(nBytes > 0);
						userData = new byte[nBytes];
						BitReader.CopyBits(buffer, userDataSizeBits.Value, ref bitOffset, userData);
					}
					else
					{
						nBytes = (int)BitReader.ReadUIntBits(buffer, ref bitOffset, MAX_USERDATA_BITS);
						userData = new byte[nBytes];
						BitReader.CopyBits(buffer, (ulong)(nBytes * 3), ref bitOffset, userData);
						throw new NotImplementedException();
					}
				}

				if (entryIndex < stringEntries.Count)
				{
					throw new NotImplementedException();
				}
				else
				{
					Debug.Assert(entryIndex == stringEntries.Count);
					Debug.Assert(value != null);

					StringEntry newEntry = new StringEntry();
					newEntry.ID = (ushort)entryIndex;
					newEntry.UserData = userData;
					newEntry.Value = value;
					stringEntries.Add(newEntry);
				}
			}
		}
	}
}
