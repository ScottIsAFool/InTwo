using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ScoreoidPortable.Entities;


namespace InTwo.Controls
{
    public partial class UserDetails : UserControl
    {
        public UserDetails()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty EnterCommandProperty =
            DependencyProperty.Register("EnterCommand", typeof (ICommand), typeof (UserDetails), new PropertyMetadata(default(ICommand)));

        public ICommand EnterCommand
        {
            get { return (ICommand) GetValue(EnterCommandProperty); }
            set { SetValue(EnterCommandProperty, value); }
        }

        public static readonly DependencyProperty EnterCommandStringProperty =
            DependencyProperty.Register("EnterCommandString", typeof (string), typeof (UserDetails), new PropertyMetadata(default(string)));

        public string EnterCommandString
        {
            get { return (string) GetValue(EnterCommandStringProperty); }
            set { SetValue(EnterCommandStringProperty, value); }
        }

        public static readonly DependencyProperty PlayerProperty =
            DependencyProperty.Register("Player", typeof (Player), typeof (UserDetails), new PropertyMetadata(default(Player)));

        public Player Player
        {
            get { return (Player) GetValue(PlayerProperty); }
            set { SetValue(PlayerProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof (string), typeof (UserDetails), new PropertyMetadata(default(string)));

        public string Description
        {
            get { return (string) GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }
    }
}
