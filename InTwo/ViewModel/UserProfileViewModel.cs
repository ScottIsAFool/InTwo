using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Scoreoid;

namespace InTwo.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class UserProfileViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly ScoreoidClient _scoreoidClient;

        /// <summary>
        /// Initializes a new instance of the UserProfileViewModel class.
        /// </summary>
        public UserProfileViewModel(INavigationService navigationService, ScoreoidClient scoreoidClient)
        {
            _navigationService = navigationService;
            _scoreoidClient = scoreoidClient;

            if (IsInDesignMode)
            {
                CurrentPlayer = App.CurrentPlayer;
            }

            TotalScore = NumberOfGames = 0;
        }

        public string ProgressText { get; set; }
        public bool ProgressIsVisible { get; set; }

        public player CurrentPlayer { get; set; }
        public int TotalScore { get; set; }
        public int NumberOfGames { get; set; }

        #region Commands
        public RelayCommand UserProfilePageLoaded
        {
            get
            {
                return new RelayCommand(async () =>
                                            {
                                                // TODO: check for internet connection
                                                CurrentPlayer = App.CurrentPlayer;

                                                
                                                await GetPlayerInformation();
                                            });
            }
        }

        public RelayCommand EditUserCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    CurrentPlayer = App.CurrentPlayer;
                    _navigationService.NavigateTo(Constants.Pages.Scoreoid.EditUser);
                });
            }
        }

        public RelayCommand RefreshUserCommand
        {
            get
            {
                return new RelayCommand(async () =>
                                            {
                                                await GetPlayerInformation();
                                            });
            }
        }

        public RelayCommand DeleteUserCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    var result = MessageBox.Show("Are you sure you want to delete your user? Once you do this, there's no going back, I can assure you! If you just want to logout, use the logout button", "Are you sure?", MessageBoxButton.OKCancel);

                    if (result == MessageBoxResult.Cancel) return;

                    try
                    {
                        await _scoreoidClient.DeletePlayerAsync(App.SettingsWrapper.AppSettings.CurrentPlayer);

                        App.SettingsWrapper.AppSettings.CurrentPlayer = null;

                        _navigationService.NavigateTo(Constants.Pages.MainPage + Constants.ClearBackStack);
                    }
                    catch (ScoreoidException ex)
                    {

                    }
                    catch (Exception ex)
                    {

                    }
                });
            }
        }
        #endregion

        private async Task GetPlayerInformation()
        {
            ProgressIsVisible = true;
            ProgressText = "Getting latest information...";

            try
            {
                var items = await _scoreoidClient.GetPlayerAsync(CurrentPlayer.username);
                CurrentPlayer = items.items[0];

                var scores = await _scoreoidClient.GetPlayerScores(CurrentPlayer.username);

                NumberOfGames = scores.items.Any() ? scores.items.Length : 0;

                if (scores.items.Any())
                {
                    int totalScore;
                    foreach (var score in scores.items)
                    {
                        int count;
                        if (int.TryParse(score.value, out count))
                        {
                            totalScore += count;
                        }
                    }
                    TotalScore = totalScore;
                }
            }
            catch (ScoreoidException ex)
            {

            }
            catch (Exception ex)
            {
                
            }

            ProgressIsVisible = false;
            ProgressText = string.Empty;
        }
    }
}