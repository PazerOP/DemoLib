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
		
		public IEnumerable<SendProp> AllProperties
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

				foreach (SendProp prop in datatablesFirst)
				{
					if (prop.Type == SendPropType.Datatable)
					{
						foreach (var p in prop.Table.AllProperties)
						{
							SendProp cloned = p.Clone();
							cloned.Name = cloned.Name.Insert(0, NetTableName + '.');
							yield return cloned;
						}
					}
					else
					{
						SendProp cloned = prop.Clone();
						cloned.Name = cloned.Name.Insert(0, NetTableName + '.');
						yield return cloned;
					}
				}

#if false

				var flattened = Flatten(datatablesFirst);
				return flattened;

				var changesOftenFirst = flattened.OrderBy(p => p.Flags, 
					Comparer<SendPropFlags>.Create((f1, f2) =>
					{
						bool hasFlag1 = f1.HasFlag(SendPropFlags.ChangesOften);
						bool hasFlag2 = f2.HasFlag(SendPropFlags.ChangesOften);

						if (hasFlag1 == hasFlag2)
							return 0;

						if (hasFlag1)
							return -1;
						else if (hasFlag2)
							return 1;

						throw new InvalidOperationException();
					}));

				return changesOftenFirst;
#endif
			}
		}

		public IEnumerable<SendProp> SortedProperties
		{
			get
			{
				SendProp[] allProperties = AllProperties.ToArray();

				if (allProperties.Length < 2)
					return allProperties;

				int start = 0;
				for (int i = start + 1; i < allProperties.Length; i++)
				{
					bool startChangesOften = allProperties[start].Flags.HasFlag(SendPropFlags.ChangesOften);
					if (startChangesOften)
					{
						start++;
						continue;
					}

					if (allProperties[i].Flags.HasFlag(SendPropFlags.ChangesOften))
					{
						SendProp temp = allProperties[start];
						allProperties[start] = allProperties[i];
						allProperties[i] = temp;

						start++;
						continue;
					}
				}

				return allProperties;
			}
		}

		IEnumerable<SendProp> Flatten(IEnumerable<SendProp> input)
		{
			foreach (SendProp prop in input)
			{
				if (prop.Type == SendPropType.Datatable)
				{
					foreach (var p in prop.Table.AllProperties)
					{
						SendProp cloned = p.Clone();
						cloned.Name = cloned.Name.Insert(0, NetTableName + '.');
						yield return cloned;
					}
				}
				else
				{
					SendProp cloned = prop.Clone();
					cloned.Name = cloned.Name.Insert(0, NetTableName + '.');
					yield return cloned;
				}
			}
		}

		IEnumerable<SendProp> ExcludeProps
		{
			get
			{
				return Properties.Where(p => p.Flags.HasFlag(SendPropFlags.Exclude));
			}
		}

		IEnumerable<SendProp> ChildDataTables
		{
			get
			{
				return Properties.Where(p => p.Type == SendPropType.Datatable);
			}
		}
	}
}
