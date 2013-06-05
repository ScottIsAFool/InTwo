namespace InTwo
{
    public static class Constants
    {
        public const string NokiaMusicAppId = "_x3lWsCXNPJQnprGCJd3";
        public const string NokiaMusicToken = "WL5DlkicW-mj5lSRzO0etQ";
        public const string NokiaAdExchangeId = "ScottLovegrove_InTwo_WP";

#if DEBUG
        public const string ScoreoidGameId = "dfe4d56007"; // In Two Test
#else
        public const string ScoreoidGameId = "edb21044df"; // In Two Production
#endif
        public const string ScoreoidApiKey = "1fb20791da97e2395364b19595aace22d727de60";
        public const string UserCreatedSuccessfully = "The player has been created";
        public const string UserAlreadyExists = "A player with that username already exists";
        public const string ClearBackStack = "?clearbackstack=true";

        public const string ProfilePicturesFolder = "ProfilePictures";
        public static readonly string ProfilePictureUri = "isostore:/" + ProfilePicturesFolder + "/{0}.jpg";
        public static readonly string ProfilePictureStorageFilePath = ProfilePicturesFolder + "\\{0}.jpg";
        public const string GameDataFile = "GameData.json";

        public const int MaximumNumberOfRounds = 5;

        public static class Scores
        {
            public const int CorrectArtist = 50;
            public const int CorrectSong = 200;
            public const int CorrectSongAndArtistBonus = 50;
        }

        public static class Tiles
        {
            private const string WideTileFileName = "InTwoWideTile.png";
            private const string NormalTileFileName = "InTwoNormalTile.png";
            private const string SharedFolderPath = "Shared\\ShellContent\\";
            private const string SharedFolderIsoPath = "isostore:/Shared/ShellContent/";
            public const string WideTileFile = SharedFolderPath + WideTileFileName;
            public const string WideTileIsoUri = SharedFolderIsoPath + WideTileFileName;

            public const string NormalTileFile = SharedFolderPath + NormalTileFileName;
            public const string NormalTileIsoUri = SharedFolderIsoPath + NormalTileFileName;
        }

        public static class Messages
        {
            public const string RefreshCurrentPlayerMsg = "RefreshCurrentPlayerMsg";
            public const string HereAreTheGenresMsg = "HereAreTheGenresMsg";
            public const string HereAreTheTracksMsg = "HereAreTheTracksMsg";
            public const string ShareScoreMsg = "ShareScoreMsg";
            public const string IsPlayingMsg = "IsPlayingMsg";
            public const string RefreshCurrentPlayerInfoMsg = "RefreshCurrentPlayerInfoMsg";
        }

        public static class Pages
        {
            public const string MainPage = "/Views/MainPage.xaml";
            public const string ScoreBoard = "/Views/ScoreBoardView.xaml";
            public const string Settings = "/Views/SettingsView.xaml";
            public const string DownloadingSongs = "/Views/DownloadingSongsView.xaml";
            public const string Game = "/Views/GameView.xaml";

            public static class Scoreoid
            {
                public const string CreateNewUser = "/Views/Scoreoid/CreateNewUserView.xaml";
                public const string EditUser = "/Views/Scoreoid/EditUserView.xaml";
                public const string SignIn = "/Views/Scoreoid/SignInView.xaml";
                public const string UserProfile = "/Views/Scoreoid/UserProfileView.xaml";
            }

            public static class Welcome
            {
                public const string WelcomePage = "/Views/Welcome/WelcomeView.xaml";
                public const string ScoreoidWelcome = "/Views/Welcome/ScoreoidWelcomeView.xaml";
                public const string DownloadSongsNow = "/Views/Welcome/DownloadDataNowView.xaml";
            }
        }
    }
}
