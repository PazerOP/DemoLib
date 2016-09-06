using System;
using System.Runtime.CompilerServices;

namespace BitSet
{
	public static partial class BitReader
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte ReadUInt8(byte[] buffer, ref ulong startBit)
		{
			var retVal = ReadUInt8(buffer, startBit);
			startBit += 8;
			return retVal;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte ReadUInt8(byte[] buffer, ulong startBit)
		{
			return ReadUInt8(buffer, (int)(startBit / 8), (byte)(startBit % 8));
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte ReadUInt8(byte[] buffer, int startByte = 0)
		{
			return buffer[startByte];
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte ReadUInt8(byte[] buffer, int startByte, byte bitOffset)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static byte ReadUInt8(byte* buffer, int startByte = 0)
		{
			return buffer[startByte];
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static byte ReadUInt8(byte* buffer, int startByte, byte bitOffset)
		{
			switch (bitOffset)
			{
				case 0: return ReadUInt8(buffer, startByte);

				case 1:
				case 2:
				case 3:
				case 4:
				case 5:
				case 6:
				case 7:
				throw new NotImplementedException();
			}

			throw new ArgumentOutOfRangeException(nameof(bitOffset));
		}
	}
	public static partial class BitWriter
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteUInt8(byte value, byte[] buffer, int startByte = 0)
		{
			buffer[startByte] = value;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteUInt8(byte value, byte[] buffer, int startByte, byte bitOffset)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteUInt8(byte value, byte* buffer, int startByte = 0)
		{
			buffer[startByte] = value;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteUInt8(byte value, byte* buffer, int startByte, byte bitOffset)
		{
			switch (bitOffset)
			{
				case 0: WriteUInt8(value, buffer, startByte); return;

				case 1:
				case 2:
				case 3:
				case 4:
				case 5:
				case 6:
				case 7:
				throw new NotImplementedException();
			}

			throw new ArgumentOutOfRangeException(nameof(bitOffset));
		}
	}
}
