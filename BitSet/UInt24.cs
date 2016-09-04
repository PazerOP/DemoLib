using System;
using System.Runtime.CompilerServices;

namespace BitSet
{
	public static partial class BitReader
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint ReadUInt24(byte[] buffer, ref ulong startBit)
		{
			var retVal = ReadUInt24(buffer, startBit);
			startBit += 24;
			return retVal;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint ReadUInt24(byte[] buffer, ulong startBit)
		{
			return ReadUInt24(buffer, (int)(startBit / 8), (byte)(startBit % 8));
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint ReadUInt24(byte[] buffer, int startByte = 0)
		{
			return (uint)(ReadUInt16(buffer, startByte) | (ReadUInt8(buffer, startByte + 2) << 16));
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint ReadUInt24(byte[] buffer, int startByte, byte bitOffset)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static uint ReadUInt24(byte* buffer, int startByte = 0)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static uint ReadUInt24(byte* buffer, int startByte, byte bitOffset)
		{
			throw new NotImplementedException();
		}
	}
	public static partial class BitWriter
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteUInt24(uint value, byte[] buffer, int startByte = 0)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteUInt24(uint value, byte[] buffer, int startByte, byte bitOffset)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteUInt24(uint value, byte* buffer, int startByte = 0)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteUInt24(uint value, byte* buffer, int startByte, byte bitOffset)
		{
			throw new NotImplementedException();
		}
	}
}
