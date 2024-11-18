using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LAN.Android
{
    public struct Settings
    {
#if UNITY_ANDROID
        public static readonly string IGNORE_BATTERY_OPTIMIZATION_SETTINGS = "android.settings.IGNORE_BATTERY_OPTIMIZATION_SETTINGS";
        public static readonly string APPLICATION_DETAILS_SETTINGS = "android.settings.APPLICATION_DETAILS_SETTINGS";
        public static readonly string LOCATION_SOURCE_SETTINGS = "android.settings.LOCATION_SOURCE_SETTINGS";
        public static readonly string MANAGE_OVERLAY_PERMISSION = "android.settings.action.MANAGE_OVERLAY_PERMISSION";
#endif
    }

    public static class UnityActivityJavaClass
    {
#if UNITY_ANDROID
        public static readonly AndroidJavaClass URI_CLASS = new("android.net.Uri");
        public static readonly AndroidJavaClass VERSION_INFO_CLASS = new("android.os.Build$VERSION");
        public static readonly AndroidJavaClass UNITY_PLAYER_CLASS = new("com.unity3d.player.UnityPlayer");
        public static readonly AndroidJavaClass PACKAGE_MANAGER_CLASS = new("android.content.pm.PackageManager");
        //public static readonly AndroidJavaClass SETTINGS_CLASS = new("android.provider.Settings");
        //public static readonly AndroidJavaClass SETTINGS_SECURE_CLASS = new("android.provider.Settings$Secure");
        public static readonly AndroidJavaClass SETTINGS_GLOBAL_CLASS = new("android.provider.Settings$Global");

        public static readonly string INTENT_CLASS_PATH = "android.content.Intent";
        public static readonly string COMPONENT_NAME_CLASS_PATH = "android.content.ComponentName";

        public static readonly string START_ACTIVITY_METHOD = "startActivity";
        public static readonly string RESOLVE_ACTIVITY_METHOD = "resolveActivity";
        public static readonly string SET_COMPONENT_METHOD = "setComponent";
        public static readonly string GET_PACKAGE_MANAGER_METHOD = "getPackageManager";
        public static readonly string GET_SYSTEM_SERVICE_METHOD = "getSystemService";
        public static readonly string PACKAGE_MANAGER_MATCH_DEFAULT_ONLY_PROP = "MATCH_DEFAULT_ONLY";

        public static readonly string CHECK_SELF_PERMISSION_METHOD = "checkSelfPermission";
        public static readonly string REQUEST_PERMISSIONS_METHOD = "requestPermissions";

        private static AndroidJavaObject currentActivity = null;
        public static AndroidJavaObject CurrentActivity {
            get
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    return currentActivity ??= UNITY_PLAYER_CLASS.GetStatic<AndroidJavaObject>("currentActivity");
                }
                return currentActivity;
            }
        }

        private static AndroidJavaObject contentResolver = null;
        public static AndroidJavaObject ContentResolver {
            get
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    return contentResolver ??= CurrentActivity.Call<AndroidJavaObject>("getContentResolver");
                }
                return null;
            }
        }

        public static AndroidJavaObject UriPackageObject {
            get
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    string packageScheme = "package:" + PackageName;
                    AndroidJavaObject uri = URI_CLASS.CallStatic<AndroidJavaObject>("parse", packageScheme);
                    Debug.Log(packageScheme + " :: " + uri.Call<string>("getScheme"));
                    return uri;
                }
                return null;
            }
        }
#endif

        public static string PackageName {
            get
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    return CurrentActivity.Call<string>("getPackageName");
                }
                return Application.identifier;
            }
        }

        public static bool IsDebuggingModeEnabled
        {
            get
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    // Check if developer options are enabled
                    bool status = SETTINGS_GLOBAL_CLASS.CallStatic<int>("getInt", CurrentActivity.Call<AndroidJavaObject>("getContentResolver"), "adb_enabled", 0) == 1;

                    Debug.Log($"USB Debugging is {(status ? "enabled" : "disabled")}!");
                    return status;
                }
                return false;
            }
        }

        public static bool IsDevOptionsEnabled
        {
            get
            {

                if (Application.platform == RuntimePlatform.Android)
                {
                    // Check if developer options are enabled
                    bool status = SETTINGS_GLOBAL_CLASS.CallStatic<int>("getInt", CurrentActivity.Call<AndroidJavaObject>("getContentResolver"), "development_settings_enabled", 0) == 1;

                    Debug.Log($"Developer options are {(status ? "enabled" : "disabled")}!");
                    return status;
                }
                return false;
            }
        }

#if UNITY_ANDROID
        public static AndroidJavaObject CreateIntent(string packangeName)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                return new AndroidJavaObject(INTENT_CLASS_PATH, packangeName);
            }
            return null;
        }

        public static AndroidJavaObject CreateIntentWithComponent(string packangeName, string className)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                return new AndroidJavaObject(INTENT_CLASS_PATH).Call<AndroidJavaObject>(SET_COMPONENT_METHOD, new AndroidJavaObject(COMPONENT_NAME_CLASS_PATH, packangeName, className));
            }
            return null;
        }

        public static void StartActivity(AndroidJavaObject intent)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                if (Application.isFocused)
                {
                    CurrentActivity.Call(START_ACTIVITY_METHOD, intent);
                }
            }
        }

        public static bool TryStartActivity(AndroidJavaObject intent) {
            try {

                if (Application.platform == RuntimePlatform.Android)
                {
                    CurrentActivity.Call(START_ACTIVITY_METHOD, intent);
                }
            } catch (Exception e) {
                Debug.LogWarning("Exception1: " + e.Message);
                return false;
            }
            return true;
        }

        public static AndroidJavaObject GetSystemService(string serviceName)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                return CurrentActivity.Call<AndroidJavaObject>(GET_SYSTEM_SERVICE_METHOD, serviceName);
            }
            return null;
        }

        public static AndroidJavaObject GetPackageManager()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                return CurrentActivity.Call<AndroidJavaObject>(GET_PACKAGE_MANAGER_METHOD);
            }
            return null;
        }
#endif

        public static bool HasPermission(string permission) {

            if (Application.platform == RuntimePlatform.Android)
            {
                return CurrentActivity.Call<int>(CHECK_SELF_PERMISSION_METHOD, permission) >= 0;
            }
            return false;
        }

        public static void RequestPermissions(string[] permissions) {
            if (Application.platform == RuntimePlatform.Android)
            {

                if (Application.isFocused)
                {
                    CurrentActivity.Call(REQUEST_PERMISSIONS_METHOD, permissions, 201);
                }
            }
        }

        public static void OpenSetting()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaObject intent = CreateIntent(Settings.APPLICATION_DETAILS_SETTINGS);
                intent.Call<AndroidJavaObject>("setData", UriPackageObject);
                StartActivity(intent);
            }
        }

        public static void OpenManageOverlay()
        {

            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaObject intent = CreateIntent(Settings.MANAGE_OVERLAY_PERMISSION);
                intent.Call<AndroidJavaObject>("setData", UriPackageObject);
                StartActivity(intent);
            }
        }

        /// <summary>
        /// Open Location Service Setting
        /// </summary>
        public static void OpenGPSSetting()
        {
            try
            {

                if (Application.platform == RuntimePlatform.Android)
                {
                    StartActivity(CreateIntent(Settings.LOCATION_SOURCE_SETTINGS));
                }
            } catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

            Debug.LogWarning("GPS service disabled by user");
        }
    }
}