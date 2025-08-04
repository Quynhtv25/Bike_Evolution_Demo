
using UnityEngine;
using IPS.Api.Ads;
using IPS;


public partial class AdsManager {
    private uint bannerLastLevel = 1;
    private double bannerLastReloadTime = -1000;
    public bool CanReloadBannerByLevel => (AdmobSettings.Instance.ReloadBannerOnLoadScene && Tracking.CurrentLevel - bannerLastLevel >= AdmobSettings.Instance.BannerReloadByLevelCapping);
    private bool BannerPlayTimesReady => UserData.PlayTimes >= AdmobSettings.Instance.BannerFromPlayTimes;
    private bool BannerLevelReady => UserData.CurrentLevel >= AdmobSettings.Instance.BannerFromLevel;
    private bool BannerReloadCappingReady => AdmobSettings.Instance.BannerReloadCapping > 0 && Time.time - bannerLastReloadTime >= AdmobSettings.Instance.BannerReloadCapping;

    private void LoadBannerMediation(Mediation mediation, string placement) {
#if ADS
#if UNITY_EDITOR
        AdmobMediation.Instance.ShowBanner(placement);
        return;
#endif

        if (AdmobSettings.Instance.UseBannerCollapsible) {
            if ((mediation != Mediation.admob || !AdmobSettings.Instance.CollapsibleFallbackEnable)) {
                AdmobMediation.Instance.ShowBanner(placement);
                return;
            }
        }

#if IS
        if (ISSettings.Instance.UseBannerAd) {
            ISMediation.Instance.ShowBanner(placement);
            return;
        }
#endif

#if MAX
        if (MaxSettings.Instance.UseBannerAd) {
            MaxMediation.Instance.ShowBanner(placement);
            return;
        }
#endif
        if (AdmobSettings.Instance.UseBannerAd) {
            AdmobMediation.Instance.ShowBanner(placement);
        }

#endif
    }

    public void ShowBanner(string placement) {
        if (!string.IsNullOrEmpty(placement)) currentBannerPlacement = placement;
        bannerLastLevel = (uint)Tracking.CurrentLevel;
        Logs.Log($"[Ads.Banner] ShowBanner Triggered playTimesReady={BannerPlayTimesReady}, levelReady={BannerLevelReady}");
#if ADS
        if (IsRemovedAds) return;

        if (!BannerPlayTimesReady || !BannerLevelReady) return;

#if IS
        LoadBannerMediation(Mediation.ironSource, placement);
        return;
#elif MAX
        LoadBannerMediation(Mediation.appLovin, placement);
        return;
#endif
        LoadBannerMediation(Mediation.admob, placement);
#else
        Logs.Log("[Ads] Turn ON ADS config first!");
#endif
    }

    public void HideBanner() {
        Logs.Log("[Ads.Banner] HideBanner triggered");
#if ADS
        // if (IsRemovedAds) return;

#if IS
        if (ISMediation.Initialized) ISMediation.Instance.HideBanner();
#endif

#if MAX
        if (MaxMediation.Initialized) MaxMediation.Instance.HideBanner();
#endif

        if (AdmobMediation.Initialized) AdmobMediation.Instance.HideBanner();
#endif
    }

    public void DestroyBanner() {
        Logs.Log("[Ads.Banner] DestroyBanner triggered.");
#if ADS
#if IS
        if (ISMediation.Initialized) ISMediation.Instance.DestroyBanner();
#endif
#if MAX
        if (MaxMediation.Initialized) MaxMediation.Instance.DestroyBanner();
#endif
        if (AdmobMediation.Initialized) AdmobMediation.Instance.DestroyBanner();
#endif
    }

        /// <summary>
        /// The height in pixel of the banner, use for controll your UI follow up banner.
        /// <para>Example: when user buy removed ads, the bottom button should be move down than normal.</para>
        /// </summary>
    public float GetBannerHeight() {
#if ADS
        if (IsRemovedAds) return 0;
#if MAX
        if (MaxSettings.Instance.UseBannerAd) {
            return MaxMediation.Instance.BannerHeight;
        }
#endif

#if IS
        if (ISSettings.Instance.UseBannerAd) {
            return ISMediation.Instance.BannerHeight;
        }
#endif
        if (AdmobSettings.Instance.UseBannerAd) {
            return AdmobMediation.Instance.BannerHeight;
        }
#endif
        return 0;
    }

        /// <summary> The estimate height in pixel of the banner, value is fixed event banner is showing or not </summary>
        public float GetBannerHeightEstimate() {
#if ADS
        if (IsRemovedAds) return 0;
#if MAX
        if (AdmobSettings.Instance.UseBannerCollapsible) {
            return AdmobMediation.Instance.BannerHeightEstimate;
        }
        else if (MaxSettings.Instance.UseBannerAd) {
            return MaxMediation.Instance.BannerHeightEstimate;
        }
#endif

#if IS
        if (ISSettings.Instance.UseBannerAd) {
            return ISMediation.Instance.BannerHeightEstimate;
        }
#endif
        if (AdmobSettings.Instance.UseBannerAd) {
            return AdmobMediation.Instance.BannerHeightEstimate;
        }
#endif
        return 0;
    }

    public bool IsBannerOnBottom {
        get {
#if ADS
#if MAX
            if (MaxSettings.Instance.UseBannerAd) return MaxSettings.Instance.ShowBannerOnBottom;
#endif
#if IS
            if (ISSettings.Instance.UseBannerAd) return ISSettings.Instance.ShowBannerOnBottom;
#endif
            if (AdmobSettings.Instance.UseBannerAd) return AdmobSettings.Instance.ShowBannerOnBottom;
#endif
            return true;
        }
    }
}