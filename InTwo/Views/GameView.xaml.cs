using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            //if (!App.SettingsWrapper.AppSettings.DontShowBackExitMessage)
            //{
            //    var message = new CustomMessageBox
            //    {
            //        Title = "Are you sure?",
            //        Message = "You just pressed the back key which would exit you from this game, is that what you want to so?",
            //        LeftButtonContent = "Yes, please",
            //        RightButtonContent = "Oops, no",
            //        Content = Utils.CreateDontShowCheckBox("DontShowBackExitMessage")
            //    };
            //    message.Dismissed += (sender, args) =>
            //    {
            //        ((CustomMessageBox) sender).Dismissing += (o, eventArgs) => eventArgs.Cancel = true;
            //        if (args.Result == CustomMessageBoxResult.RightButton)
            //        {
            //            //e.Cancel = true;
            //        }
            //    };

            //    message.Show();
            //}
            base.OnBackKeyPress(e);
        }


    }
}