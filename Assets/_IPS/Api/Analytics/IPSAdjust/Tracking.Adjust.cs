

using IPS.Api.Analytics;

public partial class Tracking {
    public void LogAdjust(string eventToken) {
#if ADJUST
        IPSAdjustAnalytics.Instance.LogEvent(eventToken);
#endif
    }

    public void LogAdjustAdImpression(string adType, Mediation adMediation, string adNetwork,  string adUnitName, double revenue, string currency) {
#if ADJUST
        IPSAdjustAnalytics.Instance.LogAdRevenue(adType, GetAdMediation(adMediation) , adNetwork, adUnitName, revenue, currency);
#endif
    }

    private string GetAdMediation(Mediation mediation) {
#if ADJUST
        if (mediation == Mediation.admob) return IPSAdjustAnalytics.AdAdmobMediation;
        if (mediation == Mediation.appLovin) return IPSAdjustAnalytics.AdMaxMediation;
        if (mediation == Mediation.ironSource) return IPSAdjustAnalytics.AdIronSourceMediation;
#endif
        return null;
    }

    public void LogAdjustPurchaseEvent(string eventToken, string transactionId, string productId, double revenue, string currency) {
#if ADJUST
        IPSAdjustAnalytics.Instance.LogPurchase(eventToken, transactionId, productId, currency, revenue);
#endif
    }
}