using System;
using System.Runtime.CompilerServices;

namespace BitSet
{
	public static partial class BitReader
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint ReadUInt32(byte[] buffer, ref ulong startBit)
		{
			var retVal = ReadUInt32(buffer, startBit);
			startBit += 32;
			return retVal;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint ReadUInt32(byte[] buffer, ulong startBit)
		{
			return ReadUInt32(buffer, (int)(startBit / 8), (byte)(startBit % 8));
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint ReadUInt32(byte[] buffer, int startByte = 0)
		{
			return BitConverter.ToUInt32(buffer, startByte);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint ReadUInt32(byte[] buffer, int startByte, byte bitOffset)
		{
			switch (bitOffset)
			{
				case 0:	return ReadUInt32(buffer, startByte);
				
				case 1:
				case 2:
				case 3:
				case 4:
				case 5:
				case 6:
				case 7:
				return (uint)((ReadUInt48(buffer, startByte) >> bitOffset) & 0xFFFFFFFF);
			}

			throw new ArgumentOutOfRangeException(nameof(bitOffset));
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static uint ReadUInt32(byte* buffer, int startByte = 0)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static uint ReadUInt32(byte* buffer, int startByte, byte bitOffset)
		{
			throw new NotImplementedException();
		}
	}
	public static partial class BitWriter
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteUInt32(uint value, byte[] buffer, int startByte = 0)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteUInt32(uint value, byte[] buffer, int startByte, byte bitOffset)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteUInt32(uint value, byte* buffer, int startByte = 0)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteUInt32(uint value, byte* buffer, int startByte, byte bitOffset)
		{
			throw new NotImplementedException();
		}
	}
}
