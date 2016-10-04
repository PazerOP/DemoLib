using System;
using System.Collections.Generic;
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
		readonly SolidColorBrush RedBrush = new SolidColorBrush(Color.FromArgb(255, 189, 59, 59));
		readonly SolidColorBrush BluBrush = new SolidColorBrush(Color.FromArgb(255, 91, 122, 140));

		Task<DemoReader> m_Reader;
		public MainWindow()
		{
			InitializeComponent();

			m_Reader = Task.Run(() =>
			{
				var stream = File.Open(@"D:\Steam\steamapps\common\Team Fortress 2\tf\demos\testdemo.dem", FileMode.Open, FileAccess.Read);
				return DemoReader.FromStream(stream);
			});

			m_Reader.ContinueWith(lastTask =>
			{
				lastTask.Result.Events.NewTick += Events_NewTick;
				lastTask.Result.Events.PlayerAdded += Events_PlayerAdded;

				progress.Dispatcher.Invoke(() => progress.Maximum = lastTask.Result.Header.m_PlaybackTicks.Value);

				lastTask.Result.SimulateDemo();
			});
		}

		private void Events_PlayerAdded(Player p)
		{
			//p.EnteredPVS += UpdatePlayerPosition;			
			p.LeftPVS += PlayerLeftPVS;

			p.PropertiesUpdated += UpdatePlayerPosition;
		}

		private void Events_NewTick(WorldState ws)
		{
			var tick = ws.Tick;
			TickLabel.Dispatcher.InvokeAsync(() =>
			{
				TickLabel.Content = string.Format("Tick {0}", tick);

				progress.Value = tick;
			}, DispatcherPriority.DataBind);
		}

		private void PlayerLeftPVS(Player p)
		{
			BaseGrid.Dispatcher.Invoke(() =>
			{
				var e = GetPlayerImage(p);
				e.Visibility = Visibility.Hidden;
			});
		}

		static string GetImagePath(Team t, Class c)
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

			return string.Format("/classicons/{0}_{1}.png", className, teamName);
		}

		void UpdatePlayerPosition(Player p)
		{
			Debug.Assert(p.InPVS);

			TF2Net.Data.Vector worldPos = p.Position.Value;
			Team? t = p.Team.Value;
			Class? c = p.Class.Value;

			var entityIndex = p.EntityIndex;

			BaseGrid.Dispatcher.InvokeAsync(() =>
			{
				var e = GetPlayerImage(p);
				if (worldPos == null || !t.HasValue || !c.HasValue)
				{
					e.Visibility = Visibility.Hidden;
					return;
				}

				Point displayPos = TranslateCoordinate(new Point(worldPos.X, worldPos.Y));

				Thickness newMargin = new Thickness();
				newMargin.Top = displayPos.Y - 25;
				newMargin.Left = displayPos.X - 25;
				e.Margin = newMargin;

				e.Source = new BitmapImage(new Uri(GetImagePath(t.Value, c.Value), UriKind.Relative));

				e.Visibility = Visibility.Visible;
			}, DispatcherPriority.Render);
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

				Grid.SetColumnSpan(i, 2);

				BaseGrid.Children.Add(i);

				m_Images.Add(p.Info.GUID, i);
			}
			return i;
		}

		readonly Dictionary<string, Ellipse> m_Ellipses = new Dictionary<string, Ellipse>();
		Ellipse GetPlayerMarker(Player p)
		{
			Ellipse e;
			if (!m_Ellipses.TryGetValue(p.Info.GUID, out e))
			{
				e = new Ellipse();
				e.Width = 10;
				e.Height = 10;

				e.SnapsToDevicePixels = false;
				e.UseLayoutRounding = false;
				e.Stroke = Brushes.Black;
				e.HorizontalAlignment = HorizontalAlignment.Left;
				e.VerticalAlignment = VerticalAlignment.Top;
				BaseGrid.Children.Add(e);

				m_Ellipses.Add(p.Info.GUID, e);
			}
			return e;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
		}

		Point TranslateCoordinate(Point world)
		{
			const double height = 1024 * 8.5;
			const double width = height * (16.0 / 9.0);
			
			var topLeftWorld = new Point(-8636, 4301);
			var bottomRightWorld = new Point(topLeftWorld.X + width, topLeftWorld.Y - height);

			var widthBoost = Math.Max(0, (BaseGrid.ActualWidth - MapImage.ActualWidth) / 2);
			var heightBoost = Math.Max(0, (BaseGrid.ActualHeight - MapImage.ActualHeight) / 2);

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
