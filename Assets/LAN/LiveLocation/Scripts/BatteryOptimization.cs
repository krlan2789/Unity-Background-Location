using AnakPintar.Android;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnakPintar.LiveLocation {
    public static class BatteryOptimization {
        private static KeyValuePair<string, string>[] POWER_MANAGER_COMPONENTS_NAME = new KeyValuePair<string, string>[] {
            KeyValuePair.Create("com.samsung.android.lool", "com.samsung.android.sm.battery.ui.BatteryActivity"),
            KeyValuePair.Create("com.samsung.android.lool", "com.samsung.android.sm.ui.battery.BatteryActivity"),
            KeyValuePair.Create("com.miui.securitycenter", "com.miui.permcenter.autostart.AutoStartManagementActivity"),
            KeyValuePair.Create("com.coloros.safecenter", "com.coloros.safecenter.permission.startup.StartupAppListActivity"),
            KeyValuePair.Create("com.coloros.safecenter", "com.coloros.safecenter.startupapp.StartupAppListActivity"),
            KeyValuePair.Create("com.oppo.safe", "com.oppo.safe.permission.startup.StartupAppListActivity"),
            KeyValuePair.Create("com.vivo.permissionmanager", "com.vivo.permissionmanager.activity.BgStartUpManagerActivity"),
            KeyValuePair.Create("com.asus.mobilemanager", "com.asus.mobilemanager.MainActivity"),
            KeyValuePair.Create("com.huawei.systemmanager", "com.huawei.systemmanager.startupmgr.ui.StartupNormalAppListActivity"),
            KeyValuePair.Create("com.huawei.systemmanager", "com.huawei.systemmanager.optimize.process.ProtectActivity"),
            KeyValuePair.Create("com.huawei.systemmanager", "com.huawei.systemmanager.appcontrol.activity.StartupAppControlActivity"),
            KeyValuePair.Create("com.iqoo.secure", "com.iqoo.secure.ui.phoneoptimize.AddWhiteListActivity"),
            KeyValuePair.Create("com.iqoo.secure", "com.iqoo.secure.ui.phoneoptimize.BgStartUpManager"),
            KeyValuePair.Create("com.letv.android.letvsafe", "com.letv.android.letvsafe.AutobootManageActivity"),
            KeyValuePair.Create("com.transsion.phonemanager", "com.itel.autobootmanager.activity.AutoBootMgrActivity"),
            KeyValuePair.Create("com.htc.pitroad", "com.htc.pitroad.landingpage.activity.LandingPageActivity"),
            KeyValuePair.Create("com.google.android.apps.turbo", "com.google.android.libraries.smartbattery.appusage.library.EvaluateAppBucketsJo"),    //  Sony Xperia XZ3
        };

#if UNITY_ANDROID && !UNITY_EDITOR
        private static AndroidJavaObject unityActivity = null;
        private static AndroidJavaObject powerManager = null;
#endif

        public static bool IsIgnoringBatteryOptimizations {
            get {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (powerManager == null) Setup();
                if (powerManager == null) return true;
                Debug.Log("PowerManager not null");
                return powerManager.Call<bool>("isIgnoringBatteryOptimizations", UnityActivityJavaClass.PackageName);
#else
                return true;
#endif
            }
        }

        public static void Setup() {
#if UNITY_ANDROID && !UNITY_EDITOR
            unityActivity = UnityActivityJavaClass.CurrentActivity;
            powerManager = UnityActivityJavaClass.GetSystemService("power");
#endif
        }

        public static void OpenSetting() {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (unityActivity == null) Setup();
            if (unityActivity == null) return;

            try {
                int sdkVersion = UnityActivityJavaClass.VERSION_INFO_CLASS.GetStatic<int>("SDK_INT");

                if (sdkVersion > 23 && !IsIgnoringBatteryOptimizations) {
                    bool success = TryStartPowerManagerByVendor();
                    if (!success) {
                        success = UnityActivityJavaClass.TryStartActivity(UnityActivityJavaClass.CreateIntent(Settings.IGNORE_BATTERY_OPTIMIZATION_SETTINGS).Call<AndroidJavaObject>("setData", UnityActivityJavaClass.URI_CLASS.CallStatic<AndroidJavaObject>("parse", "package:" + UnityActivityJavaClass.PackageName)));
                        if (!success) {
                            success = UnityActivityJavaClass.TryStartActivity(UnityActivityJavaClass.CreateIntent(Settings.IGNORE_BATTERY_OPTIMIZATION_SETTINGS));
                            if (!success) {
                                success = UnityActivityJavaClass.TryStartActivity(UnityActivityJavaClass.CreateIntent(Settings.APPLICATION_DETAILS_SETTINGS).Call<AndroidJavaObject>("setData", UnityActivityJavaClass.URI_CLASS.CallStatic<AndroidJavaObject>("parse", "package:" + UnityActivityJavaClass.PackageName)));
                                if (!success) {
                                    UnityActivityJavaClass.TryStartActivity(UnityActivityJavaClass.CreateIntent(Settings.APPLICATION_DETAILS_SETTINGS));
                                }
                            }
                        }
                    }
                }
            } catch (Exception e) {
                Debug.LogWarning("Exception: " + e.Message);
            }
#endif
            Debug.Log("Start battery optimization");
        }

        public static bool TryStartPowerManagerByVendor() {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (unityActivity == null) Setup();
            if (unityActivity == null) return false;

            foreach (var kvp in POWER_MANAGER_COMPONENTS_NAME) {
                var intentObject = UnityActivityJavaClass.CreateIntentWithComponent(kvp.Key, kvp.Value);
                var hasActivity = UnityActivityJavaClass.GetPackageManager().Call<AndroidJavaObject>(UnityActivityJavaClass.RESOLVE_ACTIVITY_METHOD, intentObject, UnityActivityJavaClass.PACKAGE_MANAGER_CLASS.GetStatic<int>(UnityActivityJavaClass.PACKAGE_MANAGER_MATCH_DEFAULT_ONLY_PROP));
                if (hasActivity != null) {
                    Debug.Log($"{kvp.Key} : {kvp.Value}");
                    UnityActivityJavaClass.StartActivity(intentObject);
                    return true;
                } else {
                    Debug.Log($"Not found: {kvp.Key} : {kvp.Value}");
                }
            }
#endif
            return false;
        }
    }
}