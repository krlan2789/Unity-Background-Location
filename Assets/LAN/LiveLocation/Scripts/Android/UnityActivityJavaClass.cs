using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LAN.Android {
    public struct Settings {
#if UNITY_ANDROID && !UNITY_EDITOR
        public static readonly string IGNORE_BATTERY_OPTIMIZATION_SETTINGS = "android.settings.IGNORE_BATTERY_OPTIMIZATION_SETTINGS";
        public static readonly string APPLICATION_DETAILS_SETTINGS = "android.settings.APPLICATION_DETAILS_SETTINGS";
        public static readonly string LOCATION_SOURCE_SETTINGS = "android.settings.LOCATION_SOURCE_SETTINGS";
#endif
    }
    public static class UnityActivityJavaClass {
#if UNITY_ANDROID && !UNITY_EDITOR
        public static readonly AndroidJavaClass URI_CLASS = new("android.net.Uri");
        public static readonly AndroidJavaClass VERSION_INFO_CLASS = new("android.os.Build$VERSION");
        public static readonly AndroidJavaClass UNITY_PLAYER_CLASS = new("com.unity3d.player.UnityPlayer");
        public static readonly AndroidJavaClass PACKAGE_MANAGER_CLASS = new("android.content.pm.PackageManager");

        public static readonly string INTENT_CLASS_PATH = "android.content.Intent";
        public static readonly string COMPONENT_NAME_CLASS_PATH = "android.content.ComponentName";

        public static readonly string START_ACTIVITY_METHOD = "startActivity";
        public static readonly string RESOLVE_ACTIVITY_METHOD = "resolveActivity";
        public static readonly string SET_COMPONENT_METHOD = "setComponent";
        public static readonly string GET_PACKAGE_MANAGER_METHOD = "getPackageManager";
        public static readonly string GET_SYSTEM_SERVICE_METHOD = "getSystemService";
        public static readonly string PACKAGE_MANAGER_MATCH_DEFAULT_ONLY_PROP = "MATCH_DEFAULT_ONLY";

        private static AndroidJavaObject currentActivity = null;
        public static AndroidJavaObject CurrentActivity {
            get {
                return currentActivity ??= UNITY_PLAYER_CLASS.GetStatic<AndroidJavaObject>("currentActivity");
            }
        }

        private static AndroidJavaObject contentResolver = null;
        public static AndroidJavaObject ContentResolver {
            get {
                return contentResolver ??= CurrentActivity.Call<AndroidJavaObject>("getContentResolver");
            }
        }

        public static string PackageName {
            get {
                return CurrentActivity.Call<string>("getPackageName");
            }
        }

        public static AndroidJavaObject CreateIntent(string packangeName) {
            return new AndroidJavaObject(INTENT_CLASS_PATH, packangeName);
        }

        public static AndroidJavaObject CreateIntentWithComponent(string packangeName, string className) {
            return new AndroidJavaObject(INTENT_CLASS_PATH).Call<AndroidJavaObject>(SET_COMPONENT_METHOD, new AndroidJavaObject(COMPONENT_NAME_CLASS_PATH, packangeName, className));
        }

        public static void StartActivity(AndroidJavaObject intent) {
            CurrentActivity.Call(START_ACTIVITY_METHOD, intent);
        }

        public static bool TryStartActivity(AndroidJavaObject intent) {
            try {
                CurrentActivity.Call(START_ACTIVITY_METHOD, intent);
            } catch (Exception e) {
                Debug.LogWarning("Exception1: " + e.Message);
                return false;
            }
            return true;
        }

        public static AndroidJavaObject GetSystemService(string serviceName) {
            return CurrentActivity.Call<AndroidJavaObject>(GET_SYSTEM_SERVICE_METHOD, serviceName);
        }

        public static AndroidJavaObject GetPackageManager() {
            return CurrentActivity.Call<AndroidJavaObject>(GET_PACKAGE_MANAGER_METHOD);
        }
#endif
    }
}