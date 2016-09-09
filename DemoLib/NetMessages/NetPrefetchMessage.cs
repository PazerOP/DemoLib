using System;
using System.Diagnostics;
using BitSet;

namespace DemoLib.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	class NetPrefetchMessage : INetMessage
	{
		public enum PrefetchType
		{
			Sound = 0,
		}

		public PrefetchType Type { get; set; }

		public int SoundIndex { get; set; }

		public string Description
		{
			get
			{
				return string.Format("svc_Prefetch: type {0} index {1}", Type, SoundIndex);
			}
		}

		public ulong Size
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public void ReadMsg(DemoReader reader, byte[] buffer, ref ulong bitOffset)
		{
			Type = PrefetchType.Sound;
			SoundIndex = (int)BitReader.ReadUIntBits(buffer, ref bitOffset, SourceConstants.MAX_SOUND_INDEX_BITS);
		}

		public void WriteMsg(byte[] buffer, ref ulong bitOffset)
		{
			throw new NotImplementedException();
		}
	}
}
