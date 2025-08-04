using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IPS.Api.Ads {

    [CreateAssetMenu(fileName = "MaxSettings", menuName = "IPS/Api/MaxSettings")]
    public class MaxSettings : ScriptableObject, IAdSettings {
        [Tooltip("Can setup from RemoteConfig by key 'ad_log_enable'")]
        public bool EnableLog => AdmobSettings.Instance.EnableLog;
        [Tooltip("if enableTest, you no need to fill any id. You can also setup from RemoteConfig by key 'ad_test_enable'")]
        public bool UseTestAd => AdmobSettings.Instance.UseTestAd;

        public string[] TestDevices => AdmobSettings.Instance.TestDevices;

        [Tooltip("if not enableTest, this device can be test real ads without `No Fill` message like other device.")]
        public string TestDevice_android = "";
        public string TestDevice_iOS = "";

        [Header("App id")]
        [SerializeField] string maxSDKKey;
                
        [Header("AOA Settings")]
        [SerializeField] bool useAOAAd = true;
        [SerializeField] string aoaTierId_Android;
        [SerializeField] string aoaTierId_iOS;

        [Header("Banner")]
        [SerializeField] bool useBannerAd = true;
        [SerializeField] string bannerID_android;
        [SerializeField] string bannerID_iOS;
        [SerializeField] bool useBannerAdaptive = true;
        [SerializeField] [Tooltip("Set to false if you want to show banner on top")] bool showBannerOnBottom = true;

        [Header("MRec")]
        [SerializeField] bool useMRecAd = true;
        [SerializeField] string mrecID_android;
        [SerializeField] string mrecID_iOS;
        public bool ShowMRecOnBottom = true;
        [Tooltip("The padding y in dp from top/bottom screen to the top/bottom of the MRec. The (x, y) anchor coordinate of the MREC relative to the top left corner of the screen. If y=0 then use ad-view position below by default")]
        [HideInInspector] public uint MrecPaddingY_Dp { get; set; } = 60;
        public bool UseMRecCustomPosition => ShowMRecOnBottom && MrecPaddingY_Dp > 0;

        [Header("Interstitial")]
        [SerializeField] bool useInterstitialAd = true;
        [SerializeField] string interstitialID_android;
        [SerializeField] string interstitialID_iOS;

        [Header("RewardVideo")]
        [SerializeField] bool useRewardVideoAd = true;
        [SerializeField] string rewardID_android;
        [SerializeField] string rewardID_iOS;

        //[Header("RewardInterstitial")]
        //[SerializeField] bool useRewardInterstitialAd = false;
        //[SerializeField] string rewardInterID_android;
        //[SerializeField] string rewardInterID_iOS;        
        public string AppID => maxSDKKey;

        public string RewardID {
            get {
                if (!useRewardVideoAd) return string.Empty;
                return IPSConfig.IsAndroid ? rewardID_android : rewardID_iOS;
            }
        }

        //public string RewardInterId {
        //    get {
        //        if (!useRewardInterstitialAd) return string.Empty;
        //        return IPSConfig.IsAndroid ? rewardInterID_android : rewardInterID_iOS;
        //    }
        //}

        public string InterstitialID {
            get {
                if (!useInterstitialAd) return string.Empty;
                return IPSConfig.IsAndroid ? interstitialID_android : interstitialID_iOS;
            }
        }

        public string BannerID {
            get {
                if (!useBannerAd) return string.Empty;
                return IPSConfig.IsAndroid ? bannerID_android : bannerID_iOS;
            }
        }

        public string MRecID {
            get {
                if (!useMRecAd) return string.Empty;
                return IPSConfig.IsAndroid ? mrecID_android : mrecID_iOS;
            }
        }

        public string AOAId {
            get {
                if (!useAOAAd) return string.Empty;
                return IPSConfig.IsAndroid ? aoaTierId_Android : aoaTierId_iOS;
            }
        }

        public bool ShowBannerOnBottom => showBannerOnBottom;

         #region Interface
        public bool UseAOAAd {
            get => useAOAAd && !string.IsNullOrEmpty(AOAId);
            set => useAOAAd = value;
        }

        public bool UseNativeAd {
            get => false;
            set { }
        }

        public bool UseInterstitialAd {
            get => useInterstitialAd && !string.IsNullOrEmpty(InterstitialID);
            set => useInterstitialAd = value;
        }

        public bool UseRewardedVideoAd {
            get => useRewardVideoAd && !string.IsNullOrEmpty(RewardID);
            set => useRewardVideoAd = value;
        }

        public bool UseRewardedInterstitialAd {
            get => false;
            set { }
        }

        public bool UseBannerAdaptive {
            get => useBannerAdaptive;
            set {
                if (value == useBannerAdaptive) return;
                useBannerAdaptive = value;
#if MAX
                if (MaxMediation.Initialized) MaxMediation.Instance.SetBannerExtraParam();
#endif
            }
        }

        public bool UseBannerAd {
            get => useBannerAd && !string.IsNullOrEmpty(BannerID);
            set => useBannerAd = value;
        }

        public bool UseMRecAd {
            get => useMRecAd && !string.IsNullOrEmpty(MRecID);
            set => useMRecAd = value;
        }
        #endregion
        #region Singleton
        private static MaxSettings _instance;
        public static MaxSettings Instance {
            get {
                if (_instance != null) return _instance;
                _instance = Resources.Load<MaxSettings>($"{typeof(MaxSettings).Name}/{typeof(MaxSettings).Name}");
                if (_instance == null) {
                    Debug.LogError($"[Ads] MaxSettings file not found at Resources/{typeof(MaxSettings).Name}/{typeof(MaxSettings).Name}");
                }
                return _instance;
            }
        }
        #endregion

    }
}