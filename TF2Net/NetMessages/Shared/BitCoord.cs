using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitSet;

namespace TF2Net.NetMessages
{
	static class BitCoord
	{
		const uint COORD_INTEGER_BITS = 14;
		const uint COORD_FRACTIONAL_BITS = 5;
		const uint COORD_DENOMINATOR = (1 << (int)(COORD_FRACTIONAL_BITS));
		const double COORD_RESOLUTION = (1.0 / (COORD_DENOMINATOR));

		public static double Read(byte[] source, ref ulong readBitOffset)
		{
			// Read the required integer and fraction flags
			int intVal = BitReader.ReadUInt1(source, ref readBitOffset);
			int fractVal = BitReader.ReadUInt1(source, ref readBitOffset);

			// If we got either parse them, otherwise it's a zero.
			if (intVal != 0 || fractVal != 0)
			{
				// Read the sign bit
				var signBit = BitReader.ReadUInt1(source, ref readBitOffset);

				// If there's an integer, read it in
				if (intVal != 0)
				{
					// Adjust the integers from [0..MAX_COORD_VALUE-1] to [1..MAX_COORD_VALUE]
					intVal = (int)BitReader.ReadUIntBits(source, ref readBitOffset, (byte)COORD_INTEGER_BITS);
					intVal++;
				}

				// If there's a fraction, read it in
				if (fractVal != 0)
				{
					fractVal = (int)BitReader.ReadUIntBits(source, ref readBitOffset, (byte)COORD_FRACTIONAL_BITS);
				}

				// Calculate the correct floating point value
				double retVal = Math.Abs(intVal + ((float)fractVal * COORD_RESOLUTION));

				// Fixup the sign if negative.
				if (signBit != 0)
					retVal = -retVal;

				return retVal;
			}

			return 0;
	    }

	    public static double Read(BitStream source)
	    {
	        // Read the required integer and fraction flags
	        bool intFlag = source.ReadBool();
	        bool fractFlag = source.ReadBool();
	        ulong intVal = 0;
	        ulong fractVal = 0;

	        // If we got either parse them, otherwise it's a zero.
	        if (intFlag || fractFlag)
	        {
	            // Read the sign bit
	            var signBit = source.ReadBool();

	            // If there's an integer, read it in
	            if (intFlag)
	            {
	                // Adjust the integers from [0..MAX_COORD_VALUE-1] to [1..MAX_COORD_VALUE]
	                intVal = source.ReadULong((byte)COORD_INTEGER_BITS) + 1;
	            }

	            // If there's a fraction, read it in
	            if (fractFlag)
	            {
	                fractVal = source.ReadULong((byte)COORD_FRACTIONAL_BITS);
	            }

	            // Calculate the correct floating point value
	            double retVal = Math.Abs(intVal + (fractVal * COORD_RESOLUTION));

	            // Fixup the sign if negative.
	            if (signBit)
	                retVal = -retVal;

	            return retVal;
	        }

	        return 0;
	    }
    }
}
