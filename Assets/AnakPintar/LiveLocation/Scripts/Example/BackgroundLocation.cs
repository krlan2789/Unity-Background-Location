using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;

namespace AnakPintar.LiveLocation.Example {
    public class BackgroundLocation : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI syncDateText;
        [SerializeField] private TextMeshProUGUI locationText;

        [SerializeField] private string webSocketURL = "wss://websocket.anakpintarstudio.com?id=sales1";
        [SerializeField] private LiveLocation liveLocation;

        private void Start() {
            liveLocation.onLocationFound += UpdateLocation;
            liveLocation.Setup(webSocketURL, "");
        }

        public void StartService() {
            liveLocation.StartService();
        }

        public void StopService() {
            liveLocation.StopService();
        }

        public void UpdateLocation(string errorMessage, Location location) {
            Debug.Log($"Error: {errorMessage} | Status: {LiveLocation.IsRunning} | Coordinate: ({location.longitude}, {location.latitude})");
            locationText.text = $"({location.longitude}, {location.latitude})";
            syncDateText.text = location.updateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }

}