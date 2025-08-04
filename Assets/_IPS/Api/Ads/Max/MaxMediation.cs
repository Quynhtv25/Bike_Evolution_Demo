#if ADS && MAX
using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace IPS.Api.Ads {
    public partial class MaxMediation : SingletonBehaviourDontDestroy<MaxMediation>, IMediation {
        private AOAMax aoa;
        private MRecMax mrec;
        private BannerMax banner;
        private InterstitialMax interstitial;
        private RewardedVideoMax rewardVideo;

        public static event Action<Mediation, float> onBannerChanged;
        public static event Action<Mediation, string> onBannerNewRequest;
        public static event Action<Mediation> onBannerDisplayed;
        public static event Action onAdClicked;

        public static event Action<Mediation> onMRecDisplayed;
        public static event Action<Mediation, string> onMRecNewRequest;

        public static event Action onRewardInterLoaded;
        public static event Action<Mediation> onRewardInterDisplayed;
        public static event Action<AdSlotFormat, string> onRewardInterFailedToLoad;

        public static event Action onRewardVideoLoaded;
        public static event Action<Mediation> onRewardVideoDisplayed;
        public static event Action<AdSlotFormat, string> onRewardVideoFailedToLoad;
        public static event Action<Mediation, string> onRewardVideoNewRequest;

        public static event Action onInterLoaded;
        public static event Action<Mediation> onInterDisplayed, onInterClosed;
        public static event Action onInterClicked;
        public static event Action<AdSlotFormat, string> onInterFailedToLoad;
        public static event Action<Mediation, string> onInterNewRequest;

        public static event Action onAOALoaded;
        public static event Action<Mediation> onAOADisplayed;

        public static event Action<AdSlotFormat, string> onAdFailedToLoad;
        public static event Action<AdSlotFormat, string> onAdFailedToShow;
        public static event Action<MaxSdkBase.AdInfo, AdSlotFormat, string> onAdPaid;
        
        private bool sdkInitCompleted;
        private Action callOnAvailable;
        
        public bool IsShowingMRec => mrec != null && mrec.IsShowing;

        protected override void OnAwake() {
            MaxSettings config = MaxSettings.Instance;           

            Debug.Log($"[Ads.Max] SDK Initializing version={MaxSdk.Version} log={config.EnableLog}");
            Logs.Log($"[Ads.Max] Start Initializing: appID = {config.AppID}, aoa={config.AOAId}, banner={config.BannerID}, inter={config.InterstitialID}, rw={config.RewardID}");

            MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration => {
                // AppLovin SDK is initialized, configure and start loading ads.
                OnInitCompleted();
            };

            if (config.TestDevices != null) {
                MaxSdk.SetTestDeviceAdvertisingIdentifiers(config.TestDevices);
            }
            MaxSdk.SetVerboseLogging(config.EnableLog);
            MaxSdk.SetSdkKey(config.AppID);
            MaxSdk.InitializeSdk();

#if UNITY_EDITOR
            Invoke(nameof(OnInitCompleted), 3);
#endif
        }

        private void OnInitCompleted() {
            Debug.Log("[Ads.Max] MAX SDK Initialized");
            Excutor.Schedule(() => {
                MaxSettings config = MaxSettings.Instance;
                InitAppOpenAd(config);
                InitInterstitialAd(config);
                InitRewardedVideoAd(config);
                InitBannerAd(config);
                InitMRecAd(config);

                callOnAvailable?.Invoke();
                callOnAvailable = null;
                sdkInitCompleted = true;
            });
        }

        public void ShowDebugger() {
            if (sdkInitCompleted) {
                MaxSdk.ShowMediationDebugger();
            }
        }

        public void SetLogEnable(bool logEnable) {
            aoa?.SetLogEnable(logEnable);
            mrec?.SetLogEnable(logEnable);
            banner?.SetLogEnable(logEnable);
            interstitial?.SetLogEnable(logEnable);
            rewardVideo?.SetLogEnable(logEnable);
        }

        private void OnAdPaidCallback(MaxSdkBase.AdInfo adInfo, AdSlotFormat adType, string placement) {
            onAdPaid?.Invoke(adInfo, adType, placement);
        }

        private void OnAdFailedToLoad(AdSlotFormat adType, string error) {
            onAdFailedToLoad?.Invoke(adType, error);
        }
            
        private void OnAdFailedToShow(AdSlotFormat adType, string msg) {
            onAdFailedToShow?.Invoke(adType, msg);
        }

        /// <summary>
        /// Use for buy Remove Ads only
        /// </summary>
        public void OnRemoveAds() {
            MaxSettings.Instance.UseBannerAd = false;
            MaxSettings.Instance.UseInterstitialAd = false;
            MaxSettings.Instance.UseAOAAd = false;
            MaxSettings.Instance.UseNativeAd = false;
            MaxSettings.Instance.UseMRecAd = false;

            if (interstitial != null) interstitial.AutoRequest = false;
            DestroyInterstitial();
            interstitial = null;

            if (banner != null) banner.AutoRequest = false;
            DestroyBanner();
            banner = null;

            if (mrec != null) mrec.AutoRequest = false;
            DestroyMRec();
            mrec = null;

            if (aoa != null) aoa.AutoRequest = false;
            aoa?.Destroy();
            aoa = null;
        }

        #region AppOpen
        private void InitAppOpenAd(MaxSettings config) {
            if (!config.UseAOAAd || aoa != null) return;
            if ((!AdmobSettings.Instance.AoaOpenEnable || (UserData.FirstInstall && !AdmobSettings.Instance.AoaOpenFirstInstallEnable)) 
                && !AdmobSettings.Instance.AoaResumeEnable) return;

            aoa = new AOAMax(config.AOAId, config.EnableLog, OnAOALoadedCallback);
            aoa.onAdFailedToLoad += OnAdFailedToLoad;
            aoa.onAdFailedToShow += OnAdFailedToShow;
            aoa.onAdDisplayed += OnAOADisplayedCallback;
            aoa.onAdPaid += OnAdPaidCallback;
        }

        private void OnAOALoadedCallback() {
            onAOALoaded?.Invoke();
        }

        private void OnAOADisplayedCallback() {
            onAOADisplayed?.Invoke(Mediation.appLovin);
        }

        public bool HasAOA => aoa != null && aoa.IsAvailable;
        public void ShowAOA(string placement, Action callback) {
            aoa?.Show(placement, callback);
        }

        public void DestroyAOA() {
            aoa?.Destroy();
        }
        #endregion

        #region BannerAd
        private void InitBannerAd(MaxSettings config) {
            if (!config.UseBannerAd || banner != null) return;
            banner = new BannerMax(config.BannerID, config.UseBannerAdaptive, AdmobSettings.Instance.BannerReloadCapping, config.ShowBannerOnBottom, config.EnableLog, OnBannerChanged);
            banner.onAdFailedToLoad += OnAdFailedToLoad;
            banner.onAdPaid += OnAdPaidCallback;
            banner.onAdDisplayed += OnBannerDisplayCallback;
            banner.onAdClicked += OnAdClickedCallback;
            banner.onNewRequest += OnBannerNewRequest;
        }

        private void OnBannerNewRequest(bool closeAd, string placement) {
            onBannerNewRequest?.Invoke(Mediation.appLovin, placement);
        }
                
        private void OnBannerDisplayCallback() {
            onBannerDisplayed?.Invoke(Mediation.appLovin);
        }

        private void OnAdClickedCallback() {
            onAdClicked?.Invoke();
        }

        private void OnBannerChanged(float height) {
            onBannerChanged?.Invoke(Mediation.appLovin, height);
        }

        public void SetBannerExtraParam() {
            if (banner != null) banner.SetExtra(MaxSettings.Instance.UseBannerAdaptive, AdmobSettings.Instance.BannerReloadCapping);
        }
        
        public void SetBannerAutoRequest(bool value) {
            if (banner != null) banner.AutoRequest = value;
            else if (!sdkInitCompleted) callOnAvailable += () => { SetBannerAutoRequest(value); };
        }

        public void ShowBanner(string placement) {
            if (banner != null) {
                banner.Show(placement);
            }
            else if (!sdkInitCompleted && MaxSettings.Instance.UseBannerAd) callOnAvailable += () => ShowBanner(placement);
        }

        public void HideBanner() {
            banner?.Hide();
        }

        /// <summary>
        /// Use for buy Remove Ads only
        /// </summary>
        public void DestroyBanner() {
            banner?.Destroy();
            //banner = null;
        }

        /// <summary>
        /// The height in pixel of the banner, use for controll your UI follow up banner.
        /// <para>Example: when user buy removed ads, the bottom button should be move down than normal.</para>
        /// </summary>
        public float BannerHeight => banner != null ? banner.BannerHeight : 0;

        /// <summary> The estimate height in pixel of the banner, value is fixed event banner is showing or not </summary>
        public float BannerHeightEstimate => banner != null ? banner.BannerHeightEstimate : 0;

        public int BannerHeightEstimateDp => banner != null ? banner.BannerHeightEstimateDp : 0;
        #endregion BannerAd

        #region MRecAd
        private void InitMRecAd(MaxSettings config) {
            if (!config.UseMRecAd || mrec != null) return;
            if (config.UseMRecCustomPosition) {
                config.MrecPaddingY_Dp = config.ShowMRecOnBottom ? (uint)BannerHeightEstimateDp + 10 : 0;
                mrec = new MRecMax(config.MRecID, config.MrecPaddingY_Dp, config.ShowMRecOnBottom, config.EnableLog);
            }
            else {
                mrec = new MRecMax(config.MRecID, config.ShowMRecOnBottom, config.EnableLog);
            }
            mrec.onAdFailedToLoad += OnAdFailedToLoad;
            mrec.onAdPaid += OnAdPaidCallback;
            mrec.onAdDisplayed += OnMRecDisplayed;
            mrec.onAdClicked += OnAdClickedCallback;
            mrec.onNewRequest += OnMRecNewRequest;
        }

        private void OnMRecNewRequest(bool closeAd, string placement) {
            onMRecNewRequest?.Invoke(Mediation.appLovin, placement);
        }

        private void OnMRecDisplayed() {
            onMRecDisplayed?.Invoke(Mediation.appLovin);
        }
        
        public void SetMRecAutoRequest(bool value) {
            if (mrec != null) mrec.AutoRequest = value;
            else if (!sdkInitCompleted) callOnAvailable += () => { SetMRecAutoRequest(value); };
        }

        public void SetMRecPosition(float yPadding, bool destroyCurrent = false) {
            if (mrec != null) mrec.SetPaddingY(yPadding, false);
        }

        public void ShowMRec(string placement, float yPadding) {
            if (mrec != null) {
                if (yPadding > 0) mrec.SetPaddingY(yPadding, true);
                mrec.Show(placement);
            }
            else if (!sdkInitCompleted && MaxSettings.Instance.UseMRecAd) callOnAvailable += () => ShowMRec(placement, yPadding);
        }

        public void ShowMRec(string placement) {
            if (mrec != null) {
                mrec.Show(placement);
            }
            else if (!sdkInitCompleted && MaxSettings.Instance.UseMRecAd) callOnAvailable += () => ShowMRec(placement);
        }

        public void HideMRec() {
            mrec?.Hide();
        }

        /// <summary>
        /// Use for buy Remove Ads only
        /// </summary>
        public void DestroyMRec() {
            mrec?.Destroy();
        }
        public Vector2 MRecSize {
            get {
#if UNITY_EDITOR
                return mrec != null ? AdsExtension.DpToPixel(MRecMax.SizeDp.x, MRecMax.SizeDp.y) : default;
#endif
                return mrec != null ? mrec.Size : default;
            }

        }

        public Vector2 MRecSizeEstimate {
            get {
#if UNITY_EDITOR
                return AdsExtension.DpToPixel(MRecMax.SizeDp.x, MRecMax.SizeDp.y);
#endif
                return mrec != null ? mrec.SizeEstimate : default;
            }

        }
#endregion MRecAd

        #region InterstitialAd
        private void InitInterstitialAd(MaxSettings config) {
            if (!config.UseInterstitialAd || interstitial != null) return;
            interstitial = new InterstitialMax(config.InterstitialID, config.EnableLog, OnInterstitialLoadedCallback);
            interstitial.onAdFailedToLoad += OnInterFailedToLoad;
            interstitial.onAdFailedToShow += OnAdFailedToShow;
            interstitial.onAdDisplayed += OnInterDisplayedCallback;
            interstitial.onAdClicked += OnInterClickedCallback;
            interstitial.onAdPaid += OnAdPaidCallback;
            interstitial.onAdSuccess += OnInterClosedCallback;
            interstitial.onNewRequest += OnInterNewRequest;
        }

        private void OnInterNewRequest(bool closeAd, string placement) {
            onInterNewRequest?.Invoke(closeAd ? Mediation.admob : Mediation.appLovin, placement); // if closeAd=true, fake mediation=admob to continue request of this mediation
        }

        private void OnInterClickedCallback() {
            onInterClicked?.Invoke();
        }

        private void OnInterstitialLoadedCallback() {
            onInterLoaded?.Invoke();
        }

        private void OnInterDisplayedCallback() {
            onInterDisplayed?.Invoke(Mediation.appLovin);
        }
 
        private void OnInterClosedCallback() {
            onInterClosed?.Invoke(Mediation.appLovin);
        }
                        
        private void OnInterFailedToLoad(AdSlotFormat adType, string msg) {
            onInterFailedToLoad?.Invoke(adType, msg);
        }

        public void SetInterAutoRequest(bool value) {
            if (interstitial != null) {
                interstitial.AutoRequest = value;
                if (HasInterstitial) return;
            }
            else if (!sdkInitCompleted) callOnAvailable += () => { SetInterAutoRequest(value); };
        }

        public bool HasInterstitial => interstitial != null ? interstitial.IsAvailable : false;

        public void RequestInterstitial() {
            if (interstitial != null) {
                Logs.Log($"[Ads.Max] RequestInterstitial triggered");
                interstitial.Request();
            }
            else if (!sdkInitCompleted) callOnAvailable += RequestInterstitial;
        }

        public void ShowInterstitial(string placement, Action onAdClosed = null) {
            interstitial?.Show(placement, onAdClosed);
        }

        public void HideInterstitial() {
            interstitial?.Hide();
        }

        /// <summary>
        /// Use for buy Remove Ads only
        /// </summary>
        public void DestroyInterstitial() {
            interstitial?.Destroy();
        }
        #endregion InterstitialAd

        #region RewardedVideoAd
        private void InitRewardedVideoAd(MaxSettings config) {
            if (!config.UseRewardedVideoAd || rewardVideo != null) return;
            rewardVideo = new RewardedVideoMax(config.RewardID, config.EnableLog, OnRewardedVideoAdLoaded);
            rewardVideo.onAdFailedToLoad += OnRewardVideoFailedToLoad;
            rewardVideo.onAdFailedToShow += OnAdFailedToShow;
            rewardVideo.onAdDisplayed += OnRewardVideoDisplayedCallback;
            rewardVideo.onAdPaid += OnAdPaidCallback;
            rewardVideo.onNewRequest += OnRewardVideoNewRequest;
        }

        private void OnRewardVideoNewRequest(bool closeAd, string placement) {
            onRewardVideoNewRequest?.Invoke(closeAd ? Mediation.admob : Mediation.appLovin, placement); // if closeAd=true, fake mediation=admob to continue request of this mediation
        }

        private void OnRewardedVideoAdLoaded() {
            onRewardVideoLoaded?.Invoke();
        }
        
        private void OnRewardVideoDisplayedCallback() {
            onRewardVideoDisplayed?.Invoke(Mediation.appLovin);
        }        
         
        private void OnRewardVideoFailedToLoad(AdSlotFormat adType, string msg) {
            onRewardVideoFailedToLoad?.Invoke(adType, msg);
        }

        public void SetRewardVideoAutoRequest(bool value) {
            if (rewardVideo != null) {
                rewardVideo.AutoRequest = value;
                if (HasRewardVideo) return;
            }
            else if (!sdkInitCompleted) callOnAvailable += () => { SetRewardVideoAutoRequest(value); };
        }

        public bool HasRewardVideo => rewardVideo != null ? rewardVideo.IsAvailable : false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onGetRewardSuccess"></param>
        /// <param name="onAdClosed">this will alway be call although watch success or failed.</param>
        public void ShowRewardVideo(string placement, Action onGetRewardSuccess, Action onAdClosed = null) {
            rewardVideo?.Show(placement, onGetRewardSuccess, onAdClosed);
        }
        
        public void RequestRewardVideo() {
            if (rewardVideo != null) {
                Logs.Log($"[Ads.Max] RequestRewardVideo triggered");
                rewardVideo.Request();
            }
            else if (!sdkInitCompleted) callOnAvailable += RequestRewardVideo;
        }

        public void HideRewardVideo() {
            rewardVideo?.Hide();
        }

        public void DestroyRewardVideo() {
            rewardVideo?.Destroy();
        }
        #endregion RewardedVideoAd

    }
}
#endif