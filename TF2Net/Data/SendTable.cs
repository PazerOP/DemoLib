﻿using System.Collections.Generic;
using System.Diagnostics;

namespace TF2Net.Data
{
	[DebuggerDisplay("SendTable {NetTableName}, {Properties.Count} SendProps")]
	public class SendTable
	{
		/// <summary>
		/// The name matched between client and server.
		/// </summary>
		public string NetTableName { get; set; }

		public IList<SendProp> Properties { get; set; } = new List<SendProp>();
	}
}