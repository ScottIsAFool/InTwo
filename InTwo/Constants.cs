﻿namespace InTwo
{
    public static class Constants
    {
        public const string NokiaMusicAppId = "";
        public const string NokiaMusicToken = "";

#if DEBUG
        public const string ScoreoidGameId = ""; // In Two Test
        public const string FlurryKey = "";
#else
        public const string ScoreoidGameId = ""; // In Two Production
        public const string FlurryKey = "";
#endif
        public const string ScoreoidApiKey = "";
        public const string UserCreatedSuccessfully = "The player has been created";
        public const string UserAlreadyExists = "A player with that Username already exists";
        public const string ClearBackStack = "?clearbackstack=true";

        public const string ProfilePicturesFolder = "ProfilePictures";
        public static readonly string ProfilePictureUri = "isostore:/" + ProfilePicturesFolder + "/{0}.jpg";
        public static readonly string ProfilePictureStorageFilePath = ProfilePicturesFolder + "\\{0}.jpg";
        public const string GameDataFile = "GameData.json";
        public const string GenreDataFile = "GenreData.json";

        public const string RemoveAdsProduct = "RemoveAds";

        public const int MaximumNumberOfRounds = 5;

        public static class Scores
        {
            public const int CorrectArtist = 50;
            public const int CorrectSong = 200;
            public const int CorrectSongAndArtistBonus = 50;
            public const int ShowHintPunishment = 50;
        }

        public static class Tiles
        {
            private const string WideTileFileName = "InTwoWideTile.png";
            private const string NormalTileFileName = "InTwoNormalTile.png";
            private const string SharedFolderPath = "Shared\\ShellContent\\";
            private const string SharedFolderIsoPath = "isostore:/Shared/ShellContent/";
            public const string WideTileFile = SharedFolderPath + WideTileFileName;
            public const string WideTileIsoUri = SharedFolderIsoPath + WideTileFileName;
            public const string WideTileBackFile = SharedFolderPath + "{0}-Back-Wide.png";
            public const string WideTileBackIsoUri = SharedFolderIsoPath + "{0}-Back-Wide.png";
            public const string NormalTileBackFile = SharedFolderPath + "{0}-Back.png";
            public const string NormalTileBackIsoUri = SharedFolderIsoPath + "{0}-Back.png";

            public const string NormalTileFile = SharedFolderPath + NormalTileFileName;
            public const string NormalTileIsoUri = SharedFolderIsoPath + NormalTileFileName;
            public const string UserProfileIsoUriFormat = SharedFolderIsoPath + "{0}.jpg";
            public const string UserProfileFileFormat = SharedFolderPath + "{0}.jpg";
            public const string UserProfileWideIsoUriFormat = SharedFolderIsoPath + "{0}-Wide.jpg";
            public const string UserProfileWideFileFormat = SharedFolderPath + "{0}-Wide.jpg";
            public const string AppTileFormat = "/Assets/Tiles/{0}FlipCycleTile{1}.png";
        }

        public static class Messages
        {
            public const string RefreshCurrentPlayerMsg = "RefreshCurrentPlayerMsg";
            public const string HereAreTheGenresMsg = "HereAreTheGenresMsg";
            public const string HereAreTheTracksMsg = "HereAreTheTracksMsg";
            public const string ShareScoreMsg = "ShareScoreMsg";
            public const string IsPlayingMsg = "IsPlayingMsg";
            public const string RefreshCurrentPlayerInfoMsg = "RefreshCurrentPlayerInfoMsg";
            public const string SubmitScoreMsg = "SubmitScoreMsg";
            public const string UpdatePrimaryTileMsg = "UpdatePrimaryTileMsg";
            public const string RequestScoreMsg = "RequestScoreMsg";
            public const string ForceSettingsSaveMsg = "ForceSettingsSaveMsg";
            public const string NewGameMsg = "ResetGameMsg";
            public const string UpdateTheDefaultsManMsg = "UpdateTheDefaultsManMsg";
        }

        public static class Pages
        {
            private const string ViewsPath = "/Views/";
            public const string MainPage = ViewsPath + "MainPage.xaml";
            public const string ScoreBoard = ViewsPath + "ScoreBoardView.xaml";
            public const string SettingsView = ViewsPath + "SettingsView.xaml";
            public const string DownloadingSongs = ViewsPath + "DownloadingSongsView.xaml";
            public const string Game = ViewsPath + "GameView.xaml";
            public const string Removeads = ViewsPath + "RemoveAdsView.xaml";
            public const string HowToPlay = ViewsPath + "HowToPlayView.xaml";

            public static class Scoreoid
            {
                private const string ScoreoidPath = ViewsPath + "Scoreoid/";
                public const string CreateNewUser = ScoreoidPath + "CreateNewUserView.xaml";
                public const string EditUser = ScoreoidPath + "EditUserView.xaml";
                public const string SignIn = ScoreoidPath + "SignInView.xaml";
                public const string UserProfile = ScoreoidPath + "UserProfileView.xaml";
            }

            public static class Welcome
            {
                private const string WelcomePath = ViewsPath + "Welcome/";
                public const string WelcomePage = WelcomePath + "WelcomeView.xaml";
                public const string ScoreoidWelcome = WelcomePath + "ScoreoidWelcomeView.xaml";
                public const string DownloadSongsNow = WelcomePath + "DownloadDataNowView.xaml";
                public const string StopMusic = WelcomePath + "StopMusicView.xaml";
            }
        }

        public static class Settings
        {
            public const string HasRemovedAds = "HasRemovedAds";
            public const string AppSettings = "AppSettings";
            public const string GameState = "GameState";
        }
    }
}
