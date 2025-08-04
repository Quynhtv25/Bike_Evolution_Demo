#if MAX
using System;
using UnityEngine;

namespace IPS.Api.Ads {
    internal class InterstitialMax : MaxSlotBase {
        protected override AdSlotFormat AdType => AdSlotFormat.Inter;
        public Action onAdSuccess;
        protected override bool IsLoaded => MaxSdk.IsInterstitialReady(adSlotId);

        public InterstitialMax(string slotID, bool enableLog = false, Action onAdLoaded = null) : base (slotID, enableLog) {
            this.onAdLoaded = onAdLoaded;

            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnAdLoaded;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnAdFailedToLoad;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnAdFullScreenContentOpened;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnAdFullScreenContentClosed;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnAdFullScreenContentFailed;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnAdClicked;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnAdPaid;

            if (AutoRequest) Request();
        }

        protected override void DoRequest() {
            Log("Request starting...");
            Destroy();
            MaxSdk.LoadInterstitial(adSlotId);
        }

        public void Show(string placement, Action callback = null) {
            SetPlacement(placement);
            if (IsAvailable) {
                Log("Show start..");
                this.onAdClose = callback;
                MaxSdk.ShowInterstitial(adSlotId);
#if UNITY_EDITOR
                var fake = GameObject.Find("Interstitial(Clone)");
                if (fake != null) {
                    fake.GetComponent<Canvas>().sortingOrder = 32767;// Int16.MaxValue;
                }
#endif
            }
            else {
                Log("Show failed: ad not ready. Invoke callback.");
                this.onAdClose = null;
                callback?.Invoke();
            }
        }

        public override void Hide() {

        }

        public override void Destroy() {

        }

        protected override void OnAdFullScreenContentClosed(string adUnitId, MaxSdkBase.AdInfo ad) {
            SafeCallback(onAdSuccess);
            base.OnAdFullScreenContentClosed(adUnitId, ad);
        }
    }
}
#endif