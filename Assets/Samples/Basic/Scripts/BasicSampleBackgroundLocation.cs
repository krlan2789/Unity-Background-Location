using LAN.LiveLocation;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_ANDROID
using LAN.Android;
using UnityEngine.Android;
#endif

namespace LAN.LiveLocation.Samples {
    public class BasicSampleBackgroundLocation : MonoBehaviour {
        [SerializeField] private Text syncDateText;
        [SerializeField] private Text locationText;

        [SerializeField] private string webSocketURL = "wss://websocket.lan.com?token=user1234";
        [SerializeField] private LiveLocation liveLocation;

        private void Start() {
            liveLocation.OnLocationFound += UpdateLocation;
        }

        public void StartService() {
            bool isDebugModeOn = false;

#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                isDebugModeOn = UnityActivityJavaClass.IsDebuggingModeEnabled;
                isDebugModeOn = isDebugModeOn || UnityActivityJavaClass.IsDevOptionsEnabled;
            }
#endif

            if (!isDebugModeOn)
            {
                liveLocation.Setup(webSocketURL, "{}");
                liveLocation.StartService();
            }
            else
            {
                locationText.text = "Debugging mode is enabled! Disable it and try again.";
            }
        }

        public void StopService() {
            liveLocation.StopService();
        }

        public void RequestPermission()
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                if (!LiveLocation.HasPermission)
                {
                    Debug.Log("Has no Permission!");
                    LiveLocationProvider.RequestPermissions(LiveLocationProvider.RequiredPermissions);
                }
                else Debug.Log("Has Permission!");
            }
#endif
        }

        public void OptimizeBatteryUsage()
        {
#if UNITY_ANDROID

            if (Application.platform == RuntimePlatform.Android)
            {
                BatteryOptimization.OpenSetting();
            }
#endif
        }

        public void UpdateLocation() {
            var location = LiveLocation.LastLocation;
            locationText.text = $"({location.longitude}, {location.latitude})";
            syncDateText.text = location.updateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public void UpdateLocation(string errorMessage, Location location) {
            Debug.Log($"Error: {errorMessage} | Status: {LiveLocation.IsRunning} | Coordinate: ({location.longitude}, {location.latitude})");
            locationText.text = $"({location.longitude}, {location.latitude})";
            syncDateText.text = location.updateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }

}