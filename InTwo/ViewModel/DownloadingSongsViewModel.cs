using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anotar.MetroLog;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using InTwo.Model;
using Newtonsoft.Json;
using Nokia.Music;
using Nokia.Music.Types;
using ScottIsAFool.WindowsPhone.ViewModel;

namespace InTwo.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class DownloadingSongsViewModel : ViewModelBase
    {
        private readonly IExtendedNavigationService _navigationService;
        private readonly IMusicClient _musicClient;
        private readonly IAsyncStorageService _asyncStorageService;
        private readonly IApplicationSettingsService _settingsService;
        /// <summary>
        /// Initializes a new instance of the DownloadingSongsViewModel class.
        /// </summary>
        public DownloadingSongsViewModel(IExtendedNavigationService navigationService, IMusicClient musicClient, IAsyncStorageService asyncStorageService, IApplicationSettingsService settingsService)
        {
            _navigationService = navigationService;
            _musicClient = musicClient;
            _asyncStorageService = asyncStorageService;
            _settingsService = settingsService;
        }

        public bool RetryIsVisible { get; set; }

        public RelayCommand DownloadDataCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    Log.Info("Downloading game data");
                    await DownloadData();
                });
            }
        }

        private async Task DownloadData()
        {
            if (!_navigationService.IsNetworkAvailable)
            {
                RetryIsVisible = true;

                return;
            }

            RetryIsVisible = false;

            try
            {
                ProgressIsVisible = true;
                
                var genresResponse = await _musicClient.GetGenresAsync();

                if (genresResponse.Error != null || (genresResponse.Result == null))
                {
                    Log.ErrorException("Error getting genres", genresResponse.Error);
                    // TODO: Display an error
                    RetryIsVisible = true;
                    ProgressIsVisible = false;

                    return;
                }

                var genres = genresResponse.Result;

                _settingsService.Set("Genres", genres);
                _settingsService.Save();

                Log.Info("Genres saved");

                var tracks = new List<Product>();
                foreach (var genre in genres.Where(x => !x.Name.Equals("Comedy")))
                {
                    var trackResponse = await _musicClient.GetTopProductsForGenreAsync(genre, Category.Track, 0, 100);

                    if (trackResponse.Error == null)
                    {
                        tracks.AddRange(trackResponse.Result);
                    }
                }

                await _asyncStorageService.WriteAllTextAsync(Constants.GameDataFile, await JsonConvert.SerializeObjectAsync(tracks));

                Log.Info("Tracks written to IsolatedStorage");

                _navigationService.NavigateTo(Constants.Pages.MainPage + Constants.ClearBackStack);
            }
            catch (Exception ex)
            {
                Log.ErrorException("Error download data", ex);
            }

            ProgressIsVisible = false;
        }
    }
}