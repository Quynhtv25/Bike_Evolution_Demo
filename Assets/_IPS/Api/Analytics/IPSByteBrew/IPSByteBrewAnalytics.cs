#if BYTEBREW

using ByteBrewSDK;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IPS.Api.Analytics {
    public class IPSByteBrewAnalytics : SingletonBehaviourDontDestroy<IPSByteBrewAnalytics> {
        Action callOnAvailable;
        bool sdkInitialized;

#if UNITY_EDITOR
        [UnityEditor.MenuItem("IPS/Api/Analytics/ByteBrewSettings")]
        public static void SelectSettings() {
            UnityEditor.Selection.activeObject = Resources.Load<ByteBrewSettings>($"{typeof(ByteBrewSettings).Name}");
        }
#endif

        protected override void OnAwake() {
            Debug.Log($"[BYTEBREW] Prepare Initialize..");
            gameObject.AddComponent<ByteBrew>();
        }

        private void Start() {
            ByteBrew.InitializeByteBrew();
            StartCoroutine(IEWaitForSDKInit());
        }

        private IEnumerator IEWaitForSDKInit() {
            yield return new WaitUntil(() => ByteBrew.IsInitilized || ByteBrew.IsByteBrewInitialized());
            sdkInitialized = true;
            Debug.Log($"[BYTEBREW] Initialized");
            if (callOnAvailable != null) {
                callOnAvailable.Invoke();
                callOnAvailable = null;
            }
        }

        public void SetUserProperty(string propertyName, string propertyValue) {
            if (sdkInitialized) {
                if (UnityEngine.Application.isEditor) {
                    Logs.Log($"[BYTEBREW] SetUserProperty propertyName={propertyName}, propertyValue={propertyValue}");
                    return;
                }

                Logs.Log($"[BYTEBREW] SetUserProperty propertyName={propertyName}, propertyValue={propertyValue}");
                ByteBrew.SetCustomUserDataAttribute(propertyName, propertyValue);
            }
            else callOnAvailable += () => SetUserProperty(propertyName, propertyValue);
        }

        public void LogEvent(string eventName) {
            this.Log(eventName, null);
        }

        public void LogEvent(string eventName, float value) {
            if (sdkInitialized) {
                if (UnityEngine.Application.isEditor) {
                    Logs.Log($"[BYTEBREW] eventName={eventName}, floatValue={value}");
                    return;
                }

                Logs.Log($"[BYTEBREW] eventName={eventName}, floatValue={value}");
                ByteBrew.NewCustomEvent(eventName, value);
            }
            else callOnAvailable += () => LogEvent(eventName, value);
        }

        public void LogEvent(string eventName, string value) {
            if (sdkInitialized) {
                if (UnityEngine.Application.isEditor) {
                    Logs.Log($"[BYTEBREW] eventName={eventName}, stringValue={value}");
                    return;
                }

                Logs.Log($"[BYTEBREW] eventName={eventName}, stringValue={value}");
                ByteBrew.NewCustomEvent(eventName, value);
            }
            else callOnAvailable += () => LogEvent(eventName, value);
        }

        public void LogEvent(string eventName, string paraName, string paraValue) {
            this.Log(eventName, new Dictionary<string, string>() { { paraName, paraValue } });
        }

        public void LogEvent(string eventName, ParameterBuilder parameterBuilder) {
            this.Log(eventName, parameterBuilder != null ? parameterBuilder.BuildDictString() : null);
        }

        private void Log(string eventName, Dictionary<string, string> para) {
            if (sdkInitialized) {
                Logs.Log($"[BYTEBREW] eventName={eventName}, paraCount={(para != null ? para.Count : 0)}");
                if (UnityEngine.Application.isEditor) {
                    return;
                }

                if (para != null && para.Count > 0) {
                    ByteBrew.NewCustomEvent(eventName, para);
                }
                else ByteBrew.NewCustomEvent(eventName);
            }
            else callOnAvailable += () => Log(eventName, para);
        }

        /// <summary>
        /// Track when a Ad is shown to the user
        /// </summary>
        /// <param name="adType">Placement type of the Ad. (ex. Interstitial, Reward)</param>
        /// <param name="adProvider">The provider of the Ad. (ex. AdMob, IronSource)</param>
        /// <param name="adUnitName">The Ad Unit Name or ID that was used to show the impression</param>
        /// <param name="revenue">Revenue earned from the impression shown</param>
        public void LogAdRevenue(string adType, string adProvider, string adUnitName, double revenue) {
            if (sdkInitialized) {
                Logs.Log($"[BYTEBREW] logAdRevenue, mediation={adProvider}, adType={adType}, adUnit={adUnitName}, revenue={revenue}");
                if (UnityEngine.Application.isEditor) {
                    return;
                }

                if (ByteBrew.IsInitilized) ByteBrew_Helper.NewTrackedAdEvent(placementType: adType, adProvider: adProvider, adUnitName: adUnitName, revenue: revenue);
            }
            else callOnAvailable += () => LogAdRevenue(adType, adProvider, adUnitName, revenue);
        }

        /// <summary>
        /// Track a purchase Event
        /// </summary>
        /// <param name="currency">The currency used for the purchase</param>
        /// <param name="revenue">The amount spent on the purchase</param>
        /// <param name="productId">The ID or name of the item purchased</param>
        /// <param name="category">The name of the category item was purchased (ex. currency)</param>
        public void LogPurchase(string productId, string currency, double revenue, string category) {
            if (sdkInitialized) {
                Logs.Log($"[BYTEBREW] LogPurchase, productId={productId}, category={category}, revenue={revenue}{currency}");
                if (UnityEngine.Application.isEditor) {
                    return;
                }

                string store = IPSConfig.IsAndroid ? "Google App Store" : "Apple App Store";
                ByteBrew.TrackInAppPurchaseEvent(store, currency, (float)revenue, productId, category);
            }
            else callOnAvailable += () => LogPurchase(productId, currency, revenue, category);
        }

        /// <summary>
        /// Add a progression event
        /// </summary>
        /// <param name="progressionStatus">Type of progression (0: Start, 1: Fail, 2: Completed)</param>
        /// <param name="environment">The environment that the event is happening in (ex. Tutorial, Level)</param>
        /// <param name="stage">Stage or progression that the player is in (ex. GoldLevelArena, Level_1, tutorial_menu_purchase)</param>
        /// <param name="value">Value that ties to an event (ex. 500, -300, Chainsaw) </param>
        private void LogProgress(ByteBrewProgressionTypes progressionStatus, string environment, string stage, string value = default) {
            if (sdkInitialized) {
                Logs.Log($"[BYTEBREW] LogProgress, status={progressionStatus}, enviroment={environment}, stage={stage}, value={value}");
                if (UnityEngine.Application.isEditor) {
                    return;
                }

                if (!string.IsNullOrEmpty(value)) ByteBrew.NewProgressionEvent(progressionStatus, environment, stage, value);
                else ByteBrew.NewProgressionEvent(progressionStatus, environment, stage);
            }
            else callOnAvailable += () => LogProgress(progressionStatus, environment, stage, value);
        }

        /// <summary>
        /// Add a progression event
        /// </summary>
        /// <param name="environment">The environment that the event is happening in (ex. Tutorial, Level)</param>
        /// <param name="stage">Stage or progression that the player is in (ex. GoldLevelArena, Level_1, tutorial_menu_purchase)</param>
        /// <param name="value">Value that ties to an event (ex. 500, -300, Chainsaw) </param>
        public void LogProgressStart(string enviroment, string stage, string value = default) {
            LogProgress(ByteBrewProgressionTypes.Started, enviroment, stage, value);
        }
        
        /// <summary>
        /// Add a progression event
        /// </summary>
        /// <param name="environment">The environment that the event is happening in (ex. Tutorial, Level)</param>
        /// <param name="stage">Stage or progression that the player is in (ex. GoldLevelArena, Level_1, tutorial_menu_purchase)</param>
        /// <param name="value">Value that ties to an event (ex. 500, -300, Chainsaw) </param>
        public void LogProgressComplete(string enviroment, string stage, string value = default) {
            LogProgress(ByteBrewProgressionTypes.Completed, enviroment, stage, value);
        }
        
        /// <summary>
        /// Add a progression event
        /// </summary>
        /// <param name="environment">The environment that the event is happening in (ex. Tutorial, Level)</param>
        /// <param name="stage">Stage or progression that the player is in (ex. GoldLevelArena, Level_1, tutorial_menu_purchase)</param>
        /// <param name="value">Value that ties to an event (ex. 500, -300, Chainsaw) </param>
        public void LogProgressFail(string enviroment, string stage, string value = default) {
            LogProgress(ByteBrewProgressionTypes.Failed, enviroment, stage, value);
        }
    }
}
#endif