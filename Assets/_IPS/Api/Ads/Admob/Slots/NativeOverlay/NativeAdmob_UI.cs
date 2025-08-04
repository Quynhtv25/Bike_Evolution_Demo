#if ADMOB_NATIVE_UI
using GoogleMobileAds.Api;
using System;
using UnityEngine;

namespace IPS.Api.Ads {
    internal class NativeAdmob_UI : AdmodSlotBase {
        protected override AdSlotFormat AdType => AdSlotFormat.Native;
#if ADS
        protected override bool IsLoaded => ad != null;
#else
        protected override bool IsLoaded => false;
#endif

        NativeOverlayAd  ad;
        private bool showing;

        public NativeAdmob_UI(string slotID, bool preload, bool enableLog = false) : base(slotID, enableLog) {
            AutoRequest = true;
            if (preload) Request();
        }

        protected override void DoRequest() {
            //if (onNativeAdLoaded == null) return;
            Log("Request starting...");
            CleanAd();

            var options = new NativeAdOptions {
                AdChoicesPlacement = AdChoicesPlacement.TopRightCorner,
                MediaAspectRatio = MediaAspectRatio.Any,
            };

            NativeOverlayAd.Load(adSlotId, CreateAdRequest(), options, OnNativeOverlayAdLoadCallback);
        }

        public void Show(string placement) {
            if (showing) Destroy();

            SetPlacement(placement);

            bool available = IsAvailable;
            if (!available && !AutoRequest) Request();
            if (showing || !available) return;

            Log("Show Start.");
            showing = true;

            // Define a native template style with a custom style.
            var style = new NativeTemplateStyle {
                TemplateId = NativeTemplateId.Medium,
                MainBackgroundColor = Color.white,
                CallToActionText = new NativeTemplateTextStyle {
                    BackgroundColor = Color.green,
                    TextColor = Color.white,
                    FontSize = 9,
                    Style = NativeTemplateFontStyle.Bold
                }
            };

            // Renders a native overlay ad at the default size
            // and anchored to the bottom of the screne.
            ad.RenderTemplate(style, AdPosition.Bottom);
            ad.Show();
        }

        public override void Hide() {
            showing = false;
            placement = null;
            if (ad != null) ad.Hide();
        }

        public override void Destroy() {
            showing = false;            
            placement = null;
            CleanAd();
        }

        private void CleanAd() {
            if (ad != null) {
                ad.Destroy();
                ad = null;
            }
        }

        private void OnNativeOverlayAdLoadCallback(NativeOverlayAd ad, LoadAdError error) {
            if (error != null || ad == null) {
                OnAdFailedToLoad(error);
                return;
            }
            else OnNativeAdLoaded(ad);
        }

        private void OnNativeAdLoaded(NativeOverlayAd ad) {
            this.ad = ad;
            this.ad.OnAdPaid += OnAdPaid;
            this.ad.OnAdClicked += OnAdClicked;
            this.ad.OnAdFullScreenContentOpened += OnAdFullScreenContentOpened;
            this.ad.OnAdFullScreenContentClosed += OnAdFullScreenContentClosed;

            base.OnAdLoaded();
            if (!showing && !string.IsNullOrEmpty(placement)) {
                Log("Show from OnAdLoaded.");
                showing = true;
                Show(placement);
            }
        }
    }
}

#endif