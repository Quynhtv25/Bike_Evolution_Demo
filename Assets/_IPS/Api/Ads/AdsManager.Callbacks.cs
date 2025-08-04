
#if ADMOB
using GoogleMobileAds.Api;
#endif
using IPS.Api.Ads;
using System;
using UnityEngine;

public partial class AdsManager {

    private void OnBannerHeightChanged(Mediation mediation, float height) {
#if ADS
        if (IsRemovedAds) SetRemovedAds();
        onBannerHeightChanged?.Invoke(mediation, height);
#endif
    }
    
    private void OnBannerDisplayed(Mediation mediation) {
        bannerLastReloadTime = Time.time;
        //if (AdmobSettings.Instance.BannerReloadCapping > 0) {
        //    if (IsInvoking(nameof(ReloadBannerByCapping))) CancelInvoke(nameof(ReloadBannerByCapping));
        //    Invoke(nameof(ReloadBannerByCapping), AdmobSettings.Instance.BannerReloadCapping);
        //}
        Tracking.Instance.LogAdBannerDisplayed(currentBannerPlacement, mediation);
    }

    //private void ReloadBannerByCapping() {
    //    if (GetBannerHeight() > 0) {
    //        DestroyBanner();
    //        ShowBanner(string.Empty);
    //    }
    //}

    private void OnAdClicked() {
        PauseInsideApp = true;
    }

//#if ADS
    private void OnInterFailedToLoad(AdSlotFormat adType, string error) {
        OnAdFailedToLoad(adType, error);
        if (AdmobSettings.Instance.UseInterstitialAd && !AdmobMediation.Instance.HasInterstitial) {
            // TODO: request backfill (do nothing here because request was call inside `HasInterstitial`)
        }
    }

    private void OnAdFailedToLoad(AdSlotFormat adType, string error) {
        Tracking.Instance.LogAdFailedToLoad(adType, error);
    }

    private void OnAdFailedToShow(AdSlotFormat adType, string error) {
        Tracking.Instance.LogAdFailedToShow(adType, adType == AdSlotFormat.Banner ? currentBannerPlacement : currentPlacement, error);
    }
    
    #region Rewarded Interstitial
    private void OnRewardInterFailedToLoad(AdSlotFormat adType, string error) {
        OnAdFailedToLoad(adType, error);
        if (AdmobSettings.Instance.UseRewardedInterstitialAd && !AdmobMediation.Instance.HasRewardInterstitial) {
            // TODO: request backfill (do nothing here because request was call inside `HasRewardInterstitial`)
        }
    }

    private void OnRewardInterAvailable() {
        onRewardInterAvailable?.Invoke();
        Tracking.Instance.LogAdRewardInterAvailable();
    }

    private void OnRewardInterDisplayed(Mediation mediation) {
        lastRewardShowAtTime = Time.time;
        Tracking.Instance.LogAdRewardInterDisplayed(currentPlacement, mediation);
    }

    private void OnRewardInterCompleted(string placement) {
        Tracking.Instance.LogAdRewardInterCompleted(placement);
    }

    private void OnRewardInterEligible() {
        Tracking.Instance.LogAdRewardInterEligible(currentPlacement);
    }
    #endregion

    #region Rewarded Video
    private void OnRewardVideoFailedToLoad(AdSlotFormat adType, string error) {
        OnAdFailedToLoad(adType, error);
        if (AdmobSettings.Instance.UseRewardedVideoAd && !AdmobMediation.Instance.HasRewardVideo) {
            // TODO: request backfill (do nothing here because request was call inside `HasRewardVideo`)
        }
    }

    private void OnRewardVideoAvailable() {
        onRewardVideoAvailable?.Invoke();
        Tracking.Instance.LogAdRewardVideoAvailable();
    }

    private void OnRewardVideoDisplayed(Mediation mediation) {
        lastRewardShowAtTime = Time.time;
        Tracking.Instance.LogAdRewardVideoDisplayed(currentPlacement, mediation);
    }

    private void OnRewardVideoCompleted(string placement) {
        Tracking.Instance.LogAdRewardVideoCompleted(placement);
    }

    private void OnRewardVideoEligible() {
        Tracking.Instance.LogAdRewardVideoEligible(currentPlacement);
    }
    #endregion

    private void OnMRecDisplayed(Mediation mediation) {
        if (onMRecDisplayed != null) onMRecDisplayed.Invoke();
        Tracking.Instance.LogAdMRecDisplayed(currentMRecPlacement, mediation);
    }

    private void OnInterAvailable() {
        Tracking.Instance.LogAdInterAvailable();
    }

    private void OnInterEligible(bool canshow, int playtimesCanShow, int levelCanShow) {
        Tracking.Instance.LogAdInterEligible(currentPlacement, canshow, playtimesCanShow, levelCanShow);
        if (InterCappingReady) Tracking.Instance.LogAdInterPassedCappingTime(currentPlacement);
    }

    private void OnInterDisplayed(Mediation mediation) {
        Tracking.Instance.LogAdInterDisplayed(currentPlacement, mediation);
        lastInterShowAtTime = Time.time;
    }

    public static event Action onInterClosed;

    private void OnInterClosed(Mediation mediation) {
        if (onInterClosed != null) onInterClosed.Invoke();
        Tracking.Instance.LogAdInterClosed(currentPlacement);
    }

    private void OnInterClicked() {
        Tracking.Instance.LogAdInterClicked(currentPlacement);
    }

    private void OnAOAAvailable() {
        Tracking.Instance.LogAdAOAAvailable();
    }

    private void OnAOADisplayed(Mediation mediation) {
        Tracking.Instance.LogAdAOADisplayed(currentPlacement, mediation);
    }

#if ADMOB
    private void OnAdPaid(AdValue adValue, AdSlotFormat adFormat, string adPlacement) {
        Tracking.Instance.LogAdImpression(Mediation.admob, adNetwork: "admob", adUnitName: adFormat.ToString().ToLower(),
                                            adType: adFormat, currency: adValue.CurrencyCode, revenue: adValue.Value,
                                            adCountry: adValue.CurrencyCode, adPrecision: adValue.Precision.ToString(),
                                            placement: adPlacement, adLtv: null);
    }
#endif

#if MAX
    private void OnAdPaid(MaxSdkBase.AdInfo adInfo, AdSlotFormat adFormat, string placement) {
        Tracking.Instance.LogAdImpression(Mediation.appLovin, adNetwork: adInfo.NetworkName, adUnitName: adFormat.ToString().ToLower(),
                                            adType: adFormat, currency: "USD", revenue: adInfo.Revenue,
                                            adCountry: "unknown", adPrecision: adInfo.RevenuePrecision,
                                            placement: placement, adLtv: null);
    }
#endif

#if IS
    private void OnAdPaid(IronSourceAdInfo adInfo, AdSlotFormat adFormat, string placement) {
        Tracking.Instance.LogAdImpression(Mediation.ironSource, adNetwork: adInfo.adNetwork, adUnitName: adInfo.instanceName, adType: adFormat,
                           currency: "USD", revenue: adInfo.revenue != null ? (double)adInfo.revenue : 0, 
                           adCountry: adInfo.country, adPrecision: adInfo.precision, placement: placement, adLtv: adInfo.lifetimeRevenue);
    }
#endif
    
}