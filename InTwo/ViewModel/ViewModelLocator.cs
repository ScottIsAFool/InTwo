/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:InTwo"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using InTwo.Design;
using Microsoft.Practices.ServiceLocation;
using Nokia.Music;
using Scoreoid;

namespace InTwo.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                // Create design time view services and models DesignApplicationSettingsService
                if (!SimpleIoc.Default.IsRegistered<IApplicationSettingsService>())
                    SimpleIoc.Default.Register<IApplicationSettingsService, DesignApplicationSettingsService>();
            }
            else
            {
                // Create run time view services and models
                if (!SimpleIoc.Default.IsRegistered<IApplicationSettingsService>())
                    SimpleIoc.Default.Register<IApplicationSettingsService, ApplicationSettingsService>();
            }

            if (!SimpleIoc.Default.IsRegistered<INavigationService>())
                SimpleIoc.Default.Register<INavigationService, NavigationService>();

            if(!SimpleIoc.Default.IsRegistered<MusicClient>())
                SimpleIoc.Default.Register(() => new MusicClient(Constants.NokiaMusicAppId));

            if(!SimpleIoc.Default.IsRegistered<ScoreoidClient>())
                SimpleIoc.Default.Register(()=> new ScoreoidClient(Constants.ScoreoidApiKey, Constants.ScoreoidGameId));

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<ScoreoidViewModel>();
            SimpleIoc.Default.Register<ScoreBoardViewModel>();
            SimpleIoc.Default.Register<UserProfileViewModel>();
        }

        public MainViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }

        public ScoreoidViewModel Scoreoid
        {
            get { return ServiceLocator.Current.GetInstance<ScoreoidViewModel>(); }
        }

        public ScoreBoardViewModel ScoreBoard
        {
            get { return ServiceLocator.Current.GetInstance<ScoreBoardViewModel>(); }
        }

        public UserProfileViewModel UserProfile
        {
            get { return ServiceLocator.Current.GetInstance<UserProfileViewModel>(); }
        }
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
            foreach (var vm in ServiceLocator.Current.GetAllInstances<ViewModelBase>())
            {
                vm.Cleanup();
            }
        }
    }
}