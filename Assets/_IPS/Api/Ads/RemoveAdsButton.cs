using System;
using UnityEngine;
using UnityEngine.UI;

namespace IPS {

    [RequireComponent(typeof(UnityEngine.UI.Button))]
    public class RemoveAdsButton : MonoBehaviour {
        [SerializeField] bool hideOnSuccess = true;
        [SerializeField] bool alsoRemoveRewardAds = false;
        [SerializeField] TMPro.TextMeshProUGUI priceText;
        [Tooltip("Call when user click buy remove ads only")] 
        public UnityEngine.Events.UnityEvent onSuccess;
        [Tooltip("Call when user already owned remove ads")] 
        public UnityEngine.Events.UnityEvent onRestorePurchase;

        private void Start() {
            if (AdsManager.Instance.IsRemovedAds) {
                Hide();
                return;
            }

            gameObject.GetComponent<Button>().onClick.AddListener(BuyRemoveAds);
            this.AddListener<AdsManager.OnAdRemoved>(Hide, false);

            UpdatePriceText();

            IAP.Instance.onInitialized += UpdatePriceText;
        }

        private void OnEnable() {
            if (AdsManager.Initialized && AdsManager.Instance.IsRemovedAds) Hide();
        }

        private void OnDestroy() {
            if (IAP.Initialized) IAP.Instance.onInitialized -= UpdatePriceText;
        }

        private void UpdatePriceText() {
            if (AdsManager.Instance.IsRemovedAds) {
                Hide();
                return;
            }
            if (priceText == null) return;
            var product = IAP.Instance.GetLocalPrice(IAP.RemoveAdsKey);
            if (product != null) priceText.text = product.LocalizedPriceSymbol;
            else Debug.LogError($"Product not exist id={IAP.RemoveAdsKey}");
        }

        private void BuyRemoveAds() {
            if (IPSConfig.CheatEnable) OnBuySuccessfully();
            else {
                IAP.Instance.Buy(IAP.RemoveAdsKey, OnBuySuccessfully, OnBuyFailed);
            }
        }

        private void OnBuySuccessfully() {
            GetComponent<Button>().interactable = false;
            if (onSuccess != null) onSuccess.Invoke();
            AdsManager.Instance.SetRemovedAds(alsoRemoveRewardAds);
            NoticeText.Instance.ShowNotice("Successfully");
            if (hideOnSuccess) Hide();
        }

        private void OnBuyFailed() {
            NoticeText.Instance.ShowNotice("Somethings wrong! Try again.");
        }

        private void Hide() {
            gameObject.SetActive(false);
            if (onRestorePurchase != null) onRestorePurchase.Invoke();
        }
    }

}