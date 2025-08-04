#if ADMOB
using GoogleMobileAds.Api;


#endif
using UnityEngine;

namespace IPS.Api.Ads {
    internal class MRecAdmob : AdmodSlotBase {
        protected override AdSlotFormat AdType => AdSlotFormat.MRec;
#if ADMOB
        protected override bool IsLoaded => ad != null;
#else
        protected override bool IsLoaded => false;
#endif
        public bool IsShowing => isShowing;
#if ADMOB
        private BannerView ad;
        private AdSize adSize; // default use MREC = 300x250 (pt in iOS, dp in Android)
        Vector2 adSizePixel;
        Vector2Int adPosition; // follow ScreenAnchor: TopLeft (0,0), BottomRight (Screen.width, Screen.height). The top-left corner of the BannerView is positioned at the x and y values passed to the constructor, where the origin is the top-left of the screen.
#endif

        bool showOnBottom;
        bool isShowing, destroying;

        public MRecAdmob(string adSlotID, bool showOnBottom, bool enableLog = false) : base(adSlotID, enableLog) {
            this.showOnBottom = showOnBottom;
        }

        protected override void DoRequest() {
            Destroy();
            destroying = false;

#if ADMOB
            Log($"Request Start at ({adPosition.x}, {adPosition.y}), size=({adSize.Width},{adSize.Height}).");
            ad = new BannerView(adSlotId, adSize, adPosition.x, adPosition.y);

            ad.OnBannerAdLoaded += OnMRecAdLoaded;
            ad.OnBannerAdLoadFailed += OnAdFailedToLoad;
            ad.OnAdFullScreenContentClosed += OnBannerAdClosed;
            ad.OnAdFullScreenContentOpened += OnAdFullScreenContentOpened;
            ad.OnAdPaid += OnAdPaid;
            ad.OnAdClicked += OnAdClicked;

            var request = CreateAdRequest();
            ad.LoadAd(request);
#endif
        }

//        public void Show() {
//#if ADMOB
//            Show(adPosition.x, adPosition.y);
//#endif
//        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="centerPoint"> follow ScreenAnchor: TopLeft (0,0), BottomRight (Screen.width, Screen.height). Ad anchor also is Top-Left</param>
        /// <param name="adSize">default use MREC = 300x250 (pt in iOS, dp in Android)</param>
        public void Show(string placement, float yPadding, Vector2Int? adSize) {
#if ADMOB
            SetPlacement(placement);

            float density = Screen.dpi / 160f;

            if (adSize.HasValue) {
                int w = (int)(adSize.Value.x / density);
                int h = (int)(adSize.Value.y / density);

                if (w < 0) {
                    Logs.Log($"Invalid adSize width = {w}dp. Use default = 320dp");
                    w = AdSize.MediumRectangle.Width;
                }

                if (h < 0) {
                    Logs.Log($"Invalid adSize height = {h}dp. Use default = 50dp");
                    h = AdSize.MediumRectangle.Height;
                }

                this.adSize = new AdSize(w, h);
                this.adSizePixel = new Vector2(adSize.Value.x, adSize.Value.y);
            }
            else {
                this.adSize = AdSize.MediumRectangle;
                this.adSizePixel = new Vector2(this.adSize.Width * density, this.adSize.Height * density);
            }

            // Convert value from pixel to dp for android, pt for iOS: px = dp * Screen.dpi / 160 => dp = px * 160 / Screen.dpi
            adPosition.x = (int)(Mathf.Abs((Screen.width/2 / density) - this.adSize.Width/2));
            adPosition.y = (int)(Mathf.Abs(showOnBottom ? (Screen.height - yPadding)/density - this.adSize.Height : yPadding/ density));

            destroying = false;
            isShowing = true;
//            Log($"Prepare Show MRec at ({centerPoint.x}, {centerPoint.y}), size=({adSize.Value.x},{adSize.Value.y}).");

            if (!IsAvailable || isShowing) return;

            ad.Show();
#endif
        }

        public override void Hide() {
#if ADMOB
            if (ad != null) {
                isShowing = false;
                ad.Hide();
            }
#endif
        }

        /// <summary>
        /// Use for remove ad only. If you want to show banner after destroy, you must to call Request normaly (Use HideBanner in this case)
        /// </summary>
        public override void Destroy() {
#if ADMOB
            if (ad != null) {
                Log("Destroyed.");
                isShowing = false;
                isRequesting = false;
                ad.Destroy();
                ad = null;
            }
            else if (isRequesting) destroying = true;
#endif
        }

        /// <summary>
        /// The height in pixel of the banner, use for controll your UI follow up banner.
        /// <para>Example: when user buy removed ads, the bottom button should be move down than normal.</para>
        /// </summary>
#if ADMOB
        public Vector2 Size => (ad != null/* && isBannerShowing*/) ? adSizePixel : default;
        public Vector2 SizeEstimate => adSizePixel;
#else
        public Vector2 Size => default;
        public Vector2 SizeEstimate => default;
#endif

        private void OnMRecAdLoaded() {
#if ADMOB
            base.OnAdLoaded();
            isShowing = true;

            if (destroying) {
                Destroy();
                return;
            }

            Log($"Prepare Show MRec at ({adPosition.x}, {adPosition.y})dp, size=({adSize.Width},{adSize.Height})dp.");
#endif
        }

        private void OnBannerAdClosed() {
            Log("OnAdFullScreenContentClosed");
            isShowing = false;
            NewRequest(true);
        }
    }
}