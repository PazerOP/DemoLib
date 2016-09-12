using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TF2Net.Data
{
	public enum GameEventDataType
	{
		Local = 0, // not networked
		String,    // zero terminated ASCII string
		Float,     // float 32 bit
		Long,      // signed int 32 bit
		Short,     // signed int 16 bit
		Byte,      // unsigned int 8 bit
		Bool,      // unsigned int 1 bit
	};

	[DebuggerDisplay("Game event declaration: {Name}")]
	public class GameEventDeclaration : IReadOnlyGameEventDeclaration
	{
		public int ID { get; set; }
		public string Name { get; set; }

		public IDictionary<string, GameEventDataType> Values { get; set; }
		IReadOnlyDictionary<string, GameEventDataType> IReadOnlyGameEventDeclaration.Values { get { return Values.AsReadOnly(); } }
	}

	public interface IReadOnlyGameEventDeclaration
	{
		int ID { get; }
		string Name { get; }

		IReadOnlyDictionary<string, GameEventDataType> Values { get; }
	}
}
