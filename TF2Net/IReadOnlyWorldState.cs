using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TF2Net.Data;

namespace TF2Net
{
	public interface IReadOnlyWorldState
	{
		IReadOnlyList<IReadOnlyGameEventDeclaration> EventDeclarations { get; }
	}
}
