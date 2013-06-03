using System;
using System.Windows.Media;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;

namespace InTwo.Views
{
    public partial class GameView : PhoneApplicationPage
    {
        private bool _isPlaying;
        public GameView()
        {
            InitializeComponent();

            GamePlayer.CurrentStateChanged += (sender, args) =>
            {
                _isPlaying = GamePlayer.CurrentState == MediaElementState.Playing;
                Messenger.Default.Send(new NotificationMessage(_isPlaying, Constants.Messages.IsPlayingMsg));
            };
        }

        private void PlayPauseButton_OnClick(object sender, EventArgs e)
        {
            if (_isPlaying)
            {
                GamePlayer.Pause();
            }
            else
            {
                GamePlayer.Play();
            }
        }
    }
}