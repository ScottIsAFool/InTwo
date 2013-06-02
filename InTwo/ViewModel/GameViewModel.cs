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

                GameLocked = true;

                GameTrack = new Product
                                {
                                    Name = "I don't wanna miss a thing",
                                    Performers = new[] { new Artist { Name = "Aerosmith", Thumb320Uri = new Uri("http://assets.ent.nokia.com/p/d/music_image/320x320/1470.jpg") } },
                                    Thumb320Uri = new Uri("http://4.musicimg.ovi.com/u/1.0/image/156920531/?w=320&q=90")
                                };
                ArtistImage = GameTrack.Thumb320Uri;
                MaximumRoundPoints = 300;
            }

            GameLength = TimeSpan.FromSeconds(2);
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
        public Uri ArtistImage { get; set; }
        public TimeSpan CurrentPosition { get; set; }
        public TimeSpan GameLength { get; set; }

        public bool GameLocked { get; set; }
        public bool CanShowAnswers { get; set; }
        public string ArtistGuess { get; set; }
        public string SongGuess { get; set; }
        public int MaximumRoundPoints { get; set; }

        private void OnArtistGuessChange()
        {
            CalculateAvailableScore();
        }
        
        private void OnSongGuessChange()
        {
            CalculateAvailableScore();
        }

        private void OnCurrentPositionChanged()
        {
            // TODO: Check what game they're playing and stop the audio when it hits that limit.
            if (CurrentPosition > GameLength)
            {
                AudioUrl = null;
            }
        }

        public int AppBarIndex { get { return GameLocked ? 1 : 0; } }

        private void SetNextRound()
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

        private void CalculateAvailableScore()
        {
            var score = 0;

            if (!string.IsNullOrEmpty(ArtistGuess))
            {
                score += Constants.Scores.CorrectArtist;
            }

            if (!string.IsNullOrEmpty(SongGuess))
            {
                score += Constants.Scores.CorrectSong;
            }

            if (!string.IsNullOrEmpty(SongGuess) && !string.IsNullOrEmpty(ArtistGuess))
            {
                score += Constants.Scores.CorrectSongAndArtistBonus;
            }

            AdjustScoreForGameLength(score);

            MaximumRoundPoints = score;
        }

        private int AdjustScoreForGameLength(int score)
        {
            var seconds = GameLength.Seconds;

            if (seconds <= 5)
            {
            }
            else if (seconds <= 10)
            {
                score = (int) Math.Floor(score*0.95);
            }
            else if (seconds <= 15)
            {
                score = (int) Math.Floor(score*0.9);
            }
            else if (seconds <= 20)
            {
                score = (int) Math.Floor(score*0.8);
            }
            else if (seconds <= 25)
            {
                score = (int) Math.Floor(score*0.75);
            }
            else if (seconds <= 30)
            {
                score = (int) Math.Floor(score*0.5);
            }
            return score;
        }

        public RelayCommand SubmitGuessCommand
        {
            get
            {
                return new RelayCommand(() =>
                                        {
                                            if (CheckAnswers())
                                            {

                                            }
                                        });
            }
        }

        private bool CheckAnswers()
        {


            return false;
        }

        public RelayCommand StartGameCommand
        {
            get
            {
                return new RelayCommand(() =>
                                        {
                                            GameLocked = true;

                                            SetNextRound();
                                        });
            }
        }

        public RelayCommand NextRoundCommand
        {
            get
            {
                return new RelayCommand(SetNextRound);
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