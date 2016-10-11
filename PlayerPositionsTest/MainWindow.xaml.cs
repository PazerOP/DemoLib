using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using DemoLib;
using TF2Net;
using TF2Net.Data;
using TF2Net.Entities;

namespace PlayerPositionsTest
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		//readonly SolidColorBrush RedBrush = new SolidColorBrush(Color.FromArgb(255, 189, 59, 59));
		//readonly SolidColorBrush BluBrush = new SolidColorBrush(Color.FromArgb(255, 91, 122, 140));

		static readonly IReadOnlyDictionary<Team, IReadOnlyDictionary<Class, string>> ClassIcons;

		static MainWindow()
		{
			Dictionary<Team, IReadOnlyDictionary<Class, string>> classIcons = new Dictionary<Team, IReadOnlyDictionary<Class, string>>();

			Func<Team, IReadOnlyDictionary<Class, string>> GenerateTeamDict =
				(Team t) =>
				{
					Dictionary<Class, string> retVal = new Dictionary<Class, string>();
					for (int i = 1; i <= 9; i++)
					{
						Class c = (Class)i;
						retVal.Add(c, GetClassIconUri(t, c));
					}
					return retVal;
				};

			classIcons.Add(Team.Red, GenerateTeamDict(Team.Red));
			classIcons.Add(Team.Blue, GenerateTeamDict(Team.Blue));
			ClassIcons = classIcons;
		}

		public PlayerStatusesList RedStatuses { get; } = new PlayerStatusesList();
		public PlayerStatusesList BlueStatuses { get; } = new PlayerStatusesList();

		// Using a DependencyProperty as the backing store for Statuses.  This enables animation, styling, binding, etc...
		//public static readonly DependencyProperty StatusesProperty =
		//	DependencyProperty.Register("Statuses", typeof(PlayerStatusesList), typeof(MainWindow), new PropertyMetadata(new PlayerStatusesList()));

		Task<DemoReader> m_Reader;
		public MainWindow()
		{
			InitializeComponent();

			m_Reader = Task.Run(() =>
			{
				var stream = File.Open("demos/cp_process_final.dem", FileMode.Open, FileAccess.Read);
				return DemoReader.FromStream(stream);
			});

			m_Reader.ContinueWith(lastTask =>
			{
				lastTask.Result.Events.NewTick.Add(Events_NewTick);
				lastTask.Result.Events.NewTick.Add(UpdatePlayerStatuses);
				lastTask.Result.Events.NewTick.Add(UpdatePlayerPositions);

				lastTask.Result.Events.NewTick.Add(UpdateRocketPositions);

				lastTask.Result.Events.GameEvent.Add(GameEventTriggered);

				progress.Dispatcher.Invoke(() => progress.Maximum = lastTask.Result.Header.m_PlaybackTicks.Value);

				lastTask.Result.SimulateDemo();
			});
		}

		private void GameEventTriggered(WorldState ws, IReadOnlyGameEvent e)
		{
			if (e.Declaration.Name == "player_hurt")
				Debugger.Break();

			Debug.WriteLine(e.Declaration.Name);
		}

		private void Events_NewTick(WorldState ws)
		{
			var tick = ws.Tick;
			TickLabel.Dispatcher.InvokeAsync(() =>
			{
				TickLabel.Content = string.Format("Tick {0}", tick);

				if (ws.EndTick.HasValue)
				{
					progress.Maximum = ws.EndTick.Value - ws.BaseTick;
					progress.Value = ws.Tick - ws.BaseTick;
					progress.IsIndeterminate = false;
				}
			}, DispatcherPriority.DataBind);
		}

		void UpdatePlayerPositions(WorldState ws)
		{
			foreach (Player p in ws.Players)
			{
				TF2Net.Data.Vector worldPos = p.Position.Value;
				Team? t = p.Team.Value;
				Class? c = p.Class.Value;
				bool? isDead = p.IsDead.Value;

				if (worldPos == null ||
					(t != Team.Red && t != Team.Blue) ||
					(!c.HasValue || c.Value == Class.Undefined || c.Value == Class.Civilian) ||
					(!isDead.HasValue || isDead == true) ||
					!p.InPVS)
				{
					IconsGrid.Dispatcher.InvokeAsync(() =>
					{
						var i = GetPlayerImage(p);
						i.Visibility = Visibility.Hidden;
					});

					continue;
				}

				IconsGrid.Dispatcher.InvokeAsync(() =>
				{
					var i = GetPlayerImage(p);
					Point displayPos = TranslateCoordinate(new Point(worldPos.X, worldPos.Y));

					Thickness newMargin = i.Margin;
					newMargin.Top = displayPos.Y - 25;
					newMargin.Left = displayPos.X - 25;
					i.Margin = newMargin;

					{
						string existingSource = null;
						BitmapImage src = i.Source as BitmapImage;
						if (src != null)
							existingSource = src.UriSource.AbsoluteUri;

						string newSource = ClassIcons[t.Value][c.Value];
						if (existingSource != newSource)
							i.Source = new BitmapImage(new Uri(newSource));
					}
					
					i.Visibility = Visibility.Visible;
				});
			}
		}
		void UpdatePlayerStatuses(WorldState ws)
		{
			foreach (Player p in ws.Players)
			{
				Team? t = p.Team.Value;

				Class? c = p.Class.Value;
				if (!c.HasValue)
					c = Class.Undefined;

				bool? isDead = p.IsDead.Value;
				if (!isDead.HasValue)
					continue;

				int? health = p.Health.Value;
				if (!health.HasValue)
					continue;

				uint? maxHealth = p.MaxHealth.Value;
				if (!maxHealth.HasValue)
					continue;

				Dispatcher.InvokeAsync(() =>
				{
					if (t != Team.Red && t != Team.Blue)
					{
						RedStatuses.Remove(RedStatuses.SingleOrDefault(s => s.GUID == p.Info.GUID));
						BlueStatuses.Remove(BlueStatuses.SingleOrDefault(s => s.GUID == p.Info.GUID));
						return;
					}
					
					PlayerStatus status = t == Team.Red ? RedStatuses.SingleOrDefault(s => s.GUID == p.Info.GUID) : BlueStatuses.SingleOrDefault(s => s.GUID == p.Info.GUID);
					bool added = false;
					if (status == null)
					{
						status = new PlayerStatus();
						status.GUID = p.Info.GUID;
						added = true;
					}

					status.Nickname = p.Info.Name;
					status.IsDead = isDead.Value;
					status.Team = t.Value;
					status.Health = health.Value;
					status.MaxHealth = maxHealth.Value;

					status.ClassPortrait = string.Format("{0} {1} {2} alpha", t.Value, c.Value, isDead.Value ? "grey" : "");

					if (added)
					{
						if (t == Team.Red)
							RedStatuses.Add(status);
						else
							BlueStatuses.Add(status);
					}

				}, DispatcherPriority.Background);
			}

			RedTeamHealth.Dispatcher.InvokeAsync(() =>
			{
				int health = 0;
				uint maxHealth = 0;
				foreach (PlayerStatus status in RedStatuses)
				{
					maxHealth += status.MaxOverheal;
					if (status.IsDead)
						continue;
					health += status.Health;
				}

				RedTeamHealth.Maximum = maxHealth;
				RedTeamHealth.Value = health;
			});
			BlueTeamHealth.Dispatcher.InvokeAsync(() =>
			{
				int health = 0;
				uint maxHealth = 0;
				foreach (PlayerStatus status in BlueStatuses)
				{
					maxHealth += status.MaxOverheal;
					if (status.IsDead)
						continue;
					health += status.Health;
				}

				BlueTeamHealth.Maximum = maxHealth;
				BlueTeamHealth.Value = health;
			});
		}

		Dictionary<TFRocket, Image> m_OldRockets = new Dictionary<TFRocket, Image>();
		void UpdateRocketPositions(WorldState ws)
		{
			Dictionary<TFRocket, Image> newRockets = new Dictionary<TFRocket, Image>();
			foreach (Entity e in ws.EntitiesInPVS)
			{
				if (e.Class.Classname != "CTFProjectile_Rocket")
					continue;

				TFRocket rocket = new TFRocket(e);
				TF2Net.Data.Vector position = rocket.Position.Value;
				if (position == null)
					continue;

				TF2Net.Data.Vector angle = rocket.Angle.Value;
				if (angle == null)
					continue;

				Team? team = rocket.Team.Value;
				if (team != Team.Red && team != Team.Blue)
					continue;

				Image elip;
				if (!m_OldRockets.TryGetValue(rocket, out elip))
				{
					IconsGrid.Dispatcher.Invoke(() =>
					{
						elip = new Image();

						if (team.Value == Team.Red)
							elip.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/images/projectiles/pill_red.png"));
						else
							elip.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/images/projectiles/pill_blue.png"));

						elip.Width = 15;
						elip.Height = 15;
						elip.SetValue(RenderOptions.BitmapScalingModeProperty, BitmapScalingMode.HighQuality);
						elip.HorizontalAlignment = HorizontalAlignment.Left;
						elip.VerticalAlignment = VerticalAlignment.Top;

						RotateTransform rotate = new RotateTransform(angle.X);
						elip.RenderTransform = rotate;

						IconsGrid.Children.Add(elip);
					});
				}
				else
					m_OldRockets.Remove(rocket);

				elip.Dispatcher.InvokeAsync(() =>
				{
					Point adjusted = TranslateCoordinate(new Point(position.X, position.Y));
					Thickness m = elip.Margin;
					m.Left = adjusted.X - 7.5;
					m.Top = adjusted.Y - 7.5;
					elip.Margin = m;
				});

				newRockets.Add(rocket, elip);
			}

			foreach (var kvPair in m_OldRockets)
				IconsGrid.Dispatcher.InvokeAsync(() => IconsGrid.Children.Remove(kvPair.Value));

			m_OldRockets = newRockets;
		}

		static string GetClassIconUri(Team t, Class c)
		{
			string teamName;
			switch (t)
			{
				case Team.Blue:
				teamName = "blu";
				break;

				case Team.Red:
				teamName = "red";
				break;

				default:
				throw new ArgumentOutOfRangeException(nameof(t));
			}

			string className;
			switch (c)
			{
				case Class.Scout:
				className = "scout";
				break;

				case Class.Soldier:
				className = "soldier";
				break;

				case Class.Pyro:
				className = "pyro";
				break;

				case Class.Demo:
				className = "demo";
				break;

				case Class.Heavy:
				className = "heavy";
				break;

				case Class.Engie:
				className = "engineer";
				break;

				case Class.Medic:
				className = "medic";
				break;

				case Class.Sniper:
				className = "sniper";
				break;

				case Class.Spy:
				className = "spy";
				break;

				default:
				throw new ArgumentOutOfRangeException(nameof(c));
			}

			return string.Format("pack://siteoforigin:,,,/images/classicons/{0}_{1}.png", className, teamName);
		}

		readonly Dictionary<string, Image> m_Images = new Dictionary<string, Image>();
		Image GetPlayerImage(Player p)
		{
			Image i;
			if (!m_Images.TryGetValue(p.Info.GUID, out i))
			{
				i = new Image();
				i.Width = 50;
				i.Height = 50;

				i.SnapsToDevicePixels = false;
				i.UseLayoutRounding = false;

				i.HorizontalAlignment = HorizontalAlignment.Left;
				i.VerticalAlignment = VerticalAlignment.Top;

				RenderOptions.SetBitmapScalingMode(i, BitmapScalingMode.HighQuality);

				Panel.SetZIndex(i, 50);

				IconsGrid.Children.Add(i);

				m_Images.Add(p.Info.GUID, i);
			}
			return i;
		}

		Point TranslateCoordinate(Point world)
		{
			const double height = 1024 * 8.4;
			const double originalWidth = height * (16.0 / 9.0);
			double width = height * (MapImage.ActualWidth / MapImage.ActualHeight);
			
			var centerWorld = new Point(-7516 + (originalWidth / 2), 4299 - (height / 2));
			var topLeftWorld = new Point(centerWorld.X - (width / 2), centerWorld.Y + (height / 2));
			var bottomRightWorld = new Point(topLeftWorld.X + width, topLeftWorld.Y - height);

			var widthBoost = Math.Max(0, (IconsGrid.ActualWidth - MapImage.ActualWidth) / 2);
			var heightBoost = Math.Max(0, (IconsGrid.ActualHeight - MapImage.ActualHeight) / 2);

			return new Point(
				Rescale(0, MapImage.ActualWidth, topLeftWorld.X, bottomRightWorld.X, world.X) + widthBoost,
				Rescale(0, MapImage.ActualHeight, topLeftWorld.Y, bottomRightWorld.Y, world.Y) + heightBoost);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Lerp(double param1, double param2, double x)
		{
			return (param1 + (param2 - param1) * x);
		}

		/// <summary>
		/// Interpolates linearly from a to b as t goes from x to y.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Rescale(double a, double b, double x, double y, double t)
		{
			return Lerp(a, b, (t - x) / (y - x));
		}
	}
}
