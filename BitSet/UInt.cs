using System;
using System.Linq;
using System.Text;

namespace BitSet
{
	public static partial class BitReader
	{
		public static ulong ReadUIntBits(byte[] buffer, ref ulong bitOffset, byte bits)
		{
#if false
			switch (bits)
			{
				case 1:		return ReadUInt1(buffer, ref bitOffset);
				case 2:		return ReadUInt2(buffer, ref bitOffset);
				case 3:		return ReadUInt3(buffer, ref bitOffset);
				case 4:		return ReadUInt4(buffer, ref bitOffset);
				case 5:		return ReadUInt5(buffer, ref bitOffset);
				case 6:		return ReadUInt6(buffer, ref bitOffset);

				case 16:	return ReadUInt16(buffer, ref bitOffset);

				case 20:	return ReadUInt20(buffer, ref bitOffset);

				case 32:	return ReadUInt32(buffer, ref bitOffset);

				case 48:	return ReadUInt48(buffer, ref bitOffset);

				default:	throw new NotImplementedException();
			}
#endif

			byte[] temp = new byte[8];
			CopyBits(buffer, bits, ref bitOffset, temp);
			return BitConverter.ToUInt64(temp, 0);
		}

		public static float ReadSingle(byte[] buffer, ref ulong bitOffset)
		{
			return BitConverter.ToSingle(BitConverter.GetBytes(ReadUInt(buffer, ref bitOffset)), 0);
		}

		public static ulong ReadULong(byte[] buffer, ref ulong bitOffset)
		{
			return ReadUIntBits(buffer, ref bitOffset, sizeof(ulong) << 3);
		}
		public static uint ReadUInt(byte[] buffer, ref ulong bitOffset)
		{
			return (uint)ReadUIntBits(buffer, ref bitOffset, sizeof(uint) << 3);
		}
		public static int ReadInt(byte[] buffer, ref ulong bitOffset)
		{
			return unchecked((int)ReadUIntBits(buffer, ref bitOffset, sizeof(int) << 3));
		}
		public static ushort ReadUShort(byte[] buffer, ref ulong bitOffset)
		{
			return (ushort)ReadUIntBits(buffer, ref bitOffset, sizeof(ushort) << 3);
		}
		public static short ReadShort(byte[] buffer, ref ulong bitOffset)
		{
			return unchecked((short)ReadUIntBits(buffer, ref bitOffset, sizeof(short) << 3));
		}
		public static char ReadChar(byte[] buffer, ref ulong bitOffset)
		{
			return Encoding.ASCII.GetChars(new byte[] { ReadByte(buffer, ref bitOffset) }).Single();
		}
		public static byte ReadByte(byte[] buffer, ref ulong bitOffset)
		{
			return (byte)ReadUIntBits(buffer, ref bitOffset, sizeof(byte) << 3);
		}
		public static bool ReadBool(byte[] buffer, ref ulong bitOffset)
		{
			return ReadUIntBits(buffer, ref bitOffset, 1) != 0;
		}
	}
}
