
using IPS.Api.Ads;
using IPS;
using UnityEngine;

public partial class AdsManager {
    #region Remove Ads
    private bool? hasRemoveAd;
#if ADS && IAP
    private string RemoveAdsKey => IAP.RemoveAdsKey;
#else
        private string RemoveAdsKey => "remove.ads";
#endif

    /// <summary>
    /// If true, User no longer to see Banner or Interstitial ads, but still can see Reward video ads.
    /// </summary>
    public bool IsRemovedAds {
        get {
#if NOAD && !PRODUCTION
            return true;
#endif
#if ADS && IAP
            if (hasRemoveAd.HasValue) return hasRemoveAd.Value;
            return (IAP.Initialized && IAP.Instance.IsRemovedAds) || (IPSConfig.CheatEnable && PlayerPrefs.HasKey(RemoveAdsKey));
#else
                return (IPSConfig.CheatEnable && PlayerPrefs.HasKey(RemoveAdsKey));
#endif
        }
    }
        
    public bool IsRemoveAdsReward { get; set; }

    /// <summary>
    /// Remove all Banner & Interstital ads (Still keep reward video ads)
    /// <para>Call this when user buy Remove_Ads.</para>
    /// </summary>
    public void SetRemovedAds(bool removeReward = false) {
#if ADS
        IsRemoveAdsReward = removeReward;
        Debug.Log("[Ads] You have No-ad!");
        hasRemoveAd = true;
        if (IPSConfig.CheatEnable) PlayerPrefs.SetInt(RemoveAdsKey, 1);
        AdmobMediation.Instance.OnRemoveAds();
#if MAX
        MaxMediation.Instance.OnRemoveAds();        
#endif

#if IS
        ISMediation.Instance.OnRemoveAds();
#endif
        this.Dispatch<OnAdRemoved>();
#endif
    }

    private void RestoreRemoveAd() {
#if ADS && IAP
        if (!IAP.Initialized) return;
        if (IAP.Instance.IsRemovedAds) {
            SetRemovedAds(true);
        }
        else if (IAP.Instance.IsOwnedStarterPack) {
            SetRemovedAds(false);
        }
        else hasRemoveAd = false;
#endif
    }
    #endregion
}