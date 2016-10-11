using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TF2Net.Data;

namespace TF2Net.Entities
{
	public interface IStaticPropertySet
	{
		IReadOnlyList<SendProp> Properties { get; }
	}
	public interface IPropertySet : IStaticPropertySet
	{
		SingleEvent<Action<SendProp>> PropertyAdded { get; }
		SingleEvent<Action<IPropertySet>> PropertiesUpdated { get; }

		void AddProperty(SendProp prop);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class IPropertySetExtensions
	{
		public static SendProp GetProperty(this IStaticPropertySet set, SendPropDefinition def)
		{
			var retVal = set.Properties.SingleOrDefault(x => x.Definition == def);
			Debug.Assert(retVal == null || ReferenceEquals(retVal.Definition, def));
			Debug.Assert(retVal == null || ReferenceEquals(retVal.Entity, set));
			return retVal;
		}
		public static SendProp GetProperty(this IStaticPropertySet set, string propName)
		{
			var retVal = set.Properties.SingleOrDefault(x => x.Definition.FullName == propName);
			return retVal;
		}
	}
}
