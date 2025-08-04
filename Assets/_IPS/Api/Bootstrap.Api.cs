using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_IOS
using Unity.Advertisement.IosSupport; // Go to Package Manager, search "iOS 14 Advertising Support" then install
#endif

namespace IPS {
    /// <summary>
    /// Place this into the first scene
    /// </summary>
    public partial class Bootstrap {
        partial void AwakeApi() {
#if GDPR
            if (GDPR.HasConsent) Init();
            else {
                GDPR.onConsentCallback += Init;
            }
#else
            Init();
#endif
        }

        private void Init() {
#if UNITY_IOS && !LION
            // check with iOS to see if the user has accepted or declined tracking
            var status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();

            if (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED) {
                StartCoroutine(IEWaitForRequestATT(PreloadApi));
            }

#else
            PreloadApi();
#endif

        }

#if UNITY_IOS
        private IEnumerator IEWaitForRequestATT(System.Action callback) {
            ATTrackingStatusBinding.RequestAuthorizationTracking();
            yield return new WaitUntil(() => ATTrackingStatusBinding.GetAuthorizationTrackingStatus() != ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED);

            if (callback != null) callback.Invoke();
        }
#endif

        /// <summary> DO NOT CHANGE THE ORDER OF PRELOAD LIST </summary>
        private void PreloadApi() {
            PreloadRemoteConfig();
            PreloadInAppUpdate();
            PreloadCloudMessaging();
            PreloadCloudStorage();
            PreloadAnalytics();
            PreloadIAP();
            PreloadAds();
            PreloadInAppReview();
        }

        partial void PreloadAnalytics();
        partial void PreloadCloudMessaging();
        partial void PreloadCloudStorage();
        partial void PreloadAds();
        partial void PreloadIAP();
        partial void PreloadRemoteConfig();
        partial void PreloadInAppReview();
        partial void PreloadInAppUpdate();
    }
}