#if ADMOB_NATIVE
using GoogleMobileAds.Api;
using IPS.Api.Ads;
#endif
using UnityEngine;

namespace IPS {
    public class NativeAdAutoSpawn : MonoBehaviour {
        [SerializeField] string trackingScreenName;
        [SerializeField] private NativeAdViewBase nativeAdPrefab;
        [SerializeField] Transform container;
        [SerializeField] uint siblingIndex = 0;        

        NativeAdViewBase nativeadContent;

        private void OnEnable() {
            //Invoke(nameof(TestSpawn), 2);
            //return;
            if (nativeadContent == null) {
                nativeadContent = Instantiate(nativeAdPrefab.gameObject, container).GetComponent<NativeAdViewBase>();
                nativeadContent.transform.localScale = Vector3.one;
                nativeadContent.transform.localPosition = Vector3.zero;
                nativeadContent.transform.SetSiblingIndex((int)siblingIndex);
                nativeadContent.gameObject.SetActive(false);
                nativeadContent.SetTrackingScreenName(trackingScreenName);
            }
#if ADMOB_NATIVE
            if (AdmobMediation.Initialized) AdmobMediation.Instance.ShowNative(!string.IsNullOrEmpty(trackingScreenName) ? trackingScreenName : container.name, SpawnNativeAd);
            if (AdsManager.Initialized) this.AddListener<AdsManager.OnAdRemoved>(OnAdRemoved);
#endif
        }

        private void OnAdRemoved() {
            if (container) container.gameObject.SetActive(false);
        }

        private void OnDisable() {
#if ADMOB_NATIVE
            if (AdmobMediation.Initialized) AdmobMediation.Instance.DestroyNative();
#endif
            if (nativeadContent) nativeadContent.gameObject.SetActive(false);
        }

        private void TestSpawn() {
#if ADMOB_NATIVE
            SpawnNativeAd(null);
#endif
        }

#if ADMOB_NATIVE
        private void SpawnNativeAd(NativeAd adView) {
            Logs.Log($"[Ads.Admob] SpawnNative Triggered adView={(adView != null ? "nativeAd" : "null")}");
            if (adView == null) return;
            nativeadContent.gameObject.SetActive(true);
            nativeadContent.DisplayNativeAd(adView);
        }
#endif
    }
}