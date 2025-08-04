#if MAX
using System;
using UnityEngine;

namespace IPS.Api.Ads {
    internal class AOAMax : MaxSlotBase {
        protected override AdSlotFormat AdType => AdSlotFormat.AOA;
        protected override bool IsLoaded => MaxSdk.IsAppOpenAdReady(adSlotId);

        public AOAMax(string slotID, bool enableLog, Action onAdLoaded) : base(slotID, enableLog) {
            this.onAdLoaded = onAdLoaded;

            MaxSdkCallbacks.AppOpen.OnAdLoadedEvent += OnAdLoaded;
            MaxSdkCallbacks.AppOpen.OnAdLoadFailedEvent += OnAdFailedToLoad;
            MaxSdkCallbacks.AppOpen.OnAdDisplayedEvent += OnAdFullScreenContentOpened;
            MaxSdkCallbacks.AppOpen.OnAdHiddenEvent += OnAdFullScreenContentClosed;
            MaxSdkCallbacks.AppOpen.OnAdDisplayFailedEvent += OnAdFullScreenContentFailed;
            MaxSdkCallbacks.AppOpen.OnAdClickedEvent += OnAdClicked;
            MaxSdkCallbacks.AppOpen.OnAdRevenuePaidEvent += OnAdPaid;

            AutoRequest = true;
            Request();
        }

        protected override void DoRequest() {
            Log("Request starting...");
            Destroy();
            MaxSdk.LoadAppOpenAd(adSlotId);
        }

        public void Show(string placement, Action onadClose = null) {
            SetPlacement(placement);
            if (!IsAvailable) {
                if (onadClose != null) onadClose.Invoke();
                return;
            }
            this.onAdClose = onadClose;
            MaxSdk.ShowAppOpenAd(adSlotId, placement);
#if UNITY_EDITOR
            var fake = GameObject.Find("Interstitial(Clone)");
            if (fake != null) {
                fake.GetComponent<Canvas>().sortingOrder = 32767;// Int16.MaxValue;
            }
#endif
        }

        public override void Hide() { }

        public override void Destroy() {
        }
    }
}
#endif