using UnityEngine;
using System.Collections.Generic;


#if ADMOB_NATIVE
using IPS.Api.Ads;
using GoogleMobileAds.Api;
#endif

public class NativeAdView_3D : NativeAdViewBase {
    [SerializeField] Texture2D defaultMainTex;
    [SerializeField] MeshRenderer adChoice;
    [SerializeField] MeshRenderer mainTexture;
    [SerializeField] MeshRenderer miniIcon;
    [SerializeField] TMPro.TextMeshPro headText;
    [SerializeField] TMPro.TextMeshPro bodyText;

   
    protected override void SetDefaultTexture() {
        if (mainTexture && defaultMainTex) {
            mainTexture.material.color = Color.white;
            mainTexture.material.SetTexture("_MainTex", defaultMainTex);
        }
        else {
            if (mainTexture) mainTexture.material.color = Color.clear;
            if (miniIcon) miniIcon.gameObject.SetActive(false);
            base.SetDefaultTexture();
        }
    }

#if ADMOB_NATIVE
    protected override bool OnDisplayNativeAd(NativeAd ad) {
        // Show adChoice
        if (adChoice) {
            var adChoicesTex = ad.GetAdChoicesLogoTexture();
            if (adChoicesTex) {
                adChoice.material.SetTexture("_MainTex", adChoicesTex);
                if (adChoice.GetComponent<Collider>() == null) {
                    adChoice.gameObject.AddComponent<BoxCollider>();
                }
                if (!ad.RegisterAdChoicesLogoGameObject(adChoice.gameObject)) {
                    Debug.LogError($"[Ads.{typeof(NativeAdView_3D)}] Failed: RegisterAdChoicesLogoGameObject");
                }
                else Logs.Log($"[Ads.{typeof(NativeAdView_3D)}] Registered AdChoices");
            }
        }
        else Debug.LogError($"[Ads.{typeof(NativeAdView_3D)}] adChoice cannot be null!");

        // Show headline text
        if (headText) {
            headText.SetText(ad.GetHeadlineText());
            if (headText.GetComponent<Collider>() == null) {
                headText.gameObject.AddComponent<BoxCollider>();
            }
            if (!ad.RegisterHeadlineTextGameObject(headText.gameObject)) {
                Debug.LogError($"[Ads.{typeof(NativeAdView_3D)}] Failed: RegisterHeadlineTextGameObject");
            }
            else Logs.Log($"[Ads.{typeof(NativeAdView_3D)}] Registered HeadlineText");
        }

        // Show body text
        if (bodyText && !string.IsNullOrEmpty(ad.GetBodyText())) {
            bodyText.SetText(ad.GetBodyText());
            if (bodyText.GetComponent<Collider>() == null) {
                bodyText.gameObject.AddComponent<BoxCollider>();
            }
            if (!ad.RegisterBodyTextGameObject(bodyText.gameObject)) {
                Debug.LogError($"[Ads.{typeof(NativeAdView_3D)}] Failed: RegisterBodyTextGameObject");
            }
            else Logs.Log($"[Ads.{typeof(NativeAdView_3D)}] Registered BodyText");
        }

        // Show main tex
        Texture2D icon = ad.GetIconTexture();
        Texture2D main = icon;
        var images = ad.GetImageTextures();
        if (images != null && images.Count > 0) {
            if (icon == null) {
                icon = images[0];
                images.RemoveAt(0);
            }

            if (images.Count > 0) main = images[images.Count - 1];
            else main = icon;
        }
        
        if (miniIcon) {
            if (icon != null) {
                miniIcon.gameObject.SetActive(true);
                miniIcon.material.SetTexture("_MainTex", icon);

                if (miniIcon.GetComponent<Collider>() == null) {
                    miniIcon.gameObject.AddComponent<BoxCollider>();
                }

                if (!ad.RegisterIconImageGameObject(miniIcon.gameObject)) {
                    Debug.LogError($"[Ads.{typeof(NativeAdView_3D)}] Failed: RegisterIconImageGameObject");
                }
                else Logs.Log($"[Ads.{typeof(NativeAdView_3D)}] Registered RegisterIconImageGameObject");
            }
            else miniIcon.gameObject.SetActive(false);
        }

        // Show main tex
        if (mainTexture) {
            if (main) {
                mainTexture.material.color = Color.white;
                mainTexture.material.SetTexture("_MainTex", main);

                var list = new List<GameObject>() { mainTexture.gameObject };
                if (ad.RegisterImageGameObjects(list) > 0) {
                    Debug.LogError($"[Ads.{typeof(NativeAdView_3D)}] Failed: RegisterImageGameObjects");
                }
                else Logs.Log($"[Ads.{typeof(NativeAdView_3D)}] Registered RegisterImageGameObjects size");

                return true;
            }
        }
        else {
            Debug.LogError($"[Ads.{typeof(NativeAdView_3D)}] ad mainTexture cannot be null!");
        }

        return false;
    }
#endif
}