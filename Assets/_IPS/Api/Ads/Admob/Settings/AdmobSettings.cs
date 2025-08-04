using System.IO;
using UnityEngine;

namespace IPS.Api.Ads {
    public interface IAdSettings {
        public bool UseAOAAd { get; set; }
        public bool UseNativeAd { get; set; }
        public bool UseInterstitialAd { get; set; }
        public bool UseRewardedVideoAd { get; set; }
        public bool UseRewardedInterstitialAd { get; set; }
        public bool UseBannerAd { get; set; }
        public bool UseBannerAdaptive { get; set; }
        public string BannerID { get; }
        public bool UseMRecAd { get; set; }
        public string MRecID {get;}

        public string InterstitialID { get; }
    }

    [System.Serializable]
    public enum BannerSize {
        AdaptiveBanner,
        SmartBanner,
        Banner_320x50,
        //MRec_300x250, // Medium Rectangle
        IABBanner_468x60,
        Leaderboard_728x90
    }

    [CreateAssetMenu(fileName = "AdmobSettings", menuName = "IPS/Api/AdmobSettings")]
    public class AdmobSettings : ScriptableObject, IAdSettings {
        [Tooltip("Show UMP for consent popup or not")]
        public bool EnableUMP = true;
        public string iOSATTMessage = "Project is supported by ads. To make these ads as relevant to you as possible, we would like to collect and share data with our ads partners. Please note that choosing no will not remove ads - Instead you may see ads that maybe unrelated to your interests, therefore may not appeal to you.";// "Your data will be used to provide you a better and personalized ad experience.";
        [Header("Debug config")]
        [Tooltip("Can setup from RemoteConfig by key 'ad_log_enable'")]
        public bool EnableLog = false;
        [Tooltip("if enableTest, you no need to fill any id. You can also setup from RemoteConfig by key 'ad_test_enable'")]
        public bool UseTestAd = false;
        [Tooltip("if not enableTest, this device can be test real ads without `No Fill` message like other device.")]
        public string TestDevice_android = "";
        public string TestDevice_iOS = "";

        [Header("App id")]
        [SerializeField] string appID_android;
        [SerializeField] string appID_iOS;

        [Header("Ads Resune Settings")]
        public bool AdResumeEnable = true;
        public int AdsResumeCapping = 0;
        public ulong AdsResumeFromPlayedTime = 0;

        [Header("AOA Settings")]
        [SerializeField] bool useAOAAd = false;
        [SerializeField] string aoaTierId_Android;
        [SerializeField] string aoaTierId_iOS;
        [SerializeField] bool aoaOpenEnable = true;
        [SerializeField] bool aoaOpenFirstInstallEnable = false;
        [SerializeField] bool aoaResumeEnable = true;
        [Tooltip("AOA will be show when loading scene finished, before enter the game scene. If use GDPR set=2, else set=1")]
        public int AoaShowBeforeSceneIdx = 1;

        [Header("Admob - Native Ad Settings")]
        [SerializeField] bool useNativeAd = false;
        public bool UseNativeAd_UI = true;
        public bool UseNativeAd_3D = false;
        [SerializeField] string nativeId_Android;
        [SerializeField] string nativeId_iOS;
        public bool PreloadNativeAd = true;

        [Header("Banner")]
        [SerializeField] bool useBannerAd = false;
        [SerializeField] string bannerID_android;
        [SerializeField] string bannerID_iOS;
        public BannerSize BannerSize = BannerSize.AdaptiveBanner;
        [SerializeField] bool useBannerCollapsible = true;
        [Tooltip("If use collapsible and collapsible load failed, use other mediation or not")]
        public bool CollapsibleFallbackEnable = false;
        [Tooltip("If true, collapsible will auto refresh popup after 30-120s, turn this off to manual refresh in your game (call `Destroy` then call `Show` in this case).")]
        public bool CollapsibleAutoRefresh = false;
        [Tooltip("If collapsibleAutoRefresh is FALSE, use this interval to check can show new collapsible or refresh last UUID only")]
        public int CollapsibleReloadInterval = 30;
        public uint BannerFromPlayTimes = 0;
        public uint BannerFromLevel = 0;
        [Tooltip("Destroy current banner then load new banner after this capping time. Value = 0 mean never destroy banner, it will be auto refresh by console settings.")]
        public uint BannerReloadCapping = 0;

        [Tooltip("Banner will destroy and reload new if this value > 0 and after count of end level")]
        public uint BannerReloadByLevelCapping = 0;
        public bool ReloadBannerOnLoadScene => BannerReloadByLevelCapping > 0;

        [SerializeField] [Tooltip("Set to false if you want to show banner on top")] bool showBannerOnBottom = true;
        
        [Header("MREC (Medium Rectangle)")]
        [Tooltip("DO NOT USE BOTH Banner & MREC in the same Mediation")]
        [SerializeField] bool useMRecAd = false;
        [SerializeField] string mrecrID_android;
        [SerializeField] string mrecID_iOS;
        public bool ShowMRecOnBottom = true;
        [Tooltip("The padding y in dp from top/bottom screen to the top/bottom of the MRec. The (x, y) anchor coordinate of the MREC relative to the top left corner of the screen. If y=0 then use ad-view position below by default")]
        [HideInInspector] public uint MrecPaddingY_Dp { get; set; } = 60;

        [Header("Interstitial")]
        [SerializeField] bool useInterstitialAd = false;
        [SerializeField] string interstitialID_android;
        [SerializeField] string interstitialID_iOS;
        [SerializeField] bool interResumeEnable = true;
        [SerializeField] long interCapping = 0;
        [SerializeField] long interAfterRewardCapping = 0;
        public uint InterFromPlayTimes = 0;
        public uint InterFromLevel = 0;
        public uint InterFromSeconds = 0;
        public bool ForceInterEnable = false;
        public long ForceInterCapping = 90;

        [Header("RewardVideo")]
        [SerializeField] bool useRewardVideoAd = false;
        [SerializeField] string rewardID_android;
        [SerializeField] string rewardID_iOS;
        
        [Header("RewardInterstitial")]
        [SerializeField] bool useRewardInterstitialAd = false;
        [SerializeField] string rewardInterID_android;
        [SerializeField] string rewardInterID_iOS;

        public int RemoteFetchRemaining { get; set; } = 0;
        public bool AdsRemoteConfigFetched { get; set; } = false;

        #region Interface
        public bool UseAOAAd {
            get => useAOAAd && !string.IsNullOrEmpty(AOAId);
            set => useAOAAd = value;
        }

        public bool AoaOpenEnable {
            get => UseAOAAd && aoaOpenEnable;
            set => aoaOpenEnable = value; 
        }

        public bool AoaOpenFirstInstallEnable { 
            get => UseAOAAd && aoaOpenFirstInstallEnable; 
            set => aoaOpenFirstInstallEnable = value; 
        }

        public bool AoaResumeEnable {
            get => UseAOAAd && aoaResumeEnable;
            set => aoaResumeEnable = value;
        }

        public bool UseNativeAd {
            get => useNativeAd && !string.IsNullOrEmpty(NativeId);
            set => useNativeAd = value;
        }
        public bool UseInterstitialAd {
            get => useInterstitialAd && !string.IsNullOrEmpty(InterstitialID);
            set => useInterstitialAd = value;
        }

        public bool InterResumeEnable {
            get => UseInterstitialAd && interResumeEnable;
            set => interResumeEnable = value;
        }

        public long InterCapping {
            get => interCapping;
            set => interCapping = value;
        }

        public long InterAfterRewardCapping {
            get => interAfterRewardCapping;
            set => interAfterRewardCapping = value;
        }

        public bool UseRewardedVideoAd {
            get => useRewardVideoAd && !string.IsNullOrEmpty(RewardVideoID);
            set => useRewardVideoAd = value;
        }

        public bool UseRewardedInterstitialAd {
            get => useRewardInterstitialAd && !string.IsNullOrEmpty(RewardInterId);
            set => useRewardInterstitialAd= value;
        }

        public bool UseBannerAd {
            get => useBannerAd && !string.IsNullOrEmpty(BannerID);
            set => useBannerAd = value;
        }

        public bool UseBannerAdaptive {
            get { return false; }
            set { }
        }

        public bool UseMRecAd {
            get => useMRecAd && !string.IsNullOrEmpty(MRecID);
            set => useMRecAd = value;
        }
        #endregion

        private bool IsAndroid => Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsEditor;

        public string AppID_Android => !UseTestAd ? appID_android : "ca-app-pub-3940256099942544~3347511713";
        public string AppID_iOS => !UseTestAd ? appID_iOS : "ca-app-pub-3940256099942544~1458002511";

        public string AppID => IsAndroid ? AppID_Android : AppID_iOS;

        private string[] testDevices;
        public string[] TestDevices {
            get {
                if (testDevices != null && testDevices.Length > 0) return testDevices;
                string data = IsAndroid ? TestDevice_android : TestDevice_iOS;
                if (!string.IsNullOrEmpty(data)) testDevices = data.Trim(' ').Split(',');
                return testDevices;
            }
        }

        public string AOAId {
            get {
                if (!useAOAAd) return string.Empty;
                if (IsAndroid) { return !UseTestAd ? aoaTierId_Android : "ca-app-pub-3940256099942544/9257395921"; }
                else { return !UseTestAd ? aoaTierId_iOS : "ca-app-pub-3940256099942544/5575463023"; }
            }
        }

        public string NativeId {
            get {
                if (!useNativeAd) return string.Empty;
                if (IsAndroid) { return !UseTestAd ? nativeId_Android : "ca-app-pub-3940256099942544/2247696110"; }
                else { return !UseTestAd ? nativeId_iOS : "ca-app-pub-3940256099942544/3986624511"; }
            }
        }

        public string RewardInterId {
            get {
                if (!useRewardInterstitialAd) return string.Empty;
                if (IsAndroid) { return !UseTestAd ? rewardInterID_android : "ca-app-pub-3940256099942544/5354046379"; }
                else { return !UseTestAd ? rewardInterID_iOS : "ca-app-pub-3940256099942544/6978759866"; }
            }
        }

        public string RewardVideoID {
            get {
                if (!useRewardVideoAd) return string.Empty;
                if (IsAndroid) { return !UseTestAd ? rewardID_android : "ca-app-pub-3940256099942544/5224354917"; }
                else { return !UseTestAd ? rewardID_iOS : "ca-app-pub-3940256099942544/1712485313"; }
            }
        }

        public string InterstitialID {
            get {
                if (!useInterstitialAd) return string.Empty;
                if (IsAndroid) { return !UseTestAd ? interstitialID_android : "ca-app-pub-3940256099942544/1033173712"; }
                else { return !UseTestAd ? interstitialID_iOS : "ca-app-pub-3940256099942544/4411468910"; }
            }
        }

        public string MRecID {
            get {
                if (!useMRecAd) return string.Empty;
                if (IsAndroid) { return !UseTestAd ? mrecrID_android : "ca-app-pub-3940256099942544/2014213617"; }//"ca-app-pub-3940256099942544/6300978111"; }
                else { return !UseTestAd ? mrecID_iOS : "ca-app-pub-3940256099942544/2934735716"; }
            }
        }

        public string BannerID {
            get {
                if (!useBannerAd) return string.Empty;
                if (IsAndroid) { return !UseTestAd ? bannerID_android : "ca-app-pub-3940256099942544/2014213617"; }//"ca-app-pub-3940256099942544/6300978111"; }
                else { return !UseTestAd ? bannerID_iOS : "ca-app-pub-3940256099942544/2934735716"; }
            }
        }

        public bool UseBannerCollapsible {
            get => useBannerAd && useBannerCollapsible;
            set => useBannerCollapsible = useBannerAd && value;
        }

        public bool ShowBannerOnBottom {
            get => showBannerOnBottom;
            set => showBannerOnBottom = value;
        }

        #region Singleton
        private static AdmobSettings _instance;
        public static AdmobSettings Instance {
            get {
                if (_instance != null) return _instance;
                _instance = Resources.Load<AdmobSettings>($"{typeof(AdmobSettings).Name}/{typeof(AdmobSettings).Name}");
                if (_instance == null) {
                    Debug.LogError($"[Ads] AdmobConfig file not found at Resources/{typeof(AdmobSettings).Name}/{typeof(AdmobSettings).Name}");
                }
                return _instance;
            }
        }
        #endregion


#if UNITY_EDITOR
        [UnityEditor.MenuItem("IPS/Api/Ads/AdsSettings")]
        public static void OpenAdmobSettingsFile() {
            string dir = $"Assets/{ApiSettings.LIB_FOLDER}/Api/Ads/Admob/Settings/Resources/{typeof(AdmobSettings).Name}/";
            string assetPath = Path.Combine(dir, $"{typeof(AdmobSettings).Name}.asset");
            if (AdmobSettings.Instance == null) {
                Directory.CreateDirectory(dir);
                var ins = ScriptableObject.CreateInstance<AdmobSettings>();
                UnityEditor.AssetDatabase.CreateAsset(ins, assetPath);
                UnityEditor.AssetDatabase.SaveAssets();
            }

            UnityEditor.Selection.activeObject = UnityEditor.AssetDatabase.LoadAssetAtPath<AdmobSettings>(assetPath);
        }
#endif
    }
}