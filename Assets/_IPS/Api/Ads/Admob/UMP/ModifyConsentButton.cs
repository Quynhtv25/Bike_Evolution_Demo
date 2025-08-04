using UnityEngine;
#if ADMOB
using GoogleMobileAds.Ump.Api;
#endif

namespace IPS.Api.Ads {
    [RequireComponent(typeof(ButtonEffect))]
    public class ModifyConsentButton : MonoBehaviour {
#if ADMOB
        void Start() {
            var btn = GetComponent<ButtonEffect>();
            btn.AddListener(OnClick);
            btn.gameObject.SetActive(ConsentInformation.PrivacyOptionsRequirementStatus == PrivacyOptionsRequirementStatus.Required);
        }


        void OnClick() {
            ConsentForm.ShowPrivacyOptionsForm(formError => {
                GetComponent<ButtonEffect>().Interactable = ConsentInformation.PrivacyOptionsRequirementStatus == PrivacyOptionsRequirementStatus.Required;
            });

        }
#else
        private void Start() {
            gameObject.SetActive(false);
        }
#endif
    }
}