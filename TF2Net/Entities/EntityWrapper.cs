using System;

namespace TF2Net.Entities
{
	public abstract class BaseEntityWrapper
	{
		public IBaseEntity Entity { get; }

		public BaseEntityWrapper(IBaseEntity e, string className)
		{
			if (e.Class.Classname != className)
				throw new ArgumentException(string.Format("Invalid entity class for this {0}", nameof(BaseEntityWrapper)));

			Entity = e;
		}
	}

	public abstract class AbstractEntityWrapper : BaseEntityWrapper
	{
		public new IEntity Entity { get { return (IEntity)base.Entity; } }

		public AbstractEntityWrapper(IEntity e, string className) : base(e, className) { }
	}

	public abstract class EntityWrapper : AbstractEntityWrapper
	{
		public new Entity Entity { get { return (Entity)base.Entity; } }

		public EntityWrapper(Entity e, string className) : base(e, className) { }
	}
}
