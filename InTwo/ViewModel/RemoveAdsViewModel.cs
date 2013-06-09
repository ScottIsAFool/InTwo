using System;
using System.Linq;
using System.Windows;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using InTwo.Model;
using Windows.ApplicationModel.Store;

namespace InTwo.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class RemoveAdsViewModel : ViewModelBase
    {
        private readonly IExtendedNavigationService _navigationService;
        
        /// <summary>
        /// Initializes a new instance of the RemoveAdsViewModel class.
        /// </summary>
        public RemoveAdsViewModel(IExtendedNavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        #region Commands
        public RelayCommand RemoveAdsPageLoaded
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var licence = CurrentApp.LicenseInformation.ProductLicenses[Constants.RemoveAdsProduct];
                    if (licence.IsActive)
                    {
                        MessageBox.Show("You've already purchased this, you lucky dog.", "Nice!", MessageBoxButton.OK);
                        App.SettingsWrapper.HasRemovedAds = true;

                        _navigationService.GoBack();
                    }
                });
            }
        }

        public RelayCommand RemoveAdsCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    try
                    {
                        await CurrentApp.RequestProductPurchaseAsync(Constants.RemoveAdsProduct, false);

                        MessageBox.Show("Thank you for letting me buy a chocolate bar. Ads are now gone!", "Nice!", MessageBoxButton.OK);
                        App.SettingsWrapper.HasRemovedAds = true;

                        _navigationService.GoBack();
                    }
                    catch
                    {
                        
                    }
                });
            }
        }
        #endregion
    }
}