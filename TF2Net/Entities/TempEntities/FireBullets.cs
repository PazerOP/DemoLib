using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TF2Net.Data;

namespace TF2Net.Entities.TempEntities
{
	public class FireBullets : BaseEntityWrapper
	{
		public Player Player { get; }
		public WeaponType Weapon { get; }
		public IReadOnlyVector Origin { get; }

		public const string CLASSNAME = "CTEFireBullets";
		public FireBullets(IBaseEntity e) : base(e, CLASSNAME)
		{
			Origin = (Vector)e.GetProperty("DT_TEFireBullets.m_vecOrigin")?.Value ?? new Vector();

			{
				uint playerIndex = (uint)e.GetProperty("DT_TEFireBullets.m_iPlayer").Value;
				Player = e.World.Players.ElementAt((int)playerIndex);
			}

			Weapon = (WeaponType)(uint)e.GetProperty("DT_TEFireBullets.m_iWeaponID").Value;
		}
	}
}
