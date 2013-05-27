﻿using System;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using InTwo.Model;
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
                CurrentPlayer = App.CurrentPlayer;
            }
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

                                                      if (!_navigationService.IsNetworkAvailable) return;

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

                        if (!_navigationService.IsNetworkAvailable) return;

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