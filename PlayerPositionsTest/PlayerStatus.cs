using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using TF2Net.Data;

namespace PlayerPositionsTest
{
	public class PlayerStatus : INotifyPropertyChanged
	{
		public PlayerStatus() { }

		public string GUID { get; set; }

		string m_Nickname;
		public string Nickname
		{
			get { return m_Nickname; }
			set
			{
				if (value != m_Nickname)
				{
					m_Nickname = value;
					NotifyPropertyChanged();
				}
			}
		}

		bool m_IsDead;
		public bool IsDead
		{
			get { return m_IsDead; }
			set
			{
				if (value != m_IsDead)
				{
					m_IsDead = value;
					NotifyPropertyChanged();
				}
			}
		}

		Team m_Team;
		public Team Team
		{
			get { return m_Team; }
			set
			{
				if (value != m_Team)
				{
					m_Team = value;
					NotifyPropertyChanged();
				}
			}
		}

		int m_Health;
		public int Health
		{
			get { return m_Health; }
			set
			{
				if (value != m_Health)
				{
					m_Health = value;
					NotifyPropertyChanged();
				}
			}
		}

		uint m_MaxHealth;
		public uint MaxHealth
		{
			get { return m_MaxHealth; }
			set
			{
				if (value != m_MaxHealth)
				{
					m_MaxHealth = value;
					NotifyPropertyChanged();
					NotifyPropertyChanged(nameof(MaxOverheal));
				}
			}
		}
		
		public uint MaxOverheal { get { return (uint)(m_MaxHealth * 1.5) / 5 * 5; } }

		string m_ClassPortrait;
		public string ClassPortrait
		{
			get { return m_ClassPortrait; }
			set
			{
				if (value != m_ClassPortrait)
				{
					m_ClassPortrait = value;
					NotifyPropertyChanged();
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
