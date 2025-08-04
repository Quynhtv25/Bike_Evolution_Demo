#if ADMOB
using GoogleMobileAds.Ump.Api;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IPS.Api.Ads {
    public partial class AdmobMediation {
        private void OnStartUMP() {
            Time.timeScale = 0;
            LoadingMask.Instance.Show(string.Empty);
        }

        private void OnFinishedUMP() {
            Time.timeScale = 1;
            LoadingMask.Instance.Hide();
        }

        private void SendConsentRequest() {
#if ADMOB
            //ResetUMPState();
            Debug.Log("[Ads.Admob] UMP initializing..");
            OnStartUMP();
            ConsentRequestParameters request = new ConsentRequestParameters();
            ConsentInformation.Update(request, OnConsentInfoUpdated);
#endif
        }

#if ADMOB
        void OnConsentInfoUpdated(FormError consentError) {
            if (consentError != null) {
                // Handle the error.
                Excutor.Schedule(() => {
                    Tracking.Instance.LogException("[Ads.Admob]", "UMPSendConsentRequest", "OnConsentInfoUpdated error: " + consentError.Message);
                    StartRequestAds();
                    OnFinishedUMP();
                });
                return;
            }

            // If the error is null, the consent information state was updated.
            // You are now ready to check if a form is available.
            ConsentForm.LoadAndShowConsentFormIfRequired((FormError formError) => {
                Excutor.Schedule(() => {
                    if (formError != null) {
                        Tracking.Instance.LogException("[Ads.Admob]", "UMPSendConsentRequest", "LoadAndShowConsentFormIfRequired error: " + formError.Message);
                    }
                    else {
                        if (ConsentInformation.CanRequestAds()) {
                            Debug.Log("[Ads.Admob] UMP Consent has been gathered");
                            UserData.GDPRConsentStatus = ConsentInformation.ConsentStatus != ConsentStatus.Required;
                        }
                        else {
                            Tracking.Instance.LogException("[Ads.Admob]", "UMPSendConsentRequest", "Success but CanRequestAds=false");
                        }
                    }
                    StartRequestAds();
                    OnFinishedUMP();
                });
            });
        }
#endif

        partial void StartRequestAds();

//        /// <summary> USE FOR DEBUG VERSION ONLY </summary>
//        private void ResetUMPState() {
//#if ADMOB
//            if (IPSConfig.CheatEnable) ConsentInformation.Reset();
//#endif
//        }

    }
}
