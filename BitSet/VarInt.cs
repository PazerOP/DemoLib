namespace BitSet
{
	public static partial class BitReader
	{
		const int MAX_VARINT_BITS = 35;

		public static uint ReadVarInt(byte[] buffer, ref ulong bitOffset)
		{
			uint dest = 0;

			for (byte run = 0; run < 35; run += 7)
			{
				byte oneByte = (byte)ReadUInt(buffer, ref bitOffset, 8);
				dest |= ((oneByte & (uint)0x7F) << run);

				if ((oneByte >> 7) == 0)
					break;
			}

			return dest;
		}
	}
}
