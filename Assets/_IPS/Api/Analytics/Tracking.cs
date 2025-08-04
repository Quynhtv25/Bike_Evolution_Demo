#if FIREBASE
using Firebase.Crashlytics;
#endif
#if BYTEBREW
using ByteBrewSDK;
#endif

using IPS;
using IPS.Api.Analytics;
using System;
using UnityEngine;
using UnityEngine.Profiling;

/**<summary> The main controller of game analytics. (Current use Facebook + Firebase + AppsFlyer + ByteBrew + Adjust + AppMetrica) </summary>*/

public enum ProgressType { Start, Fail, Complete }

public partial class Tracking : SingletonBehaviourDontDestroy<Tracking> {
    private TrackingEvent _event;
    public TrackingEvent EventName {
        get {
            if (_event != null) return _event;
            _event = TrackingSettings.Instance.eventName;
            if (_event == null) Debug.LogError("[TrackingEvent] Setup tracking event first!");
            return _event;
        }
    }

    private CustomService _customService;
    public CustomService CustomService {
        get {
            if (_customService != null) return _customService;
            _customService = TrackingSettings.Instance.customService;
            return _customService;
        }
    }

    public enum Service { All, Firebase, Facebook, AppsFlyer, Adjust, ByteBrew, AppMetrica, AllExceptAppsFlyerAdjustFacebook }

    public override void Preload() {
#if FACEBOOK
            IPSFacebookAnalytic.Instance.Preload();
#endif

#if FIREBASE
            IPSFirebaseAnalytic.Instance.Preload();
#endif

#if APPSFLYER
            IPSAppsFlyerAnalytic.Instance.Preload();
#endif

#if BYTEBREW
            IPSByteBrewAnalytics.Instance.Preload();
#endif
#if ADJUST
            IPSAdjustAnalytics.Instance.Preload();
#endif
#if APPMETRICA
            IPSAppMetricaAnalytics.Instance.Preload();
#endif

    }

    private System.Diagnostics.Stopwatch timer;

    public void StartTimer() {
        if (timer == null) timer = new System.Diagnostics.Stopwatch();
        timer.Start();
    }
    public void StopTimer() => timer.Stop();
    public float TimerSeconds => timer.ElapsedMilliseconds / 1000f;

    protected override void OnAwake() {
        int retentionDay = Retention;
        engagement_timestamp = DateTime.Now;
        SetTotalEngagementMinutesToday(retentionDay, TotalEngagementMinutesToday(retentionDay) + ((DateTime.Now - engagement_timestamp).TotalMinutes));
        Application.lowMemory += OnLowMemory;
        OnAwakeGames();
        OnAwakeAds();
        OnAwakePurchase();
    }

    protected override void OnDestroy() {
        Application.lowMemory -= OnLowMemory;
        base.OnDestroy();
    }

    partial void OnAwakeAds();
    partial void OnAwakeGames();
    partial void OnAwakePurchase();

   private void OnLowMemory() {
        try {
            int allocated = (int)(Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024));
            int totalRamMB = SystemInfo.systemMemorySize;
            int unusedReserved = (int)(Profiler.GetTotalUnusedReservedMemoryLong() / (1024 * 1024));

            LogEvent("lowmemory", ParameterBuilder.Create("model", SystemInfo.deviceModel)
                                    .Add("total_ram", $"{totalRamMB}MB")
                                    .Add("allocated", $"{allocated}MB")
                                    .Add("unuse_reserved", $"{unusedReserved}MB")
                                , service: Service.AllExceptAppsFlyerAdjustFacebook);
#if FIREBASE
            IPSFirebaseAnalytic.Instance.LogCrashlytic($"OnLowMemory: allocated={allocated}MB, unusedReserved={unusedReserved}MB, totalSystem={totalRamMB}MB");
            System.GC.Collect();
            Resources.UnloadUnusedAssets();
    #endif
        }
        catch { throw; }
    }

    public void LogException(string className, string methodName, string msg) {
        Logs.LogError($"[Tracking][{className}.{methodName}] Exception: " + msg);
        if (msg.Length > 90) msg = msg.Substring(0, 90);

        try {
            var param = ParameterBuilder.Create(ParamErrorMsg, msg)
                                        .Add("class", className)
                                        .Add("method", methodName)
                                        .Add(ParamIsAndroid, IPSConfig.IsAndroid)
                                        .Add(ParamVersionName, Application.version)
                                        .Add(ParamVersionCode, BootstrapConfig.Instance.VersionCode);

             LogEvent("exception", param);
#if FIREBASE
            IPSFirebaseAnalytic.Instance.LogCrashlytic("[" + className + "." + methodName + "] exception: " + msg);
#endif
        } catch { throw; }
    }

    private void LogCustomService(string eventName, ParameterBuilder param) {
        if (string.IsNullOrEmpty(eventName)) return;
        CustomService?.LogCustomEvent(eventName, param);
    }
    
    /// <summary>
    /// Add a progression event (currently for ByteBrew)
    /// </summary>
    /// <param name="environment">The environment that the event is happening in (ex. Tutorial, Level)</param>
    /// <param name="stage">Stage or progression that the player is in (ex. GoldLevelArena, Level_1, tutorial_menu_purchase)</param>
    /// <param name="value">Value that ties to an event (ex. 500, -300, Chainsaw) </param>
    public void LogProgressStart(string enviroment, string stage, string value = default) {
#if BYTEBREW
        IPSByteBrewAnalytics.Instance.LogProgressStart(enviroment, stage, value);
#endif
    }

    /// <summary>
    /// Add a progression event (currently for ByteBrew)
    /// </summary>
    /// <param name="environment">The environment that the event is happening in (ex. Tutorial, Level)</param>
    /// <param name="stage">Stage or progression that the player is in (ex. GoldLevelArena, Level_1, tutorial_menu_purchase)</param>
    /// <param name="value">Value that ties to an event (ex. 500, -300, Chainsaw) </param>
    public void LogProgressFail(string enviroment, string stage, string value = default) {
#if BYTEBREW
        IPSByteBrewAnalytics.Instance.LogProgressFail(enviroment, stage, value);
#endif
    }

    /// <summary>
    /// Add a progression event (currently for ByteBrew)
    /// </summary>
    /// <param name="environment">The environment that the event is happening in (ex. Tutorial, Level)</param>
    /// <param name="stage">Stage or progression that the player is in (ex. GoldLevelArena, Level_1, tutorial_menu_purchase)</param>
    /// <param name="value">Value that ties to an event (ex. 500, -300, Chainsaw) </param>
    public void LogProgressComplete(string enviroment, string stage, string value = default) {
#if BYTEBREW
        IPSByteBrewAnalytics.Instance.LogProgressComplete(enviroment, stage, value);
#endif
    }

    public void LogEvent(string eventName, string eventParam, string paramValue, Service service = Service.AllExceptAppsFlyerAdjustFacebook) {
        if (string.IsNullOrEmpty(eventName)) return;

        if (eventName.Length >= 40) {
            Logs.LogError($"Event `{eventName}` length={eventName.Length} too long! (must < 40)");
            return;
        }

        bool nullParam = string.IsNullOrEmpty(eventParam) || string.IsNullOrEmpty(paramValue);
//#if UNITY_EDITOR
        Logs.Log($"<color=yellow>[Tracking] eventName={eventName}, eventParam={eventParam}, paramValue={paramValue}</color>");
//#endif

#if FACEBOOK
            if (service == Service.All || service == Service.Facebook) {
                if (nullParam) IPSFacebookAnalytic.Instance.LogEvent(eventName);
                else IPSFacebookAnalytic.Instance.LogEvent(eventName, eventParam, paramValue);
            }
#endif

#if FIREBASE
        if (service == Service.All || service == Service.Firebase || service == Service.AllExceptAppsFlyerAdjustFacebook) {
                if (nullParam) IPSFirebaseAnalytic.Instance.LogEvent(eventName);
                else IPSFirebaseAnalytic.Instance.LogEvent(eventName, eventParam, paramValue);
            }
#endif

#if APPSFLYER
            if (service == Service.All || service == Service.AppsFlyer) {
                if (nullParam) IPSAppsFlyerAnalytic.Instance.LogEvent(eventName);
                else IPSAppsFlyerAnalytic.Instance.LogEvent(eventName, eventParam, paramValue);
            }
#endif

#if BYTEBREW
        if (service == Service.All || service == Service.ByteBrew || service == Service.AllExceptAppsFlyerAdjustFacebook) {
            if (nullParam) IPSByteBrewAnalytics.Instance.LogEvent(eventName);
            else IPSByteBrewAnalytics.Instance.LogEvent(eventName, eventParam, paramValue);
        }
#endif
        LogCustomService(eventName, ParameterBuilder.Create(eventParam, paramValue));
    }

    public void LogEvent(string eventName, ParameterBuilder builder = null, Service service = Service.AllExceptAppsFlyerAdjustFacebook) {
        if (string.IsNullOrEmpty(eventName)) return;
        
        if (eventName.Length >= 40) {
            Logs.LogError($"Event `{eventName}` length={eventName.Length} too long! (must < 40)");
            return;
        }

//#if UNITY_EDITOR
        Logs.Log($"<color=yellow>[Tracking] eventName={eventName}, param={(builder != null ? builder.ToString() : "null")}</color>");
//#endif

#if FACEBOOK
            if (service == Service.All || service == Service.Facebook) {
                IPSFacebookAnalytic.Instance.LogEvent(eventName, builder);
            }
#endif

#if FIREBASE
        if (service == Service.All || service == Service.Firebase || service == Service.AllExceptAppsFlyerAdjustFacebook) {
                IPSFirebaseAnalytic.Instance.LogEvent(eventName, builder);
            }
#endif

#if APPSFLYER
            if (service == Service.All || service == Service.AppsFlyer) {
                IPSAppsFlyerAnalytic.Instance.LogEvent(eventName, builder);
            }
#endif
            
#if BYTEBREW
            if (service == Service.All || service == Service.ByteBrew || service == Service.AllExceptAppsFlyerAdjustFacebook) {
                IPSByteBrewAnalytics.Instance.LogEvent(eventName, builder);
            }
#endif
            LogCustomService(eventName, builder);
    }

    public void SetUserProperty(string propertyName, string propertyValue) {
        if (string.IsNullOrEmpty(propertyName) || string.IsNullOrEmpty(propertyValue)) return;
//#if UNITY_EDITOR
        Logs.Log($"[Tracking] SetUserProperty: [{propertyName}:{propertyValue}]");
//#endif

#if FIREBASE
        IPSFirebaseAnalytic.Instance.SetUserProperty(propertyName, propertyValue);
#endif
        
#if BYTEBREW
            IPSByteBrewAnalytics.Instance.SetUserProperty(propertyName, propertyValue);
#endif

            CustomService?.LogUserProperty(propertyName, propertyValue);
    }
}
