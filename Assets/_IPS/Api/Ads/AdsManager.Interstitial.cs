
using IPS;
using IPS.Api.Ads;
using System;
using UnityEngine;
#if LION
using LionStudios.Suite.Ads;
#endif

public partial class AdsManager {

    public bool HasInterstitial {
        get {
#if LION
            return LionAds.IsInterstitialReady;
#endif
#if ADS
#if IS
            return !IsRemovedAds && (ISSettings.Instance.UseInterstitialAd && ISMediation.Initialized && ISMediation.Instance.HasInterstitial) 
                                    || ((AdmobMediation.Initialized && AdmobMediation.Instance.HasInterstitial));
#elif MAX
            return !IsRemovedAds && (MaxSettings.Instance.UseInterstitialAd && MaxMediation.Initialized && MaxMediation.Instance.HasInterstitial) 
                                    || ((AdmobMediation.Initialized && AdmobMediation.Instance.HasInterstitial));
#else
            return !IsRemovedAds && ((AdmobMediation.Initialized && AdmobMediation.Instance.HasInterstitial));
#endif
#endif
            return false;
        }
    }

    private bool InterResumeEnable {
        get {
#if ADS
            return AdmobSettings.Instance.InterResumeEnable;
#endif
            return false;
        }
    }

    public bool InterCappingReady {
        get {
#if ADS
            return Time.time - lastInterShowAtTime >= AdmobSettings.Instance.InterCapping 
                && Time.time - lastRewardShowAtTime >= AdmobSettings.Instance.InterAfterRewardCapping;
#endif
            return false;
        }
    }

    public bool InterPlayTimesReady => UserData.PlayTimes >= AdmobSettings.Instance.InterFromPlayTimes;
    public bool InterLevelReady => Tracking.CurrentLevel >= AdmobSettings.Instance.InterFromLevel;
    public bool InterSecondsReady => Time.time >= AdmobSettings.Instance.InterFromSeconds;

    public bool InterScriptReady => InterPlayTimesReady && InterLevelReady && InterSecondsReady;

    private void LoadInterMediation(Mediation mediation, string placement = default) {
        Logs.Log($"[Ads] LoadInterMediation triggered,  last mediation={mediation}");
#if ADS
        if (mediation != Mediation.admob && AdmobSettings.Instance.UseInterstitialAd) {
            Logs.Log($"[Ads] LoadInterMediation triggered,  next mediation={Mediation.admob}");
            AdmobMediation.Instance.RequestInterstitial();
            return;
        }
#if IS
        if (ISSettings.Instance.UseInterstitialAd) {
            Logs.Log($"[Ads] LoadInterMediation triggered,  next mediation={Mediation.ironSource}");
            ISMediation.Instance.RequestInterstitial();
            return;
        }
#endif
#if MAX
        if (MaxSettings.Instance.UseInterstitialAd) {
            Logs.Log($"[Ads] LoadInterMediation triggered,  next mediation={Mediation.appLovin}");
            MaxMediation.Instance.RequestInterstitial();
            return;
        }
#endif
#endif
    }

    public void ShowInterstitial(string placement, Action callback = null, bool forceShow = false) {
        currentPlacement = placement;

        if (!forceShow) {
            bool scriptReady = InterPlayTimesReady && InterLevelReady && InterSecondsReady;
            if (!scriptReady) {
                if (callback != null) callback.Invoke();
                Logs.Log($"[Ads] Interstitial not ready by script playtimes={InterPlayTimesReady}, level={InterLevelReady}, seconds={InterSecondsReady}");
                return;
            }
        }

        bool forceComplete = IsRemovedAds || (!forceShow && !InterCappingReady);
        OnInterEligible(!forceComplete, (int)AdmobSettings.Instance.InterFromPlayTimes, (int)AdmobSettings.Instance.InterFromLevel);

        if (forceComplete) {
            callback?.Invoke();
            return;
        }

        OnShowAdStart();
        callback += OnShowAdFinished;

#if LION
        try {
            Logs.Log($"[ADS][Lion] ShowInterstitial placement={placement}");
            OnInterDisplayed(Mediation.appLovin);
            callback += () => {
                lastInterShowAtTime = Time.time;
                OnInterClosed(Mediation.appLovin);
            };
            LionAds.TryShowInterstitial(placement, callback);
        }
        catch (Exception e) {
            if (callback != null) callback.Invoke();
            Tracking.Instance.LogException(typeof(AdsManager).Name, nameof(ShowInterstitial), e != null ? e.Message : "Unknow");
        }
        return;
#endif

#if ADS

#if IS
        if (ISSettings.Instance.UseInterstitialAd && ISMediation.Initialized && ISMediation.Instance.HasInterstitial) {
            callback += () => { lastInterShowAtTime = Time.time; };
            ISMediation.Instance.ShowInterstitial(placement, callback);
            return;
        }
#endif
#if MAX
        if (MaxSettings.Instance.UseInterstitialAd && MaxMediation.Initialized && MaxMediation.Instance.HasInterstitial) {
            callback += () => { lastInterShowAtTime = Time.time; };
            MaxMediation.Instance.ShowInterstitial(placement, callback);
            return;
        }
#endif
        if (AdmobMediation.Initialized && AdmobMediation.Instance.HasInterstitial) {
            callback += () => { lastInterShowAtTime = Time.time; };
            AdmobMediation.Instance.ShowInterstitial(placement, callback);
        }
        else callback?.Invoke();
#else
        Logs.Log("[Ads] Turn ON ADS config first!");
        if (callback != null) callback.Invoke();
#endif
    }

}