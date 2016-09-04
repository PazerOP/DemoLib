using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace BitSet
{
	public static partial class BitReader
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte ReadUInt6(byte[] buffer, ref ulong startBit)
		{
			var retVal = ReadUInt6(buffer, startBit);
			startBit += 6;
			return retVal;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte ReadUInt6(byte[] buffer, ulong startBit)
		{
			return ReadUInt6(buffer, (int)(startBit / 8), (byte)(startBit % 8));
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte ReadUInt6(byte[] buffer, int startByte = 0)
		{
			return (byte)(buffer[startByte] & 0x3F);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte ReadUInt6(byte[] buffer, int startByte, byte bitOffset)
		{
			switch (bitOffset)
			{
				case 0:		return (byte)(buffer[startByte] & 0x3F);
				case 1:		return (byte)((buffer[startByte] & 0x7E) >> 1);
				case 2:		return (byte)((buffer[startByte] & 0xFC) >> 2);

				case 3:
				case 4:
				case 5:
				case 6:
				case 7:
				return (byte)((ReadUInt16(buffer, startByte) >> bitOffset) & 0x3F);
			}

			throw new ArgumentOutOfRangeException(nameof(bitOffset));
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static byte ReadUInt6(byte* buffer, int startByte = 0)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static byte ReadUInt6(byte* buffer, int startByte, byte bitOffset)
		{
			switch (bitOffset)
			{
				case 5: return (byte)(((buffer[startByte + 1] & 0x07) << 3) | ((buffer[startByte] & 0xE0) >> 5));

				case 0:
				case 1:
				case 2:
				case 3:
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
		public static void WriteUInt6(byte value, byte[] buffer, int startByte = 0)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteUInt6(byte value, byte[] buffer, int startByte, byte bitOffset)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteUInt6(byte value, byte* buffer, int startByte = 0)
		{
			throw new NotImplementedException();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteUInt6(byte value, byte* buffer, int startByte, byte bitOffset)
		{
			switch (bitOffset)
			{
				case 5:
				{
					buffer[startByte + 1] = (byte)((buffer[startByte + 1] & ~0x07) | ((value >> 3) & 0x07));
					buffer[startByte] = (byte)((buffer[startByte] & ~0xE0) | ((value << 5) & 0xE0 ));
					Debug.Assert(((*(ushort*)(&buffer[startByte]) & 0x7E0) >> 5) == (value & 0x3F));
					return;
				}

				case 0:
				case 1:
				case 2:
				case 3:
				case 4:
				case 6:
				case 7:
				throw new NotImplementedException();
			}

			throw new ArgumentOutOfRangeException(nameof(bitOffset));
		}
	}
}
