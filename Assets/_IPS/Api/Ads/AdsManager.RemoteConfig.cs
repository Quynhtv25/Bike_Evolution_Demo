
using IPS.Api.Ads;
using IPS;
using System;
using UnityEngine;
using Unity.VisualScripting;
using IPS.Api.RemoteConfig;

namespace IPS.Api.RemoteConfig {
    public partial class RemoteKey {
        [Header("---- ADS CONFIG -----")]
        public string ad_test_devices_android = "ad_test_devices_android";
        public string ad_test_devices_ios = "ad_test_devices_ios";

        [Header("UMP")]
        public string admob_UMP_enable = "admob_UMP_enable";

        [Header("Ads resume")]
        public string ad_resume_enable = "ad_resume_enable";
        public string ad_resume_from_played_times = "ad_resume_from_played_times";
        public string ad_resume_capping = "ad_resume_capping";
        public string ad_aoa_resume_enable = "ad_aoa_resume_enable";
        public string ad_inter_resume_enable = "ad_inter_resume_enable";

        [Header("Ads AOA")]
        public string ad_aoa_openapp_first_install = "ad_aoa_openapp_first_install";
        public string ad_aoa_openapp = "ad_aoa_openapp";
        public string ad_aoa_max_enable = "ad_aoa_max_enable";

        [Header("Ads inter")]
        public string ad_inter_enable = "ad_inter_enable";
        public string ad_inter_from_played_times = "ad_inter_from_played_times";
        [Tooltip("Interstitial only show after both condition of level & played times & seconds")]
        public string ad_inter_from_level = "ad_inter_from_level";
        public string ad_inter_from_seconds = "ad_inter_from_seconds";
        public string ad_inter_capping = "ad_inter_capping";
        public string ad_inter_after_reward_capping = "ad_inter_after_reward_capping";

        [Tooltip("Force inter after x seconds, usefull incase endless level or level need very long time to finish")]
        public string ad_inter_force_enable = "ad_inter_force_enable";
        public string ad_inter_force_capping = "ad_inter_force_capping";

        [Header("Ads banner")]
        public string ad_banner_enable = "ad_banner_enable";
        public string ad_banner_adaptive_enable = "ad_banner_adaptive_enable";
        public string ad_banner_collapsible_enable = "ad_banner_collapsible_enable";
        public string ad_banner_collapsible_fallback_enable = "ad_banner_collapsible_fallback_enable";
        public string ad_banner_collapsible_auto_refresh = "ad_banner_collapsible_auto_refresh";
        public string ad_banner_collapsible_reload_interval = "ad_banner_collapsible_reload_interval";
        public string ad_banner_from_played_times = "ad_banner_from_played_times";
        public string ad_banner_from_level = "ad_banner_from_level";
        public string ad_banner_reload_capping = "ad_banner_reload_capping";
        public string ad_banner_reload_level_capping = "ad_banner_reload_level_capping";

        [Header("Ads MRec")]
        public string ad_mrec_enable = "ad_mrec_enable";
    }
}

namespace IPS {
    public partial class Bootstrap {
#if ADS && REMOTE
        partial void FetchRemoteConfigAds() {
            var key = RemoteKey.Instance;
            var adSettings = AdmobSettings.Instance;
            adSettings.EnableLog = IPSConfig.LogEnable;

            FetchBool(key.admob_UMP_enable, adSettings.EnableUMP, (value) => {
                if (!AdmobSettings.Instance.AdsRemoteConfigFetched) {
                    AdmobSettings.Instance.EnableUMP = value;
                    AdmobSettings.Instance.AdsRemoteConfigFetched = true;
                }
            });
            FetchString(key.ad_test_devices_android, adSettings.TestDevice_android, (value) => adSettings.TestDevice_android = value);
            FetchString(key.ad_test_devices_ios, adSettings.TestDevice_iOS, (value) => adSettings.TestDevice_iOS = value);

            FetchBool(key.ad_resume_enable, adSettings.AdResumeEnable, (value) => {
                adSettings.AdResumeEnable = value;
                if (value) {
                    FetchLong(key.ad_resume_from_played_times, (long)adSettings.AdsResumeFromPlayedTime, (value) => adSettings.AdsResumeFromPlayedTime = (ulong)value);
                    FetchLong(key.ad_resume_capping, adSettings.AdsResumeCapping, (value) => adSettings.AdsResumeCapping = (int)value);
                }
            });

            FetchRemoteConfig(adSettings);
#if IS
        FetchRemoteConfig(ISSettings.Instance);
#elif MAX
            FetchRemoteConfig(MaxSettings.Instance);
#endif
        }
#endif

        private void FetchRemoteConfig(IAdSettings settings) {
            if (settings == null) return;
#if ADS && REMOTE
            var key = RemoteKey.Instance;

            if (!string.IsNullOrEmpty(settings.BannerID)) FetchBool(key.ad_banner_enable, settings.UseBannerAd, (value) => {
                Logs.Log("RemoteConfig Banner Fetched ");
                settings.UseBannerAd = value;
                if (!value && AdsManager.Initialized) AdsManager.Instance.DestroyBanner();

                if (settings is AdmobSettings) {
                    var s = settings as AdmobSettings;
                    FetchBool(key.ad_banner_collapsible_enable, s.UseBannerCollapsible, (value) => { s.UseBannerCollapsible = value; });
                    FetchBool(key.ad_banner_collapsible_fallback_enable, s.CollapsibleFallbackEnable, (value) => { s.CollapsibleFallbackEnable = value; });
                    FetchBool(key.ad_banner_collapsible_auto_refresh, s.CollapsibleAutoRefresh, (value) => {
                        s.CollapsibleAutoRefresh = value;
                        if (AdmobMediation.Initialized) AdmobMediation.Instance.SetCollapsibleBannerAutoRefresh(value);
                        FetchLong(key.ad_banner_collapsible_reload_interval, (long)s.CollapsibleReloadInterval, (value) => { s.CollapsibleReloadInterval = (int)value; });
                    });
                }
                else FetchBool(key.ad_banner_adaptive_enable, settings.UseBannerAdaptive, (value) => settings.UseBannerAdaptive = value);
            });

            if (!string.IsNullOrEmpty(settings.MRecID)) FetchBool(key.ad_mrec_enable, settings.UseMRecAd, (value) => {
                settings.UseMRecAd = value;
                if (!value && AdsManager.Initialized) AdsManager.Instance.DestroyMRec();
            });


            if (!string.IsNullOrEmpty(settings.InterstitialID)) {
                FetchBool(key.ad_inter_enable, settings.UseInterstitialAd, (value) => {
                    settings.UseInterstitialAd = value;
                });
            }

            if (settings is AdmobSettings) {
                var s = settings as AdmobSettings;
                FetchBool(key.ad_aoa_openapp_first_install, s.AoaOpenFirstInstallEnable, (value) => { s.AoaOpenFirstInstallEnable = value; });
                FetchBool(key.ad_aoa_openapp, s.AoaOpenEnable, (value) => { s.AoaOpenEnable = value; });
                FetchBool(key.ad_aoa_resume_enable, s.AoaResumeEnable, (value) => s.AoaResumeEnable = value);

                FetchLong(key.ad_inter_from_played_times, s.InterFromPlayTimes, (value) => s.InterFromPlayTimes = (uint)value);
                FetchLong(key.ad_inter_from_level, s.InterFromLevel, (value) => s.InterFromLevel = (uint)value);
                FetchLong(key.ad_inter_from_seconds, s.InterFromSeconds, (value) => s.InterFromSeconds = (uint)value);
                FetchBool(key.ad_inter_resume_enable, s.InterResumeEnable, (value) => s.InterResumeEnable = value);
                FetchLong(key.ad_inter_capping, s.InterCapping, (value) => { s.InterCapping = value; });
                FetchLong(key.ad_inter_after_reward_capping, s.InterAfterRewardCapping, (value) => { s.InterAfterRewardCapping = value; });
                FetchBool(key.ad_inter_force_enable, s.ForceInterEnable, (value) => s.ForceInterEnable = value);
                FetchLong(key.ad_inter_force_capping, s.ForceInterCapping, (value) => { s.ForceInterCapping = value; });

                FetchLong(key.ad_banner_reload_capping, s.BannerReloadCapping, (value) => s.BannerReloadCapping = (uint)value);
                FetchLong(key.ad_banner_reload_level_capping, s.BannerReloadByLevelCapping, (value) => s.BannerReloadByLevelCapping = (uint)value);
                FetchLong(key.ad_banner_from_level, s.BannerFromLevel, (value) => s.BannerFromLevel = (uint)value);
                FetchLong(key.ad_banner_from_played_times, s.BannerFromPlayTimes, (value) => s.BannerFromPlayTimes = (uint)value);
            }
            else if (settings is MaxSettings) {
                FetchBool(key.ad_aoa_max_enable, settings.UseAOAAd, (value) => { settings.UseAOAAd = value; });
            }         
#endif
        }

#if ADS && REMOTE
        private void FetchBool(string key, bool defaultValue, Action<bool> callback) {
            AdmobSettings.Instance.RemoteFetchRemaining++;
            callback += (result) => FetchCompleted();
            RemoteConfig.Instance.AddDefault(key, defaultValue);
            RemoteConfig.Instance.GetBoolAsync(key, callback);
        }

        private void FetchLong(string key, long defaultValue, Action<long> callback) {
            AdmobSettings.Instance.RemoteFetchRemaining++;
            callback += (result) => FetchCompleted();
            RemoteConfig.Instance.AddDefault(key, defaultValue);
            RemoteConfig.Instance.GetLongAsync(key, callback);
        }

        private void FetchString(string key, string defaultValue, Action<string> callback) {
            AdmobSettings.Instance.RemoteFetchRemaining++;
            callback += (result) => FetchCompleted();
            RemoteConfig.Instance.AddDefault(key, defaultValue);
            RemoteConfig.Instance.GetStringAsync(key, callback);
        }

        private void FetchCompleted() {
            AdmobSettings.Instance.RemoteFetchRemaining--;
            if (AdmobSettings.Instance.RemoteFetchRemaining <= 0) {
                Debug.Log("[Ads.Remote] Fetch all config completed.");
            }
        }
#endif
    }
}