#if ADMOB
using GoogleMobileAds.Api;
#endif
using System;
using UnityEngine;

namespace IPS.Api.Ads {
    internal class BannerAdmob : AdmodSlotBase {
        protected override AdSlotFormat AdType => AdSlotFormat.Banner;
#if ADMOB
        protected override bool IsLoaded => ad != null;
#else
        protected override bool IsLoaded => false;
#endif

#if ADMOB
        private BannerView ad;
        private AdSize bannerSize;
        private AdPosition bannerPosition;
#endif

        bool isBannerShowing, destroying, useCollapsible;
        bool autoRefreshCollapsible;
        private Action<float> onHeightChanged;
        private string lastUUID;
        private bool useNewUUID;

        float bannerHeightRatio = 1;
        float bannerHeightCache = 0;

        public BannerAdmob(string adSlotID, BannerSize size = BannerSize.AdaptiveBanner, bool useCollapsible = true, bool autoRefreshCollapsible = false, bool showOnBottom = true, bool enableLog = false, Action<float> onHeightChanged = null) : base(adSlotID, enableLog) {
#if ADMOB
            this.bannerPosition = showOnBottom ? AdPosition.Bottom : AdPosition.Top;
            this.bannerSize = ConvertAdsize(size);
            this.onHeightChanged = onHeightChanged;
            this.useCollapsible = useCollapsible;
            this.autoRefreshCollapsible = autoRefreshCollapsible;

            if (size == BannerSize.AdaptiveBanner || size == BannerSize.SmartBanner) {
                float f = Screen.dpi / 160;
                if (f <= 2) f *= Mathf.Lerp(1.3f, 2f, 2 - f);// 2.6f;
                else if (f >= 3) f *= .73f;// 2.2f;
                else f *= Mathf.Lerp(1.3f, .73f, f - 2);
                bannerHeightRatio = f;
            }
            else bannerHeightRatio = 1;
#endif
        }

        public void SetCollapAutoRefresh(bool autoRefresh) {
            this.autoRefreshCollapsible = autoRefresh;
        }

#if ADMOB
        private AdSize ConvertAdsize(BannerSize bannerSize) {
            switch (bannerSize) {
                case BannerSize.AdaptiveBanner: return AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
                case BannerSize.SmartBanner: return AdSize.SmartBanner;
                case BannerSize.Banner_320x50: return AdSize.Banner;
                case BannerSize.IABBanner_468x60: return AdSize.IABBanner;
                case BannerSize.Leaderboard_728x90: return AdSize.Leaderboard;
                default: return AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
            }
        }
#endif

        protected override void DoRequest() {
            if (destroying) return;
            Log("Request starting...");
            CleanAd();
            DoRequestBannerDelay();
        }

        private void DoRequestBannerDelay() {
#if ADMOB
            ad = new BannerView(adSlotId, bannerSize, bannerPosition);
            ad.OnBannerAdLoaded += OnBannerAdLoaded;
            ad.OnBannerAdLoadFailed += OnAdFailedToLoad;
            ad.OnAdFullScreenContentClosed += OnBannerAdClosed;
            ad.OnAdFullScreenContentOpened += OnBannerAdDisplayed;
            ad.OnAdPaid += OnAdPaid;
            ad.OnAdClicked += OnAdClicked;

            var request = CreateAdRequest();
            if (useCollapsible) {
                request.Extras.Add("collapsible", bannerPosition == AdPosition.Bottom ? "bottom" : "top");
                if (!autoRefreshCollapsible) {
                    if (useNewUUID || string.IsNullOrEmpty(lastUUID)) lastUUID = Guid.NewGuid().ToString();
                    Log($"Load collapsible with UUID={lastUUID}, newUUID={useNewUUID}");
                    request.Extras.Add("collapsible_request_id", lastUUID);
                }
            }
            ad.LoadAd(request);
#endif
        }

        public void Show(string placement, bool useNewUUID) {
            this.useNewUUID = useNewUUID;
            Show(placement);
        }

        public void Show(string placement) {
#if ADMOB
            SetPlacement(placement);
            destroying = false;
            bool available = IsAvailable;
            if (!available && !AutoRequest) Request();
            if (isBannerShowing || !available) return;

            Log("Show Start.");
            isBannerShowing = true;
            ad.Show();
            Excutor.Schedule(onHeightChanged, BannerHeight);
#endif
        }

        public override void Hide() {
#if ADMOB
            if (ad != null) {
                isBannerShowing = false;
                ad.Hide();
                Excutor.Schedule(onHeightChanged, BannerHeight);
            }
#endif
        }

        /// <summary>
        /// Use for remove ad only. If you want to show banner after destroy, you must to call Request normaly (Use HideBanner in this case)
        /// </summary>
        public override void Destroy() {
#if ADMOB
            destroying = true;
            Log("Destroyed triggered");
            CleanAd();
#endif
        }

        private void CleanAd() {
#if ADMOB
            if (ad != null) {
                Log("Destroyed.");
                isBannerShowing = false;
                isRequesting = false;
                ad.Destroy();
                ad = null;
                Excutor.Schedule(onHeightChanged, BannerHeight);
            }
#endif
        }

        /// <summary>
        /// The height in pixel of the banner, use for controll your UI follow up banner.
        /// <para>Example: when user buy removed ads, the bottom button should be move down than normal.</para>
        /// </summary>
        public float BannerHeight {
            get {
#if ADMOB
                if (ad == null) return 0;
                if (bannerHeightCache <= 0) bannerHeightCache = (ad.GetHeightInPixels() * 160f/Screen.dpi) * bannerHeightRatio;
                return bannerHeightCache;
#else
                return 0;
#endif
            }
        }


        /// <summary>
        /// The estimate height in pixel of the banner, value is fixed event banner is showing or not
        /// </summary>
        public float BannerHeightEstimate {
            get {
#if ADMOB
                if (bannerHeightCache <= 0) bannerHeightCache = (ad.GetHeightInPixels() * 160f/Screen.dpi) * bannerHeightRatio;
#endif
                return bannerHeightCache;
            }
        }

        public int BannerHeightEstimateDp {
            get {
#if ADMOB
                return bannerSize.Height;
#else
                return 0;
#endif
            }
        }

        private void OnBannerAdLoaded() {
#if ADMOB
            base.OnAdLoaded();
            isBannerShowing = true;
            if (ad.IsCollapsible()) {
                Log("Loaded Collapsible banner");
            }
            Excutor.Schedule(onHeightChanged, BannerHeight);

            if (destroying) {
                CleanAd();
                return;
            }
#endif
        }

        private void OnBannerAdDisplayed() {
#if ADMOB
            base.OnAdFullScreenContentOpened();
            isBannerShowing =true;
#endif
            Excutor.Schedule(onHeightChanged, BannerHeight);
        }

        private void OnBannerAdClosed() {
            Log("OnAdFullScreenContentClosed");
            Excutor.Schedule(onHeightChanged, BannerHeight);
            //NewRequest(true);
        }
    }
}