using BitSet;
using TF2Net.Data;
using TF2Net.NetMessages;

namespace TF2Net.Extensions
{
    public static class BitStreamExtensions
    {
        public static Vector ReadVector(this BitStream stream)
        {
            bool flagX = stream.ReadBool();
            bool flagY = stream.ReadBool();
            bool flagZ = stream.ReadBool();

            Vector vector = new Vector();
            if (flagX)
                vector.X = BitCoord.Read(stream);
            if (flagY)
                vector.Y = BitCoord.Read(stream);
            if (flagZ)
                vector.Z = BitCoord.Read(stream);
            return vector;
        }
    }
}