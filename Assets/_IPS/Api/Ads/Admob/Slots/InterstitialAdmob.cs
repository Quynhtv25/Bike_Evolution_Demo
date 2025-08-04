#if ADMOB
using GoogleMobileAds.Api;
#endif
using System;

namespace IPS.Api.Ads {
    internal class InterstitialAdmob : AdmodSlotBase {
        protected override AdSlotFormat AdType => AdSlotFormat.Inter;
        public Action onAdSuccess;

#if ADMOB
        private InterstitialAd ad;
        protected override bool IsLoaded => ad != null && ad.CanShowAd();
#else
        protected override bool IsLoaded => false;
#endif

        public InterstitialAdmob(string slotID, bool enableLog = false, Action onAdLoaded = null) : base (slotID, enableLog) {
            this.onAdLoaded = onAdLoaded;
            if (AutoRequest) Request();
        }

        protected override void DoRequest() {
#if ADMOB
            Log("Request starting...");
            Destroy();
            InterstitialAd.Load(adSlotId, CreateAdRequest(), OnAdLoadCallback);
#endif
        }

#if ADMOB
        private void OnAdLoadCallback(InterstitialAd ad, LoadAdError err) {
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

        protected override void OnAdFullScreenContentClosed() {
            SafeCallback(onAdSuccess);
            base.OnAdFullScreenContentClosed();
        }
#endif

        public void Show(string placement, Action callback = null) {
#if ADMOB
            SetPlacement(placement);
            if (IsAvailable) {
                Log("Show start..");
                this.onAdClose = callback;
                ad.Show();
            }
            else {
                Log("Show failed: ad not ready. Invoke callback.");
                this.onAdClose = null;
                callback?.Invoke();
            }
#else
            callback?.Invoke();
#endif
        }

        public override void Hide() {

        }

        public override void Destroy() {
#if ADMOB
            if (ad != null) {
                ad.Destroy();
                ad = null;
            }
#endif
        }

    }
}