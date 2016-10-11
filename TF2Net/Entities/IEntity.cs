using TF2Net.Data;

namespace TF2Net.Entities
{
	public interface IBaseEntity : IStaticPropertySet
	{
		WorldState World { get; }
		ServerClass Class { get; }
		SendTable NetworkTable { get; }
	}
	public interface IEntity : IBaseEntity, IPropertySet { }
}
