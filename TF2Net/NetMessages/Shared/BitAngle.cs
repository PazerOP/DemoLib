using BitSet;

namespace TF2Net.NetMessages
{
	static class BitAngle
	{
		public static double Read(BitStream stream, byte bitCount)
		{
			double shift = (1 << bitCount);

			var rawValue = stream.ReadULong(bitCount);
			return rawValue * (360 / shift);
		}
	}
}
