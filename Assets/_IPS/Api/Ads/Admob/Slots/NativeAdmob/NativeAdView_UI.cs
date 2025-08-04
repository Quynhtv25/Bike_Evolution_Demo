using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Xml.Linq;



#if ADMOB_NATIVE
using IPS.Api.Ads;
using GoogleMobileAds.Api;
#endif

public class NativeAdView_UI : NativeAdViewBase {
    [SerializeField] Sprite defaultMainSpr;
    [SerializeField] Image adChoice;
    [SerializeField] Image mainImage;
    [SerializeField] Image miniIcon;
    [SerializeField] TMPro.TextMeshProUGUI headText;
    [SerializeField] TMPro.TextMeshProUGUI descriptionText;
    [SerializeField] GameObject infoGroup;

    protected override void OnEnable() {
        if (infoGroup) infoGroup.gameObject.SetActive(false);
        if (headText) headText.SetText(string.Empty);
        if (descriptionText) descriptionText.SetText(string.Empty);
        if (miniIcon) miniIcon.gameObject.SetActive(false);
        base.OnEnable();
    }

    protected override void SetDefaultTexture() {
        if (mainImage && defaultMainSpr) {
            mainImage.color = Color.white;
            mainImage.sprite = defaultMainSpr;
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
            if (adChoice.GetComponent<Collider>() == null) {
                adChoice.gameObject.AddComponent<BoxCollider>();
            }
            if (!ad.RegisterAdChoicesLogoGameObject(adChoice.gameObject)) {
                Debug.LogError($"[Ads.{typeof(NativeAdView_UI)}] Failed: RegisterAdChoicesLogoGameObject");
            }
            else Logs.Log($"[Ads.{typeof(NativeAdView_UI)}] Registered AdChoices");

            var adChoicesTex = ad.GetAdChoicesLogoTexture();
            if (adChoicesTex) {
                var spr = Sprite.Create(adChoicesTex, new Rect(0, 0, adChoicesTex.width, adChoicesTex.height), Vector2.zero);
                adChoice.sprite = spr;
            }
        }
        else Debug.LogError($"[Ads.{typeof(NativeAdView_UI)}] adChoice cannot be null!");

        // Show headline text
        if (headText) {
            string headline = ad.GetHeadlineText();
            if (!string.IsNullOrEmpty(headline)) {
                if (infoGroup) infoGroup.SetActive(true);

                headText.SetText(headline);
                if (headText.GetComponent<Collider>() == null) {
                    headText.gameObject.AddComponent<BoxCollider>();
                }
                if (!ad.RegisterHeadlineTextGameObject(headText.gameObject)) {
                    Debug.LogError($"[Ads.{typeof(NativeAdView_UI)}] Failed: RegisterHeadlineTextGameObject");
                }
                else Logs.Log($"[Ads.{typeof(NativeAdView_UI)}] Registered HeadlineText");
            }
        }

        if (descriptionText) {
            string bodyText = ad.GetBodyText();
            if (!string.IsNullOrEmpty(bodyText)) {
                if (infoGroup) infoGroup.SetActive(true);

                descriptionText.gameObject.SetActive(true);
                descriptionText.SetText(bodyText);

                if (descriptionText.GetComponent<Collider>() == null) {
                    descriptionText.gameObject.AddComponent<BoxCollider>();
                }
                if (!ad.RegisterBodyTextGameObject(descriptionText.gameObject)) {
                    Debug.LogError($"[Ads.{typeof(NativeAdView_UI)}] Failed: RegisterBodyTextGameObject");
                }
                else Logs.Log($"[Ads.{typeof(NativeAdView_UI)}] Registered BodyText");
            }
            else descriptionText.gameObject.SetActive(false);
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
                if (infoGroup) infoGroup.SetActive(true);
                miniIcon.gameObject.SetActive(true);
                miniIcon.sprite = Sprite.Create(icon, new Rect(0, 0, icon.width, icon.height), Vector2.zero);

                if (miniIcon.GetComponent<Collider>() == null) {
                    miniIcon.gameObject.AddComponent<BoxCollider>();
                }

                if (!ad.RegisterIconImageGameObject(miniIcon.gameObject)) {
                    Debug.LogError($"[Ads.{typeof(NativeAdView_UI)}] Failed: RegisterIconImageGameObject");
                }
                else Logs.Log($"[Ads.{typeof(NativeAdView_UI)}] Registered RegisterIconImageGameObject");
            }
            else miniIcon.gameObject.SetActive(false);
        }

        if (mainImage) {
            //FetchMainImageCollider();

            if (main) {
                var spr = Sprite.Create(main, new Rect(0, 0, main.width, main.height), Vector2.zero);
                mainImage.color = Color.white;
                mainImage.sprite = spr;

                var list = new List<GameObject>() {mainImage.gameObject};
                if (ad.RegisterImageGameObjects(list) > 0) {
                    Debug.LogError($"[Ads.{typeof(NativeAdView_UI)}] Failed: RegisterImageGameObjects");
                }
                else Logs.Log($"[Ads.{typeof(NativeAdView_UI)}] Registered RegisterImageGameObjects size");

                return true;
            }
        }
        else {
            Debug.LogError($"[Ads.{typeof(NativeAdView_UI)}] ad MainImage cannot be null!");
        }

        return false;
    }

    
    [ContextMenu("FetchMainImageCollider")]
    private void FetchMainImageCollider() {
        var collide = mainImage.GetComponent<Collider>();
        if (collide == null) {
            collide = mainImage.gameObject.AddComponent<BoxCollider>();
        }

        Vector2 parentSize = (mainImage.rectTransform.parent as RectTransform).sizeDelta;
        Vector2 anchorMin = mainImage.rectTransform.anchorMin;
        Vector2 anchorMax = mainImage.rectTransform.anchorMax;
        Vector2 sizeDelta = mainImage.rectTransform.sizeDelta;

        float width = (anchorMax.x - anchorMin.x) * parentSize.x + sizeDelta.x;
        float height = (anchorMax.y - anchorMin.y) * parentSize.y + sizeDelta.y;

        var bound = collide.bounds;
        bound.size = new Vector3(width, height);
        Logs.Log($"[Ads.{typeof(NativeAdView_UI)}] mainImage boundSize=({bound.size})");
    }
#endif
}