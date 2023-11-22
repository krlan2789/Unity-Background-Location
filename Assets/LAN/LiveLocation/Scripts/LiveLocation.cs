using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Android;

namespace LAN.LiveLocation {
    public class LiveLocation : MonoBehaviour {
        [SerializeField] protected string apiUrl = "wss://websocket.kuryana.id?token=user123456";
        [SerializeField] protected NetworkMethod networkMethod = NetworkMethod.WEBSOCKET;

        public event Action<string, Location> onLocationFound;

        protected static string MessageDecriptor { set; get; }
        protected static bool IsInitialized { set; get; }
        protected static bool IsUpdatingLocation { set; get; }
        public static Location LastLocation { protected set; get; }
        public static bool IsRunning { protected set; get; }
        public static bool HasPermission {
            get {
#if UNITY_ANDROID && !UNITY_EDITOR
                return LiveLocationProvider.HasPermission;
#else
                return Input.location.isEnabledByUser;
#endif
            }
        }

        protected virtual void Awake() {
            MessageDecriptor = "";
            IsInitialized = false;
            IsUpdatingLocation = false;
            LastLocation = Location.Default;
            IsRunning = false;
        }

        public virtual LiveLocation SetNetworkMethod(NetworkMethod method) {
            networkMethod = method;
            return this;
        }

        public virtual void Setup(string apiUrl, string messageDecriptor) {
            if (!string.IsNullOrEmpty(apiUrl)) this.apiUrl = apiUrl;
            if (!string.IsNullOrEmpty(messageDecriptor)) MessageDecriptor = messageDecriptor;

#if UNITY_ANDROID && !UNITY_EDITOR
            void callback(bool status) {
                if (!IsInitialized && status) {
                    bool prevStatus = false;
                    if (LiveLocationProvider.Status == LiveLocationStatus.RUNNING) {
                        prevStatus = true;
                        StopService();
                    }

                    //  Initialize Live Location
                    Debug.Log("Setting up LiveLocationProvider");
                    LiveLocationProvider.Setup();
                    LiveLocationProvider.SetupURL(this.apiUrl, networkMethod);
                    LiveLocationProvider.ClearHeader();
                    LiveLocationProvider.AddHeader("Authentication", "encrypted_string_api_key");
                    LiveLocationProvider.SetMessageDescriptor(MessageDecriptor);
                    IsInitialized = true;
                    if (prevStatus) StartService();
                }
                Debug.LogWarning("LiveLocation is setted up!");
            }

            if (!HasPermission) RequestPermission(callback);
            else if (!Input.location.isEnabledByUser) ActivateGPS(callback);
            else callback(true);
#else
            IsInitialized = HasPermission;
#endif
        }

        public virtual Coroutine RequestPermission(Action<bool> callback = null) {
            return StartCoroutine(RequestingPremission(callback));
        }

        protected virtual IEnumerator RequestingPremission(Action<bool> callback) {
#if UNITY_ANDROID && !UNITY_EDITOR
            Debug.Log("Requesting permissions!");
            Permission.RequestUserPermissions(LiveLocationProvider.RequiredPermissions);

            //  Initializing
            int maxWait = 20;
            while (!HasPermission && maxWait > 0) {
                yield return new WaitForSecondsRealtime(1);
                maxWait--;
            }
#endif
            Debug.Log($"Permission {(HasPermission ? "granted" : "denied")}!");
            if (HasPermission) ActivateGPS(callback);
            else callback?.Invoke(false);
            yield return null;
        }

        /// <summary>
        /// Activating the device location service / GPS
        /// </summary>
        public virtual LiveLocation ActivateGPS(Action<bool> callback = null) {
            Debug.Log("Activating GPS Services");
            if (!Input.location.isEnabledByUser) {
#if UNITY_ANDROID && !UNITY_EDITOR
                try {
                    using var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    using AndroidJavaObject currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
                    using var intentObject = new AndroidJavaObject("android.content.Intent", "android.settings.LOCATION_SOURCE_SETTINGS");
                    currentActivityObject.Call("startActivity", intentObject);
                } catch (Exception e) {
                    Debug.LogError(e.Message);
                }
#else
                Debug.LogWarning("GPS service disabled by user");
#endif
            } else {
                Debug.Log("GPS Service is activated");
                callback?.Invoke(true);
            }
            return this;
        }

        public virtual Coroutine StartService() {
            if (!IsInitialized) Setup(apiUrl, MessageDecriptor);
            return StartCoroutine(StartingService());
        }

        protected virtual IEnumerator StartingService() {
            //  Start service
            Debug.Log("Starting location services");
#if UNITY_ANDROID && !UNITY_EDITOR
            Debug.Log("LiveLocationProvider.Status: " + LiveLocationProvider.Status);
            if (LiveLocationProvider.Status == LiveLocationStatus.RUNNING) {
                Debug.LogWarning("LiveLocation service is already running");
            } else {
                LiveLocationProvider.Start();
                IsRunning = true;

                if (!IsUpdatingLocation) {
                    //  Call UpdateLocation method every second
                    InvokeRepeating(nameof(UpdateLocation), .5f, 1f);
                }
                yield break;
            }
#else
            //  Start service
            Input.location.Start();

            //  Initializing
            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
                yield return new WaitForSecondsRealtime(1);
                maxWait--;
            }

            //  Access denied
            if (maxWait < 1 || Input.location.status == LocationServiceStatus.Failed) {
                onLocationFound?.Invoke("Failed to get access or permission location", Location.Default);
                yield break;
            } else {
                //  Access Granted
                Debug.Log("Location.Status: " + Input.location.status);
                IsRunning = true;

                if (!IsUpdatingLocation) {
                    //  Call UpdateLocation method every second
                    InvokeRepeating(nameof(UpdateLocation), 2f, 1f);
                }
            }
#endif
        }

        /// <summary>
        /// Stop location services
        /// </summary>
        public virtual void StopService() {
            onLocationFound = null;
            IsRunning = false;
            CancelInvoke(nameof(UpdateLocation));
#if UNITY_ANDROID && !UNITY_EDITOR
            LiveLocationProvider.Stop();
#else
            if (Input.location.status == LocationServiceStatus.Running) Input.location.Stop();
#endif
            IsUpdatingLocation = false;
        }

        /// <summary>
        /// Update realtime location data
        /// </summary>
        public virtual void UpdateLocation() {
            IsUpdatingLocation = true;
#if UNITY_ANDROID && !UNITY_EDITOR
            if (HasPermission && IsRunning) {
                //  Access granted and it has been initialized
                LastLocation = LiveLocationProvider.LastLocation;

                //DateTime after = DateTime.Now;
                //TimeSpan duration = after.Subtract(searchDuration);

                Debug.LogWarning(JsonUtility.ToJson(LastLocation));
                onLocationFound?.Invoke(LiveLocationProvider.ErrorMessage, LastLocation);
            } else {
                onLocationFound?.Invoke("Failed to get access or permission location", Location.Default);
            }
#else
            if (HasPermission && Input.location.isEnabledByUser) {
                //  Access granted and it has been initialized
                Location location = new() {
                    latitude = Input.location.lastData.latitude,
                    longitude = Input.location.lastData.longitude,
                    accuracy = Input.location.lastData.horizontalAccuracy,
                    updateTime = new DateTime((long)Input.location.lastData.timestamp)
                };

                onLocationFound?.Invoke(null, location);
                LastLocation = location;
            } else {
                //  Service stopped
                onLocationFound?.Invoke("Failed to get access or permission location", Location.Default);
            }
#endif
        }

        public static double ToDoubleExact(string value, double defaultValue = 0) {
            double target = (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out target) ? target : defaultValue);
            return target;
        }
    }
}
