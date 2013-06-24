using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using InTwo.Model;
using Microsoft.Phone.Tasks;
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
    public class UserProfileViewModel : ViewModelBase
    {
        private readonly IExtendedNavigationService _navigationService;
        private readonly IScoreoidClient _scoreoidClient;
        private readonly IPhotoChooserService _photoChooserService;
        private readonly IAsyncStorageService _asyncStorageService;

        /// <summary>
        /// Initializes a new instance of the UserProfileViewModel class.
        /// </summary>
        public UserProfileViewModel(IExtendedNavigationService navigationService, IScoreoidClient scoreoidClient, IPhotoChooserService photoChooserService, IAsyncStorageService asyncStorageService)
        {
            _navigationService = navigationService;
            _scoreoidClient = scoreoidClient;
            _photoChooserService = photoChooserService;
            _asyncStorageService = asyncStorageService;

            if (IsInDesignMode)
            {
                CurrentPlayer = new Player
                {
                    Username = "scottisafool",
                    BestScore = 336,
                    Rank = 1
                };
            }

            TotalScore = NumberOfGames = 0;
        }

        public override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, async m =>
            {
                if (m.Notification.Equals(Constants.Messages.RefreshCurrentPlayerInfoMsg))
                {
                    if (!_navigationService.IsNetworkAvailableSilent) return;

                    CurrentPlayer = App.CurrentPlayer;

                    await GetPlayerInformation();

                    Messenger.Default.Send(new NotificationMessage(Constants.Messages.UpdatePrimaryTileMsg));
                }
            });
        }

        public Player CurrentPlayer { get; set; }
        public int TotalScore { get; set; }
        public int NumberOfGames { get; set; }

        public bool HasProfilePicture { get; set; }

        private void NavigateTo(string link)
        {
            Log.Info("Navigating to " + link);
            _navigationService.NavigateTo(link);
        }

        #region Commands
        public RelayCommand UserProfilePageLoaded
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    Log.Info("USerProfilePageLoaded");
                    CurrentPlayer = App.CurrentPlayer;

                    HasProfilePicture = await CheckForProfilePicture();
                });
            }
        }

        public RelayCommand EditUserCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Log.Info("Editing player details");
                    CurrentPlayer = App.CurrentPlayer;
                    NavigateTo(Constants.Pages.Scoreoid.EditUser);
                });
            }
        }

        public RelayCommand RefreshUserCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (!_navigationService.IsNetworkAvailable) return;

                    SetProgressBar("Getting latest information...");

                    Log.Info("Refreshing player details");

                    CurrentPlayer = App.CurrentPlayer;

                    await GetPlayerInformation();

                    SetProgressBar();
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
                        Log.Info("Deleting user");
                        await _scoreoidClient.DeletePlayerAsync(App.SettingsWrapper.AppSettings.PlayerWrapper.CurrentPlayer.Username);

                        App.SettingsWrapper.AppSettings.PlayerWrapper = null;

                        NavigateTo(Constants.Pages.MainPage + Constants.ClearBackStack);
                    }
                    catch (ScoreoidException ex)
                    {
                        MessageBox.Show("There was sadly an error deleting you, a sign you shouldn't go?", "Error", MessageBoxButton.OK);
                        Log.InfoException("Error deleting the user.", ex);
                    }
                    catch (Exception ex)
                    {
                        Log.FatalException("DeleteUserCommand", ex);
                        MessageBox.Show("There was sadly an error deleting you, a sign you shouldn't go?", "Error", MessageBoxButton.OK);
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
                    Log.Info("Choosing a new profile picture");

                    var photoResult = await _photoChooserService.ShowAsync(true);

                    if (photoResult.TaskResult == TaskResult.OK && photoResult.ChosenPhoto != null)
                    {
                        // Save the image to isolated storage
                        var fileName = string.Format(Constants.ProfilePictureStorageFilePath, App.CurrentPlayer.Username);

                        if (!await _asyncStorageService.DirectoryExistsAsync(Constants.ProfilePicturesFolder))
                        {
                            await _asyncStorageService.CreateDirectoryAsync(Constants.ProfilePicturesFolder);
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

                            await CreateTileImages(writeableBitmap);
                        }

                        FlurryWP8SDK.Api.LogEvent("UserPhotoCreated");

                        // Tell other UI references to update their profile image
                        Messenger.Default.Send(new NotificationMessage(Constants.Messages.RefreshCurrentPlayerMsg));

                        RaisePropertyChanged(() => CurrentPlayer);
                        HasProfilePicture = await CheckForProfilePicture();
                    }
                });
            }
        }

        public RelayCommand ClearProfilePicture
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    Log.Info("Clearing profile picture");

                    var fileName = string.Format(Constants.ProfilePictureStorageFilePath, App.CurrentPlayer.Username);

                    if (!(await _asyncStorageService.FileExistsAsync(fileName))) return;

                    await _asyncStorageService.DeleteFileAsync(fileName);

                    HasProfilePicture = await CheckForProfilePicture();

                    // Tell other UI references to update their profile image
                    Messenger.Default.Send(new NotificationMessage(Constants.Messages.RefreshCurrentPlayerMsg));
                });
            }
        }

        public RelayCommand LogoutCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Log.Info("Signing out.");
                    App.CurrentPlayer = null;

                    NavigateTo(Constants.Pages.MainPage + Constants.ClearBackStack);

                    Messenger.Default.Send(new NotificationMessage(Constants.Messages.RefreshCurrentPlayerMsg));

                    TileService.Current.ClearTile();
                });
            }
        }
        #endregion

        private async Task GetPlayerInformation()
        {
            try
            {
                Log.Info("Getting all player's details for user [{0}]", CurrentPlayer.Username);

                var items = await _scoreoidClient.GetPlayerAsync(CurrentPlayer.Username);
                CurrentPlayer = items;

                Log.Info("Getting all player's scores for user [{0}]", CurrentPlayer.Username);

                var scores = await _scoreoidClient.GetPlayerScoresAsync(CurrentPlayer.Username);

                NumberOfGames = scores.Any() ? scores.Count : 0;

                CurrentPlayer.Boost = NumberOfGames.ToString(CultureInfo.InvariantCulture);

                if (scores.Any())
                {
                    var totalScore = 0;
                    foreach (var score in scores)
                    {
                        int count;
                        if (int.TryParse(score.TheScore, out count))
                        {
                            totalScore += count;
                        }

                        if (score.TheScore == CurrentPlayer.BestScore.ToString(CultureInfo.InvariantCulture))
                        {
                            CurrentPlayer.LastLevel = score.Data;
                        }
                    }
                    TotalScore = totalScore;

                    CurrentPlayer.Bonus = TotalScore;
                }
            }
            catch (ScoreoidException ex)
            {
                Log.InfoException("Error getting user details", ex);
            }
            catch (Exception ex)
            {
                Log.FatalException("GetPlayerInformation", ex);
            }
        }

        private async Task<bool> CheckForProfilePicture()
        {
            if (App.CurrentPlayer == null) return false;
            var fileName = string.Format(Constants.ProfilePictureStorageFilePath, App.CurrentPlayer.Username);

            return await _asyncStorageService.FileExistsAsync(fileName);
        }

        private async Task CreateTileImages(WriteableBitmap image)
        {
            Log.Info("Creating tile images from user profile images");

            var normalFileName = string.Format(Constants.Tiles.UserProfileFileFormat, CurrentPlayer.Username);
            var wideFileName = string.Format(Constants.Tiles.UserProfileWideFileFormat, CurrentPlayer.Username);

            if (await _asyncStorageService.FileExistsAsync(normalFileName))
            {
                await _asyncStorageService.DeleteFileAsync(normalFileName);
            }

            if (await _asyncStorageService.FileExistsAsync(wideFileName))
            {
                await _asyncStorageService.DeleteFileAsync(wideFileName);
            }

            var normalTile = CreateProfileTileImages(image, 336, 336);

            var wideTile = CreateProfileTileImages(image, 336, 691);

            await Utils.SaveTile(normalTile, 336, 336, normalFileName);

            await Utils.SaveTile(wideTile, 336, 691, wideFileName);

            Messenger.Default.Send(new NotificationMessage(Constants.Messages.UpdatePrimaryTileMsg));
        }

        private static UIElement CreateProfileTileImages(WriteableBitmap image, int height, int width)
        {
            var resizedImage = image.PixelWidth < image.PixelHeight
                                               ? image.Resize(691, (image.PixelHeight*691/image.PixelWidth), WriteableBitmapExtensions.Interpolation.Bilinear)
                                               : image.Resize((image.PixelWidth*691/image.PixelHeight), 691, WriteableBitmapExtensions.Interpolation.Bilinear);

            if (width > height)
            {
                var top = resizedImage.PixelHeight > resizedImage.PixelWidth
// ReSharper disable PossibleLossOfFraction
                              ? (int)Math.Floor((double)((resizedImage.PixelHeight - 336)/2))
// ReSharper restore PossibleLossOfFraction
                              : 0;
                var left = resizedImage.PixelWidth > resizedImage.PixelHeight
// ReSharper disable PossibleLossOfFraction
                               ? (int)Math.Floor((double) ((resizedImage.PixelWidth - 691)/2))
// ReSharper restore PossibleLossOfFraction
                               : 0;
                resizedImage = resizedImage.Crop(left, top, width, height);
            }

            var grid = new Grid
            {
                Height = height,
                Width = width
            };

            var theImage = new Image
            {
                Source = resizedImage,
                Stretch = Stretch.Fill,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Width = width,
                Height = height
            };
            grid.Children.Add(theImage);

            return grid;
        }
    }
}