using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace TF2Net.Data
{
	[DebuggerDisplay("SendTable {NetTableName}, {Properties.Count} SendProps")]
	public class SendTable
	{
		public SendTable()
		{
			m_FlattenedProps = new Lazy<ImmutableArray<SendPropDefinition>>(
				() => ImmutableArray.Create(SetupFlatPropertyArray().ToArray()));
		}

		/// <summary>
		/// The name matched between client and server.
		/// </summary>
		public string NetTableName { get; set; }

		public IList<SendPropDefinition> Properties { get; set; } = new List<SendPropDefinition>();

		private Lazy<ImmutableArray<SendPropDefinition>> m_FlattenedProps;
		public ImmutableArray<SendPropDefinition> FlattenedProps { get { return m_FlattenedProps.Value; } }

		public bool Unknown1 { get; set; }
		
		IEnumerable<SendPropDefinition> Excludes
		{
			get
			{
				foreach (SendPropDefinition prop in Properties)
				{
					if (prop.Flags.HasFlag(SendPropFlags.Exclude))
						yield return prop;
					else if (prop.Type == SendPropType.Datatable)
					{
						foreach (SendPropDefinition childExclude in prop.Table.Excludes)
							yield return childExclude;
					}
				}
			}
		}

		IEnumerable<FlattenedProp> Flatten(IEnumerable<SendPropDefinition> excludes)
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

			foreach (SendPropDefinition prop in datatablesFirst)
			{
				if (excludes.Any(e => e.Name == prop.Name && e.ExcludeName == prop.Parent.NetTableName))
					continue;

				if (excludes.Contains(prop))
					continue;

				Debug.Assert(!prop.Flags.HasFlag(SendPropFlags.Exclude));

				if (prop.Type == SendPropType.Datatable)
				{
					foreach (FlattenedProp childProp in prop.Table.Flatten(excludes))
					{
						childProp.FullName = childProp.FullName.Insert(0, NetTableName + '.');
						yield return childProp;
					}
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

		List<SendPropDefinition> SetupFlatPropertyArray()
		{
			var excludes = Excludes;
			
			List<SendPropDefinition> props = new List<SendPropDefinition>();

			SendTable_BuildHierarchy(excludes, props);

			SendTable_SortByPriority(props);

			return props;
		}

		void SendTable_BuildHierarchy(IEnumerable<SendPropDefinition> excludes, List<SendPropDefinition> allProperties)
		{
			List<SendPropDefinition> localProperties = new List<SendPropDefinition>();

			SendTable_BuildHierarchy_IterateProps(excludes, localProperties, allProperties);

			allProperties.AddRange(localProperties);
		}

		IEnumerable<SendPropDefinition> TestSortedProps
		{
			get { return SetupFlatPropertyArray(); }
		}

		void SendTable_SortByPriority(List<SendPropDefinition> props)
		{
			int start = 0;
			for (int i = start; i < props.Count; i++)
			{
				if (props[i].Flags.HasFlag(SendPropFlags.ChangesOften))
				{
					if (i != start)
					{
						var temp = props[i];
						props[i] = props[start];
						props[start] = temp;
					}

					start++;
					continue;
				}
			}
		}

		void SendTable_BuildHierarchy_IterateProps(IEnumerable<SendPropDefinition> excludes, List<SendPropDefinition> localProperties, List<SendPropDefinition> childDTProperties)
		{
			foreach (var prop in Properties)
			{
				if (prop.Flags.HasFlag(SendPropFlags.Exclude) || excludes.Contains(prop))
				{
					continue;
				}
				
				if (excludes.Any(e => e.Name == prop.Name && e.ExcludeName == prop.Parent.NetTableName))
					continue;

				if (prop.Type == SendPropType.Datatable)
				{
					if (prop.Flags.HasFlag(SendPropFlags.Collapsible))
						prop.Table.SendTable_BuildHierarchy_IterateProps(excludes, localProperties, childDTProperties);
					else
					{
						prop.Table.SendTable_BuildHierarchy(excludes, childDTProperties);
					}
				}
				else
				{
					localProperties.Add(prop);
				}
			}
		}
	}
}
