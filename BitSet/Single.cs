using System;
using System.Runtime.CompilerServices;

namespace BitSet
{
	public static partial class BitReader
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float ReadSingle(byte[] buffer, int startByte = 0)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float ReadSingle(byte[] buffer, int startByte, byte bitOffset)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static float ReadSingle(byte* buffer, int startByte = 0)
		{
			return *(float*)(&buffer[startByte]);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static float ReadSingle(byte* buffer, int startByte, byte bitOffset)
		{
			throw new NotImplementedException();
		}
	}
	public static partial class BitWriter
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteSingle(float value, byte[] buffer, int startByte = 0)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteSingle(float value, byte[] buffer, int startByte, byte bitOffset)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteSingle(float value, byte* buffer, int startByte = 0)
		{
			*(float*)(&buffer[startByte]) = value;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteSingle(float value, byte* buffer, int startByte, byte bitOffset)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteSingle(double value, byte[] buffer, int startByte = 0)
		{
			WriteSingle((float)value, buffer, startByte);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteSingle(double value, byte[] buffer, int startByte, byte bitOffset)
		{
			WriteSingle((float)value, buffer, startByte, bitOffset);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteSingle(double value, byte* buffer, int startByte = 0)
		{
			WriteSingle((float)value, buffer, startByte);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteSingle(double value, byte* buffer, int startByte, byte bitOffset)
		{
			WriteSingle((float)value, buffer, startByte, bitOffset);
		}
	}
}
