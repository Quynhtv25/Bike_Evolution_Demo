using System;
using IPS;
using IPS.Api.Analytics;
using Unity.VisualScripting;
using UnityEngine;

public partial class Tracking {
    private const string ParamLevelId = "level_id";
    private const string ParamPlayTimes = "play_times";
    private const string ParamDurations = "durations";
    private const string ParamFailCount = "failcount";
    private const string ParamQuitCount = "quitcount";
    private const string ParamLevelData = "level_data";
    private const string ParamDifficulty = "difficulty";

    #region Retention
    private const string FirstLoginDateKey = "FIRST_LOGIN_DATE";
    private const string LastLoginDateKey = "LAST_LOGIN_DATE";

    private DateTime FirstLoginDate {
        get => UserData.GetDateTime(FirstLoginDateKey, DateTime.Now);
        set => UserData.SetDateTime(FirstLoginDateKey, value);
    }

    private DateTime LastLoginDate {
        get => UserData.GetDateTime(LastLoginDateKey, DateTime.Now);
        set => UserData.SetDateTime(LastLoginDateKey, value);
    }

    private int Retention => DateTime.Compare(DateTime.Now.Date, FirstLoginDate.Date) > 0
                                ? (DateTime.Now.Date - FirstLoginDate.Date).Days
                                : 0;
    private int DaysPlayed {
        get => UserData.GetInt("TOTAL_DAYS_PLAYED", 0);
        set => UserData.SetInt("TOTAL_DAYS_PLAYED", value);
    }

    private void LogRetention() {
        if (!UserData.HasKey(FirstLoginDateKey)) {
            FirstLoginDate = DateTime.Now;
            DaysPlayed = 1;
        }

        LastLoginDate = DateTime.Now;
        if (DateTime.Compare(LastLoginDate.Date, FirstLoginDate.Date) > 0) {
            DaysPlayed++;
        }

        SetUserProperty(EventName.property_days_played, DaysPlayed.ToString());
        SetUserProperty(EventName.property_retention_day, Retention.ToString());
    }
    
    
    private double TotalEngagementMinutesToday(int dayNum) {
        return UserData.GetDouble($"engagement_minutes_d{dayNum}", 0);
    }

    private void SetTotalEngagementMinutesToday(int dayNum, double value) {
        UserData.SetDouble($"engagement_minutes_d{dayNum}", value);
    }

    private DateTime engagement_timestamp;
    private void LogEngagementSession() {
        int retentionDay = Retention;
        SetTotalEngagementMinutesToday(retentionDay, TotalEngagementMinutesToday(retentionDay) + ((DateTime.Now - engagement_timestamp).TotalMinutes));
        engagement_timestamp = DateTime.Now;
        SetUserProperty($"total_engagement_minutes_d{retentionDay}", TotalEngagementMinutesToday(retentionDay).ToString());
    }
    #endregion
    
    public static int CurrentLevel {
        get => PlayerPrefs.GetInt("TrackingCurrentLevel", UserData.CurrentLevel);
        set => PlayerPrefs.SetInt("TrackingCurrentLevel", value);
    }

    private int LastLevelCompleted {
        get => UserData.GetInt($"last_completed_level", 0);
        set {
            if (value > LastLevelCompleted) UserData.SetInt($"last_completed_level", value);
        }
    }

    public bool IsWinLastLevel => LastLevelCompleted == CurrentLevel;

    private int LevelFailedCount(string levelId) {
        if (string.IsNullOrEmpty(levelId)) return 0;
        return UserData.GetInt($"failed_level_{levelId}_count", 0);
    }
    private int LevelQuitCount(string levelId) {
        if (string.IsNullOrEmpty(levelId)) return 0;
        return UserData.GetInt($"quit_level_{levelId}_count", 0);
    }

    private void InCreaseLevelQuitCount(string levelId) {
        UserData.SetInt($"quit_level_{levelId}_count", UserData.GetInt($"quit_level_{levelId}_count") + 1);
    }

    private void InCreaseLevelFailedCount(string levelId) {
        UserData.SetInt($"failed_level_{levelId}_count", UserData.GetInt($"failed_level_{levelId}_count") + 1);
    }

    public void LogOpenApp(string versionName, int versionCode) {
        var param = ParameterBuilder.Create(ParamIsAndroid, IPSConfig.IsAndroid)
                                    .Add(ParamVersionName, Application.version)
                                    .Add(ParamVersionCode, BootstrapConfig.Instance.VersionCode);

        if (UserData.FirstInstall) Tracking.Instance.LogEvent("ftue_open_app", param);
        Tracking.Instance.LogEvent("ips_open_app", param);
        Tracking.Instance.LogEvent(EventName.open_app, param);
    }

    public void LogLoadingStart() {
        if (UserData.FirstInstall) Tracking.Instance.LogEvent("ftue_loading_start");
        Tracking.Instance.LogEvent("ips_loading_start");
        Tracking.Instance.LogEvent(EventName.loading_start);
    }

    public void LogLoadingAsyncEnd(float durationSeconds, int percentage) {
        Tracking.Instance.LogEvent("loading_async_complete", ParameterBuilder.Create(ParamDurations, durationSeconds).Add("fake_progress", percentage));
    }

    public void LogLoadingEnd(float durationSeconds) {
        if (UserData.FirstInstall) Tracking.Instance.LogEvent("ftue_loading_end", ParamDurations, durationSeconds.ToString());
        Tracking.Instance.LogEvent("ips_loading_end", ParamDurations, durationSeconds.ToString());
        Tracking.Instance.LogEvent(EventName.loading_end);
    }

    public void LogEnterHomeScene(float durationSeconds) {
        if (UserData.FirstInstall) Tracking.Instance.LogEvent("ftue_enter_home_scene", ParamDurations, durationSeconds.ToString());
        Tracking.Instance.LogEvent("enter_homescene", ParamDurations, durationSeconds.ToString());
        LogEngagementSession();
    }

    /// <summary>
    /// Call if game has only one tut (has no tutId)
    /// </summary>
    public void LogTutStart() {
        Logs.Log($"[IPS][Tracking] Tutorial Start (no step)");
        if (UserData.FirstInstall) Tracking.Instance.LogEvent("ftue_tut_start");
        Tracking.Instance.LogEvent("tut_start", service: Service.AllExceptAppsFlyerAdjustFacebook);
        LogEngagementSession();
    }

    /// <summary>
    /// Call if game has only one tut (has no tutId)
    /// </summary>
    public void LogTutComplete(bool completed = true) {
        Logs.Log($"[IPS][Tracking] Tutorial Complete (no step)");
        if (UserData.FirstInstall) Tracking.Instance.LogEvent("ftue_tut_complete");
        Tracking.Instance.LogEvent("tut_complete", service: Service.AllExceptAppsFlyerAdjustFacebook);
        if (!string.IsNullOrEmpty(EventName.af_tutorial_completion)) { 
            Tracking.Instance.LogEvent(EventName.af_tutorial_completion, 
                                        ParameterBuilder.Create("af_tutorial_id", "tut")
                                        .Add("af_success", completed),
                                        service: Tracking.Service.AppsFlyer);
        }
        LogEngagementSession();
    }

    public void LogTutStart(string tutStep) {
        Logs.Log($"[IPS][Tracking] Tutorial Start tut={tutStep}");
        Tracking.Instance.LogEvent("tutorial_start", "tut_id", tutStep, Tracking.Service.AllExceptAppsFlyerAdjustFacebook);
        Tracking.Instance.LogProgressStart("FTUE_Tut", tutStep);
#if LION
        if (CustomService) {
            (CustomService as LionTrackingEvent).LogTutStart(CurrentLevel, tutStep);
        }
#endif
        LogEngagementSession();
    }

    public void LogTutEnd(string tutStep, bool completed = true) {
        if (!string.IsNullOrEmpty(EventName.af_tutorial_completion)) { 
            Tracking.Instance.LogEvent(EventName.af_tutorial_completion, 
                                        ParameterBuilder.Create("af_tutorial_id", tutStep)
                                        .Add("af_success", completed),
                                        service: Tracking.Service.AppsFlyer);
        }

        Logs.Log($"[IPS][Tracking] Tutorial End tut={tutStep}, completed={completed}");

        Tracking.Instance.LogEvent(completed ? "tutorial_completed" : "tutorial_fail", "tut_id", tutStep, Tracking.Service.AllExceptAppsFlyerAdjustFacebook);
        if (completed) Tracking.Instance.LogProgressComplete("FTUE_Tut", tutStep);
#if LION
        if (CustomService) {
            (CustomService as LionTrackingEvent).LogTutEnd(CurrentLevel, tutStep);
        }
#endif
        LogEngagementSession();

    }

    public void LogLevelReviveSuccess(ParameterBuilder param) {
        if (param == null) param = ParameterBuilder.Create();
        PauseLevel(false);

        Logs.Log($"[IPS][Tracking] Level Revive Success lv={currentLevelId}, dataName={currentLevelDataName}, para={param.ToString()}");

        LogEvent(EventName.level_revive_int, param.Add(EventName.level_param_id, currentLevelIndex)
                                            .Add(EventName.level_param_timestamp, levelStartTimeStamp)
                                            .Add(EventName.level_param_mode_id, currentLevelModeIndex)
                                            , service: Service.AllExceptAppsFlyerAdjustFacebook);
#if LION
        if (CustomService != null) {
            (CustomService as LionTrackingEvent).LogLevelReviveSuccess();
        }
#endif
        LogEngagementSession();

    }

    public void LogLevelReviveOffer(ParameterBuilder extra = null, string levelDataName = default) {
        if (!isPlayingInLevel) return;


        PauseLevel(true); 
        if (extra == null) extra = ParameterBuilder.Create();
        if (currentLevelIndex > 0) extra.Add(ParamLevelId, currentLevelIndex);
        else extra.Add(ParamLevelId, currentLevelId);

        extra.Add(ParamDurations, LevelDuration)
            .Add(EventName.level_param_timestamp, levelStartTimeStamp)
            .Add(EventName.level_param_mode_id, currentLevelModeIndex);

        Logs.Log($"[IPS][Tracking] Level Revive Offer lv={currentLevelId}, dataName={levelDataName}, para={extra.ToString()}");

        LogEvent("ips_level_revive_offer", extra, service: Service.AllExceptAppsFlyerAdjustFacebook);

#if LION
        if (CustomService != null) {
            (CustomService as LionTrackingEvent).LogLevelReviveOffer();
        }
#endif
        LogEngagementSession();
    }

    private void PauseLevel(bool pause) {
        if (pause) levelDuration += (new TimeSpan(DateTime.Now.Ticks)).TotalSeconds - levelStartTime;
        else levelStartTime = (new TimeSpan(DateTime.Now.Ticks)).TotalSeconds;
    }

    public void LogLevelLoaded(int lv, int hard = 0, int mode = 0, string levelDataName = default, ParameterBuilder extraParam = null) {
        CurrentLevel = lv;
        currentLevelIndex = lv;
        currentLevelModeIndex = mode;
        currentLevelId = lv.ToString();
        levelHardType = hard;

        if (extraParam == null) extraParam = ParameterBuilder.Create();
        extraParam = StartLevelParamId(extraParam, ParameterBuilder.Create(), EventName.level_param_id);

        LogEvent("level_loaded", extraParam);
    }

    public void LogLevelStart(int lv, int hard = 0, int mode = 0, string levelDataName = default, ParameterBuilder extraParam = null) {
        if (isPlayingInLevel) {
            Debug.LogError($"User still playing in level {currentLevelId}. You must call 'LogEndLevel' before this.");
            LogLevelCompleted(false, skip: true, reason: "start_new_level");
            //return;
        }

        CurrentLevel = lv;
        currentLevelIndex = lv;
        currentLevelModeIndex = mode;
        currentLevelId = lv.ToString();
        levelHardType = hard;
        levelStartTimeStamp = (new TimeSpan(DateTime.Now.Ticks)).Ticks;

        if (currentLevelIndex <= 20) {
            LogEvent(EventName.af_level_start, EventName.af_level_param_id, currentLevelId, service: Service.AppsFlyer);
        }

        if (currentLevelIndex < 100 && currentLevelIndex >= 0) {
            LogEvent($"{EventName.start_level_x}{currentLevelIndex}", service: Service.Firebase);
        }


        if (extraParam == null) extraParam = ParameterBuilder.Create();
        extraParam = StartLevelParamId(extraParam, ParameterBuilder.Create(), EventName.level_param_id);

        LogEvent(EventName.level_start_int, extraParam);
        LogLevelStart(false, lv.ToString(), hard, mode, levelDataName, extraParam);
#if LION
        if (CustomService != null) {
            (CustomService as LionTrackingEvent).LogLevelStart(lv, false, LevelFailedCount(lv.ToString()));
        }
#endif
    }

    public void LogLevelStart(string levelName, int hard = 0, int mode = 0, string levelDataName = default, ParameterBuilder extraParam = null) {
        LogLevelStart(true, levelName, hard, mode, levelDataName, extraParam);
    }


    private void LogLevelStart(bool logEventStart, string lv, int hard = 0, int mode = 0, string levelDataName = default, ParameterBuilder extraParam = null) {
        if (isPlayingInLevel) {
            Debug.LogError($"User still playing in level {currentLevelId}. You must call 'LogEndLevel' before this.");
            LogLevelCompleted(false, skip: true, reason: "start_new_level");
            //return;
        }

        currentLevelId = lv;
        currentLevelModeIndex = mode;
        levelHardType = hard;
        if (!string.IsNullOrEmpty(levelDataName)) {
            currentLevelDataName = levelDataName;
            SetUserProperty(ParamLevelData, levelDataName);
        }

        if (extraParam == null) extraParam = ParameterBuilder.Create();
        extraParam = StartLevelParamId(extraParam, ParameterBuilder.Create(), EventName.level_param_id);

        if (logEventStart) LogEvent(EventName.level_start_string, extraParam, service: Service.AllExceptAppsFlyerAdjustFacebook);

        LogAdjust(EventName.aj_level_start);
        LogIPSStartLevel(lv, extraParam);
    }

    public void LogLevelQuit(ParameterBuilder extra) {
        LogLevelCompleted(skip: true, reason: "quit", extra: extra);
    }

    /// <summary>
    /// Call when user win or lose immediatly (make sure call this before any ads show)
    /// </summary>
    /// <param name="win"></param>
    /// <param name="skip">true if user quit level or restart in level (not win/not lose)</param>
    /// <param name="reason">Why user lose: restart_level, quit_level (go_home), out_of_space, out_of_time, out_of_live, ...</param>
    /// <param name="extra"></param>
    /// <param name="wave"></param>
    public void LogLevelCompleted(bool? win = null, bool skip = false, string reason = "lose", ParameterBuilder extra = null, int? wave = null) {
        if (!isPlayingInLevel) return;

        bool retry = currentLevelIndex < LastLevelCompleted;
        int resume = levelDuration > 0 ? 1 : 0;
        levelDuration += (new TimeSpan(DateTime.Now.Ticks)).TotalSeconds - levelStartTime;

        SetUserProperty(EventName.property_last_level, !string.IsNullOrEmpty(currentLevelId) ? currentLevelId : currentLevelIndex.ToString());

        if (win.HasValue && !win.Value) InCreaseLevelFailedCount(currentLevelId);
        if (skip) InCreaseLevelQuitCount(currentLevelId);

        var para = ParameterBuilder.Create();
        // case win in first play times of this level (never fail this level before)
        if ((!win.HasValue || win.Value) && currentLevelIndex > LastLevelCompleted) {
            if (currentLevelIndex <= 20) {
                LogEvent($"{EventName.af_complete_level_x}{currentLevelIndex}", EventName.af_level_param_id, currentLevelId, service: Service.AppsFlyer);
                LogEvent($"{EventName.checkpoint_x}{currentLevelId}", StartLevelParamId(extra, para, EventName.level_param_id).Add(EventName.level_param_duration, LevelDuration), service: Service.Firebase);
            }
            LastLevelCompleted = currentLevelIndex;

            LogEvent($"{EventName.af_level_achieved}", StartLevelParamId(extra, para, EventName.af_level_param_id), service: Service.AppsFlyer);

            if (currentLevelIndex > 0) {
                SetUserProperty(EventName.property_level_max, currentLevelId);
                SetUserProperty(EventName.property_current_level, (currentLevelIndex + 1).ToString());
            }
            else SetUserProperty(EventName.property_current_level, currentLevelId);
        }

        // for game has win or lose
        para = StartLevelParamId(extra, para, EventName.level_param_id)
                .Add("status", skip ? "skip" : (!win.HasValue || win.Value) ? "win" : "lose")
                .Add("retry", retry)
                .Add("reason", reason)
                .Add("resume_from_background", resume)
                .Add(ParamDurations, LevelDuration)
                .Add(EventName.level_param_duration, LevelDuration)
                .Add("win", win.HasValue ? win.Value.ToString().ToLower() : "true");
        if (win.HasValue) {
            Logs.Log($"[IPS][Tracking] Level Complete lv={currentLevelId}, win={win.Value}, skip={skip}, reason={reason}, dataName={currentLevelDataName}");

            if (currentLevelIndex < 100 && currentLevelIndex > 0) {
                LogEvent(win.Value ? $"{EventName.win_level_x}{currentLevelId}" : $"{EventName.lose_level_x}{currentLevelId}", service: Service.Firebase);
            }

            LogEvent(win.Value ? $"{EventName.af_level_win}" : $"{EventName.af_level_lose}", StartLevelParamId(extra, ParameterBuilder.Create(), EventName.af_level_param_id), service: Service.AppsFlyer);

            LogIPSEndLevel(resume, para.Add(ParamLevelId, currentLevelId).Add(ParamDurations, LevelDuration));
                    
            if (win.Value) {
                LogAdjust(EventName.aj_level_achieved);
                LogEvent(EventName.win_level, para, Service.AllExceptAppsFlyerAdjustFacebook);
            }
            else {
                LogAdjust(EventName.aj_level_fail);
                LogEvent(EventName.lose_level, para, Service.AllExceptAppsFlyerAdjustFacebook);
            }            
        }
        else LogIPSEndLevel(resume, para);

        LogEvent(currentLevelIndex < 0 && !string.IsNullOrEmpty(EventName.level_end_string) ? 
                            EventName.level_end_string : EventName.level_end_int,
                            para);

        if (currentLevelIndex >= 0 && CustomService) {
            CustomService.LogLevelEnd(currentLevelIndex, GetLevelDifficulty(levelHardType), 
                                                                win.HasValue ? win.Value : true, 
                                                                retry, LevelDuration, wave, quitIngame: skip, reason: reason);
        }
    }

    public static string GetLevelDifficulty(int levelHard) {
        return levelHard == 0 ? "normal" : levelHard == 1 ? "medium" : "hard";
    }
    
    private ParameterBuilder StartLevelParamId(ParameterBuilder extra, ParameterBuilder para, string paramId) {
        para.parameters.Clear();
        if (extra != null) para.parameters.AddRange(extra.parameters);
        if (currentLevelIndex >= 0) para.Add(paramId, currentLevelIndex);
        para.Add(paramId, currentLevelId);

        para.Add(ParamPlayTimes, UserData.PlayTimes)
            .Add(ParamLevelData, currentLevelDataName)
            .Add(EventName.level_param_timestamp, levelStartTimeStamp)
            .Add(EventName.level_param_mode_id, currentLevelModeIndex)
            .Add("current_gold", UserData.CurrentCoin)
            .Add(ParamDifficulty, GetLevelDifficulty(levelHardType))
            .Add(ParamFailCount, LevelFailedCount(currentLevelId))
            .Add(ParamQuitCount, LevelQuitCount(currentLevelId))
            .Add("retention_day", Retention)
            .Add($"engagement_minutes_today", TotalEngagementMinutesToday(Retention));

        return para;
    }

    /// <summary>
    /// Call when user have an action on the level (Ex: level need to complete 10 trays, user move a tray but not complete it => also call this)
    /// </summary>
    /// <param name="para">All extra parameter in level. Common use LevelManager.LevelParamExtra</param>
    public void LogLevelStepMoveMade(ParameterBuilder para) {
        if (para == null) para = ParameterBuilder.Create();
        LogEngagementSession();
        LogEvent("ips_step_move_made", StartLevelParamId(para, ParameterBuilder.Create(), EventName.level_param_id));
    }

    /// <summary>
    /// Call when user complete a small target in the level (Ex: level need to complete 10 trays, user complete 1 tray => call this)
    /// </summary>
    /// <param name="para">All extra parameter in level. Common use LevelManager.LevelParamExtra</param>
    public void LogLevelStepTargetMade(ParameterBuilder para) {
        if (para == null) para = ParameterBuilder.Create();
        LogEngagementSession();
        LogEvent("ips_step_target_made", StartLevelParamId(para, ParameterBuilder.Create(), EventName.level_param_id));
    }

    /// <summary> Call when enter a screen (home, ingame, endgame, shop, ...) </summary>
    public void LogScreen(string screenName) {
        LogEvent("ips_screenview", ParamPlacement, screenName, Service.AllExceptAppsFlyerAdjustFacebook);
        LogEngagementSession();
    }

    /// <summary> Call when open a popup or any frame (settings popup, pause popup, remove ads popup, daily popup,...) </summary>
    public void LogUIFrame(string screenName, string frameName) {
        LogEvent("ui_appear", ParameterBuilder.Create().Add("screen_name", screenName).Add("name", frameName), service: Service.AllExceptAppsFlyerAdjustFacebook);
        LogEvent(EventName.frame_show, EventName.frame_show_param, frameName);
        LogEngagementSession();
    }

    /// <summary> Call when clicked any button in the game</summary>
    public void LogButtonClick(string buttonName, string uiName, bool separate = false, bool matchScreenName = false) {
        var param = ParameterBuilder.Create().Add(ParamPlacement, buttonName).Add(ParamLevelId, CurrentLevel);
        LogEvent("button_click", param.Add("screen_name", uiName).Add("name", buttonName), service: Service.AllExceptAppsFlyerAdjustFacebook);
        if (separate) LogEvent($"{EventName.button_click_x}{buttonName}{(matchScreenName ? string.Format("_{0}", uiName) : string.Empty)}", ParamLevelId, CurrentLevel.ToString());
        LogEngagementSession();
    }    

    public void LogUseCurrency(string placement) {
        LogEvent(EventName.use_currency, ParameterBuilder.Create(EventName.level_param_id, currentLevelId).Add(EventName.use_currency_param_name, placement));
#if LION
        if (CustomService) {
            (CustomService as LionTrackingEvent).LogUseCurrency(placement);
        }
#endif
        LogEngagementSession();
    }

    public void LogUseBooster(string boosterName, ParameterBuilder extra = null) {
        if (extra == null) extra = ParameterBuilder.Create();
#if LION
        if (CustomService) {
            (CustomService as LionTrackingEvent).LogUseBooster(boosterName, extra);
        }
#endif
        extra.Add(EventName.level_param_id, currentLevelId).Add(EventName.use_booster_param_name, boosterName);
        LogEvent(EventName.use_booster, extra);
        LogEngagementSession();
    }
    
    private bool isPlayingInLevel;
    private int currentLevelModeIndex = 0;
    private int currentLevelIndex = -1;
    private string currentLevelId;
    private string currentLevelDataName;
    private long levelStartTimeStamp;
    private double levelStartTime;
    private double levelDuration;
    private int levelHardType;

    private int LevelDuration => (int)levelDuration;
    private void LogIPSStartLevel(string levelId, ParameterBuilder extraParams = null, Service service = Service.AllExceptAppsFlyerAdjustFacebook) {
        if (isPlayingInLevel) {
            Debug.LogError($"User still playing in level {currentLevelId}. You must call 'LogEndLevel' before this.");
            return;
        }

        if (extraParams == null) extraParams = ParameterBuilder.Create();
        
        levelDuration = 0;
        levelStartTime = (new TimeSpan(DateTime.Now.Ticks)).TotalSeconds;
        currentLevelId = levelId;
        isPlayingInLevel = true;

        if (!string.IsNullOrEmpty(levelId)) {
            extraParams.Add("id", levelId);
            if (currentLevelIndex > 0) extraParams.Add(ParamLevelId, currentLevelIndex);
            else extraParams.Add(ParamLevelId, levelId);
        }

        UserData.IncreasePlayTimes();
        extraParams.Add(ParamPlayTimes, UserData.PlayTimes);
        extraParams.Add(ParamFailCount, LevelFailedCount(currentLevelId)).Add(ParamQuitCount, LevelQuitCount(currentLevelId));

        Logs.Log($"[IPS][Tracking] Level Start lv={levelId}, dataName={currentLevelDataName}");
        LogEvent("ips_start_level", extraParams, service);
        LogEngagementSession();
    }

    private void LogIPSEndLevel(int resume, ParameterBuilder extraParams = null, Service service = Service.AllExceptAppsFlyerAdjustFacebook) {
        if (!isPlayingInLevel) return;
        if (extraParams == null) extraParams = ParameterBuilder.Create();

        isPlayingInLevel = false;

        if (!string.IsNullOrEmpty(currentLevelId)) {
            extraParams.Add("id", currentLevelId);
            //if (currentLevelIndex > 0) extraParams.Add(ParamLevelId, currentLevelIndex);
            //else extraParams.Add(ParamLevelId, currentLevelId);
        }

        //extraParams.Add("resume_from_background", resume);

        //extraParams.Add(ParamDurations, LevelDuration);
        
        LogEvent("ips_end_level", extraParams, service);
        LogEngagementSession();
    }

    private void OnApplicationFocus(bool focus) {
        if (isPlayingInLevel) {
            PauseLevel(!focus);
        }
        
        int retentionDay = Retention;
        if (!focus) {
            SetTotalEngagementMinutesToday(retentionDay, TotalEngagementMinutesToday(retentionDay) + ((DateTime.Now - engagement_timestamp).TotalMinutes));
        }
        else {
            engagement_timestamp = DateTime.Now;
            SetUserProperty($"total_engagement_minutes_d{retentionDay}", TotalEngagementMinutesToday(retentionDay).ToString());
        }
    }

    partial void OnAwakeGames() {
        Excutor.Schedule(LogRetention, 2);
    }
}
