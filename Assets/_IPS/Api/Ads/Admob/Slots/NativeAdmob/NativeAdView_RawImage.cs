using UnityEngine;
using UnityEngine.UI;

#if ADMOB_NATIVE
using IPS.Api.Ads;
using GoogleMobileAds.Api;
#endif

public class NativeAdView_RawImage : NativeAdViewBase {
    [SerializeField] Texture2D defaultMainSpr;
    [SerializeField] RawImage adChoice;
    [SerializeField] RawImage mainImage;
    [SerializeField] TMPro.TextMeshProUGUI headText;
    
    
    protected override void SetDefaultTexture() {
        if (mainImage && defaultMainSpr) {
            mainImage.color = Color.white;
            mainImage.texture = defaultMainSpr;
        }
        else {
            if (mainImage) mainImage.color = Color.clear;
            base.SetDefaultTexture();
        }
    }

#if ADMOB_NATIVE
    protected override bool OnDisplayNativeAd(NativeAd ad) {
        // Show adChoice
        if (adChoice) {
            var adChoicesTex = ad.GetAdChoicesLogoTexture();
            if (adChoicesTex) {
                adChoice.texture = adChoicesTex;
                if (adChoice.GetComponent<Collider>() == null) {
                    adChoice.gameObject.AddComponent<BoxCollider>();
                }
                if (!ad.RegisterAdChoicesLogoGameObject(adChoice.gameObject)) {
                    Debug.LogError($"[Ads.{typeof(NativeAdView_RawImage)}] Failed: RegisterAdChoicesLogoGameObject");
                }
                else Logs.Log($"[Ads.{typeof(NativeAdView_RawImage)}] Registered AdChoices");
            }
        }
        else Debug.LogError($"[Ads.{typeof(NativeAdView_RawImage)}] adChoice cannot be null!");

        // Show headline text
        if (headText) {
            headText.SetText(ad.GetHeadlineText());
            if (headText.GetComponent<Collider>() == null) {
                headText.gameObject.AddComponent<BoxCollider>();
            }
            if (!ad.RegisterHeadlineTextGameObject(headText.gameObject)) {
                Debug.LogError($"[Ads.{typeof(NativeAdView_RawImage)}] Failed: RegisterHeadlineTextGameObject");
            }
            else Logs.Log($"[Ads.{typeof(NativeAdView_RawImage)}] Registered HeadlineText");
        }

        // Show main tex
        if (mainImage) {
            var tex = ad.GetIconTexture();
            if (tex != null) {
                mainImage.color = Color.white;
                mainImage.texture = tex;
                if (mainImage.GetComponent<Collider>() == null) {
                    mainImage.gameObject.AddComponent<BoxCollider>();
                }

                if (!ad.RegisterIconImageGameObject(mainImage.gameObject)) {
                    Debug.LogError($"[Ads.{typeof(NativeAdView_RawImage)}] Failed: RegisterIconImageGameObject");
                }
                else {
                    Logs.Log($"[Ads.{typeof(NativeAdView_RawImage)}] Registered IconImage");
                }

                return true;
            }
            else {
                Debug.LogError($"[Ads.{typeof(NativeAdView_RawImage)}] Failed: ad return null IconTexture");
                return false;
            }
        }
        else {
            Debug.LogError($"[Ads.{typeof(NativeAdView_RawImage)}] ad MainImage cannot be null!");
            return false;
        }
    }
#endif
}