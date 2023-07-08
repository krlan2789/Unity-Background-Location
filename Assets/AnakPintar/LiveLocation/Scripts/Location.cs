namespace AnakPintar.LiveLocation {
    [System.Serializable]
    public class Location {
        public double longitude, latitude, altitude;
        public float speedAccuracyMeterPerSec, bearing, speed, verticalAccuracyMeters;
        public float accuracy;
        public System.DateTime updateTime;

        public static Location Default {
            get {
                return new Location() {
                    latitude = 0, longitude = 0, altitude = 0, speedAccuracyMeterPerSec = 0, bearing = 0, speed = 0, verticalAccuracyMeters = 0, accuracy = 0, updateTime = System.DateTime.Now
                };
            }
        }
    }
}
