using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitSet;

namespace TF2Net.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	public class NetFixAngleMessage : INetMessage
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

		public void ReadMsg(BitStream stream)
		{
			Relative = stream.ReadBool();

			Angle = new double[3];
			Angle[0] = BitAngle.Read(stream, ANGLE_BITS);
			Angle[1] = BitAngle.Read(stream, ANGLE_BITS);
			Angle[2] = BitAngle.Read(stream, ANGLE_BITS);
		}

		public void ApplyWorldState(WorldState ws)
		{
			throw new NotImplementedException();
		}
	}
}
