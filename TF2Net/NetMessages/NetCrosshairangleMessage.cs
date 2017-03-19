using System;
using BitSet;
using TF2Net.Data;

namespace TF2Net.NetMessages
{
    internal class NetCrosshairangleMessage : INetMessage
    {
        public string Description => string.Format("svc_Crosshairangle {0} {1} {2}", X, Y, Z);

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public void ReadMsg(BitStream stream)
        {
            X = BitAngle.Read(stream, 16);
            Y = BitAngle.Read(stream, 16);
            Z = BitAngle.Read(stream, 16);
        }

        public void ApplyWorldState(WorldState ws)
        {
            throw new NotImplementedException();
        }
    }
}
