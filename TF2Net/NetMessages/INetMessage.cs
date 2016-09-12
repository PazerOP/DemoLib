using BitSet;

namespace TF2Net.NetMessages
{
	public interface INetMessage
	{
		string Description { get; }

		void ReadMsg(BitStream stream, IReadOnlyWorldState ws);
		void ApplyWorldState(WorldState ws);
	}
}
