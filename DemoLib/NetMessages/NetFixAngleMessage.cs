using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitSet;

namespace DemoLib.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	class NetFixAngleMessage : INetMessage
	{
		const int ANGLE_BITS = 16;

		public bool Relative { get; set; }
		public double[] Angle { get; set; }

		public string Description
		{
			get
			{
				return string.Format("svc_FixAngle: {0} {1:N1} {2:N1} {3:N1}",
					Relative ? "relative" : "absolute",
					Angle[0], Angle[1], Angle[2]);
			}
		}

		public ulong Size
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public void ReadMsg(byte[] buffer, ref ulong bitOffset)
		{
			Relative = BitReader.ReadUIntBits(buffer, ref bitOffset, 1) != 0;

			Angle = new double[3];
			Angle[0] = BitAngle.Read(buffer, ref bitOffset, ANGLE_BITS);
			Angle[1] = BitAngle.Read(buffer, ref bitOffset, ANGLE_BITS);
			Angle[2] = BitAngle.Read(buffer, ref bitOffset, ANGLE_BITS);
		}

		public void WriteMsg(byte[] buffer, ref ulong bitOffset)
		{
			throw new NotImplementedException();
		}
	}
}
