
using IPS.Api.Ads;
using System;
using UnityEngine;

using System.Collections.Generic;

#if LION
using LionStudios.Suite.Ads;
#endif

public partial class AdsManager {

    public bool HasRewardVideo {
        get {
#if LION
            return LionAds.IsRewardedReady;
#endif

#if ADS
#if IS
            return (ISSettings.Instance.UseRewardedVideoAd && ISMediation.Initialized && ISMediation.Instance.HasRewardVideo)
                || (AdmobMediation.Initialized && AdmobMediation.Instance.HasRewardVideo);
#elif MAX
            return (MaxSettings.Instance.UseRewardedVideoAd && MaxMediation.Initialized && MaxMediation.Instance.HasRewardVideo)
                || (AdmobMediation.Initialized && AdmobMediation.Instance.HasRewardVideo);
#else
            return (AdmobMediation.Initialized && AdmobMediation.Instance.HasRewardVideo);
#endif
#endif
            return false;
        }
    }

    private void LoadRewardVideoMediation(Mediation lastMediation, string placement = default) {
#if ADS
        Logs.Log($"[Ads] LoadRewardVideoMediation triggered,  last mediation={lastMediation}");

        if (lastMediation != Mediation.admob && AdmobSettings.Instance.UseRewardedVideoAd) {
            Logs.Log($"[Ads] LoadRewardVideoMediation triggered,  next mediation={Mediation.admob}");
            AdmobMediation.Instance.RequestRewardVideo();
            return;
        }
#if IS
        if (ISSettings.Instance.UseRewardedVideoAd) {
            Logs.Log($"[Ads] LoadRewardVideoMediation triggered,  next mediation={Mediation.ironSource}");
            ISMediation.Instance.RequestRewardVideo();
            return;
        }
#endif
#if MAX
        if (MaxSettings.Instance.UseRewardedVideoAd) {
            Logs.Log($"[Ads] LoadRewardVideoMediation triggered,  next mediation={Mediation.appLovin}");
            MaxMediation.Instance.RequestRewardVideo();
            return;
        }
#endif

        if (AdmobSettings.Instance.UseRewardedVideoAd) {
            Logs.Log($"[Ads] LoadRewardVideoMediation triggered, lastMediation={lastMediation},  next mediation={Mediation.admob}");
            AdmobMediation.Instance.RequestRewardVideo();
            return;
        }
#endif
    }

    public void ShowRewardVideo(string placement, Action onSuccess, Action onClosed = null, bool showMsgAdNotReady = true) {
        currentPlacement = placement;
        OnRewardVideoEligible();
        OnShowAdStart();
        onClosed += () => {
            lastRewardShowAtTime = Time.time;
            OnShowAdFinished();
        };

        if (IPSConfig.IsEditor || (IPSConfig.CheatEnable && IsRemovedAds)) {
            Logs.Log("[Ads] Show RewardedVideo force success & close!");
            onSuccess.Invoke();
            onClosed?.Invoke();
            return;
        }

#if LION
        try {
            if (!HasRewardVideo) {
                if (showMsgAdNotReady) NoticeText.Instance.ShowNotice("AD NOT READY!");
                if (onClosed != null) onClosed.Invoke();
                return;
            }
            Logs.Log($"[ADS][Lion] ShowRewardVideo placement={placement}");
            OnRewardVideoDisplayed(Mediation.appLovin);
            onSuccess += () => {
                OnRewardVideoCompleted(currentPlacement);
            };
            LionAds.TryShowRewarded(placement, onSuccess, onClosed, null);
        }
        catch (Exception e) {
#if PRODUCTION
            if (showMsgAdNotReady) NoticeText.Instance.ShowNotice("AD NOT READY!");
            if (onClosed != null) onClosed.Invoke();
#else
            if (onSuccess != null) onSuccess.Invoke();
            if (onClosed != null) onClosed.Invoke();
#endif
            Tracking.Instance.LogException(typeof(AdsManager).Name, nameof(ShowRewardVideo), e != null ? e.Message : "Unknow");
        }
        return;
#endif

#if ADS
#if IS
        if (ISMediation.Initialized && ISMediation.Instance.HasRewardVideo) {
            onSuccess += OnShowAdFinished;
            onSuccess += () => { OnRewardVideoCompleted(placement); };

            ISMediation.Instance.ShowRewardVideo(placement, onSuccess, onClosed);
            return;
        }
#endif

#if MAX
        if (MaxSettings.Instance.UseRewardedVideoAd && MaxMediation.Initialized && MaxMediation.Instance.HasRewardVideo) {
            onSuccess += OnShowAdFinished;
            onSuccess += () => { OnRewardVideoCompleted(placement); };

            MaxMediation.Instance.ShowRewardVideo(placement, onSuccess, onClosed);
            return;
        }
#endif

        if (AdmobMediation.Initialized && AdmobMediation.Instance.HasRewardVideo) {
            onSuccess += OnShowAdFinished;
            onSuccess += () => { OnRewardVideoCompleted(placement); };

            AdmobMediation.Instance.ShowRewardVideo(placement, onSuccess, onClosed);
        }
        else {
            if (showMsgAdNotReady) NoticeText.Instance.ShowNotice("AD NOT READY!");
            onClosed?.Invoke();
        }
#else
            Logs.Log("[Ads] Turn ON ADS config first!");
        //if (showMsgAdNotReady) NoticeText.Instance.ShowNotice("AD NOT AVAILABLE!");
        if (onSuccess != null) onSuccess.Invoke();
        if (onClosed != null) onClosed.Invoke();
#endif
    }


}