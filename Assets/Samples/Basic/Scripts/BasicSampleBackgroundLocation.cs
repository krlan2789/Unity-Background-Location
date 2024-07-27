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
            liveLocation.Setup(webSocketURL, "");
        }

        public void StartService() {
            liveLocation.StartService();
        }

        public void StopService() {
            liveLocation.StopService();
        }

        public void RequestPermission()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (!LiveLocation.HasPermission)
            {
                Debug.Log("Has no Permission!");
                LiveLocationProvider.RequestPermissions(LiveLocationProvider.RequiredPermissions);
            }
            else Debug.Log("Has Permission!");
#endif
        }

        public void OptimizeBatteryUsage()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            BatteryOptimization.OpenSetting();
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