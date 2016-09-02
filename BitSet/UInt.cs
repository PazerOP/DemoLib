using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitSet
{
	public static partial class BitReader
	{
		public static ulong ReadUInt(byte[] buffer, ref ulong startBit, byte bits)
		{
			switch (bits)
			{
				case 1:		return ReadUInt1(buffer, ref startBit);

				case 6:		return ReadUInt6(buffer, ref startBit);

				default:	throw new NotImplementedException();
			}
		}
	}
}
