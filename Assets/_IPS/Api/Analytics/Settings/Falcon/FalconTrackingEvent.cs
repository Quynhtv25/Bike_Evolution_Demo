#if FALCON_ANALYTIC
using Falcon.FalconAnalytics.Scripts.Enum;
using Falcon.FalconAnalytics.Scripts.Models.Messages.PreDefines;
using Falcon.FalconCore.Scripts;
using System;
using UnityEngine;

namespace IPS.Api.Analytics {
    [CreateAssetMenu(fileName ="FalconTrackingEvent", menuName ="IPS/Api/Analytics/FalconTrackingEvent")]
    public class FalconTrackingEvent : CustomService {

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        static void OnBeforeSplashScreen() {
            FalconMain.OnInitComplete += ((sender, args) =>
                Debug.Log("[FALCON] SDK initialized."));
        }

        private string userId;

        public override string UserId {
            get {
                if (string.IsNullOrEmpty(userId)) {
                    userId = Falcon.FalconCore.Scripts.Repositories.News.FDeviceInfoRepo.DeviceId;
                }
                return userId;
            } 
        }

        public override void LogUserProperty(string propertyName, string propertyValue) {
            new FPropertyLog(propertyName, propertyValue, 0).Send();
        }

        public void LogFunel(string funelName, string action, int priority, int? currentLevel) {
            new FFunnelLog(funelName, action, priority, currentLevel).Send();
        }

        public override void LogLevelStart(int lv) {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lv"></param>
        /// <param name="difficulty">Ex: easy, hard, nohope, ...</param>
        /// <param name="win"></param>
        /// <param name="retry">Have ever user pass this level before? If true, param should be true, else false</param>
        /// <param name="durations">Total seconds user spent for this level</param>
        /// <param name="wave">Total wave user had passed before lose</param>
        public override void LogLevelEnd(int lv, string difficulty, bool win, bool retry, double durations, int? wave) {
            LevelStatus status;
            if (!retry) status = win ? LevelStatus.Pass : LevelStatus.Fail;
            else status = win ? LevelStatus.ReplayPass : LevelStatus.ReplayFail;

            TimeSpan time = TimeSpan.FromSeconds(durations);

            Logs.Log($"[Falcon.FLevelLog] lv={lv}, difficulty={difficulty}, status={status}, durationorigin={durations}, timelog={time.TotalSeconds}, wave={(wave.HasValue ? wave.Value : "null")}");
            if (Application.isEditor) return;
            new FLevelLog(lv, difficulty, status, time, wave).Send();
        }

        public override void LogAds(IPS.Api.Ads.AdSlotFormat adFormat, string placement, string adPrecision, string adCountry, double adRev, string adNetwork, string adMediation, double? adLtv = null) {
            AdType adType;
            if (adFormat == IPS.Api.Ads.AdSlotFormat.Banner) adType = AdType.Banner;
            else if (adFormat == IPS.Api.Ads.AdSlotFormat.Reward) adType = AdType.Reward;
            else if (adFormat == IPS.Api.Ads.AdSlotFormat.Inter) adType = AdType.Interstitial;
            else if (adFormat == IPS.Api.Ads.AdSlotFormat.AOA) adType = AdType.AppOpen;
            else {
                Logs.LogError($"[Falcon.FAdLog] No adType match with format={adFormat}");
                return;
            }

            Logs.Log($"[Falcon.FAdLog] adType={adType}, placement={placement}, adPrecision={adPrecision}, adCountry={adCountry}, adRev={adRev}, adNetwork={adNetwork}, mediation={adMediation}, lv={UserData.CurrentLevel}, ltv={adLtv}");
            if (Application.isEditor) return;
            new FAdLog(adType, placement, adPrecision, adCountry, adRev, adNetwork, adMediation, UserData.CurrentLevel, adLtv).Send();
        }

        public override void LogIap(string productId, decimal localizePrice, string isoCurrencyCode, string placement, string transactionId) {
            if (string.IsNullOrEmpty(productId)) return;

            Logs.Log($"[Falcon.FInAppLog] productId={productId}, price={localizePrice}{isoCurrencyCode}, placement={placement}, transactionId={transactionId}, lv={UserData.CurrentLevel}");
            if (Application.isEditor) return;
            new FInAppLog(productId, localizePrice, isoCurrencyCode, placement, transactionId, currentLevel: UserData.CurrentLevel).Send();
        }

        public override void LogCustomEvent(string eventName, ParameterBuilder param) {

        }
    }
}
#endif