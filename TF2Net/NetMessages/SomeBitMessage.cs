using System;
using System.Linq;
using BitSet;
using TF2Net.Data;

namespace TF2Net.NetMessages
{
    internal class SomeBitMessage : INetMessage
    {
        private readonly int _bitCount;

        public SomeBitMessage(int bitCount)
        {
            _bitCount = bitCount;
        }

        public string Description => _bitCount + " bit message";

        public void ReadMsg(BitStream stream)
        {
            foreach (int i in Enumerable.Range(1, _bitCount))
            {
                stream.ReadBool();
            }
        }

        public void ApplyWorldState(WorldState ws)
        {
            throw new NotImplementedException();
        }
    }
}
