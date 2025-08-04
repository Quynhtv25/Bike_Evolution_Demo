#if MAX

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace IPS.Api.Ads {
    /// <summary>
    /// Banners are automatically sized to 320×50 on phones and 728×90 on tablets
    /// You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
    /// </summary>
    internal class BannerMax : MaxSlotBase {
        protected override AdSlotFormat AdType => AdSlotFormat.Banner;
        protected override bool IsLoaded => ad != null;

        private MaxSdkBase.BannerPosition bannerPosition;
        MaxSdkBase.AdInfo ad;

        bool useAdaptive;
        bool isBannerShowing, destroying;

        private Action<float> onHeightChanged;
        private int bannerHeightPixel;
        private int bannerHeightDp;

        private uint refreshInterval = 0;
        private bool autoRefresh = true;

        IEnumerator ieRefresh;

        public BannerMax(string adSlotID, bool useAdaptive, uint refreshInterval = 0, bool showOnBottom = true,  bool enableLog = false, Action<float> onHeightChanged = null) : base(adSlotID, enableLog) {
            this.bannerPosition = showOnBottom ? MaxSdkBase.BannerPosition.BottomCenter : MaxSdkBase.BannerPosition.TopCenter;
            this.useAdaptive = useAdaptive;
            this.refreshInterval = refreshInterval;
            this.onHeightChanged = onHeightChanged;

            autoRefresh = refreshInterval >= 10 && refreshInterval <= 120;

            MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnAdLoaded;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnAdFailedToLoad;
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAdPaid;
            MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnAdFullScreenContentOpened;
            MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnAdFullScreenContentClosed;
            MaxSdkCallbacks.Banner.OnAdClickedEvent += OnAdClicked;

            if (AutoRequest) Request();
        }

        public void SetExtra(bool adaptiveEnable, uint refreshInerval = 0) {
            if (adaptiveEnable == this.useAdaptive) return;
            this.useAdaptive = adaptiveEnable;
            this.refreshInterval = refreshInerval;

            CheckAutoRefresh();
        }

        private IEnumerator LoadBannerCustomInterval() {
            var wait = Yielder.Wait(refreshInterval);
            while(!autoRefresh) {
                MaxSdk.LoadBanner(adSlotId);
                yield return wait;
            }
        }

        private void CreateBanner() {
            Log($"CreateBanner useAdaptive={useAdaptive}, refreshInterval={refreshInterval}");

            MaxSdk.CreateBanner(adSlotId, bannerPosition);
            MaxSdk.SetBannerBackgroundColor(adSlotId, UnityEngine.Color.black);
            MaxSdk.SetBannerExtraParameter(adSlotId, "adaptive_banner", useAdaptive ? "true" : "false");

            CheckAutoRefresh();

            CalculateBannerHeight();
        }

        private void CheckAutoRefresh() {
            if (refreshInterval == 0 || (refreshInterval >= 10 && refreshInterval <= 120)) {
                autoRefresh = true;
                MaxSdk.SetBannerExtraParameter(adSlotId, "ad_refresh_seconds", refreshInterval.ToString());
                MaxSdk.StartBannerAutoRefresh(adSlotId);
            }
            else {
                autoRefresh = false;
                MaxSdk.StopBannerAutoRefresh(adSlotId);

                if (ieRefresh != null) {
                    Excutor.Instance.StopCoroutine(ieRefresh);
                }
                ieRefresh = LoadBannerCustomInterval();
                Excutor.Schedule(ieRefresh);
            }
        }

        private void CalculateBannerHeight() {
            if (bannerHeightPixel > 0) return;

            if (useAdaptive) {
                bannerHeightPixel = (int)MaxSdkUtils.GetAdaptiveBannerHeight();
                bannerHeightDp = AdsExtension.PixelToDp(bannerHeightPixel);
            }
            else {
                bannerHeightDp = !MaxSdkUtils.IsTablet() ? 50 : 90;
                bannerHeightPixel = AdsExtension.DpToPixel(bannerHeightDp);
            }

            Log($"CalculateBanner adaptive={useAdaptive}, heightPx={bannerHeightPixel}, heightDp={bannerHeightDp}");
        }

        protected override void OnAdLoaded(string adUnitId, MaxSdkBase.AdInfo ad) {
            base.OnAdLoaded(adUnitId, ad);
            if (ad != null) {
                this.ad = ad;

                if (!isBannerShowing && !destroying) {
                    Excutor.Schedule(() => { 
                        isBannerShowing = true;
                        MaxSdk.ShowBanner(adSlotId);
                        SetMaxSortingOrder();
                        Log($"Show trigger by Loaded, height={BannerHeight}px");
                        OnAdFullScreenContentOpened(adSlotId, ad);
                        if (onHeightChanged != null) onHeightChanged.Invoke(BannerHeight);                    
                    });
                }
            }

            if (destroying) {
                Destroy();
                return;
            }
        }

        protected override void OnAdFullScreenContentClosed(string adUnitId, MaxSdkBase.AdInfo ad) {
            Log("OnAdFullScreenContentClosed");
            this.ad = null;
            isBannerShowing = false;
            Excutor.Schedule(onHeightChanged, BannerHeight);
        }

        protected override void DoRequest() {
            if (destroying) return;
            Log("Request starting...");
            CleanAd();
            CreateBanner();
        }

        public void Show(string placement) {
            SetPlacement(placement);
            destroying = false;
            bool available = IsAvailable;
            if (!available && !AutoRequest) Request();
            if (isBannerShowing || !available) return;

            Log("Show Start.");
            isBannerShowing = true;
            MaxSdk.ShowBanner(adSlotId);
            SetMaxSortingOrder();
            OnAdFullScreenContentOpened(adSlotId, ad);
            CheckAutoRefresh();
            Excutor.Schedule(onHeightChanged, BannerHeight);
        }

        private void SetMaxSortingOrder() {
#if UNITY_EDITOR
            var fake = GameObject.Find("BannerBottom(Clone)");
            if (fake == null) fake = GameObject.Find("BannerTop(Clone)");
            if (fake != null) {
                fake.GetComponent<Canvas>().sortingOrder = 32767;// Int16.MaxValue;
            }
#endif
        }

        public override void Hide() {
            if (IsLoaded) {
                Log("Hide Banner");
                isBannerShowing = false;
                MaxSdk.HideBanner(adSlotId);
                if (ieRefresh != null) {
                    Excutor.Instance.StopCoroutine(ieRefresh);
                    ieRefresh = null;
                }

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
            isBannerShowing = false;
            isRequesting = false;
            if (IsLoaded) {
                Log("Destroyed by CleanAd.");
                this.ad = null;
                MaxSdk.DestroyBanner(adSlotId);
                if (ieRefresh != null) {
                    Excutor.Instance.StopCoroutine(ieRefresh);
                    ieRefresh = null;
                }
                Excutor.Schedule(onHeightChanged, BannerHeight);
            }
        }

        /// <summary>
        /// The height in pixel of the banner, use for controll your UI follow up banner.
        /// <para>Example: when user buy removed ads, the bottom button should be move down than normal.</para>
        /// </summary>
        public float BannerHeight {
            get {
                if (bannerHeightPixel <= 0) CalculateBannerHeight();
                return isBannerShowing ? bannerHeightPixel : 0;
            }
        }
        
        /// <summary>
        /// The estimate height in pixel of the banner, value is fixed event banner is showing or not
        /// </summary>
        public float BannerHeightEstimate {
            get {
                if (bannerHeightPixel <= 0) CalculateBannerHeight();
                return bannerHeightPixel;
            }
        }

        public int BannerHeightEstimateDp => bannerHeightDp;
    }
}
#endif