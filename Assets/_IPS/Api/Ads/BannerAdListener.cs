using System;
using UnityEngine;
using IPS;

[RequireComponent(typeof(RectTransform))]
public class BannerAdListener : MonoBehaviour {
    [SerializeField] float bonusDelta = 0;
    RectTransform myRect;

    private void Start() {
#if ADS
        AdsManager.Instance.onBannerHeightChanged += OnBannerHeightChanged;
        OnBannerHeightChanged(Mediation.admob, AdsManager.Instance.GetBannerHeight());
#endif
    }

    private void OnDestroy() {
#if ADS
        if (AdsManager.Initialized) AdsManager.Instance.onBannerHeightChanged -= OnBannerHeightChanged;
#endif
    }

    private void OnBannerHeightChanged(Mediation mediation, float height) {
#if ADS
        Logs.Log($"OnBannerHeightChanged mediation={mediation} height={height}");
        height *= SafeArea.canvasRatio;//AdsManager.Instance.GetBannerHeight() * SafeArea.canvasRatio;
        myRect.sizeDelta = new Vector2(myRect.sizeDelta.x, height + (height > 0 ? bonusDelta : 0));
#endif
    }
}