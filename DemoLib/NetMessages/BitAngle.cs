using BitSet;

namespace DemoLib.NetMessages
{
	static class BitAngle
	{
		public static double Read(byte[] buffer, ref ulong bitOffset, byte bitCount)
		{
			double shift = (1 << bitCount);

			var rawValue = BitReader.ReadUIntBits(buffer, ref bitOffset, bitCount);
			return rawValue * (360 / shift);
		}
	}
}
