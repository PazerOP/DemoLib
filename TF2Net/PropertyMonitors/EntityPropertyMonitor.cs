using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TF2Net.Data;

namespace TF2Net.PropertyMonitors
{
	internal class EntityPropertyMonitor<T> : IEntityPropertyMonitor<T>
	{
		public Entity Entity { get; }
		public string PropertyName { get; }

		Func<object, T> Decoder { get; }

		bool m_ValueChanged;
		public T Value { get; private set; }
		object IPropertyMonitor.Value { get { return Value; } }

		SingleEvent<Action<IPropertyMonitor>> IPropertyMonitor.ValueChanged { get; } = new SingleEvent<Action<IPropertyMonitor>>();
		SingleEvent<Action<IPropertyMonitor<T>>> IPropertyMonitor<T>.ValueChanged { get; } = new SingleEvent<Action<IPropertyMonitor<T>>>();
		SingleEvent<Action<IEntityPropertyMonitor>> IEntityPropertyMonitor.ValueChanged { get; } = new SingleEvent<Action<IEntityPropertyMonitor>>();
		public SingleEvent<Action<IEntityPropertyMonitor<T>>> ValueChanged { get; } = new SingleEvent<Action<IEntityPropertyMonitor<T>>>();

		public EntityPropertyMonitor(string propertyName, Entity e, Func<object, T> decoder)
		{
			ValueChanged.Add((self) => ((IPropertyMonitor)self).ValueChanged.Invoke(self));
			ValueChanged.Add((self) => ((IPropertyMonitor<T>)self).ValueChanged.Invoke(self));
			ValueChanged.Add((self) => ((IEntityPropertyMonitor)self).ValueChanged.Invoke(self));

			PropertyName = propertyName;
			Entity = e;
			Decoder = decoder;

			Entity.EnteredPVS.Add(Entity_EnteredPVS);
			Entity.LeftPVS.Add(Entity_LeftPVS);
			Entity.PropertiesUpdated.Add(Entity_PropertiesUpdated);

			if (Entity.InPVS)
				Entity_EnteredPVS(Entity);
		}

		private void Entity_PropertiesUpdated(Entity e)
		{
			Debug.Assert(Entity == e);
			if (m_ValueChanged)
			{
				ValueChanged.Invoke(this);
				m_ValueChanged = false;
			}
		}

		private void Entity_LeftPVS(Entity e)
		{
			Debug.Assert(Entity == e);
			e.PropertyAdded.Remove(Entity_PropertyAdded);
		}

		private void Entity_EnteredPVS(Entity e)
		{
			Debug.Assert(Entity == e);
			e.PropertyAdded.Add(Entity_PropertyAdded);

			foreach (SendProp prop in e.Properties)
				Entity_PropertyAdded(prop);
		}

		private void Entity_PropertyAdded(SendProp prop)
		{
			if (prop.Definition.FullName == PropertyName)
				prop.ValueChanged += Prop_ValueChanged;
		}

		private void Prop_ValueChanged(SendProp prop)
		{
			Value = Decoder(prop.Value);
			m_ValueChanged = true;
		}
	}
}
