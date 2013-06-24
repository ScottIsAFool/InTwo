using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;
using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Tasks;

namespace InTwo
{
    static class ReviewBugger
    {
        private const int NumOfRunsBeforeFeedback = 7;
        private static int _numOfRuns = -1;
        private static readonly IsolatedStorageSettings Settings = IsolatedStorageSettings.ApplicationSettings;
        private static readonly Button YesButton = new Button { Content = "Yes", Width = 120 };
        private static readonly Button LaterButton = new Button { Content = "Later", Width = 120 };
        private static readonly Button NeverButton = new Button { Content = "Never", Width = 120 };
        private static readonly MessagePrompt MessagePrompt = new MessagePrompt();

        public static void CheckNumOfRuns()
        {
            if (!Settings.Contains("numOfRuns"))
            {
                _numOfRuns = 1;
                Settings.Add("numOfRuns", 1);
            }
            else if (Settings.Contains("numOfRuns") && (int)Settings["numOfRuns"] != -1)
            {
                Settings.TryGetValue("numOfRuns", out _numOfRuns);
                _numOfRuns++;
                Settings["numOfRuns"] = _numOfRuns;
            }
        }

        public static void DidReview()
        {
            if (Settings.Contains("numOfRuns"))
            {
                _numOfRuns = -1;
                Settings["numOfRuns"] = -1;
            }
        }

        public static bool IsTimeForReview()
        {
            return _numOfRuns % NumOfRunsBeforeFeedback == 0;
        }

        public static void PromptUser()
        {
            YesButton.Click += yesButton_Click;
            LaterButton.Click += laterButton_Click;
            NeverButton.Click += neverButton_Click;

            MessagePrompt.Message = "Good ratings and reviews encourage me to create and release updates for this app. Would you like to rate now? Go on, I'll buy you a biscuit!!";

            MessagePrompt.ActionPopUpButtons.RemoveAt(0);
            MessagePrompt.ActionPopUpButtons.Add(YesButton);
            MessagePrompt.ActionPopUpButtons.Add(LaterButton);
            MessagePrompt.ActionPopUpButtons.Add(NeverButton);
            MessagePrompt.Show();
        }

        static void yesButton_Click(object sender, RoutedEventArgs e)
        {
            var review = new MarketplaceReviewTask();
            review.Show();
            MessagePrompt.Hide();
            DidReview();
        }

        static void laterButton_Click(object sender, RoutedEventArgs e)
        {
            _numOfRuns = -1;
            MessagePrompt.Hide();
        }

        static void neverButton_Click(object sender, RoutedEventArgs e)
        {
            DidReview();
            MessagePrompt.Hide();
        }
    }
}