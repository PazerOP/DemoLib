using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitSet;
using TF2Net.Data;

namespace TF2Net.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	public class NetTempEntityMessage : INetMessage
	{
		public int EntryCount { get; set; }

		public BitStream Data { get; set; }

		public string Description
		{
			get
			{
				return string.Format("svc_TempEntities: number {0}, bytes {1}", EntryCount, BitInfo.BitsToBytes(Data.Length));
			}
		}

		public void ReadMsg(BitStream stream)
		{
			EntryCount = stream.ReadInt(SourceConstants.EVENT_INDEX_BITS);

			ulong bitCount = stream.ReadVarUInt();

			Data = stream.Subsection(stream.Cursor, stream.Cursor + bitCount);
			stream.Seek(bitCount, System.IO.SeekOrigin.Current);
		}

		public void ApplyWorldState(WorldState ws)
		{
			return;
			BitStream local = Data.Clone();
			local.Cursor = 0;

			for (int i = 0; i < EntryCount; i++)
			{
				double delay = 0;
				if (local.ReadBool())
					delay = local.ReadInt(8) / 100.0;

				if (local.ReadBool())
				{
					uint classID = local.ReadUInt(ws.ClassBits);

					ServerClass serverClass = ws.ServerClasses[(int)classID];
					SendTable sendTable = ws.SendTables.Single(st => st.NetTableName == serverClass.DatatableName);
					var flattened = sendTable.FlattenedProps;

					var tempents = ws.SendTables.Where(st => st.NetTableName.StartsWith("DT_TE"));

					Console.WriteLine("help mom");
				}
			}
		}
	}
}
