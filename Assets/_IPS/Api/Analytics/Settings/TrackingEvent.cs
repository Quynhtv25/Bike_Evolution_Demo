
using UnityEngine;

namespace IPS.Api.Analytics {
    [CreateAssetMenu(fileName = "TrackingEvent", menuName = "IPS/Api/Analytics/TrackingEvent")]
    public partial class TrackingEvent : ScriptableObject {
        [Header("Gameplay - User Property")]
        public string property_days_played = "days_played";
        public string property_retention_day = "retention_day";
        public string property_level_max = "level_max";
        public string property_current_level = "level";
        public string property_last_level = "last_level";

        [Header("Adjust Config")]
        public string aj_inters_show;
        public string aj_inters_displayed;
        public string aj_rewarded_show;
        public string aj_rewarded_displayed;
        public string aj_purchase;
        public string aj_content_id;
        public string aj_purchase_orders;
        public string aj_currency_code;
        public string aj_level_start;
        public string aj_level_achieved;
        public string aj_level_fail;

        [Header("Gameplay - AppsFlyer")]
        public string af_tutorial_completion = "af_tutorial_completion";
        public string af_level_start = "af_level_start";
        public string af_level_achieved = "af_level_achieved";
        public string af_level_win = "af_level_win";
        public string af_level_lose = "af_level_lose";
        public string af_complete_level_x = "completed_level_";
        public string af_level_param_id = "lv";

        [Header("Gameplay - Firebase")]
        public string start_level_x = "start_level_";
        public string win_level_x = "win_level_";
        public string lose_level_x = "lose_level_";
        public string checkpoint_x = "checkpoint_";

        [Space()]
        public string open_app;
        public string loading_start;
        public string loading_end;
        public string level_start_int = "level_start";
        public string level_revive_int = "level_revive";
        public string level_end_int = "level_end";
        public string level_start_string = "game_start";
        public string level_end_string = "game_end";

        [Space()]
        [Tooltip("Use for level int")]
        public string win_level = "level_passed";
        public string lose_level = "level_failed"; 
        public string level_param_duration = "timeplayed"; 
        public string level_param_id = "level";
        public string level_param_mode_id = "mode";
        public string level_param_timestamp;
        [Space()]
        public string button_click_x;
        public string frame_show;
        public string frame_show_param;
        public string use_currency = "use_currency";
        public string use_currency_param_name = "name";
        public string use_booster = "use_booster";
        public string use_booster_param_name = "name";

        [Header("IAP")]
        public string iap_sdk;
        public float IapRevenueRatio = 0.63f;
    }

     public class CustomService : ScriptableObject {
        public virtual string UserId => string.Empty;
        public virtual void LogLevelStart(int lv) { }
        public virtual void LogLevelEnd(int lv, string difficulty, bool win, bool retry, double durations, int? wave, int score = 0, bool quitIngame = false, string reason = null){ }
        public virtual void LogAds(IPS.Api.Ads.AdSlotFormat adFormat, string placement, string adPrecision, string adCountry, double adRev, string adNetwork, string adMediation, double? adLtv = null) { }
        public virtual void LogIap(string productId, decimal localizePrice, string isoCurrencyCode, string placement, string transactionId){ }
        public virtual void LogCustomEvent(string eventName, ParameterBuilder param){ }
        public virtual void LogUserProperty(string propertyName, string propertyValue){ }
    }
}