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

		public static void ParseUpdate(BitStream stream, StringTable table, ushort entries)
		{
			Debug.Assert(stream.Cursor == 0);

			IList<StringTableEntry> history = new List<StringTableEntry>();

			byte entryBits = (byte)ExtMath.Log2(table.MaxEntries);
			int lastEntry = -1;
			for (int i = 0; i < entries; i++)
			{
				// Did we read the entry from the BitStream or just assume it was lastEntry + 1?
				//bool readEntry = false;

				int entryIndex = lastEntry + 1;

				if (!stream.ReadBool())
				{
					entryIndex = (int)stream.ReadUInt(entryBits);
					//readEntry = true;
				}

				lastEntry = entryIndex;

				if (entryIndex < 0 || entryIndex > table.MaxEntries)
					throw new FormatException("Server sent bogus string index for stringtable");

				string value = null;
				if (stream.ReadBool())
				{
					bool substringcheck = stream.ReadBool();

					if (substringcheck)
					{
						int index = (int)stream.ReadUInt(5);
						int bytesToCopy = (int)stream.ReadUInt(SUBSTRING_BITS);

						string restOfString = stream.ReadCString();

						var testLength = history[index].Value?.Length;

						value = history[index].Value?.Substring(0, bytesToCopy) + restOfString;
					}
					else
					{
						value = stream.ReadCString();
					}
				}
				
				BitStream userData = null;
				if (stream.ReadBool())
				{
					if (table.UserDataSize.HasValue)
					{
						userData = stream.Subsection(stream.Cursor, stream.Cursor + table.UserDataSizeBits.Value);
						stream.Seek(table.UserDataSizeBits.Value, System.IO.SeekOrigin.Current);
					}
					else
					{
						ulong nBytes = stream.ReadUInt(MAX_USERDATA_BITS);
						userData = stream.Subsection(stream.Cursor, stream.Cursor + (nBytes * 8));
						stream.Seek(nBytes * 8, System.IO.SeekOrigin.Current);
					}
				}
				
				StringTableEntry existingEntry = table.Entries.SingleOrDefault(s => s.ID == entryIndex);
				if (existingEntry != null)
				{
					existingEntry.UserData = userData;

					if (value != null && value != existingEntry.Value)
					{
						//throw new FormatException("Corrupted demo?");
						existingEntry.Value = value;
					}
					else
						value = existingEntry.Value;

					existingEntry.Value = value;
				}
				else
				{
					//Debug.Assert(entryIndex == table.Entries.Count);
					if (value == null)
						Debug.Assert(true);
					//Debug.Assert(value != null);

					//if (value == null)
					//	value = string.Empty;

					StringTableEntry newEntry = new StringTableEntry(table);
					newEntry.ID = (ushort)entryIndex;
					newEntry.UserData = userData;
					newEntry.Value = value;
					table.Add(newEntry);

					existingEntry = newEntry;
				}

				if (history.Count > 31)
					history.RemoveAt(0);

				history.Add(existingEntry);
			}
			
			Debug.Assert((stream.Length - stream.Cursor) < 8);
		}
	}
}
