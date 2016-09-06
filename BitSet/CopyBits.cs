using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BitSet
{
	public static partial class BitReader
	{
		[DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
		private static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

		public static void CopyBits(byte[] src, ulong bitsToCopy, ref ulong readBitOffset, byte[] dest)
		{
			ulong dummy = 0;
			CopyBits(src, bitsToCopy, ref readBitOffset, dest, ref dummy);
		}

		public static void CopyBits(byte[] src, ulong bitsToCopy, ref ulong readBitOffset,
			byte[] dest, ref ulong writeBitOffset)
		{
			var copied = CopyBits(src, bitsToCopy, (byte)(readBitOffset % 8), readBitOffset / 8,
				dest, (byte)(writeBitOffset % 8), writeBitOffset / 8);

			readBitOffset += copied;
			writeBitOffset += copied;
		}

		public static unsafe ulong CopyBits(byte[] src, ulong bitsToCopy, byte readBitOffset, ulong readByteOffset,
			byte[] dest, byte writeBitOffset = 0, ulong writeByteOffset = 0)
		{
			if ((bitsToCopy % 8) == 0 && readBitOffset == 0 && writeBitOffset == 0)
			{
				// We're perfectly aligned, so we can just copy straight over.
				Array.ConstrainedCopy(src, (int)readByteOffset, dest, (int)writeByteOffset, (int)(bitsToCopy / 8));
				return bitsToCopy;
			}

			fixed (void* voidSrc = src, voidDst = dest)
			{
				var bitsToCopyOriginal = bitsToCopy;

				while (bitsToCopy > 0)
				{
					ulong buffer = 0;
					byte bytesToRead = (byte)Math.Min(BitInfo.BitsToBytes(bitsToCopy + readBitOffset), 8);

					CopyMemory(new IntPtr(&((byte*)&buffer)[0]), new IntPtr(&((byte*)voidSrc)[readByteOffset]), bytesToRead);
					//memcpy(&((byte*)&buffer)[0], &((byte*)voidSrc)[readByteOffset], bytesToRead);

					// Cut off any high bits we don't want
					ulong readMask = (0xFFFFFFFFFFFFFFFF << readBitOffset) &
						(0xFFFFFFFFFFFFFFFF >> (int)(64 - Math.Min(readBitOffset + bitsToCopy, 64)));

					buffer &= readMask;

					// Shift everything right so the first read bit is actually at bit 0
					buffer = (buffer >> readBitOffset);

					buffer = (buffer << writeBitOffset);

					ulong writeMask =
						(0xFFFFFFFFFFFFFFFF << writeBitOffset) &
						(0xFFFFFFFFFFFFFFFF >> (int)(64 - Math.Min(writeBitOffset + bitsToCopy, 64)));

					unchecked
					{
						if ((bitsToCopy + writeBitOffset) > 56)             // >= 8 bytes available
						{
							// Write 8 bytes.

							// Clear bits to 0 first
							*(ulong*)(&(((byte*)voidDst)[writeByteOffset])) &= ~writeMask;

							// Now write the data
							*(ulong*)(&(((byte*)voidDst)[writeByteOffset])) |= (buffer & writeMask);
						}
						else if ((bitsToCopy + writeBitOffset) > 24)        // >= 4 bytes available
						{
							// Write byte 1 through 4
							*(uint*)(&(((byte*)voidDst)[writeByteOffset])) &= ~(uint)writeMask;
							*(uint*)(&(((byte*)voidDst)[writeByteOffset])) |= (uint)(buffer & writeMask);

							if ((bitsToCopy + writeBitOffset) > 40)         // >= 6 bytes available
							{
								// Write byte 5 and 6
								*(ushort*)(&(((byte*)voidDst)[writeByteOffset + 4])) &= (ushort)~(writeMask >> 4 * 8);
								*(ushort*)(&(((byte*)voidDst)[writeByteOffset + 4])) |= (ushort)((buffer >> 4 * 8) & (writeMask >> 4 * 8));
							}
							else if ((bitsToCopy + writeBitOffset) > 32)    // >= 5 bytes available
							{
								// Write byte 5
								*(&(((byte*)voidDst)[writeByteOffset + 4])) &= (byte)~(writeMask >> 4 * 8);
								*(&(((byte*)voidDst)[writeByteOffset + 4])) |= (byte)((buffer >> 4 * 8) & (writeMask >> 4 * 8));
							}

							if ((bitsToCopy + writeBitOffset) > 48)         // >= 7 bytes available
							{
								// Write byte 7
								*(&(((byte*)voidDst)[writeByteOffset + 6])) &= (byte)~(writeMask >> 6 * 8);
								*(&(((byte*)voidDst)[writeByteOffset + 6])) |= (byte)((buffer >> 6 * 8) & (writeMask >> 6 * 8));
							}
						}
						else if ((bitsToCopy + writeBitOffset) > 8)         // >= 2 bytes available
						{
							// Write byte 1 and 2
							*(ushort*)(&(((byte*)voidDst)[writeByteOffset])) &= (ushort)~writeMask;
							*(ushort*)(&(((byte*)voidDst)[writeByteOffset])) |= (ushort)(buffer & writeMask);

							if ((bitsToCopy + writeBitOffset) > 16)         // >= 3 bytes available
							{
								// Write byte 3
								*(&(((byte*)voidDst)[writeByteOffset + 2])) &= (byte)~(writeMask >> 2 * 8);
								*(&(((byte*)voidDst)[writeByteOffset + 2])) |= (byte)((buffer >> 2 * 8) & (writeMask >> 2 * 8));
							}
						}
						else                                                // >= 1 byte available
						{
							// Write byte 1
							*(&(((byte*)voidDst)[writeByteOffset])) &= (byte)~writeMask;
							*(&(((byte*)voidDst)[writeByteOffset])) |= (byte)(buffer & writeMask);
						}
					}

					byte bytesRead = Math.Min(bytesToRead, (byte)7);
					bitsToCopy -= Math.Min(bitsToCopy, (ulong)(bytesRead * 8));
					readByteOffset += bytesRead;
					writeByteOffset += bytesRead;
				}

				return bitsToCopyOriginal;
			}			
		}
	}
}
