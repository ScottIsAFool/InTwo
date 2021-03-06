using System.Net;
using System.Net.Http;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using InTwo.Design;
using InTwo.Model;
using Microsoft.Practices.ServiceLocation;
using Nokia.Music;
using ScoreoidPortable;


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

                if(!SimpleIoc.Default.IsRegistered<IPhotoChooserService>())
                    SimpleIoc.Default.Register<IPhotoChooserService, DesignPhotoChooserService>();

                if (!SimpleIoc.Default.IsRegistered<IAsyncStorageService>())
                    SimpleIoc.Default.Register<IAsyncStorageService, DesignAsyncStorageService>();

                if(!SimpleIoc.Default.IsRegistered<IScoreoidClient>())
                    SimpleIoc.Default.Register<IScoreoidClient, DesignScoreoidClient>();
            }
            else
            {
                // Create run time view services and models
                if (!SimpleIoc.Default.IsRegistered<IApplicationSettingsService>())
                    SimpleIoc.Default.Register<IApplicationSettingsService, ApplicationSettingsService>();

                if (!SimpleIoc.Default.IsRegistered<IPhotoChooserService>())
                    SimpleIoc.Default.Register<IPhotoChooserService, PhotoChooserWithCameraService>();

                if(!SimpleIoc.Default.IsRegistered<IAsyncStorageService>())
                    SimpleIoc.Default.Register<IAsyncStorageService, AsyncStorageService>();

                if (!SimpleIoc.Default.IsRegistered<IScoreoidClient>())
                    SimpleIoc.Default.Register<IScoreoidClient>(() => new ScoreoidClient(Constants.ScoreoidApiKey, Constants.ScoreoidGameId));
            }

            if (!SimpleIoc.Default.IsRegistered<IExtendedNavigationService>())
                SimpleIoc.Default.Register<IExtendedNavigationService, ExtendedNavigationService>();

            if (!SimpleIoc.Default.IsRegistered<IMusicClient>())
                SimpleIoc.Default.Register<IMusicClient>(() => new MusicClient(Constants.NokiaMusicAppId));

            SimpleIoc.Default.Register(() => new HttpClient(new HttpClientHandler{AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip}));
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<ScoreoidViewModel>(true);
            SimpleIoc.Default.Register<ScoreBoardViewModel>();
            SimpleIoc.Default.Register<UserProfileViewModel>(true);
            SimpleIoc.Default.Register<DownloadingSongsViewModel>();
            SimpleIoc.Default.Register<GameViewModel>();
            SimpleIoc.Default.Register<RemoveAdsViewModel>();
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

        public DownloadingSongsViewModel Downloading
        {
            get { return ServiceLocator.Current.GetInstance<DownloadingSongsViewModel>(); }
        }

        public GameViewModel Game
        {
            get { return ServiceLocator.Current.GetInstance<GameViewModel>(); }
        }

        public RemoveAdsViewModel RemoveAds
        {
            get { return ServiceLocator.Current.GetInstance<RemoveAdsViewModel>(); }
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