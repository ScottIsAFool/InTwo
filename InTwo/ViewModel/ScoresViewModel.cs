using System;
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
    public class ScoresViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly ScoreoidClient _scoreoidClient;
        /// <summary>
        /// Initializes a new instance of the ScoresViewModel class.
        /// </summary>
        public ScoresViewModel(INavigationService navigation, ScoreoidClient scoreoidClient)
        {
            _navigationService = navigation;
            _scoreoidClient = scoreoidClient;


        }

        public string ProgressText { get; set; }
        public bool ProgressIsVisible { get; set; }

        public player SelectedPlayer { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }

        #region Commands

        #region Page Loaded Commands
        public RelayCommand CreateNewUserPageLoaded
        {
            get
            {
                return new RelayCommand(() =>
                                            {
                                                SelectedPlayer = new player();
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
                                                      if (SelectedPlayer == null
                                                          || string.IsNullOrEmpty(SelectedPlayer.username)) return;

                                                      try
                                                      {
                                                          ProgressIsVisible = true;
                                                          ProgressText = "Creating user";

                                                          var response = await _scoreoidClient.CreatePlayerAsync(SelectedPlayer);

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

        public RelayCommand UpdateUserCommand
        {
            get
            {
                return new RelayCommand(async () =>
                    {
                        if (SelectedPlayer == null
                            || string.IsNullOrEmpty(SelectedPlayer.username)) return;

                        try
                        {
                            ProgressIsVisible = true;
                            ProgressText = "Creating user";

                            //var response = await _scoreoidClient.CreatePlayerAsync(SelectedPlayer);

                            //MessageBox.Show(response, "Success", MessageBoxButton.OK);
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
                        ProgressIsVisible = true;
                        ProgressText = "Logging in...";

                        try
                        {
                            var response = await _scoreoidClient.GetPlayerAsync(Username, Password);

                            SelectedPlayer = response.items[0];

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

        #endregion
    }
}