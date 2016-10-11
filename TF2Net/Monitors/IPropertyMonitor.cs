using System;
using TF2Net.Data;

namespace TF2Net.Monitors
{
	public interface IPlayerPropertyMonitor<T> : IEntityPropertyMonitor<T>, IPlayerPropertyMonitor
	{
		new SingleEvent<Action<IPlayerPropertyMonitor<T>>> ValueChanged { get; }
	}
	public interface IPlayerPropertyMonitor : IEntityPropertyMonitor
	{
		Player Player { get; }
		new SingleEvent<Action<IPlayerPropertyMonitor>> ValueChanged { get; }
	}

	public interface IEntityPropertyMonitor<T> : IPropertyMonitor<T>, IEntityPropertyMonitor
	{
		new SingleEvent<Action<IEntityPropertyMonitor<T>>> ValueChanged { get; }
	}
	public interface IEntityPropertyMonitor : IPropertyMonitor
	{
		Entity Entity { get; }
		new SingleEvent<Action<IEntityPropertyMonitor>> ValueChanged { get; }
	}

	public interface IPropertyMonitor<T> : IPropertyMonitor
	{
		new T Value { get; }
		new SingleEvent<Action<IPropertyMonitor<T>>> ValueChanged { get; }
	}
	public interface IPropertyMonitor
	{
		object Value { get; }
		SendProp Property { get; }
		string PropertyName { get; }
		SingleEvent<Action<IPropertyMonitor>> ValueChanged { get; }
	}
}
