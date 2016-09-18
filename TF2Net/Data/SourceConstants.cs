namespace TF2Net.Data
{
	public static class SourceConstants
	{
		public const int MAX_OSPATH = 260;

		internal const int NETMSG_TYPE_BITS = 6;

		internal const int EVENT_INDEX_BITS = 8;
		internal const int MAX_EVENT_BITS = 9;

		internal const int NET_MAX_PAYLOAD_BITS = 17;
		internal const int NET_MAX_PAYLOAD = (1 << NET_MAX_PAYLOAD_BITS);

		internal const int MAX_DECAL_INDEX_BITS = 9;
		internal const int MAX_EDICT_BITS = 11;
		internal const int MAX_EDICTS = (1 << MAX_EDICT_BITS);

		internal const int MAX_USER_MSG_LENGTH_BITS = 11;
		internal const int MAX_USER_MSG_LENGTH = (1 << MAX_USER_MSG_LENGTH_BITS);

		internal const int MAX_ENTITY_MSG_LENGTH_BITS = 11;
		internal const int MAX_ENTITY_MSG_LENGTH = (1 << MAX_ENTITY_MSG_LENGTH_BITS);

		internal const int MAX_SERVER_CLASS_BITS = 9;
		internal const int MAX_SERVER_CLASSES = (1 << MAX_SERVER_CLASS_BITS);

		internal const int MAX_SOUND_INDEX_BITS = 14;

		internal const int SP_MODEL_INDEX_BITS = 11;

		internal const int NUM_NETWORKED_EHANDLE_SERIAL_NUMBER_BITS = 10;

		internal const int MAX_DATATABLES = 1024;
		internal const int MAX_DATATABLE_PROPS = 4096;

		internal const int COORD_INTEGER_BITS = 14;
		internal const int COORD_FRACTIONAL_BITS = 5;
		internal const int COORD_DENOMINATOR = 1 << COORD_FRACTIONAL_BITS;
		internal const double COORD_RESOLUTION = 1.0 / COORD_DENOMINATOR;

		internal const int COORD_INTEGER_BITS_MP = 11;
		internal const int COORD_FRACTIONAL_BITS_MP_LOWPRECISION = 3;
		internal const int COORD_DENOMINATOR_LOWPRECISION = 1 << COORD_FRACTIONAL_BITS_MP_LOWPRECISION;
		internal const double COORD_RESOLUTION_LOWPRECISION = 1.0 / COORD_DENOMINATOR_LOWPRECISION;

		internal const int SPROP_NUMFLAGBITS_NETWORKED = 16;
		internal const int SPROP_NUMFLAGBITS = 17;
	}
}
