namespace InTwo
{
    public static class Constants
    {
        public const string NokiaMusicAppId = "_x3lWsCXNPJQnprGCJd3";
        public const string NokiaMusicToken = "WL5DlkicW-mj5lSRzO0etQ";

#if DEBUG
        public const string ScoreoidGameId = "dfe4d56007";
#else
        public const string ScoreoidGameId = "edb21044df";
#endif
        public const string ScoreoidApiKey = "1fb20791da97e2395364b19595aace22d727de60";
        public const string UserCreatedSuccessfully = "The player has been created";
        public const string UserAlreadyExists = "A player with that username already exists";

        public static class Pages
        {
            public const string MainPage = "/Views/MainPage.xaml";
            public const string ScoreBoard = "/Views/ScoreBoardView.xaml";
            public const string Settings = "/Views/SettingsView.xaml";

            public static class Scoreoid
            {
                public const string CreateNewUser = "/Views/Scoreoid/CreateNewUserView.xaml";
                public const string EditUser = "/Views/Scoreoid/EditUserView.xaml";
                public const string SignIn = "/Views/Scoreoid/SignInView.xaml";
            }

            public static class Welcome
            {
                public const string WelcomePage = "/Views/Welcome/WelcomeView.xaml";
                public const string ScoreoidWelcome = "/Views/Welcome/ScoreoidWelcomeView.xaml";
            }
        }
    }
}
