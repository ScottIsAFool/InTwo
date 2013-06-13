using System;
using System.Globalization;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using InTwo.ViewModel;
using Nokia.Music.Types;
using Scoreoid;

namespace InTwo.Model
{
    public class AppSettings : ObservableObject
    {
        public AppSettings()
        {
            ShowWelcomeMessage = true;
            if (ViewModelBase.IsInDesignModeStatic)
            {
                PlayerWrapper = new PlayerWrapper(new player
                {
                    username = "scottisafool",
                    best_score = "336",
                    rank = "1"
                });

                MostRecentScore = new score
                {
                    created = DateTime.Now.ToString(CultureInfo.CurrentUICulture.DateTimeFormat),
                    data = "Rock",
                    value = "666"
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
        public score MostRecentScore { get; set; }
        
        private void OnDefaultGameLengthChanged()
        {
            if (DefaultGameLength.Seconds == 0)
            {
                MessageBox.Show("Really? You think you can do it in 0 seconds? Well, as good as you think you are, you have to select at least 1 second. Sorry.", "Sorry", MessageBoxButton.OK);
                DefaultGameLength = TimeSpan.FromSeconds(2);
            }

            AlertTheGameViewModel();
        }

        private void OnDefaultGenreChanged()
        {
            AlertTheGameViewModel();
        }

        private void AlertTheGameViewModel()
        {
            Messenger.Default.Send(new NotificationMessage(Constants.Messages.UpdateTheDefaultsManMsg));
        }

        private void OnUseProfilePictureInTileChanged()
        {
            UpdateTile();
        }

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
