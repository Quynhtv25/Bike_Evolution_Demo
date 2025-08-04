#if ADJUST

using AdjustSdk;
using System;
using UnityEngine;

namespace IPS.Api.Analytics {
    public class IPSAdjustAnalytics : SingletonBehaviourDontDestroy<IPSAdjustAnalytics> {
        Action callOnAvailable;
        bool sdkInitialized;

        protected override void OnAwake() {
            Debug.Log($"[ADJUST] Prepare Initialize..");
            gameObject.AddComponent<Adjust>();
        }

        private void Start() {
            var settings = Resources.Load<AdjustSettings>($"{typeof(AdjustSettings).Name}/{typeof(AdjustSettings).Name}");
    
            if (settings != null) {
                var config = new AdjustConfig(settings.appToken, IPSConfig.CheatEnable ? AdjustEnvironment.Sandbox : AdjustEnvironment.Production);
                config.LogLevel = IPSConfig.CheatEnable ? AdjustLogLevel.Verbose : AdjustLogLevel.Suppress;
                config.DefaultTracker = settings.defaultTracker;
                config.IsDeferredDeeplinkOpeningEnabled = settings.launchDeferredDeeplink;
                config.IsAdServicesEnabled = settings.adServices;
                config.IsIdfaReadingEnabled = settings.idfaReading;
                config.IsSkanAttributionEnabled = settings.skanAttribution;
                
                Adjust.InitSdk(config);

                sdkInitialized = true;
                Debug.Log($"[ADJUST] Initialized");
                if (callOnAvailable != null) {
                    callOnAvailable.Invoke();
                    callOnAvailable = null;
                }
            }
            else Debug.LogError("[ADJUST] Cannot found IPS.AdjustSettings!");
        }

        public void LogEvent(string eventToken) {
            if (string.IsNullOrEmpty(eventToken)) return;
            if (sdkInitialized) {
                Logs.Log($"[ADJUST] eventToken={eventToken}");
                if (UnityEngine.Application.isEditor) {
                    return;
                }

                Adjust.TrackEvent(new AdjustEvent(eventToken));
            }
            else callOnAvailable += () => LogEvent(eventToken);
        }

        public const string AdAdmobMediation = "admob_sdk";
        public const string AdMaxMediation = "applovin_max_sdk";
        public const string AdIronSourceMediation = "ironsource_sdk";

        /// <summary>
        /// Track when a Ad is shown to the user
        /// </summary>
        /// <param name="adType">Placement type of the Ad. (ex. Interstitial, Reward)</param>
        /// <param name="adMediation">The provider of the Ad. (ex. `applovin_max_sdk`, `admob_sdk`, `ironsource_sdk`)</param>
        /// <param name="adNetwork">The network of the Ad. (ex. AdMob, IronSource, AppLovin, Vungle, Pangle, ...)</param>
        /// <param name="adUnitName">The Ad Unit Name or ID that was used to show the impression</param>
        /// <param name="revenue">Revenue earned from the impression shown</param>
        public void LogAdRevenue(string adType, string adMediation, string adNetwork, string adUnitName, double revenue, string currency) {
            if (sdkInitialized) {
                Logs.Log($"[ADJUST] logAdRevenue, mediation={adMediation}, network={adNetwork}, adType={adType}, adUnit={adUnitName}, revenue={revenue}");
                if (UnityEngine.Application.isEditor) {
                    return;
                }
                AdjustAdRevenue ev = new AdjustAdRevenue(adMediation);
                ev.AdRevenueNetwork = adNetwork;
                ev.AdRevenuePlacement = adType;
                ev.AdRevenueUnit = adUnitName;
                ev.SetRevenue(revenue, currency);
                Adjust.TrackAdRevenue(ev);
            }
            else callOnAvailable += () => LogAdRevenue(adType, adMediation, adNetwork, adUnitName, revenue, currency);
        }

        /// <summary>
        /// Track a purchase Event
        /// </summary>
        /// <param name="currency">The currency used for the purchase</param>
        /// <param name="revenue">The amount spent on the purchase</param>
        /// <param name="productId">The ID or name of the item purchased</param>
        public void LogPurchase(string token, string transaction, string productId, string currency, double revenue) {
            if (string.IsNullOrEmpty(token)) return;
            if (sdkInitialized) {
                Logs.Log($"[ADJUST] LogPurchase, transactionId={transaction}, productId={productId}, revenue={revenue}{currency}");
                if (UnityEngine.Application.isEditor) {
                    return;
                }

                AdjustEvent ev = new AdjustEvent(token);
                ev.TransactionId = transaction;
                ev.ProductId = productId;
                ev.SetRevenue(revenue, currency);

                if (IPSConfig.IsAndroid) {
                    Adjust.VerifyAndTrackPlayStorePurchase(ev, OnPurchaseVerifyCallback);
                }
                else Adjust.VerifyAndTrackAppStorePurchase(ev, OnPurchaseVerifyCallback);
            }
            else callOnAvailable += () => LogPurchase(token, transaction, productId, currency, revenue);
        }

        private void OnPurchaseVerifyCallback(AdjustPurchaseVerificationResult result) {
            if (result != null) {
                Logs.Log($"[ADJUST] Purcharse Verify Result: status={result.VerificationStatus}, code={result.Code}, msg={result.Message}");
            }
            else {
                Logs.Log($"[ADJUST] Purcharse Verify Result: UNKNOW");
            }
        }

    }
}
#endif