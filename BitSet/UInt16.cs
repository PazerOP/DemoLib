using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace BitSet
{
	public static partial class BitReader
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint ReadUInt16(byte[] buffer, ref ulong startBit)
		{
			var retVal = ReadUInt16(buffer, startBit);
			startBit += 16;
			return retVal;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint ReadUInt16(byte[] buffer, ulong startBit)
		{
			return ReadUInt16(buffer, (int)(startBit / 8), (byte)(startBit % 8));
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ushort ReadUInt16(byte[] buffer, int startByte = 0)
		{
			return (ushort)(buffer[startByte] | (buffer[startByte + 1] << 8));
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ushort ReadUInt16(byte[] buffer, int startByte, byte bitOffset)
		{
			switch (bitOffset)
			{
				case 0:	return ReadUInt16(buffer, startByte);

				case 1:
				case 2:
				case 3:
				case 4:
				case 5:
				case 6:
				case 7:
					return (ushort)((ReadUInt24(buffer, startByte) >> bitOffset) & 0xFFFF);
			}

			throw new ArgumentOutOfRangeException(nameof(bitOffset));
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ushort ReadUInt16(IReadOnlyList<byte> buffer, int startByte = 0)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ushort ReadUInt16(IReadOnlyList<byte> buffer, int startByte, byte bitOffset)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static ushort ReadUInt16(byte* buffer, int startByte = 0)
		{
			return *(ushort*)(&buffer[startByte]);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static ushort ReadUInt16(byte* buffer, int startByte, byte bitOffset)
		{
			throw new NotImplementedException();
		}
	}
	public static partial class BitWriter
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteUInt16(ushort value, byte[] buffer, int startByte = 0)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteUInt16(ushort value, byte[] buffer, int startByte, byte bitOffset)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteUInt16(ushort value, IList<byte> buffer, int startByte = 0)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteUInt16(ushort value, IList<byte> buffer, int startByte, byte bitOffset)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteUInt16(ushort value, byte* buffer, int startByte = 0)
		{
			*(ushort*)(&buffer[startByte]) = value;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteUInt16(ushort value, byte* buffer, int startByte, byte bitOffset)
		{
			throw new NotImplementedException();
		}
	}
}
