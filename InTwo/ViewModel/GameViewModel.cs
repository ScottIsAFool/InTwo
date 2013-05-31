using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using InTwo.Model;
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
    public class GameViewModel : ViewModelBase
    {
        private readonly IExtendedNavigationService _navigationService;
        public const string AllGenres = "All Genres";
        private List<Product> _tracks;

        /// <summary>
        /// Initializes a new instance of the GameViewModel class.
        /// </summary>
        public GameViewModel(IExtendedNavigationService navigationService)
        {
            _navigationService = navigationService;
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                Genres = new List<Genre> {new Genre {Name = AllGenres}};
                SelectedGenre = Genres[0];
            }
        }

        public override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.HereAreTheGenresMsg))
                {
                    Genres = (List<Genre>) m.Sender;
                    SelectedGenre = Genres[0];
                }
                if (m.Notification.Equals(Constants.Messages.HereAreTheTracksMsg))
                {
                    _tracks = (List<Product>) m.Sender;
                }
            });
        }

        public List<Genre> Genres { get; set; }
        public Genre SelectedGenre { get; set; }
        public Product GameTrack { get; set; }
        public Uri AudioUrl { get; set; }
        public TimeSpan CurrentPosition { get; set; }

        private void OnCurrentPositionChanged()
        {
            // TODO: Check what game they're playing and stop the audio when it hits that limit.
        }

        public int AppBarIndex { get; set; }

        private void SetNextGame()
        {
            if (!_navigationService.IsNetworkAvailable) return;

            if (SelectedGenre.Name.Equals(AllGenres))
            {
                var randomNumber = Utils.GetRandomNumber(0, _tracks.Count);

                GameTrack = _tracks[randomNumber];
            }
            else
            {
                var genreTracks = _tracks.Where(x => x.Genres.Contains(SelectedGenre))
                                         .ToList();

                var randomNumber = Utils.GetRandomNumber(0, genreTracks.Count);

                GameTrack = genreTracks[randomNumber];
            }

            AudioUrl = GameTrack.GetSampleUri();
        }

        public RelayCommand NextGameCommand
        {
            get
            {
                return new RelayCommand(SetNextGame);
            }
        }

        public RelayCommand RepeatAudioCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    AudioUrl = GameTrack.GetSampleUri();
                });
            }
        }
    }
}