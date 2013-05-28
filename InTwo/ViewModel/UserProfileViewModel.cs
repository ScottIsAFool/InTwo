﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using InTwo.Model;
using Microsoft.Phone.Tasks;
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
        private readonly IExtendedNavigationService _navigationService;
        private readonly ScoreoidClient _scoreoidClient;
        private readonly IPhotoChooserService _photoChooserService;
        private readonly IAsyncStorageService _asyncStorageService;

        /// <summary>
        /// Initializes a new instance of the UserProfileViewModel class.
        /// </summary>
        public UserProfileViewModel(IExtendedNavigationService navigationService, ScoreoidClient scoreoidClient, IPhotoChooserService photoChooserService, IAsyncStorageService asyncStorageService)
        {
            _navigationService = navigationService;
            _scoreoidClient = scoreoidClient;
            _photoChooserService = photoChooserService;
            _asyncStorageService = asyncStorageService;

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
                                                if (!_navigationService.IsNetworkAvailable) return;

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

                    if (!_navigationService.IsNetworkAvailable) return;

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

        public RelayCommand ChooseUserProfilePictureCommand
        {
            get
            {
                return new RelayCommand(async () =>
                                                  {
                                                      var photoResult = await _photoChooserService.ShowAsync(true);

                                                      if (photoResult.TaskResult == TaskResult.OK && photoResult.ChosenPhoto != null)
                                                      {
                                                          // Save the image to isolated storage
                                                          var fileName = string.Format(Constants.ProfilePictureUri, App.CurrentPlayer.username).Replace("isostore:", "");

                                                          if (!await _asyncStorageService.DirectoryExistsAsync("ProfilePictures"))
                                                          {
                                                              await _asyncStorageService.CreateDirectoryAsync("ProfilePictures");
                                                          }

                                                          if (await _asyncStorageService.FileExistsAsync(fileName))
                                                          {
                                                              await _asyncStorageService.DeleteFileAsync(fileName);
                                                          }

                                                          using (var file = await _asyncStorageService.CreateFileAsync(fileName))
                                                          {
                                                              var bitmap = new BitmapImage();
                                                              bitmap.SetSource(photoResult.ChosenPhoto);

                                                              var writeableBitmap = new WriteableBitmap(bitmap);
                                                              writeableBitmap.SaveJpeg(file, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight, 0, 85);
                                                          }

                                                          Messenger.Default.Send(new NotificationMessage(Constants.RefreshCurrentPlayerMsg));

                                                          RaisePropertyChanged(() => CurrentPlayer);
                                                      }
                                                  });
            }
        }

        public RelayCommand LogoutCommand
        {
            get
            {
                return new RelayCommand(() =>
                                            {
                                                App.CurrentPlayer = null;

                                                _navigationService.NavigateTo(Constants.Pages.MainPage + Constants.ClearBackStack);

                                                Messenger.Default.Send(new NotificationMessage(Constants.RefreshCurrentPlayerMsg));
                                            });
            }
        }
        #endregion

        private async Task GetPlayerInformation()
        {
            if (!_navigationService.IsNetworkAvailable) return;

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
                    int totalScore = 0;
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