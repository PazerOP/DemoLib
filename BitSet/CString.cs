using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitSet
{
	public static partial class BitReader
	{
		public static string ReadCString(byte[] buffer, ref ulong bitOffset)
		{
			StringBuilder builder = new StringBuilder();

			char c;
			while ((c = (char)ReadUInt(buffer, ref bitOffset, 8)) != '\0')
			{
				builder.Append(c);
			}

			return builder.ToString();
		}
	}
}
