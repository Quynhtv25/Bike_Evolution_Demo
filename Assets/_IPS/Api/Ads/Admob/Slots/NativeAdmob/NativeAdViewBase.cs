using UnityEngine;
using IPS;



#if ADMOB_NATIVE
using IPS.Api.Ads;
using GoogleMobileAds.Api;
#endif

public interface INativeAdView {
    void SetTrackingScreenName(string screenName);
#if ADMOB_NATIVE
    void DisplayNativeAd(NativeAd nativeAd);
#endif
}

public abstract class NativeAdViewBase : MonoBehaviour, INativeAdView {
    [Header("Tracking Placement")]
    [SerializeField] string screenName;
    [SerializeField] private bool autoListener = false;
    [SerializeField] private GameObject loadingObj;
    
    private bool listenerRegisterd;

    protected virtual void OnEnable() {
        if (autoListener) {
            ShowAd();
            if (AdsManager.Initialized) this.AddListener<AdsManager.OnAdRemoved>(OnAdRemoved);        
        }
    }

    private void OnDisable() {
        if (autoListener) HideAd();
    }

    private void OnAdRemoved() {
        gameObject.SetActive(false);
    }

    public void SetTrackingScreenName(string name) {
        screenName = name;
    }

    protected virtual void SetDefaultTexture() {
        ShowLoading(true);
    }

#if ADMOB_NATIVE
    protected abstract bool OnDisplayNativeAd(NativeAd ad);
#endif

    private void ShowLoading(bool show) {
        if (loadingObj != null) loadingObj.SetActive(show);
    }

    private void ShowAd() {
        if (!listenerRegisterd) {
            SetDefaultTexture();
        }
#if ADMOB_NATIVE
        if (AdmobMediation.Initialized && !listenerRegisterd) {
            AdmobMediation.Instance.ShowNative(!string.IsNullOrEmpty(screenName) ? screenName : transform.parent.name, DisplayNativeAd);
        }
#endif
    }

    private void HideAd() {
#if ADMOB_NATIVE
        if (listenerRegisterd && AdmobMediation.Initialized) {
            listenerRegisterd = false;
            AdmobMediation.Instance.DestroyNative();
        }
#endif
    }

#if ADMOB_NATIVE
    public void DisplayNativeAd(NativeAd ad) {
        listenerRegisterd = true;
        SetDefaultTexture();
        if (ad == null) return;
        
        Debug.Log($"[Ads.{GetType().Name}] DisplayNativeAd..");
        if (OnDisplayNativeAd(ad)) ShowLoading(false);
    }
#endif
}