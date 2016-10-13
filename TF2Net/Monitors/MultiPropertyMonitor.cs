using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TF2Net.Data;

namespace TF2Net.Monitors
{
	[DebuggerDisplay("{Value}")]
	class MultiPropertyMonitor<T> : IPropertyMonitor<T>
	{
		public string PropertyName { get { return string.Join("\n", PropertyMonitors.Select(pm => pm.PropertyName)); } }

		public T Value { get; private set; }
		object IPropertyMonitor.Value { get { return Value; } }
		public SendProp Property { get; private set; }

		SingleEvent<Action<IPropertyMonitor>> IPropertyMonitor.ValueChanged { get; } = new SingleEvent<Action<IPropertyMonitor>>();
		public SingleEvent<Action<IPropertyMonitor<T>>> ValueChanged { get; } = new SingleEvent<Action<IPropertyMonitor<T>>>();

		IEnumerable<IPropertyMonitor<T>> PropertyMonitors { get; }
		
		public MultiPropertyMonitor(IEnumerable<IPropertyMonitor<T>> propertyMonitors)
		{
			ValueChanged.Add(self => ((IPropertyMonitor)self).ValueChanged.Invoke(self));

			PropertyMonitors = propertyMonitors;

			foreach (var prop in PropertyMonitors)
				prop.ValueChanged.Add(PropValueChanged);
		}

		void PropValueChanged(IPropertyMonitor<T> propMonitor)
		{
			T newValue = propMonitor.Value;
			if (!Value.Equals(newValue))
			{
				Value = newValue;
				Property = propMonitor.Property;

				if (!(Property == null || propMonitor.Property == null || propMonitor.Property.LastChangedTick >= Property.LastChangedTick))
					Debugger.Break();

				ValueChanged.Invoke(this);
			}
		}
	}
}
