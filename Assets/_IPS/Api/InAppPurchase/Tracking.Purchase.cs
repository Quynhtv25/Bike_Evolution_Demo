using IPS;
using IPS.Api.Analytics;
using System;
using UnityEngine;

public partial class Tracking {
    private const string ParamProductId = "product_id";
    private const string ParamTransactionId = "transactionId";

    partial void OnAwakePurchase() {

    }

    public void LogPurchaseEligible(string packId, string placement) {
        LogEvent("purchase_eligible", ParameterBuilder.Create(ParamProductId, packId).Add(ParamPlacement, placement).Add("level", CurrentLevel));
    }
    
    public void LogPurchaseStart(string packId, string placement) {
        LogEvent("purchase_start", ParameterBuilder.Create(ParamProductId, packId).Add(ParamPlacement, placement));
    }

    public void LogPurchaseFailure(string packId, string transactionId, string errorMsg) {
        if (errorMsg.Length > 99) errorMsg = errorMsg.Substring(0, 99);
        LogEvent("purchase_fail", ParameterBuilder.Create(ParamProductId, packId).Add(ParamTransactionId, transactionId).Add(ParamErrorMsg, errorMsg));
    }

    public void LogPurchaseSuccess(string packId, string currency, double revenue, string transactionId) {
        try {
            revenue *= EventName.IapRevenueRatio;
            var param = ParameterBuilder.Create(ParamProductId, packId)
                                        .Add("currency", currency)
                                        .Add("value", revenue)
                                        .Add(ParamTransactionId, transactionId)
                                        .Add(EventName.level_param_id, CurrentLevel)
                                        .Add(ParamIsAndroid, IPSConfig.IsAndroid)
                                        .Add(ParamVersionName, Application.version)
                                        .Add(ParamVersionCode, BootstrapConfig.Instance.VersionCode);

#if APPSFLYER
            IPSAppsFlyerAnalytic.Instance.LogPurchase(packId, currency, revenue);
#endif

#if FIREBASE
            //IPSFirebaseAnalytic.Instance.LogPurchase(packId, currency, revenue);
            IPSFirebaseAnalytic.Instance.LogEvent("purchase", param);
            IPSFirebaseAnalytic.Instance.LogEvent(EventName.iap_sdk, param);
#endif

#if BYTEBREW
            IPSByteBrewAnalytics.Instance.LogPurchase(packId, currency, revenue, string.Empty);
            IPSByteBrewAnalytics.Instance.LogEvent("ips_purchase", param);
#endif
#if ADJUST
            IPSAdjustAnalytics.Instance.LogPurchase(EventName.aj_purchase, transactionId, packId, currency, revenue);
#endif

            if (CustomService != null) {
                var meta = IAP.Instance.GetLocalPrice(packId);
                if (meta != null) {
                    CustomService?.LogIap(packId, meta.localizedPrice, meta.isoCurrencyCode, string.Empty, transactionId);
                }
            }
        }
        catch (Exception e) {
            LogException(typeof(Tracking).Name, nameof(LogPurchaseSuccess), $"packId={packId}, transactionId={transactionId}, errmsg={(e != null ? e.Message : string.Empty)}");
        }
    }

}