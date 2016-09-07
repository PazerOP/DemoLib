namespace DemoLib.NetMessages
{
	interface INetMessage
	{
		void WriteMsg(byte[] buffer, ref ulong bitOffset);
		void ReadMsg(byte[] buffer, ref ulong bitOffset);

		ulong Size { get; }

		string Description { get; }
	}
}
