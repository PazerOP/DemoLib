using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TF2Net.Data
{
	public enum PlayerState
	{
		/// <summary>
		/// Happily running around in the game.
		/// </summary>
		Active = 0,

		/// <summary>
		/// First entering the server (shows level intro screen).
		/// </summary>
		Welcome = 1,

		/// <summary>
		/// Game observer mode.
		/// </summary>
		Observer = 2,

		/// <summary>
		/// Player is dying.
		/// </summary>
		Dying = 3,
	}
}
