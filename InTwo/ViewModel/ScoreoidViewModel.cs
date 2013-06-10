using System;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using InTwo.Model;
using Scoreoid;
using ScottIsAFool.WindowsPhone.ViewModel;

namespace InTwo.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ScoreoidViewModel : ViewModelBase
    {
        private readonly IExtendedNavigationService _navigationService;
        private readonly ScoreoidClient _scoreoidClient;

        /// <summary>
        /// Initializes a new instance of the ScoresViewModel class.
        /// </summary>
        public ScoreoidViewModel(IExtendedNavigationService navigation, ScoreoidClient scoreoidClient)
        {
            _navigationService = navigation;
            _scoreoidClient = scoreoidClient;

            if (IsInDesignMode)
            {
                CurrentPlayer = new player
                {
                    username = "scottisafool",
                    best_score = "336",
                    rank = "1"
                };
            }
        }

        public override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessageAction<bool>>(this, async m =>
            {
                if (m.Notification.Equals(Constants.Messages.SubmitScoreMsg))
                {
                    var score = (score) m.Sender;

                    var result = await SubmitScore(score);

                    m.Execute(result);
                }
            });
        }

        private async Task<bool> SubmitScore(score score)
        {
            try
            {
                await _scoreoidClient.CreateScoreAsync(App.CurrentPlayer.username, score);

                App.SettingsWrapper.AppSettings.MostRecentScore = score;

                Messenger.Default.Send(new NotificationMessage(Constants.Messages.ForceSettingsSaveMsg));

                return true;
            }
            catch (ScoreoidException)
            {

            }
            catch (Exception)
            {
                
            }
            return false;
        }

        public player CurrentPlayer { get; set; }
        
        public string Username { get; set; }
        public string Password { get; set; }

        public bool CanLogIn
        {
            //get { return CurrentPlayer != null && CurrentPlayer.username.Length > 0 && !ProgressIsVisible; }
            get { return true; }
        }
        
        #region Commands

        #region Page Loaded Commands
        public RelayCommand CreateNewUserPageLoaded
        {
            get
            {
                return new RelayCommand(() =>
                {
                    CurrentPlayer = new player();
                });
            }
        }
        public RelayCommand EditUserPageLoaded
        {
            get
            {
                return new RelayCommand(() =>
                {
                    CurrentPlayer = App.CurrentPlayer;
                });
            }
        }
        #endregion

        public RelayCommand CreateNewUserCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (CurrentPlayer == null
                        || string.IsNullOrEmpty(CurrentPlayer.username)) return;

                    if (!_navigationService.IsNetworkAvailable) return;

                    try
                    {
                        ProgressIsVisible = true;
                        ProgressText = "Creating user...";

                        await _scoreoidClient.CreatePlayerAsync(CurrentPlayer);

                        App.CurrentPlayer = CurrentPlayer;

                        MessageBox.Show("Player created successfully, you can now sign in with this username.", "Success", MessageBoxButton.OK);

                        _navigationService.GoBack();
                    }
                    catch (ScoreoidException ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
                    }
                    catch (Exception ex)
                    {

                    }

                    ProgressIsVisible = false;
                    ProgressText = string.Empty;
                });
            }
        }

        public RelayCommand UpdateUserCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (CurrentPlayer == null
                        || string.IsNullOrEmpty(CurrentPlayer.username)) return;

                    if (!_navigationService.IsNetworkAvailable) return;

                    try
                    {
                        ProgressIsVisible = true;
                        ProgressText = "Updating user...";

                        var response = await _scoreoidClient.UpdatePlayerAsync(CurrentPlayer);

                        App.CurrentPlayer = CurrentPlayer;

                        MessageBox.Show(response, "Success", MessageBoxButton.OK);
                    }
                    catch (ScoreoidException ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
                    }
                    catch (Exception ex)
                    {

                    }

                    ProgressIsVisible = false;
                    ProgressText = string.Empty;
                });
            }
        }

        public RelayCommand SignInCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (string.IsNullOrEmpty(Username)) return;

                    if (!_navigationService.IsNetworkAvailable) return;

                    ProgressIsVisible = true;
                    ProgressText = "Signing in...";

                    try
                    {
                        var response = await _scoreoidClient.GetPlayerAsync(Username, Password);

                        App.CurrentPlayer = response.items[0];

                        ProgressIsVisible = false;
                        ProgressText = string.Empty;

                        Messenger.Default.Send(new NotificationMessage(Constants.Messages.RefreshCurrentPlayerInfoMsg));

                        _navigationService.GoBack();
                    }
                    catch (ScoreoidException ex)
                    {

                    }
                    catch (Exception ex)
                    {

                    }

                    ProgressIsVisible = false;
                    ProgressText = string.Empty;
                });
            }
        }

        public RelayCommand CancelCreationCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    CurrentPlayer = null;
                    if (_navigationService.CanGoBack)
                        _navigationService.GoBack();
                });
            }
        }

        #endregion
    }
}