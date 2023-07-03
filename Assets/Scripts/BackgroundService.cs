using System.Collections.Generic;
using UnityEngine;

public class BackgroundService {
    [System.Serializable]
    public class Location {
        public double longitude, latitude;
        public float accuracy;
        public long updateTime;
    }

    public enum NetworkMethod {
        WEBSOCKET, HTTPS
    }

    private static AndroidJavaClass unityClass;
    private static AndroidJavaObject unityActivity;
    private static AndroidJavaClass customClass;
    
    //  Replace with the library/package name to be used
    private static readonly string PackageName = "com.singularity_code.live_location.LiveLocationService";
    //  Default Unity Java class
    private static readonly string UnityDefaultJavaClassName = "com.unity3d.player.UnityPlayer";
    //  Method name to send UnityActivity instance to library/package
    private static readonly string CustomClassSetActivityInstanceMethod = "SetActivityInstance";
    //  Method name to start a service/task
    private static readonly string CustomClassStartServiceMethod = "StartService";
    //  Method name to stop a serivce/task
    private static readonly string CustomClassStopServiceMethod = "StopService";
    //  Method name to get location value
    private static readonly string CustomClassGetLocationMethod = "GetLocation";

    public static void CreateService() {

    }

    public static void SendActivityReference()
    {
        unityClass = new AndroidJavaClass(UnityDefaultJavaClassName);
        unityActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity"); //  Call at static method
        customClass = new AndroidJavaClass(PackageName);
        customClass.CallStatic(CustomClassSetActivityInstanceMethod, unityActivity);
    }

    public static void StartService(string websocketUrl, Dictionary<string, string> customHeaders = null, Dictionary<string, string> customParams = null, NetworkMethod networkMethod = NetworkMethod.WEBSOCKET)
    {
        //  Call static method to starting a service/task and pass 3 parameters.
        //  example:
        //  public static void StartService(String WebSocketURL, Map<String, String> Headers, Map<String, String> Params, NetworkMethod networkMethod) {}
        customClass.CallStatic(CustomClassStartServiceMethod, websocketUrl, customHeaders, customParams, networkMethod);
    }

    public static void StopService()
    {
        customClass.CallStatic(CustomClassStopServiceMethod);
    }

    public static Location GetLocation()
    {
        return customClass.CallStatic<Location>(CustomClassGetLocationMethod);
    }
}
