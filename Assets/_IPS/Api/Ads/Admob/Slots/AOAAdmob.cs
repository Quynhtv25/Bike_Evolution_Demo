//#if ADMOB_AOA
using System;
using System.Collections;
#if ADMOB
using GoogleMobileAds.Api;
#endif
using UnityEngine;

namespace IPS.Api.Ads {
    internal class AOAAdmob : AdmodSlotBase {
        protected override AdSlotFormat AdType => AdSlotFormat.AOA;

#if ADMOB
        private AppOpenAd ad;
#endif

        public AOAAdmob(string slotID, bool enableLog, Action onAdLoaded) : base(slotID, enableLog) {
            this.onAdLoaded = onAdLoaded;
            AutoRequest = true;
            Request();
        }

#if ADMOB
        protected override bool IsLoaded => ad != null && ad.CanShowAd();
#else
        protected override bool IsLoaded => false;
#endif

        public void Show(string placement, Action onadClose = null) {
#if ADMOB
            SetPlacement(placement);
            if (!IsAvailable) {
                if (onadClose != null) onadClose.Invoke();
                return;
            }
            this.onAdClose = onadClose;
            ad.Show();
#else
            onadClose?.Invoke();
#endif
        }

        public override void Hide() { }

        public override void Destroy() {
#if ADMOB
            if (ad != null) {
                ad.Destroy();
                ad = null;
            }
#endif
        }

        protected override void DoRequest() {
#if ADMOB
            Log("Request starting...");
            ad = null;
            AppOpenAd.Load(adSlotId, CreateAdRequest(), OnAdLoadCallback);
#endif
        }

#if ADMOB
        private void OnAdLoadCallback(AppOpenAd ad, AdError error) {
            if (error != null || ad == null) {
                OnAdFailedToLoad(error);
                return;
            }

            this.ad = ad;
            this.ad.OnAdFullScreenContentClosed += OnAdFullScreenContentClosed;
            this.ad.OnAdFullScreenContentFailed += OnAdFullScreenContentFailed;
            this.ad.OnAdFullScreenContentOpened += OnAdFullScreenContentOpened;
            this.ad.OnAdPaid += OnAdPaid;

            OnAdLoaded();
        }
#endif
    }
}
//#endif