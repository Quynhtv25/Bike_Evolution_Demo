using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;

namespace IPS {
    [RequireComponent(typeof(UnityEngine.UI.Button))]
    public class ButtonEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler {
        [SerializeField] private bool playSound = true;
        [SerializeField] private bool scaleEffect = true;
        [SerializeField] private float scaleValue = 1.1f;
        [SerializeField] private bool isAttention = false;
        [SerializeField] private bool autoAssignOnClick = true;

        [Header("Select Animation")]
        [SerializeField] private float selectAnimTime = .1f;
        [SerializeField] private RectTransform selecting;
        [SerializeField] private RectTransform unselect;

        Vector3 originScale = Vector3.one;
        bool originPosCached;
        Vector2 originPosSelecting, originPosUnSelect;
        Button btn;
        BageIcon badge;

        event UnityAction onClick;
        private bool CanTouch => Button != null && Button.interactable;

        protected Button Button {
            get {
                if (btn != null) return btn;
                btn = GetComponent<Button>();
                return btn;
            }
        }
        
        public void AddListener(UnityAction callback) => onClick += callback;
        public void RemoveListener(UnityAction callback) => onClick -= callback;
        public void RemoveAllListener() {
            Button.onClick.RemoveAllListeners();
            onClick = null;
        }

        public bool Interactable {
            get => Button != null && Button.interactable;
            set {
                if (this.Button != null) Button.interactable = value;
            }
        }

        void Start() {
            originScale = transform.localScale;
            badge = GetComponentInChildren<BageIcon>();
            if (isAttention) RunAttention();
            if (autoAssignOnClick) Button.onClick.AddListener(OnClickButton);
            OnStart();
        }

        void OnDestroy() {
            transform.DOKill();
        }

        public void SetOriginScale(Vector3 scale) {
            originScale = scale;
        }

        protected virtual void OnStart() {}

        protected virtual void OnClickButton() {}

        public void OnPointerDown(PointerEventData eventData) {
            if (!CanTouch) return;
            if (scaleEffect) {
                transform.DOKill();
                transform.DOScale(originScale * scaleValue, 0.2f).SetUpdate(true);
            }
        }

        public void OnPointerUp(PointerEventData eventData) {
            if (!CanTouch) return;
            if (scaleEffect) {
                transform.DOKill();
                transform.DOScale(originScale, 0.1f).SetUpdate(true).OnComplete(() => {
                   if (isAttention) RunAttention(); 
                });
            }
        }

        public void OnPointerClick(PointerEventData eventData) {
            if (!CanTouch) return;
            if (playSound) {
                SFX.Instance.PlaySound(SoundEvent.ButtonClicked);
                SFX.Instance.VibrateHaptic();
            }

            if (onClick != null) onClick.Invoke();
            if (badge) badge.Hide();
        }

        public void RunAttention() {
            if (!Button.interactable) return;
            isAttention = true;
            transform.DOKill();
            transform.DOScale(originScale * .97f, 0.5f).SetUpdate(true).OnComplete(() => {
                transform.DOScale(originScale * 1.03f, 0.5f).SetUpdate(true).OnComplete(() => {
                    RunAttention();
                });
            });
        }

        public void StopAttention() {
            transform.DOKill();
            transform.localScale = originScale;
        }

        public void SetSelecting(bool selecting, bool scaleEffect = false) {
            if (!scaleEffect) {
                if (selecting) {
                    CacheOriginPos();
                    unselect.DOKill();
                    unselect.DOAnchorPosY(originPosSelecting.y, selectAnimTime).OnComplete(() => {
                        this.selecting.gameObject.SetActive(true);
                        this.unselect.gameObject.SetActive(false);
                    });
                }
                else {
                    CacheOriginPos();
                    unselect.DOKill();
                    this.selecting.gameObject.SetActive(false);
                    unselect.gameObject.SetActive(true);
                    unselect.DOAnchorPosY(originPosUnSelect.y, selectAnimTime).OnComplete(() => { });
                }
            }
            else {
                if (selecting) {
                    CacheOriginPos();
                    unselect.DOKill();
                    unselect.DOAnchorPosY(originPosSelecting.y, selectAnimTime).OnComplete(() => {
                        this.selecting.gameObject.SetActive(true);
                        this.unselect.gameObject.SetActive(false);
                    });
                    unselect.localScale = Vector3.one * 1.58f;
                }
                else {
                    CacheOriginPos();
                    unselect.DOKill();
                    this.selecting.gameObject.SetActive(false);
                    unselect.gameObject.SetActive(true);
                    unselect.DOAnchorPosY(originPosUnSelect.y, selectAnimTime).OnComplete(() => { });
                    unselect.localScale = Vector3.one;
                }
            }
        }

        private void CacheOriginPos() {
            if (originPosCached) return;
            originPosSelecting = selecting.anchoredPosition;
            originPosUnSelect = unselect.anchoredPosition;
            originPosCached = true;
        }

        public void SetText(string text) {
            var tmp = GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (tmp != null) tmp.SetText(text);
        }

        public void SetImage(Sprite sprite) {
            if (Button.image) Button.image.sprite = sprite;
        }

        public void SetImageColor(Color color) {
            if (Button.image) Button.image.color = color;
        }

        public void SetOverrideSprite(Sprite sprite) {
            if (Button.image) Button.image.overrideSprite = sprite;
        }
    }
}