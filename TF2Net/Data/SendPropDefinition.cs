using System;
using System.Diagnostics;
using System.Text;
using BitSet;

namespace TF2Net.Data
{
	[DebuggerDisplay("{ToString(),nq}")]
	public class SendPropDefinition : ICloneable
	{
		private SendPropDefinition() { }
		public SendPropDefinition(SendTable parent)
		{
			Parent = parent;
		}
		public SendTable Parent { get; }

		public SendPropType Type { get; set; }
		public string Name { get; set; }
		public SendPropFlags Flags { get; set; }

		public string ExcludeName { get; set; }

		public int? ArrayElements { get; set; }
		public SendPropDefinition ArrayProperty { get; set; }

		public double? LowValue { get; set; }
		public double? HighValue { get; set; }
		public ulong? BitCount { get; set; }

		// If we're SendPropType.Datatable
		public SendTable Table { get; set; }

		public object Decode(BitStream stream)
		{
			switch (Type)
			{
				case SendPropType.Int:		return ReadInt(stream);
				case SendPropType.Vector:	return ReadVector(stream);
				case SendPropType.Float:	return ReadFloat(stream);
				case SendPropType.String:	return ReadString(stream);
				case SendPropType.Array:	return ReadArray(stream);
				case SendPropType.VectorXY:	return ReadVectorXY(stream);

				default:
				throw new NotImplementedException();
			}
		}

		object[] ReadArray(BitStream stream)
		{
			int maxElements = ArrayElements.Value;
			byte numBits = 1;
			while ((maxElements >>= 1) != 0)
				numBits++;

			uint elementCount = stream.ReadUInt(numBits);
			object[] retVal = new object[elementCount];

			for (int i = 0; i < elementCount; i++)
				retVal[i] = ArrayProperty.Decode(stream);

			return retVal;
		}

		string ReadString(BitStream stream)
		{
			ulong chars = stream.ReadULong(9);

			byte[] raw = stream.ReadBytes(chars);
			return Encoding.ASCII.GetString(raw);
		}

		object ReadInt(BitStream stream)
		{
			if (Flags.HasFlag(SendPropFlags.VarInt))
			{
				if (Flags.HasFlag(SendPropFlags.Unsigned))
					return stream.ReadVarUInt();
				else
					return stream.ReadVarInt();
			}
			else
			{
				if (Flags.HasFlag(SendPropFlags.Unsigned))
					return stream.ReadUInt((byte)BitCount.Value);
				else
					return stream.ReadInt((byte)BitCount.Value);
			}
		}

		double ReadBitCoord(BitStream stream, bool isIntegral, bool isLowPrecision)
		{
			double value = 0;
			bool isNegative = false;
			bool inBounds = stream.ReadBool();

			if (isIntegral)
			{
				bool hasIntVal = stream.ReadBool();
				if (hasIntVal)
				{
					isNegative = stream.ReadBool();

					if (inBounds)
						value = stream.ReadULong(SourceConstants.COORD_INTEGER_BITS_MP) + 1;
					else
					{
						value = stream.ReadULong(SourceConstants.COORD_INTEGER_BITS) + 1;

						if (value < (1 << SourceConstants.COORD_INTEGER_BITS_MP))
							throw new FormatException("Something's fishy...");
					}
				}
			}
			else
			{
				bool hasIntVal = stream.ReadBool();
				isNegative = stream.ReadBool();

				if (hasIntVal)
				{
					if (inBounds)
						value = stream.ReadULong(SourceConstants.COORD_INTEGER_BITS_MP) + 1;
					else
					{
						value = stream.ReadULong(SourceConstants.COORD_INTEGER_BITS) + 1;

						if (value < (1 << SourceConstants.COORD_INTEGER_BITS_MP))
							throw new FormatException("Something's fishy...");
					}
				}

				var fractVal = stream.ReadULong(isLowPrecision ? (byte)SourceConstants.COORD_FRACTIONAL_BITS_MP_LOWPRECISION : (byte)SourceConstants.COORD_FRACTIONAL_BITS);

				value = value + fractVal * (isLowPrecision ? SourceConstants.COORD_RESOLUTION_LOWPRECISION : SourceConstants.COORD_RESOLUTION);
			}

			if (isNegative)
				value = -value;

			return value;
		}

		bool ReadSpecialFloat(BitStream stream, out double retVal)
		{
			if (Flags.HasFlag(SendPropFlags.Coord))
			{
				throw new NotImplementedException();
				return true;
			}
			else if (Flags.HasFlag(SendPropFlags.CoordMP))
			{
				retVal = ReadBitCoord(stream, false, false);
				return true;
			}
			else if (Flags.HasFlag(SendPropFlags.CoordMPLowPrecision))
			{
				retVal = ReadBitCoord(stream, false, true);
				return true;
			}
			else if (Flags.HasFlag(SendPropFlags.CoordMPIntegral))
			{
				retVal = ReadBitCoord(stream, true, false);
				return true;
			}
			else if (Flags.HasFlag(SendPropFlags.NoScale))
			{
				retVal = stream.ReadSingle();
				return true;
			}
			else if (Flags.HasFlag(SendPropFlags.Normal))
			{
				throw new NotImplementedException();
				return true;
			}

			retVal = default(double);
			return false;
		}

		double ReadFloat(BitStream stream)
		{
			double retVal;
			if (ReadSpecialFloat(stream, out retVal))
				return retVal;

			ulong raw = stream.ReadULong((byte)BitCount.Value);
			double percentage = (double)raw / ((1UL << (byte)BitCount.Value) - 1);
			retVal = LowValue.Value + (HighValue.Value - LowValue.Value) * percentage;

			return retVal;
		}

		Vector ReadVector(BitStream stream)
		{
			Vector retVal = new Vector();

			retVal.X = ReadFloat(stream);
			retVal.Y = ReadFloat(stream);

			if (!Flags.HasFlag(SendPropFlags.Normal))
				retVal.Z = ReadFloat(stream);
			else
			{
				throw new NotImplementedException();
			}

			return retVal;
		}
		
		Vector ReadVectorXY(BitStream stream)
		{
			Vector retVal = new Vector();

			retVal.X = ReadFloat(stream);
			retVal.Y = ReadFloat(stream);

			return retVal;
		}

		public override string ToString()
		{
			string bitCount = (BitCount.HasValue && BitCount.Value > 0) ? string.Format("[{0}]", BitCount.Value) : string.Empty;

			return string.Format("{0}{1} \"{2}.{3}\" ({4})", Type, bitCount, Parent.NetTableName, Name, Flags);
		}

		public SendPropDefinition Clone()
		{
			SendPropDefinition retVal = (SendPropDefinition)MemberwiseClone();
			return retVal;
		}
		object ICloneable.Clone() { return Clone(); }
	}
	public enum SendPropType
	{
		Int = 0,
		Float,
		Vector,
		VectorXY,
		String,
		Array,
		Datatable,
		//Quaternion,
		//Int64,
	}
	[Flags]
	public enum SendPropFlags
	{
		/// <summary>
		/// Unsigned integer data.
		/// </summary>
		Unsigned = (1 << 0),

		/// <summary>
		/// If this is set, the float/vector is treated like a world coordinate.
		/// Note that the bit count is ignored in this case.
		/// </summary>
		Coord = (1 << 1),

		/// <summary>
		/// For floating point, don't scale into range, just take value as is.
		/// </summary>
		NoScale = (1 << 2),

		/// <summary>
		/// For floating point, limit high value to range minus one bit unit
		/// </summary>
		RoundDown = (1 << 3),

		/// <summary>
		/// For floating point, limit low value to range minus one bit unit
		/// </summary>
		RoundUp = (1 << 4),

		/// <summary>
		/// If this is set, the vector is treated like a normal (only valid for vectors)
		/// </summary>
		Normal = (1 << 5),

		/// <summary>
		/// This is an exclude prop (not excludED, but it points at another prop to be excluded).
		/// </summary>
		Exclude = (1 << 6),

		/// <summary>
		/// Use XYZ/Exponent encoding for vectors.
		/// </summary>
		EncodeXYZE = (1 << 7),

		/// <summary>
		/// This tells us that the property is inside an array, so it shouldn't be put into the
		/// flattened property list. Its array will point at it when it needs to.
		/// </summary>
		InsideArray = (1 << 8),

		/// <summary>
		/// Set for datatable props using one of the default datatable proxies like
		/// SendProxy_DataTableToDataTable that always send the data to all clients.
		/// </summary>
		ProxyAlwaysYes = (1 << 9),

		/// <summary>
		/// this is an often changed field, moved to head of sendtable so it gets a small index
		/// </summary>
		ChangesOften = (1 << 10),

		/// <summary>
		/// Set automatically if SPROP_VECTORELEM is used.
		/// </summary>
		IsVectorElement = (1 << 11),

		/// <summary>
		/// Set automatically if it's a datatable with an offset of 0 that doesn't change the pointer
		/// (ie: for all automatically-chained base classes).
		/// In this case, it can get rid of this SendPropDataTable altogether and spare the
		/// trouble of walking the hierarchy more than necessary.
		/// </summary>
		Collapsible = (1 << 12),

		/// <summary>
		/// Like SPROP_COORD, but special handling for multiplayer games
		/// </summary>
		CoordMP = (1 << 13),

		/// <summary>
		/// Like SPROP_COORD, but special handling for multiplayer games where the fractional component only gets a 3 bits instead of 5
		/// </summary>
		CoordMPLowPrecision = (1 << 14),

		/// <summary>
		/// SPROP_COORD_MP, but coordinates are rounded to integral boundaries
		/// </summary>
		CoordMPIntegral = (1 << 15),

		/// <summary>
		/// reuse existing flag so we don't break demo. note you want to include SPROP_UNSIGNED if needed, its more efficient
		/// </summary>
		VarInt = Normal,
	}
}
