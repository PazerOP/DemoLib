using TF2Net.Data;

namespace TF2Net.Entities.TempEntities
{
	public class TFExplosion : BaseEntityWrapper
	{
		public IReadOnlyVector Origin { get; }
		public IReadOnlyVector Normal { get; }

		public const string CLASSNAME = "CTETFExplosion";
		public TFExplosion(IBaseEntity e) : base(e, CLASSNAME)
		{
			{
				Vector origin = new Vector();
				origin.X = (double?)e.GetProperty("DT_TETFExplosion.m_vecOrigin[0]")?.Value ?? 0;
				origin.Y = (double?)e.GetProperty("DT_TETFExplosion.m_vecOrigin[1]")?.Value ?? 0;
				origin.Z = (double?)e.GetProperty("DT_TETFExplosion.m_vecOrigin[2]")?.Value ?? 0;
				Origin = origin;
			}

			Normal = (Vector)e.GetProperty("DT_TETFExplosion.m_vecNormal").Value;
		}
	}
}
