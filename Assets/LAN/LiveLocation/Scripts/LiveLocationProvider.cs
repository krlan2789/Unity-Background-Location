using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using LAN.Android;
using UnityEngine;
using UnityEngine.Android;

namespace LAN.LiveLocation {
    public enum NetworkMethod {
        RESTFULL = 0, WEBSOCKET = 1
    }

    public enum LiveLocationStatus {
        RUNNING = 0, DEAD = 1
    }

    public class LiveLocationProvider {
#if UNITY_ANDROID && !UNITY_EDITOR
        private static AndroidJavaObject alienPortalInstance;
#endif

        private static string[] requiredPermissions = new string[] {
            "android.permission.FOREGROUND_SERVICE",
            "android.permission.FOREGROUND_SERVICE_LOCATION",
            "android.permission.ACCESS_BACKGROUND_LOCATION",
            Permission.CoarseLocation,
            Permission.FineLocation,
            "android.permission.INTERNET"
        };

        public static string[] RequiredPermissions {
            get {
                return requiredPermissions;
            }
        }

        public static bool HasPermission {
            get {
#if UNITY_ANDROID && !UNITY_EDITOR
                foreach (string perm in requiredPermissions) {
                    if (AndroidSdkVersion >= 29 && perm == "android.permission.FOREGROUND_SERVICE_LOCATION") continue;
                    if (!Permission.HasUserAuthorizedPermission(perm)) return false;
                }
#endif
                return true;
            }
        }

        /// <summary>
        /// Status for Location Provider. RUNNING or DEAD
        /// </summary>
        public static LiveLocationStatus Status {
            get {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (alienPortalInstance == null) return LiveLocationStatus.DEAD;
                string _status = alienPortalInstance.Call<string>("getStatus").ToUpper();
                return _status == "RUNNING" ? LiveLocationStatus.RUNNING : LiveLocationStatus.DEAD;
#else
                return LiveLocationStatus.RUNNING;
#endif
            }
        }

        /// <summary>
        /// Erorr message when has an error
        /// </summary>
        public static string ErrorMessage {
            get {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (alienPortalInstance == null) return "";
                return alienPortalInstance.Call<string>("getErrorMessage");
#else
                return "";
#endif
            }
        }

        public static double Latitude {
            get {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (alienPortalInstance == null) return 0;
                string val = alienPortalInstance.Call<string>("getLatitude");
                return ToDoubleExact(val);
#else
                return 0;
#endif
            }
        }

        public static double Longitude {
            get {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (alienPortalInstance == null) return 0;
                string val = alienPortalInstance.Call<string>("getLongitude");
                return ToDoubleExact(val);
#else
                return 0;
#endif
            }
        }

        public static float Accuracy {
            get {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (alienPortalInstance == null) return 0;
                string val = alienPortalInstance.Call<string>("getAccuracy");
                return ToFloatExact(val);
#else
                return 0;
#endif
            }
        }

        public static DateTime UpdatedTime {
            get {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (alienPortalInstance == null) return DateTime.Now;
                string val = alienPortalInstance.Call<string>("getUpdatedTime");
                if (string.IsNullOrEmpty(val)) return DateTime.Now;
                return new DateTime(long.Parse(val));
#else
                return DateTime.Now;
#endif
            }
        }

        /// <summary>
        /// Object that contain Longitude, Latitude, Accuracy, UpdateTime
        /// </summary>
        public static Location LastLocation {
            get {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (alienPortalInstance == null) return Location.Default;
                return new Location() {
                    accuracy = Accuracy, latitude = Latitude, longitude = Longitude, updateTime = UpdatedTime
                };
#else
                return Location.Default;
#endif
            }
        }

        public static int AndroidSdkVersion
        {
            get
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                var versionInfo = new AndroidJavaClass("android.os.Build$VERSION");
                return versionInfo.GetStatic<int>("SDK_INT");
#else
                return 0;
#endif
            }
        }

        /// <summary>
        /// Main setup to use this feature
        /// </summary>
        public static void Setup() {
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaClass alienPortalClass = new("com.singularity_code.live_location.util.other.AlienPortal");
            alienPortalInstance = alienPortalClass.CallStatic<AndroidJavaObject>("newInstance");

            SetContext();
            SetGPSSamplingRate();
            SetupChannel();
            SetupNotification();
#endif
            Debug.Log("Setup service");
        }

        /// <summary>
        /// Additional setup to customize channel information
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        public static void SetupChannel(string id = "Live Location", string name = "Live Location", string description = "Live Location Tracking") {
            SetChannelId(id);
            SetChannelName(name);
            SetChannelDescription(description);
            Debug.Log("Setup notification channel");

        }

        /// <summary>
        /// Additional setup to customize notification information
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        public static void SetupNotification(string title = "Live Location Tracking", string message = "Live location is running") {
            SetNotificationTitle(title);
            SetNotificationMessage(message);
            Debug.Log("Setup foreground notification");

        }

        /// <summary>
        /// Required when send a location information in realtime is needed
        /// </summary>
        /// <param name="url"></param>
        /// <param name="networkMethod"></param>
        public static void SetupURL(string url, NetworkMethod networkMethod = NetworkMethod.WEBSOCKET) {
            SetupAPIorSocketURL(url);
            SetupNetworkMethod(networkMethod);
            Debug.Log("Setup API url");
        }

        public static void SetGPSSamplingRate(long samplingRate = 5000) {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (alienPortalInstance == null) return;
            alienPortalInstance.Call("setGPSSamplingRate", samplingRate);
#endif
        }

        public static void SetContext() {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (alienPortalInstance == null) return;
            alienPortalInstance.Call("setContext", UnityActivityJavaClass.CurrentActivity);
#endif
        }

        public static void SetChannelId(string id) {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (alienPortalInstance == null) return;
            alienPortalInstance.Call("setChannelId", id);
#endif
        }

        public static void SetChannelName(string name) {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (alienPortalInstance == null) return;
            alienPortalInstance.Call("setChannelName", name);
#endif
        }

        public static void SetChannelDescription(string description) {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (alienPortalInstance == null) return;
            alienPortalInstance.Call("setChannelDescription", description);
#endif
        }

        public static void SetupAPIorSocketURL(string url) {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (alienPortalInstance == null) return;
            alienPortalInstance.Call("setupAPIorSocketURL", url);
#endif
        }

        public static void SetNotificationTitle(string title) {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (alienPortalInstance == null) return;
            alienPortalInstance.Call("setNotificationTitle", title);
#endif
        }

        public static void SetNotificationMessage(string message) {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (alienPortalInstance == null) return;
            alienPortalInstance.Call("setNotificationMessage", message);
#endif
        }

        public static void SetupNetworkMethod(NetworkMethod methodEnumIndex) {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (alienPortalInstance == null) return;
            alienPortalInstance.Call("setupNetworkMethod", (int)methodEnumIndex);
#endif
        }

        public static void AddHeader(string key, string value) {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (alienPortalInstance == null) return;
            alienPortalInstance.Call("addHeader", key, value);
#endif
        }

        public static void ClearHeader() {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (alienPortalInstance == null) return;
            alienPortalInstance.Call("clearHeader");
#endif
        }

        public static void SetMessageDescriptor(string descriptor) {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (alienPortalInstance == null) return;
            alienPortalInstance.Call("setMessageDescriptor", descriptor);
#endif
        }

        /// <summary>
        /// Start the service
        /// </summary>
        public static void Start() {
            Debug.Log("Start service");
#if UNITY_ANDROID && !UNITY_EDITOR
            if (alienPortalInstance == null) return;
            alienPortalInstance.Call("start");
#endif
        }


        /// <summary>
        /// Stop the service
        /// </summary>
        public static void Stop() {
            Debug.Log("Stop service");
#if UNITY_ANDROID && !UNITY_EDITOR
            if (alienPortalInstance == null) return;
            alienPortalInstance.Call("stop");
#endif
        }

        public static double ToDoubleExact(string value) {
            if (string.IsNullOrEmpty(value)) return 0;
            value = value.Replace(",", ".");
            double target = (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out target) ? target : 0);
            return target;
        }

        public static float ToFloatExact(string value) {
            if (string.IsNullOrEmpty(value)) return 0;
            value = value.Replace(",", ".");
            float target = (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out target) ? target : 0);
            return target;
        }
    }
}