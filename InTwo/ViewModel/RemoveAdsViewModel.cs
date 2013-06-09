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
        private readonly IApplicationSettingsService _settingsService;

        private ProductListing _productInfo;

        /// <summary>
        /// Initializes a new instance of the RemoveAdsViewModel class.
        /// </summary>
        public RemoveAdsViewModel(IExtendedNavigationService navigationService, IApplicationSettingsService settingsService)
        {
            _navigationService = navigationService;
            _settingsService = settingsService;
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
                        _settingsService.Set(Constants.Settings.HasRemovedAds, true);
                        _settingsService.Save();

                        _navigationService.GoBack();
                    }
                });
            }
        }
        #endregion
    }
}