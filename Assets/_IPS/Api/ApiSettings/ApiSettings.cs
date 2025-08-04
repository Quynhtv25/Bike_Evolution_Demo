
using UnityEngine;

namespace IPS {
    [CreateAssetMenu(fileName = "ApiSettings", menuName = LIB_MENU + "/Api/ApiSettings")]
    public class ApiSettings : ScriptableObject {
        public const string LIB_MENU = "IPS";
        public const string LIB_FOLDER = "_IPS";

        [Tooltip("Turn on for release to store. If build from Jenskin this will be true automatically.")]
        public bool productionBuild = false;
        public bool useGDPR = false;

        [Header("===== Analytics =====")]
        public bool useFirebase;
        public bool useFacebook;
        public bool useByteBrew;
        public bool useAppmetrica;
        public bool useAdjust;
        [Space]
        public bool useAppsFlyer;
        [Tooltip("AppsFlyer ROI360 for IAP")]
        public bool useAppsFlyerROI;

        [Header("Features")]
        public bool useAds;
        public bool useInAppReview;
        public bool useInAppPurchase;
        public bool useInAppUpdate;

        [Header("Firebase Features")] 
        public bool useRemoteConfig;
        public bool useCloudMessaging;
        public bool useCloudStorage;

        [Header("Other")]
        [Tooltip("Install FalconCore & Use AppsFlyer from menu Falcon/Menu. Do not import script AppsflyerSetting, then edit file FalconAppsFlyerAndAdjust to use IPS.Api.Analytics.AppsFlyerSettings")]
        public bool useFalconSDK;
        public bool useLionSDK;

        public const string DebugDefine = "CUSTOM_DEBUG";
        public const string NoAdDefine = "NOAD";
        public const string ProductionDefine = "PRODUCTION";

        public const string GDPRDefine = "GDPR";
        public const string AdsDefine = "ADS";
        public const string InAppPurchaseDefine = "IAP";
        public const string RemoteDefine = "REMOTE";
        public const string CloudMessagingDefine = "FCM";
        public const string CloudStorageDefine = "FCS";
        public const string InAppReviewDefine = "IAR";
        public const string InAppUpdateDefine = "IAD";

        public const string FirebaseDefine = "FIREBASE";
        public const string FacebookDefine = "FACEBOOK";
        public const string AppsFlyerDefine = "APPSFLYER";
        public const string AppsFlyerROIDefine = "AF_ROI";
        public const string ByteBrewDefine = "BYTEBREW";
        public const string AppMetricaDefine = "APPMETRICA";
        public const string AdjustDefine = "ADJUST";

        public const string FalconSDKDefine = "FALCON_ANALYTIC";
        public const string LionSDKDefine = "LION";

    }
}
