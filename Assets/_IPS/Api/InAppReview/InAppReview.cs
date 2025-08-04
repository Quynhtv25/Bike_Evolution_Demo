using System;
using System.Collections;
using UnityEngine;

#if IAR && UNITY_ANDROID
using Google.Play.Review;
#endif

namespace IPS {
    public class InAppReview : SingletonBehaviour<InAppReview> {

        private bool isRequesting;

        protected override void OnAwake() {
        }

        public void RequestNativeReview(Action callback = null) {
            if (isRequesting) return;
#if IAR
            isRequesting = true;
            StartCoroutine(IERequestNativeReview(callback));
#endif
        }

#if IAR
        public IEnumerator IERequestNativeReview(Action onSuccess = null) {
            Debug.Log("[IAR] Start request native review..");

#if UNITY_ANDROID
            var reviewMng = new ReviewManager();

            var requestFlowOperation = reviewMng.RequestReviewFlow();
            yield return requestFlowOperation;

            if (requestFlowOperation.Error != ReviewErrorCode.NoError) {
                Logs.LogError($"[IAR] {requestFlowOperation.Error.ToString()}");
                isRequesting = false;
                yield break;
            }

            Debug.Log("[IAR] Start lauch in-app review..");
            var launchFlowOperation = reviewMng.LaunchReviewFlow(requestFlowOperation.GetResult());
            yield return launchFlowOperation;

            if (launchFlowOperation.Error != ReviewErrorCode.NoError) {
                // Log error. For example, using requestFlowOperation.Error.ToString().
                Logs.LogError($"[IAR] {launchFlowOperation.Error.ToString()}");
                isRequesting = false;
                yield break;
            }
#elif UNITY_IOS
            if (UnityEngine.iOS.Device.RequestStoreReview()) {
                Debug.Log("[IAR] Start lauch in-app review..");
            }
            else {
                Debug.LogError($"[IAR] Request native review failed. iOS version {UnityEngine.iOS.Device.systemVersion} not recent enough or StoreKit is not linked");
                isRequesting = false;
                yield break;
            }
#endif
            Debug.Log("[IAR] Native review finished");
            isRequesting = false;
            if (onSuccess != null) onSuccess.Invoke();
            yield return null;
        }
#endif
    }
}