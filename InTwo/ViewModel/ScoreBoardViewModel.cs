using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using Cimbalino.Phone.Toolkit.Services;
using FlurryWP8SDK.Models;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using InTwo.Model;
using Scoreoid;
using ScottIsAFool.WindowsPhone.ViewModel;

namespace InTwo.ViewModel
{
    public class ScoreBoardViewModel : ViewModelBase
    {
        private readonly IExtendedNavigationService _navigationService;
        private readonly ScoreoidClient _scoreoidClient;

        private bool _scoresLoaded;

        public ScoreBoardViewModel(IExtendedNavigationService navigationService, ScoreoidClient scoreoidClient)
        {
            _navigationService = navigationService;
            _scoreoidClient = scoreoidClient;

            if (IsInDesignMode)
            {
                ScoreBoardItems = new List<player>
                {
                    new player
                    {
                        first_name = "Scott",
                        last_name = "Lovegrove",
                        username = "scottisafool",
                        best_score = "336",
                        rank = "1"
                    },
                    new player
                    {
                        first_name = "Mel",
                        last_name = "Sheppard",
                        username = "msheppard27",
                        best_score = "335",
                        rank = "2",
                    }
                }; 
                MostRecentScore = 336;
            }
        }

        public override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.ShareScoreMsg))
                {
                    var type = (ShareType)m.Sender;

                    var messageTemplate = "I've been playing {0}, my current best score is " + App.CurrentPlayer.best_score + ", try and beat me! http://www.windowsphone.com/s?appid=219b592c-c382-4b87-95db-8c85c96651c2";
                    string message;

                    Log.Info("Sharing score via {0}", type);

                    FlurryWP8SDK.Api.LogEvent("SharedScore", new List<Parameter> {new Parameter("ShareType", type.ToString())});

                    switch (type)
                    {
                        case ShareType.Email:
                            message = string.Format(messageTemplate, "In Two");
                            new EmailComposeService().Show("I've been playing In Two", message);
                            break;
                        case ShareType.Sms:
                            message = string.Format(messageTemplate, "In Two");
                            new SmsComposeService().Show("", message);
                            break;
                        case ShareType.Social:
                            message = string.Format(messageTemplate, "@InTwoApp");
                            new ShareLinkService().Show("", message, new Uri("http://www.windowsphone.com/s?appid=219b592c-c382-4b87-95db-8c85c96651c2", UriKind.Absolute));
                            break;
                    }
                }
            });
        }
        
        public List<player> ScoreBoardItems { get; set; }
        public int MostRecentScore { get; set; }

        public RelayCommand ScoreBoardPageLoaded
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    Log.Info("ScoreBoardPageLoaded");
                    Messenger.Default.Send(new NotificationMessageAction<int>(Constants.Messages.RequestScoreMsg, score =>
                    {
                        MostRecentScore = score;
                    }
                ));
                    await GetAllScoreData(false);
                });
            }
        }

        public RelayCommand RefreshScoresCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    Log.Info("Refreshing scores");
                    await GetAllScoreData(true);
                });
            }
        }

        private async Task GetAllScoreData(bool isRefresh)
        {
            if (!_scoresLoaded || isRefresh)
            {
                if (!_navigationService.IsNetworkAvailable) return;

                ProgressText = "Getting scores...";
                ProgressIsVisible = true;

                await GetScores();

                await UpdateUserScores();

                ProgressIsVisible = false;
                ProgressText = string.Empty;
            }
        }

        private async Task UpdateUserScores()
        {
            try
            {
                if (App.CurrentPlayer == null)
                {
                    return;
                }

                Log.Info("Getting all scores for user [{0}].", App.CurrentPlayer.username);
                var items = await _scoreoidClient.GetPlayerAsync(App.CurrentPlayer.username);

                if (items == null || !items.items.Any()) return;

                App.CurrentPlayer = items.items[0];
            }
            catch (ScoreoidException ex)
            {
                Log.Info("No scores for user [{0}]", App.CurrentPlayer.username);
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                Log.FatalException("UpdateUserScores", ex);
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
            }
        }

        private async Task GetScores()
        {
            try
            {
                Log.Info("Getting scores");
                var scoresResult = await _scoreoidClient.GetBestScoresAsync();

                ScoreBoardItems = scoresResult.items.ToList();

                _scoresLoaded = true;
            }
            catch (ScoreoidException ex)
            {
                Log.Info("No scores to get");
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                Log.FatalException("GetScores", ex);
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
            }
        }
    }
}
