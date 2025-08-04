using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IPS {
    public class SplashLogoLoading : MonoBehaviour {
        [SerializeField] Image logo;
        [Header("ZoomIn")]
        [SerializeField] bool zoomIn = true;
        [SerializeField] float zoomInFrom = 0.5f;
        [SerializeField] float zoonInDuration = 1.5f;
        [SerializeField] bool punchZoom = false;
        [SerializeField] float punchZoomBonus = 0.3f;
        [SerializeField] float punchZoomDuration = .3f;
        [Header("FadeIn")]
        [SerializeField] float fadeInDuration = .3f;
        [Header("FadeOut")]
        [SerializeField] bool fadeOut = true;
        [SerializeField] float fadeOutDuration = .3f;
        [SerializeField] float fadeOutDelay = .2f;

        private void OnEnable() {
            if (logo) {
                logo.gameObject.SetActive(true);
                logo.DOFade(1, fadeInDuration).From(0);
                if (zoomIn) {
                    var tween = logo.transform.DOScale(1, zoonInDuration).From(zoomInFrom).OnComplete(() => {
                        if (punchZoom) logo.transform.DOPunchScale(new Vector2(punchZoomBonus, punchZoomBonus), punchZoomDuration, 1).SetLoops(-1, LoopType.Yoyo);
                        if (fadeOut) logo.DOFade(0, fadeOutDuration).SetDelay(fadeOutDelay);
                    });
                }
            }
        }
    }
}