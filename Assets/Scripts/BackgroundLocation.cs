using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;

public class BackgroundLocation : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI syncDateText;
    [SerializeField] private TextMeshProUGUI locationText;

    [SerializeField] private string webSocketURL = "";

    private string[] requiredPermissions = new string[] { "android.permission.FOREGROUND_SERVICE", "android.permission.ACCESS_BACKGROUND_LOCATION", Permission.CoarseLocation, Permission.FineLocation, "android.permission.INTERNET" };

    private IEnumerator Start() {
        if (!HasPermission()) {
            Permission.RequestUserPermissions(requiredPermissions);
            DateTime startTime = DateTime.Now;

            while (!HasPermission()) {
                if (DateTime.Now.Subtract(startTime).TotalSeconds > 8) yield break;
            }
        }
        if (!Input.location.isEnabledByUser) {
            if (Input.location.status != LocationServiceStatus.Running) {
                Input.location.Start();
            }
        }

        yield return null;
        Setup();
    }

    private bool HasPermission() {
        foreach (string perm in requiredPermissions) {
            if (!Permission.HasUserAuthorizedPermission(perm)) return false;
        }
        return true;
    }

    private void Setup() {
        LiveLocationProvider.Setup();
        LiveLocationProvider.ClearHeader();
        LiveLocationProvider.AddHeader("authentication", "Bearer alsdkmlam");
        LiveLocationProvider.SetMessageDescriptor("{\"username\":\"sales001\",\"nama\":\"Sales 001\"}");
    }

    public void StartService() {
        LiveLocationProvider.Start();
    }

    public void StopService() {
        LiveLocationProvider.Stop();
    }

    public void UpdateLocation() {
        locationText.text = $"({LiveLocationProvider.Longitude}, {LiveLocationProvider.Latitude})";
        syncDateText.text = LiveLocationProvider.UpdatedTime;
        //syncDateText.text = $"Last Updated\n{System.DateTime.FromFileTime(LiveLocationProvider.GetUpdatedTime()):yyyy-MM-dd HH:mm:ss}";
    }
}
