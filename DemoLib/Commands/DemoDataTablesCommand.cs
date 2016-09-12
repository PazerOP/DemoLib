using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using BitSet;
using DemoLib.DataExtraction;
using TF2Net.Data;

namespace DemoLib.Commands
{
	class DemoDataTablesCommand : TimestampedDemoCommand
	{
		const int PROPINFOBITS_NUMPROPS = 10;
		const int PROPINFOBITS_TYPE = 5;
		const int PROPINFOBITS_FLAGS = SPROP_NUMFLAGBITS_NETWORKED;
		const int PROPINFOBITS_NUMELEMENTS = 10;
		const int PROPINFOBITS_NUMBITS = 7;

		const int SPROP_NUMFLAGBITS_NETWORKED = 16;

		public IList<SendTable> SendTables { get; set; } = new List<SendTable>();
		public IList<ServerClass> ServerClasses { get; set; } = new List<ServerClass>();

		public DemoDataTablesCommand(Stream input) : base(input)
		{
			Type = DemoCommandType.dem_datatables;

			byte[] data;
			using (BinaryReader reader = new BinaryReader(input, Encoding.ASCII, true))
			{
				int length = reader.ReadInt32();
				data = reader.ReadBytes(length);
			}

			ulong cursor = 0;

			while (BitReader.ReadBool(data, ref cursor))
			{
				SendTables.Add(ParseSendTable(data, ref cursor));
			}

			short serverClasses = BitReader.ReadShort(data, ref cursor);
			Debug.Assert(serverClasses > 0);

			ServerClasses = new List<ServerClass>(serverClasses);

			for (int i = 0; i < serverClasses; i++)
			{
				short classID = BitReader.ReadShort(data, ref cursor);
				if (classID >= serverClasses)
					throw new DemoParseException("Invalid server class ID");

				ServerClass sc = new ServerClass();
				sc.Classname = BitReader.ReadCString(data, ref cursor);
				sc.DatatableName = BitReader.ReadCString(data, ref cursor);
				ServerClasses.Add(sc);
			}
		}
				
		static SendTable ParseSendTable(byte[] buffer, ref ulong bitOffset)
		{
			bitOffset++; //bool needsDecoder = BitReader.ReadBool(Data, ref cursor);

			SendTable table = new SendTable();

			table.NetTableName = BitReader.ReadCString(buffer, ref bitOffset);
			
			int propertyCount = (int)BitReader.ReadUIntBits(buffer, ref bitOffset, PROPINFOBITS_NUMPROPS);

			for (int i = 0; i < propertyCount; i++)
			{
				SendProp prop = new SendProp(table);
				
				prop.Type = (SendPropType)BitReader.ReadUIntBits(buffer, ref bitOffset, PROPINFOBITS_TYPE);
				Debug.Assert(Enum.GetValues(typeof(SendPropType)).Cast<SendPropType>().Contains(prop.Type));

				prop.Name = BitReader.ReadCString(buffer, ref bitOffset);

				prop.Flags = (SendPropFlags)BitReader.ReadUIntBits(buffer, ref bitOffset, PROPINFOBITS_FLAGS);

				if (prop.Type == SendPropType.Datatable)
				{
					prop.ExcludeName = BitReader.ReadCString(buffer, ref bitOffset);
				}
				else
				{
					if ((prop.Flags & SendPropFlags.Exclude) != 0)
						prop.ExcludeName = BitReader.ReadCString(buffer, ref bitOffset);
					else if (prop.Type == SendPropType.Array)
						prop.ArrayElements = (int)BitReader.ReadUIntBits(buffer, ref bitOffset, PROPINFOBITS_NUMELEMENTS);
					else
					{
						prop.LowValue = BitReader.ReadSingle(buffer, ref bitOffset);
						prop.HighValue = BitReader.ReadSingle(buffer, ref bitOffset);

						prop.BitCount = (int)BitReader.ReadUIntBits(buffer, ref bitOffset, PROPINFOBITS_NUMBITS);
					}
				}

				table.Properties.Add(prop);
			}

			return table;
		}
	}
}
