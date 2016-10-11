using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TF2Net.Data
{
	public enum UserMessageType
	{
		Geiger = 0,
		Train = 1,
		HudText = 2,
		SayText = 3,
		SayText2 = 4,
		TextMsg = 5,
		ResetHUD = 6,
		GameTitle = 7,
		ItemPickup = 8,
		ShowMenu = 9,
		Shake = 10,

		HudNotifyCustom = 27,

		BreakModel = 41,
		CheapBreakModel = 42,

		MVMResetPlayerStats = 57,
	}
}
