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
				m_Listeners = value;
				RegisterEventHandlers();
			}
		}

		public ulong Tick { get; set; }

		public double LastFrameTime { get; set; }
		public double LastFrameTimeStdDev { get; set; }

		public SignonState SignonState { get; set; }

		public ServerInfo ServerInfo { get; set; }

		private IEnumerable<KeyValuePair<uint, Entity>> NonNullEntities
		{
			get { return Entities
					.Select((e, i) => new KeyValuePair<uint, Entity>((uint)i, e))
					.Where(kv => kv.Value != null); }
		}

		public IEnumerable<Player> Players
		{
			get
			{
				var userinfoTable = StringTables.Single(st => st.TableName == "userinfo");

				foreach (var user in userinfoTable.Entries)
				{
					var localCopy = user.UserData?.Clone();
					if (localCopy == null)
						continue;

					localCopy.Cursor = 0;
					yield return new Player(localCopy, this, uint.Parse(user.Value) + 1);
				}
			}
		}

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

		void RegisterEventHandlers()
		{
			if (Listeners == null)
				return;
		}
	}
}
