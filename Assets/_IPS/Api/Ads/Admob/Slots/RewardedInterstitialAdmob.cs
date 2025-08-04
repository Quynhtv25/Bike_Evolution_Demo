using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if ADMOB
using GoogleMobileAds.Api;
#endif

namespace IPS.Api.Ads {
    internal class RewardedInterstitialAdmob : AdmodSlotBase {
#if ADMOB
        private RewardedInterstitialAd ad;
#endif
        protected override AdSlotFormat AdType => AdSlotFormat.RewardInter;
#if ADMOB
        protected override bool IsLoaded => ad != null && ad.CanShowAd();
#else
        protected override bool IsLoaded => false;
#endif

        private Action onRewardSuccess;

        public RewardedInterstitialAdmob(string slotID, bool enableLog, Action onAdLoaded) : base(slotID, enableLog) {
            this.onAdLoaded = onAdLoaded;
            AutoRequest = true;
            if (AutoRequest) Request();
        }

        protected override void DoRequest() {
#if ADMOB
            if (ad != null) {
                ad.Destroy();
                ad = null;
            }

            var adRequest = CreateAdRequest();
            //adRequest.Keywords.Add();
            RewardedInterstitialAd.Load(adSlotId, adRequest, OnAdLoadCallback);
#endif
        }

#if ADMOB
        private void OnAdLoadCallback(RewardedInterstitialAd ad, LoadAdError err) {
            if (err != null || ad == null) {
                OnAdFailedToLoad(err);
                return;
            }

            this.ad = ad;
            this.ad.OnAdFullScreenContentOpened += OnAdFullScreenContentOpened;
            this.ad.OnAdFullScreenContentClosed += OnAdFullScreenContentClosed;
            this.ad.OnAdFullScreenContentFailed += OnAdFullScreenContentFailed;
            this.ad.OnAdClicked += OnAdClicked;
            this.ad.OnAdPaid += OnAdPaid;

            OnAdLoaded();
        }
#endif

        /// <summary>
        /// `onAdClosed` always be call although success or failure.
        /// </summary>
        /// <param name="onRewardSuccess"></param>
        /// <param name="onAdClosed"></param>
        public void Show(string placement, Action onRewardSuccess, Action onAdClosed = null) {
#if ADMOB
            SetPlacement(placement);
            if (IsAvailable) {
                this.onRewardSuccess = onRewardSuccess;
                this.onAdClose = onAdClosed;

                Log("Show start...");
                ad.Show(OnRewardVideoEarnedReward);
            }
            else {
                Log("Show failed: ad not ready. Invoke onClosed callback.");
                this.onRewardSuccess = null;
                this.onAdClose = null;
                onAdClosed?.Invoke();
            }
#else
            onAdClosed?.Invoke();
#endif
        }

        public override void Hide() {
        }

        public override void Destroy() {
        }

#if ADMOB
        private void OnRewardVideoEarnedReward(GoogleMobileAds.Api.Reward reward) {
            Log("OnEarnedReward successfully.");
            SafeCallback(onRewardSuccess);
        }
#endif
    }
}