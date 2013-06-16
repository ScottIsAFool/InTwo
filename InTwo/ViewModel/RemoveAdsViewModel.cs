using System;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using InTwo.Model;
using ScottIsAFool.WindowsPhone.ViewModel;
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
                        Log.Info("Purchase enquiry");
                        await CurrentApp.RequestProductPurchaseAsync(Constants.RemoveAdsProduct, false);

                        Log.Info("PURCHASED!! GET IN!!");

                        MessageBox.Show("Thank you for letting me buy a chocolate bar. Ads are now gone!", "Nice!", MessageBoxButton.OK);
                        App.SettingsWrapper.HasRemovedAds = true;

                        _navigationService.GoBack();
                    }
                    catch
                    {
                        Log.Info("User most likely cancelled purchased");
                    }
                });
            }
        }
        #endregion
    }
}