#if ADMOB_NATIVE
using GoogleMobileAds.Api;
using System;
using UnityEngine;

namespace IPS.Api.Ads {
    internal class NativeAdmob : AdmodSlotBase {
        protected override AdSlotFormat AdType => AdSlotFormat.Native;
        protected override bool IsLoaded => ad != null;

        private NativeAd ad;
        private Action<NativeAd> onNativeAdLoaded;

        private bool showing;

        public NativeAdmob(string slotID, bool preload, bool enableLog = false) : base(slotID, enableLog) {
            AutoRequest = true;
            if (preload) Request();
        }

        protected override void DoRequest() {
            //if (onNativeAdLoaded == null) return;
            Log("Request starting...");
            CleanAd();
            AdLoader adLoader = new AdLoader.Builder(adSlotId).ForNativeAd().Build();
            adLoader.OnAdFailedToLoad += OnNativeAdFailedToLoad;
            adLoader.OnNativeAdLoaded += OnNativeAdLoaded;
            adLoader.OnNativeAdClosed += OnNativeAdClosed;
            adLoader.OnNativeAdClicked += OnNativeAdClicked;

            adLoader.LoadAd(CreateAdRequest());
        }

        public void Show(string placement, Action<NativeAd> onSuccess) {
            if (showing) Destroy();

            onSuccess += (ad) => { showing = true; };
            SetPlacement(placement);

            if (IsAvailable) {
                this.onNativeAdLoaded = null;
                onSuccess?.Invoke(ad);
            }
            else {
                this.onNativeAdLoaded = onSuccess;
                if (!isRequesting) Request();
            }
        }

        public override void Hide() {

        }

        public override void Destroy() {
            showing = false;
            this.onNativeAdLoaded = null;
            CleanAd();
        }

        private void CleanAd() {
            if (ad != null) {
                ad.Destroy();
                ad = null;
            }
        }

        private void OnNativeAdFailedToLoad(object sender, AdFailedToLoadEventArgs arg) {
            base.OnAdFailedToLoad(arg.LoadAdError);
        }

        private void OnNativeAdLoaded(object sender, NativeAdEventArgs arg) {
            this.ad = arg.nativeAd;
            this.ad.OnPaidEvent += OnAdPaid;
            base.OnAdLoaded();
            Excutor.Schedule(() => {
                onNativeAdLoaded?.Invoke(ad);
                onNativeAdLoaded = null;
            });
        }

        private void OnAdPaid(object sender, AdValueEventArgs arg) {
            base.OnAdPaid(arg.AdValue);
        }

        private void OnNativeAdClosed(object sender, EventArgs arg) {
            base.OnAdFullScreenContentClosed();
        }

        private void OnNativeAdClicked(object sender, EventArgs arg) {
            base.OnAdClicked();
        }
    }
}
#endif