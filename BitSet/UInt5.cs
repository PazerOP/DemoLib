using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace BitSet
{
	public static partial class BitReader
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte ReadUInt5(byte[] buffer, int startByte = 0)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte ReadUInt5(byte[] buffer, int startByte, byte bitOffset)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static byte ReadUInt5(byte* buffer, int startByte = 0)
		{
			return (byte)(buffer[startByte] & 0x1F);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static byte ReadUInt5(byte* buffer, int startByte, byte bitOffset)
		{
			switch (bitOffset)
			{
				case 0: return ReadUInt5(buffer, startByte);

				case 2: return (byte)((buffer[startByte] & 0x7C) >> 2);
				case 3: return (byte)((buffer[startByte] & 0xF8) >> 3);

				case 5: return (byte)(((buffer[startByte + 1] & 0x03) << 3) | ((buffer[startByte] & 0xE0) >> 5));

				case 1:

				case 4:

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
		public static void WriteUInt5(byte value, byte[] buffer, int startByte = 0)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteUInt5(byte value, byte[] buffer, int startByte, byte bitOffset)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteUInt5(byte value, byte* buffer, int startByte = 0)
		{
			buffer[startByte] = (byte)((buffer[startByte] & ~0x1F) | (value & 0x1F));
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteUInt5(byte value, byte* buffer, int startByte, byte bitOffset)
		{
			switch (bitOffset)
			{
				case 0: WriteUInt5(value, buffer, startByte); return;

				case 2: buffer[startByte] = (byte)((buffer[startByte] & ~0x7C) | ((value << 2) & 0x7C)); return;
				case 3: buffer[startByte] = (byte)((buffer[startByte] & ~0xF8) | ((value << 3) & 0xF8)); return;

				case 5:
				{
					buffer[startByte + 1] = (byte)((buffer[startByte + 1] & ~0x03) | ((value >> 3) & 0x03));
					buffer[startByte] = (byte)((buffer[startByte] & ~0xE0) | ((value << 5) & 0xE0));
					Debug.Assert(((*(ushort*)(&buffer[startByte]) & 0x3E0) >> 5) == (value & 0x1F));
					return;
				}

				case 1:

				case 4:

				case 6:
				case 7:
				throw new NotImplementedException();
			}

			throw new ArgumentOutOfRangeException(nameof(bitOffset));
		}
	}
}
