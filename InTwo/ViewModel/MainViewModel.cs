using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using InTwo.Model;
using Microsoft.Phone.Controls;
using Newtonsoft.Json;
using Nokia.Music.Types;

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

        private bool _hasCheckedForData;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IExtendedNavigationService navigationService, IAsyncStorageService asyncStorageService, IApplicationSettingsService settingsService)
        {
            _navigationService = navigationService;
            _asyncStorageService = asyncStorageService;
            _settingsService = settingsService;
        }

        public string ProgressText { get; set; }
        public bool ProgressIsVisible { get; set; }

        private async Task<bool> CheckForGameData()
        {
            var genres = _settingsService.Get("Genres", default(List<Genre>));

            if (genres == default(List<Genre>))
            {
                return false;
            }

            genres.Insert(0, new Genre {Name = GameViewModel.AllGenres});

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

        public RelayCommand MainPageLoaded
        {
            get
            {
                return new RelayCommand(async () =>
                                                  {
                                                      if (_hasCheckedForData) return;

                                                      var dataExists = await CheckForGameData();

                                                      if (!dataExists)
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
                                                                                           ((CustomMessageBox) sender).Dismissing += (o, eventArgs) => eventArgs.Cancel = true;

                                                                                           _navigationService.NavigateTo(Constants.Pages.DownloadingSongs);
                                                                                       }
                                                                                   };
                                                          message.Show();
                                                      }

                                                      _hasCheckedForData = true;
                                                  });
            }
        }

        public RelayCommand<string> NavigateToPage
        {
            get
            {
                return new RelayCommand<string>(_navigationService.NavigateTo);
            }
        }
    }
}