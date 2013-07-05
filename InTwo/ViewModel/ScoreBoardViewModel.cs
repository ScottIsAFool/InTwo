using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using Windows.System;
using Cimbalino.Phone.Toolkit.Services;
using FlurryWP8SDK.Models;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using InTwo.Model;
using ScoreoidPortable;
using ScoreoidPortable.Entities;
using ScottIsAFool.WindowsPhone.ViewModel;

namespace InTwo.ViewModel
{
    public class ScoreBoardViewModel : ViewModelBase
    {
        private readonly IExtendedNavigationService _navigationService;
        private readonly IScoreoidClient _scoreoidClient;

        private bool _scoresLoaded;

        public ScoreBoardViewModel(IExtendedNavigationService navigationService, IScoreoidClient scoreoidClient)
        {
            _navigationService = navigationService;
            _scoreoidClient = scoreoidClient;

            if (IsInDesignMode)
            {
                ScoreBoardItems = new ObservableCollection<ScoreWrapper>
                {
                    new ScoreWrapper
                    {
                        Score = new ScoreItem
                        {
                            Player = new Player
                            {
                                FirstName = "Scott",
                                LastName = "Lovegrove",
                                Username = "scottisafool",
                                BestScore = 336,
                                Rank = 1
                            }
                        },
                        Rank = 1
                    },
                    new ScoreWrapper
                    {
                        Score = new ScoreItem
                        {
                            Player = new Player
                            {
                                FirstName = "Mel",
                                LastName = "Sheppard",
                                Username = "msheppard27",
                                BestScore = 335,
                                Rank = 2,
                            }
                        },
                        Rank = 2
                    }
                };
                MostRecentScore = 336;
            }
            else
            {
                ScoreBoardItems = new ObservableCollection<ScoreWrapper>();
            }
        }

        public override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.ShareScoreMsg))
                {
                    var type = (ShareType)m.Sender;

                    var messageTemplate = "I've been playing {0}, my current best score is " + App.CurrentPlayer.BestScore + ", try and beat me! ";
                    string message;

                    Log.Info("Sharing score via {0}", type);

                    FlurryWP8SDK.Api.LogEvent("SharedScore", new List<Parameter> { new Parameter("ShareType", type.ToString()) });

                    switch (type)
                    {
                        case ShareType.Email:
                            message = string.Format(messageTemplate + "http://www.windowsphone.com/s?appid=219b592c-c382-4b87-95db-8c85c96651c2", "In Two");
                            new EmailComposeService().Show("I've been playing In Two for Windows Phone", message);
                            break;
                        case ShareType.Sms:
                            message = string.Format(messageTemplate + "http://www.windowsphone.com/s?appid=219b592c-c382-4b87-95db-8c85c96651c2", "In Two");
                            new SmsComposeService().Show("", message);
                            break;
                        case ShareType.Social:
                            message = string.Format(messageTemplate, "@InTwoApp");
                            new ShareLinkService().Show("", message, new Uri("http://www.windowsphone.com/s?appid=219b592c-c382-4b87-95db-8c85c96651c2", UriKind.Absolute));
                            break;
                        case ShareType.Mehdoh:
                            message = string.Format(messageTemplate + "http://www.windowsphone.com/s?appid=219b592c-c382-4b87-95db-8c85c96651c2", "@InTwoApp");
                            Launcher.LaunchUriAsync(new Uri("mehdoh:TwitterPost?Text=" + message));
                            break;
                    }
                }
            });
        }

        public ObservableCollection<ScoreWrapper> ScoreBoardItems { get; set; }
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

                SetProgressBar("Getting scores...");

                await GetScores();

                await UpdateUserScores();

                SetProgressBar();
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

                Log.Info("Getting all scores for user [{0}].", App.CurrentPlayer.Username);
                var item = await _scoreoidClient.GetPlayerAsync(App.CurrentPlayer.Username);

                if (item == null) return;

                App.CurrentPlayer = item;
            }
            catch (ScoreoidException ex)
            {
                Log.Info("No scores for user [{0}]", App.CurrentPlayer.Username);
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
                var scoresResult = await _scoreoidClient.GetBestScoresAsync(SortBy.Score, OrderBy.Descending);

                ScoreBoardItems.Clear();

                var i = 1;
                scoresResult.ForEach(score =>
                {
                    ScoreBoardItems.Add(new ScoreWrapper
                    {
                        Rank = i,
                        Score = score
                    });
                    i++;
                });

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
