using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitSet;
using TF2Net.Data;
using TF2Net.Entities;

namespace TF2Net.NetMessages
{
	internal static class EntityCoder
	{
		public static void ApplyEntityUpdate(IEntity e, BitStream stream)
		{
			var testGuessProps = e.NetworkTable.FlattenedProps;

			bool atLeastOne = false;
			int index = -1;
			while ((index = ReadFieldIndex(stream, index)) != -1)
			{
				Debug.Assert(index < testGuessProps.Length);
				Debug.Assert(index < SourceConstants.MAX_DATATABLE_PROPS);

				var prop = testGuessProps[index];

				SendProp s = e.GetProperty(prop);

				bool wasNull = false;
				if (s == null)
				{
					s = new SendProp(e, prop);
					wasNull = true;
				}

				object newValue = prop.Decode(stream);
				s.Value = newValue;
				atLeastOne = true;

				if (wasNull)
					e.AddProperty(s);
			}

			if (atLeastOne)
				e.PropertiesUpdated.Invoke(e);
		}

		public static int ReadFieldIndex(BitStream stream, int lastIndex)
		{
			if (!stream.ReadBool())
				return -1;

			var diff = ReadUBitVar(stream);
			return (int)(lastIndex + diff + 1);
		}

		public static uint ReadUBitVar(BitStream stream)
		{
			switch (stream.ReadByte(2))
			{
				case 0: return stream.ReadUInt(4);
				case 1: return stream.ReadUInt(8);
				case 2: return stream.ReadUInt(12);
				case 3: return stream.ReadUInt(32);
			}

			throw new Exception("Should never get here...");
		}
	}
}
