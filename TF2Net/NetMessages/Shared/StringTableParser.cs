using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitSet;
using TF2Net.Data;

namespace TF2Net.NetMessages.Shared
{
	static class StringTableParser
	{
		const byte SUBSTRING_BITS = 5;
		const byte MAX_USERDATA_BITS = 14;

		public static void ParseUpdate(BitStream stream,
			IList<StringTableEntry> stringEntries, ushort entries, ushort maxEntries,
			ushort? userDataSize, byte? userDataSizeBits)
		{
			List<string> history = new List<string>();

			byte entryBits = (byte)ExtMath.Log2(maxEntries);
			int lastEntry = -1;
			for (int i = 0; i < entries; i++)
			{
				int entryIndex = lastEntry + 1;

				if (!stream.ReadBool())
					entryIndex = stream.ReadInt(entryBits);

				lastEntry = entryIndex;

				if (entryIndex < 0 || entryIndex > maxEntries)
					throw new FormatException("Server sent bogus string index for stringtable");

				string value = null;
				if (stream.ReadBool())
				{
					bool substringcheck = stream.ReadBool();

					if (substringcheck)
					{
						int index = stream.ReadInt(5);
						int bytesToCopy = stream.ReadInt(SUBSTRING_BITS);
						
						value = history[index].Substring(0, bytesToCopy) + stream.ReadCString();
					}
					else
					{
						value = stream.ReadCString();
					}
				}

				ulong? nBytes;
				BitStream userData = null;
				if (stream.ReadBool())
				{
					if (userDataSize.HasValue)
					{
						userData = stream.Subsection(stream.Cursor, stream.Cursor + userDataSizeBits.Value);
						stream.Seek(userDataSizeBits.Value, System.IO.SeekOrigin.Current);
					}
					else
					{
						nBytes = stream.ReadULong(MAX_USERDATA_BITS);
						userData = stream.Subsection(stream.Cursor, stream.Cursor + (nBytes.Value * 8));
						stream.Seek(nBytes.Value * 8, System.IO.SeekOrigin.Current);
					}
				}

				if (stringEntries.Any(s => s.ID == entryIndex))
				{
					throw new NotImplementedException();
				}
				else
				{
					//Debug.Assert(entryIndex == stringEntries.Count);
					Debug.Assert(value != null);

					StringTableEntry newEntry = new StringTableEntry();
					newEntry.ID = (ushort)entryIndex;
					newEntry.UserData = userData;
					newEntry.Value = value;
					stringEntries.Add(newEntry);
				}

				if (history.Count > 31)
					history.RemoveAt(0);

				history.Add(value);
			}
		}
	}
}
