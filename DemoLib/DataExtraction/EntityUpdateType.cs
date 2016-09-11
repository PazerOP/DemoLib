namespace DemoLib.DataExtraction
{
	public enum EntityUpdateType
	{
		EnterPVS = 0,   // Entity came back into pvs, create new entity if one doesn't exist

		LeavePVS,       // Entity left pvs

		DeltaEnt,       // There is a delta for this entity.
		PreserveEnt,    // Entity stays alive but no delta ( could be LOD, or just unchanged )

		Finished,       // finished parsing entities successfully
		Failed,         // parsing error occured while reading entities
	};
}
