using IPS;
using IPS.Api.Ads;
using IPS.Api.Analytics;
using System;
using UnityEngine;

public enum Mediation {
    admob = 1,
    ironSource = 2,
    appLovin = 3, //Max
}

namespace IPS.Api.Analytics {
    public partial class TrackingEvent {
        [Header("MEDIATION - MMP")]
        public string MaxSDKKey = string.Empty;
        public string AppsFlyerKey = string.Empty;


        [Header("Ads - AppsFlyer Events")]
        [Header("AOA Event")]
        public string af_aoa_api_called = "af_aoa_api_called";
        public string af_aoa_displayed = "af_aoa_displayed";

        [Space()]
        [Header("NATIVE Event")]
        public string af_native_api_called = "af_native_api_called";
        public string af_native_displayed = "af_native_displayed";
        
        [Space()]
        [Header("MREC Event")]
        public string af_mrec_displayed = "af_mrec_displayed";

        [Space()]
        public string af_banner_displayed = "af_banner_displayed";

        [Space()]
        [Header("INTER Event")]
        public string af_inters_ad_eligible = "af_inters_ad_eligible";
        public string af_inters_passed_capping_time = "af_inters_passed_capping_time";
        public string af_inters_api_called = "af_inters_api_called";
        public string af_inters_load_fail = "af_inters_load_fail";
        public string af_inters_displayed = "af_inters_displayed";
        public string af_inters_displayed_xcount = "af_inters_displayed_";

        [Space()]
        [Header("REWARDED Video Event")]
        public string af_rewarded_ad_eligible = "af_rewarded_ad_eligible";
        public string af_rewarded_api_called = "af_rewarded_api_called";
        public string af_rewarded_load_fail = "af_rewarded_load_fail";
        public string af_rewarded_displayed = "af_rewarded_displayed";
        public string af_rewarded_completed = "af_rewarded_completed";
        public string af_rewarded_displayed_xcount = "af_rewarded_displayed_";
        public string af_rewarded_displayed_param_placement = "reward_type";

        [Space()]
        [Header("REWARDED INTER Event")]
        public string af_rewarded_inter_ad_eligible = "af_rewarded_inter_ad_eligible";
        public string af_rewarded_inter_api_called = "af_rewarded_inter_api_called";
        public string af_rewarded_inter_displayed = "af_rewarded_inter_displayed";
        public string af_rewarded_inter_completed = "af_rewarded_inter_completed";

        [Header("Ads - FIREBASE Events")]
        public string ad_revenue_sdk = "ad_revenue_sdk";
        public string ad_aoa_loaded = "ad_aoa_loaded";
        public string ad_aoa_show = "ad_aoa_show";
        public string ad_aoa_click = "ad_aoa_click";

        public string ad_mrec_show = "ad_mrec_show";

        public string ad_native_loaded = "ad_native_loaded";
        public string ad_native_show = "ad_native_show";
        [Space()]
        public string ad_banner_loaded = "ad_banner_loaded";
        public string ad_banner_show = "ad_banner_show";
        [Space()]
        public string ad_inter_eligible = "ad_inter_eligible";
        public string ad_inter_passed_capping_time = "ad_inter_passed_capping_time";
        public string ad_inter_loaded = "ad_inter_loaded";
        public string ad_inter_show = "ad_inter_show";        

        [Tooltip("Format: {ad_inter_show_x}{placement}")]
        public string ad_inter_show_x;
        [Tooltip("Format: {ad_inter_show_xcount}{InterShowCount}")]
        public string ad_inter_show_xcount;
        public int ad_inter_show_xcount_MaxEventCount = 30;
        public string ad_inter_click = "ad_inter_click";
        public string ad_inter_close = "ad_inter_close";

        [Space()]
        public string ad_reward_eligible = "ad_reward_eligible";
        public string ad_reward_click = "ad_reward_click";
        public string ad_reward_loaded = "ad_reward_loaded";
        public string ad_reward_show = "ad_reward_show";
        [Tooltip("Format: {ad_reward_show_x}{placement}")]
        public string ad_reward_show_x;
        [Tooltip("Format: {ad_reward_show_xcount}{RewardedVideoShowCount}")]
        public string ad_reward_show_xcount;
        public int ad_reward_show_xcount_MaxEventCount = 30;
        public string ad_reward_completed = "ad_reward_completed";

        [Space()]
        public string ad_reward_inter_eligible = "ad_reward_inter_eligible";
        public string ad_reward_inter_click = "ad_reward_inter_click";
        public string ad_reward_inter_loaded = "ad_reward_inter_loaded";
        public string ad_reward_inter_show = "ad_reward_inter_show";
        public string ad_reward_inter_show_x;
        public string ad_reward_inter_completed = "ad_reward_inter_completed";

        [Header("Ads - User properties")]
        public string last_placement = "last_placement";
        public string last_placement_inter = "last_placement_inter";
        public string total_interstitial_ads = "total_interstitial_ads";

        public string last_placement_rewarded_video = "last_placement_rewarded_video";
        public string total_rewarded_video_ads = "total_rewarded_video_ads";

        public string last_placement_rewarded_inter = "last_placement_rewarded_inter";
        public string total_rewarded_inter_ads = "total_rewarded_inter_ads";

        [Header("IPS Event")]
        public string ips_ad_aoa_loaded = "ips_ad_aoa_loaded";
        public string ips_ad_aoa_show = "ips_ad_aoa_show";

        public string ips_ad_mrec_show = "ips_ad_mrec_show";
        
        public string ips_ad_native_loaded = "ips_ad_native_loaded";
        public string ips_ad_native_show = "ips_ad_native_show";
        public string ips_ad_banner_show = "ips_ad_banner_show";

        public string ips_ad_inter_eligible = "ips_ad_inter_eligible";
        public string ips_ad_inter_passed_capping_time = "ips_ad_inter_passed_capping_time";
        public string ips_ad_inter_loaded = "ips_ad_inter_loaded";
        public string ips_ad_inter_show = "ips_ad_inter_show";
        public string ips_ad_inter_click = "ips_ad_inter_click";

        public string ips_ad_reward_eligible = "ips_ad_reward_eligible";
        public string ips_ad_reward_click = "ips_ad_reward_click";
        public string ips_ad_reward_loaded = "ips_ad_reward_loaded";
        public string ips_ad_reward_show = "ips_ad_reward_show";
        public string ips_ad_reward_completed = "ips_ad_reward_completed";

        public string ips_ad_reward_inter_eligible = "ips_ad_reward_inter_eligible";
        public string ips_ad_reward_inter_click = "ips_ad_reward_inter_click";
        public string ips_ad_reward_inter_loaded = "ips_ad_reward_inter_loaded";
        public string ips_ad_reward_inter_show = "ips_ad_reward_inter_show";
        public string ips_ad_reward_inter_completed = "ips_ad_reward_inter_completed";    
    }
}

public partial class Tracking {
    private const string ParamIsAndroid = "is_android";
    private const string ParamVersionName = "version_name";
    private const string ParamVersionCode = "version_code";
    private const string ParamErrorMsg = "errormsg";
    private const string ParamPlacement = "placement";
    private const string ParamPlacementAdReward = "placement_rw";

    private const string ad_platform = "ad_platform";
    private const string ad_source = "ad_source";
    private const string ad_unit_name = "ad_unit_name";
    private const string ad_format = "ad_format";
    private const string currencyName = "currency";
    private const string valueRevenue = "value";
        
    public int InterShowCount {
        get => PlayerPrefs.GetInt("AdInterShowCount", 0);
        set => PlayerPrefs.SetInt("AdInterShowCount", value);
    }

    private int RewardedInterShowCount {
        get => PlayerPrefs.GetInt("AdRewardedInterShowCount", 0);
        set => PlayerPrefs.SetInt("AdRewardedInterShowCount", value);
    }

    private int RewardedVideoShowCount {
        get => PlayerPrefs.GetInt("AdRewardedVideoShowCount", 0);
        set => PlayerPrefs.SetInt("AdRewardedVideoShowCount", value);
    }

    partial void OnAwakeAds() {
    }

    public void LogAdImpression(Mediation mediation, string adNetwork, string adUnitName, IPS.Api.Ads.AdSlotFormat adType, string currency, double revenue, string adCountry, string adPrecision, string placement = null, double? adLtv = null) {
        try {
            if (mediation == Mediation.admob) revenue /= 1000000f;
            ParameterBuilder builder = ParameterBuilder.Create().Add(ad_platform, mediation.ToString())
                                                                .Add(ParamPlacement, placement)
                                                                .Add(ad_source, adNetwork)
                                                                .Add(ad_unit_name, adUnitName)
                                                                .Add(ad_format, adType.ToString().ToUpper())
                                                                .Add(currencyName, !string.IsNullOrEmpty(currency) ? currency : "USD")
                                                                .Add(valueRevenue, revenue)
                                                                .Add("rev", revenue)
                                                                .Add(EventName.level_param_id, CurrentLevel)
                                                                .Add(ParamIsAndroid, IPSConfig.IsAndroid)
                                                                .Add(ParamVersionName, Application.version)
                                                                .Add(ParamVersionCode, BootstrapConfig.Instance.VersionCode);


#if APPSFLYER
            IPSAppsFlyerAnalytic.Instance.LogAdRevenue((int)mediation, adNetwork, revenue, currency, builder);
#endif

#if FIREBASE
            //IPSFirebaseAnalytic.Instance.LogAdImpression(builder);
            IPSFirebaseAnalytic.Instance.LogEvent("ad_impression", builder);
            IPSFirebaseAnalytic.Instance.LogEvent("ips_ad_impression", builder);
            IPSFirebaseAnalytic.Instance.LogEvent(EventName.ad_revenue_sdk, builder);
#endif

#if BYTEBREW
            IPSByteBrewAnalytics.Instance.LogAdRevenue(adType.ToString(), mediation.ToString(), adUnitName, revenue);
            IPSByteBrewAnalytics.Instance.LogEvent("ips_ad_impression", builder);
#endif
#if ADJUST
        LogAdjustAdImpression(adType.ToString().ToLower(), mediation, adNetwork, adUnitName, revenue, currency);
#endif
            //LogCustomService("ad_impression", builder);
            CustomService?.LogAds(adType, placement, adPrecision, adCountry, revenue, adNetwork, mediation.ToString(), adLtv);
        }
        catch (Exception e) {
            LogException(typeof(Tracking).Name, nameof(LogAdImpression), $"mediation={mediation}, adNetwork={adNetwork}, adType={adType}, revenue={revenue}, errmsg={(e != null ? e.Message : string.Empty)}");
        }
    }

            #region Event for Ads
        /// <summary> Trigger when inter ad loaded callback </summary>
        public void LogAdAOAAvailable() {
            LogEvent(EventName.af_aoa_api_called, service: Service.AppsFlyer);
            LogEvent(EventName.ad_aoa_loaded, service: Service.AllExceptAppsFlyerAdjustFacebook);
            LogEvent(EventName.ips_ad_aoa_loaded, service: Service.AllExceptAppsFlyerAdjustFacebook);
        }

        /// <summary> Trigger when inter ad displayed callback </summary>
        public void LogAdAOADisplayed(string placement, Mediation mediation) {
            LogEvent(EventName.af_aoa_displayed, service: Service.AppsFlyer);
            LogEvent(EventName.ad_aoa_show, ParameterBuilder.Create(ParamPlacement, placement).Add(EventName.level_param_id, CurrentLevel).Add(ad_platform, mediation.ToString()), service: Service.AllExceptAppsFlyerAdjustFacebook);
            LogEvent(EventName.ips_ad_aoa_show, ParameterBuilder.Create(ParamPlacement, placement).Add(EventName.level_param_id, CurrentLevel).Add(ad_platform, mediation.ToString()), service: Service.AllExceptAppsFlyerAdjustFacebook);
        }

        /// <summary> Trigger when inter ad loaded callback </summary>
        public void LogAdNativeAvailable() {
            LogEvent(EventName.af_native_api_called, service: Service.AppsFlyer);
            LogEvent(EventName.ad_native_loaded, service: Service.AllExceptAppsFlyerAdjustFacebook);
            LogEvent(EventName.ips_ad_native_loaded, service: Service.AllExceptAppsFlyerAdjustFacebook);
        }

        /// <summary> Trigger when inter ad displayed callback </summary>
        public void LogAdNativeDisplayed(string placement) {
            LogEvent(EventName.af_native_displayed, service: Service.AppsFlyer);
            LogEvent(EventName.ad_native_show, ParameterBuilder.Create(ParamPlacement, placement).Add(EventName.level_param_id, CurrentLevel).Add(ad_platform, Mediation.admob.ToString()), service: Service.AllExceptAppsFlyerAdjustFacebook);
            LogEvent(EventName.ips_ad_native_show, ParameterBuilder.Create(ParamPlacement, placement).Add(EventName.level_param_id, CurrentLevel).Add(ad_platform, Mediation.admob.ToString()), service: Service.AllExceptAppsFlyerAdjustFacebook);
        }

        /// <summary> Trigger when MREC ad displayed callback </summary>
        public void LogAdMRecDisplayed(string placement, Mediation mediation) {
            LogEvent(EventName.af_mrec_displayed, service: Service.AppsFlyer);
            LogEvent(EventName.ad_mrec_show, ParameterBuilder.Create(ParamPlacement, placement).Add(EventName.level_param_id, CurrentLevel).Add(ad_platform, mediation.ToString()), Service.AllExceptAppsFlyerAdjustFacebook);
            LogEvent(EventName.ips_ad_mrec_show, ParameterBuilder.Create(ParamPlacement, placement).Add(EventName.level_param_id, CurrentLevel).Add(ad_platform, mediation.ToString()), Service.AllExceptAppsFlyerAdjustFacebook);
        }

        /// <summary> Trigger when banner ad displayed callback </summary>
        public void LogAdBannerDisplayed(string placement, Mediation mediation) {
            LogEvent(EventName.af_banner_displayed, service: Service.AppsFlyer);
            LogEvent(EventName.ad_banner_show, ParameterBuilder.Create(ParamPlacement, placement).Add(EventName.level_param_id, CurrentLevel).Add(ad_platform, mediation.ToString()), Service.AllExceptAppsFlyerAdjustFacebook);
            LogEvent(EventName.ips_ad_banner_show, ParameterBuilder.Create(ParamPlacement, placement).Add(EventName.level_param_id, CurrentLevel).Add(ad_platform, mediation.ToString()), Service.AllExceptAppsFlyerAdjustFacebook);
        }

        /// <summary> Trigger when call show inter (by logic game, include ad not available and no-ad had bought) </summary>
        public void LogAdInterEligible(string placement, bool canshow, int playtimesCanShow, int levelCanShow) {
            var param = ParameterBuilder.Create(ParamPlacement, placement);
            param.Add("version", Application.version)
                .Add("build_code", BootstrapConfig.Instance.VersionCode)
                .Add("canshow", canshow);

            if (!canshow) {
                param.Add("ad_playtimes_canshow", playtimesCanShow)
                    .Add("ad_level_canshow", levelCanShow)
                    .Add("user_playtimes", UserData.PlayTimes)
                    .Add("user_current_level", Tracking.CurrentLevel)
                    .Add("has_noad", AdsManager.Instance.IsRemovedAds);                    
            }

            LogAdjust(EventName.aj_inters_show);
            LogEvent(EventName.af_inters_ad_eligible, service: Service.AppsFlyer);
            LogEvent(EventName.ad_inter_eligible, param, service: Service.AllExceptAppsFlyerAdjustFacebook);
            LogEvent(EventName.ips_ad_inter_eligible, param, service: Service.AllExceptAppsFlyerAdjustFacebook);
        }

        /// <summary>
        /// Trigger when call show inter egilible with passed capping time
        /// </summary>
        public void LogAdInterPassedCappingTime(string placement) {
            LogEvent(EventName.af_inters_passed_capping_time, service: Service.AppsFlyer);
            LogEvent(EventName.ad_inter_passed_capping_time, ParamPlacement, placement, service: Service.AllExceptAppsFlyerAdjustFacebook);
            LogEvent(EventName.ips_ad_inter_passed_capping_time, ParamPlacement, placement, service: Service.AllExceptAppsFlyerAdjustFacebook);
        }

        /// <summary> Trigger when inter ad loaded callback </summary>
        public void LogAdInterAvailable() {
            LogEvent(EventName.af_inters_api_called, service: Service.AppsFlyer);
            LogEvent(EventName.ad_inter_loaded, service: Service.AllExceptAppsFlyerAdjustFacebook);
            LogEvent(EventName.ips_ad_inter_loaded, service: Service.AllExceptAppsFlyerAdjustFacebook);
        }

        /// <summary> Trigger when inter ad displayed callback </summary>
        public void LogAdInterDisplayed(string placement, Mediation mediation) {
            InterShowCount++;
            int count = InterShowCount;
            if (count <= 20) {
                LogEvent($"{EventName.af_inters_displayed_xcount}{count}", ParamPlacement, placement, service: Service.AppsFlyer);
            }

            LogAdjust(EventName.aj_inters_displayed);
            LogEvent(EventName.af_inters_displayed, ParamPlacement, placement, service: Service.AppsFlyer);
            LogEvent($"{EventName.ad_inter_show_x}{placement}", service: Service.Firebase);
            if (count <= EventName.ad_inter_show_xcount_MaxEventCount) {
                LogEvent($"{EventName.ad_inter_show_xcount}{count}", service: Service.Firebase);
            }
            LogEvent(EventName.ad_inter_show, ParameterBuilder.Create(ParamPlacement, placement).Add(EventName.level_param_id, CurrentLevel).Add(ad_platform, mediation.ToString()), service: Service.AllExceptAppsFlyerAdjustFacebook );
            LogEvent(EventName.ips_ad_inter_show, ParameterBuilder.Create(ParamPlacement, placement).Add(EventName.level_param_id, CurrentLevel).Add(ad_platform, mediation.ToString()), service: Service.AllExceptAppsFlyerAdjustFacebook );
            SetUserProperty(EventName.last_placement_inter, placement);
            SetUserProperty(EventName.last_placement, placement);
            SetUserProperty(EventName.total_interstitial_ads, count.ToString());
        }

        public void LogAdInterClicked(string placement) {            
            LogEvent(EventName.ad_inter_click, ParamPlacement, placement, service: Service.AllExceptAppsFlyerAdjustFacebook);
            LogEvent(EventName.ips_ad_inter_click, ParamPlacement, placement, service: Service.AllExceptAppsFlyerAdjustFacebook);
        }

        public void LogAdInterClosed(string placement) {
            if (InterShowCount == 1) LogEvent("ftue_inter_close", ParamPlacement, placement, service: Service.AllExceptAppsFlyerAdjustFacebook);
            LogEvent(EventName.ad_inter_close, ParamPlacement, placement, service: Service.AllExceptAppsFlyerAdjustFacebook);
        }

        /// <summary> Trigger when call show reward (by logic game, include ad not available) </summary>
        public void LogAdRewardVideoEligible(string placement) {
            var param = ParameterBuilder.Create(ParamPlacement, placement).Add(ParamPlacementAdReward, placement);
            param.Add(EventName.level_param_id, CurrentLevel).Add("version", Application.version).Add("build_code", BootstrapConfig.Instance.VersionCode);
            LogAdjust(EventName.aj_rewarded_show);
            LogEvent(EventName.af_rewarded_ad_eligible, service: Service.AppsFlyer);
            LogEvent(EventName.ad_reward_eligible, param, service: Service.AllExceptAppsFlyerAdjustFacebook);
            LogEvent(EventName.ips_ad_reward_eligible, param, service: Service.AllExceptAppsFlyerAdjustFacebook);
        }

        /// <summary> Trigger when reward ad loaded callback </summary>
        public void LogAdRewardVideoAvailable() {
            LogEvent(EventName.af_rewarded_api_called, service: Service.AppsFlyer);
            LogEvent(EventName.ad_reward_loaded, service: Service.AllExceptAppsFlyerAdjustFacebook);
            LogEvent(EventName.ips_ad_reward_loaded, service: Service.AllExceptAppsFlyerAdjustFacebook);
        }

        /// <summary> Trigger when reward ad displayed callback </summary>
        public void LogAdRewardVideoDisplayed(string placement, Mediation mediation) {
            RewardedVideoShowCount++;
            int count = RewardedVideoShowCount;
            if (count <= 20) {
                LogEvent($"{EventName.af_rewarded_displayed_xcount}{count}", ParamPlacement, placement, service: Service.AppsFlyer);
            }

            var param = ParameterBuilder.Create(ParamPlacement, placement).Add(ParamPlacementAdReward, placement).Add(ad_platform, mediation.ToString());
            param.Add(EventName.level_param_id, CurrentLevel).Add("version", Application.version).Add("build_code", BootstrapConfig.Instance.VersionCode);

            LogAdjust(EventName.aj_rewarded_displayed);
            LogEvent(EventName.af_rewarded_displayed, ParameterBuilder.Create(EventName.af_rewarded_displayed_param_placement, placement).Add(EventName.level_param_id, currentLevelId), service: Service.AppsFlyer);

            LogEvent($"{EventName.ad_reward_show_x}{placement}", service: Service.Firebase);
            if (count <= EventName.ad_reward_show_xcount_MaxEventCount) {
                LogEvent($"{EventName.ad_reward_show_xcount}{count}", service: Service.Firebase);
            }
            LogEvent(EventName.ad_reward_show, ParameterBuilder.Create(EventName.af_rewarded_displayed_param_placement, placement).Add(EventName.level_param_id, CurrentLevel), service: Service.AllExceptAppsFlyerAdjustFacebook);
            LogEvent(EventName.ips_ad_reward_show, param, service: Service.AllExceptAppsFlyerAdjustFacebook);
            SetUserProperty(EventName.last_placement_rewarded_video, placement);
            SetUserProperty(EventName.last_placement, placement);
            SetUserProperty(EventName.total_rewarded_video_ads, count.ToString());
        }

        public void LogAdRewardVideoCompleted(string placement) {
            var param = ParameterBuilder.Create(ParamPlacement, placement).Add(ParamPlacementAdReward, placement).Add(ParamLevelId, CurrentLevel);

            LogEvent(EventName.af_rewarded_completed, ParamPlacement, placement, service: Service.AppsFlyer);
            LogEvent(EventName.ad_reward_completed, param, service: Service.AllExceptAppsFlyerAdjustFacebook);
            LogEvent(EventName.ips_ad_reward_completed, param, service: Service.AllExceptAppsFlyerAdjustFacebook);
        }

        /// <summary> Trigger when call show reward inter (by logic game, include ad not available) </summary>
        public void LogAdRewardInterEligible(string placement) {
            var param = ParameterBuilder.Create(ParamPlacement, placement).Add(ParamPlacementAdReward, placement);

            LogEvent(EventName.af_rewarded_inter_ad_eligible, service: Service.AppsFlyer);
            LogEvent(EventName.ad_reward_inter_eligible, param, service: Service.AllExceptAppsFlyerAdjustFacebook);
            LogEvent(EventName.ips_ad_reward_inter_eligible, param, service: Service.AllExceptAppsFlyerAdjustFacebook);
        }

            /// <summary> Trigger when reward interstitial ad loaded callback </summary>
        public void LogAdRewardInterAvailable() {
            LogEvent(EventName.af_rewarded_inter_api_called, service: Service.AppsFlyer);
            LogEvent(EventName.ad_reward_inter_loaded, service: Service.AllExceptAppsFlyerAdjustFacebook);
            LogEvent(EventName.ips_ad_reward_inter_loaded, service: Service.AllExceptAppsFlyerAdjustFacebook);
        }

        /// <summary> Trigger when reward ad displayed callback </summary>
        public void LogAdRewardInterDisplayed(string placement, Mediation mediation) {
            RewardedInterShowCount++;
            int count = RewardedInterShowCount;
            if (count <= 20) {
                LogEvent($"af_rewarded_inter_displayed_{count}_times", ParamPlacement, placement, service: Service.AppsFlyer);
            }

            var param = ParameterBuilder.Create(ParamPlacement, placement).Add(EventName.level_param_id, CurrentLevel).Add(ParamPlacementAdReward, placement).Add(ad_platform, mediation.ToString());

            LogEvent(EventName.af_rewarded_inter_displayed, ParamPlacement, placement, service: Service.AppsFlyer);
            LogEvent($"{EventName.ad_reward_inter_show_x}{placement}", service: Service.Firebase);
            LogEvent(EventName.ad_reward_inter_show, param, service: Service.AllExceptAppsFlyerAdjustFacebook);
            LogEvent(EventName.ips_ad_reward_inter_show, param, service: Service.AllExceptAppsFlyerAdjustFacebook);
            SetUserProperty(EventName.last_placement_rewarded_inter, placement);
            SetUserProperty(EventName.last_placement, placement);
            SetUserProperty(EventName.total_rewarded_inter_ads, count.ToString());
        }

        public void LogAdRewardInterCompleted(string placement) {
            var param = ParameterBuilder.Create(ParamPlacement, placement).Add(ParamPlacementAdReward, placement);

            LogEvent(EventName.af_rewarded_inter_completed, ParamPlacement, placement, service: Service.AppsFlyer);
            LogEvent(EventName.ad_reward_inter_completed, param, service: Service.AllExceptAppsFlyerAdjustFacebook);
            LogEvent(EventName.ips_ad_reward_inter_completed, param, service: Service.AllExceptAppsFlyerAdjustFacebook);
        }

        public void LogAdFailedToLoad(AdSlotFormat adType, string err) {
            if (err.Length > 90) err = err.Substring(0, 80);
            if (adType == AdSlotFormat.Inter && !string.IsNullOrEmpty(EventName.af_inters_load_fail)) {
                LogEvent(EventName.af_inters_load_fail, ParamErrorMsg, err, Tracking.Service.AppsFlyer);
            }
            else if (adType == AdSlotFormat.Reward && !string.IsNullOrEmpty(EventName.af_inters_load_fail)) {
                LogEvent(EventName.af_rewarded_load_fail, ParamErrorMsg, err, Tracking.Service.AppsFlyer);
            }
            else {
                LogEvent($"af_{adType.ToString().ToLower()}_api_failed", ParamErrorMsg, err, Tracking.Service.AppsFlyer);
            }

            string adsType = adType == AdSlotFormat.AOA ? "Open" : adType.ToString();
            LogEvent($"ads_{adsType}_fail", ParamErrorMsg, err, Tracking.Service.AllExceptAppsFlyerAdjustFacebook);
            LogEvent($"ips_ad_{adType.ToString().ToLower()}_load_fail", ParamErrorMsg, err, Tracking.Service.AllExceptAppsFlyerAdjustFacebook);

        }

        public void LogAdFailedToShow(AdSlotFormat adType, string placement, string err) {
            if (err.Length > 90) err = err.Substring(0, 80);
        var param = ParameterBuilder.Create(ParamPlacement, placement).Add(ParamErrorMsg, err)
                                    .Add(ParamIsAndroid, IPSConfig.IsAndroid)
                                    .Add(ParamVersionName, Application.version)
                                    .Add(ParamVersionCode, BootstrapConfig.Instance.VersionCode);

            if (adType == AdSlotFormat.Reward || adType == AdSlotFormat.RewardInter) {
                param.Add(ParamPlacementAdReward, placement);
            }

            LogEvent($"af_{adType.ToString().ToLower()}_display_failed", param, Tracking.Service.AppsFlyer);
            LogEvent($"ips_ad_{adType.ToString().ToLower()}_show_fail", param, Tracking.Service.AllExceptAppsFlyerAdjustFacebook);

            string adsType = adType == AdSlotFormat.AOA ? "Open" : adType.ToString();
            LogEvent($"ads_{adsType}_show_fail", param, Tracking.Service.AllExceptAppsFlyerAdjustFacebook);
        }
        #endregion

}