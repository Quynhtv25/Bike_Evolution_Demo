using DG.Tweening;
using IPS.Api.Ads;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace IPS {
    public class AdsBreakUI : SingletonBehaviourResourcesDontDestroy<AdsBreakUI> {
        [SerializeField] GameObject mainFrame;
        [SerializeField] TMPro.TextMeshProUGUI countdownText;
        string placement;
        Action callback;

        const int MaxCountdown = 3;
        int timer;
        bool forceInterLoop = false;
        Coroutine forceInter;
        protected override void OnAwake() {
        }

        /// <summary>
        /// Start countdown for force inter, Incase want to force ingame only, you should call this when start level, and call "Stop" function when finish level. 
        /// </summary>
        /// <param name="placement">Position where the popup show (ingame, home, end game...)</param>
        /// <param name="delayTime">delay time to show popup, should be </param>
        /// <param name="callback">callback for inter show</param>
        public void StartForceInterCountdown(string placement, Action callback = null, bool loop = true) {
            if (AdsManager.Instance.IsRemovedAds || !AdmobSettings.Instance.ForceInterEnable) {
                if (callback != null) callback.Invoke();
                return;
            }

            forceInterLoop = loop;

            if (forceInter != null) {
                StopCoroutine(forceInter);
                forceInter = null;
            }

            forceInter = StartCoroutine(IEForceInter(placement, callback));
        }

        public void StopForceInterCountdown() {
            Logs.Log($"[AdsBreakUI] Stop countdown for force interstitial at placement={placement}");
            forceInterLoop = false;
            if (forceInter != null) {
                StopCoroutine(forceInter);
                forceInter = null;
            }
        }

        private IEnumerator IEForceInter(string placement, Action callback) {
            Logs.Log($"[AdsBreakUI] Start countdown for force interstitial at placement={placement} capping={AdmobSettings.Instance.ForceInterCapping} seconds");
            yield return Yielder.Wait(AdmobSettings.Instance.ForceInterCapping);

            Logs.Log($"[AdsBreakUI] Force interstitial at {placement} capping ready");
            while (!AdsManager.Instance.HasInterstitial || !AdsManager.Instance.InterScriptReady) yield return null;

            Show(placement, callback);
        }

        /// <summary>
        /// Show popup immediately
        /// </summary>
        /// <param name="placement">Position where the popup show (ingame, home, end game...)</param>
        /// <param name="callback">callback for inter show</param>
        /// 
        [SerializeField] protected Ease showType = Ease.OutBack;
        [SerializeField] float animTime = 0.75f;
        [SerializeField] Image fillAmount;
        float screenSize = 0;
        private void FetchScreenSize() {
            if(screenSize == 0) {
                var scaler = GetComponentInParent<CanvasScaler>();
                if(scaler)
                    screenSize = scaler.matchWidthOrHeight == 0 ? 1090 : Screen.width * (1920f / Screen.height) + 10;
                else
                    screenSize = 1090;
            }
        }
        public void Show(string placement, Action callback = null, bool fromRight = true) {
            mainFrame.gameObject.SetActive(true);
            FetchScreenSize();
            mainFrame.gameObject.transform.DOKill();
            (mainFrame.gameObject.transform as RectTransform).DOAnchorPosX(0, animTime).From(new Vector2(fromRight ? screenSize : -screenSize, 0))
                .SetUpdate(true).SetEase(showType).OnComplete(() => {
                    Tracking.Instance.LogUIFrame(ScreenName.Ingame.ToString(), typeof(AdsBreakUI).Name);
                    this.placement = placement;
                    this.callback = callback;
                    timer = MaxCountdown;
                    CountdownShowAd();
                });
        }

        public void Hide(bool toLeft = true) {
            if(!mainFrame)
                return;
            mainFrame.gameObject.SetActive(false);
            //FetchScreenSize();
            //mainFrame.gameObject.transform.DOKill();
            /*            (mainFrame.gameObject.transform as RectTransform).DOAnchorPosX(toLeft ? -screenSize : screenSize, animTime)
                            .SetUpdate(true).SetEase(showType).OnComplete(() => {
                                mainFrame.gameObject.SetActive(false);
                            });*/
        }

        private void CountdownShowAd() {
            if (timer > 0) {
                if(fillAmount != null) {
                    fillAmount.fillAmount = 0f;
                    fillAmount.DOKill();
                    fillAmount.DOFillAmount(1, 0.99f);
                }
                countdownText.SetText(timer.ToString());
                Invoke(nameof(CountdownShowAd), 1);
                timer--;
                return;
            }
            AdsManager.Instance.ShowInterstitial(placement, () => {
                if (callback != null) callback.Invoke();
                if (forceInterLoop) {
                    StartForceInterCountdown(placement, callback, forceInterLoop);
                }
                Hide();
            }, forceShow: true);
        }
    }
}