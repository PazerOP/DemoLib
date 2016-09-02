using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoLib
{
	class BitIO
	{
		static ulong CopyBits(byte[] source, ulong bitsToCopy, byte readBitOffset, ulong readByteOffset,
			byte[] dest, byte writeBitOffset = 0, ulong writeByteOffset = 0)
		{
			var bitsToCopyOriginal = bitsToCopy;

			if ((bitsToCopy % 8) == 0 && readBitOffset == 0 && writeBitOffset == 0)
			{
				// We're perfectly aligned, so we can just copy straight over.
				memcpy(&(((uint8_t*)voidDst)[writeByteOffset]), &(((uint8_t*)voidSrc)[readByteOffset]), size_t(bitsToCopy / 8));
				return bitsToCopy;
			}
		}
	}
}
