using System.Windows;
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
            //var logs = WPLogger.GetLogs();
            var logs = string.Empty;

            new EmailComposeService().Show("In Two Logs", logs);
        }
    }
}
