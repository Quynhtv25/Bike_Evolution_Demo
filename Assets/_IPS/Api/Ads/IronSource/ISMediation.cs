#if ADS && IS
using System;
using UnityEngine;

namespace IPS.Api.Ads {
    public partial class ISMediation : SingletonBehaviourDontDestroy<ISMediation>, IMediation {
        private BannerIS banner;
        private MRecIS mrec;
        private InterstitialIS interstitial;
        private RewardedVideoIS rewardVideo;

        public static event Action<Mediation, float> onBannerChanged;
        public static event Action<Mediation, string> onBannerNewRequest;
        public static event Action<Mediation> onBannerDisplayed;
        public static event Action onAdClicked;

        public static event Action<Mediation> onMRecDisplayed;
        public static event Action<Mediation, string> onMRecNewRequest;

        public static event Action onRewardVideoLoaded;
        public static event Action<Mediation> onRewardVideoDisplayed;
        public static event Action<AdSlotFormat, string> onRewardFailedToLoad;
        public static event Action<Mediation, string> onRewardVideoNewRequest;

        public static event Action onInterLoaded;
        public static event Action<Mediation> onInterDisplayed, onInterClosed;
        public static event Action<AdSlotFormat, string> onInterFailedToLoad;
        public static event Action<Mediation, string> onInterNewRequest;

        public static event Action<AdSlotFormat, string> onAdFailedToLoad;
        public static event Action<AdSlotFormat, string> onAdFailedToShow;
        public static event Action<IronSourceAdInfo, AdSlotFormat, string> onAdPaid;

        private bool sdkInitCompleted;
        private Action callOnAvailable;
                
        public bool IsShowingMRec => mrec != null && mrec.IsShowing;

        protected override void OnAwake() {
            ISSettings config = ISSettings.Instance;           

            Debug.Log($"[Ads.IS] SDK Initializing log={config.EnableLog}");
            Logs.Log($"[Ads.IS] Start Initializing: appID = {config.AppID}, useBanner={config.UseBannerAd}, useInterstitial={config.UseInterstitialAd}, useRewardVideo={config.UseRewardedVideoAd}");

            IronSourceEvents.onSdkInitializationCompletedEvent += OnInitCompleted;

            IronSource.Agent.validateIntegration();

            IronSource.Agent.setAdaptersDebug(IPSConfig.CheatEnable);
            IronSource.Agent.setConsent(UserData.GDPRConsentStatus);
            IronSource.Agent.setMetaData("do_not_sell", "false");
            IronSource.Agent.setMetaData("is_child_directed", "false");

            if (IPSConfig.CheatEnable || IPSConfig.LogEnable) {
                IronSource.Agent.setMetaData("is_test_suite", "enable");
            }

            IronSource.Agent.init(ISSettings.Instance.AppID);

            var adQuality = new AdQualityIronSource();
            adQuality.Init(ISSettings.Instance.AppID, IPSConfig.CheatEnable);

#if UNITY_EDITOR
            Invoke(nameof(OnInitCompleted), 3);
#endif
        }

        public void ShowDebugger() {
            if (sdkInitCompleted) {
                IronSource.Agent.launchTestSuite();
            }
        }

        private void OnInitCompleted() {
            if (sdkInitCompleted) return;

            Debug.Log("IronSource SDK Initialized");
            //BigoAdSdk.SetUserConsent(ConsentOptions.GDPR, true);
            //BigoAdSdk.SetUserConsent(ConsentOptions.CCPA, true);

            Excutor.Schedule(() => {
                ISSettings config = ISSettings.Instance;
                InitInterstitialAd(config);
                InitRewardedVideoAd(config);
                InitBannerAd(config);
                InitMRecAd(config);

                callOnAvailable?.Invoke();
                callOnAvailable = null;
                sdkInitCompleted = true;
            });
        }

        public void OnPause(bool isPaused) {
            IronSource.Agent.onApplicationPause(isPaused);
        }

        partial void FetchRemoteConfig(IAdSettings settings);
        
        public void SetLogEnable(bool logEnable) {
            mrec?.SetLogEnable(logEnable);
            banner?.SetLogEnable(logEnable);
            interstitial?.SetLogEnable(logEnable);
            rewardVideo?.SetLogEnable(logEnable);
        }

        private void OnAdPaidCallback(IronSourceAdInfo adInfo, AdSlotFormat adType, string placement) {
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
            ISSettings.Instance.UseBannerAd = false;
            ISSettings.Instance.UseInterstitialAd = false;
            ISSettings.Instance.UseAOAAd = false;
            ISSettings.Instance.UseNativeAd = false;
            ISSettings.Instance.UseMRecAd = false;

            if (interstitial != null) interstitial.AutoRequest = false;
            DestroyInterstitial();
            interstitial = null;

            if (banner != null) banner.AutoRequest = false;
            DestroyBanner();
            banner = null;

            if (mrec != null) mrec.AutoRequest = false;
            DestroyMRec();
            mrec = null;
        }

        #region BannerAd
        private void InitBannerAd(ISSettings config) {
            if (!config.UseBannerAd || banner != null) return;
            if (config.UseMRecAd) {
                Debug.LogError("DO NOT USE BOTH BANNER + MREC At the SAME Mediation. Using Banner so turn off MRec in this case.");
                //config.UseMRecAd = false;
            }
            banner = new BannerIS(config.UseBannerAdaptive, config.ShowBannerOnBottom, config.EnableLog, OnBannerChanged);
            banner.onAdFailedToLoad += OnAdFailedToLoad;
            banner.onAdPaid += OnAdPaidCallback;
            banner.onAdLoaded += OnBannerDisplayCallback;
            banner.onAdClicked += OnAdClickedCallback;
            banner.onNewRequest += OnBannerNewRequest;
        }

        private void OnBannerNewRequest(bool closeAd, string placement) {
            onBannerNewRequest?.Invoke(Mediation.ironSource, placement);
        }
        
        private void OnBannerDisplayCallback() {
            onBannerDisplayed?.Invoke(Mediation.ironSource);
        }

        private void OnAdClickedCallback() {
            onAdClicked?.Invoke();
        }

        private void OnBannerChanged(float height) {
            onBannerChanged?.Invoke(Mediation.ironSource, height);
        }

        public void SetBannerAutoRequest(bool value) {
            if (banner != null) banner.AutoRequest = value;
            else if (!sdkInitCompleted) callOnAvailable += () => { SetBannerAutoRequest(value); };
        }
        
        public void SetBannerAdaptive() {
            if (banner != null) banner.SetAdaptive(MaxSettings.Instance.UseBannerAdaptive);
        }

        public void ShowBanner(string placement) {
            if (banner != null) {
                banner.Show(placement);
            }
            else if (!sdkInitCompleted && ISSettings.Instance.UseBannerAd) callOnAvailable += () => ShowBanner(placement);
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
        private void InitMRecAd(ISSettings config) {
            if (!config.UseMRecAd || mrec != null) return;
            config.MrecPaddingY_Dp = config.ShowMRecOnBottom ? (uint)BannerHeightEstimateDp + 10 : 0;
            mrec = new MRecIS(config.ShowMRecOnBottom, config.EnableLog);
            mrec.onAdFailedToLoad += OnAdFailedToLoad;
            mrec.onAdPaid += OnAdPaidCallback;
            mrec.onAdDisplayed += OnMRecDisplayed;
            mrec.onAdClicked += OnAdClickedCallback;
            mrec.onNewRequest += OnMRecNewRequest;
        }

        private void OnMRecNewRequest(bool closeAd, string placement) {
            onMRecNewRequest?.Invoke(closeAd ? Mediation.admob : Mediation.ironSource, placement); // if closeAd=true, fake mediation=admob to continue request of this mediation
        }

        private void OnMRecDisplayed() {
            onMRecDisplayed?.Invoke(Mediation.ironSource);
        }
        public void SetMRecAutoRequest(bool value) {
            if (mrec != null) mrec.AutoRequest = value;
            else if (!sdkInitCompleted) callOnAvailable += () => { SetMRecAutoRequest(value); };
        }

        public void ShowMRec(string placement) {
            if (mrec != null) {
                mrec.Show(placement);
            }
            else if (!sdkInitCompleted && ISSettings.Instance.UseMRecAd) callOnAvailable += () => ShowMRec(placement);
        }

        public void HideMrec() {
            mrec?.Hide();
        }

        /// <summary>
        /// Use for buy Remove Ads only
        /// </summary>
        public void DestroyMRec() {
            mrec?.Destroy();
        }

        public float MRecHeight => mrec != null ? mrec.Height : 0;
        public Vector2 MRecSize => mrec != null ? mrec.Size : default;
        public Vector2 MRecSizeEstimate => mrec != null ? mrec.SizeEstimate : default;
        #endregion MRecAd

        #region InterstitialAd
        private void InitInterstitialAd(ISSettings config) {
            if (!config.UseInterstitialAd || interstitial != null) return;
            interstitial = new InterstitialIS(config.EnableLog, OnInterstitialLoadedCallback);
            interstitial.onAdFailedToLoad += OnInterFailedToLoad;
            interstitial.onAdFailedToShow += OnAdFailedToShow;
            interstitial.onAdDisplayed += OnInterDisplayedCallback;
            interstitial.onAdSuccess += OnInterClosedCallback;
            interstitial.onAdPaid += OnAdPaidCallback;
            interstitial.onNewRequest += OnInterNewRequest;
        }

        private void OnInterNewRequest(bool closeAd, string placement) {
            onInterNewRequest?.Invoke(closeAd ? Mediation.admob : Mediation.ironSource, placement); // if closeAd=true, fake mediation=admob to continue request of this mediation
        }

        private void OnInterstitialLoadedCallback() {
            onInterLoaded?.Invoke();
        }

        private void OnInterDisplayedCallback() {
            onInterDisplayed?.Invoke(Mediation.ironSource);
        }

        private void OnInterClosedCallback() {
            onInterClosed?.Invoke(Mediation.ironSource);
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
                Logs.Log($"[Ads.IS] RequestInterstitial triggered");
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
        private void InitRewardedVideoAd(ISSettings config) {
            if (!config.UseRewardedVideoAd || rewardVideo != null) return;
            rewardVideo = new RewardedVideoIS(config.EnableLog, OnRewardedVideoAdLoaded);
            rewardVideo.onAdFailedToLoad += OnRewardFailedToLoad;
            rewardVideo.onAdFailedToShow += OnAdFailedToShow;
            rewardVideo.onAdDisplayed += OnRewardVideoDisplayedCallback;
            rewardVideo.onAdPaid += OnAdPaidCallback;
            rewardVideo.onNewRequest += OnRewardVideoNewRequest;
        }

        private void OnRewardVideoNewRequest(bool closeAd, string placement) {
            onRewardVideoNewRequest?.Invoke(closeAd ? Mediation.admob : Mediation.ironSource, placement); // if closeAd=true, fake mediation=admob to continue request of this mediation
        }

        private void OnRewardedVideoAdLoaded() {
            onRewardVideoLoaded?.Invoke();
        }
        
        private void OnRewardVideoDisplayedCallback() {
            onRewardVideoDisplayed?.Invoke(Mediation.ironSource);
        }        
         
        private void OnRewardFailedToLoad(AdSlotFormat adType, string msg) {
            onRewardFailedToLoad?.Invoke(adType, msg);
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
                Logs.Log($"[Ads.IS] RequestRewardVideo triggered");
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