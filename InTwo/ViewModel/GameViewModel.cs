﻿using System;
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
using Scoreoid;
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
                _gameTimer = new DispatcherTimer();
                
                GameLength = TimeSpan.FromSeconds(2);

                _gameTimer.Interval = GameLength;
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
        public bool SubmittingScore { get; set; }

        public string AnotherRoundOrNot
        {
            get { return RoundNumber < Constants.MaximumNumberOfRounds ? "ready for another?" : "shall we submit your scores now?"; }
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
            
            var result = await speechRecognizer.RecognizeWithUIAsync();
            
            if (result.ResultStatus == SpeechRecognitionUIStatus.Succeeded)
            {
                var text = result.RecognitionResult.Text;

                if (text.ToLower().StartsWith("artist is"))
                {
                    text = text.Replace("Artist is", "").Replace("artist is", "").Replace(".", " ").Trim();
                    ArtistGuess = text;
                }
                else if (text.ToLower().StartsWith("song is"))
                {
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
                if (App.SettingsWrapper.AppSettings.AllowStopMusic)
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
                            StopMusic();
                            StartNewGame();
                        }
                    };
                    message.Show();
                }
            }
            else
            {
                StartNewGame();
            }
        }

        private bool IsMusicPlaying()
        {
            // TODO: Check whether music is actually playing or not   
            return false;
        }

        private void StopMusic()
        {
            
        }

        private async void SubmitScore()
        {
            SetProgressBar("Submitting score...");

            SubmittingScore = true;

            var score = new score
            {
                difficulty = SelectedGenre.Name,
                platform = "WP8",
                value = RoundPoints.ToString()
            };

            Messenger.Default.Send(new NotificationMessageAction<bool>(score, Constants.Messages.SubmitScoreMsg, success =>
            {
                SetProgressBar();

                if (success)
                {
                    SubmittingScore = false;
                    _navigationService.NavigateTo(Constants.Pages.ScoreBoard);
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
                    var result = MessageBox.Show("Really? You're giving up? Are you sure?", "Really?", MessageBoxButton.OKCancel);

                    if (result == MessageBoxResult.OK)
                    {
                        CanShowAnswers = true;
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