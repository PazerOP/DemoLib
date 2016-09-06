using System;

namespace BitSet
{
	public static partial class BitReader
	{
		public static ulong ReadUInt(byte[] buffer, ref ulong bitOffset, byte bits)
		{
#if false
			switch (bits)
			{
				case 1:		return ReadUInt1(buffer, ref bitOffset);
				case 2:		return ReadUInt2(buffer, ref bitOffset);
				case 3:		return ReadUInt3(buffer, ref bitOffset);
				case 4:		return ReadUInt4(buffer, ref bitOffset);
				case 5:		return ReadUInt5(buffer, ref bitOffset);
				case 6:		return ReadUInt6(buffer, ref bitOffset);

				case 16:	return ReadUInt16(buffer, ref bitOffset);

				case 20:	return ReadUInt20(buffer, ref bitOffset);

				case 32:	return ReadUInt32(buffer, ref bitOffset);

				case 48:	return ReadUInt48(buffer, ref bitOffset);

				default:	throw new NotImplementedException();
			}
#endif
			
			byte[] temp = new byte[8];
			CopyBits(buffer, bits, ref bitOffset, temp);
			return BitConverter.ToUInt64(temp, 0);
		}
	}
}
