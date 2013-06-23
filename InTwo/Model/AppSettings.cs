using System;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using InTwo.ViewModel;
using JetBrains.Annotations;
using Nokia.Music.Types;
using ScoreoidPortable.Entities;


namespace InTwo.Model
{
    public class AppSettings : ObservableObject
    {
        public AppSettings()
        {
            ShowWelcomeMessage = true;
            if (ViewModelBase.IsInDesignModeStatic)
            {
                PlayerWrapper = new PlayerWrapper(new Player
                {
                    Username = "scottisafool",
                    BestScore = 336,
                    Rank = 1
                });

                MostRecentScore = new Score
                {
                    Data = "Rock",
                    TheScore = "666"
                };
            }
            else
            {
                PlayerWrapper = new PlayerWrapper();
                AlwaysStartFromTheBeginning = true;
            }
            DefaultGameLength = TimeSpan.FromSeconds(2);
            DefaultGenre = new Genre {Name = GameViewModel.AllGenres};
        }

        public bool ShowWelcomeMessage { get; set; }
        public bool DontShowSpeechGuessprompt { get; set; }
        public bool DontShowBackExitMessage { get; set; }
        public bool DontShowAllowStopMusicMessage { get; set; }
        public bool DontShowNotSignedInMessage { get; set; }
        public bool AllowStopMusic { get; set; }
        public bool UseProfilePictureInTile { get; set; }
        public bool UseTransparentTileBackground { get; set; }
        public bool AlwaysStartFromTheBeginning { get; set; }
        public TimeSpan DefaultGameLength { get; set; }
        public Genre DefaultGenre { get; set; }
        public PlayerWrapper PlayerWrapper { get; set; }
        public Score MostRecentScore { get; set; }

        [UsedImplicitly]
        private void OnDefaultGameLengthChanged()
        {
            if (DefaultGameLength.Seconds == 0)
            {
                MessageBox.Show("Really? You think you can do it in 0 seconds? Well, as good as you think you are, you have to select at least 1 second. Sorry.", "Sorry", MessageBoxButton.OK);
                DefaultGameLength = TimeSpan.FromSeconds(2);
            }

            AlertTheGameViewModel();
        }

        [UsedImplicitly]
        private void OnDefaultGenreChanged()
        {
            AlertTheGameViewModel();
        }

        private void AlertTheGameViewModel()
        {
            Messenger.Default.Send(new NotificationMessage(Constants.Messages.UpdateTheDefaultsManMsg));
        }

        [UsedImplicitly]
        private void OnUseProfilePictureInTileChanged()
        {
            UpdateTile();
        }

        [UsedImplicitly]
        private void OnUseTransparentTileBackgroundChanged()
        {
            UpdateTile();
        }

        private static void UpdateTile()
        {
            Messenger.Default.Send(new NotificationMessage(Constants.Messages.UpdatePrimaryTileMsg));
        }
    }
}
