using System;
using System.Collections.Generic;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using InTwo.Model;
using Newtonsoft.Json;
using Nokia.Music;
using Nokia.Music.Types;

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
        private readonly MusicClient _musicClient;
        private readonly IAsyncStorageService _asyncStorageService;
        private readonly IApplicationSettingsService _settingsService;
        /// <summary>
        /// Initializes a new instance of the DownloadingSongsViewModel class.
        /// </summary>
        public DownloadingSongsViewModel(IExtendedNavigationService navigationService, MusicClient musicClient, IAsyncStorageService asyncStorageService, IApplicationSettingsService settingsService)
        {
            _navigationService = navigationService;
            _musicClient = musicClient;
            _asyncStorageService = asyncStorageService;
            _settingsService = settingsService;
        }

        public RelayCommand DownloadSongsPageLoaded
        {
            get
            {
                return new RelayCommand(async () =>
                                                  {
                                                      if (!_navigationService.IsNetworkAvailable) return;

                                                      try
                                                      {
                                                          var genresResponse = await _musicClient.GetGenresAsync();

                                                          if (genresResponse.Error != null)
                                                          {
                                                              // TODO: Display an error
                                                          }

                                                          var genres = genresResponse.Result;

                                                          _settingsService.Set("Genres", genres);
                                                          _settingsService.Save();

                                                          var tracks = new List<Product>();
                                                          foreach (var genre in genres)
                                                          {
                                                              var trackResponse = await _musicClient.GetTopProductsForGenreAsync(genre, Category.Track, 0, 100);

                                                              if (trackResponse.Error == null)
                                                              {
                                                                  tracks.AddRange(trackResponse.Result);
                                                              }
                                                          }

                                                          await _asyncStorageService.WriteAllTextAsync(Constants.GameDataFile, await JsonConvert.SerializeObjectAsync(tracks));

                                                          _navigationService.NavigateTo(Constants.Pages.MainPage + Constants.ClearBackStack);
                                                      }
                                                      catch (Exception ex)
                                                      {
                                                          // TODO: Display an error
                                                      }
                                                  });
            }
        }
    }
}