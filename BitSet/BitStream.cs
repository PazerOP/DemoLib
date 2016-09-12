using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitSet
{
	public class BitStream : ICloneable
	{
		public ulong Length
		{
			get { return m_MaxCursor - m_MinCursor; }
		}

		public ulong Cursor
		{
			get { return m_Cursor - m_MinCursor; }
			set { throw new NotImplementedException(); }
		}

		byte[] m_Data;
		ulong m_Cursor;

		ulong m_MinCursor = ulong.MinValue;
		ulong m_MaxCursor = ulong.MaxValue;

		private BitStream() { }

		public BitStream(byte[] data, ulong cursor = 0, ulong minCursor = ulong.MinValue, ulong maxCursor = ulong.MaxValue)
		{
			if (data == null)
				throw new ArgumentNullException(nameof(data));
			if (cursor > ((ulong)data.Length * 8))
				throw new ArgumentException("Cursor position is larger than data length");

			m_Data = data;
			m_Cursor = cursor;
		}

		public ulong ReadULong(byte bits = 64)
		{
			if (bits < 1 || bits > 64)
				throw new ArgumentOutOfRangeException(nameof(bits));

			return BitReader.ReadUIntBits(m_Data, ref m_Cursor, bits);
		}

		public short ReadShort(byte bits = 16)
		{
			if (bits < 1 || bits > 16)
				throw new ArgumentOutOfRangeException(nameof(bits));

			return (short)BitReader.ReadUIntBits(m_Data, ref m_Cursor, bits);
		}
		public ushort ReadUShort(byte bits = 16)
		{
			if (bits < 1 || bits > 16)
				throw new ArgumentOutOfRangeException(nameof(bits));

			return (ushort)BitReader.ReadUIntBits(m_Data, ref m_Cursor, bits);
		}

		public uint ReadUInt(byte bits = 32)
		{
			if (bits < 1 || bits > 32)
				throw new ArgumentOutOfRangeException(nameof(bits));

			return (uint)BitReader.ReadUIntBits(m_Data, ref m_Cursor, bits);
		}
		public int ReadInt(byte bits = 32)
		{
			if (bits < 1 || bits > 32)
				throw new ArgumentOutOfRangeException(nameof(bits));

			return (int)BitReader.ReadUIntBits(m_Data, ref m_Cursor, bits);
		}

		public byte ReadByte(byte bits = 8)
		{
			if (bits < 1 || bits > 8)
				throw new ArgumentOutOfRangeException(nameof(bits));

			return (byte)BitReader.ReadUIntBits(m_Data, ref m_Cursor, bits);
		}

		public float ReadSingle()
		{
			return BitReader.ReadSingle(m_Data, ref m_Cursor);
		}

		public bool ReadBool()
		{
			return BitReader.ReadUIntBits(m_Data, ref m_Cursor, 1) == 1;
		}

		public string ReadCString()
		{
			return BitReader.ReadCString(m_Data, ref m_Cursor);
		}

		public char ReadChar()
		{
			return Encoding.ASCII.GetChars(new byte[1] { ReadByte() }).Single();
		}

		public uint ReadVarInt()
		{
			return BitReader.ReadVarInt(m_Data, ref m_Cursor);
		}

		public BitStream Subsection(ulong minCursor = ulong.MinValue, ulong maxCursor = ulong.MaxValue)
		{
			BitStream retVal = new BitStream();
			retVal.m_Data = m_Data;
			retVal.m_Cursor = 0;

			retVal.m_MinCursor = m_MinCursor + minCursor;

			ulong overflowCheck = unchecked(retVal.m_MinCursor + maxCursor);
			if (overflowCheck < maxCursor)
				retVal.m_MaxCursor = ulong.MaxValue;
			else
				retVal.m_MaxCursor = overflowCheck;

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
			return new BitStream(m_Data, m_Cursor);
		}
		object ICloneable.Clone() { return Clone(); }
	}
}
