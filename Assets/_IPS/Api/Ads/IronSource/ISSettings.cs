
using UnityEngine;

namespace IPS.Api.Ads {

    [CreateAssetMenu(fileName = "ISSettings", menuName = "IPS/Api/IronSourceSettings")]
    public class ISSettings : ScriptableObject, IAdSettings {
        public bool UseVersion770 = false;
        [Tooltip("Can setup from RemoteConfig by key 'ad_log_enable'")]
        public bool EnableLog => AdmobSettings.Instance.EnableLog;
        [Tooltip("if enableTest, you no need to fill any id. You can also setup from RemoteConfig by key 'ad_test_enable'")]
        public bool UseTestAd => AdmobSettings.Instance.UseTestAd;
        [Tooltip("if not enableTest, this device can be test real ads without `No Fill` message like other device.")]
        public string TestDevice_android = "";
        public string TestDevice_iOS = "";

        [Header("App id")]
        [SerializeField] string appKey_android;
        [SerializeField] string appKey_iOS;

        [Header("Banner")]
        [SerializeField] bool useBannerAd = true;
        [SerializeField] bool useBannerAdaptive = true;
        [SerializeField] [Tooltip("Set to false if you want to show banner on top")] bool showBannerOnBottom = true;

        [Header("MREC")]
        [Tooltip("DO NOT USE BOTH Banner & MREC in the same Mediation")]
        [SerializeField] bool useMRecAd = false;
        public bool ShowMRecOnBottom = true;
        [Tooltip("The padding y in dp from top/bottom screen to the top/bottom of the MRec. The (x, y) anchor coordinate of the MREC relative to the top left corner of the screen. If y=0 then use ad-view position below by default")]
        [HideInInspector] public uint MrecPaddingY_Dp { get; set; } = 60;

        [Header("Interstitial")]
        [SerializeField] bool useInterstitialAd = true;

        [Header("RewardVideo")]
        [SerializeField] bool useRewardVideoAd = true;
        
        public string AppID_Android => UseTestAd ? "85460dcd" : appKey_android;
        public string AppID_iOS => UseTestAd ? "85460dcd" : appKey_iOS;
        
        public string AppID {
            get {
#if UNITY_ANDROID
                return AppID_Android;
#else
                return AppID_iOS;
#endif
            }
        }

        public bool UseMRecAd {
#if IS
            get => useMRecAd;
#else
            get => false;
#endif
            set => useMRecAd = value;
        }

        public bool ShowBannerOnBottom => showBannerOnBottom;
        public bool AutoRefreshBanner => true;

        public bool UseIS => !string.IsNullOrEmpty(appKey_android) || !string.IsNullOrEmpty(appKey_iOS) || useBannerAd || useInterstitialAd || useMRecAd || useRewardVideoAd;
        public bool UseBannerAdaptive {
            get => useBannerAdaptive;
            set {
                if (value == useBannerAdaptive) return;
                useBannerAdaptive = value;
#if IS
                if (ISMediation.Initialized) ISMediation.Instance.SetBannerAdaptive();
#endif
            }
        }

         #region Interface
        public bool UseAOAAd {
            get => false;
            set { }
        }

        public bool UseNativeAd {
            get => false;
            set { }
        }

        public bool UseInterstitialAd {
#if IS
            get => useInterstitialAd;
#else
            get => false;
#endif
            set => useInterstitialAd = value;
        }

        public bool UseRewardedVideoAd {
#if IS
            get => useRewardVideoAd;
#else
            get => false;
#endif
            set => useRewardVideoAd = value;
        }
        public bool UseRewardedInterstitialAd {
            get => false;
            set { }
        }

        public bool UseBannerAd {
#if IS
            get => useBannerAd;
#else
            get => false;
#endif
            set => useBannerAd = value;
        }

        public string BannerID => "Banner";
        public string InterstitialID => "Interstitial";
        public string MRecID => "MRec";

#endregion
        #region Singleton
        private static ISSettings _instance;
        public static ISSettings Instance {
            get {
                if (_instance != null) return _instance;
                _instance = Resources.Load<ISSettings>($"{typeof(ISSettings).Name}/{typeof(ISSettings).Name}");
                if (_instance == null) {
                    Debug.LogError($"[Ads] ISSettings file not found at Resources/{typeof(ISSettings).Name}/{typeof(ISSettings).Name}");
                }
                return _instance;
            }
        }
        #endregion

    }
}