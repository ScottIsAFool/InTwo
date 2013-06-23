using System;
using System.Threading.Tasks;
using System.Windows;

using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using InTwo.Model;
using ScoreoidPortable;
using ScoreoidPortable.Entities;
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
        private readonly IScoreoidClient _scoreoidClient;

        /// <summary>
        /// Initializes a new instance of the ScoresViewModel class.
        /// </summary>
        public ScoreoidViewModel(IExtendedNavigationService navigation, IScoreoidClient scoreoidClient)
        {
            _navigationService = navigation;
            _scoreoidClient = scoreoidClient;

            if (IsInDesignMode)
            {
                CurrentPlayer = new Player
                {
                    Username = "scottisafool",
                    BestScore = 336,
                    Rank = 1
                };
            }
        }

        public override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessageAction<bool>>(this, async m =>
            {
                if (m.Notification.Equals(Constants.Messages.SubmitScoreMsg))
                {
                    var score = (Score) m.Sender;

                    var result = await SubmitScore(score);

                    m.Execute(result);
                }
            });
        }

        private async Task<bool> SubmitScore(Score score)
        {
            try
            {
                if (score == null)
                {
                    Log.Info("Score is null, nothing to submit");
                    return false;
                }

                Log.Info("Submitting score of [{0}] for user [{1}]", score.TheScore, App.CurrentPlayer.Username);

                var response = await _scoreoidClient.CreateScoreAsync(App.CurrentPlayer.Username, score);

                if (response)
                {
                    Log.Info("Score submitted");

                    score.CreatedDate = DateTime.Now;

                    App.SettingsWrapper.AppSettings.MostRecentScore = score;

                    Messenger.Default.Send(new NotificationMessage(Constants.Messages.NewGameMsg));

                    Messenger.Default.Send(new NotificationMessage(Constants.Messages.ForceSettingsSaveMsg));

                    return true;
                }

                return false;
            }
            catch (ScoreoidException sex)
            {
                Log.InfoException("Error submitting score to scoreoid", sex);
            }
            catch (Exception ex)
            {
                Log.FatalException("SubmitScore", ex);
            }
            return false;
        }

        public Player CurrentPlayer { get; set; }
        
        public string Username { get; set; }
        public string Password { get; set; }

        public bool CanLogIn
        {
            get { return !string.IsNullOrEmpty(Username) && !ProgressIsVisible; }
            //get { return true; }
        }
        
        #region Commands

        #region Page Loaded Commands
        public RelayCommand CreateNewUserPageLoaded
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Log.Info("CreateNewUserPageLoaded");
                    CurrentPlayer = new Player();
                });
            }
        }
        public RelayCommand EditUserPageLoaded
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Log.Info("EditUserPageLoaded");
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
                        || string.IsNullOrEmpty(CurrentPlayer.Username)) return;

                    if (!_navigationService.IsNetworkAvailable) return;

                    try
                    {
                        SetProgressBar("Creating user...");

                        Log.Info("Submitting user [{0}] to be created.", CurrentPlayer.Username);

                        await _scoreoidClient.CreatePlayerAsync(CurrentPlayer);

                        Log.Info("Player submitted successfully");

                        App.CurrentPlayer = CurrentPlayer;

                        MessageBox.Show("Player created successfully, you can now sign in with this Username.", "Success", MessageBoxButton.OK);

                        _navigationService.GoBack();
                    }
                    catch (ScoreoidException ex)
                    {
                        Log.Info("There was an error submitting this player");
                        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
                    }
                    catch (Exception ex)
                    {
                        Log.FatalException("CreateNewUserCommand", ex);
                        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
                    }

                    SetProgressBar();
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
                        || string.IsNullOrEmpty(CurrentPlayer.Username)) return;

                    if (!_navigationService.IsNetworkAvailable) return;

                    try
                    {
                        SetProgressBar("Updating user...");

                        Log.Info("Updating details for user [{0}].", CurrentPlayer.Username);

                        var response = await _scoreoidClient.EditPlayerAsync(CurrentPlayer);

                        if (response)
                        {
                            Log.Info("User details updated successfully");

                            App.CurrentPlayer = CurrentPlayer;
                        }
                        else
                        {
                            Log.Info("There was an error editing this player");
                        }
                    }
                    catch (ScoreoidException ex)
                    {
                        Log.Info("There was an error editing this player"); 
                        MessageBox.Show("There was an error editing this player " + ex.Message, "Error", MessageBoxButton.OK);
                    }
                    catch (Exception ex)
                    {
                        Log.ErrorException("There was an error editing this player", ex);
                        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
                    }

                    SetProgressBar();
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

                    SetProgressBar("Signing in...");

                    try
                    {
                        Log.Info("Signing in as user [{0}]", Username);

                        var response = await _scoreoidClient.SignInAsync(Username, Password);

                        if (!response)
                        {
                            Log.Info("Not signed in as [{0}]", Username);
                            return;
                        }

                        Log.Info("Successfully signed in as [{0}]", Username);

                        var player = await _scoreoidClient.GetPlayerAsync(Username);

                        App.CurrentPlayer = player;

                        SetProgressBar();

                        Messenger.Default.Send(new NotificationMessage(Constants.Messages.RefreshCurrentPlayerInfoMsg));

                        _navigationService.GoBack();
                    }
                    catch (ScoreoidException ex)
                    {
                        Log.InfoException("There was an error signing in as this player", ex);
                        MessageBox.Show("There was an error signing in as this player.", "Error", MessageBoxButton.OK);
                    }
                    catch (Exception ex)
                    {
                        Log.FatalException("There was an error signing in as this player", ex);
                        MessageBox.Show("There was an error signing in as this player.", "Error", MessageBoxButton.OK);
                    }

                    SetProgressBar();
                });
            }
        }

        public RelayCommand CancelCreationCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Log.Info("Creation of new user cancelled.");
                    CurrentPlayer = null;
                    if (_navigationService.CanGoBack)
                        _navigationService.GoBack();
                });
            }
        }

        #endregion
    }
}