using CoreLocation;

namespace IotApp.iOS
{
    public class LocationService : CLLocationManagerDelegate
    {
        private static CLLocationManager locationManager;
        public static CLLocationManager Instance
        {
            get => locationManager ?? (locationManager  = new CLLocationManager());
        }
    }
}
