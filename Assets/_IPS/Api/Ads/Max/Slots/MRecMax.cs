#if MAX

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace IPS.Api.Ads {
    /// <summary>
    /// MRECs are sized to 300x250 on phones and tablets
    /// </summary>
    internal class MRecMax : MaxSlotBase {
        protected override AdSlotFormat AdType => AdSlotFormat.MRec;
        protected override bool IsLoaded => ad != null;
        public bool IsShowing => isShowing;

        MaxSdkBase.AdInfo ad;
        bool isShowing, destroying;
        MaxSdkBase.AdViewPosition fixedPosition;
        Vector2 customPosition;
        Vector2 adSizePixel;

#if UNITY_EDITOR
        public static readonly Vector2Int SizeDp = new Vector2Int(300, 250); 
#else
        public readonly Vector2Int SizeDp = new Vector2Int(300, 250); 
#endif

        private bool showOnBottom;
        private bool UseCustomPosition => customPosition.y > 0;

        public MRecMax(string adSlotID, bool showOnBottom, bool enableLog = false) : base(adSlotID, enableLog) {
            RegisterEvent();
            this.showOnBottom = showOnBottom;
            fixedPosition = showOnBottom ? MaxSdkBase.AdViewPosition.BottomCenter : MaxSdkBase.AdViewPosition.TopCenter;
            Log($"MRec init with fixedPosition={fixedPosition.ToString()}");
        }

        public MRecMax(string adSlotID, float yPadding, bool onBottom, bool enableLog) : base(adSlotID, enableLog) {
            RegisterEvent();
            this.showOnBottom = onBottom;
            if (yPadding > 0) {
                Log($"MRec init with yPadding={yPadding}dp, onBottom={onBottom}");
                SetPaddingY(yPadding, false);
            }
            else {
                fixedPosition = showOnBottom ? MaxSdkBase.AdViewPosition.BottomCenter : MaxSdkBase.AdViewPosition.TopCenter;
                Log($"MRec init with fixedPosition={fixedPosition.ToString()}");
            }
        }

        private void RegisterEvent() {
            MaxSdkCallbacks.MRec.OnAdLoadedEvent += OnAdLoaded;
            MaxSdkCallbacks.MRec.OnAdLoadFailedEvent += OnAdFailedToLoad;
            MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnAdPaid;
            MaxSdkCallbacks.MRec.OnAdExpandedEvent += OnAdFullScreenContentOpened;
            MaxSdkCallbacks.MRec.OnAdCollapsedEvent += OnAdFullScreenContentClosed;
            MaxSdkCallbacks.MRec.OnAdClickedEvent += OnAdClicked;

            adSizePixel = AdsExtension.DpToPixel(SizeDp.x, SizeDp.y);

            if (AutoRequest) Request();
        }

        /// <summary>The (x, y) anchor coordinate of the MREC relative to the top left corner of the screen. Param yPadding is a pixel which is padding from top screen to the top of the MRec</summary>
        public void SetPaddingY(float yPadding, bool destroyCurrent = false) {
            if (yPadding <= 0) return;
            float density = AdsExtension.GetScreenDensity(true);
            // Convert value from pixel to dp for android, pt for iOS: px = dp * Screen.dpi / 160 => dp = px * 160 / Screen.dpi
            this.customPosition.x = (Screen.width/2 - adSizePixel.x/2) / density;
            this.customPosition.y = (showOnBottom ? (Screen.height - adSizePixel.y)/density - yPadding : yPadding);
            Log($"SetPosition: final={customPosition}, positionY={yPadding}, MaxDensity={density}, densityDpi={Screen.dpi / 160f}, adSizePixel={adSizePixel}");

            if (destroyCurrent) CleanAd();
        }

        private void CreateMRec() {
            if (UseCustomPosition) {
                Log($"CreateMRec customPositionY={customPosition.y}dp");
                MaxSdk.CreateMRec(adSlotId, customPosition.x, customPosition.y);
            }
            else {
                Log($"CreateMRec fixedPosition={fixedPosition}");
                MaxSdk.CreateMRec(adSlotId, fixedPosition);
            }
        }

        protected override void OnAdLoaded(string adUnitId, MaxSdkBase.AdInfo ad) {
            base.OnAdLoaded(adUnitId, ad);
            if (ad != null) {
                this.ad = ad;

                if (isShowing && !destroying) {
                    Log("Show from OnAdLoaded.");
                    isShowing = true;
                    MaxSdk.ShowMRec(adSlotId);
                    OnAdFullScreenContentOpened(adUnitId, ad);
                }
            }

            if (destroying) {
                Destroy();
                return;
            }
        }

        protected override void OnAdFullScreenContentClosed(string adUnitId, MaxSdkBase.AdInfo ad) {
            if (!adUnitId.Equals(adSlotId)) return;
            this.ad = null;
            isShowing = false;
            base.OnAdFullScreenContentClosed(adUnitId, ad);
        }

        protected override void DoRequest() {
            if (destroying) return;
            Log("Request starting...");
            CleanAd();
            CreateMRec();
        }

        public void Show(string placement) {
            if (isShowing) {
                Log($"MRec currently showing, change placement from={this.placement}, to={placement}");
                SetPlacement(placement);
                return;
            }

            SetPlacement(placement);
            isShowing = true;

            destroying = false;
            bool available = IsAvailable;
            if (!available && !AutoRequest) Request();

            if (!available) return;

            if (IsLoaded) {
                MaxSdk.ShowMRec(adSlotId);
                OnAdFullScreenContentOpened(adSlotId, ad);
            }
        }

        public override void Hide() {
            if (IsLoaded && isShowing) {
                MaxSdk.HideMRec(adSlotId);
                Log(string.Format("OnAdFullScreenContentHide placement={0}", placement));
                SafeCallback(onAdClose);
            }

            isShowing = false;
        }

        /// <summary>
        /// Use for remove ad only. If you want to show MRec after destroy, you must to call Request normaly (Use HideMRec in this case)
        /// </summary>
        public override void Destroy() {
            destroying = true;
            Log("Destroy triggered.");
            CleanAd();
        }

        private void CleanAd() {
            isShowing = false;
            if (IsLoaded) {
                Log("Destroyed.");
                isRequesting = false;
                this.ad = null;
                MaxSdk.DestroyMRec(adSlotId);
            }
        }

        public Vector2 Size => isShowing ? adSizePixel : default;
        public Vector2 SizeEstimate {
            get {
                if (adSizePixel.x == 0) adSizePixel = AdsExtension.DpToPixel(SizeDp.x, SizeDp.y);
                return adSizePixel;
            }
        }
    }
}
#endif