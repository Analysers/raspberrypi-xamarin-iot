using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

using Estimote;
using UserNotifications;
using Acr.UserDialogs;
using System.Threading.Tasks;
using CoreLocation;
using FreshMvvm;
using IotApp.Azure;
using IotApp.Models;
using FFImageLoading.Forms;
using FFImageLoading.Svg.Forms;
using FFImageLoading;
using FFImageLoading.Forms.Touch;
using FFImageLoading.Transformations;
using Newtonsoft.Json;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using IotApp.Helpers;

namespace IotApp.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IMonitoringV2ManagerDelegate
    {
        private ISettings _settings;
        private IDeviceTwin _deviceTwin;
        private IAzureIoTHub _azureIoTHub;
        private SecureBeaconManager _secureBeaconManager;
        private MonitoringV2Manager _monitoringV2ManagerImmediate;

        private static CLBeaconRegion MySecureBeacon => new CLBeaconRegion(new NSUuid("00000000-0000-0000-0000-000000000000"), 0, 0, nameof(MySecureBeacon))
        {
            NotifyOnEntry = true,
            NotifyOnExit = true, 
            NotifyEntryStateOnDisplay = true
        };

        public double ImmediateMeanTriggerDistance = 1.5;
        public Dictionary<string, string> BeaconsImmediate = new Dictionary<string, string>
        {
            { "replace_with_your_beacon_id", "Bedroom" }
        };

        private static CLCircularRegion HomeRegion =>
            new CLCircularRegion(new CLLocationCoordinate2D(-37.8220206, 144.9606272), 200, "Microsoft Melbourne")
            {
                NotifyOnEntry = true,
                NotifyOnExit = true
            };

        public AppDelegate()
        {
            FreshIOC.Container.Register<ISettings, Settings>().AsSingleton();
            FreshIOC.Container.Register<IDeviceTwin, DeviceTwin>().AsSingleton();
            FreshIOC.Container.Register<IAzureIoTHub, AzureIoTHub>().AsSingleton();
            FreshIOC.Container.Register(UserDialogs.Instance);
        }

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            AppCenter.Start("app-center-secret", typeof(Analytics), typeof(Crashes));

            _settings = FreshIOC.Container.Resolve<ISettings>();
            _deviceTwin = FreshIOC.Container.Resolve<IDeviceTwin>();
            _azureIoTHub = FreshIOC.Container.Resolve<IAzureIoTHub>();

            global::Xamarin.Forms.Forms.Init();

            InitializeCachedImageService();

            Estimote.Config.SetupAppId("estimote-app-id", "estimote-app-token");

            _secureBeaconManager = new SecureBeaconManager();
            _monitoringV2ManagerImmediate = new MonitoringV2Manager(ImmediateMeanTriggerDistance, this);

            LoadApplication(new App(_azureIoTHub));

            _settings.PropertyChanged += async (sender, args) => { await OccupantStateUpdated(); };

            _secureBeaconManager.RequestAlwaysAuthorization();
            _monitoringV2ManagerImmediate.RequestAlwaysAuthorization();

            UNUserNotificationCenter.Current.RequestAuthorization(
                UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound,
                (approved, err) =>
                {
                    // Handle approval
                });
            UIApplication.SharedApplication.RegisterForRemoteNotifications();
            UNUserNotificationCenter.Current.Delegate = new UserNotificationCenterDelegate(_azureIoTHub);

            MonitorHomeGeolocation();
            MonitorSecureBeacons();
            MonitorProximityBeacons();

            return base.FinishedLaunching(app, options);
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            var token = deviceToken.Description.Trim('<', '>').Replace(" ", "");

            if (token != Config.DeviceToken)
            {
                Config.DeviceToken = token;

                var cmd = new RegisterDeviceCommand { DeviceToken = token, User = Config.Person };
                Task.Run(() => ResilientCall.ExecuteWithRetry(() => _azureIoTHub.InvokeMethod("RegisterAppForPushNotifications", JsonConvert.SerializeObject(cmd))));
            }
        }

        private async Task OccupantStateUpdated()
        {
            var occupantState = GetOccupantState();
            _settings.OccupantState = occupantState;
            await _azureIoTHub.UpdateOccupantState(occupantState);

            AppCenterHelper.Track(nameof(OccupantStateUpdated));
        }

        public override void OnActivated(UIApplication uiApplication)
        {
            base.OnActivated(uiApplication);

            RequestUpdates();
        }

        private OccupantState GetOccupantState()
        {
            var isSleeping = _settings.AtHome && _settings.AtBedroom;
            var atImmediateLocation = _settings.AtHome && _settings.AtBedroom;

            return new OccupantState
            {
                AtHome = _settings.AtHome,
                IsSleeping = isSleeping
            };
        }

        private void RequestUpdates()
        {
            LocationService.Instance.RequestState(HomeRegion);

            Task.Delay(1000);

            RequestBeaconStates();
        }

        public void MonitorHomeGeolocation()
        {
            LocationService.Instance.AuthorizationChanged += (s, e) =>
            {
                if (e.Status == CLAuthorizationStatus.AuthorizedAlways)
                {
                    LocationService.Instance.StartMonitoring(HomeRegion);
                }
            };

            // https://medium.com/@mizutori/tracking-highly-accurate-location-in-ios-vol-3-7cd827a84e4d
            LocationService.Instance.RequestAlwaysAuthorization();
            LocationService.Instance.AllowsBackgroundLocationUpdates = true;
            LocationService.Instance.PausesLocationUpdatesAutomatically = false;
            LocationService.Instance.DesiredAccuracy = CLLocation.AccurracyBestForNavigation;
            LocationService.Instance.DistanceFilter = CLLocationDistance.FilterNone;

            LocationService.Instance.RegionEntered += async (object sender, CLRegionEventArgs e) => {

                if (e.Region is CLCircularRegion && e.Region.Identifier == "Microsoft Melbourne")
                {
                    _settings.AtHome = true;

                    AppCenterHelper.Track("Microsoft Melbourne Entered");
                }
            };
            LocationService.Instance.RegionLeft += async (object sender, CLRegionEventArgs e) => {

                if (e.Region is CLCircularRegion && e.Region.Identifier == "Microsoft Melbourne")
                {
                    _settings.AtHome = false;

                    AppCenterHelper.Track("Microsoft Melbourne Left");
                }
            };
            LocationService.Instance.DidDetermineState += (object sender, CLRegionStateDeterminedEventArgs e) => {

                if (e.Region is CLCircularRegion && e.Region.Identifier == "Microsoft Melbourne")
                {
                    _settings.AtHome = e.State == CLRegionState.Inside;

                    AppCenterHelper.Track("Microsoft Melbourne Determine State");
                }
            };

            try
            {
                LocationService.Instance.StartMonitoring(HomeRegion);
            }
            catch (Exception ex)
            {
                AppCenterHelper.Error(nameof(MonitorHomeGeolocation), ex);
            }
        }

        private void MonitorSecureBeacons()
        {
            _secureBeaconManager.AuthorizationStatusChanged += (object sender, SecureBeaconManagerAuthorizationStatusChangedEventArgs e) =>
            {
                StartMonitorinBeacons();
            };

            _secureBeaconManager.EnteredRegion += (object sender, SecureBeaconManagerEnteredRegionEventArgs e) =>
            {
                switch (e.Region.Identifier)
                {
                    case nameof(MySecureBeacon):
                        // Do stuff
                        break;
                }

                AppCenterHelper.Track($"SecureBeacon EnteredRegion: {e.Region.Identifier}");
            };

            _secureBeaconManager.ExitedRegion += (object sender, SecureBeaconManagerExitedRegionEventArgs e) =>
            {
                switch (e.Region.Identifier)
                {
                    case nameof(MySecureBeacon):
                        // Do stuff
                        break;
                }

                AppCenterHelper.Track($"SecureBeacon ExitedRegion: {e.Region.Identifier}");
            };

            _secureBeaconManager.DeterminedState += (object sender, SecureBeaconManagerDeterminedStateEventArgs e) =>
            {
                switch (e.Region.Identifier)
                {
                    case nameof(MySecureBeacon):
                        // Do stuff
                        break;
                }

                AppCenterHelper.Track($"SecureBeacon DeterminedState: {e.Region.Identifier}");
            };

            try
            {
                StartMonitorinBeacons();
            }
            catch (Exception ex)
            {
                AppCenterHelper.Error(nameof(MonitorSecureBeacons), ex);
            }
        }

        private void StartMonitorinBeacons()
        {
            var status = BeaconManager.AuthorizationStatus;

            switch (status)
            {
                case CLAuthorizationStatus.NotDetermined:
                    _secureBeaconManager.RequestAlwaysAuthorization();
                    break;
                case CLAuthorizationStatus.Authorized:
                    _secureBeaconManager.StartMonitoring(MySecureBeacon);
                    break;
                case CLAuthorizationStatus.Denied:
                    {
                        var alert = UIAlertController.Create("Access Denied", "You have denied access to location services. Change this in app settings.", UIAlertControllerStyle.Alert);
                        alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                        UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(alert, animated: true, completionHandler: null);
                        break;
                    }
                case CLAuthorizationStatus.Restricted:
                case CLAuthorizationStatus.AuthorizedWhenInUse:
                    {
                        var alert = UIAlertController.Create("Location Not Available", "You have no access to location services.", UIAlertControllerStyle.Alert);
                        alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                        UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(alert, animated: true, completionHandler: null);
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void MonitorProximityBeacons()
        {
            _monitoringV2ManagerImmediate.StartMonitoring(BeaconsImmediate.Select(x => x.Key).ToArray());
            _monitoringV2ManagerImmediate.EnterdDesiredRange += (object sender, MonitoringV2ManagerEnterdDesiredRangeEventArgs e) =>
            {
                switch (BeaconsImmediate[e.Identifier])
                {
                    case "Bedroom":
                        _settings.AtBedroom = true;
                        break;
                }

                AppCenterHelper.Track($"LocationBeacon EnterdDesiredRange: {BeaconsImmediate[e.Identifier]}");
            };
            _monitoringV2ManagerImmediate.ExitedDesiredRange += (object sender, MonitoringV2ManagerExitedDesiredRangeEventArgs e) =>
            {
                switch (BeaconsImmediate[e.Identifier])
                {
                    case "Bedroom":
                        _settings.AtBedroom = false;
                        break;
                }

                AppCenterHelper.Track($"LocationBeacon ExitedDesiredRange: {BeaconsImmediate[e.Identifier]}");
            };
        }

        public void RequestBeaconStates()
        {
            var monitoredBeacons = new Dictionary<string, MonitoringState>();

            foreach (var id in BeaconsImmediate.Keys)
            {
                var state = _monitoringV2ManagerImmediate.StateForBeacon(id);
                monitoredBeacons.Add(BeaconsImmediate[id], state);
            }

            foreach (var monitoredBeacon in monitoredBeacons)
            {
                switch (monitoredBeacon.Key)
                {
                    case "Bedroom":
                        _settings.AtBedroom = monitoredBeacon.Value == MonitoringState.InsideZone;
                        break;
                }
            }

            _secureBeaconManager.RequestState(MySecureBeacon);

            AppCenterHelper.Track(nameof(RequestBeaconStates));
        }

        public void Failed(MonitoringV2Manager manager, NSError error)
        {
            AppCenterHelper.Error("MonitoringV2Manager failed");
        }

        private static void InitializeCachedImageService()
        {
            ImageService.Instance.Initialize();
            CachedImage.FixedOnMeasureBehavior = true;
            CachedImageRenderer.Init();
            var ignore = typeof(SvgCachedImage);
            var ignore2 = new CircleTransformation();
            var ignore3 = new TintTransformation();
        }
    }
}