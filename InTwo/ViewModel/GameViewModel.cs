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
using JetBrains.Annotations;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Nokia.Music.Types;
using ScoreoidPortable.Entities;
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
        private List<string> _usedTracks; 
        private readonly DispatcherTimer _gameTimer;
        private SpeechRecognizerUI _speechRecognizer;

        private bool _alreadyAskedAboutMusic;
        private bool _speechIsSupported;

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
                Genres = new List<Genre> {new Genre {Name = "Rock"}};
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

                _usedTracks = new List<string>();

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
        public Uri ArtistImage { get { return CanShowAnswers || ShowHint ? GameTrack.Thumb320Uri : new Uri("/Images/ArtistImagePlaceholder.png", UriKind.Relative); } }
        public TimeSpan GameLength { get; set; }
        public bool IsPlaying { get; set; }
        public bool ShowHint { get; set; }

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

        public void DoINeedToSubmitScores()
        {
            if (IsLastRound)
            {
                SubmitScore();
            }
        }

        #region Property Changed Methods
        [UsedImplicitly]
        private void OnArtistGuessChanged()
        {
            CalculateAvailableScore();
        }

        [UsedImplicitly]
        private void OnSongGuessChanged()
        {
            CalculateAvailableScore();
        }

        [UsedImplicitly]
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

            if (ShowHint)
            {
                score -= Constants.Scores.ShowHintPunishment;
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
                var availableTracks = _tracks.Where(x => !_usedTracks.Contains(x.Id)).ToList();
                var randomNumber = Utils.GetRandomNumber(0, _tracks.Count);

                if (availableTracks.Count == 0)
                {
                    // TODO: Get more tracks??
                }

                GameTrack = availableTracks[randomNumber];
                _usedTracks.Add(GameTrack.Id);
            }
            else
            {
                var genreTracks = _tracks.Where(x => x.Genres.Contains(SelectedGenre) && !_usedTracks.Contains(x.Id))
                                         .ToList();

                if (genreTracks.Count == 0)
                {
                    // TODO: Get more tracks??
                }

                var randomNumber = Utils.GetRandomNumber(0, genreTracks.Count);

                GameTrack = genreTracks[randomNumber];
                _usedTracks.Add(GameTrack.Id);
            }

            Debug.WriteLine("Artist: {0}", GameTrack.Performers[0].Name);
            Debug.WriteLine("Track: {0}", GameTrack.Name);
            Log.Debug("ID: [{0}], Artist: [{1}], Track: [{2}]", GameTrack.Id, GameTrack.Performers[0].Name, GameTrack.Name);

            AudioUrl = GameTrack.GetSampleUri();
        }

        private void ResetGameForNewRound()
        {
            AudioUrl = null;
            CanShowAnswers = ShowHint = false;
            ArtistGuess = string.Empty;
            SongGuess = string.Empty;
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

            if (score > 0 && ShowHint)
            {
                score -= Constants.Scores.ShowHintPunishment;
            }

            RoundPoints += AdjustScoreForGameLength(score);

            return (artistGuessCorrect || songGuessCorrect);
        }

        private async Task LaunchSpeech()
        {
            // This is needed so that the music isn't playing while you're trying to guess
            AudioUrl = null;

            _speechRecognizer.Settings.ExampleText = "Artist is Aerosmith";
            _speechRecognizer.Settings.ListenText = "Make your guess";

            var result = await _speechRecognizer.RecognizeWithUIAsync();
            
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
            _usedTracks = new List<string>();
            RoundPoints = 0;
            ResetGameForNewRound();
        }

        private void CheckIfMusicPlayingAndCanStopIt()
        {
            try
            {
                _speechRecognizer = new SpeechRecognizerUI();
                _speechRecognizer.Recognizer.GetRecognizer();
                _speechIsSupported = true;
            }
            catch (Exception ex)
            {
                _speechIsSupported = false;
                Log.ErrorException("CheckForSupportedSpeech", ex);
            }

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

            var score = CreateNewScore();

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

        private Score CreateNewScore()
        {
            var score = new Score
            {
                Data = SelectedGenre.Name,
                Platform = "WP8",
                TheScore = RoundPoints.ToString(CultureInfo.InvariantCulture),

            };
            return score;
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
                    AudioUrl = null;

                    if (CheckAnswers())
                    {
                        CanShowAnswers = true;
                        var message = new CustomMessageBox
                        {
                            Title = "Congratulations!",
                            Message = string.Format("Well done, you've scored {0} points so far in this game. Right, enough jibba jabba, {1}", RoundPoints, AnotherRoundOrNot),
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
                                    
                                    var score = CreateNewScore();
                                    score.CreatedDate = DateTime.Now;
                                    App.SettingsWrapper.AppSettings.MostRecentScore = score;

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
                    AudioUrl = null;

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

                    FlurryWP8SDK.Api.LogEvent("GameStarted", new List<Parameter> {new Parameter("GameLength", GameLength.Seconds.ToString(CultureInfo.InvariantCulture)), new Parameter("GameGenre", SelectedGenre.Name)});

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
                    if (!_speechIsSupported)
                    {
                        MessageBox.Show("Unfortunately, your current language is not supported for speech recognition, sorry for any inconvenience.", "Unsupported Language", MessageBoxButton.OK);
                        return;
                    }

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

        public RelayCommand NewGameCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    AudioUrl = null;

                    if (RoundNumber > 1)
                    {
                        var messageBox = new CustomMessageBox
                        {
                            Title = "Are you sure?",
                            Message = "You haven't submitted your score yet, are you sure you want to start a new game? You weren't doing that badly, you did have " + RoundPoints + " points already!",
                            LeftButtonContent = "do it!",
                            RightButtonContent = "no, wait!"
                        };

                        messageBox.Dismissed += (sender, args) =>
                        {
                            if (args.Result == CustomMessageBoxResult.LeftButton)
                            {
                                var score = new Score
                                {
                                    TheScore = RoundPoints.ToString(CultureInfo.InvariantCulture),
                                    CreatedDate = DateTime.Now,
                                    Data = SelectedGenre.Name
                                };

                                App.SettingsWrapper.AppSettings.MostRecentScore = score;

                                StartNewGame();
                            }
                        };

                        messageBox.Show();
                    }
                });
            }
        }

        public RelayCommand ShowHintCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var messageBox = new CustomMessageBox
                    {
                        Title = "Show hint?",
                        Message = "Are you sure you want a hint? This will display the album cover, but more importantly, it will knock 50 points off what you can get this round. Carry on?",
                        LeftButtonContent = "Yeah, go on",
                        RightButtonContent = "No, thanks"
                    };

                    messageBox.Dismissed += (sender, args) =>
                    {
                        if (args.Result == CustomMessageBoxResult.LeftButton)
                        {
                            ShowHint = true;
                            CalculateAvailableScore();
                        }
                    };

                    messageBox.Show();
                });
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