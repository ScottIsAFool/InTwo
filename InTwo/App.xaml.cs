using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;
using Cimbalino.Phone.Toolkit.Services;
using Coding4Fun.Toolkit.Controls;
using InTwo.Model;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using InTwo.Resources;
using Microsoft.Practices.ServiceLocation;
using Scoreoid;

namespace InTwo
{
    public partial class App : Application
    {
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public static PhoneApplicationFrame RootFrame { get; private set; }

        public static SettingsWrapper SettingsWrapper
        {
            get { return ((SettingsWrapper)Current.Resources["Settings"]); }
        }

        public static player CurrentPlayer
        {
            get { return SettingsWrapper.AppSettings.PlayerWrapper.CurrentPlayer; }
            set { SettingsWrapper.AppSettings.PlayerWrapper.CurrentPlayer = value; }
        }

        /// <summary>
        /// Shows the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <param name="action">The action.</param>
        public static void ShowMessage(string message, string title = "", Action action = null)
        {
            var prompt = new ToastPrompt
            {
                Title = title,
                Message = message,
                //TextWrapping = TextWrapping.Wrap
                //Margin = new Thickness(0, 32, 0, 0)
            };

            if (action != null)
                prompt.Tap += (s, e) => action();
            prompt.Show();
        }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions.
            UnhandledException += Application_UnhandledException;

            // Standard XAML initialization
            InitializeComponent();

            // Phone-specific initialization
            //InitializePhoneApplication();

            // Language display initialization

            SettingsWrapper.AppSettings.UsersAccentBrush = (Color) Current.Resources["PhoneAccentColor"];

            ThemeManager.ToDarkTheme();
            ThemeManager.SetAccentColor(AccentColor.Magenta);

            // Show graphics profiling information while debugging.
            if (Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode,
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Prevent the screen from turning off while under the debugger by disabling
                // the application's idle detection.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            InitializePhoneApplication();
            GetSettings();
        }

        private static void GetSettings()
        {
            var settingsService = ServiceLocator.Current.GetInstance<IApplicationSettingsService>();

            var appSettings = settingsService.Get("AppSettings", new AppSettings());

            SettingsWrapper.AppSettings = appSettings;
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            if (!e.IsApplicationInstancePreserved)
            {
                InitializePhoneApplication();
                GetSettings();
            }
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            SaveSettings();
        }

        private static void SaveSettings()
        {
            var settingsService = ServiceLocator.Current.GetInstance<IApplicationSettingsService>();

            settingsService.Set("AppSettings", SettingsWrapper.AppSettings);

            settingsService.Save();
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            SaveSettings();
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                Debugger.Break();
            }
            SaveSettings();
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
            SaveSettings();
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new TransitionFrame
            {
                Background = new SolidColorBrush(Colors.Transparent)
            };

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Handle reset requests for clearing the backstack
            RootFrame.Navigated += CheckForResetNavigation;

            RootFrame.Navigating += RootFrameOnNavigating;

            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            InitializeLanguage();

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        private bool _isFirstPass = true;

        private void RootFrameOnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            if (SettingsWrapper.AppSettings.ShowWelcomeMessage
                && e.NavigationMode == NavigationMode.New
                && !e.Uri.ToString().Contains("/Welcome/")
                && _isFirstPass)
            {
                e.Cancel = true;
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    var mainFrame = Application.Current.RootVisual as PhoneApplicationFrame;
                    mainFrame.Navigate(new Uri(Constants.Pages.Welcome.WelcomePage, UriKind.Relative));
                    _isFirstPass = false;
                });
            }
        }

        private void CheckForResetNavigation(object sender, NavigationEventArgs e)
        {
            // If the app has received a 'reset' navigation, then we need to check
            // on the next navigation to see if the page stack should be reset
            if (e.NavigationMode == NavigationMode.Reset)
                RootFrame.Navigated += ClearBackStackAfterReset;
        }

        private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
        {
            // Unregister the event so it doesn't get called again
            RootFrame.Navigated -= ClearBackStackAfterReset;

            // Only clear the stack for 'new' (forward) and 'refresh' navigations
            if (e.NavigationMode != NavigationMode.New && e.NavigationMode != NavigationMode.Refresh)
                return;

            // For UI consistency, clear the entire page stack
            while (RootFrame.RemoveBackEntry() != null)
            {
                ; // do nothing
            }
        }

        #endregion

        // Initialize the app's font and flow direction as defined in its localized resource strings.
        //
        // To ensure that the font of your application is aligned with its supported languages and that the
        // FlowDirection for each of those languages follows its traditional direction, ResourceLanguage
        // and ResourceFlowDirection should be initialized in each resx file to match these values with that
        // file's culture. For example:
        //
        // AppResources.es-ES.resx
        //    ResourceLanguage's value should be "es-ES"
        //    ResourceFlowDirection's value should be "LeftToRight"
        //
        // AppResources.ar-SA.resx
        //     ResourceLanguage's value should be "ar-SA"
        //     ResourceFlowDirection's value should be "RightToLeft"
        //
        // For more info on localizing Windows Phone apps see http://go.microsoft.com/fwlink/?LinkId=262072.
        //
        private void InitializeLanguage()
        {
            try
            {
                // Set the font to match the display language defined by the
                // ResourceLanguage resource string for each supported language.
                //
                // Fall back to the font of the neutral language if the Display
                // language of the phone is not supported.
                //
                // If a compiler error is hit then ResourceLanguage is missing from
                // the resource file.
                RootFrame.Language = XmlLanguage.GetLanguage(AppResources.ResourceLanguage);

                // Set the FlowDirection of all elements under the root frame based
                // on the ResourceFlowDirection resource string for each
                // supported language.
                //
                // If a compiler error is hit then ResourceFlowDirection is missing from
                // the resource file.
                FlowDirection flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), AppResources.ResourceFlowDirection);
                RootFrame.FlowDirection = flow;
            }
            catch
            {
                // If an exception is caught here it is most likely due to either
                // ResourceLangauge not being correctly set to a supported language
                // code or ResourceFlowDirection is set to a value other than LeftToRight
                // or RightToLeft.

                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                throw;
            }
        }
    }
}