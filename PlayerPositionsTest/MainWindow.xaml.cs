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
				lastTask.Result.Events.StringTableUpdated += Events_StringTableUpdated;
				lastTask.Result.SimulateDemo();
			});
		}

		private void Events_NewTick(WorldState ws)
		{
			TickLabel.Dispatcher.Invoke(() =>
			{
				TickLabel.Content = string.Format("Tick {0}", ws.Tick);
			});
		}

		private void Events_StringTableUpdated(WorldState ws, StringTable st)
		{
			if (st.TableName == "userinfo")
			{
				foreach (Player p in ws.Players)
				{
					p.EnteredPVS -= PlayerEnteredPVS;
					p.EnteredPVS += PlayerEnteredPVS;

					p.PropertiesUpdated -= PlayerPropertiesUpdated;
					p.PropertiesUpdated += PlayerPropertiesUpdated;

					p.LeftPVS -= PlayerLeftPVS;
					p.LeftPVS += PlayerLeftPVS;
				}
			}
		}

		private void PlayerLeftPVS(WorldState ws, Player p)
		{
			BaseGrid.Dispatcher.Invoke(() =>
			{
				Ellipse e = GetPlayerMarker(p);
				e.Visibility = Visibility.Hidden;
			});
		}

		Dictionary<string, Ellipse> m_Ellipses = new Dictionary<string, Ellipse>();
		private void PlayerEnteredPVS(WorldState ws, Player p)
		{
			BaseGrid.Dispatcher.Invoke(() =>
			{
				Ellipse e = GetPlayerMarker(p);

				UpdatePlayerPosition(p);
			});
		}

		private void PlayerPropertiesUpdated(WorldState ws, Player p)
		{
			BaseGrid.Dispatcher.Invoke(() =>
			{
				UpdatePlayerPosition(p);
			});
		}

		Ellipse GetPlayerMarker(Player p)
		{
			Ellipse e;
			if (!m_Ellipses.TryGetValue(p.GUID, out e))
			{
				e = new Ellipse();
				e.Width = 10;
				e.Height = 10;

				if (p.EntityIndex == 5)
					e.Fill = Brushes.Lime;
				else
					e.Fill = Brushes.Red;

				e.SnapsToDevicePixels = false;
				e.UseLayoutRounding = false;
				e.Stroke = Brushes.Black;
				e.HorizontalAlignment = HorizontalAlignment.Left;
				e.VerticalAlignment = VerticalAlignment.Top;
				BaseGrid.Children.Add(e);

				m_Ellipses.Add(p.GUID, e);
			}
			return e;
		}

		void UpdatePlayerPosition(Player p)
		{
			Debug.Assert(p.InPVS);

			Ellipse e = GetPlayerMarker(p);

			TF2Net.Data.Vector worldPos = p.Position;
			if (worldPos == null)
			{
				e.Visibility = Visibility.Hidden;
				return;
			}

			Point displayPos = TranslateCoordinate(new Point(worldPos.X, worldPos.Y));

			Thickness newMargin = new Thickness();
			newMargin.Top = displayPos.Y;
			newMargin.Left = displayPos.X;

			var team = p.Team;
			if (p.EntityIndex == 5)
				e.Fill = Brushes.Lime;
			else if (team == Team.Red)
				e.Fill = Brushes.Red;
			else if (team == Team.Blue)
				e.Fill = Brushes.DarkCyan;
			else if (team == Team.Spectator)
				e.Fill = Brushes.White;
			else
				e.Fill = Brushes.Orange;


			e.Visibility = Visibility.Visible;
			e.Margin = newMargin;
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
