using System;
using System.Diagnostics;
using BitSet;
using TF2Net.Data;
using TF2Net.Extensions;

namespace TF2Net.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	public class NetBspDecalMessage : INetMessage
    {
        const byte MaxDecalIndexBits = 9;
        const byte MaxEdictBits = 11;
        const byte SpModelIndexBits = 11;

        public string Description => string.Format("svc_bspdecal: {0} {1} {2}", Position, DecalTextureIndex, EntIndex);
        public Vector Position { get; set; }
        public ulong DecalTextureIndex { get; set; }
        public ulong EntIndex { get; set; }
        public ulong ModelIndex { get; set; }
        public bool LowPrioritiy { get; set; }


        public void ReadMsg(BitStream stream)
        {
            Position = stream.ReadVector();
            DecalTextureIndex = stream.ReadULong(MaxDecalIndexBits);

            bool b = stream.ReadBool();
            if (b)
            {
                EntIndex = stream.ReadULong(MaxEdictBits);
                ModelIndex = stream.ReadULong(SpModelIndexBits);
            }
            else
            {
                EntIndex = 0;
                ModelIndex = 0;
            }
            LowPrioritiy = stream.ReadBool();
        }


        public void ApplyWorldState(WorldState ws)
        {
            throw new NotImplementedException();
        }
    }
}
