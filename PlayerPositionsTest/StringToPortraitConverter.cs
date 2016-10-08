using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TF2Net.Data;

namespace PlayerPositionsTest
{
	[ValueConversion(typeof(string), typeof(ImageSource))]
	class StringToPortraitConverter : MarkupExtension, IValueConverter
	{
		class ClassPortraitSet
		{
			public string Blue { get; }
			public string BlueDead { get; }
			public string Red { get; }
			public string RedDead { get; }

			public string AlphaBlue { get; }
			public string AlphaBlueDead { get; }
			public string AlphaRed { get; }
			public string AlphaRedDead { get; }

			public ClassPortraitSet(Class c)
			{
				string cls = c.ToString().ToLower();

				try
				{
					//Blue = new BitmapImage(new Uri(string.Format(@"class_portraits/noalpha/{0}_blue.png", cls), UriKind.Relative));
					//BlueDead = new BitmapImage(new Uri(string.Format("class_portraits/noalpha/{0}_blue_grey.png", cls), UriKind.Relative));
					//Red = new BitmapImage(new Uri(string.Format("class_portraits/noalpha/{0}.png", cls), UriKind.Relative));
					//RedDead = new BitmapImage(new Uri(string.Format("class_portraits/noalpha/{0}_grey.png", cls), UriKind.Relative));

					//AlphaBlue = new BitmapImage(new Uri(string.Format("class_portraits/alpha/{0}_blue.png", cls), UriKind.Relative));
					//AlphaBlueDead = new BitmapImage(new Uri(string.Format("class_portraits/alpha/{0}_blue_grey.png", cls), UriKind.Relative));
					//AlphaRed = new BitmapImage(new Uri(string.Format("class_portraits/alpha/{0}.png", cls), UriKind.Relative));
					//AlphaRedDead = new BitmapImage(new Uri(string.Format("class_portraits/alpha/{0}_grey.png", cls), UriKind.Relative));

					Blue = string.Format("class_portraits/noalpha/{0}_blue.png", cls);
					BlueDead = string.Format("class_portraits/noalpha/{0}_blue_grey.png", cls);
					Red = string.Format("class_portraits/noalpha/{0}.png", cls);
					RedDead = string.Format("class_portraits/noalpha/{0}_grey.png", cls);

					AlphaBlue = string.Format("class_portraits/alpha/{0}_blue.png", cls);
					AlphaBlueDead = string.Format("class_portraits/alpha/{0}_blue_grey.png", cls);
					AlphaRed = string.Format("class_portraits/alpha/{0}.png", cls);
					AlphaRedDead = string.Format("class_portraits/alpha/{0}_grey.png", cls);
				}
				catch (Exception e)
				{
					Debugger.Break();
					throw;
				}
			}

			public string Get(Team t, bool isDead, bool alpha)
			{
				if (t == Team.Red)
				{
					if (isDead)
					{
						if (alpha)
							return AlphaRedDead;
						else
							return RedDead;
					}
					else
					{
						if (alpha)
							return AlphaRed;
						else
							return Red;
					}
				}
				else if (t == Team.Blue)
				{
					if (isDead)
					{
						if (alpha)
							return AlphaBlueDead;
						else
							return BlueDead;
					}
					else
					{
						if (alpha)
							return AlphaBlue;
						else
							return Blue;
					}
				}
				else
					throw new ArgumentOutOfRangeException(nameof(t));
			}
		}

		static readonly IReadOnlyDictionary<Class, ClassPortraitSet> ClassPortraits;
		static StringToPortraitConverter()
		{
			ClassPortraits = new Dictionary<Class, ClassPortraitSet>()
			{
				{ Class.Scout, new ClassPortraitSet(Class.Scout) },
				{ Class.Soldier, new ClassPortraitSet(Class.Soldier) },
				{ Class.Pyro, new ClassPortraitSet(Class.Pyro) },
				{ Class.Demo, new ClassPortraitSet(Class.Demo) },
				{ Class.Heavy, new ClassPortraitSet(Class.Heavy) },
				{ Class.Engie, new ClassPortraitSet(Class.Engie) },
				{ Class.Medic, new ClassPortraitSet(Class.Medic) },
				{ Class.Sniper, new ClassPortraitSet(Class.Sniper) },
				{ Class.Spy, new ClassPortraitSet(Class.Spy) }
			};
		}

		public StringToPortraitConverter() { }

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				throw new ArgumentNullException(nameof(value));
			if (!(value is string))
				throw new ArgumentException(nameof(value));

			string str = (string)value;
			if (str.IndexOf("undefined", StringComparison.OrdinalIgnoreCase) >= 0)
				return "class_portraits/alpha/silhouette.png";

			Class? c = null;
			for (int i = 1; i <= 9; i++)
			{
				string className = ((Class)i).ToString();

				if (str.IndexOf(className, StringComparison.OrdinalIgnoreCase) >= 0)
				{
					c = (Class)i;
					break;
				}
			}

			if (!c.HasValue)
				throw new ArgumentException("No valid class specified", nameof(value));

			ClassPortraitSet set = ClassPortraits[c.Value];

			Team? t = null;
			if (str.IndexOf("blue", StringComparison.OrdinalIgnoreCase) >= 0)
				t = Team.Blue;
			else if (str.IndexOf("red", StringComparison.OrdinalIgnoreCase) >= 0)
				t = Team.Red;

			if (!t.HasValue)
				throw new ArgumentException("No valid team specified", nameof(value));

			bool grey = false;
			if (str.IndexOf("grey", StringComparison.OrdinalIgnoreCase) >= 0)
				grey = true;

			bool alpha = false;
			if (str.IndexOf("alpha", StringComparison.OrdinalIgnoreCase) >= 0)
				alpha = true;

			if (t.Value == Team.Red)
			{
				if (grey)
				{
					if (alpha)
						return set.AlphaRedDead;
					else
						return set.RedDead;
				}
				else
				{
					if (alpha)
						return set.AlphaRed;
					else
						return set.Red;
				}
			}
			else if (t.Value == Team.Blue)
			{
				if (grey)
				{
					if (alpha)
						return set.AlphaBlueDead;
					else
						return set.BlueDead;
				}
				else
				{
					if (alpha)
						return set.AlphaBlue;
					else
						return set.Blue;
				}
			}

			return DependencyProperty.UnsetValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string str = (string)value;
			for (int i = 1; i <= 9; i++)
			{
				Class c = (Class)i;
				var set = ClassPortraits[c];

				if (str == set.AlphaBlue)
					return string.Format("{0} alpha blue", c);
				if (str == set.AlphaBlueDead)
					return string.Format("{0} alpha blue grey", c);
				if (str == set.AlphaRed)
					return string.Format("{0} alpha red", c);
				if (str == set.AlphaRedDead)
					return string.Format("{0} alpha red grey", c);
				if (str == set.Blue)
					return string.Format("{0} blue", c);
				if (str == set.BlueDead)
					return string.Format("{0} blue grey", c);
				if (str == set.Red)
					return string.Format("{0} red", c);
				if (str == set.RedDead)
					return string.Format("{0} red grey", c);
			}

			throw new ArgumentException(nameof(value));
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}
	}
}
