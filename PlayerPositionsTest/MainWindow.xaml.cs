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
		
		public PlayerStatusesList Statuses
		{
			get { return (PlayerStatusesList)GetValue(StatusesProperty); }
			set { SetValue(StatusesProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Statuses.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty StatusesProperty =
			DependencyProperty.Register("Statuses", typeof(PlayerStatusesList), typeof(MainWindow), new PropertyMetadata(new PlayerStatusesList()));

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

				progress.Dispatcher.Invoke(() => progress.Maximum = lastTask.Result.Header.m_PlaybackTicks.Value);

				lastTask.Result.SimulateDemo();
			});
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
					BaseGrid.Dispatcher.InvokeAsync(() =>
					{
						var i = GetPlayerImage(p);
						i.Visibility = Visibility.Hidden;
					});

					continue;
				}

				BaseGrid.Dispatcher.InvokeAsync(() =>
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

				PlayerStatusesControl.Dispatcher.InvokeAsync(() =>
				{
					if (t != Team.Red && t != Team.Blue)
					{
						Statuses.Remove(Statuses.SingleOrDefault(s => s.GUID == p.Info.GUID));
						return;
					}
					
					PlayerStatus status = Statuses.SingleOrDefault(s => s.GUID == p.Info.GUID);
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
						Statuses.Add(status);

				}, DispatcherPriority.Background);
			}
		}

		private void PlayerLeftPVS(Player p)
		{
			BaseGrid.Dispatcher.Invoke(() =>
			{
				var e = GetPlayerImage(p);
				e.Visibility = Visibility.Hidden;
			});
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

				Grid.SetColumn(i, 1);

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
