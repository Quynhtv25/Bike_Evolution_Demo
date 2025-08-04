using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IPS {
    public enum ScreenName { None, Home, Ingame, Revive, Win, Lose, Shop, Settings }

    public abstract class HUD<K> : SingletonBehaviour<K> where K : MonoBehaviour {
        [SerializeField] bool keepLastFrame = true;
        [Tooltip("The master background also a button, this allow user click to background to hide popup")]
        [SerializeField] Button backgroundButton;
        [Tooltip("Change this size to 0 will reload all frames childs automaticaly")]
        [SerializeField] protected Frame[] frames;

        List<Frame> activings = new List<Frame>();

        public bool HasActiving => activings.Count > 0;
        public bool IsBusy { get; private set; }

        private float backgroundAlpha = -1;

        private void OnValidate() {
            BindAllFrame();
        }

        [ContextMenu("BindAllFrame")]
        protected void ForceBind() {
            frames = GetComponentsInChildren<Frame>();
            if (frames.Length == 0) { 
                Debug.LogError($"{name} No one frame was assign!");
            } 
        }

        private void BindAllFrame() {
            if (frames == null || frames.Length == 0) {
                ForceBind();
            }

            
        }

        private void Start() {
            if (backgroundButton) backgroundButton.onClick.AddListener(OnBack);
            OnStart();
        }

        protected abstract void OnStart();

        private void StartBusy() => IsBusy = true;
        private void EndBusy() => IsBusy = false;

        public void ShowPage<T>(bool fromLeft, Action callback = null) where T : Page {
            Show<T>(fromLeft, callback);
        }

        public void HidePage<T>(bool toLeft, Action callback = null) where T: Page {
            Hide<T>(toLeft, callback);
        }

        /// <summary>
        /// Show a frame and cache in the activing queue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="anim"></param>
        /// <param name="callback"></param>
        /// <param name="deactiveCurrent">disable current popup in the scene or not, current still in the activing queue for restoreable</param>
        /// <param name="overrideCurrent">remove current out of activing queue not not</param>
        public T Show<T>(bool anim = true, Action callback = null, bool deactiveCurrent = true, bool overrideCurrent = true) where T : Frame {
            var frame = Get<T>();
            if (frame == null) {
                callback?.Invoke();
                return frame;
            }

            if (activings.Contains(frame)) {
                if (frame.isActiveAndEnabled) {
                    callback?.Invoke();
                    return frame;
                }
                activings.Remove(frame);
            }

            if (deactiveCurrent && HasActiving) {
                activings[activings.Count - 1].Hide(anim);
            }

            if (overrideCurrent) HideCurrent(anim, restorePrevious: false);

            StartBusy();
            activings.Add(frame);
            frame.Show(anim, callback: callback);

            if (backgroundButton) {
                backgroundButton.gameObject.SetActive(true);
                if (anim) {
                    if (backgroundAlpha < 0) backgroundAlpha = backgroundButton.image.color.a;
                    if (backgroundAlpha > 0) {
                        backgroundButton.image.DOKill();
                        backgroundButton.image.DOFade(backgroundAlpha, frame.AnimDuration).From(0).SetUpdate(true).OnComplete(() => {
                            backgroundButton.interactable = frame.HideOnTapBg;
                            EndBusy();
                        });
                    }
                }
                else {
                    backgroundButton.interactable = frame.HideOnTapBg;
                    EndBusy();
                }
            }
            else EndBusy();
            return frame;
        }

        public void Hide<T>(bool anim = true, Action callback = null) where T: Frame {
            var frame = Get<T>();
            if (frame == null) {
                callback?.Invoke();
                return;
            }

            if (activings.Contains(frame)) {
                activings.Remove(frame);
            }

            if (!frame.gameObject.activeSelf) {
                callback?.Invoke();
                return;
            }

            callback += EndBusy;
            frame.Hide(anim, callback);

            // has no one previous to restore, so turn off black background
            if (activings.Count == 0 && backgroundButton && backgroundButton.gameObject.activeInHierarchy) {
                if (anim && backgroundAlpha > 0) {
                    backgroundButton.image.DOKill();
                    backgroundButton.image.DOFade(0, .1f).SetUpdate(true).OnComplete(() => {
                        backgroundButton.gameObject.SetActive(false);
                        EndBusy();
                    });
                }
                else {
                    backgroundButton.gameObject.SetActive(false);
                    EndBusy();
                }
            }
        }

        public void HideCurrent(bool anim = true, Action calback = null, bool restorePrevious = true) {
            if (!HasActiving) {
                Logs.Log("Nothing activing to Hide!");
                calback?.Invoke();
                return;
            }
            StartBusy();
            activings[activings.Count - 1].Hide(anim, calback);
            activings.RemoveAt(activings.Count - 1);

            if (restorePrevious && HasActiving) {
                activings[activings.Count - 1].Show(anim, EndBusy);
            }
            else {
                // has no one previous to restore, so turn off black background
                if (backgroundButton && backgroundButton.gameObject.activeInHierarchy) {
                    if (anim && backgroundAlpha > 0) {
                        backgroundButton.image.DOKill();
                        backgroundButton.image.DOFade(0, .1f).SetUpdate(true).OnComplete(() => {
                            backgroundButton.gameObject.SetActive(false);
                            EndBusy();
                        });
                    }
                    else {
                        backgroundButton.gameObject.SetActive(false);
                        EndBusy();
                    }
                }
                else EndBusy();
            }
        }

        public void HideAll(bool anim, Action hideAction = null) {
            activings.Clear();
            Array.ForEach(frames, (f) => f.Hide(anim));
            EndBusy();
            hideAction?.Invoke();
        }

        public T Get<T>() where T : Frame {
            var t = Array.Find(frames, (f) => f is T);
            if (t != null) return (T)t;

            t = FindFirstObjectByType<T>(FindObjectsInactive.Include);
            if (t != null) {
                var temp = new Frame[frames.Length + 1];
                Array.Copy(frames, temp, frames.Length);
                temp[temp.Length - 1] = t;
                frames = temp;
                return (T)t;
            }

            Logs.LogError($"{name} Type {typeof(T)} was not assign!");
            return null;
        }

        private void OnBack() {
            HideCurrent();
        }

        private void Update() {
            if (!IsBusy && Input.GetKeyUp(KeyCode.Escape)) {
                if (keepLastFrame && activings.Count == 1) return;
                OnBack();
            }
        }
    }

    
    /// <summary> Extend by Frame but transition is slide up from bottom to top </summary>
    public abstract class PageSlideUp : Frame {
        float screenSize = 0;

        private void FetchScreenSize() {
            if (screenSize == 0) {
                var scaler = GetComponentInParent<CanvasScaler>();
                if (scaler) screenSize = scaler.matchWidthOrHeight == 1 ? 1930 : Screen.height * (1080f / Screen.width) + 10;
                else screenSize = 1930;
            }
        }

        /// <summary> DO NOT USE THIS. Call via HUD.Instance.Show instead.</summary>
        public override void Show(bool anim = true, Action callback = null) {
            if (gameObject.activeSelf) {
                if (callback != null) callback.Invoke();
                return;
            }

            gameObject.SetActive(true);
            FetchScreenSize();

            if (!anim) {
                (transform as RectTransform).anchoredPosition = Vector2.zero;
                if (callback != null) callback.Invoke();
                return;
            }

            transform.DOKill();
            (transform as RectTransform).DOAnchorPosY(0, animTime * 2).From(new Vector2(0, -screenSize))
                .SetUpdate(true).SetEase(showType).OnComplete(() => {
                    if (callback != null) callback.Invoke();
                });

        }

        /// <summary> DO NOT USE THIS. Call via HUD.Instance.Show instead.</summary>
        public override void Hide(bool anim = true, Action callback = null) {
            if (!gameObject.activeSelf) {
                if (callback != null) callback.Invoke();
                return;
            }

            if (!anim) {
                gameObject.SetActive(false);
                if (callback != null) callback.Invoke();
                return;
            }

            FetchScreenSize();

            transform.DOKill();
            (transform as RectTransform).DOAnchorPosY(-screenSize, animTime * 2)
                .SetUpdate(true).OnComplete(() => {
                    if (callback != null) callback.Invoke();
                    gameObject.SetActive(false);
                });
        }
    }

    /// <summary> Extend by Frame but transition is swipe from left to right or right to left </summary>
    public abstract class Page : Frame {
        float screenSize = 0;

        private void FetchScreenSize() {
            if (screenSize == 0) {
                var scaler = GetComponentInParent<CanvasScaler>();
                if (scaler) screenSize = scaler.matchWidthOrHeight == 0 ? 1090 : Screen.width * (1920f / Screen.height) + 10;
                else screenSize = 1090;
            }
        }

        /// <summary> DO NOT USE THIS. Call via HUD.Instance.Show instead.</summary>
        public override void Show(bool fromLeft = true, Action callback = null) {
            if (gameObject.activeSelf) {
                if (callback != null) callback.Invoke();
                return;
            }

            gameObject.SetActive(true);
            FetchScreenSize();

            if (Canvas != null) {
                if (bgAlpha < 0) bgAlpha = Canvas.alpha;
                Canvas.DOKill();
                Canvas.DOFade(bgAlpha, animTime).From(0).SetUpdate(true);
            }

            MainFrame.DOKill();
            (MainFrame as RectTransform).DOAnchorPosX(0, animTime).From(new Vector2(fromLeft ? -screenSize : screenSize, 0))
                .SetUpdate(true).SetEase(showType).OnComplete(() => {
                    if (callback != null) callback.Invoke();
                });
        }

        /// <summary> DO NOT USE THIS. Call via HUD.Instance.Show instead.</summary>
        public override void Hide(bool toLeft = true, Action callback = null) {
            if (!gameObject.activeSelf) {
                if (callback != null) callback.Invoke();
                return;
            }

            if (Canvas) {
                if (bgAlpha < 0) bgAlpha = Canvas.alpha;
                Canvas.DOKill();
                Canvas.DOFade(0, animTime).SetUpdate(true);
            }

            FetchScreenSize();

            MainFrame.DOKill();
            (MainFrame as RectTransform).DOAnchorPosX(toLeft ? -screenSize : screenSize, animTime)
                .SetUpdate(true).SetEase(showType).OnComplete(() => {
                    if (callback != null) callback.Invoke();
                    gameObject.SetActive(false);
                });
        }
    }

    /// <summary>
    /// Base class for popup type, when show a popup, do not hide other page
    /// </summary>
    public abstract class Popup : Frame {

    }

    [RequireComponent(typeof(CanvasGroup))]
    public class Frame : UIComponent, IFrame {
        [Tooltip("If turn on this, the main HUD need to have a background image with turn on RaycastTarget, and the `bg` bellow need to turn off RaycastTarget")]
        [SerializeField] private bool hideOnTapBg = false;
        [Tooltip("If turn on hideOnTapBg, this bg need to turn off RaycastTarget")]
        [SerializeField] protected Image bg;
        [Tooltip("Mainframe will be use for scale in/out")]
        [SerializeField] protected Transform mainFrame;
        [SerializeField] protected float animTime = 0.3f;
        
        [Header("Show")]
        [SerializeField] protected Ease showType = Ease.OutBack;
        [SerializeField] protected float showFrom = 0.5f;
        [SerializeField] protected float showTo = 1f;

        [Header("Show")]
        [SerializeField] protected Ease hideType = Ease.OutBack;
        [SerializeField] protected float hideTo = 1f;
            
        public event Action onShowTrigger, onHideTrigger;

        protected float bgAlpha = -1;
        private CanvasGroup canvas;
        protected Transform MainFrame {
            get {
                if (mainFrame != null) return mainFrame;
                mainFrame = transform.childCount == 1 ? transform.GetChild(0) : transform;
                return mainFrame;
            }
        }

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

        public float AnimDuration => animTime;
        public bool HideOnTapBg => hideOnTapBg;

        /// <summary> DO NOT USE THIS. Call via HUD.Instance.Show instead.</summary>
        public virtual void Show(bool animate = true, Action callback = null) {
            if (gameObject.activeSelf) {
                if (onShowTrigger != null) onShowTrigger.Invoke();
                if (callback != null) callback.Invoke();
                return;
            }

            gameObject.SetActive(true);      
            if (onShowTrigger != null) onShowTrigger.Invoke();

            if (animate) {
                if (Canvas) {
                    if (bgAlpha < 0) bgAlpha = Canvas.alpha;
                    Canvas.DOKill();
                    Canvas.DOFade(bgAlpha, animTime).From(0).SetUpdate(true);
                }
                MainFrame.DOKill();
                MainFrame.DOScale(showTo, animTime).From(showFrom).SetUpdate(true).SetEase(showType).OnComplete(() => {
                    if (callback != null) callback.Invoke();
                });
            }
            else {
                 if (Canvas) {
                    if (bgAlpha < 0) bgAlpha = Canvas.alpha;
                    Canvas.DOFade(bgAlpha, 0).From(bgAlpha).SetUpdate(true);
                }
                MainFrame.localScale = Vector3.one;
                if (callback != null) callback.Invoke();
            }
        }

        /// <summary> DO NOT USE THIS. Call via HUD.Instance.Show instead.</summary>
        public virtual void Hide(bool animate = true, Action callback = null) {
            if (onHideTrigger != null) onHideTrigger.Invoke();

            if (!gameObject.activeSelf) {
                if (callback != null) callback.Invoke();
                return;
            }

            if (animate) {
                if (Canvas) {
                    if (bgAlpha < 0) bgAlpha = Canvas.alpha;
                    Canvas.DOKill();
                    Canvas.DOFade(0, animTime).SetUpdate(true);
                }
                MainFrame.DOKill();
                MainFrame.DOScale(hideTo,animTime).SetEase(hideType).SetUpdate(true).OnComplete(() => {
                    gameObject.SetActive(false);
                    if (callback != null) callback.Invoke();
                });
            }
            else {
                gameObject.SetActive(false);
                if (callback != null) callback.Invoke();
            }
        }

        protected virtual void OnDestroy() {
            if (Canvas) Canvas.DOKill();
            if (MainFrame) MainFrame.DOKill();
        }
    }

    public interface IFrame {
        void Show(bool animate = true, Action callback = null);
        void Hide(bool animate = true, Action callback = null);
    }
}