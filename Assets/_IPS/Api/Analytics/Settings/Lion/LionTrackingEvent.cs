#if LION
using LionStudios.Suite.Analytics;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace IPS.Api.Analytics {
    [CreateAssetMenu(fileName ="LionTrackingEvent", menuName ="IPS/Api/Analytics/LionTrackingEvent")]
    public class LionTrackingEvent : CustomService {

        bool currentLevelIsTut = false;
        int currentLevelIdx = 0;
        int currentLevelAttempt = 0;
        const string currentLevelType = "main";

        public void LogTutStart(int lv, string stepName) {
            //LogMissionStep(currentLevelIsTut, currentLevelType, $"{currentLevelType}_{lv}", lv.ToString(), 0, 1, $"ftue_{stepName}_start");
        }

        public void LogTutEnd(int lv, string stepName) {
            LogMissionStep(currentLevelIsTut, currentLevelType, $"{currentLevelType}_{lv}", lv.ToString(), 0, 1, "ftue");
        }

        public void LogLevelStart(int lv, bool isTut = false, int failCount = 0) {
            currentLevelIsTut = isTut;
            currentLevelIdx = lv;
            currentLevelAttempt = failCount + 1;
            LogMissionStart(isTut, currentLevelType, $"{currentLevelType}_{lv}", lv.ToString(), currentLevelAttempt);
        }

        public void LogLevelReviveOffer(int score = 0) {
            LogMissionStep(currentLevelIsTut, currentLevelType, $"{currentLevelType}_{currentLevelIdx}", currentLevelIdx.ToString(), score, currentLevelAttempt, "soft_fail");
        }
        public void LogLevelReviveSuccess(int score = 0) {
            LogMissionStep(currentLevelIsTut, currentLevelType, $"{currentLevelType}_{currentLevelIdx}", currentLevelIdx.ToString(), score, currentLevelAttempt, "revive");
        }

        public override void LogLevelEnd(int lv, string difficulty, bool win, bool retry, double durations, int? wave, int score = 0, bool quitIngame = false, string reason = null) {
            if (win) {
                LogMissionCompleted(currentLevelIsTut, currentLevelType, $"{currentLevelType}_{currentLevelIdx}", currentLevelIdx.ToString(), score, currentLevelAttempt);
            }
            else if (quitIngame) {
                LogLevelQuit(score);
            }
            else {
                LogMissionFailed(currentLevelIsTut, currentLevelType, $"{currentLevelType}_{currentLevelIdx}", currentLevelIdx.ToString(), score, currentLevelAttempt, reason);
            }

        }

        public void LogLevelQuit(int score = 0) {
            LogMissionAbandoned(currentLevelIsTut, currentLevelType, $"{currentLevelType}_{currentLevelIdx}", currentLevelIdx.ToString(), score, currentLevelAttempt);
        }

        public void LogUseCurrency(string currencyName) {
            LogMissionStep(currentLevelIsTut, currentLevelType, $"{currentLevelType}_{currentLevelIdx}", currentLevelIdx.ToString(), 0, currentLevelAttempt, $"use_currency_{currencyName}");
        }

        public void LogUseBooster(string boosterName, ParameterBuilder extra = null) {
            LogMissionStep(currentLevelIsTut, currentLevelType, $"{currentLevelType}_{currentLevelIdx}", currentLevelIdx.ToString(), 0, currentLevelAttempt, $"use_booster_{boosterName}");
            LionAnalytics.PowerUpUsed(currentLevelIdx.ToString(), currentLevelType, currentLevelAttempt, boosterName, extra != null ? extra.BuildDictObject() : null);
        }

        #region Origin Lion
        /// <summary>
        /// Marks the beginning of a new level or mission attempt.
        /// </summary>
        /// <param name="isTutorial"></param>
        /// <param name="missionType">Top-level type of the mission. Ex: "main"</param>
        /// <param name="missionName">Specifies the level grouping (e.g., season, bundle, pack). A single missionID may be paired with different missionName parameters.</param>
        /// <param name="missionID">Identifier for the level, mission, or mission step</param>
        /// <param name="missionAttempt">Count of attempts for the level or mission</param>
        private void LogMissionStart(bool isTutorial, string missionType, string missionName,string missionID, int? missionAttempt = null) {
            Logs.Log($"[IPS][Lion][Tracking] MisionStarted missionType={missionType}, missionName={missionName}, missionID={missionID}, missionAttempt={missionAttempt}");
            LionAnalytics.MissionStarted(isTutorial, missionType, missionName, missionID, missionAttempt);
        }

        /// <summary>
        /// Indicates a significant step or checkpoint within a level or mission.
        /// </summary>
        /// <param name="isTutorial"></param>
        /// <param name="missionType">Top-level type of the mission. Ex: "main"</param>
        /// <param name="missionName">Specifies the level grouping (e.g., season, bundle, pack). A single missionID may be paired with different missionName parameters.</param>
        /// <param name="missionID">Identifier for the level, mission, or mission step</param>
        /// <param name="missionAttempt">Count of attempts for the level or mission</param>
        private void LogMissionStep(bool isTutorial, string missionType, string missionName, string missionID, int userScore, int? missionAttempt = null, string stepName = null, ParameterBuilder extra = null) {
            Logs.Log($"[IPS][Lion][Tracking] MisionStep stepName={stepName}, missionType={missionType}, missionName={missionName}, missionID={missionID}, missionAttempt={missionAttempt}");
            var dict = AdditionalData();
            if (extra != null) {
                foreach (var kvp in extra.BuildDictObject()) {
                    dict.Add(kvp.Key, kvp.Value);
                }
            }

            LionAnalytics.MissionStep(isTutorial, missionType, missionName, missionID, userScore, missionAttempt, dict, stepName: stepName);
        }

        /// <summary>
        /// Signifies the successful completion of a level or mission.
        /// </summary>
        /// <param name="isTutorial"></param>
        /// <param name="missionType">Top-level type of the mission. Ex: "main"</param>
        /// <param name="missionName">Specifies the level grouping (e.g., season, bundle, pack). A single missionID may be paired with different missionName parameters.</param>
        /// <param name="missionID">Identifier for the level, mission, or mission step</param>
        /// <param name="missionAttempt">Count of attempts for the level or mission</param>
        private void LogMissionCompleted(bool isTutorial, string missionType, string missionName, string missionID, int userScore, int? missionAttempt = null) {
            Logs.Log($"[IPS][Lion][Tracking] MisionCompleted missionType={missionType}, missionName={missionName}, missionID={missionID}, missionAttempt={missionAttempt}, userScore={userScore}");
            LionAnalytics.MissionCompleted(isTutorial, missionType, missionName, missionID, userScore, missionAttempt, AdditionalData());
        }


        /// <summary>
        /// Marks the end of a level or mission attempt with an unsuccessful outcome.
        /// </summary>
        /// <param name="isTutorial"></param>
        /// <param name="missionType">Top-level type of the mission. Ex: "main"</param>
        /// <param name="missionName">Specifies the level grouping (e.g., season, bundle, pack). A single missionID may be paired with different missionName parameters.</param>
        /// <param name="missionID">Identifier for the level, mission, or mission step</param>
        /// <param name="missionAttempt">Count of attempts for the level or mission</param>
        private void LogMissionFailed(bool isTutorial, string missionType, string missionName, string missionID, int userScore, int? missionAttempt = null, string failReason = null) {
            Logs.Log($"[IPS][Lion][Tracking] MisionFailed missionType={missionType}, missionName={missionName}, missionID={missionID}, missionAttempt={missionAttempt}, userScore={userScore}");
            LionAnalytics.MissionFailed(isTutorial, missionType, missionName, missionID, userScore, missionAttempt, AdditionalData(), failReason: failReason);
        }

        /// <summary>
        /// Indicates the player left the level or mission without completing it.
        /// </summary>
        /// <param name="isTutorial"></param>
        /// <param name="missionType">Top-level type of the mission. Ex: "main"</param>
        /// <param name="missionName">Specifies the level grouping (e.g., season, bundle, pack). A single missionID may be paired with different missionName parameters.</param>
        /// <param name="missionID">Identifier for the level, mission, or mission step</param>
        /// <param name="missionAttempt">Count of attempts for the level or mission</param>
        private void LogMissionAbandoned(bool isTutorial, string missionType, string missionName, string missionID, int userScore, int? missionAttempt = null) {
            Logs.Log($"[IPS][Lion][Tracking] MissionAbandoned missionType={missionType}, missionName={missionName}, missionID={missionID}, missionAttempt={missionAttempt}, userScore={userScore}");
            LionAnalytics.MissionAbandoned(isTutorial, missionType, missionName, missionID, userScore, missionAttempt, AdditionalData());
        }

        public void LogRewardedVideoEligible(string placement, int level) {
            Logs.Log($"[IPS][Lion][Tracking] RewardVideoShow (Eligible) placement={placement}, level={level}");
            LionAnalytics.RewardVideoShow(placement, "unknown", level,  additionalData: AdditionalData());
        }

        public void LogRewardedVideoStart(string placement, string network, int level) {
            Logs.Log($"[IPS][Lion][Tracking] RewardVideoDisplay placement={placement}, network={network}, level={level}");
            LionAnalytics.RewardVideoStart(placement, network, level,  additionalData: AdditionalData());
        }

        public void LogRewardedVideoEnd(string placement, string network, int level) {
            Logs.Log($"[IPS][Lion][Tracking] LogRewardedVideoEnd placement={placement}, network={network}, level={level}");
            LionAnalytics.RewardVideoEnd(placement, network, level, additionalData: AdditionalData());
        }

        public void LogRewardedVideoSuccess(string placement, string network, int level) {
            Logs.Log($"[IPS][Lion][Tracking] LogRewardedVideoSuccess placement={placement}, network={network}, level={level}");
            LionAnalytics.RewardVideoCollect(placement, reward: null, level: level, additionalData: AdditionalData());
        }

        private Dictionary<string, object> AdditionalData() {
            var data = new MissionData {
                mission_type = currentLevelType,
                mission_name = $"{currentLevelType}_{currentLevelIdx}",
                mission_id = currentLevelIdx,
                mission_attempt = currentLevelAttempt
            };

            var dict = new Dictionary<string, object>();
            dict.Add("mission_data", JObject.Parse(JsonUtility.ToJson(data)));
            return dict;
        }
        #endregion
    }

    [System.Serializable]
    internal struct MissionData {
        public string mission_type;
        public string mission_name;
        public int mission_id;
        public int mission_attempt;
    }
}
#endif