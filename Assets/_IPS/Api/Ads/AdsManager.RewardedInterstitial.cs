
using IPS.Api.Ads;
using System;
using UnityEngine;

public partial class AdsManager {

    public bool HasRewardInterstitial {
        get {
#if ADS
            return (AdmobMediation.Initialized && AdmobMediation.Instance.HasRewardInterstitial);
#endif
            return false;
        }
    }

    public void ShowRewardInterstitial(string placement, Action onSuccess, Action onClosed = null, bool showMsgAdNotReady = true) {
#if ADS
        currentPlacement = placement;
        OnRewardInterEligible();
        OnShowAdStart();
        onClosed += () => {
            lastRewardShowAtTime = Time.time;
            OnShowAdFinished();
        };

        if (IPSConfig.IsEditor || (IPSConfig.CheatEnable && IsRemovedAds)) {
            onSuccess.Invoke();
            onClosed?.Invoke();
            return;
        }

//#if IS
//        if (ISMediation.Initialized && ISMediation.Instance.HasRewardInterstitial) {
//            onSuccess += OnShowAdFinished;
//            onSuccess += () => { OnRewardInterCompleted(placement); };

//            ISMediation.Instance.ShowRewardInterstitial(placement, onSuccess, onClosed);
//            return;
//        }
//#endif

//#if MAX
//        if (MaxMediation.Initialized && MaxMediation.Instance.HasRewardInterstitial) {
//            onSuccess += OnShowAdFinished;
//            onSuccess += () => { OnRewardInterCompleted(placement); };

//            MaxMediation.Instance.ShowRewardInterstitial(placement, onSuccess, onClosed);
//            return;
//        }
//#endif

        if (AdmobMediation.Initialized && AdmobMediation.Instance.HasRewardInterstitial) {
            onSuccess += OnShowAdFinished;
            onSuccess += () => { OnRewardInterCompleted(placement); };

            AdmobMediation.Instance.ShowRewardInterstitial(placement, onSuccess, onClosed);
        }
        else {
            if (showMsgAdNotReady) NoticeText.Instance.ShowNotice("AD NOT AVAILABLE!");
            onClosed?.Invoke();
        }
#else
        Logs.Log("[Ads] Turn ON ADS config first!");
        NoticeText.Instance.ShowNotice("AD NOT AVAILABLE!");
        if (onClosed != null) onClosed.Invoke();
#endif
    }


}