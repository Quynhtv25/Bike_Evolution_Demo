
using IPS;
using IPS.Api.Ads;
using System;
using UnityEngine;

public partial class AdsManager {
    
    public bool HasAOA {
        get {
#if ADS
#if MAX
            if (MaxSettings.Instance.UseAOAAd && MaxMediation.Instance.HasAOA) return true;
#endif
            return AdmobMediation.Instance.HasAOA;
#else
            return false;
#endif
        }
    }

    public void ShowAOA(string placement, Action callback = null) {
        currentPlacement = placement;
#if ADS

        if (IsRemovedAds || IsAdShowing || !HasAOA) {
            if (callback != null) callback.Invoke();
            return;
        }
        OnShowAdStart();

        //if (!IsBannerOnBottom) {
            HideBanner();
            callback += () => ShowBanner(string.Empty);
        //}

        callback += OnShowAdFinished;

#if MAX
        if (MaxMediation.Instance.HasAOA) {
            MaxMediation.Instance.ShowAOA(placement, callback);
            return;
        }
#endif
        AdmobMediation.Instance.ShowAOA(placement, callback);
#else
        if (callback != null) callback.Invoke();
#endif
    }

    public bool IsAOAShowing => IsAdShowing && (MaxSettings.Instance.UseAOAAd || AdmobSettings.Instance.UseAOAAd);
    public bool AOAResumeEnable => AdmobSettings.Instance.AoaResumeEnable;
    public bool AoaOpenFirstInstallEnable => AdmobSettings.Instance.AoaOpenFirstInstallEnable;
    public bool AoaOpenEnable => AdmobSettings.Instance.AoaOpenEnable;

    public void DestroyAOA() {
#if ADS
        if (IsAOAShowing) OnShowAdFinished();
#if MAX
        if (MaxSettings.Instance.UseAOAAd) MaxMediation.Instance.DestroyAOA();
#endif
        if (AdmobSettings.Instance.UseAOAAd) AdmobMediation.Instance.DestroyAOA();
#endif
    }
}