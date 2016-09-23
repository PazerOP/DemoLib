using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitSet
{
	[DebuggerDisplay("{ToString(),nq}")]
	public class BitStream : ICloneable
	{
		public ulong Length
		{
			get { return m_MaxCursor - m_MinCursor; }
		}

		public ulong Cursor
		{
			get { return m_Cursor - m_MinCursor; }
			set
			{
				ulong newCursor = m_MinCursor + value;
				if (newCursor > m_MaxCursor)
					throw new OverflowException(string.Format("Attempted seek beyond the bounds of this {0}", nameof(BitStream)));

				m_Cursor = newCursor;
			}
		}

		byte[] m_Data;
		ulong m_Cursor;

		ulong m_MinCursor;
		ulong m_MaxCursor;

		private IEnumerable<Tuple<ulong, bool, ulong>> DebugBitsGrouped
		{
			get
			{
				ulong count = 0;
				ulong total = 0;
				bool old = false;
				foreach (bool bit in DebugBits)
				{
					if (bit != old && count > 0)
					{
						yield return Tuple.Create(count, old, total);
						count = 0;
					}

					old = bit;
					count++;
					total++;
				}
			}
		}

		private IEnumerable<bool> DebugBits
		{
			get
			{
				for (ulong i = m_Cursor; i < m_MaxCursor; i++)
				{
					ulong dummy = i;
					yield return BitReader.ReadBool(m_Data, ref dummy);
				}
			}
		}

		private BitStream() { }

		public BitStream(byte[] data, ulong? cursor = null, ulong minCursor = 0, ulong? maxCursor = null)
		{
			if (data == null)
				throw new ArgumentNullException(nameof(data));

			if (!maxCursor.HasValue)
				maxCursor = (ulong)data.LongLength * 8;

			if (minCursor > maxCursor.Value)
				throw new ArgumentOutOfRangeException(nameof(minCursor), string.Format("{0} ({1}) greater than {2} ({3})",
					nameof(minCursor), minCursor, nameof(maxCursor), maxCursor));

			if (!cursor.HasValue)
				cursor = minCursor;

			if (cursor.Value < minCursor || cursor.Value > maxCursor.Value)
				throw new ArgumentOutOfRangeException(nameof(cursor), string.Format("{0} ({1}) outside the range specified by {2} and {3} [{4}, {5}]", nameof(cursor), cursor, nameof(minCursor), nameof(maxCursor), minCursor, maxCursor));

			m_Data = data;
			m_Cursor = cursor.Value;
			m_MinCursor = minCursor;
			m_MaxCursor = maxCursor.Value;
		}

		public ulong ReadULong(byte bits = 64)
		{
			if (bits < 1 || bits > 64)
				throw new ArgumentOutOfRangeException(nameof(bits));

			ThrowIfOverflow(bits);

			return BitReader.ReadUIntBits(m_Data, ref m_Cursor, bits);
		}

		public short ReadShort(byte bits = 16)
		{
			if (bits < 1 || bits > 16)
				throw new ArgumentOutOfRangeException(nameof(bits));

			var test = ReadInt(bits);
			return (short)test;
		}
		public ushort ReadUShort(byte bits = 16)
		{
			if (bits < 1 || bits > 16)
				throw new ArgumentOutOfRangeException(nameof(bits));

			ThrowIfOverflow(bits);

			return (ushort)BitReader.ReadUIntBits(m_Data, ref m_Cursor, bits);
		}

		public uint ReadUInt(byte bits = 32)
		{
			if (bits < 1 || bits > 32)
				throw new ArgumentOutOfRangeException(nameof(bits));

			ThrowIfOverflow(bits);

			return (uint)BitReader.ReadUIntBits(m_Data, ref m_Cursor, bits);
		}
		public int ReadInt(byte bits = 32)
		{
			if (bits < 1 || bits > 32)
				throw new ArgumentOutOfRangeException(nameof(bits));

			ThrowIfOverflow(bits);

			uint raw = (uint)BitReader.ReadUIntBits(m_Data, ref m_Cursor, bits);
			if ((raw & (1UL << (bits - 1))) != 0)
			{
				uint filled = uint.MaxValue & (uint.MaxValue << bits);
				return unchecked((int)(filled | raw));
			}

			return (int)raw;
		}

		public byte ReadByte(byte bits = 8)
		{
			if (bits < 1 || bits > 8)
				throw new ArgumentOutOfRangeException(nameof(bits));

			ThrowIfOverflow(bits);

			return (byte)BitReader.ReadUIntBits(m_Data, ref m_Cursor, bits);
		}

		public byte[] ReadBytes(ulong bytes)
		{
			ThrowIfOverflow(bytes * 8);

			byte[] retVal = new byte[bytes];

			for (ulong i = 0; i < bytes; i++)
				retVal[i] = ReadByte(8);

			return retVal;
		}

		public float ReadSingle()
		{
			ThrowIfOverflow(32);

			return BitReader.ReadSingle(m_Data, ref m_Cursor);
		}

		public bool PeekBool()
		{
			ThrowIfOverflow(1);

			ulong dummy = m_Cursor;
			return BitReader.ReadUIntBits(m_Data, ref dummy, 1) == 1;
		}
		public bool ReadBool()
		{
			ThrowIfOverflow(1);

			return BitReader.ReadUIntBits(m_Data, ref m_Cursor, 1) == 1;
		}

		public string ReadCString()
		{
			ulong startCursor = m_Cursor;

			string retVal = BitReader.ReadCString(m_Data, ref m_Cursor);

			ulong endCursor = m_Cursor;
			m_Cursor = startCursor;

			ThrowIfOverflow(endCursor - startCursor);

			m_Cursor = endCursor;

			Debug.Assert(retVal != null);
			return retVal;
		}

		public char ReadChar()
		{
			ThrowIfOverflow(8);

			return Encoding.ASCII.GetChars(new byte[1] { ReadByte() }).Single();
		}

		public uint ReadVarInt()
		{
			ulong startCursor = m_Cursor;

			uint retVal = BitReader.ReadVarInt(m_Data, ref m_Cursor);

			ulong endCursor = m_Cursor;
			m_Cursor = startCursor;

			ThrowIfOverflow(endCursor - startCursor);

			m_Cursor = endCursor;

			return retVal;
		}

		public bool CheckOverflow(ulong bits)
		{
			if ((m_Cursor + bits) > m_MaxCursor)
				return true;
			else
				return false;
		}

		void ThrowIfOverflow(ulong bits)
		{
			if (CheckOverflow(bits))
				throw new OverflowException(string.Format("Attempted seek beyond the bounds of this {0}", nameof(BitStream)));
		}

		public BitStream Subsection(ulong minCursor = 0, ulong? maxCursor = null)
		{
			if (!maxCursor.HasValue)
				maxCursor = m_MaxCursor;

			if (minCursor > maxCursor.Value)
				throw new ArgumentOutOfRangeException(nameof(minCursor), string.Format("{0} ({1}) greater than {2} ({3})", nameof(minCursor), minCursor, nameof(maxCursor), maxCursor));

			if (maxCursor.Value > m_MaxCursor)
				throw new ArgumentOutOfRangeException(nameof(maxCursor), string.Format("{0} ({1}) greater than {2} ({3})", nameof(maxCursor), maxCursor, nameof(Length), Length));

			BitStream retVal = new BitStream();
			retVal.m_Data = m_Data;

			retVal.m_MinCursor = m_MinCursor + minCursor;

			if (maxCursor.HasValue)
				retVal.m_MaxCursor = m_MinCursor + maxCursor.Value;
			else
				retVal.m_MaxCursor = m_MaxCursor;

			retVal.m_Cursor = retVal.m_MinCursor;

			return retVal;
		}

		public ulong Seek(ulong bits, SeekOrigin origin)
		{
			if (origin == SeekOrigin.Begin)
				Cursor = bits;
			else if (origin == SeekOrigin.Current)
				Cursor += bits;
			else if (origin == SeekOrigin.End)
				Cursor = Length - bits;

			return Length;
		}
		public ulong Seek(long bits, SeekOrigin origin)
		{
			if (origin == SeekOrigin.Begin)
				Cursor = (ulong)bits;
			else if (origin == SeekOrigin.Current)
				Cursor = (ulong)((long)Cursor + bits);
			else if (origin == SeekOrigin.End)
				Cursor = (ulong)((long)Length - bits);

			return Cursor;
		}

		public BitStream Clone()
		{
			return (BitStream)MemberwiseClone();
		}
		object ICloneable.Clone() { return Clone(); }

		public override string ToString()
		{
			return string.Format("{0}: {1} / {2}", nameof(BitStream), Cursor, Length);
		}
	}
}
