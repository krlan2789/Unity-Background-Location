using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiveLocationProvider {
    private static AndroidJavaClass unityClass;
    private static AndroidJavaObject unityActivity;

    private static AndroidJavaObject alienPortalInstance;

    public static void Setup() {
        unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        unityActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
        //alienPortalInstance = new AndroidJavaObject("com.singularity_code.live_location.util.other.AlienPortal");
        AndroidJavaClass alienPortalClass = new("com.singularity_code.live_location.util.other.AlienPortal");
        alienPortalInstance = alienPortalClass.CallStatic<AndroidJavaObject>("newInstance");

        SetContext();
        SetGPSSamplingRate();
        SetChannelId("Live Location");
        SetChannelName("Live Location");
        SetChannelDescription("Live Location Tracking");
        SetNotificationTitle("Live Location Tracking");
        SetNotificationMessage("Live location is running");
        SetupAPIorSocketURL("wss://websocket.anakpintarstudio.com?id=sales1");
        SetupNetworkMethod(1);
    }

    public static string Status {
        get { return alienPortalInstance.Call<string>("getStatus"); }
    }

    public static string ErrorMessage {
        get { return alienPortalInstance.Call<string>("getErrorMessage"); }
    }

    public static string Latitude {
        get { return alienPortalInstance.Call<string>("getLatitude"); }
    }

    public static string Longitude {
        get { return alienPortalInstance.Call<string>("getLongitude"); }
    }

    public static string Accuracy {
        get { return alienPortalInstance.Call<string>("getAccuracy"); }
    }

    public static string UpdatedTime {
        get { return alienPortalInstance.Call<string>("getUpdatedTime"); }
    }

    public static void SetGPSSamplingRate(long samplingRate = 2000) {
        alienPortalInstance.Call("setGPSSamplingRate", samplingRate);
    }

    public static void SetContext() {
        alienPortalInstance.Call("setContext", unityActivity);
    }

    public static void SetChannelId(string id) {
        alienPortalInstance.Call("setChannelId", id);
    }

    public static void SetChannelName(string name) {
        alienPortalInstance.Call("setChannelName", name);
    }

    public static void SetChannelDescription(string description) {
        alienPortalInstance.Call("setChannelDescription", description);
    }

    public static void SetupAPIorSocketURL(string url) {
        alienPortalInstance.Call("setupAPIorSocketURL", url);
    }

    public static void SetNotificationTitle(string title) {
        alienPortalInstance.Call("setNotificationTitle", title);
    }

    public static void SetNotificationMessage(string message) {
        alienPortalInstance.Call("setNotificationMessage", message);
    }

    public static void SetupNetworkMethod(int methodEnumIndex = 0) {
        alienPortalInstance.Call("setupNetworkMethod", methodEnumIndex);
    }

    public static void AddHeader(string key, string value) {
        alienPortalInstance.Call("addHeader", key, value);
    }

    public static void ClearHeader() {
        alienPortalInstance.Call("clearHeader");
    }

    public static void SetMessageDescriptor(string descriptor) {
        alienPortalInstance.Call("setMessageDescriptor", descriptor);
    }

    public static void Start() {
        alienPortalInstance.Call("start");
    }

    public static void Stop() {
        alienPortalInstance.Call("stop");
    }
}
