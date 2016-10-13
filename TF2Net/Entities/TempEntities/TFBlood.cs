using TF2Net.Data;

namespace TF2Net.Entities.TempEntities
{
	public class TFBlood : BaseEntityWrapper
	{
		public IReadOnlyVector Origin { get; }

		public uint? TargetEntityIndex { get; }
		public Entity TargetEntity { get { return TargetEntityIndex.HasValue ? Entity.World.Entities[TargetEntityIndex.Value] : null; } }

		public const string CLASSNAME = "CTETFBlood";
		public TFBlood(IBaseEntity e) : base(e, CLASSNAME)
		{
			{
				Vector origin = new Vector();
				// "DT_TETFBlood.m_vecOrigin[0]"
				origin.X = (double?)e.GetProperty("DT_TETFBlood.m_vecOrigin[0]")?.Value ?? 0;
				origin.Y = (double?)e.GetProperty("DT_TETFBlood.m_vecOrigin[1]")?.Value ?? 0;
				origin.Z = (double?)e.GetProperty("DT_TETFBlood.m_vecOrigin[2]")?.Value ?? 0;
				Origin = origin;
			}

			TargetEntityIndex = (uint?)e.GetProperty("DT_TETFBlood.entindex")?.Value;
		}
	}
}
