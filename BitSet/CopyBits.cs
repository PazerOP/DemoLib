using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitSet
{
	public static partial class BitReader
	{
		static ulong CopyBits(byte[] src, ulong bitsToCopy, byte readBitOffset, ulong readByteOffset,
			byte[] dest, byte writeBitOffset = 0, ulong writeByteOffset = 0)
		{
			if ((bitsToCopy % 8) == 0 && readBitOffset == 0 && writeBitOffset == 0)
			{
				// We're perfectly aligned, so we can just copy straight over.
				Array.ConstrainedCopy(src, (int)readByteOffset, dest, (int)writeByteOffset, (int)(bitsToCopy / 8));
				return bitsToCopy;
			}

			var bitsToCopyOriginal = bitsToCopy;

			while (bitsToCopy > 0)
			{
				ulong buffer = BitConverter.ToUInt64(src, (int)readByteOffset);
				byte bytesToRead = (byte)Math.Min(BitInfo.BitsToBytes(bitsToCopy + readBitOffset), 8);

				memcpy(&((uint8_t*)&buffer)[0], &((uint8_t*)voidSrc)[readByteOffset], bytesToRead);

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

				if ((bitsToCopy + writeBitOffset) > 56)             // >= 8 bytes available
				{
					// Write 8 bytes.

					// Clear bits to 0 first
					*(uint64_t*)(&(((uint8_t*)voidDst)[writeByteOffset])) &= ~writeMask;

					// Now write the data
					*(uint64_t*)(&(((uint8_t*)voidDst)[writeByteOffset])) |= (buffer & writeMask);
				}
				else if ((bitsToCopy + writeBitOffset) > 24)        // >= 4 bytes available
				{
					// Write byte 1 through 4
					*(uint32_t*)(&(((uint8_t*)voidDst)[writeByteOffset])) &= ~writeMask;
					*(uint32_t*)(&(((uint8_t*)voidDst)[writeByteOffset])) |= (buffer & writeMask);

					if ((bitsToCopy + writeBitOffset) > 40)         // >= 6 bytes available
					{
						// Write byte 5 and 6
						*(uint16_t*)(&(((uint8_t*)voidDst)[writeByteOffset + 4])) &= ~(writeMask >> 4 * 8);
						*(uint16_t*)(&(((uint8_t*)voidDst)[writeByteOffset + 4])) |= ((buffer >> 4 * 8) & (writeMask >> 4 * 8));
					}
					else if ((bitsToCopy + writeBitOffset) > 32)    // >= 5 bytes available
					{
						// Write byte 5
						*(uint8_t*)(&(((uint8_t*)voidDst)[writeByteOffset + 4])) &= ~(writeMask >> 4 * 8);
						*(uint8_t*)(&(((uint8_t*)voidDst)[writeByteOffset + 4])) |= ((buffer >> 4 * 8) & (writeMask >> 4 * 8));
					}

					if ((bitsToCopy + writeBitOffset) > 48)         // >= 7 bytes available
					{
						// Write byte 7
						*(uint8_t*)(&(((uint8_t*)voidDst)[writeByteOffset + 6])) &= ~(writeMask >> 6 * 8);
						*(uint8_t*)(&(((uint8_t*)voidDst)[writeByteOffset + 6])) |= ((buffer >> 6 * 8) & (writeMask >> 6 * 8));
					}
				}
				else if ((bitsToCopy + writeBitOffset) > 8)         // >= 2 bytes available
				{
					// Write byte 1 and 2
					*(uint16_t*)(&(((uint8_t*)voidDst)[writeByteOffset])) &= ~writeMask;
					*(uint16_t*)(&(((uint8_t*)voidDst)[writeByteOffset])) |= (buffer & writeMask);

					if ((bitsToCopy + writeBitOffset) > 16)         // >= 3 bytes available
					{
						// Write byte 3
						*(uint8_t*)(&(((uint8_t*)voidDst)[writeByteOffset + 2])) &= ~(writeMask >> 2 * 8);
						*(uint8_t*)(&(((uint8_t*)voidDst)[writeByteOffset + 2])) |= ((buffer >> 2 * 8) & (writeMask >> 2 * 8));
					}
				}
				else                                                // >= 1 byte available
				{
					// Write byte 1
					*(&(((uint8_t*)voidDst)[writeByteOffset])) &= ~writeMask;
					*(&(((uint8_t*)voidDst)[writeByteOffset])) |= (buffer & writeMask);
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
