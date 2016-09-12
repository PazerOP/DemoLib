using System;
using System.Diagnostics;
using BitSet;
using TF2Net.Data;

namespace TF2Net.NetMessages
{
	[DebuggerDisplay("{Description, nq}")]
	public class NetPrefetchMessage : INetMessage
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

		public void ReadMsg(BitStream stream, IReadOnlyWorldState ws)
		{
			Type = PrefetchType.Sound;
			SoundIndex = stream.ReadInt(SourceConstants.MAX_SOUND_INDEX_BITS);
		}

		public void ApplyWorldState(WorldState ws)
		{
			throw new NotImplementedException();
		}
	}
}
