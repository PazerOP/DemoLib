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

namespace TF2Net
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

		public ulong Tick { get; set; }

		public double LastFrameTime { get; set; }
		public double LastFrameTimeStdDev { get; set; }

		public SignonState SignonState { get; set; }

		public ServerInfo ServerInfo { get; set; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly List<Player> m_Players = new List<Player>();
		public IReadOnlyList<Player> Players { get { return m_Players; } }

		public Entity[] Entities { get; } = new Entity[SourceConstants.MAX_EDICTS];

		public IList<StringTable> StringTables { get; } = new List<StringTable>();

		public IDictionary<string, string> ConVars { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		public byte ClassBits { get { return (byte)Math.Ceiling(Math.Log(ServerClasses.Count, 2)); } }

		public IList<ServerClass> ServerClasses { get; set; }
		public IList<SendTable> SendTables { get; set; }

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

		public ushort? ViewEntity { get; set; }

		private void Listeners_StringTableCreated(StringTable st)
		{
			if (st.TableName == "userinfo")
				st.StringTableUpdated.Add(UserInfoStringTableUpdated);
		}

		private void UserInfoStringTableUpdated(StringTable st)
		{
			List<Player> all = new List<Player>();
			foreach (var user in st.Entries)
			{
				var localCopy = user.UserData?.Clone();
				if (localCopy == null)
					continue;

				localCopy.Cursor = 0;
				UserInfo decoded = new UserInfo(localCopy);

				uint entityIndex = uint.Parse(user.Value) + 1;

				Player existing = m_Players.SingleOrDefault(p => p.Info.GUID == decoded.GUID);
				if (existing != null)
				{
					Debug.Assert(entityIndex == existing.EntityIndex);
					existing.Info = decoded;
					all.Add(existing);
				}
				else
				{
					Player newPlayer = new Player(decoded, this, entityIndex);
					m_Players.Add(newPlayer);
					Listeners.PlayerAdded.Invoke(newPlayer);
					all.Add(newPlayer);
				}
			}

			var toRemove = m_Players.Where(p => !all.Any(p2 => p.Info.GUID == p2.Info.GUID)).ToArray();
			foreach (Player removed in toRemove)
			{
				Listeners.PlayerRemoved.Invoke(removed);

				if (!m_Players.Remove(removed))
					throw new InvalidOperationException();
			}
		}

		void RegisterEventHandlers()
		{
			if (Listeners == null)
				return;

			Listeners.StringTableCreated.Add(Listeners_StringTableCreated);
		}
	}
}
