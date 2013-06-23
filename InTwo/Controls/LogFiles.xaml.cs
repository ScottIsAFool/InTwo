using System.Windows;
using Cimbalino.Phone.Toolkit.Helpers;
using Cimbalino.Phone.Toolkit.Services;
using ScottIsAFool.WindowsPhone.Logging;

namespace InTwo.Controls
{
    public partial class LogFiles
    {
        public LogFiles()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var logs = WPLogger.GetLogs();

            var version = new ApplicationManifest().App.Version;

            new EmailComposeService().Show(string.Format("In Two Logs (from {0})", version), logs);
        }
    }
}
