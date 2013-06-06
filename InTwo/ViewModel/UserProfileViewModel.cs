using System;
using System.IO;
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
        
        public override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, async m =>
            {
                if (m.Notification.Equals(Constants.Messages.RefreshCurrentPlayerInfoMsg))
                {
                    if (_navigationService.IsNetworkAvailableSilent) return;

                    await GetPlayerInformation();

                    Messenger.Default.Send(new NotificationMessage(Constants.Messages.UpdatePrimaryTileMsg));
                }
            });
        }

        public player CurrentPlayer { get; set; }
        public int TotalScore { get; set; }
        public int NumberOfGames { get; set; }

        public bool HasProfilePicture { get; set; }

        #region Commands
        public RelayCommand UserProfilePageLoaded
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    //if (!_navigationService.IsNetworkAvailable) return;

                    CurrentPlayer = App.CurrentPlayer;

                    HasProfilePicture = await CheckForProfilePicture();
                    //await GetPlayerInformation();
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
                    if (!_navigationService.IsNetworkAvailable) return;

                    ProgressIsVisible = true;
                    ProgressText = "Getting latest information...";

                    await GetPlayerInformation();

                    ProgressIsVisible = false;
                    ProgressText = string.Empty;
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
                        await _scoreoidClient.DeletePlayerAsync(App.SettingsWrapper.AppSettings.PlayerWrapper.CurrentPlayer);

                        App.SettingsWrapper.AppSettings.PlayerWrapper = null;

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
                        var fileName = string.Format(Constants.ProfilePictureStorageFilePath, App.CurrentPlayer.username);

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

                            await CreateTileImages(bitmap);
                        }


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
                    var fileName = string.Format(Constants.ProfilePictureStorageFilePath, App.CurrentPlayer.username);

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
                    App.CurrentPlayer = null;

                    _navigationService.NavigateTo(Constants.Pages.MainPage + Constants.ClearBackStack);

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
                var items = await _scoreoidClient.GetPlayerAsync(CurrentPlayer.username);
                CurrentPlayer = items.items[0];

                var scores = await _scoreoidClient.GetPlayerScores(CurrentPlayer.username);

                NumberOfGames = scores.items.Any() ? scores.items.Length : 0;

                CurrentPlayer.boost = NumberOfGames.ToString();

                if (scores.items.Any())
                {
                    var totalScore = 0;
                    foreach (var score in scores.items)
                    {
                        int count;
                        if (int.TryParse(score.value, out count))
                        {
                            totalScore += count;
                        }

                        if (score.ToString() == CurrentPlayer.best_score)
                        {
                            CurrentPlayer.last_level = score.difficulty;
                        }
                    }
                    TotalScore = totalScore;

                    CurrentPlayer.bonus = TotalScore.ToString();
                }
            }
            catch (ScoreoidException ex)
            {

            }
            catch (Exception ex)
            {
                
            }
        }

        private async Task<bool> CheckForProfilePicture()
        {
            if (App.CurrentPlayer == null) return false;
            var fileName = string.Format(Constants.ProfilePictureStorageFilePath, App.CurrentPlayer.username);

            return await _asyncStorageService.FileExistsAsync(fileName);
        }
        
        private async Task CreateTileImages(ImageSource image)
        {
            var normalFileName = string.Format(Constants.Tiles.UserProfileFileFormat, CurrentPlayer.username);
            var wideFileName = string.Format(Constants.Tiles.UserProfileWideFileFormat, CurrentPlayer.username);

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

        private static UIElement CreateProfileTileImages(ImageSource theImage, int height, int width)
        {
            var grid = new Grid
            {
                Height = height,
                Width = width
            };

            var image = new Image
            {
                Source = theImage,
                Stretch = Stretch.Fill,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Width = width,
                Height = height
            };
            grid.Children.Add(image);

            return grid;
        }
    }
}