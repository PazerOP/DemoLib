using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace BitSet
{
	public static partial class BitReader
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ushort ReadUInt16(byte[] buffer, int startByte = 0)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ushort ReadUInt16(byte[] buffer, int startByte, byte bitOffset)
		{
			throw new NotImplementedException();
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
