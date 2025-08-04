#if ADS
using IPS.Api.Ads;

#endif
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace IPS {
    public class SafeArea : MonoBehaviour {
        [SerializeField] bool maskTop = true;
        [SerializeField] bool maskBottom = true;
        [SerializeField] Color maskColor = Color.black; 

        [SerializeField] bool adjustOnBannerChanged = true;
        [SerializeField] UnityEvent onBannerShowing;
        [SerializeField] UnityEvent onBannerHide;

        private RectTransform recTransform;
        public static float canvasRatio = -1;
        Vector2 safeAnchorMin, safeAnchorMax;

        private void Awake() {
            recTransform = GetComponent<RectTransform>();
            if (canvasRatio < 0) {
                var scaler = GetComponentInParent<CanvasScaler>();

                if (scaler != null && scaler.matchWidthOrHeight == 0 && scaler.referenceResolution.y > 0) {
                    float rateMatchWidth = Screen.width / scaler.referenceResolution.x;
                    float currentHeight = Screen.height / rateMatchWidth;
                    canvasRatio = currentHeight / scaler.referenceResolution.y;
                }
                else canvasRatio = 1;

                Debug.Log($"<color=yellow>[SafeArea.{transform.parent.name}] dpi={Screen.dpi}, ScreenSize=({Screen.width},{Screen.height}), safeArea={Screen.safeArea}, scalerResolution={scaler.referenceResolution}, matchWidthOrHeight={scaler.matchWidthOrHeight}, canvasRatio={canvasRatio}</color>");
            }
        }

        private void Start() {
            var safeArea = Screen.safeArea;
            var anchorMin = safeArea.position;
            var anchorMax = anchorMin + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            safeAnchorMin = anchorMin;
            safeAnchorMax = anchorMax;

            recTransform.anchorMin = anchorMin;
            recTransform.anchorMax = anchorMax;
            recTransform.pivot = new Vector2(0, 1);

            Vector3 pos;
            if(maskTop) {
                var maskTop = new GameObject("SafeTop").AddComponent<Image>();
                maskTop.transform.SetParent(transform);
                maskTop.transform.SetAsLastSibling();
                maskTop.AddComponent<Canvas>().overrideSorting = true;
                maskTop.canvas.sortingOrder = 32000;
                maskTop.color = maskColor;
                maskTop.rectTransform.anchorMin = Vector2.up;
                maskTop.rectTransform.anchorMax = Vector2.one;
                maskTop.rectTransform.sizeDelta = new Vector2(0, 500);
                maskTop.rectTransform.pivot = Vector2.zero;
                maskTop.rectTransform.anchoredPosition = Vector2.zero;
                maskTop.rectTransform.localScale = Vector3.one;

                pos = maskTop.transform.localPosition;
                pos.z = 0;
                maskTop.transform.localPosition = pos;
            }

            if (maskBottom) {
                var maskBottom = new GameObject("SafeBottom").AddComponent<Image>();
                maskBottom.transform.SetParent(transform);
                maskBottom.transform.SetAsLastSibling();
                maskBottom.AddComponent<Canvas>().overrideSorting = true;
                maskBottom.canvas.sortingOrder = 30000;
                maskBottom.color = maskColor;
                maskBottom.rectTransform.anchorMin = Vector2.zero;
                maskBottom.rectTransform.anchorMax = Vector2.right;
                maskBottom.rectTransform.sizeDelta = new Vector2(0, 500);
                maskBottom.rectTransform.pivot = Vector2.up;
                maskBottom.rectTransform.anchoredPosition = Vector2.zero;
                maskBottom.rectTransform.localScale = Vector3.one;

                pos = maskBottom.transform.localPosition;
                pos.z = 0;
                maskBottom.transform.localPosition = pos;
            }
                        
#if ADS
            if (adjustOnBannerChanged) {
                recTransform.pivot = new Vector2(0, AdsManager.Instance.IsBannerOnBottom ? 1 : 0);
                AdsManager.Instance.onBannerHeightChanged += OnBannerHeightChanged;
                OnBannerHeightChanged(Mediation.admob, AdsManager.Instance.GetBannerHeight());
            }
#endif
        }

#if ADS
        private void OnBannerHeightChanged(Mediation mediation, float height) {
#if ADS
            if (!adjustOnBannerChanged) return;

            float value = height * canvasRatio;

            Logs.Log($"[SafeArea.{transform.parent.name}] OnBannerHeighChanged mediation={mediation}, banner real height={height}, apply height={value}, canvasRatio={canvasRatio}");
            if (value > 0) {
                if (mediation == Mediation.ironSource) {
                    if (!AdsManager.Instance.IsBannerOnBottom) recTransform.anchorMax = new Vector2(safeAnchorMax.x, 1);
                    else recTransform.anchorMin = new Vector2(safeAnchorMin.x, 0);
                }
                else {
                    recTransform.anchorMin = safeAnchorMin;
                    recTransform.anchorMax = safeAnchorMax;
                }
                onBannerShowing?.Invoke();
            }
            else {
                recTransform.anchorMin = safeAnchorMin;
                recTransform.anchorMax = safeAnchorMax;
                onBannerHide?.Invoke();
            }
            recTransform.sizeDelta = new Vector2(recTransform.sizeDelta.x, -value);
#else
            onBannerHide?.Invoke();
#endif
        }

        private void OnDestroy() {
#if ADS
            if(adjustOnBannerChanged && AdsManager.Initialized) AdsManager.Instance.onBannerHeightChanged -= OnBannerHeightChanged;
#endif
        }
#endif
    }
}