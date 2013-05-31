using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cimbalino.Phone.Toolkit.Services;
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
            }
        }

        public override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.ShareScoreMsg))
                {
                    var type = (ShareType)m.Sender;

                    switch (type)
                    {
                        case ShareType.Email:

                            break;
                            case ShareType.Sms:

                            break;
                            case ShareType.Social:

                            break;
                    }
                }
            });
        }
        
        public List<player> ScoreBoardItems { get; set; }

        public RelayCommand ScoreBoardPageLoaded
        {
            get
            {
                return new RelayCommand(async () =>
                {
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
                var items = await _scoreoidClient.GetPlayerAsync(App.SettingsWrapper.AppSettings.CurrentPlayer.username);

                if (items == null || !items.items.Any()) return;

                App.SettingsWrapper.AppSettings.CurrentPlayer = items.items[0];
            }
            catch (ScoreoidException ex)
            {

            }
            catch (Exception ex)
            {

            }
        }

        private async Task GetScores()
        {

            try
            {
                var scoresResult = await _scoreoidClient.GetBestScoresAsync();

                ScoreBoardItems = scoresResult.items.ToList();

                _scoresLoaded = true;
            }
            catch (ScoreoidException ex)
            {
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
