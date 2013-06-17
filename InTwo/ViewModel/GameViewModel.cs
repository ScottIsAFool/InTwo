using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Cimbalino.Phone.Toolkit.Services;
using FlurryWP8SDK.Models;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using InTwo.Controls;
using InTwo.Model;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Nokia.Music.Types;
using Scoreoid;
using ScottIsAFool.WindowsPhone.ViewModel;
using Windows.Phone.Speech.Recognition;
using Windows.System;
using Artist = Nokia.Music.Types.Artist;
using Genre = Nokia.Music.Types.Genre;

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
        private readonly IApplicationSettingsService _applicationSettings;
        public const string AllGenres = "All Genres";
        private List<Product> _tracks;
        private readonly DispatcherTimer _gameTimer;

        private bool _alreadyAskedAboutMusic;

        /// <summary>
        /// Initializes a new instance of the GameViewModel class.
        /// </summary>
        public GameViewModel(IExtendedNavigationService navigationService, IApplicationSettingsService applicationSettings)
        {
            _navigationService = navigationService;
            _applicationSettings = applicationSettings;
            
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                Genres = new List<Genre> {new Genre {Name = "Gock"}};
                SelectedGenre = Genres[0];

                GameLocked = false;

                GameTrack = new Product
                {
                    Name = "I don't wanna miss a thing",
                    Performers = new[] {new Artist {Name = "Aerosmith", Thumb320Uri = new Uri("http://assets.ent.nokia.com/p/d/music_image/320x320/1470.jpg")}},
                    Thumb320Uri = new Uri("http://4.musicimg.ovi.com/u/1.0/image/156920531/?w=320&q=90")
                };
                MaximumRoundPoints = 300;
            }
            else
            {
                _gameTimer = new DispatcherTimer();
                
                GameLength = TimeSpan.FromSeconds(2);

                _gameTimer.Interval = GameLength;
                _gameTimer.Tick += GameTimerOnTick;

                GetDefaults();
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
                if (m.Notification.Equals(Constants.Messages.NewGameMsg))
                {
                    StartNewGame();
                }
                if (m.Notification.Equals(Constants.Messages.UpdateTheDefaultsManMsg))
                {
                    GetDefaults();
                }
            });

            Messenger.Default.Register<NotificationMessageAction<int>>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.RequestScoreMsg))
                {
                    m.Execute(RoundPoints);
                }
            });
        }

        private void GetDefaults()
        {
            GameLength = App.SettingsWrapper.AppSettings.DefaultGameLength;
            SelectedGenre = App.SettingsWrapper.AppSettings.DefaultGenre;
        }

        public List<Genre> Genres { get; set; }
        public Genre SelectedGenre { get; set; }
        public Product GameTrack { get; set; }
        public Uri AudioUrl { get; set; }
        public Uri ArtistImage { get { return CanShowAnswers ? GameTrack.Thumb320Uri : new Uri("/Images/ArtistImagePlaceholder.png", UriKind.Relative); } }
        public TimeSpan GameLength { get; set; }
        public bool IsPlaying { get; set; }

        public bool GameLocked { get; set; }
        public bool CanShowAnswers { get; set; }
        public string ArtistGuess { get; set; }
        public string SongGuess { get; set; }
        public int MaximumRoundPoints { get; set; }
        public int RoundPoints { get; set; }
        public int RoundNumber { get; set; }
        public bool SubmittingScore { get; set; }

        public string AnotherRoundOrNot
        {
            get { return RoundNumber < Constants.MaximumNumberOfRounds ? "ready for another?" : "shall we submit your scores now?"; }
        }

        public bool IsLastRound
        {
            get { return RoundNumber == Constants.MaximumNumberOfRounds; }
        }

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

            MaximumRoundPoints = AdjustScoreForGameLength(score); 
        }

        private int AdjustScoreForGameLength(int score)
        {
            var seconds = GameLength.Seconds;

            if (seconds <= 5)
            {
            }
            else if (seconds <= 10)
            {
                score = (int)Math.Floor(score * 0.95);
            }
            else if (seconds <= 15)
            {
                score = (int)Math.Floor(score * 0.9);
            }
            else if (seconds <= 20)
            {
                score = (int)Math.Floor(score * 0.8);
            }
            else if (seconds <= 25)
            {
                score = (int)Math.Floor(score * 0.75);
            }
            else if (seconds <= 30)
            {
                score = (int)Math.Floor(score * 0.5);
            }

            return score;
        }
        #endregion

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

            Debug.WriteLine("Artist: {0}", GameTrack.Performers[0].Name);
            Debug.WriteLine("Track: {0}", GameTrack.Name);

            AudioUrl = GameTrack.GetSampleUri();
        }

        private void ResetGameForNewRound()
        {
            AudioUrl = null;
            CanShowAnswers = false;
            ArtistGuess = string.Empty;
            SongGuess = string.Empty;
            RoundPoints = 0;
            RoundNumber = 0;

            CalculateAvailableScore();
        }

        private bool CheckAnswers()
        {
            bool artistGuessCorrect;
            bool songGuessCorrect;

            Utils.SeeIfWeHaveAWinner(GameTrack, ArtistGuess, SongGuess, out artistGuessCorrect, out songGuessCorrect);

            var score = 0;

            if (artistGuessCorrect)
            {
                score += Constants.Scores.CorrectArtist;
            }

            if (songGuessCorrect)
            {
                score += Constants.Scores.CorrectSong;
            }

            if (artistGuessCorrect && songGuessCorrect)
            {
                score += Constants.Scores.CorrectSongAndArtistBonus;
            }

            RoundPoints = AdjustScoreForGameLength(score);

            return (artistGuessCorrect || songGuessCorrect);
        }

        private async Task LaunchSpeech()
        {
            var speechRecognizer = new SpeechRecognizerUI();
            speechRecognizer.Settings.ExampleText = "Artist is Aerosmith";
            speechRecognizer.Settings.ListenText = "Make your guess";
            
            var result = await speechRecognizer.RecognizeWithUIAsync();
            
            if (result.ResultStatus == SpeechRecognitionUIStatus.Succeeded)
            {
                FlurryWP8SDK.Api.LogEvent("VoiceCommandUsedArtist");
                var text = result.RecognitionResult.Text;

                if (text.ToLower().StartsWith("artist is"))
                {
                    FlurryWP8SDK.Api.LogEvent("VoiceCommandUsedArtist");
                    text = text.Replace("Artist is", "").Replace("artist is", "").Replace(".", " ").Trim();
                    ArtistGuess = text;
                }
                else if (text.ToLower().StartsWith("song is"))
                {
                    FlurryWP8SDK.Api.LogEvent("VoiceCommandUsedSong");
                    text = text.Replace("song is", "").Replace("Song is", "").Replace(".", " ").Trim();
                    SongGuess = text;
                }
            }
        }

        private void StartNewGame()
        {
            GameLocked = false;
            ResetGameForNewRound();
        }

        private void CheckIfMusicPlayingAndCanStopIt()
        {
            if (IsMusicPlaying())
            {
                if (App.SettingsWrapper.AppSettings.AllowStopMusic || _alreadyAskedAboutMusic)
                {
                    StopMusic();
                }
                else
                {
                    var message = new CustomMessageBox
                    {
                        Title = "stop music?",
                        Message = "We've noticed you're already listening to some music, mind if we stop it so you can play the game?",
                        LeftButtonContent = "go ahead",
                        RightButtonContent = "no thanks",
                        Content = Utils.CreateDontShowCheckBox("DontShowAllowStopMusicMessage")
                    };

                    message.Dismissed += (sender, args) =>
                    {
                        if (args.Result == CustomMessageBoxResult.LeftButton)
                        {
                            _alreadyAskedAboutMusic = true;
                            StopMusic();
                            StartNewGame();
                        }
                    };
                    message.Show();
                }
            }
        }

        private static bool IsMusicPlaying()
        {
            FrameworkDispatcher.Update();

            var musicPlaying = !MediaPlayer.GameHasControl;

            return musicPlaying;
        }

        private static void StopMusic()
        {
            MediaPlayer.Pause();
        }

        private void SubmitScore()
        {
            if (App.CurrentPlayer == null)
            {
                var message = new CustomMessageBox
                {
                    Title = "Not logged in",
                    Message = "You need to be logged in in order to submit your score. You can just play for fun, of course, but let's not be anti-social.",
                    LeftButtonContent = "sign in",
                    RightButtonContent= "just carry on",
                    Content = Utils.CreateDontShowCheckBox("DontShowNotSignedInMessage")
                };

                message.Dismissed += (sender, args) =>
                {
                    ((CustomMessageBox)sender).Dismissing += (o, eventArgs) => eventArgs.Cancel = true;
                    if (args.Result == CustomMessageBoxResult.LeftButton)
                    {
                        NavigateTo(Constants.Pages.Scoreoid.SignIn);
                    }
                };
                message.Show();
            }
            else
            {
                ActuallySubmitTheScore();
            }
        }

        private void NavigateTo(string link)
        {
            Log.Info("Navigating to " + link);
            _navigationService.NavigateTo(link);
        }

        private void ActuallySubmitTheScore()
        {
            SetProgressBar("Submitting score...");

            SubmittingScore = true;

            var score = new score
            {
                data = SelectedGenre.Name,
                platform = "WP8",
                value = RoundPoints.ToString(CultureInfo.InvariantCulture),
                created = DateTime.Now.ToString(CultureInfo.CurrentUICulture.DateTimeFormat)
            };

            Messenger.Default.Send(new NotificationMessageAction<bool>(score, Constants.Messages.SubmitScoreMsg, success =>
            {
                SetProgressBar();

                if (success)
                {
                    SubmittingScore = false;
                    NavigateTo(Constants.Pages.ScoreBoard);
                }
                else
                {
                    DisplayRetryPrompt();
                }
            }));
        }

        private void DisplayRetryPrompt()
        {
            var message = new CustomMessageBox
            {
                Message = "There was a problem trying to upload your score to scoreoid.",
                Title = "Issues",
                LeftButtonContent = "try again",
                RightButtonContent = "cancel"
            };
            message.Dismissed += (sender, args) =>
            {
                ((CustomMessageBox)sender).Dismissing += (o, eventArgs) => eventArgs.Cancel = true;
                if (args.Result == CustomMessageBoxResult.LeftButton)
                {
                    SubmitScore();
                }
            };
            message.Show();
        }

        public void DoINeedToSubmitScores()
        {
            if (IsLastRound)
            {
                SubmitScore();
            }
        }

        #region Commands
        public RelayCommand GamePageLoaded
        {
            get
            {
                return new RelayCommand(CheckIfMusicPlayingAndCanStopIt);
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
                        CanShowAnswers = true;
                        var message = new CustomMessageBox
                        {
                            Title = "Congratulations!",
                            Message = string.Format("Well done, you scored {0} points this round. Right, enough jibba jabba, {1}", RoundPoints, AnotherRoundOrNot),
                            LeftButtonContent = "yes please",
                            RightButtonContent = "nah, not yet"
                        };

                        message.Dismissed += async (sender, args) =>
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
                                    SubmitScore();
                                }
                            }
                        };

                        message.Show();
                    }
                    else
                    {
                        MessageBox.Show("Ouch, sorry, whatever you're doing to those answers, it isn't right. Keep trying though.", "D'oh", MessageBoxButton.OK);
                    }
                });
            }
        }

        public RelayCommand GiveUpCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var messageBox = new CustomMessageBox
                    {
                        Title = "Really?",
                        Message = "Really? You're giving up? Are you sure?",
                        LeftButtonContent = "yeah :(",
                        RightButtonContent = "no"
                    };
                    messageBox.Dismissed += (sender, args) =>
                    {
                        if(args.Result == CustomMessageBoxResult.LeftButton)
                        {
                            FlurryWP8SDK.Api.LogEvent("GivenUp");

                            RoundNumber++;
                            AudioUrl = null;
                            CanShowAnswers = true;
                        }
                    };
                    messageBox.Show();
                });
            }
        }

        public RelayCommand StartGameCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (GameLength.Seconds == 0)
                    {
                        MessageBox.Show("Really? You think you can do it in 0 seconds? Well, as good as you think you are, you have to select at least 1 second. Sorry.", "Sorry", MessageBoxButton.OK);
                        return;
                    }

                    CheckIfMusicPlayingAndCanStopIt();

                    GameLocked = true;

                    FlurryWP8SDK.Api.LogEvent("GameStarted", new List<Parameter> {new Parameter("GameLength", GameLength.Seconds.ToString()), new Parameter("GameGenre", SelectedGenre.Name)});

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
                    FlurryWP8SDK.Api.LogEvent("NokiaMusicLaunched");
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

        public RelayCommand SubmitScoreCommand
        {
            get
            {
                return new RelayCommand(SubmitScore);
            }
        }
        #endregion

        #region GameState methods
        internal void RestoreGameState()
        {
            var gameState = _applicationSettings.Get<GameState>(Constants.Settings.GameState, null);
            if (gameState == null) return;

            Log.Info("Previous game state exists, restoring");

            GameLength = gameState.GameLength;
            SelectedGenre = gameState.GameGenre;
            RoundNumber = gameState.RoundNumber;
            RoundPoints = gameState.CurrentScore;

            GameLocked = true;
        }

        internal void SaveGameState()
        {
            if (!GameLocked) return;

            Log.Info("Saving game state");

            var gameState = new GameState(GameLength, SelectedGenre, RoundNumber, RoundPoints);
            _applicationSettings.Set(Constants.Settings.GameState, gameState);
        }
        #endregion
    }
}