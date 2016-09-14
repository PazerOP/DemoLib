using BitSet;

namespace TF2Net.NetMessages
{
	public interface INetMessage
	{
		string Description { get; }

		void ReadMsg(BitStream stream);
		void ApplyWorldState(WorldState ws);
	}
}
