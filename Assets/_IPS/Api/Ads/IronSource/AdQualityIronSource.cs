#if IS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace IPS.Api.Ads {
    public class AdQualityIronSource {
        /// <summary> Call this from Awake method of your first scene </summary>
        /// <param name="appKey">IronSource appKey which config in LevelPlay setting</param>
        public void Init(string appKey, bool testMode = false, string userId = default) {
            ISAdQualityConfig adQualityConfig = new ISAdQualityConfig();

            if (!string.IsNullOrEmpty(userId)) adQualityConfig.UserId = userId;
            adQualityConfig.TestMode = testMode;
            adQualityConfig.LogLevel = testMode ? ISAdQualityLogLevel.VERBOSE : ISAdQualityLogLevel.NONE;

            adQualityConfig.AdQualityInitCallback = new AdQualitySdkInit();

            IronSourceAdQuality.Initialize(appKey, adQualityConfig);
        }

        public void ChangeUserId(string userId) {
            IronSourceAdQuality.ChangeUserId(userId);
        }

        /// <summary> Called when the ad displayed. The feature is support for fullscreen ads only (interstitial/rewarded video). 
        /// <para>For LevelPlay (IronSource), MAX and DT FairBid this data is being collected automatically</para></summary>
        public void LogAdRevenue(double revenue, ISAdQualityAdType adType, ISAdQualityMediationNetwork mediationNetwork = ISAdQualityMediationNetwork.SELF_MEDIATED) {
            ISAdQualityCustomMediationRevenue customMediationRevenue = new ISAdQualityCustomMediationRevenue();
            customMediationRevenue.MediationNetwork = mediationNetwork;
            customMediationRevenue.AdType = adType;
            customMediationRevenue.Revenue = revenue;
            IronSourceAdQuality.SendCustomMediationRevenue(customMediationRevenue);
        }

        public class AdQualitySdkInit : ISAdQualityInitCallback {
            public void adQualitySdkInitSuccess() {
                Debug.Log("[Ad] adQualitySdkInitSuccess");
            }
            public void adQualitySdkInitFailed(ISAdQualityInitError adQualitySdkInitError, string errorMessage) {
                Debug.Log("[Ad] adQualitySdkInitFailed " + adQualitySdkInitError + " message: " + errorMessage);
            }
        }
    }
}
#endif