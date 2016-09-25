using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

		public bool Unknown1 { get; set; }
		
		IEnumerable<FlattenedProp> AllProperties
		{
			get
			{
				var datatablesFirst = Properties.OrderByDescending(p => p.Type,
					Comparer<SendPropType>.Create((p1, p2) =>
					{
						bool isDT1 = p1 == SendPropType.Datatable;
						bool isDT2 = p2 == SendPropType.Datatable;

						if (isDT1 == isDT2)
							return 0;

						if (isDT1)
							return 1;
						else if (isDT2)
							return -1;

						throw new InvalidOperationException();
					}));
				
				return Flatten(Excludes);
			}
		}
		
		IEnumerable<SendProp> Excludes
		{
			get
			{
				foreach (SendProp prop in Properties)
				{
					if (prop.Type == SendPropType.Datatable)
					{
						foreach (SendProp childProp in prop.Table.Properties)
						{
							if (childProp.Flags.HasFlag(SendPropFlags.Exclude))
								yield return childProp;
						}
					}
					else
					{
						if (prop.Flags.HasFlag(SendPropFlags.Exclude))
							yield return prop;
					}
				}
			}
		}

		IEnumerable<FlattenedProp> Flatten(IEnumerable<SendProp> excludes)
		{
			foreach (SendProp prop in Properties)
			{
				if (excludes.Any(e => e.Name == prop.Name && e.ExcludeName == prop.Parent.NetTableName))
					continue;

				if (prop.Type == SendPropType.Datatable)
				{
					foreach (FlattenedProp childProp in prop.Table.Flatten(excludes))
						yield return childProp;
				}
				else
				{
					FlattenedProp flatProp = new FlattenedProp();
					flatProp.Property = prop;
					flatProp.FullName = flatProp.Property.Name.Insert(0, NetTableName + '.');
					yield return flatProp;
				}
			}
		}

		public IEnumerable<FlattenedProp> SortedProperties
		{
			get
			{
				var allProperties = AllProperties.ToArray();

				if (allProperties.Length < 2)
					return allProperties;

				int start = 0;
				for (int i = start + 1; i < allProperties.Length; i++)
				{
					bool startChangesOften = allProperties[start].Property.Flags.HasFlag(SendPropFlags.ChangesOften);
					if (startChangesOften)
					{
						start++;
						continue;
					}

					if (allProperties[i].Property.Flags.HasFlag(SendPropFlags.ChangesOften))
					{
						var temp = allProperties[start];
						allProperties[start] = allProperties[i];
						allProperties[i] = temp;

						start++;
						continue;
					}
				}

				return allProperties;
			}
		}
	}
}
