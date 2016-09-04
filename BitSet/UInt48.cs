using System;
using System.Runtime.CompilerServices;

namespace BitSet
{	public static partial class BitReader
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong ReadUInt48(byte[] buffer, ref ulong startBit)
		{
			var retVal = ReadUInt48(buffer, startBit);
			startBit += 48;
			return retVal;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong ReadUInt48(byte[] buffer, ulong startBit)
		{
			return ReadUInt48(buffer, (int)(startBit / 8), (byte)(startBit % 8));
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong ReadUInt48(byte[] buffer, int startByte = 0)
		{
			return ReadUInt32(buffer, startByte) | ((ulong)ReadUInt8(buffer, startByte + 4) << 32);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong ReadUInt48(byte[] buffer, int startByte, byte bitOffset)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static ulong ReadUInt48(byte* buffer, int startByte = 0)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static ulong ReadUInt48(byte* buffer, int startByte, byte bitOffset)
		{
			throw new NotImplementedException();
		}
	}
	public static partial class BitWriter
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteUInt48(ulong value, byte[] buffer, int startByte = 0)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteUInt48(ulong value, byte[] buffer, int startByte, byte bitOffset)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteUInt48(ulong value, byte* buffer, int startByte = 0)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteUInt48(ulong value, byte* buffer, int startByte, byte bitOffset)
		{
			throw new NotImplementedException();
		}
	}
}
