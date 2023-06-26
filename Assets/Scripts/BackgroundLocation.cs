using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BackgroundLocation : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI syncDateText;
    [SerializeField] private TextMeshProUGUI locationText;

    [SerializeField] private string webSocketURL = "";

    private void Awake() {
        BackgroundService.SendActivityReference();
    }

    public void StartService() {
        var _headers = new Dictionary<string, string> {
            { "authentication", "api_key" }
        };
        var _params = new Dictionary<string, string> {
            { "user_id", "123456789" },
            { "password", "123456789" }
        };

        BackgroundService.StartService(webSocketURL, _headers, _params);
    }

    public void StopService() {
        BackgroundService.StopService();
    }

    public void UpdateLocation() {
        BackgroundService.Location location = BackgroundService.GetLocation();
        locationText.text = $"({location.longitude}, {location.latitude})";
        syncDateText.text = $"Last Updated\n{System.DateTime.FromFileTime(location.updateTime):yyyy-MM-dd HH:mm:ss}";
    }
}
