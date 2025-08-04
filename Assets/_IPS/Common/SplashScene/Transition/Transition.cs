using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
#if ADS
using IPS.Api.Ads;
#endif
#if SPINE
using Spine.Unity;
#endif

namespace IPS {
    [RequireComponent(typeof(CanvasGroup))]
    public class Transition : MonoBehaviour {
        [SerializeField] private GameObject mainFrame;
        [SerializeField] protected Image logo;
        [SerializeField] LoadingProgressBar loadingProgressBar;
#if SPINE
        [SerializeField] private SkeletonGraphic skeletonItem;
#endif

        private static Transition instance;
        public static Transition Instance {
            get {
                if (instance != null) return instance;
                instance = FindObjectOfType<Transition>();
                if (instance == null) {
                    var obj = Resources.Load<GameObject>($"{typeof(Transition).Name}/{typeof(Transition).Name}");
                    if (obj == null) Debug.LogError($"Prefab not found at: Resources/{typeof(Transition).Name}/{typeof(Transition).Name}");
                    else instance = Instantiate(obj.gameObject).GetComponent<Transition>();
                }
                return instance;
            }
        }

        private CanvasGroup canvas;
        protected CanvasGroup Canvas {
            get {
                if (canvas != null) return canvas;
                canvas = GetComponent<CanvasGroup>();
                if (canvas == null) {
                    canvas = gameObject.AddComponent<CanvasGroup>();
                    canvas.alpha = 1;
                }
                return canvas;
            }
        }

        public bool IsShowing => mainFrame.gameObject.activeSelf;

        private enum FadeState { None, FadeIn, FadeOut }
        private FadeState fadeState;

        private void Awake() {
            if (instance != null && instance.GetInstanceID() != this.GetInstanceID()) {
                DestroyImmediate(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void FadeIn(Action onFinished = null, Action onStepUpdate= null, float from = 0, float to = 1, float duration = 0.3f) {
            if (mainFrame.gameObject.activeSelf || fadeState == FadeState.FadeIn) {
                onStepUpdate?.Invoke();
                onFinished?.Invoke();
                return;
            }

            Logs.Log("[Transition] Fadein");
            fadeState = FadeState.FadeIn;
            mainFrame.SetActive(true);

            Canvas.DOKill();
            var tween = Canvas.DOFade(to, duration).From(from).SetUpdate(true).OnComplete(() => {
                fadeState = FadeState.None;
                onFinished?.Invoke();
                onFinished = null;
            });

            if (onStepUpdate != null) tween.OnStepComplete(() => { onStepUpdate?.Invoke(); });
        }

        public void FadeOut(Action onFinished = null, Action onStepUpdate = null, float from = 1, float to = 0, float duration = 0.3f) {
            if (!mainFrame.gameObject.activeSelf || fadeState == FadeState.FadeOut) {
                onStepUpdate?.Invoke();
                onFinished?.Invoke();
                return;
            }

            Logs.Log("[Transition] Fadeout");
            fadeState = FadeState.FadeOut;
            Canvas.DOKill();
            var tween = Canvas.DOFade(to, duration).From(from == 1 ? Canvas.alpha : from).SetUpdate(true).OnComplete(() => {
                fadeState = FadeState.None;
                Hide();
                onFinished?.Invoke();
                onFinished = null;
            });

            if (onStepUpdate != null) tween.OnStepComplete(() => { onStepUpdate?.Invoke(); });
        }

        public void ShowProgress(float progress, bool showText = true) {
            loadingProgressBar.SetProgress(progress, showText);
        }

        public void ShowLogo(bool show) {
            if (logo) logo.gameObject.SetActive(show && logo.sprite != null);
        }

        public void SetLogoImage(Sprite sprite) {
            if (!logo) return;
            if (sprite == null) logo.gameObject.SetActive(false);
            else {
                logo.gameObject.SetActive(true);
                logo.sprite = sprite;
            }
        }

        public void HideProgress() {
            loadingProgressBar.Hide();
        }

        private void Hide() {
            mainFrame.SetActive(false);
            HideProgress();
        }

        public void LoadCurrentScene(bool fadeOut, bool showProgress = false, bool showLogo = false, Action onComplete = null, bool recycleAllPool = true, bool showBanner = true) {
            LoadScene(SceneManager.GetActiveScene().buildIndex, fadeOut, showProgress, showLogo, onComplete, recycleAllPool, showBanner);
        }

        public void LoadScene(int buildIndex, bool fadeOutOnComplete, bool showProgress = true, bool showLogo = true, Action onComplete = null, bool recycleAllPool = true, bool showBanner = true, bool showRate = true) {
#if FCM
            if (!IPS.Api.Notifications.PushNotifications.Initialized && buildIndex == 1 && UserData.PlayTimes > 0) {
                onComplete += () => Excutor.Schedule(IPS.Api.Notifications.PushNotifications.Instance.Preload, 1);
            }
#endif

            if (showProgress) ShowProgress(0);
            ShowLogo(showLogo);

            FadeIn(() => {
                StartCoroutine(IELoadSceneAsync(SceneManager.LoadSceneAsync(buildIndex), showProgress, fadeOutOnComplete, onComplete, recycleAllPool, showBanner, showRate));
            });
        }

        public void LoadScene(string sceneName, bool fadeOutOnComplete, bool showProgress = true, bool showLogo = true, Action onComplete = null, bool recycleAllPool = true, bool showBanner = true, bool showRate = true) {
            if (showProgress) ShowProgress(0);
            ShowLogo(showLogo);
            
            FadeIn(() => {
                StartCoroutine(IELoadSceneAsync(SceneManager.LoadSceneAsync(sceneName), showProgress, fadeOutOnComplete, onComplete, recycleAllPool, showBanner, showRate));
            });
        }

        private IEnumerator IELoadSceneAsync(AsyncOperation async, bool showProgress, bool fadeOut, Action onNewSceneLoaded, bool recycleAllPool = true, bool showBanner = true, bool showRate = true) {
            if (recycleAllPool) PoolManager.Instance.RecycleAll();

#if ADS
            if (AdsManager.Instance.CanReloadBannerByLevel) {
                AdsManager.Instance.DestroyBanner();
                if (showBanner) {
                    onNewSceneLoaded += () => AdsManager.Instance.ShowBanner(string.Empty);
                }
            }
            else {
                AdsManager.Instance.HideBanner();
                if (showBanner) {
                    onNewSceneLoaded += () => AdsManager.Instance.ShowBanner(string.Empty);
                }
            }

            AdsManager.Instance.HideMRec();
#endif

            while(!async.isDone) {
                if (showProgress) ShowProgress(async.progress);
                yield return null;
            }
            
            GC.Collect();
            Resources.UnloadUnusedAssets();
            if (onNewSceneLoaded != null) onNewSceneLoaded.Invoke();

            if (fadeOut) FadeOut(() => {
#if FIREBASE
                if (showRate && Tracking.Instance.IsWinLastLevel) {
#if ADS
                    RatePopup.Instance.ShowIfAvailable();
#endif
                }
#endif
            });
        }
    }
}