namespace DemoLib.NetMessages
{
	interface INetMessage
	{
		void WriteMsg(byte[] buffer, ref ulong bitOffset);
		void ReadMsg(DemoReader reader, byte[] buffer, ref ulong bitOffset);

		ulong Size { get; }

		string Description { get; }
	}
}
