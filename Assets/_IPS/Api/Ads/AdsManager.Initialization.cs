
using IPS;
using IPS.Api.Ads;
using System.Collections;
using UnityEngine;

public partial class AdsManager {
    public void ShowDebugger() {
#if LION
        MaxSdk.ShowMediationDebugger();
#endif
#if IS
        ISMediation.Instance.ShowDebugger();
#endif
#if MAX
        MaxMediation.Instance.ShowDebugger();
#endif
    }

    public void SetLogEnable(bool enable) {
#if ADS
        if (!Initialized) return;
        if (AdmobMediation.Initialized) AdmobMediation.Instance.SetLogEnable(enable);
#if MAX
        if (MaxMediation.Initialized) MaxMediation.Instance.SetLogEnable(enable);
#endif
#if IS
        if (ISMediation.Initialized) ISMediation.Instance.SetLogEnable(enable);
#endif
#endif
    }

    partial void InitMediation() {
#if ADS
        Debug.Log($"[Ads] Initializing... log={AdmobSettings.Instance.EnableLog}");
        SetLogEnable(AdmobSettings.Instance.EnableLog);

        InitAdmob();
        if (AdmobSettings.Instance.EnableUMP) {
            StartCoroutine(IEWaitingForConsent());
        }
        else {
            DoInit();
        }

#endif
    }

    private IEnumerator IEWaitingForConsent() {
        yield return new WaitUntil(() => { return UserData.GDPRConsentRequested; });
        DoInit();
    }

    private void DoInit() {
#if !IS && !MAX
        if (AdmobSettings.Instance.CollapsibleFallbackEnable) {
            AdmobSettings.Instance.CollapsibleFallbackEnable = false;
        }
#endif
        InitIS();
        InitMax();
        RestoreRemoveAd();
        SetAutoRequest();
    }

    private void SetAutoRequest() {
#if ADS
        if (!AdmobSettings.Instance.UseBannerAd) {
#if IS
            ISMediation.Instance.SetBannerAutoRequest(true);
#endif
#if MAX
            MaxMediation.Instance.SetBannerAutoRequest(true);
#endif
        }
        else if (!AdmobSettings.Instance.CollapsibleFallbackEnable || !UseBannerMediation) {
            AdmobMediation.Instance.SetBannerAutoRequest(true);
        }


        if (!AdmobSettings.Instance.UseMRecAd) {
#if IS
            ISMediation.Instance.SetMRecAutoRequest(true);
#endif
#if MAX
            MaxMediation.Instance.SetMRecAutoRequest(true);
#endif
        }
        else if (!UseMRecMediation) {
            AdmobMediation.Instance.SetMRecAutoRequest(true);
        }


        if (!AdmobSettings.Instance.UseInterstitialAd) {
#if MAX
            MaxMediation.Instance.SetInterAutoRequest(true);
#endif
#if IS
            ISMediation.Instance.SetInterAutoRequest(true);
#endif
        }
        else if (!UseInterMediation) {
            AdmobMediation.Instance.SetInterAutoRequest(true);
        }
        else LoadInterMediation(Mediation.admob);


        if (!AdmobSettings.Instance.UseRewardedVideoAd) {
#if MAX
            MaxMediation.Instance.SetRewardVideoAutoRequest(true);
#endif
#if IS
            ISMediation.Instance.SetRewardVideoAutoRequest(true);
#endif
        }
        else if (!UseRewardMediation) {
            AdmobMediation.Instance.SetRewardVideoAutoRequest(true);
        }
        else LoadRewardVideoMediation(Mediation.admob);
#endif
    }

    private bool UseBannerMediation {
        get {
#if MAX
            return MaxSettings.Instance.UseBannerAd;
#elif IS
            return ISSettings.Instance.UseBannerAd;
#endif
            return false;
        }
    }
     
    private bool UseMRecMediation {
        get {
#if MAX
            return MaxSettings.Instance.UseMRecAd;
#elif IS
            return ISSettings.Instance.UseMRecAd;
#endif
            return false;
        }
    }

    private bool UseInterMediation {
        get {
#if MAX
            return MaxSettings.Instance.UseInterstitialAd;
#elif IS
            return ISSettings.Instance.UseInterstitialAd;
#endif
            return false;
        }
    }

    private bool UseRewardMediation {
        get {
#if MAX
            return MaxSettings.Instance.UseRewardedVideoAd;
#elif IS
            return ISSettings.Instance.UseRewardedVideoAd;
#endif
            return false;
        }
    }

    partial void DestroyMediation() {
#if ADS
        DestroyAdmob();
        DestroyIS();
        DestroyMax();
#endif
    }

    private void InitAdmob() {
#if ADMOB
        var settings = AdmobSettings.Instance;
        if (settings == null) return;

        if (settings.UseInterstitialAd) {
            lastInterShowAtTime = -100;
            lastRewardShowAtTime = -100;
        }

        AdmobMediation.onAdPaid += OnAdPaid;
        AdmobMediation.onBannerChanged += OnBannerHeightChanged;
        AdmobMediation.onBannerDisplayed += OnBannerDisplayed;
        AdmobMediation.onBannerNewRequest += LoadBannerMediation;

        AdmobMediation.onAdClicked += OnAdClicked;
        AdmobMediation.onAdFailedToLoad += OnAdFailedToLoad;
        AdmobMediation.onAdFailedToShow += OnAdFailedToShow;

        AdmobMediation.onMRecDisplayed += OnMRecDisplayed;
        AdmobMediation.onMRecNewRequest += LoadMRecMediation;

        AdmobMediation.onAOALoaded += OnAOAAvailable;
        AdmobMediation.onAOADisplayed += OnAOADisplayed;

        AdmobMediation.onInterLoaded += OnInterAvailable;
        AdmobMediation.onInterFailedToLoad += OnInterFailedToLoad;
        AdmobMediation.onInterDisplayed += OnInterDisplayed;
        AdmobMediation.onInterClosed += OnInterClosed;
        AdmobMediation.onInterClicked += OnInterClicked;
        AdmobMediation.onInterNewRequest += LoadInterMediation;

        AdmobMediation.onRewardVideoLoaded += OnRewardVideoAvailable;
        AdmobMediation.onRewardVideoFailedToLoad += OnRewardVideoFailedToLoad;
        AdmobMediation.onRewardVideoDisplayed += OnRewardVideoDisplayed;
        AdmobMediation.onRewardVideoNewRequest += LoadRewardVideoMediation;

        AdmobMediation.onRewardInterLoaded += OnRewardInterAvailable;
        AdmobMediation.onRewardInterFailedToLoad += OnRewardInterFailedToLoad;
        AdmobMediation.onRewardInterDisplayed += OnRewardInterDisplayed;

        AdmobMediation.Instance.Preload();
#endif
    }

    private void DestroyAdmob() {
#if ADMOB
        if (!AdmobMediation.Initialized) return;
        AdmobMediation.onAdPaid -= OnAdPaid;
        AdmobMediation.onBannerChanged -= OnBannerHeightChanged;
        AdmobMediation.onBannerDisplayed -= OnBannerDisplayed;
        AdmobMediation.onBannerNewRequest -= LoadBannerMediation;

        AdmobMediation.onAdClicked -= OnAdClicked;
        AdmobMediation.onAdFailedToShow -= OnAdFailedToShow;

        AdmobMediation.onMRecDisplayed -= OnMRecDisplayed;
        AdmobMediation.onMRecNewRequest -= LoadMRecMediation;

        AdmobMediation.onAOALoaded -= OnAOAAvailable;
        AdmobMediation.onAdFailedToLoad -= OnAdFailedToLoad;
        AdmobMediation.onAOADisplayed -= OnAOADisplayed;

        AdmobMediation.onInterLoaded -= OnInterAvailable;
        AdmobMediation.onInterFailedToLoad -= OnInterFailedToLoad;
        AdmobMediation.onInterDisplayed -= OnInterDisplayed;
        AdmobMediation.onInterClosed -= OnInterClosed;
        AdmobMediation.onInterClicked -= OnInterClicked;
        AdmobMediation.onInterNewRequest -= LoadInterMediation;

        AdmobMediation.onRewardVideoLoaded -= OnRewardVideoAvailable;
        AdmobMediation.onRewardVideoFailedToLoad -= OnRewardVideoFailedToLoad;
        AdmobMediation.onRewardVideoDisplayed -= OnRewardVideoDisplayed;
        AdmobMediation.onRewardVideoNewRequest -= LoadRewardVideoMediation;

        AdmobMediation.onRewardInterLoaded -= OnRewardInterAvailable;
        AdmobMediation.onRewardInterFailedToLoad -= OnRewardInterFailedToLoad;
        AdmobMediation.onRewardInterDisplayed -= OnRewardInterDisplayed;
#endif
    }

    private void InitMax() {
#if ADS && MAX
        var settings = MaxSettings.Instance;
        if (settings == null) return;

        if (settings.UseInterstitialAd) {
            lastInterShowAtTime = -100;
            lastRewardShowAtTime = -100;
        }

        MaxMediation.onAdPaid += OnAdPaid;
        MaxMediation.onBannerChanged += OnBannerHeightChanged;
        MaxMediation.onBannerDisplayed += OnBannerDisplayed;
        MaxMediation.onBannerNewRequest += LoadBannerMediation;

        MaxMediation.onAdClicked += OnAdClicked;
        MaxMediation.onAdFailedToLoad += OnAdFailedToLoad;
        MaxMediation.onAdFailedToShow += OnAdFailedToShow;
        
        MaxMediation.onAOALoaded += OnAOAAvailable;
        MaxMediation.onAOADisplayed += OnAOADisplayed;

        MaxMediation.onMRecDisplayed += OnMRecDisplayed;
        MaxMediation.onMRecNewRequest += LoadMRecMediation;

        MaxMediation.onInterLoaded += OnInterAvailable;
        MaxMediation.onInterFailedToLoad += OnInterFailedToLoad;
        MaxMediation.onInterDisplayed += OnInterDisplayed;
        MaxMediation.onInterClosed += OnInterClosed;
        MaxMediation.onInterNewRequest += LoadInterMediation;

        MaxMediation.onRewardVideoLoaded += OnRewardVideoAvailable;
        MaxMediation.onRewardVideoFailedToLoad += OnRewardVideoFailedToLoad;
        MaxMediation.onRewardVideoDisplayed += OnRewardVideoDisplayed;
        MaxMediation.onRewardVideoNewRequest += LoadRewardVideoMediation;

        
        MaxMediation.onRewardInterLoaded += OnRewardInterAvailable;
        MaxMediation.onRewardInterFailedToLoad += OnRewardInterFailedToLoad;
        MaxMediation.onRewardInterDisplayed += OnRewardInterDisplayed;

        MaxMediation.Instance.Preload();
#endif

    }

    private void DestroyMax() {
#if ADS && MAX
        if (!MaxMediation.Initialized) return;
        MaxMediation.onAdPaid -= OnAdPaid;
        MaxMediation.onBannerChanged -= OnBannerHeightChanged;
        MaxMediation.onBannerDisplayed -= OnBannerDisplayed;
        MaxMediation.onBannerNewRequest -= LoadBannerMediation;

        MaxMediation.onAdClicked -= OnAdClicked;
        MaxMediation.onAdFailedToLoad -= OnAdFailedToLoad;
        MaxMediation.onAdFailedToShow -= OnAdFailedToShow;
               
        MaxMediation.onAOALoaded -= OnAOAAvailable;
        MaxMediation.onAOADisplayed -= OnAOADisplayed;

        MaxMediation.onMRecDisplayed -= OnMRecDisplayed;
        MaxMediation.onMRecNewRequest -= LoadMRecMediation;

        MaxMediation.onInterLoaded -= OnInterAvailable;
        MaxMediation.onInterFailedToLoad -= OnInterFailedToLoad;
        MaxMediation.onInterDisplayed -= OnInterDisplayed;
        MaxMediation.onInterClosed -= OnInterClosed;
        MaxMediation.onInterNewRequest -= LoadInterMediation;

        MaxMediation.onRewardVideoLoaded -= OnRewardVideoAvailable;
        MaxMediation.onRewardVideoFailedToLoad -= OnRewardVideoFailedToLoad;
        MaxMediation.onRewardVideoDisplayed -= OnRewardVideoDisplayed;
        MaxMediation.onRewardVideoNewRequest -= LoadRewardVideoMediation;

        MaxMediation.onRewardInterLoaded -= OnRewardInterAvailable;
        MaxMediation.onRewardInterFailedToLoad -= OnRewardInterFailedToLoad;
        MaxMediation.onRewardInterDisplayed -= OnRewardInterDisplayed;
#endif
    }

    private void InitIS() {
#if ADS && IS
        var settings = ISSettings.Instance;
        if (settings == null) return;

        if (settings.UseInterstitialAd) {
            lastInterShowAtTime = -100;
            lastRewardShowAtTime = -100;
        }

        ISMediation.onAdPaid += OnAdPaid;
        ISMediation.onBannerChanged += OnBannerHeightChanged;
        ISMediation.onBannerDisplayed += OnBannerDisplayed;
        ISMediation.onBannerNewRequest += LoadBannerMediation;

        ISMediation.onAdClicked += OnAdClicked;
        ISMediation.onAdFailedToLoad += OnAdFailedToLoad;
        ISMediation.onAdFailedToShow += OnAdFailedToShow;
        
        ISMediation.onMRecDisplayed += OnMRecDisplayed;
        ISMediation.onMRecNewRequest += LoadMRecMediation;

        ISMediation.onInterLoaded += OnInterAvailable;
        ISMediation.onInterFailedToLoad += OnInterFailedToLoad;
        ISMediation.onInterDisplayed += OnInterDisplayed;
        ISMediation.onInterClosed += OnInterClosed;
        ISMediation.onInterNewRequest += LoadInterMediation;

        ISMediation.onRewardVideoLoaded += OnRewardVideoAvailable;
        ISMediation.onRewardFailedToLoad += OnRewardVideoFailedToLoad;
        ISMediation.onRewardVideoDisplayed += OnRewardVideoDisplayed;
        ISMediation.onRewardVideoNewRequest += LoadRewardVideoMediation;
                
        ISMediation.Instance.Preload();
#endif

    }

    private void DestroyIS() {
#if ADS && IS
        if (!ISMediation.Initialized) return;
        ISMediation.onAdPaid -= OnAdPaid;
        ISMediation.onBannerChanged -= OnBannerHeightChanged;
        ISMediation.onBannerDisplayed -= OnBannerDisplayed;
        ISMediation.onBannerNewRequest -= LoadBannerMediation;

        ISMediation.onAdClicked -= OnAdClicked;
        ISMediation.onAdFailedToLoad -= OnAdFailedToLoad;
        ISMediation.onAdFailedToShow -= OnAdFailedToShow;
        
        ISMediation.onMRecDisplayed -= OnMRecDisplayed;
        ISMediation.onMRecNewRequest -= LoadMRecMediation;

        ISMediation.onInterLoaded -= OnInterAvailable;
        ISMediation.onInterFailedToLoad -= OnInterFailedToLoad;
        ISMediation.onInterDisplayed -= OnInterDisplayed;
        ISMediation.onInterClosed -= OnInterClosed;
        ISMediation.onInterNewRequest -= LoadInterMediation;

        ISMediation.onRewardVideoLoaded -= OnRewardVideoAvailable;
        ISMediation.onRewardFailedToLoad -= OnRewardVideoFailedToLoad;
        ISMediation.onRewardVideoDisplayed -= OnRewardVideoDisplayed;
        ISMediation.onRewardVideoNewRequest -= LoadRewardVideoMediation;
#endif
    }

}