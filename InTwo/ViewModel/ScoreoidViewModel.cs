﻿using System;
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
    public class ScoreoidViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly ScoreoidClient _scoreoidClient;

        private player _previousPlayer;
        /// <summary>
        /// Initializes a new instance of the ScoresViewModel class.
        /// </summary>
        public ScoreoidViewModel(INavigationService navigation, ScoreoidClient scoreoidClient)
        {
            _navigationService = navigation;
            _scoreoidClient = scoreoidClient;
        }

        public string ProgressText { get; set; }
        public bool ProgressIsVisible { get; set; }

        public player CurrentPlayer { get; set; }
        
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
                                                CurrentPlayer = new player();
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

                                                      try
                                                      {
                                                          ProgressIsVisible = true;
                                                          ProgressText = "Creating user";

                                                          var response = await _scoreoidClient.CreatePlayerAsync(CurrentPlayer);

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

        public RelayCommand UpdateUserCommand
        {
            get
            {
                return new RelayCommand(async () =>
                    {
                        if (CurrentPlayer == null
                            || string.IsNullOrEmpty(CurrentPlayer.username)) return;

                        try
                        {
                            ProgressIsVisible = true;
                            ProgressText = "Creating user";

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

        public RelayCommand SignInCommand
        {
            get
            {
                return new RelayCommand(async () =>
                    {
                        if (string.IsNullOrEmpty(Username)) return;

                        ProgressIsVisible = true;
                        ProgressText = "Signing in...";

                        try
                        {
                            var response = await _scoreoidClient.GetPlayerAsync(Username, Password);

                            App.CurrentPlayer = response.items[0];
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