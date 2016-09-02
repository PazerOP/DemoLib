using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace BitSet
{
	public static partial class BitReader
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte ReadUInt2(byte[] buffer, int startByte = 0)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte ReadUInt2(byte[] buffer, int startByte, byte bitOffset)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static byte ReadUInt2(byte* buffer, int startByte = 0)
		{
			return (byte)(buffer[startByte] & 0x03);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static byte ReadUInt2(byte* buffer, int startByte, byte bitOffset)
		{
			switch (bitOffset)
			{
				case 0: return ReadUInt2(buffer, startByte);
				case 1:
				case 2:
				case 3:
				case 4:
				case 5:	return (byte)((buffer[startByte] >> bitOffset) & 0x03);
				case 6:	return (byte)(buffer[startByte] >> bitOffset);

				case 7:
				throw new NotImplementedException();
			}

			throw new ArgumentOutOfRangeException(nameof(bitOffset));
		}
	}
	public static partial class BitWriter
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteUInt2(byte value, byte[] buffer, int startByte = 0)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteUInt2(byte value, byte[] buffer, int startByte, byte bitOffset)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteUInt2(byte value, byte* buffer, int startByte = 0)
		{
			buffer[startByte] = (byte)((buffer[startByte] & ~0x03) | (value & 0x03));
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteUInt2(byte value, byte* buffer, int startByte, byte bitOffset)
		{
			switch (bitOffset)
			{
				case 0:	WriteUInt2(value, buffer, startByte); return;
				case 1: buffer[startByte] = (byte)((buffer[startByte] & ~0x06) | ((value << 1) & 0x06)); return;
				case 2: buffer[startByte] = (byte)((buffer[startByte] & ~0x0C) | ((value << 2) & 0x0C)); return;
				case 3: buffer[startByte] = (byte)((buffer[startByte] & ~0x18) | ((value << 3) & 0x18)); return;
				case 4: buffer[startByte] = (byte)((buffer[startByte] & ~0x30) | ((value << 4) & 0x30)); return;
				case 5: buffer[startByte] = (byte)((buffer[startByte] & ~0x60) | ((value << 5) & 0x60)); return;
				case 6:	buffer[startByte] = (byte)((buffer[startByte] & ~0xC0) | (value << 6)); return;

				case 7:
				throw new NotImplementedException();
			}

			throw new ArgumentOutOfRangeException(nameof(bitOffset));
		}
	}
}
