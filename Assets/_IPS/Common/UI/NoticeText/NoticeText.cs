using DG.Tweening;
using TMPro;
using UnityEngine;

public class NoticeText : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI noticeText;
    [SerializeField] private CanvasGroup canvas;
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

    private static NoticeText _ins;
    public static NoticeText Instance {
        get {
            if (_ins != null) return _ins;
            var g = Resources.Load<GameObject>($"{typeof(NoticeText).Name}/{typeof(NoticeText).Name}");
            if (g == null) {
                Debug.LogError($"File not exist in path: Resources/{typeof(NoticeText).Name}/{typeof(NoticeText).Name}");
            }
            else _ins = GameObject.Instantiate(g).GetComponent<NoticeText>();

            return _ins;
        }
    }

    private void Awake() {
        if (_ins != null && _ins.gameObject.GetInstanceID() != gameObject.GetInstanceID()) {
            DestroyImmediate(gameObject);
            return;
        }

        _ins = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy() {
        if (canvas) Canvas.DOKill();
        if (noticeText && noticeText.transform.parent) noticeText.transform.parent.DOKill();
    }

    public void ShowNotice(string message, bool errorMessage = false) {
        if (noticeText == null) {
            var g = new GameObject("Text");
            g.transform.SetParent(transform);
            noticeText = g.AddComponent<TextMeshProUGUI>();
            noticeText.fontSize = 40;
            noticeText.raycastTarget = false;
            //return;
        }

        noticeText.text = message;
        noticeText.color = Color.white;
        noticeText.outlineColor = errorMessage ? Color.red : Color.black;

        Canvas.gameObject.SetActive(true);
        Canvas.DOKill();
        Canvas.DOFade(1, .1f).SetUpdate(true);
        Canvas.DOFade(0, .3f).SetDelay(0.7f).SetUpdate(true).OnComplete(() => {
            if (Canvas && Canvas.gameObject) Canvas.gameObject.SetActive(false);
        });

        (noticeText.transform.parent as RectTransform).anchoredPosition = new Vector2(0, 0);
        (noticeText.transform.parent as RectTransform).DOAnchorPosY(100, 0.4f).SetEase(Ease.OutBack).SetUpdate(true);
    }

}