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

			BitStream stream;
			using (BinaryReader reader = new BinaryReader(input, Encoding.ASCII, true))
			{
				int length = reader.ReadInt32();
				stream = new BitStream(reader.ReadBytes(length));
			}

			while (stream.ReadBool())
				SendTables.Add(ParseSendTable(stream));

			// Link referenced datatables
			foreach (SendTable table in SendTables)
			{
				foreach (SendPropDefinition dtProp in table.Properties)
				{
					if (dtProp.Type == SendPropType.Datatable)
					{
						dtProp.Table = SendTables.Single(t => t.NetTableName == dtProp.ExcludeName);
						dtProp.ExcludeName = null;
					}
				}
			}

			short serverClasses = stream.ReadShort();
			Debug.Assert(serverClasses > 0);

			ServerClasses = new List<ServerClass>(serverClasses);

			for (int i = 0; i < serverClasses; i++)
			{
				short classID = stream.ReadShort();
				if (classID >= serverClasses)
					throw new DemoParseException("Invalid server class ID");

				ServerClass sc = new ServerClass();
				sc.Classname = stream.ReadCString();
				sc.DatatableName = stream.ReadCString();
				ServerClasses.Add(sc);
			}

			Debug.Assert((stream.Length - stream.Cursor) < 8);
		}
				
		static SendTable ParseSendTable(BitStream stream)
		{
			SendTable table = new SendTable();

			table.Unknown1 = stream.ReadBool();

			table.NetTableName = stream.ReadCString();
			
			int propertyCount = (int)stream.ReadULong(PROPINFOBITS_NUMPROPS);

			SendPropDefinition arrayElementProp = null;

			for (int i = 0; i < propertyCount; i++)
			{
				SendPropDefinition prop = new SendPropDefinition(table);
				
				prop.Type = (SendPropType)stream.ReadULong(PROPINFOBITS_TYPE);
				Debug.Assert(Enum.GetValues(typeof(SendPropType)).Cast<SendPropType>().Contains(prop.Type));

				Debug.Assert(prop.Type == SendPropType.Datatable ? prop.Flags == 0 : true);

				prop.Name = stream.ReadCString();

				prop.Flags = (SendPropFlags)stream.ReadULong(PROPINFOBITS_FLAGS);

				if (prop.Type == SendPropType.Datatable)
				{
					prop.ExcludeName = stream.ReadCString();
				}
				else
				{
					if ((prop.Flags & SendPropFlags.Exclude) != 0)
						prop.ExcludeName = stream.ReadCString();
					else if (prop.Type == SendPropType.Array)
						prop.ArrayElements = (int)stream.ReadULong(PROPINFOBITS_NUMELEMENTS);
					else
					{
						prop.LowValue = stream.ReadSingle();
						prop.HighValue = stream.ReadSingle();

						prop.BitCount = stream.ReadULong(PROPINFOBITS_NUMBITS);
					}
				}

				if (prop.Flags.HasFlag(SendPropFlags.NoScale))
				{
					if (prop.Type == SendPropType.Float)
						prop.BitCount = 32;
					else if (prop.Type == SendPropType.Vector)
					{
						if (!prop.Flags.HasFlag(SendPropFlags.Normal))
							prop.BitCount = 32 * 3;
					}
				}

				if (arrayElementProp != null)
				{
					Debug.Assert(prop.Type == SendPropType.Array);
					prop.ArrayProperty = arrayElementProp;
					arrayElementProp = null;
				}

				if (prop.Flags.HasFlag(SendPropFlags.InsideArray))
				{
					Debug.Assert(arrayElementProp == null);
					Debug.Assert(!prop.Flags.HasFlag(SendPropFlags.ChangesOften));
					arrayElementProp = prop;
				}
				else
				{
					table.Properties.Add(prop);
				}
			}

			return table;
		}
	}
}
