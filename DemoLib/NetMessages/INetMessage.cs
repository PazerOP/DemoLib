namespace DemoLib.NetMessages
{
	public interface INetMessage
	{
		void WriteMsg(byte[] buffer, ref ulong bitOffset);
		void ReadMsg(DemoReader reader, uint? serverTick, byte[] buffer, ref ulong bitOffset);

		ulong Size { get; }

		string Description { get; }
	}
}
