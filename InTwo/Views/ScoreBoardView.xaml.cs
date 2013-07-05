using System;
using Coding4Fun.Toolkit.Controls;
using GalaSoft.MvvmLight.Messaging;
using InTwo.Model;
using Microsoft.Phone.Controls;

namespace InTwo.Views
{
    public partial class ScoreBoardView 
    {
        public ScoreBoardView()
        {
            InitializeComponent();
        }

        private void ShareButton_OnClick(object sender, EventArgs e)
        {
            new AppBarPrompt(new AppBarPromptAction("share by sms", () => Messenger.Default.Send(new NotificationMessage(ShareType.Sms, Constants.Messages.ShareScoreMsg))),
                             new AppBarPromptAction("share by email", () => Messenger.Default.Send(new NotificationMessage(ShareType.Email, Constants.Messages.ShareScoreMsg))),
                             new AppBarPromptAction("share on social networks", () => Messenger.Default.Send(new NotificationMessage(ShareType.Social, Constants.Messages.ShareScoreMsg))),
                             new AppBarPromptAction("share with Mehdoh", () => Messenger.Default.Send(new NotificationMessage(ShareType.Mehdoh, Constants.Messages.ShareScoreMsg))))
                .Show();
        }
    }
}