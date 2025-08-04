using IPS.Api.Ads;
using IPS.Logcat;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IPS {
    /// <summary>
    /// This is place at the first scene before splashscreen automaticaly
    /// </summary>
    public partial class Bootstrap : MonoBehaviour {

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeFirstScene() {
            var obj = new GameObject("Bootstrap").AddComponent<Bootstrap>();
        }

        void Awake() {
            var config = BootstrapConfig.Instance;
            Debug.Log($"<color=green>[Bootstrap] open_app ver={config.VersionName}, code={config.VersionCode}, uniqueId={SystemInfo.deviceUniqueIdentifier}, GAID={config.MyGAID}</color>");
            try {
                RequestGAID();
            }
            catch (Exception e) {
                Debug.LogError($"[Bootstrap] RequestGAID failed, errormsg=: {(e != null ? e.Message : string.Empty)}");
            }

            Tracking.Instance.LogOpenApp(config.VersionName, config.VersionCode);

#if UNITY_ANDROID
            IPSConfig.IsAndroid = true;
#else
            IPSConfig.IsAndroid = false;
            IPSConfig.IsIOS = true;
#endif

            Application.targetFrameRate = config.TargetFrameRate;
            Screen.sleepTimeout = config.ScreenNeverSleep ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;
            Excutor.Instance.Preload();

            if (!BootstrapConfig.Instance.IsTester) {
#if !PRODUCTION
                IPSConfig.CheatEnable = true;
#endif
#if UNITY_EDITOR || CUSTOM_DEBUG
                TurnOnFullLog();
#endif
            }
            else {
                IPSConfig.CheatEnable = true;
                TurnOnFullLog();
            }

            AwakeApi();
        }

        partial void AwakeApi();
                
        private void RequestGAID() {
            if (BootstrapConfig.Instance.IsTester || !string.IsNullOrEmpty(BootstrapConfig.Instance.MyGAID)) return;
            string result = null;
#if UNITY_ANDROID && !UNITY_EDITOR
            try {
                AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaClass client = new AndroidJavaClass("com.google.android.gms.ads.identifier.AdvertisingIdClient");
                AndroidJavaObject adInfo = client.CallStatic<AndroidJavaObject>("getAdvertisingIdInfo", currentActivity);

                result = adInfo.Call<string>("getId").ToString();
            } 
            catch { throw; }
            Debug.Log($"<color=green>GAID: {result}</color>");
#elif UNITY_IOS && !UNITY_EDITOR
            result = UnityEngine.iOS.Device.advertisingIdentifier;
            Debug.Log($"<color=green>IDFA: {result}</color>");
#endif

            if (string.IsNullOrEmpty(result)) return;

            BootstrapConfig.Instance.MyGAID = result;
            var list = BootstrapConfig.Instance.DebugDeviceGAID;
            if (list != null && list.Length > 0 && list.Contains(result)) {
                BootstrapConfig.Instance.IsTester = true;
                TurnOnFullLog();
            }
        }

        private void TurnOnFullLog() {
            if (IPSConfig.LogEnable) return;
            Debug.Log("<color=green>Turn on full log for this device</color>");

            IPSConfig.LogEnable = true;
            EventDispatcher.Instance.SetLogEnable(BootstrapConfig.Instance.EnableCoreLog);
#if !UNITY_EDITOR
            IngameLogcat.Instance.ForceShowing();
#endif
#if ADS
            AdmobSettings.Instance.EnableLog = true;
            if (AdsManager.Initialized) AdsManager.Instance.SetLogEnable(true);
#endif
        }
    }
}