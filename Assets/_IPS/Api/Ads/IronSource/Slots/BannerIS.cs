#if IS

using System;
using UnityEngine;

namespace IPS.Api.Ads {
    /// <summary>
    /// Banners are automatically sized to Adaptive 320×50 on phones and 728×90 on tablets
    /// </summary>
    internal class BannerIS : ISSlotBase {
        protected override AdSlotFormat AdType => AdSlotFormat.Banner;
        protected override bool IsLoaded => isLoaded;

        private IronSourceBannerSize bannerSize;
        private IronSourceBannerPosition bannerPosition;

        bool isBannerShowing, destroying;
        bool isLoaded;

        float bannerHeight;
        private Action<float> onHeightChanged;

        public BannerIS(bool useAdaptive, bool showOnBottom = true, bool enableLog = false, Action<float> onHeightChanged = null) : base(enableLog) {
            this.bannerPosition = showOnBottom ? IronSourceBannerPosition.BOTTOM : IronSourceBannerPosition.TOP;
            this.onHeightChanged = onHeightChanged;

            IronSourceBannerEvents.onAdLoadedEvent += OnAdLoaded;
            IronSourceBannerEvents.onAdLoadedEvent += OnAdPaid;
            IronSourceBannerEvents.onAdLoadFailedEvent += OnAdFailedToLoad;
            IronSourceBannerEvents.onAdScreenDismissedEvent += OnAdFullScreenContentClosed;
            IronSourceBannerEvents.onAdScreenPresentedEvent += OnAdFullScreenContentOpened;
            IronSourceBannerEvents.onAdClickedEvent += OnAdClick;

            bannerSize = ConvertAdsize(useAdaptive);

            if (useAdaptive) {
#if !IS_770 //newest version > 7.7.0
                float width = IronSource.Agent.getDeviceScreenWidth();
                float height = IronSource.Agent.getMaximalAdaptiveHeight(width) * Screen.dpi / 160f;
                ISContainerParams isContainerParams = new ISContainerParams { Width = width, Height = height };
                bannerSize.setBannerContainerParams(isContainerParams);
#endif
                bannerSize.SetAdaptive(true);
            }
            bannerHeight = AdsExtension.DpToPixel((Screen.height.PixelToDp() >= 720 ? 90 : 50));
        }

#if ADS
        private IronSourceBannerSize ConvertAdsize(bool useAdaptive) {
            return !useAdaptive ? IronSourceBannerSize.BANNER : IronSourceBannerSize.SMART;
        }
#endif
        protected override void OnAdLoaded(IronSourceAdInfo ad) {
            if (ad != null) {
                base.OnAdLoaded(ad);
                isLoaded = true;
                isBannerShowing = true;
                Excutor.Schedule(onHeightChanged, BannerHeight);
            }

            if (destroying) {
                CleanAd();
                return;
            }
        }

        protected override void OnAdFullScreenContentClosed(IronSourceAdInfo ad) {
            Log("OnAdFullScreenContentClosed");
            isLoaded = false;
            isBannerShowing = false;
            Excutor.Schedule(onHeightChanged, BannerHeight);
            //NewRequest(true);
        }

        protected override void DoRequest() {
            if (destroying) return;
            Log("Request starting...");
            CleanAd();
            IronSource.Agent.loadBanner(bannerSize, bannerPosition);
        }

        public void Show(string placement) {
            SetPlacement(placement);
            destroying = false;
            bool available = IsAvailable;
            if (!available && !AutoRequest) Request();
            if (isBannerShowing || !available) return;

            Log("Show Start.");
            isBannerShowing = true;
            IronSource.Agent.displayBanner();
            Excutor.Schedule(onHeightChanged, BannerHeight);
        }

        public override void Hide() {
            if (isLoaded) {
                isBannerShowing = false;
                IronSource.Agent.hideBanner();
                Excutor.Schedule(onHeightChanged, BannerHeight);
            }
        }

        /// <summary>
        /// Use for remove ad only. If you want to show banner after destroy, you must to call Request normaly (Use HideBanner in this case)
        /// </summary>
        public override void Destroy() {
            destroying = true;
            Log("Destroy triggered.");
            CleanAd();
        }

        private void CleanAd() {
            if (isLoaded) {
                Log("Destroyed.");
                isBannerShowing = false;
                isRequesting = false;
                isLoaded = false;
                IronSource.Agent.destroyBanner();
                Excutor.Schedule(onHeightChanged, BannerHeight);
            }
        }

        /// <summary>
        /// The height in pixel of the banner, use for controll your UI follow up banner.
        /// <para>Example: when user buy removed ads, the bottom button should be move down than normal.</para>
        /// </summary>
        public float BannerHeight => isBannerShowing ? bannerHeight : 0f;

        /// <summary>
        /// The estimate height in pixel of the banner, value is fixed event banner is showing or not
        /// </summary>
        public float BannerHeightEstimate => bannerHeight;

        /// <summary>
        /// The estimate height in dp of the banner, value is fixed event banner is showing or not
        /// </summary>
        public int BannerHeightEstimateDp => bannerSize.Height;
    }
}
#endif