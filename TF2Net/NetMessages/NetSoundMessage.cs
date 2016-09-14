using System;
using System.Diagnostics;
using BitSet;

namespace TF2Net.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	public class NetSoundMessage : INetMessage
	{
		const int SOUND_COUNT_BITS = 8;
		const int RELIABLE_SIZE_BITS = 8;
		const int UNRELIABLE_SIZE_BITS = 16;

		public bool Reliable { get; set; }
		public int SoundCount { get; set; }

		public BitStream Data { get; set; }

		public string Description
		{
			get
			{
				return string.Format("svc_Sounds: number {0},{1} bytes {2}",
					SoundCount, Reliable ? " reliable," : "", BitInfo.BitsToBytes(Data.Length));
			}
		}

		public void ReadMsg(BitStream stream)
		{
			Reliable = stream.ReadBool();

			ulong bitCount;
			if (Reliable)
			{
				SoundCount = 1;
				bitCount = stream.ReadULong(RELIABLE_SIZE_BITS);
			}
			else
			{
				SoundCount = stream.ReadInt(SOUND_COUNT_BITS);
				bitCount = stream.ReadULong(UNRELIABLE_SIZE_BITS);
			}

			Data = stream.Subsection(stream.Cursor, stream.Cursor + bitCount);
			stream.Seek(bitCount, System.IO.SeekOrigin.Current);
		}

		public void ApplyWorldState(WorldState ws)
		{
			//throw new NotImplementedException();
		}
	}
}
