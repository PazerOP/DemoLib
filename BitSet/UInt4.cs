using System;
using System.Runtime.CompilerServices;

namespace BitSet
{
	public static partial class BitReader
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte ReadUInt4(byte[] buffer, ref ulong startBit)
		{
			var retVal = ReadUInt4(buffer, startBit);
			startBit += 4;
			return retVal;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte ReadUInt4(byte[] buffer, ulong startBit)
		{
			return ReadUInt4(buffer, (int)(startBit / 8), (byte)(startBit % 8));
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte ReadUInt4(byte[] buffer, int startByte = 0)
		{
			return (byte)(buffer[startByte] & 0xF);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte ReadUInt4(byte[] buffer, int startByte, byte bitOffset)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static byte ReadUInt4(byte* buffer, int startByte = 0)
		{
			return (byte)(buffer[startByte] & 0xF);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static byte ReadUInt4(byte* buffer, int startByte, byte bitOffset)
		{
			switch (bitOffset)
			{
				case 0:
				case 1:
				case 2:
				case 3:
				return (byte)((buffer[startByte] >> bitOffset) & 0xF);

				case 4:
				case 5:
				case 6:
				case 7:
				return (byte)((*(ushort*)(&buffer[startByte]) >> bitOffset) & 0xF);
			}

			throw new ArgumentOutOfRangeException(nameof(bitOffset));
		}
	}
	public static partial class BitWriter
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteUInt4(byte value, byte[] buffer, int startByte = 0)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteUInt4(byte value, byte[] buffer, int startByte, byte bitOffset)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteUInt4(byte value, byte* buffer, int startByte = 0)
		{
			buffer[startByte] = (byte)((buffer[startByte] & ~0x0F) | (value & 0x0F));
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteUInt4(byte value, byte* buffer, int startByte, byte bitOffset)
		{
			switch (bitOffset)
			{
				case 0: WriteUInt4(value, buffer, startByte); return;

				case 4: buffer[startByte] = (byte)((buffer[startByte] & 0x0F) | (value << 4)); return;

				case 1:
				case 2:
				case 3:
				case 5:
				case 6:
				case 7:
				throw new NotImplementedException();
			}

			throw new ArgumentOutOfRangeException(nameof(bitOffset));
		}
	}
}
