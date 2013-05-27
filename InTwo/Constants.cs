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

        public const string ProfilePictureUri = "isostore:/ProfilePictures/{0}.jpg";
        public const string GameDataFile = "GameData.json";
        public const string RefreshCurrentPlayerMsg = "RefreshCurrentPlayerMsg";

        public static class Pages
        {
            public const string MainPage = "/Views/MainPage.xaml";
            public const string ScoreBoard = "/Views/ScoreBoardView.xaml";
            public const string Settings = "/Views/SettingsView.xaml";
            public const string DownloadingSongs = "/Views/DownloadingSongsView.xaml";

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
