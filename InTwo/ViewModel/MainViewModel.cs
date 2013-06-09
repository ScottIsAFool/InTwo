using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using InTwo.Model;
using MetroLog;
using Microsoft.Phone.Controls;
using Newtonsoft.Json;
using Nokia.Music.Types;
using ScottIsAFool.WindowsPhone.ViewModel;

namespace InTwo.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IExtendedNavigationService _navigationService;
        private readonly IAsyncStorageService _asyncStorageService;
        private readonly IApplicationSettingsService _settingsService;
        //private readonly ILogger _logger;

        private bool _hasCheckedForData;
        private bool _dataExists;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IExtendedNavigationService navigationService, IAsyncStorageService asyncStorageService, IApplicationSettingsService settingsService)
        {
            _navigationService = navigationService;
            _asyncStorageService = asyncStorageService;
            _settingsService = settingsService;
            //_logger = LogManagerFactory.DefaultLogManager.GetLogger<MainViewModel>();

            if (!IsInDesignMode)
            {
                //_logger.Info("In MainViewModel");
            }
        }

        public List<Genre> Genres { get; set; }

        public override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.UpdatePrimaryTileMsg))
                {
                    TileService.Current.UpdatePrimaryTile();
                }
            });
        }

        private async Task<bool> CheckForGameData()
        {
            var genres = _settingsService.Get("Genres", default(List<Genre>));

            if (genres == default(List<Genre>))
            {
                return false;
            }

            var allGenreCheck = genres.FirstOrDefault(x => x.Name.Equals(GameViewModel.AllGenres));
            if (allGenreCheck == default(Genre))
            {
                genres.Insert(0, new Genre {Name = GameViewModel.AllGenres});
            }

            var comedyGenreCheck = genres.FirstOrDefault(x => x.Name.Equals("Comedy"));
            if (comedyGenreCheck != default(Genre))
            {
                genres.Remove(comedyGenreCheck);
            }

            Genres = genres;

            App.SettingsWrapper.AppSettings.DefaultGenre = Genres.FirstOrDefault(x => x.Name.Equals(App.SettingsWrapper.AppSettings.DefaultGenre.Name));

            Messenger.Default.Send(new NotificationMessage(genres, Constants.Messages.HereAreTheGenresMsg));

            if (!await _asyncStorageService.FileExistsAsync(Constants.GameDataFile))
            {
                return false;
            }

            var tracksJson = await _asyncStorageService.ReadAllTextAsync(Constants.GameDataFile);

            try
            {
                var tracks = await JsonConvert.DeserializeObjectAsync<List<Product>>(tracksJson);

                if (tracks.Any())
                {
                    Messenger.Default.Send(new NotificationMessage(tracks, Constants.Messages.HereAreTheTracksMsg));

                    return true;
                }
            }
            catch
            {
                
            }

            return false;
        }

        private void DisplayGetDataMessage()
        {
            var message = new CustomMessageBox
            {
                Caption = "No game data present",
                Message = "We can't find any game data saved to your phone. " +
                          "This data needs to be downloaded in order for you to play, would you like us to download that now? " +
                          "Please note, this doesn't download any music.",
                LeftButtonContent = "yes",
                RightButtonContent = "no",
                IsFullScreen = false
            };

            message.Dismissed += (sender, args) =>
            {
                if (args.Result == CustomMessageBoxResult.LeftButton)
                {
                    ((CustomMessageBox)sender).Dismissing += (o, eventArgs) => eventArgs.Cancel = true;

                    _navigationService.NavigateTo(Constants.Pages.DownloadingSongs);
                }
            };
            message.Show();
        }

        #region Commands
        public RelayCommand MainPageLoaded
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (!App.SettingsWrapper.AppSettings.DontShowAllowStopMusicMessage)
                    {
                        DisplayStopAudioMessage();
                    }

                    if (_hasCheckedForData && _dataExists) return;

                    _dataExists = await CheckForGameData();

                    if (!_dataExists)
                    {
                        DisplayGetDataMessage();
                    }

                    _hasCheckedForData = true;

                    if (App.CurrentPlayer != null)
                        Messenger.Default.Send(new NotificationMessage(Constants.Messages.RefreshCurrentPlayerInfoMsg));
                });
            }
        }

        private void DisplayStopAudioMessage()
        {
            var message = new CustomMessageBox
            {
                Title = "Stop music",
                Message = "If you already have music playing, do we have your permission to stop your music in order to play the game?\n\nThis can be changed in the app settings.",
                LeftButtonContent = "yes",
                RightButtonContent = "no",
                Content = Utils.CreateDontShowCheckBox("DontShowAllowStopMusicMessage")
            };

            message.Dismissed += (sender, args) => App.SettingsWrapper.AppSettings.AllowStopMusic = args.Result == CustomMessageBoxResult.LeftButton;

            message.Show();
        }

        public RelayCommand<string> NavigateToPage
        {
            get
            {
                return new RelayCommand<string>(_navigationService.NavigateTo);
            }
        }

        public RelayCommand LoginLogoutCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (App.CurrentPlayer == null)
                    {
                        _navigationService.NavigateTo(Constants.Pages.Scoreoid.SignIn);
                    }
                    else
                    {
                        App.CurrentPlayer = null;
                    }
                });
            }
        }

        public RelayCommand GoToGameCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await Task.Run(() =>
                    {
#if !DEBUG
                        if (!_navigationService.IsNetworkAvailable) return;
#endif
                        if (_dataExists)
                        {
                            Deployment.Current.Dispatcher.BeginInvoke(()=>_navigationService.NavigateTo(Constants.Pages.Game));
                            return;
                        }
                        Deployment.Current.Dispatcher.BeginInvoke(() => DisplayGetDataMessage());
                    });
                });
            }
        }

        public RelayCommand GoToSettingsCommand
        {
            get
            {
                return new RelayCommand(() => _navigationService.NavigateTo(Constants.Pages.SettingsView));
            }
        }

        public RelayCommand RemoveAdsCommand
        {
            get
            {
                return new RelayCommand(() => _navigationService.NavigateTo(Constants.Pages.Removeads));
            }
        }

        public RelayCommand RefreshGameDataCommand
        {
            get
            {
                return new RelayCommand(() => _navigationService.NavigateTo(Constants.Pages.DownloadingSongs));
            }
        }
        #endregion
    }
}