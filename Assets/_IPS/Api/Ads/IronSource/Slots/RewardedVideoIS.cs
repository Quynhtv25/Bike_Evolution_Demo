#if IS
using System;

namespace IPS.Api.Ads {
    internal class RewardedVideoIS : ISSlotBase {
        protected override AdSlotFormat AdType => AdSlotFormat.Reward;
        protected override bool IsLoaded => IronSource.Agent.isRewardedVideoAvailable();

        private Action onRewardSuccess;

        public RewardedVideoIS(bool enableLog, Action onAdLoaded) : base(enableLog) {
            this.onAdLoaded = onAdLoaded;
            IronSourceRewardedVideoEvents.onAdAvailableEvent += OnAdLoaded;
            IronSourceRewardedVideoEvents.onAdLoadFailedEvent += OnAdFailedToLoad;
            IronSourceRewardedVideoEvents.onAdUnavailableEvent += OnAdUnavailable;
            IronSourceRewardedVideoEvents.onAdOpenedEvent += OnAdFullScreenContentOpened;
            IronSourceRewardedVideoEvents.onAdOpenedEvent += OnAdPaid;
            IronSourceRewardedVideoEvents.onAdClosedEvent += OnAdFullScreenContentClosed;
            IronSourceRewardedVideoEvents.onAdShowFailedEvent += OnAdFullScreenContentFailed;
            IronSourceRewardedVideoEvents.onAdRewardedEvent += OnRewardVideoEarnedReward;
            IronSource.Agent.setManualLoadRewardedVideo(true);
            if (AutoRequest) Request();
        }

        protected override void DoRequest() {
            Log("Request starting...");
            IronSource.Agent.loadRewardedVideo();
        }

        /// <summary>
        /// `onAdClosed` always be call although success or failure.
        /// </summary>
        /// <param name="onRewardSuccess"></param>
        /// <param name="onAdClosed"></param>
        public void Show(string placement, Action onRewardSuccess, Action onAdClosed = null) {
            SetPlacement(placement);
            if (IsAvailable) {
                this.onRewardSuccess = onRewardSuccess;
                this.onAdClose = onAdClosed;

                Log("Show start...");
                IronSource.Agent.showRewardedVideo(placement);
            }
            else {
                Log("Show failed: ad not ready. Invoke onClosed callback.");
                this.onRewardSuccess = null;
                this.onAdClose = null;
                onAdClosed?.Invoke();
            }
        }

        public override void Hide() {
        }

        public override void Destroy() {
        }

        private void OnAdUnavailable() {
            LogError("OnAdUnavailable");
        }

        private void OnRewardVideoEarnedReward(IronSourcePlacement placement, IronSourceAdInfo ad) {
            Log("OnEarnedReward successfully.");
            SafeCallback(onRewardSuccess);
        }
    }
}
#endif