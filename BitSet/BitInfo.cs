using System.Runtime.CompilerServices;

namespace BitSet
{
	public static class BitInfo
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static sbyte GetFirstBitIndex(ulong mask)
		{
			for (sbyte i = 0; i < 64; i++)
			{
				if ((mask & (1UL << i)) > 0)
					return i;
			}

			return -1;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static sbyte GetFirstBitIndex(long mask)
		{
			return GetFirstBitIndex((ulong)mask);
		}

		public static ulong BitsToBytes(ulong bits)
		{
			return (bits + 7) >> 3;
		}
	}
}
