using System;
using UnityEngine;

#if ADMOB
using GoogleMobileAds.Api;
#endif
using System.Collections.Generic;

namespace IPS.Api.Ads {
    /// <summary>
    /// Put this into the first scene
    /// </summary>
    public partial class AdmobMediation : SingletonBehaviourDontDestroy<AdmobMediation>, IMediation {
        private BannerAdmob banner;
        private MRecAdmob mrec;
        private InterstitialAdmob interstitial;
        private RewardedInterstitialAdmob rewardInter;
        private RewardedVideoAdmob rewardVideo;
        private AOAAdmob aoa;

#if ADMOB_NATIVE
        private NativeAdmob native;
#elif ADMOB_NATIVE_UI
        private NativeAdmob_UI native;
#endif

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
#if ADMOB
        public static event Action<AdValue, AdSlotFormat, string> onAdPaid;
#endif

        private bool sdkInitCompleted;
        private Action callOnAvailable;

        private float lastCollapsibleShownTime;
        private bool CollapsibleIntervalReady => Time.time - lastCollapsibleShownTime >= AdmobSettings.Instance.CollapsibleReloadInterval;
        public bool IsShowingMRec => mrec != null && mrec.IsShowing;

        protected override void OnAwake() {
            if (AdmobSettings.Instance.EnableUMP) {
                SendConsentRequest();
            }
            else StartRequestAds(); // Delay 2 seconds for fetch remote config first
        }

        partial void StartRequestAds() {
#if ADMOB
            AdmobSettings config = AdmobSettings.Instance;           

            Debug.Log($"[Ads.Admob] SDK Initializing useTest={config.UseTestAd}");
            Logs.Log($"[Ads.Admob] Start Initializing: appID = {config.AppID}, useAOA={config.UseAOAAd}, useNative={config.UseNativeAd}, useBanner={config.UseBannerAd}, useInterstitial={config.UseInterstitialAd}, useRewardVideo={config.UseRewardedVideoAd}");

            List<string> testDevices = new List<string>();
            testDevices.Add(AdRequest.TestDeviceSimulator);
            if (config.TestDevices != null) {
                foreach(var i in config.TestDevices) {
                    testDevices.Add(i);
                }
            }

            RequestConfiguration requestConfiguration = new RequestConfiguration { TestDeviceIds = testDevices };
            MobileAds.SetRequestConfiguration(requestConfiguration);

            MobileAds.SetiOSAppPauseOnBackground(true);
            MobileAds.RaiseAdEventsOnUnityMainThread = true;

#if UNITY_EDITOR
            Invoke(nameof(OnInitialize), 3);
            return;
#endif
            MobileAds.Initialize((status) => {
                Debug.Log($"[Ads.Admob] SDK Initialized.");
                Dictionary<string, AdapterStatus> map = status.getAdapterStatusMap();

                foreach (KeyValuePair<string, AdapterStatus> keyValuePair in map) {
                    if (keyValuePair.Value.InitializationState == AdapterState.Ready) {
                        Debug.Log($"[Ads.Admob] Adapter: {keyValuePair.Key} initialized.");
                    }
                    else {
                        Debug.LogError($"[Ads.Admob] Adapter: {keyValuePair.Key} not ready.");
                    }
                }

                OnInitialize();                
            });
#endif
        }

        private void OnInitialize() {
            var config = AdmobSettings.Instance;
            Excutor.Schedule(() => {
                InitAOA(config);
                InitNative(config);
                InitInterstitialAd(config);
                InitRewardedVideoAd(config);
                InitRewardedInterstitialAd(config);
                InitBannerAd(config);
                InitMRecAd(config);

                callOnAvailable?.Invoke();
                callOnAvailable = null;
                sdkInitCompleted = true;
            });
        }

        /// <summary>
        /// Use for buy Remove Ads only
        /// </summary>
        public void OnRemoveAds() {
            AdmobSettings.Instance.UseBannerAd = false;
            AdmobSettings.Instance.UseInterstitialAd = false;
            AdmobSettings.Instance.UseAOAAd = false;
            AdmobSettings.Instance.UseNativeAd = false;
            AdmobSettings.Instance.UseMRecAd = false;

            if (interstitial != null) interstitial.AutoRequest = false;
            DestroyInterstitial();
            interstitial = null;

            if (banner != null) banner.AutoRequest = false;
            DestroyBanner();
            banner = null;

#if ADMOB_NATIVE
            if (native != null) native.AutoRequest = false;
            DestroyNative();
            native = null;
#endif

            if (mrec != null) mrec.AutoRequest = false;
            DestroyMRec();
            mrec = null;

            if (aoa != null) aoa.AutoRequest = false;
            aoa?.Destroy();
            aoa = null;
        }
        
        public void SetLogEnable(bool logEnable) {
            mrec?.SetLogEnable(logEnable);
            banner?.SetLogEnable(logEnable);
            interstitial?.SetLogEnable(logEnable);
            rewardVideo?.SetLogEnable(logEnable);
            rewardInter?.SetLogEnable(logEnable);
            aoa?.SetLogEnable(logEnable);

#if ADMOB_NATIVE
            native?.SetLogEnable(logEnable);
#endif
        }

#if ADMOB
        private void OnAdPaidCallback(AdValue adValue, AdSlotFormat adType, string placement) {
            onAdPaid?.Invoke(adValue, adType, placement);
        }

        private void OnAdFailedToLoad(AdSlotFormat adType, string msg) {
            onAdFailedToLoad?.Invoke(adType, msg);
        }

        private void OnAdFailedToShow(AdSlotFormat adType, string msg) {
            onAdFailedToShow?.Invoke(adType, msg);
        }
#endif
#region AOA
        private void InitAOA(AdmobSettings config) {
#if ADMOB
            if (!config.UseAOAAd || aoa != null) return;
            if ((!config.AoaOpenEnable || (UserData.FirstInstall && !config.AoaOpenFirstInstallEnable)) && !config.AoaResumeEnable) return;

            aoa = new AOAAdmob(config.AOAId, config.EnableLog, OnAOALoadedCallback);
            aoa.onAdFailedToLoad += OnAdFailedToLoad;
            aoa.onAdFailedToShow += OnAdFailedToShow;
            aoa.onAdDisplayed += OnAOADisplayedCallback;
            aoa.onAdPaid += OnAdPaidCallback;
#endif
        }

        private void OnAOALoadedCallback() {
            onAOALoaded?.Invoke();
        }

        private void OnAOADisplayedCallback() {
            onAOADisplayed?.Invoke(Mediation.admob);
        }

        public bool HasAOA => aoa != null && aoa.IsAvailable;
        public void ShowAOA(string placement, Action callback) {
            aoa?.Show(placement, callback);
        }

        public void DestroyAOA() {
            aoa?.Destroy();
        }
        #endregion AOA

        #region Native
        private string currentNativePlacement;

        partial void InitNative(AdmobSettings config);
        partial void InitNative(AdmobSettings config) {
#if ADMOB_NATIVE || ADMOB_NATIVE_UI
            if (!config.UseNativeAd || native != null) return;
#if ADMOB_NATIVE
            native = new NativeAdmob(config.NativeId, config.PreloadNativeAd, config.EnableLog);
#elif ADMOB_NATIVE_UI
            native = new NativeAdmob_UI(config.NativeId, config.PreloadNativeAd, config.EnableLog);
#endif
            native.onAdLoaded += OnNativeLoadedCallback;
            native.onAdFailedToLoad += OnAdFailedToLoad;
            native.onAdFailedToShow += OnAdFailedToShow;
            native.onAdDisplayed += OnNativeDisplayed;
            native.onAdPaid += OnAdPaidCallback;
            native.onAdClicked += OnAdClickedCallback;
#endif
        }

#if ADMOB_NATIVE || ADMOB_NATIVE_UI
        private void OnNativeLoadedCallback() {
            Tracking.Instance.LogAdNativeAvailable();
        }

        private void OnNativeDisplayed() {
            Tracking.Instance.LogAdNativeDisplayed(currentNativePlacement);        
        }

        public bool HasNative => native != null && native.IsAvailable;

#if ADMOB_NATIVE_UI
        public void ShowNative(string placement) {
            if (native == null) return;
            this.currentNativePlacement = placement;
            Logs.Log($"[Ads.Admob] Show Native UI triggered placement={placement}");
            native.Show(placement);
        }
#else
        public void ShowNative(string placement, Action<NativeAd> onSuccess) {
            if (native == null) return;
            this.currentNativePlacement = placement;
            Logs.Log($"[Ads.Admob] Show Native triggered placement={placement}");
            native.Show(placement, onSuccess);
        }
#endif

        public void HideNative() {
            native?.Hide();
        }

        public void DestroyNative() {
            if (native == null) return;
            Logs.Log("[Ads.Admob] Destroy Native triggered.");
            native.Destroy();
        }
#endif

#endregion Native

        #region BannerAd
        private void InitBannerAd(AdmobSettings config) {
#if ADMOB
            if (!config.UseBannerAd || banner != null) return;
#if !UNITY_EDITOR
            if (config.UseMRecAd) {
                Debug.LogError("DO NOT USE BOTH BANNER + MREC At the SAME Mediation.");
                //config.UseMRecAd = false;
            }
#endif
            lastCollapsibleShownTime = -1000;
            banner = new BannerAdmob(config.BannerID, config.BannerSize, useCollapsible: config.UseBannerCollapsible, autoRefreshCollapsible: config.CollapsibleAutoRefresh,
                       showOnBottom: config.ShowBannerOnBottom, enableLog: config.EnableLog, OnBannerChanged);
            banner.onAdPaid += OnAdPaidCallback;
            banner.onAdClicked += OnAdClickedCallback;
            banner.onNewRequest += OnBannerNewRequest;
            banner.onAdLoaded += OnBannerDisplayCallback;
            banner.onAdFailedToLoad += OnAdFailedToLoad;
#endif
        }
        private void OnBannerNewRequest(bool closeAd, string placement) {
            onBannerNewRequest?.Invoke(Mediation.admob, placement);
        }

        public void SetCollapsibleBannerAutoRefresh(bool value) {
            banner?.SetCollapAutoRefresh(value);
        }

        public void SetBannerAutoRequest(bool autoRequest) {
            if (banner != null) banner.AutoRequest = autoRequest;
            else if (!sdkInitCompleted) callOnAvailable += () => { SetBannerAutoRequest(autoRequest); };
        }

        private void OnAdClickedCallback() {
            onAdClicked?.Invoke();
        }

        private void OnBannerChanged(float height) {
            onBannerChanged?.Invoke(Mediation.admob, height);
            if (height > 0) lastCollapsibleShownTime = Time.time;
        }
        
        private void OnBannerDisplayCallback() {
            onBannerDisplayed?.Invoke(Mediation.admob);
        }

        public void ShowBanner(string placement) {
            if (banner != null) {
                if (AdmobSettings.Instance.UseBannerCollapsible) {
                    banner.Show(placement, CollapsibleIntervalReady);
                }
                else {
                    banner.Show(placement);
                }
            }
            else if (!sdkInitCompleted && AdmobSettings.Instance.UseBannerAd) callOnAvailable += () => ShowBanner(placement);
        }

        public void ShowBanner(string placement, bool loadCollapsible) {
            if (banner != null) {
                if (loadCollapsible && AdmobSettings.Instance.UseBannerCollapsible) {
                    banner.Show(placement, CollapsibleIntervalReady);
                }
                else {
                    banner.Show(placement);
                }
            }
            else if (!sdkInitCompleted && AdmobSettings.Instance.UseBannerAd) callOnAvailable += () => ShowBanner(placement, loadCollapsible);
        }

        public void HideBanner() {
            banner?.Hide();
        }

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
        float cacheYPadding;
        Vector2Int? cacheSize;

        private void InitMRecAd(AdmobSettings config) {
#if ADMOB
            if (!config.UseMRecAd || mrec != null) return;
            config.MrecPaddingY_Dp = config.ShowMRecOnBottom ? (uint)BannerHeightEstimateDp + 10 : 0;
            cacheYPadding = config.MrecPaddingY_Dp;
            mrec = new MRecAdmob(config.MRecID, config.ShowMRecOnBottom, enableLog: config.EnableLog);
            mrec.onAdFailedToLoad += OnAdFailedToLoad;
            mrec.onAdDisplayed += OnMRecDisplayedCallback;
            mrec.onAdPaid += OnAdPaidCallback;
            mrec.onAdClicked += OnAdClickedCallback;
            mrec.onNewRequest += OnMRecNewRequest;
#endif
        }
        private void OnMRecNewRequest(bool closeAd, string placement) {
            onMRecNewRequest?.Invoke(Mediation.admob, placement);
        }

        private void OnMRecDisplayedCallback() {
            onMRecDisplayed?.Invoke(Mediation.admob);
        }

        public void SetMRecAutoRequest(bool autoRequest) {
            if (mrec != null) mrec.AutoRequest = autoRequest;
            else if (!sdkInitCompleted) callOnAvailable += () => { SetMRecAutoRequest(autoRequest); };
        }

        public void ShowMRec(string placement) {
            ShowMRec(placement, cacheYPadding, cacheSize);
        }

        /// <summary>
        /// Follow ScreenAnchor: TopLeft (0,0), BottomRight (Screen.width, Screen.height). 
        /// <para>The top-left corner of the BannerView is positioned at the x and y values passed to the constructor, where the origin is the top-left of the screen.</para>
        /// </summary>
        /// <param name="yPadding">The padding y in pixel from top/bottom screen to the top/bottom of the MRec. Follow ScreenAnchor: TopLeft (0,0), BottomRight (Screen.width, Screen.height). Ad anchor also is Top-Left</param>
        /// <param name="size">Default null will use MREC = 300x250 (pt in iOS, dp in Android)</param>
        public void ShowMRec(string placement, float yPadding, Vector2Int? size) {
            cacheYPadding = yPadding;
            cacheSize = size;
            mrec?.Show(placement, yPadding, size);
        }

        public void HideMRec() {
            mrec?.Hide();
        }

        /// <summary>
        /// Use for buy Remove Ads only
        /// </summary>
        public void DestroyMRec() {
            mrec?.Destroy();
            //banner = null;
        }

        public Vector2 MRecSize => mrec != null ? mrec.Size : default;
        public Vector2 MRecSizeEstimate => mrec != null ? mrec.SizeEstimate : default;
    #endregion MRecAd

#region InterstitialAd
        private void InitInterstitialAd(AdmobSettings config) {
#if ADMOB
            if (!config.UseInterstitialAd || interstitial != null) return;
            interstitial = new InterstitialAdmob(config.InterstitialID, config.EnableLog, OnInterstitialLoadedCallback);
            interstitial.onAdFailedToLoad += OnInterFailedToLoad;
            interstitial.onAdFailedToShow += OnAdFailedToShow;
            interstitial.onAdDisplayed += OnInterDisplayedCallback;
            interstitial.onAdClicked += OnInterClickedCallback;
            interstitial.onAdPaid += OnAdPaidCallback;
            interstitial.onAdSuccess += OnInterClosedCallback;
            interstitial.onNewRequest += OnInterNewRequest;
#endif
        }

        private void OnInterNewRequest(bool closeAd, string placement) {
            onInterNewRequest?.Invoke(Mediation.admob, placement);
        }

        private void OnInterClickedCallback() {
            onInterClicked?.Invoke();
        }

        private void OnInterstitialLoadedCallback() {
            onInterLoaded?.Invoke();
        }
        
        private void OnInterDisplayedCallback() {
            onInterDisplayed?.Invoke(Mediation.admob);
        }

        private void OnInterClosedCallback() {
            onInterClosed?.Invoke(Mediation.admob);
        }
        
        private void OnInterFailedToLoad(AdSlotFormat adType, string msg) {
            onInterFailedToLoad?.Invoke(adType, msg);
        }

        public bool HasInterstitial => interstitial != null ? interstitial.IsAvailable : false;

        public void SetInterAutoRequest(bool value) {
            if (interstitial != null) {
                interstitial.AutoRequest = value;
                if (HasInterstitial) return;
            }
            else if (!sdkInitCompleted) callOnAvailable += () => { SetInterAutoRequest(value); };
        }

        public void RequestInterstitial() {
            if (interstitial != null) {
                Logs.Log($"[Ads.Admob] RequestInterstitial triggered");
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
        private void InitRewardedVideoAd(AdmobSettings config) {
#if ADMOB
            if (!config.UseRewardedVideoAd || rewardVideo != null) return;
            rewardVideo = new RewardedVideoAdmob(config.RewardVideoID, config.EnableLog, OnRewardedVideoAdLoaded);
            rewardVideo.onAdFailedToLoad += OnRewardVideoFailedToLoad;
            rewardVideo.onAdFailedToShow += OnAdFailedToShow;
            rewardVideo.onAdDisplayed += OnRewardVideoDisplayedCallback;
            rewardVideo.onAdPaid += OnAdPaidCallback;
            rewardVideo.onNewRequest += OnRewardVideoNewRequest;
#endif
        }

        private void OnRewardVideoNewRequest(bool closeAd, string placement) {
            onRewardVideoNewRequest?.Invoke(Mediation.admob, placement);
        }

        private void OnRewardedVideoAdLoaded() {
            onRewardVideoLoaded?.Invoke();
        }

        private void OnRewardVideoDisplayedCallback() {
            onRewardVideoDisplayed?.Invoke(Mediation.admob);
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
                Logs.Log($"[Ads.Admob] RequestRewardVideo triggered");
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

        
#region RewardedInterstitialAd
        private void InitRewardedInterstitialAd(AdmobSettings config) {
#if ADMOB
            if (!config.UseRewardedInterstitialAd || rewardInter != null) return;
            rewardInter = new RewardedInterstitialAdmob(config.RewardInterId, config.EnableLog, OnRewardedInterAdLoaded);
            rewardInter.onAdFailedToLoad += OnRewardInterFailedToLoad;
            rewardInter.onAdFailedToShow += OnAdFailedToShow;
            rewardInter.onAdDisplayed += OnRewardInterDisplayedCallback;
            rewardInter.onAdPaid += OnAdPaidCallback;
#endif
        }

        private void OnRewardedInterAdLoaded() {
            onRewardInterLoaded?.Invoke();
        }

        private void OnRewardInterDisplayedCallback() {
            onRewardInterDisplayed?.Invoke(Mediation.admob);
        }
        
        private void OnRewardInterFailedToLoad(AdSlotFormat adType, string msg) {
            onRewardInterFailedToLoad?.Invoke(adType, msg);
        }

        public void SetRewardInterAutoRequest(bool value) {
            if (rewardInter != null) {
                rewardInter.AutoRequest = value;
                if (HasRewardInterstitial) return;
            }
            else if (!sdkInitCompleted) callOnAvailable += () => { SetRewardInterAutoRequest(value); };
        }

        public bool HasRewardInterstitial => rewardInter != null ? rewardInter.IsAvailable : false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onGetRewardSuccess"></param>
        /// <param name="onAdClosed">this will alway be call although watch success or failed.</param>
        public void ShowRewardInterstitial(string placement, Action onGetRewardSuccess, Action onAdClosed = null) {
            rewardInter?.Show(placement, onGetRewardSuccess, onAdClosed);
        }

        public void HideRewardInterstitial() {
            rewardInter?.Hide();
        }

        public void DestroyRewardInterstitial() {
            rewardInter?.Destroy();
        }
#endregion RewardedVideoAd
                
    }
}