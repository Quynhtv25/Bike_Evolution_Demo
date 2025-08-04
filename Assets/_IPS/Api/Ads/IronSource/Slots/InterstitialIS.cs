#if IS
using System;

namespace IPS.Api.Ads {
    internal class InterstitialIS : ISSlotBase {
        protected override AdSlotFormat AdType => AdSlotFormat.Inter;
        public Action onAdSuccess;
        protected override bool IsLoaded => IronSource.Agent.isInterstitialReady();

        public InterstitialIS(bool enableLog = false, Action onAdLoaded = null) : base (enableLog) {
            this.onAdLoaded = onAdLoaded;

            IronSourceInterstitialEvents.onAdReadyEvent += OnAdLoaded;
            IronSourceInterstitialEvents.onAdLoadFailedEvent += OnAdFailedToLoad;
            IronSourceInterstitialEvents.onAdClosedEvent += OnAdFullScreenContentClosed;
            IronSourceInterstitialEvents.onAdShowFailedEvent += OnAdFullScreenContentFailed;
            IronSourceInterstitialEvents.onAdShowSucceededEvent += OnAdFullScreenContentOpened;
            IronSourceInterstitialEvents.onAdShowSucceededEvent += OnAdPaid;

            if (AutoRequest) Request();
        }

        protected override void DoRequest() {
            Log("Request starting...");
            Destroy();
            IronSource.Agent.loadInterstitial();
        }

        public void Show(string placement, Action callback = null) {
            SetPlacement(placement);
            if (IsAvailable) {
                Log("Show start..");
                this.onAdClose = callback;
                IronSource.Agent.showInterstitial(placement);
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

        protected override void OnAdFullScreenContentClosed(IronSourceAdInfo ad) {            
            SafeCallback(onAdSuccess);
            base.OnAdFullScreenContentClosed(ad);
        }
    }
}
#endif