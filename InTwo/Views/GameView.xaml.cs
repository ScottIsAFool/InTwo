﻿using System.ComponentModel;
using System.Windows;
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
                if (GamePlayer.CurrentState == MediaElementState.Buffering) return;

                _isPlaying = GamePlayer.CurrentState == MediaElementState.Playing;
                Messenger.Default.Send(new NotificationMessage(_isPlaying, Constants.Messages.IsPlayingMsg));
            };
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            e.Cancel = true;
            if (!App.SettingsWrapper.AppSettings.DontShowBackExitMessage)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    var message = new CustomMessageBox
                    {
                        Title = "Are you sure?",
                        Message = "You just pressed the back key which would exit you from this game, is that what you want to so?",
                        LeftButtonContent = "Yes, please",
                        RightButtonContent = "Oops, no",
                        Content = Utils.CreateDontShowCheckBox("DontShowBackExitMessage")
                    };
                    message.Dismissed += (sender, args) =>
                    {
                        ((CustomMessageBox)sender).Dismissing += (o, eventArgs) => eventArgs.Cancel = true;
                        if (args.Result == CustomMessageBoxResult.LeftButton)
                        {
                            NavigationService.GoBack();
                        }
                    };

                    message.Show();
                });

            }
        }
    }
}