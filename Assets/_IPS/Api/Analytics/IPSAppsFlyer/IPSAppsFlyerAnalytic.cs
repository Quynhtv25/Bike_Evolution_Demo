
#if APPSFLYER

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppsFlyerSDK;

// https://github.com/AppsFlyerSDK/appsflyer-unity-purchase-connector/blob/master/docs/purchaseConnectorUnity.md#set-up-purchase-connector
#if AF_ROI
using AppsFlyerConnector;
#endif

namespace IPS.Api.Analytics {
    public class IPSAppsFlyerAnalytic : SingletonBehaviourDontDestroy<IPSAppsFlyerAnalytic> {

        protected override void OnAwake() {
            Init();
        }

        private void Init() {
            var settings = Resources.Load<AppsFlyerSettings>($"{typeof(AppsFlyerSettings).Name}");
            if (settings == null) {
                Logs.LogError($"[APPSFLYER] Cannot load setting from resources path: /{typeof(AppsFlyerSettings).Name}");
                return;
            }

            bool debug = !BootstrapConfig.Instance.IsAAB;
            AppsFlyer.setIsDebug(debug);

#if UNITY_IOS
            if (string.IsNullOrEmpty(settings.AppID)) {
                Tracking.Instance.LogException(typeof(IPSAppsFlyerAnalytic).Name, nameof(Init), "AppsFlyerSettings is missing iOS appId!");
            }
#endif
            AppsFlyer.initSDK(settings.DevKey, settings.AppID);

#if AF_ROI
            // Purchase connector implementation 
            AppsFlyerPurchaseConnector.init(this, AppsFlyerConnector.Store.GOOGLE);
            AppsFlyerPurchaseConnector.setIsSandbox(debug);
            AppsFlyerPurchaseConnector.setAutoLogPurchaseRevenue(AppsFlyerAutoLogPurchaseRevenueOptions.AppsFlyerAutoLogPurchaseRevenueOptionsAutoRenewableSubscriptions, 
                                                                AppsFlyerAutoLogPurchaseRevenueOptions.AppsFlyerAutoLogPurchaseRevenueOptionsInAppPurchases);
            AppsFlyerPurchaseConnector.setPurchaseRevenueValidationListeners(true);
            AppsFlyerPurchaseConnector.build();
            AppsFlyerPurchaseConnector.startObservingTransactions();

            Debug.Log($"<color=green>[APPSFLYER] ROI360 Initialized: SDK Version={AppsFlyerPurchaseConnector.kAppsFlyerPurchaseConnectorVersion}, sandbox={debug}</color>");
#endif

#if FALCON_ANALYTIC && USE_APPSFLYER
            FalconAppsFlyerAndAdjust.StartSDK();
#else
            AppsFlyer.startSDK();
#endif

            Debug.Log($"<color=green>[APPSFLYER] Initialized: SDK Version={AppsFlyer.getSdkVersion()} debug={debug}</color>");
        }

#if AF_ROI
        public void didReceivePurchaseRevenueValidationInfo(string validationInfo) {
            Debug.Log($"<color=green>[IAP][APPSFLYER] didReceivePurchaseRevenueValidationInfo: </color>" + validationInfo);
            // deserialize the string as a dictionnary, easy to manipulate
            Dictionary<string, object> dictionary = AFMiniJSON.Json.Deserialize(validationInfo) as Dictionary<string, object>;

            // if the platform is Android, you can create an object from the dictionnary 
#if UNITY_ANDROID
            if (dictionary.ContainsKey("productPurchase") && dictionary["productPurchase"] != null) {
                // Create an object from the JSON string.
                InAppPurchaseValidationResult iapObject = JsonUtility.FromJson<InAppPurchaseValidationResult>(validationInfo);
                if (iapObject != null) {
                    var param = ParameterBuilder.Create().Add("success", iapObject.success).Add("isSubscription", false);
                    if (iapObject.productPurchase != null) {
                        param.Add("productId", iapObject.productPurchase.productId);
                        param.Add("purchaseState", iapObject.productPurchase.purchaseState);
                    }

                    if (iapObject.failureData != null) {
                        param.Add("errormsg", iapObject.failureData.status);
                    }
                    Tracking.Instance.LogEvent("af_purchase_validation", param, service: Tracking.Service.ByteBrew);
                }
            }
            else if (dictionary.ContainsKey("subscriptionPurchase") && dictionary["subscriptionPurchase"] != null) {
                SubscriptionValidationResult iapObject = JsonUtility.FromJson<SubscriptionValidationResult>(validationInfo);
                if (iapObject != null) {
                    var param = ParameterBuilder.Create().Add("success", iapObject.success).Add("isSubscription", true);
                    if (iapObject.subscriptionPurchase != null) {
                        param.Add("subscriptionId", iapObject.subscriptionPurchase.latestOrderId);
                        param.Add("purchaseState", iapObject.subscriptionPurchase.subscriptionState);
                    }

                    if (iapObject.failureData != null) {
                        param.Add("errormsg", iapObject.failureData.status);
                    }
                    Tracking.Instance.LogEvent("af_purchase_validation", param, service: Tracking.Service.ByteBrew);
                }
            }
            else {
                Tracking.Instance.LogEvent("af_purchase_validation", "data", validationInfo, service: Tracking.Service.ByteBrew);
            }
#else
            Tracking.Instance.LogEvent("af_purchase_validation", "data", validationInfo, service: Tracking.Service.ByteBrew);
#endif
        }
#endif

        public void LogEvent(string eventName) {
            this.Log(eventName, null);
        }

        public void LogEvent(string eventName, string paraName, string paraValue) {
            this.Log(eventName, new Dictionary<string, string>() { { paraName, paraValue } });
        }

        public void LogEvent(string eventName, ParameterBuilder parameterBuilder) {
            this.Log(eventName, parameterBuilder != null ? parameterBuilder.BuildDictString() : null);
        }

        public void Log(string eventName, Dictionary<string, string> para) {
            Logs.Log($"[APPSFLYER] eventName={eventName}, paraCount={(para != null ? para.Count : 0)}");
            if (UnityEngine.Application.isEditor) {
                return;
            }
            AppsFlyer.sendEvent(eventName, para);
        }

        public void LogAdRevenue(int mediation, string network, double revenue, string currency, ParameterBuilder parameter) {
            Logs.Log($"[APPSFLYER] logAdRevenue, mediation={mediation}, network={network}, revenue={revenue}{currency}");
            if (UnityEngine.Application.isEditor) {
                return;
            }

            try {
                AppsFlyer.logAdRevenue(new AFAdRevenueData(network, (MediationNetwork)mediation, currency, revenue), 
                    parameter != null ? parameter.BuildDictString() : null);
            }
            catch {
                Logs.LogError($"[APPSFLYER] logAdRevenue INVALID MEDIATION = {mediation}!");
                throw;
            }
        }

        public void LogPurchase(string productId, string currency, double revenue, int quantity = 1) {
            Logs.Log($"[APPSFLYER] LogPurchase, productId={productId}, revenue={revenue}{currency}");
            if (UnityEngine.Application.isEditor) {
                return;
            }
#if AF_ROI
            return;
#endif
            Dictionary<string, string> eventValues = new Dictionary<string, string>();
            eventValues.Add("af_productId", productId);
            eventValues.Add(AFInAppEvents.CONTENT_ID, productId);
            eventValues.Add(AFInAppEvents.CURRENCY, !string.IsNullOrEmpty(currency) ? currency : "USD");
            eventValues.Add(AFInAppEvents.REVENUE, revenue.ToString());
            eventValues.Add("af_quantity", quantity.ToString());
            AppsFlyer.sendEvent(AFInAppEvents.PURCHASE, eventValues);
        }
    }
}
#endif