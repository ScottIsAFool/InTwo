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
                var result = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
                if (!result || NetworkInterface.NetworkInterfaceType == NetworkInterfaceType.None)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() => App.ShowMessage("No network connection available"));
                }
                return result;
            }
        }
    }
}