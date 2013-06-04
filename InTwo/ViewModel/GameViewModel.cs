using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using InTwo.Controls;
using InTwo.Model;
using Microsoft.Phone.Controls;
using Nokia.Music.Types;
using ScottIsAFool.WindowsPhone.ViewModel;
using Windows.Phone.Speech.Recognition;
using Windows.System;

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
        private readonly DispatcherTimer _gameTimer;

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
                    Performers = new[] {new Artist {Name = "Aerosmith", Thumb320Uri = new Uri("http://assets.ent.nokia.com/p/d/music_image/320x320/1470.jpg")}},
                    Thumb320Uri = new Uri("http://4.musicimg.ovi.com/u/1.0/image/156920531/?w=320&q=90")
                };
                ArtistImage = GameTrack.Thumb320Uri;
                MaximumRoundPoints = 300;
            }
            else
            {
                GameLength = TimeSpan.FromSeconds(2);
                _gameTimer = new DispatcherTimer { Interval = GameLength };
                _gameTimer.Tick += GameTimerOnTick;
            }

        }

        private void GameTimerOnTick(object sender, EventArgs eventArgs)
        {
            AudioUrl = null;

            if (_gameTimer.IsEnabled)
            {
                _gameTimer.Stop();
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
                if (m.Notification.Equals(Constants.Messages.IsPlayingMsg))
                {
                    IsPlaying = (bool) m.Sender;

                    if (_gameTimer.IsEnabled)
                    {
                        _gameTimer.Stop();
                    }

                    if (IsPlaying)
                    {
                        _gameTimer.Start();
                    }
                }
            });
        }

        public List<Genre> Genres { get; set; }
        public Genre SelectedGenre { get; set; }
        public Product GameTrack { get; set; }
        public Uri AudioUrl { get; set; }
        public Uri ArtistImage { get; set; }
        public TimeSpan GameLength { get; set; }
        public bool IsPlaying { get; set; }

        public bool GameLocked { get; set; }
        public bool CanShowAnswers { get; set; }
        public string ArtistGuess { get; set; }
        public string SongGuess { get; set; }
        public int MaximumRoundPoints { get; set; }
        public int RoundPoints { get; set; }
        public int RoundNumber { get; set; }

        public string AnotherRoundOrNot
        {
            get { return RoundNumber < Constants.MaximumNumberOfRounds ? "ready for another?" : "shall we submit your scores now?"; }
        }

        #region Property Changed Methods
        private void OnArtistGuessChanged()
        {
            CalculateAvailableScore();
        }
        
        private void OnSongGuessChanged()
        {
            CalculateAvailableScore();
        }

        private void OnGameLengthChanged()
        {
            _gameTimer.Interval = GameLength;
        }
        #endregion

        public int AppBarIndex
        {
            get
            {
                if (GameLocked)
                {
                    return CanShowAnswers ? 2 : 1;
                }
                return 0;
            }
        }

        private void SetNextRound()
        {
            if (!_navigationService.IsNetworkAvailable) return;

            ResetGameForNewRound();
            
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

        private void ResetGameForNewRound()
        {
            AudioUrl = ArtistImage = null;
            CanShowAnswers = false;
            ArtistGuess = string.Empty;
            SongGuess = string.Empty;
            RoundPoints = 0;

            CalculateAvailableScore();
        }

        #region Scoring methods
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

        private void AdjustScoreForGameLength(int score)
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
        }
        #endregion

        private bool CheckAnswers()
        {
            bool artistGuessCorrect;
            bool songGuessCorrect;

            Utils.SeeIfWeHaveAWinner(GameTrack, ArtistGuess, SongGuess, out artistGuessCorrect, out songGuessCorrect);

            if (artistGuessCorrect)
            {
                RoundPoints += Constants.Scores.CorrectArtist;
            }

            if (songGuessCorrect)
            {
                RoundPoints += Constants.Scores.CorrectSong;
            }

            if (artistGuessCorrect && songGuessCorrect)
            {
                RoundPoints += Constants.Scores.CorrectSongAndArtistBonus;
            }

            AdjustScoreForGameLength(RoundPoints);

            return (artistGuessCorrect || songGuessCorrect);
        }

        private async Task LaunchSpeech()
        {
            var speechRecognizer = new SpeechRecognizerUI();
            speechRecognizer.Settings.ExampleText = "Artist is Aerosmith";
            speechRecognizer.Settings.ListenText = "Make your guess";
            speechRecognizer.Settings.ReadoutEnabled = false;
            speechRecognizer.Settings.ShowConfirmation = false;
            var result = await speechRecognizer.RecognizeWithUIAsync();
            if (result.ResultStatus == SpeechRecognitionUIStatus.Succeeded)
            {
                //MessageBox.Show(result.RecognitionResult.Text);
            }
        }

        #region Commands
        public RelayCommand GamePageLoaded
        {
            get
            {
                return new RelayCommand(ResetGameForNewRound);
            }
        }

        public RelayCommand SubmitGuessCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (CheckAnswers())
                    {
                        var message = new CustomMessageBox
                            {
                                Title = "Congratulations!",
                                Message = string.Format("Well done, you score {0} points this round. Right, enough jibba jabba, {1}", RoundPoints, AnotherRoundOrNot),
                                LeftButtonContent = "yes please",
                                RightButtonContent = "nah, not yet"
                            };

                        message.Dismissed += (sender, args) =>
                        {
                            ((CustomMessageBox) sender).Dismissing += (o, eventArgs) => eventArgs.Cancel = true;
                            if (args.Result == CustomMessageBoxResult.LeftButton)
                            {
                                if (AnotherRoundOrNot.Equals("ready for another?"))
                                {
                                    RoundNumber++;
                                    SetNextRound();
                                }
                                else
                                {
                                    // TODO: Submit scores
                                }
                            }
                        };

                        message.Show();
                    }
                });
            }
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

        public RelayCommand ViewInNokiaMusicCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await Launcher.LaunchUriAsync(GameTrack.AppToAppUri);
                });
            }
        }

        public RelayCommand AudioGuessCommnd
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (!App.SettingsWrapper.AppSettings.DontShowSpeechGuessprompt)
                    {
                        var message = new CustomMessageBox
                        {
                            Content = new SpeechHelp(),
                            IsFullScreen = true,
                            LeftButtonContent = "Guess",
                            RightButtonContent = "Cancel"
                        };

                        message.Dismissed += async (sender, args) =>
                        {
                            if (args.Result == CustomMessageBoxResult.LeftButton)
                            {
                                await LaunchSpeech();
                            }
                        };

                        message.Show();
                    }
                    else
                    {
                        await LaunchSpeech();
                    }
                });
            }
        }
        #endregion
    }
}