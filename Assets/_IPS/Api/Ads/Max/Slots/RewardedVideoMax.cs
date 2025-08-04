#if MAX
using System;
using UnityEngine;

namespace IPS.Api.Ads {
    internal class RewardedVideoMax : MaxSlotBase {
        protected override AdSlotFormat AdType => AdSlotFormat.Reward;
        protected override bool IsLoaded => MaxSdk.IsRewardedAdReady(adSlotId);

        private Action onRewardSuccess;

        public RewardedVideoMax(string slotID, bool enableLog, Action onAdLoaded) : base(slotID, enableLog) {
            this.onAdLoaded = onAdLoaded;
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnAdLoaded;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnAdFailedToLoad;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnAdFullScreenContentOpened;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnAdPaid;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnAdFullScreenContentClosed;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnAdFullScreenContentFailed;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardVideoEarnedReward;
            if (AutoRequest) Request();
        }

        protected override void DoRequest() {
            MaxSdk.LoadRewardedAd(adSlotId);
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
                MaxSdk.ShowRewardedAd(adSlotId, placement);
#if UNITY_EDITOR
                var fake = GameObject.Find("Rewarded(Clone)");
                if (fake != null) {
                    fake.GetComponent<Canvas>().sortingOrder = 32767;// Int16.MaxValue;
                }
#endif
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

        private void OnRewardVideoEarnedReward(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo) {
            Log("OnEarnedReward successfully.");
            SafeCallback(onRewardSuccess);
        }
    }
}
#endif