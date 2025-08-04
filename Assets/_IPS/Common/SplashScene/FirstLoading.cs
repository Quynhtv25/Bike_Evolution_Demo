using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if ADS
using IPS.Api.Ads;
#endif

namespace IPS {
    public class FirstLoading : MonoBehaviour {
        [SerializeField] FirstSceneCondition firstSceneCondition;
        [SerializeField] private bool hideLogoTransitionWhenDone = false;
        [SerializeField] private bool autoFadeOut = true;
        [SerializeField] private bool showBannerAfterAOA = true;
        [SerializeField] private LoadingProgressBar loadingProgress;
        [SerializeField][Tooltip("The object will be active after logo completed")] 
        private GameObject[] delayActiveObjects;
        [Header("Loading time total = logoDuration + timeout")]
        [Tooltip("You can set this to 0 if don't want to spend 2 first seconds for logo present")]
        [SerializeField] float logoDuration = 2;
        [Tooltip("Time out to waiting for AOA, will be start countdown after logo present")] 
        [SerializeField] float timeout = 6f;

        AsyncOperation asyncLoad;
        System.Diagnostics.Stopwatch timer;

        private void Awake() {
            timer = new System.Diagnostics.Stopwatch();
            timer.Start();

            if (timeout > 0) Invoke(nameof(Timeout), timeout + logoDuration);
            DontDestroyOnLoad(gameObject);
        }

        private void Timeout() {
            timeout = 0;
        }

        private void Start() {
            Tracking.Instance.LogLoadingStart();
            StartCoroutine(LoadYourAsyncScene());
        }


        IEnumerator LoadYourAsyncScene() {
            asyncLoad = SceneManager.LoadSceneAsync(firstSceneCondition.NextSceneIdx);
            asyncLoad.allowSceneActivation = false;

            if (logoDuration > 0) yield return new WaitForSeconds(logoDuration); // Time to load splash logo & some remote config

            if (!loadingProgress) {
                Transition.Instance.ShowProgress(0);
                Transition.Instance.FadeIn(null);
            }

            foreach (var obj in delayActiveObjects) {
                obj?.gameObject.SetActive(true);
            }

            float lastAmount = loadingProgress != null ? loadingProgress.CurrentProgress : 0;
            float v = 1f / (timeout - 1);

            bool realLoadingFinished = false;

            GameData.Instance.Preload();

            while (!asyncLoad.isDone) {
                while (asyncLoad.progress < 0.9f || (timeout > 0 && WaitingForAOA)) {// || Time.time < 3) {
                    lastAmount += v * Time.deltaTime;

                    if (loadingProgress) {
                        loadingProgress.SetProgress(Mathf.Clamp(lastAmount, 0, .9f), true);                        
                    }
                    else {
                        Transition.Instance.ShowProgress(Mathf.Clamp(lastAmount, 0, .9f));
                    }

                    if (asyncLoad.progress >= 0.9f && !realLoadingFinished) {
                        realLoadingFinished = true;
                        Tracking.Instance.LogLoadingAsyncEnd(timer.ElapsedMilliseconds / 1000f, (int)(lastAmount * 100));
                    }

                    yield return null;
                }

                if (loadingProgress) {
                    loadingProgress.DoFill(1, .1f);
                    yield return Yielder.Wait(.1f);
                }
                else {
                    v = (1 - lastAmount)/0.1f;
                    while(lastAmount < 1) {
                        lastAmount += v * Time.deltaTime;
                        Transition.Instance.ShowProgress(lastAmount);
                        yield return null;
                    }
                }

                Logs.Log($"Loading end: waitingAOA={WaitingForAOA}, totalLoadingTime = {Time.time}");
                //if (!Transition.Instance.IsShowing) {
                //    Logs.Log("End with fake Transition mask with full progress");
                //    loadingProgress.Hide();
                //    Transition.Instance.ShowProgress(1);
                //    Transition.Instance.FadeIn(duration: 0);
                //}

                timer.Stop();
                Tracking.Instance.LogLoadingEnd(timer.ElapsedMilliseconds/1000f);
                Excutor.Schedule(IEShowAOA());

                Tracking.Instance.StartTimer();
                SFX.Instance.Preload();
                asyncLoad.allowSceneActivation = true;
                yield return null;
            }
            asyncLoad = null;
        }

        private IEnumerator IEShowAOA() {
            AOAOpenTrigger.onFadeoutCallback = () => {
                if (hideLogoTransitionWhenDone) Transition.Instance.ShowLogo(false);
                Transition.Instance.ShowProgress(1);
                Transition.Instance.FadeIn(duration: 0);
                if (gameObject) DestroyImmediate(gameObject);
            };

            yield return new WaitUntil(() => SceneManager.GetActiveScene().buildIndex > 0);
            yield return null;
            try {
                firstSceneCondition.FirstSceneEnterAction?.Invoke();
            }
            catch (System.Exception e) {
                Logs.LogError($"FirstSceneEnterAction error: {(e != null ? e.Message : "")}");
            }

            Tracking.Instance.StopTimer();
            Tracking.Instance.LogEnterHomeScene(Tracking.Instance.TimerSeconds);

            var aoa = FindObjectOfType<AOAOpenTrigger>();
            if (aoa == null) {
                aoa = new GameObject(typeof(AOAOpenTrigger).Name).AddComponent<AOAOpenTrigger>();
                aoa.autoFadeOut = autoFadeOut;
                if (!showBannerAfterAOA) {
                    aoa.showBanner = AOAOpenTrigger.ShowBannerType.NotShow;
                }
                Logs.Log("<color=magenta>Force add AOAOpenTrigger</color>");
            }
        }

        private bool WaitingForAOA {
            get {
#if ADS
                return !AdsManager.Instance.HasAOA && 
                        ((UserData.FirstInstall && AdsManager.Instance.AoaOpenFirstInstallEnable) 
                        || (!UserData.FirstInstall && AdsManager.Instance.AoaOpenEnable));
#endif
                return false;
            }
        }
    }
}