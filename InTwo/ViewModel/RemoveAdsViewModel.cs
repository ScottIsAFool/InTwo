using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using Coding4Fun.Toolkit.Controls;
using FlurryWP8SDK.Models;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using InTwo.Model;
using Microsoft.Phone.Info;
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
        private readonly HttpClient _httpClient;

        private string _emailAddress;
        
        /// <summary>
        /// Initializes a new instance of the RemoveAdsViewModel class.
        /// </summary>
        public RemoveAdsViewModel(IExtendedNavigationService navigationService, HttpClient httpClient)
        {
            _navigationService = navigationService;
            _httpClient = httpClient;
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

                        Messenger.Default.Send(new NotificationMessage(Constants.Messages.ForceSettingsSaveMsg));

                        _navigationService.GoBack();
                    }
                    catch
                    {
                        Log.Info("User most likely cancelled purchased");
                    }
                });
            }
        }

        public RelayCommand EnterCodeCommand
        {
            get
            {
                return new RelayCommand(EnterEmailAddress);
            }
        }
        #endregion

        private void EnterEmailAddress()
        {
            Log.Info("Unlock code requested");
            var input = new InputPrompt
            {
                Title = "Enter your email address",
                Message = "Please enter the Microsoft Account registered to your phone:",
                MessageTextWrapping = TextWrapping.Wrap
            };

            input.Completed += (sender, args) =>
            {
                if (args.PopUpResult == PopUpResult.Ok)
                {
                    _emailAddress = args.Result;
                    EnterUnlockCode();
                }
            };

            input.Show();
        }

        private void EnterUnlockCode()
        {
            var input = new InputPrompt
            {
                Title = "Enter your code",
                Message = "Please enter the code provided to you:",
                MessageTextWrapping = TextWrapping.Wrap
            };

            input.Completed += async (sender, args) =>
            {
                if (args.PopUpResult == PopUpResult.Ok)
                {
                    var code = args.Result;

                    var url = string.Format("http://scottisafoolws.apphb.com/ScottIsAFool/InTwo/unlock?emailaddress={0}&code={1}", _emailAddress, code);

                    SetProgressBar("Checking code...");

                    try
                    {
                        if (!_navigationService.IsNetworkAvailable)
                        {
                            Log.Info("No network connection");
                            return;
                        }

                        Log.Info("Verifying unlock code");

                        var response = await _httpClient.GetStringAsync(url);

                        if (response.ToLower().Equals("true"))
                        {
                            Log.Info("Verification successful");

                            var myDeviceId = (byte[])DeviceExtendedProperties.GetValue("DeviceUniqueId");
                            var deviceIdAsString = Convert.ToBase64String(myDeviceId);

                            FlurryWP8SDK.Api.LogEvent("UnlockCodeSuccessfull", new List<Parameter>
                            {
                                new Parameter("EmailAddress", _emailAddress),
                                new Parameter("DeviceID", deviceIdAsString)
                            });

                            App.SettingsWrapper.HasRemovedAds = true;
                            Messenger.Default.Send(new NotificationMessage(Constants.Messages.ForceSettingsSaveMsg));
                            
                            MessageBox.Show("Thanks, ads have now been removed for you.", "Success", MessageBoxButton.OK);

                            _navigationService.GoBack();
                        }
                        else
                        {
                            MessageBox.Show("The email address and code don't match our records, sorry.", "Not verified", MessageBoxButton.OK);
                            Log.Info("Unable to verify email [{0}] with code [{1}]", _emailAddress, code);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("There was an error verifying your unlock code, please try again later.", "Error", MessageBoxButton.OK);
                        FlurryWP8SDK.Api.LogError("UnlockCode", ex);
                        Log.ErrorException("UnlockCode", ex);
                    }

                    SetProgressBar();
                }
            };

            input.Show();
        }
    }
}