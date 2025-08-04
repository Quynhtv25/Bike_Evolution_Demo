using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using IPS;
using IPS.Api.Ads;

#if ADMOB
using GoogleMobileAds.Api;
#endif

public interface IMediation {}

public partial class AdsManager : SingletonBehaviourDontDestroy<AdsManager> {
    public struct OnAdRemoved : IEventParam { }
    public static bool PauseInsideApp { get; set; }

    /// <summary>
    /// Use to listening when the reward video ad is available, usefull for turn on some ad button.
    /// </summary>
    public event Action onRewardInterAvailable;
    public event Action onRewardVideoAvailable;
    public event Action<Mediation, float> onBannerHeightChanged;

    private float lastRewardShowAtTime;
    private float lastInterShowAtTime;

    private string currentPlacement;    
    private string currentBannerPlacement;

    private double lastPauseTime;

    protected override void OnAwake() {
    }

    private void Start() {
#if IAP
        IAP.Instance.onInitialized += RestoreRemoveAd;
#endif
        StartCoroutine(IEInitMediation());
    }

    private IEnumerator IEInitMediation() {
        float timeout = 5; // Delay max 5 seconds for fetch remote config
        float startTime = Time.time;
        yield return new WaitUntil(() => AdmobSettings.Instance.AdsRemoteConfigFetched || Time.time - startTime >= timeout);
        if (!AdmobSettings.Instance.AdsRemoteConfigFetched) AdmobSettings.Instance.AdsRemoteConfigFetched = true;
        InitMediation();
    }

    protected override void OnDestroy() {
#if IAP
        if (IAP.Initialized) IAP.Instance.onInitialized -= RestoreRemoveAd;
#endif
        DestroyMediation();
        base.OnDestroy();
    }

    partial void FetchRemoteConfig();
    partial void InitMediation();
    partial void DestroyMediation();

    public bool IsAdShowing { get; private set; }

    private void OnShowAdStart() {
        IsAdShowing = true;
        LoadingMask.Instance.Show("ADS");
        Logs.Log($"[AdsManager]: OnShowAdStart placement={currentPlacement}");
    }

    private void OnShowAdFinished() {
        Excutor.Schedule(DelayAdFinishCallback, .15f);
    }

    private void DelayAdFinishCallback() {
        if (!IsAdShowing) return;
        IsAdShowing = false;
        LoadingMask.Instance.Hide();
        Logs.Log($"[AdsManager]: OnShowAdFinished placement={currentPlacement}");
    }

#if ADS
    private void OnApplicationPause(bool pause) {
        if (pause) {
            lastPauseTime = (new TimeSpan(DateTime.Now.Ticks)).TotalSeconds;
            if (IsAdShowing) PauseInsideApp = true;
            Logs.Log($"[Ads] Pause triggered lastPauseTime={lastPauseTime}, PauseInsideApp={PauseInsideApp} adsResumeEnable={AdmobSettings.Instance.AdResumeEnable}");
        }

        if (pause || IsAdShowing || !AdmobSettings.Instance.AdResumeEnable || (UserData.FirstInstall && UserData.PlayTimes < AdmobSettings.Instance.AdsResumeFromPlayedTime)) return;

        LoadingMask.Instance.Show();
        Excutor.Schedule(CheckShowAdResume, .2f);
    }

    private void CheckShowAdResume() {
        LoadingMask.Instance.Hide();
        Logs.Log($"[Ads] CheckShowAdResume triggered PauseInsideApp={PauseInsideApp}");

        if (PauseInsideApp) {
            PauseInsideApp = false;
            return;
        }

        var current = (new TimeSpan(DateTime.Now.Ticks)).TotalSeconds;
        Logs.Log($"[Ads] Resume triggered sleep={current - lastPauseTime}, capping={AdmobSettings.Instance.AdsResumeCapping}");
        if (current - lastPauseTime < AdmobSettings.Instance.AdsResumeCapping) return;

        if (AOAResumeEnable && HasAOA) {
            ShowAOA("AppResume");
        }
        else if (InterResumeEnable && HasInterstitial) {
            ShowInterstitial("AppResume", null, true);
        }
    }
#endif

}

namespace IPS.Api.Ads {
    public static class AdsExtension {

        public static bool IsTablet => PixelToDp(Screen.width) >= 720;

        public static int DpToPixel(this int dp) {
            return (int)(dp * GetScreenDensity(true));
        }

        public static int PixelToDp(this int pixel) {
            return (int)(pixel / GetScreenDensity(false));
        }

        public static Vector2 DpToPixel(int widthDp, int heightDp) {
            float density = GetScreenDensity(true);
            return new Vector2(widthDp * density, heightDp * density);
        }

        public static float GetScreenDensity(bool forceClamp) {
#if MAX
            float f = MaxSdkUtils.GetScreenDensity();// density=Screen.dpi/160f;
#else
            float f = Screen.dpi/160f;
#endif
            if (forceClamp) {
                if (f <= 2) f *= Mathf.Lerp(1.3f, 2, 2 - f);// 2.6f;
                else if (f >= 3) f *= .73f;// 2.2f;
                else f *= Mathf.Lerp(1.3f, .73f, f - 2);
            }
            return f;
        }
    }
}