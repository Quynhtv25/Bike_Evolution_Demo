#if IS

using com.unity3d.mediation;
using System;
using UnityEngine;

namespace IPS.Api.Ads {
    /// <summary>
    /// Banners are automatically sized to Adaptive 320×50 on phones and 728×90 on tablets
    /// </summary>
    internal class MRecIS : ISSlotBase {
        protected override AdSlotFormat AdType => AdSlotFormat.MRec;
        protected override bool IsLoaded => isLoaded;
        public bool IsShowing => isShowing;

        bool isShowing, destroying;
        bool isLoaded;
        Vector2 adSizePixel;

        private LevelPlayAdSize adSize; // default use MREC = 300x250 (pt in iOS, dp in Android)
        private IronSourceBannerPosition adPosition; // follow ScreenAnchor: TopLeft (0,0), BottomRight (Screen.width, Screen.height). The top-left corner of the BannerView is positioned at the x and y values passed to the constructor, where the origin is the top-left of the screen.


        public MRecIS(bool showOnBottom = true, bool enableLog = false) : base(enableLog) {
            this.adPosition = showOnBottom ? IronSourceBannerPosition.BOTTOM : IronSourceBannerPosition.TOP;

            IronSourceBannerEvents.onAdLoadedEvent += OnAdLoaded;
            IronSourceBannerEvents.onAdLoadFailedEvent += OnAdFailedToLoad;
            IronSourceBannerEvents.onAdScreenDismissedEvent += OnAdFullScreenContentClosed;
            IronSourceBannerEvents.onAdScreenPresentedEvent += OnAdFullScreenContentOpened;
            IronSourceBannerEvents.onAdScreenPresentedEvent += OnAdPaid;
            IronSourceBannerEvents.onAdClickedEvent += OnAdClick;

            this.adSize = LevelPlayAdSize.MEDIUM_RECTANGLE;
            adSizePixel = AdsExtension.DpToPixel(adSize.Width, adSize.Height);
        }

        protected override void OnAdLoaded(IronSourceAdInfo ad) {
            isLoaded = true;
            isShowing = true;
            base.OnAdLoaded(ad);

            if (destroying) {
                Destroy();
                return;
            }

        }

        protected override void OnAdFullScreenContentClosed(IronSourceAdInfo ad) {
            Log("OnAdFullScreenContentClosed");
            isShowing = false;
            isLoaded = false;
            NewRequest(true);
        }

        protected override void DoRequest() {
            Log("Request starting...");
            Destroy();
            destroying = false;
            //(adSize, adPosition);
        }

        public void Show(string placement) {
            SetPlacement(placement);
            destroying = false;
            if (isShowing || !IsAvailable) return;

            Log("Show Start.");
            isShowing = true;
            IronSource.Agent.displayBanner();
        }

        public override void Hide() {
            if (IsLoaded) {
                isShowing = false;
                isLoaded = false;
                IronSource.Agent.hideBanner();
            }
        }

        /// <summary>
        /// Use for remove ad only. If you want to show banner after destroy, you must to call Request normaly (Use HideBanner in this case)
        /// </summary>
        public override void Destroy() {
            if (IsLoaded) {
                Log("Destroyed.");
                isShowing = false;
                isLoaded = false;
                isRequesting = false;
                IronSource.Agent.destroyBanner();
            }
            else if (isRequesting) destroying = true;
        }

        /// <summary>
        /// The height in pixel of the banner, use for controll your UI follow up banner.
        /// <para>Example: when user buy removed ads, the bottom button should be move down than normal.</para>
        /// </summary>
        public float Height => isShowing ? adSize.Height : 0;

        public Vector2 Size => isShowing ? adSizePixel : Vector2.zero;
        public Vector2 SizeEstimate => adSizePixel;
     
    }
}
#endif