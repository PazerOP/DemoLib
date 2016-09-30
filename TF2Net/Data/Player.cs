using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitSet;

namespace TF2Net.Data
{
	[DebuggerDisplay("{ToString(),nq}")]
	public class Player
	{
		public WorldState World { get; }
		public uint EntityIndex { get; }

		public Entity Entity { get { return World.Entities[EntityIndex]; } }

		public string Name { get; set; }
		public int? UserID { get; set; }

		public string GUID { get; set; }

		public uint? FriendsID { get; set; }

		public string FriendsName { get; set; }

		public bool IsFakePlayer { get; set; }
		public bool IsHLTV { get; set; }

		public uint?[] CustomFiles { get; } = new uint?[4];

		public uint FilesDownloaded { get; set; }

		public bool InPVS { get { return Entity != null && Entity.InPVS; } }

		public Vector Position
		{
			get
			{
				var originXYProps = Entity.Properties
					.Where(p => p.Definition.Name == "m_vecOrigin")
					.OrderByDescending(p => p.LastChangedTick)
					.Select(p => p.Value)
					.Cast<Vector>();

				var originZProps = Entity.Properties
					.Where(p => p.Definition.Name == "m_vecOrigin[2]")
					.OrderByDescending(p => p.LastChangedTick)
					.Select(p => p.Value)
					.Cast<double>();

				var vec = originXYProps.FirstOrDefault()?.Clone();
				if (vec == null)
					return null;

				if (originZProps.Any())
					vec.Z = originZProps.First();

				return vec;
			}
		}

		public Team? Team
		{
			get
			{
				var prop = Entity.Properties.SingleOrDefault(p => p.Definition.Parent.NetTableName == "DT_BaseEntity" && p.Definition.Name == "m_iTeamNum");
				if (prop == null)
					return null;

				return (Team)(int)prop.Value;
			}
		}

		public event Action<WorldState, Player> EnteredPVS;
		public event Action<WorldState, Player> PropertiesUpdated;
		public event Action<WorldState, Player> LeftPVS;

		public Player(BitStream stream, WorldState ws, uint entityIndex)
		{
			EntityIndex = entityIndex;
			World = ws;

			Name = Encoding.ASCII.GetString(stream.ReadBytes(32)).TrimEnd('\0');

			UserID = stream.ReadInt();

			GUID = Encoding.ASCII.GetString(stream.ReadBytes(33)).TrimEnd('\0');

			FriendsID = stream.ReadUInt();

			FriendsName = Encoding.ASCII.GetString(stream.ReadBytes(32)).TrimEnd('\0');

			IsFakePlayer = stream.ReadByte() > 0 ? true : false;
			IsHLTV = stream.ReadByte() > 0 ? true : false;

			for (byte i = 0; i < 4; i++)
				CustomFiles[i] = stream.ReadUInt();

			FilesDownloaded = stream.ReadByte();
			
			World.Listeners.EntityEnteredPVS += Listeners_EntityEnteredPVS;
			if (InPVS)
				Entity.PropertiesUpdated += Entity_PropertiesUpdated;

			World.Listeners.EntityLeftPVS += Listeners_EntityLeftPVS;
		}

		private void Listeners_EntityLeftPVS(WorldState ws, Entity e)
		{
			Debug.Assert(ws == World);
			Debug.Assert(e != null);
			if (e != Entity)
				return;

			LeftPVS?.Invoke(ws, this);
			Entity.PropertiesUpdated -= Entity_PropertiesUpdated;
		}

		private void Listeners_EntityEnteredPVS(WorldState ws, Entity e)
		{
			Debug.Assert(ws == World);
			Debug.Assert(e != null);
			if (e != Entity)
				return;

			EnteredPVS?.Invoke(ws, this);

			Entity.PropertiesUpdated -= Entity_PropertiesUpdated;
			Entity.PropertiesUpdated += Entity_PropertiesUpdated;
		}

		private void Entity_PropertiesUpdated(WorldState ws, Entity e)
		{
			Debug.Assert(ws == World);
			Debug.Assert(e == Entity);

			PropertiesUpdated?.Invoke(ws, this);
		}

		public override string ToString()
		{
			return string.Format("\"{0}\": {1}", Name, GUID);
		}
	}
}
