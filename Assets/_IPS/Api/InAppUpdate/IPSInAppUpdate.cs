#if IAD
// https://developer.android.com/guide/playcore/in-app-updates/unity#immediate
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ANDROID
using Google.Play.Common;
using Google.Play.AppUpdate;
#endif

namespace IPS {
    public class IPSInAppUpdate : SingletonBehaviourDontDestroy<IPSInAppUpdate> {

        protected override void OnAwake() {
#if UNITY_ANDROID && !UNITY_EDITOR
            AppUpdateManager appUpdateManager = new AppUpdateManager();
            CheckForUpdate(appUpdateManager);
#endif
        }

#if UNITY_ANDROID
        IEnumerator CheckForUpdate(AppUpdateManager appUpdateManager) {
            PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation = appUpdateManager.GetAppUpdateInfo();

            // Wait until the asynchronous operation completes.
            yield return appUpdateInfoOperation;

            if (appUpdateInfoOperation.IsSuccessful) {
                var appUpdateInfoResult = appUpdateInfoOperation.GetResult();
                // Check AppUpdateInfo's UpdateAvailability, UpdatePriority,
                // IsUpdateTypeAllowed(), etc. and decide whether to ask the user
                // to start an in-app update.
                
                //var priority = appUpdateInfoResult.UpdatePriority;

                // Creates an AppUpdateOptions defining an immediate in-app
                // update flow and its parameters.
                if (appUpdateInfoResult == null 
                    || appUpdateInfoResult.UpdateAvailability != UpdateAvailability.UpdateAvailable 
                    || appUpdateInfoResult.AvailableVersionCode <= BootstrapConfig.Instance.VersionCode) 
                    
                    yield break;

                yield return StartFlexibleUpdate(appUpdateManager, appUpdateInfoResult);
            }
            else {
                // Log appUpdateInfoOperation.Error.
            }
        }

        IEnumerator StartImmediateUpdate(AppUpdateManager appUpdateManager, AppUpdateInfo appUpdateInfoResult) {
            // Creates an AppUpdateRequest that can be used to monitor the
            // requested in-app update flow.
            var startUpdateRequest = appUpdateManager.StartUpdate(
              // The result returned by PlayAsyncOperation.GetResult().
              appUpdateInfoResult,
              // The AppUpdateOptions created defining the requested in-app update
              // and its parameters.
              AppUpdateOptions.ImmediateAppUpdateOptions());

            yield return startUpdateRequest;

            // If the update completes successfully, then the app restarts and this line
            // is never reached. If this line is reached, then handle the failure (for
            // example, by logging result.Error or by displaying a message to the user).
            NoticeText.Instance.ShowNotice("Somethings wrong! Try again.");
            Tracking.Instance.LogException(typeof(IPSInAppUpdate).Name, nameof(StartImmediateUpdate), startUpdateRequest.Error.ToString());
        }

        IEnumerator StartFlexibleUpdate(AppUpdateManager appUpdateManager, AppUpdateInfo appUpdateInfoResult) {
            // Creates an AppUpdateRequest that can be used to monitor the
            // requested in-app update flow.
            var startUpdateRequest = appUpdateManager.StartUpdate(
              // The result returned by PlayAsyncOperation.GetResult().
              appUpdateInfoResult,
              // The AppUpdateOptions created defining the requested in-app update
              // and its parameters.
              AppUpdateOptions.FlexibleAppUpdateOptions());

            while (!startUpdateRequest.IsDone) {
                // For flexible flow,the user can continue to use the app while
                // the update downloads in the background. You can implement a
                // progress bar showing the download status during this time.
                yield return null;
            }

            var result = appUpdateManager.CompleteUpdate();
            yield return result;
            // If the update completes successfully, then the app restarts and this line
            // is never reached. If this line is reached, then handle the failure (e.g. by
            // logging result.Error or by displaying a message to the user).
            NoticeText.Instance.ShowNotice("Somethings wrong! Try again.");
            Tracking.Instance.LogException(typeof(IPSInAppUpdate).Name, nameof(StartFlexibleUpdate), startUpdateRequest.Error.ToString());
        }
#endif
    }
}
#endif