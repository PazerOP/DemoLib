using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitSet;
using TF2Net.Data;
using TF2Net.NetMessages;
using System.Collections.Immutable;
using System.Diagnostics;
using TF2Net.Extensions;

namespace TF2Net.Data
{
	public class WorldState
	{
		WorldEvents m_Listeners;
		public WorldEvents Listeners
		{
			get { return m_Listeners; }
			set
			{
				Debug.Assert(m_Listeners == null);

				m_Listeners = value;
				RegisterEventHandlers();
			}
		}

		public ulong? EndTick { get; set; }
		public ulong BaseTick { get; set; } = 0;
		public ulong Tick { get; set; }

		public double LastFrameTime { get; set; }
		public double LastFrameTimeStdDev { get; set; }

		public SignonState SignonState { get; set; }

		public ServerInfo ServerInfo { get; set; }

		public Entity[] Entities { get; } = new Entity[SourceConstants.MAX_EDICTS];
		public IEnumerable<Entity> EntitiesInPVS
		{
			get
			{
				for (int i = 0; i < SourceConstants.MAX_EDICTS; i++)
				{
					Entity e = Entities[i];
					if (e?.InPVS == true)
						yield return e;
				}
			}
		}

		public IList<StringTable> StringTables { get; } = new List<StringTable>();

		public IDictionary<string, string> ConVars { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		
		public ushort? ViewEntity { get; set; }

		public IList<ServerClass> ServerClasses { get; set; }
		public IList<SendTable> SendTables { get; set; }
		public byte ClassBits { get { return (byte)Math.Ceiling(Math.Log(ServerClasses.Count, 2)); } }

		public IList<GameEventDeclaration> EventDeclarations { get; set; }

		public IEnumerable<KeyValuePair<ServerClass, BitStream>> StaticBaselines
		{
			get
			{
				return StringTables.Single(st => st.TableName == "instancebaseline")
					.Entries.Select(e => new KeyValuePair<ServerClass, BitStream>(ServerClasses[int.Parse(e.Value)], e.UserData));
			}
		}
		public IList<SendProp>[][] InstanceBaselines { get; } = new IList<SendProp>[2][]
		{
			new IList<SendProp>[SourceConstants.MAX_EDICTS],
			new IList<SendProp>[SourceConstants.MAX_EDICTS],
		};

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly List<Player> m_CachedPlayers = new List<Player>();
		public IEnumerable<Player> Players
		{
			get
			{
				if (SignonState?.State != ConnectionState.Full)
					yield break;

				IEnumerable<StringTableEntry> table = StringTables.SingleOrDefault(st => st.TableName == "userinfo");
				if (table == null)
				{
					lock (m_CachedPlayers)
						m_CachedPlayers.Clear();

					yield break;
				}

				List<Player> touched = new List<Player>();
				foreach (var user in table)
				{
					var localData = user.UserData;
					if (localData == null)
						continue;
					Debug.Assert(localData.Cursor == 0);

					UserInfo decoded = new UserInfo(localData);

					Player existing;
					lock (m_CachedPlayers)
						existing = m_CachedPlayers.SingleOrDefault(p => p.Info.GUID == decoded.GUID);

					uint entityIndex = uint.Parse(user.Value) + 1;

					Existing:
					if (existing != null)
					{
						Debug.Assert(entityIndex == existing.EntityIndex);
						existing.Info = decoded;
						touched.Add(existing);
						yield return existing;
					}
					else
					{
						Player newPlayer;
						lock (m_CachedPlayers)
						{
							// Check again
							existing = m_CachedPlayers.SingleOrDefault(p => p.Info.GUID == decoded.GUID);
							if (existing != null)
								goto Existing;
							else
							{
								newPlayer = new Player(decoded, this, entityIndex);
								Listeners.PlayerAdded.Invoke(newPlayer);
								m_CachedPlayers.Add(newPlayer);
								touched.Add(newPlayer);
							}
						}
						
						yield return newPlayer;
					}
				}

				Console.WriteLine(touched);
			}
		}

		void RegisterEventHandlers()
		{
			if (Listeners == null)
				throw new ArgumentNullException(nameof(Listeners));
		}
	}
}
