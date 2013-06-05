using System.Windows;
using Cimbalino.Phone.Toolkit.Services;
using Microsoft.Phone.Net.NetworkInformation;

namespace InTwo.Model
{
    public class ExtendedNavigationService : NavigationService, IExtendedNavigationService
    {
        public bool IsNetworkAvailable
        {
            get
            {
                var result = GetNetworkInformation();
                if (!result)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() => App.ShowMessage("No network connection available"));
                }
                return result;
            }
        }

        public bool IsNetworkAvailableSilent
        {
            get { return GetNetworkInformation(); }
        }

        private static bool GetNetworkInformation()
        {
            return System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable()
                   || NetworkInterface.NetworkInterfaceType != NetworkInterfaceType.None;
        }
    }
}